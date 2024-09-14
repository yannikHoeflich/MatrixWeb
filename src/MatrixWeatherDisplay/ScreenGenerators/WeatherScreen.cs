using MatrixWeatherDisplay.Data;
using MatrixWeatherDisplay.Services.IconLoader;
using MatrixWeb.Extensions;
using MatrixWeb.Extensions.Data;
using MatrixWeb.Extensions.Services.Translation;
using MatrixWeb.Extensions.Weather.Data;
using MatrixWeb.Extensions.Weather.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace MatrixWeatherDisplay.Screens;
public class WeatherScreen : IScreenGenerator {

    private readonly WeatherService _weather;
    private readonly WeatherIconLoader _iconLoader;

    public Text Name { get; } = new Text(new TextElement(LanguageCode.EN, "Weather"), new TextElement(LanguageCode.DE, "Wetter"));

    public Text Description { get; } = new Text(
        new TextElement(LanguageCode.EN, "Shows the current weather as an animated icon"),
        new TextElement(LanguageCode.DE, "Zeigt das aktuelle Wetter als ein animiertes Icon an")
        );

    public TimeSpan ScreenTime { get; set; } = TimeSpan.FromSeconds(5);

    public bool IsEnabled => _weather.IsEnabled;

    public bool RequiresInternet => true;

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
