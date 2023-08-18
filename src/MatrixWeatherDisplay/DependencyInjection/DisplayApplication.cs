using System.Xml.Serialization;

using MatrixWeatherDisplay.Data;
using MatrixWeatherDisplay.Data.Extensions;
using MatrixWeatherDisplay.DependencyInjection.ScreenGeneratorCollections;
using MatrixWeatherDisplay.Logging;
using MatrixWeatherDisplay.Screens;
using MatrixWeatherDisplay.Services;
using MatrixWeatherDisplay.Services.IconLoader;
using MatrixWeatherDisplay.Services.SensorServices;
using MatrixWeatherDisplay.Services.Weather;
using MatrixWeb.Extensions;
using MatrixWeb.Extensions.Data;
using MatrixWeb.Extensions.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MatrixWeatherDisplay.DependencyInjection;
public partial class DisplayApplication {
    public IServiceProvider Services { get; }
    public IScreenGeneratorProvider ScreenGenerators { get; }

    public RedSettings? RedManager { get; private set; }

    internal Type[]? Initializables { get; init; }
    internal Type[]? AsyncInitializables { get; init; }

    private readonly ILogger _logger = Logger.Create<DisplayApplication>();

    private bool _running = false;
    private bool _shouldRun = false;

    private readonly Wrapped<TicksTime> _waitUntil = TicksTime.Zero;

    internal DisplayApplication(IServiceProvider services, IScreenGeneratorProvider screenGeneratorProvider) {
        Services = services;
        ScreenGenerators = screenGeneratorProvider;
    }

    public async Task InitDefaultServicesAsync() {
        ConfigService configService = Services.GetService<ConfigService>() ?? throw new InvalidOperationException("The service 'ConfigService' has to be registered");
        await configService.InitAsync();

        BrightnessService autoBrightnessService = Services.GetService<BrightnessService>() ?? throw new InvalidOperationException("The Service of type 'AutoBrightnessService' must be registered");
        RedManager = new RedSettings(autoBrightnessService);

        var serviceInitializer = new ServiceInitializer(Services);

        if (Initializables is not null) {
            foreach (Type serviceType in Initializables) {
                serviceInitializer.Init(serviceType);
            }
        }

        if (AsyncInitializables is not null) {
            foreach (Type serviceType in AsyncInitializables.Where(x => x != typeof(ConfigService))) {
                await serviceInitializer.InitAsync(serviceType);
            }
        }

        SensorService? sensorService = Services.GetService<SensorService>();
        if (sensorService is not null)
            _ = sensorService.ScanAsync();

        await configService.SaveAsync();
    }

    public async Task RunAsync() {
        if (_running) {
            _logger.LogInformation("Already running");
            return;
        }

        _logger.LogInformation("Starting up");

        _running = true;
        _shouldRun = true;

        DeviceService deviceService = Services.GetService<DeviceService>() ?? throw new InvalidOperationException("The Service of type 'DeviceService' must be registered");
        InternetService? internetService = Services.GetService<InternetService>();


        await deviceService.ScanAsync();

        ScreenGenerators.Reset();

        while (_shouldRun) {
            await ShowNextScreenAndWaitAsync(deviceService, internetService);
        }

        _running = false;
    }

    private async Task ShowNextScreenAndWaitAsync(DeviceService deviceService, InternetService? internetService) {
        ByteScreen? screen = await GetNextScreenAsync(deviceService, internetService);

        if (screen is null) {
            return;
        }

        await SendScreenAsync(deviceService, screen);
        await Extensions.SleepUntilAsync(_waitUntil, () => !_shouldRun);
    }

    private async Task SendScreenAsync(DeviceService deviceService, ByteScreen screen) {
        _logger.LogTrace("sending gif");
        await deviceService.SendGifAsync(screen.Image);
        _logger.LogTrace("waiting {screen time}ms", screen.ScreenTime.TotalMilliseconds);
        SetWaitUntil(screen.ScreenTime);
    }

    private async Task<ByteScreen?> GetNextScreenAsync(DeviceService deviceService, InternetService? internetService) {
        int skips = 0;
        IScreenGenerator? screenGenerator = null;
        while (_shouldRun) {
            await LogIfSkippedLastAsync(skips, screenGenerator);

            screenGenerator = ScreenGenerators.GetNextScreenGenerator();
            _logger.LogTrace("Trying to show '{screen}'", screenGenerator?.Name);

            if (await ShouldScreenGeneratorSkipAsync(internetService, screenGenerator)) {
                skips++;
                continue;
            }

            ByteScreen? screen;

            bool turnRed = RedManager is not null && RedManager.ShouldBeRed(deviceService.Brightness);
            try {
                Screen rawScreen = await screenGenerator.GenerateImageAsync();
                screen = await ByteScreen.FromScreenAsync(rawScreen, turnRed);
            } catch (Exception ex) {
                _logger.LogError("Error creating next screen {ex}", ex); 
                screen = null;
            }

            if (screen is null || screen.ScreenTime <= TimeSpan.Zero) {
                skips++;
                continue;
            }

            return screen;
        }

        return null;
    }

    public async Task StopAsync() {
        _logger.LogInformation($"Stopping");
        _shouldRun = false;

        await Extensions.SleepAsync(TimeSpan.FromSeconds(1), () => !_running);

        if (_running) {
            _logger.LogError($"Didn't stop after 1 Seconds.");
            _logger.LogError($"Probably stuck in a task or already stopped because of an error!");
            _logger.LogError($"Pretending to be stopped!");
            _running = false;
        }
    }

    public async Task InjectAsync(Screen screen) {
        ByteScreen byteScreen = await ByteScreen.FromScreenAsync(screen, false);

        DeviceService? deviceService = Services.GetService<DeviceService>();

        if (deviceService is null) {
            return;
        }

        await SendScreenAsync(deviceService, byteScreen);
    }

    private void SetWaitUntil(TimeSpan timeSpan) => _waitUntil.Value = TicksTime.Now + TicksTimeSpan.FromTimeSpan(timeSpan);
    public void StopInjection() {
        _waitUntil.Value = TicksTime.Now;
    }
}
