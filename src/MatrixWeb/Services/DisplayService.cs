using MatrixWeatherDisplay.Data;
using MatrixWeatherDisplay.DependencyInjection;
using MatrixWeatherDisplay.Logging;
using MatrixWeb.Extensions.Data;
using static MatrixWeatherDisplay.DependencyInjection.DisplayApplication;

namespace MatrixWeb.Services;
public partial class DisplayService {
    private readonly DisplayApplication _application;
    private readonly ILogger _logger = Logger.Create<DisplayApplication>();

    public RedSettings RedManager => _application.RedManager ?? throw new InvalidOperationException("Please add 'RedSettings' to 'DisplayService' and initialize");

    public DisplayService(DisplayApplication application) {
        _application = application;
    }

    public void Start() {
        var displayThread = new Thread(async () => await StartUpThreadAsync());
        displayThread.Start();
    }

    private async Task StartUpThreadAsync() {
        ILogger logger = Logger.Create<DisplayService>();
        try {
            await _application.RunAsync();
        } catch (Exception ex) {
            logger.LogCritical("Exception: {ex}", ex.ToString());
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
