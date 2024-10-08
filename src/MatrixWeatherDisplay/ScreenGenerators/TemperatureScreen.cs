﻿using MatrixWeatherDisplay.Data;
using MatrixWeatherDisplay.Data.Converter;
using MatrixWeatherDisplay.Screens;
using MatrixWeatherDisplay.Services;
using MatrixWeb.Extensions;
using MatrixWeb.Extensions.Data;
using MatrixWeb.Extensions.Services.Translation;
using MatrixWeb.Extensions.Weather.Data;
using MatrixWeb.Extensions.Weather.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace MatrixWeatherDisplay.ScreenGenerators;
public class TemperatureScreen : IScreenGenerator {
    private readonly SymbolLoader _symbolLoader;
    private readonly WeatherService _weather;
    private readonly ColorHelper _colorHelper;

    public Text Name { get; } = new Text(new TextElement(LanguageCode.EN, "outside temperature"), new TextElement(LanguageCode.DE, "außen Temperatur"));

    public Text Description { get; } = new Text(
            new TextElement(LanguageCode.EN, "Shows the current outside temperature"),
            new TextElement(LanguageCode.DE, "Zeigt die momentane außen temperatur an")
        );

    public TimeSpan ScreenTime { get; set; } = TimeSpan.FromSeconds(3);

    public bool IsEnabled => _weather.IsEnabled;

    public bool RequiresInternet => true;

    public TemperatureScreen(SymbolLoader symbolLoader, WeatherService weatherService, ColorHelper colorHelper) {
        _symbolLoader = symbolLoader;
        _weather = weatherService;
        _colorHelper = colorHelper;
    }

    public async Task<Screen> GenerateImageAsync() {
        WeatherStatus weather = await _weather.GetWeatherAsync();
        int temp = (int)Math.Round(weather.Temperature);

        Color color = ColorHelper.MapTemperature(temp);

        Image<Rgb24> image = GenerateTemperatureScreen(16, 16, temp, color);
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
