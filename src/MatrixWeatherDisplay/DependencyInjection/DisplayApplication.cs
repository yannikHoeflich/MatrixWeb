using System.Xml.Serialization;

using MatrixWeatherDisplay.Data;
using MatrixWeatherDisplay.DependencyInjection.ScreenGeneratorCollections;
using MatrixWeatherDisplay.Screens;
using MatrixWeatherDisplay.Services;
using MatrixWeatherDisplay.Services.IconLoader;
using MatrixWeb.Extensions;
using MatrixWeb.Extensions.Data;
using MatrixWeb.Extensions.Data.Config;
using MatrixWeb.Extensions.Logging;
using MatrixWeb.Extensions.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MatrixWeatherDisplay.DependencyInjection;
public partial class DisplayApplication {
    public IServiceProvider Services { get; }
    public IScreenGeneratorProvider ScreenGenerators { get; }

    public RedSettings? RedManager { get; private set; }

    internal ServiceDescriptor[]? Initializables { get; init; }
    internal ServiceDescriptor[]? AsyncInitializables { get; init; }

    private ILogger _logger;

    private bool _running = false;
    private bool _shouldRun = false;

    private readonly Wrapped<TicksTime> _waitUntil = new(TicksTime.Zero);

    internal DisplayApplication(IServiceProvider services, IScreenGeneratorProvider screenGeneratorProvider) {
        Services = services;
        ScreenGenerators = screenGeneratorProvider;

    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>true if the code flow should be continued, false if a critical error accured and the program should not be started.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<bool> InitDefaultServicesAsync() {
        ConfigService configService = Services.GetService<ConfigService>() ?? throw new InvalidOperationException("The service 'ConfigService' has to be registered");

        IEnumerable<IInitializable> initializables = Initializables.Select(x => x.GetService(Services)).OfType<IInitializable>().ToArray();
        IEnumerable<IAsyncInitializable> asyncInitializables = AsyncInitializables.Select(x => x.GetService(Services)).OfType<IAsyncInitializable>().ToArray();

        var configLayouts = initializables.Select(x => x.ConfigLayout).Concat(
                            asyncInitializables.Select(x => x.ConfigLayout))
                            .Where(x => x != ConfigLayout.Empty && x.Keys.Length > 0)
                            .DistinctBy(x => x.Keys)
                            .ToList();

        await configService.InitAsync(configLayouts);

        ILogger<DisplayApplication>? newLogger = Services.GetService<ILogger<DisplayApplication>>();
        if (newLogger is not null) {
            _logger = newLogger;
        }

        BrightnessService autoBrightnessService = Services.GetService<BrightnessService>() ?? throw new InvalidOperationException("The Service of type 'AutoBrightnessService' must be registered");
        RedManager = new RedSettings(autoBrightnessService);

        if (Initializables is not null) {
            foreach (IInitializable service in initializables) {
                InitResult result = service.Init();
                if(HandleInitResult(service,  result)) {
                    return false;
                }
            }
        }

        if (AsyncInitializables is not null) {
            foreach (IAsyncInitializable service in asyncInitializables) {
                InitResult result = await service.InitAsync();
                if (HandleInitResult(service, result)) {
                    return false;
                }
            }
        }

        await configService.SaveAsync();
        return true;
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
            if(await LogIfSkippedLastAsync(skips, screenGenerator)) {
                skips = 0;
            }

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
