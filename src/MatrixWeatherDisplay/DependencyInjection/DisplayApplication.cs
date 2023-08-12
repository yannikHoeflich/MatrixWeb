using System.Xml.Serialization;

using MatrixWeatherDisplay.Data;
using MatrixWeatherDisplay.Data.Extensions;
using MatrixWeatherDisplay.DependencyInjection.ScreenGeneratorCollections;
using MatrixWeatherDisplay.Logging;
using MatrixWeatherDisplay.Screens;
using MatrixWeatherDisplay.Services;
using MatrixWeatherDisplay.Services.Weather;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MatrixWeatherDisplay.DependencyInjection;
public partial class DisplayApplication {
    public IServiceProvider Services { get; }
    public IScreenGeneratorProvider ScreenGenerators { get; }

    public RedSettings RedManager { get; private set; }

    private readonly ILogger _logger = Logger.Create<DisplayApplication>();

    private bool _running = false;
    private bool _shouldRun = false;

    private readonly Wrapped<TicksTime> _waitUntil = TicksTime.Zero;

    internal DisplayApplication(IServiceProvider services, IScreenGeneratorProvider screenGeneratorProvider) {
        Services = services;
        ScreenGenerators = screenGeneratorProvider;
    }

    public async Task InitDefaultServices() {
        ConfigService configService = Services.GetService<ConfigService>() ?? throw new InvalidOperationException("The service 'ConfigService' has to be registered");
        await configService.InitAsync();

        BrightnessService autoBrightnessService = Services.GetService<BrightnessService>() ?? throw new InvalidOperationException("The Service of type 'AutoBrightnessService' must be registered");
        RedManager = new RedSettings(autoBrightnessService);

        await InitAsync<WeatherIconLoader>();
        await InitAsync<SymbolLoader>();

        Init<OpenWeatherMapClient>();
        Init<WeatherApiClient>();
        Init<SpotifyService>();
        Init<GasPriceService>();

        SensorService? sensorService = Services.GetService<SensorService>();
        if (sensorService is not null)
            _ = sensorService.ScanAsync();

    }

    private void Init<T>() where T : IInitializable => Init<T>(service => service.Init());
    private void Init<T>(Action<T> action) {
        T? service = Services.GetService<T>();
        if(service is null) {
            return;
        }

        action(service);
    }


    private async Task InitAsync<T>() where T : IAsyncInitializable => await InitAsync<T>(service => service.InitAsync());
    private async Task InitAsync<T>(Func<T, Task> action) {
        T? service = Services.GetService<T>();
        if (service is null) {
            return;
        }

        await action(service);
    }



    public async Task Run() {
        if (_running) {
            _logger.LogInformation("Already running");
            return;
        }

        _logger.LogInformation("Starting up");

        _running = true;
        _shouldRun = true;

        DeviceService deviceService = Services.GetService<DeviceService>() ?? throw new InvalidOperationException("The Service of type 'DeviceService' must be registered");


        await deviceService.ScanAsync();

        ScreenGenerators.Reset();

        while (_shouldRun) {
            await ShowNextScreenAndWait(deviceService);
        }

        _running = false;
    }

    private async Task ShowNextScreenAndWait(DeviceService deviceService) {
        ByteScreen? screen = await GetNextScreenAsync(deviceService);

        if (screen is null) {
            return;
        }

        _logger.LogTrace("sending gif");
        await deviceService.SendGifAsync(screen.Image);
        _logger.LogTrace("waiting {screen time}ms", screen.ScreenTime.TotalMilliseconds);
        SetWaitUntil(screen.ScreenTime);
        await Extensions.SleepUntil(_waitUntil, () => !_shouldRun);
    }

    private async Task<ByteScreen?> GetNextScreenAsync(DeviceService deviceService) {
        int skips = 0;
        IScreenGenerator? screenGenerator = null;
        while (_shouldRun) {
            await LogIfSkippedLast(skips, screenGenerator);

            screenGenerator = ScreenGenerators.GetNextScreenGenerator();

            if (screenGenerator is null) {
                skips++;
                continue;
            }

            if (screenGenerator.ScreenTime <= TimeSpan.Zero || !screenGenerator.IsEnabled) {
                skips++;
                continue;
            }

            ByteScreen? screen;

            bool turnRed = RedManager.ShouldBeRed(deviceService.Brightness);
            try {
                screen = await screenGenerator.GenerateScreenAsync(turnRed);
            } catch (Exception ex) {
                _logger.LogError("Error creating next screen {ex}", ex);
                skips++;
                continue;
            }

            if (screen is null) {
                _logger.LogWarning("The screen generation timed out. Check your internet connection");
                skips++;
                continue;
            }

            if (screen.ScreenTime <= TimeSpan.Zero) {
                skips++;
                continue;
            }

            return screen;
        }

        return null;
    }

    private async Task LogIfSkippedLast(int skips, IScreenGenerator? screenGenerator) {
        if (skips > 0) {
            _logger.LogDebug("Skipping screen '{screenName}'", screenGenerator?.GetType().Name);
        }

        if (skips > ScreenGenerators.ScreenGeneratorCount) {
            _logger.LogInformation("Skipped all screen generators, waiting 30 Seconds to retry");
            await Extensions.Sleep(TimeSpan.FromSeconds(30), () => !_shouldRun);
        }
    }

    public async Task StopAsync() {
        _logger.LogInformation($"Stopping");
        _shouldRun = false;

        await Extensions.Sleep(TimeSpan.FromSeconds(1), () => !_running);

        if (_running) {
            _logger.LogError($"Didn't stop after 1 Seconds.");
            _logger.LogError($"Probably stuck in a task or already stopped because of an error!");
            _logger.LogError($"Pretending to be stopped!");
            _running = false;
        }
    }

    public async Task Inject(Screen screen) {
        ByteScreen byteScreen = await ByteScreen.FromScreenAsync(screen);

        DeviceService? deviceService = Services.GetService<DeviceService>();

        if (deviceService is null) {
            return;
        }

        await deviceService.SendGifAsync(byteScreen.Image);
        SetWaitUntil(byteScreen.ScreenTime);
    }

    private void SetWaitUntil(TimeSpan timeSpan) => _waitUntil.Value = TicksTime.Now + TicksTimeSpan.FromTimeSpan(timeSpan);
    public void StopInjection() {
        _waitUntil.Value = TicksTime.Now;
    }
}
