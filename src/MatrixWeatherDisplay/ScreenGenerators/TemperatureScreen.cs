using MatrixWeatherDisplay.Data;
using MatrixWeatherDisplay.Data.Converter;
using MatrixWeatherDisplay.Screens;
using MatrixWeatherDisplay.Services;
using MatrixWeatherDisplay.Services.Weather;

namespace MatrixWeatherDisplay.ScreenGenerators;
public class TemperatureScreen : IScreenGenerator {
    private readonly SymbolLoader _symbolLoader;
    private readonly WeatherService _weather;


    public string Name { get; } = "außen Temperatur";

    public string Description { get; } = "Zeigt die momentane außen temperatur an";

    public TimeSpan ScreenTime { get; set; } = TimeSpan.FromSeconds(3);

    public bool IsEnabled => _weather.IsEnabled;

    public TemperatureScreen(SymbolLoader symbolLoader, WeatherService weatherService) {
        _symbolLoader = symbolLoader;
        _weather = weatherService;
    }

    public async Task<Screen> GenerateImageAsync() {
        var weather = await _weather.GetWeatherAsync();
        int temp = (int)Math.Round(weather.Temperature);

        Color color = ColorHelper.MapTemperature(temp);

        var image = GenerateTemperatureScreen(16, 16, temp, color);
        return new Screen(image, ScreenTime);
    }


    private Image<Rgb24> GenerateTemperatureScreen(int height, int width, int temperature, Color color) {
        var image = new Image<Rgb24>(width, height);

        if (temperature < 0) {
            temperature *= -1;
            if (temperature >= 10) {
                image[0, 8] = color;
                image[1, 8] = color;

                _symbolLoader.DrawNumber(image, temperature, 2, 3, 4, color);


                _symbolLoader.DrawSymbol(image, 11, 3, '°', color);
                return image;
            } else {
                image[2, 8] = color;
                image[3, 8] = color;
            }
        }

        _symbolLoader.DrawSymbol(image, 11, 3, '°', color);

        if (temperature < 10) {
            _symbolLoader.DrawNumber(image, temperature, 1, 6, 4, color);

            return image;
        }

        _symbolLoader.DrawNumber(image, temperature, 2, 1, 4, color);

        return image;
    }
}
