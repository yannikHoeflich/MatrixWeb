using MatrixWeatherDisplay.Data;
using MatrixWeatherDisplay.DependencyInjection;
using MatrixWeatherDisplay.Logging;
using static MatrixWeatherDisplay.DependencyInjection.DisplayApplication;

namespace MatrixWeb.Services;
public partial class DisplayService {
    private readonly DisplayApplication _application;
    private readonly ILogger _logger = Logger.Create<DisplayApplication>();

    public RedSettings RedManager => _application.RedManager;

    public DisplayService(DisplayApplication application) {
        _application = application;
    }

    public void Start() {
        var displayThread = new Thread(async () => await StartUpThread());
        displayThread.Start();
    }

    private async Task StartUpThread() {
        ILogger logger = Logger.Create<DisplayService>();
        try {
            await _application.Run();
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

    public async Task Inject(Screen screen) => await _application.Inject(screen);

    public void StopInjection() => _application.StopInjection();
}
