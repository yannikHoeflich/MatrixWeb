using MatrixWeatherDisplay.Data;
using MatrixWeatherDisplay.Data.Converter;
using MatrixWeatherDisplay.Screens;
using MatrixWeatherDisplay.Services;

namespace MatrixWeatherDisplay.ScreenGenerators;
public class TimeScreen : IScreenGenerator {
    private readonly SymbolLoader _symbolLoader;

    public string Name { get; } = "Uhr";

    public string Description { get; } = "Zeight die Uhrzeit an";

    public TimeSpan ScreenTime { get; set; } = TimeSpan.FromSeconds(3);

    public TimeScreen(SymbolLoader symbolLoader) {
        _symbolLoader = symbolLoader;
    }

    public Task<Screen> GenerateImageAsync() {
        var image = new Image<Rgb24>(16, 16);
        DateTime now = DateTime.Now;

        int hours = now.Hour;
        int minutes = now.Minute;

        Color colorHour = ColorHelper.MapHour(now.TotalHours());
        Color colorMinute = ColorHelper.MapMinute(now.MinutesOfHour());

        _symbolLoader.DrawNumber(image, hours, 2, 0, 4, colorHour);
        _symbolLoader.DrawNumber(image, minutes, 2, 9, 4, colorMinute);

        var screen = new Screen(image, ScreenTime);
        return Task.FromResult(screen);
    }
}
