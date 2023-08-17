using MatrixWeatherDisplay.Data;
using MatrixWeatherDisplay.Services.IconLoader;
using MatrixWeatherDisplay.Services.Weather;

namespace MatrixWeatherDisplay.Screens;
public class WeatherScreen : IScreenGenerator {

    private readonly WeatherService _weather;
    private readonly WeatherIconLoader _iconLoader;

    public string Name { get; } = "Wetter";

    public string Description { get; } = "Zeigt das aktuelle Wetter als ein animiertes Icon an";

    public TimeSpan ScreenTime { get; set; } = TimeSpan.FromSeconds(5);

    public bool IsEnabled => _weather.IsEnabled;

    public bool NeedsInternet => true;

    public WeatherScreen(WeatherService weatherService, WeatherIconLoader iconLoader) {
        _weather = weatherService;
        _iconLoader = iconLoader;
    }


    public async Task<Screen> GenerateImageAsync() {
        WeatherStatus currentWeather = await _weather.GetWeatherAsync();
        Image<Rgb24> icon = _iconLoader.GetIconAsync(currentWeather.Weather);
        return new Screen(icon, ScreenTime);
    }
}
