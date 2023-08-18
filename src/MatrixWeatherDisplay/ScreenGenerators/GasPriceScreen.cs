﻿using MatrixWeatherDisplay.Data;
using MatrixWeatherDisplay.Data.Converter;
using MatrixWeatherDisplay.Screens;
using MatrixWeatherDisplay.Services;
using MatrixWeb.Extensions;
using MatrixWeb.Extensions.Data;

namespace MatrixWeatherDisplay.ScreenGenerators;
public class GasPriceScreen : IScreenGenerator {
    private readonly GasPriceService _gasPrices;
    private readonly SymbolLoader _symbolLoader;
    private readonly ColorHelper _colorHelper;

    public string Name { get; } = "Benzin Preis";

    public string Description { get; } = "Zeigt den günstigsten Benzin preis im Umkreis an";

    public TimeSpan ScreenTime { get; set; } = TimeSpan.FromSeconds(3);

    public bool IsEnabled => _gasPrices.IsEnabled;

    public bool RequiresInternet => true;

    public GasPriceScreen(GasPriceService gasPrices, SymbolLoader symbolLoader, ColorHelper colorHelper) {
        _gasPrices = gasPrices;
        _symbolLoader = symbolLoader;
        _colorHelper = colorHelper;
    }

    public async Task<Screen> GenerateImageAsync() {
        double price = await _gasPrices.GetCheapestPriceAsync(51.954607, 8.668700);
        int priceCents = (int)(price * 100);

        Color color = _colorHelper.MapGasPrice(price, _gasPrices.MinPrice, _gasPrices.MaxPrice);

        var image = new Image<Rgb24>(16, 16);

        _symbolLoader.DrawNumber(image, priceCents, 3, 0, 4, color);
        _symbolLoader.DrawSymbol(image, 12, 4, 'c', color);

        return new Screen(image, ScreenTime);
    }
}
