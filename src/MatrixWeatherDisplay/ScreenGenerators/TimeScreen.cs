﻿using MatrixWeatherDisplay.Data;
using MatrixWeatherDisplay.Data.Converter;
using MatrixWeatherDisplay.Screens;
using MatrixWeatherDisplay.Services;
using MatrixWeb.Extensions;
using MatrixWeb.Extensions.Data;
using MatrixWeb.Extensions.Services.Translation;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using static OpenWeatherMap.NetClient.Models.Forecast5Days;

namespace MatrixWeatherDisplay.ScreenGenerators;
public class TimeScreen : IScreenGenerator {
    private readonly SymbolLoader _symbolLoader;
    private readonly ColorHelper _colorHelper;

    public Text Name { get; } = new Text(new TextElement(LanguageCode.EN, "Clock"), new TextElement(LanguageCode.DE, "Uhr"));

    public Text Description { get; } = new Text(
        new TextElement(LanguageCode.EN, "Show the current time"),
        new TextElement(LanguageCode.DE, "Zeigt die Uhrzeit an")
        );

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

        Color colorHour = ColorHelper.MapHour(now.TotalHours());
        Color colorMinute = ColorHelper.MapMinute(now.MinutesOfHour());

        _symbolLoader.DrawNumber(image, hours, 2, 0, 4, colorHour);
        _symbolLoader.DrawNumber(image, minutes, 2, 9, 4, colorMinute);

        var screen = new Screen(image, ScreenTime);
        return Task.FromResult(screen);
    }
}
