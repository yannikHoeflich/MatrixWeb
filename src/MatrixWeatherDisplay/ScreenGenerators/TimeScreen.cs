using MatrixWeatherDisplay.Data;
using MatrixWeatherDisplay.Data.Converter;
using MatrixWeatherDisplay.Screens;
using MatrixWeatherDisplay.Services;
using MatrixWeb.Extensions;
using MatrixWeb.Extensions.Data;
using static OpenWeatherMap.NetClient.Models.Forecast5Days;

namespace MatrixWeatherDisplay.ScreenGenerators;
public class TimeScreen : IScreenGenerator {
    private readonly SymbolLoader _symbolLoader;
    private readonly ColorHelper _colorHelper;

    public string Name { get; } = "Uhr";

    public string Description { get; } = "Zeight die Uhrzeit an";

    public TimeSpan ScreenTime { get; set; } = TimeSpan.FromSeconds(3);

    public bool IsEnabled { get; } = true;

    public bool RequiresInternet => false;

    public TimeScreen(SymbolLoader symbolLoader, ColorHelper colorHelper) {
        _symbolLoader = symbolLoader;
        _colorHelper = colorHelper;
    }

    public Task<Screen> GenerateImageAsync() {
        var image = new Image<Rgb24>(16, 16);
        DateTime now = DateTime.Now;

        int hours = now.Hour;
        int minutes = now.Minute;

        Color colorHour = _colorHelper.MapHour(now.TotalHours());
        Color colorMinute = _colorHelper.MapMinute(now.MinutesOfHour());

        _symbolLoader.DrawNumber(image, hours, 2, 0, 4, colorHour);
        _symbolLoader.DrawNumber(image, minutes, 2, 9, 4, colorMinute);

        var screen = new Screen(image, ScreenTime);
        return Task.FromResult(screen);
    }
}
