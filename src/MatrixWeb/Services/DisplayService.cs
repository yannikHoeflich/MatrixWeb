using MatrixWeatherDisplay.Data;
using MatrixWeatherDisplay.DependencyInjection;
using MatrixWeb.Extensions.Data;
using MatrixWeb.Extensions.Logging;
using static MatrixWeatherDisplay.DependencyInjection.DisplayApplication;

namespace MatrixWeb.Services;
public partial class DisplayService {
    private readonly DisplayApplication _application;
    private readonly ILogger _logger;

    public RedSettings RedManager => _application.RedManager ?? throw new InvalidOperationException("Please add 'RedSettings' to 'DisplayService' and initialize");

    public DisplayService(DisplayApplication application, ILogger<DisplayService> logger) {
        _application = application;
        _logger = logger;
    }

    public void Start() {
        var displayThread = new Thread(async () => await StartUpThreadAsync());
        displayThread.Start();
    }

    private async Task StartUpThreadAsync() {
        try {
            await _application.RunAsync();
        } catch (Exception ex) {
            _logger.LogCritical("Exception: {ex}", ex.ToString());
        }       
    }

    public async Task StopAsync() => await _application.StopAsync();

    public async Task RestartAsync() {
        _logger.LogInformation("Restarting display application");
        await StopAsync();
        Start();
    }

    public async Task InjectAsync(Screen screen) => await _application.InjectAsync(screen);

    public void StopInjection() => _application.StopInjection();
}
