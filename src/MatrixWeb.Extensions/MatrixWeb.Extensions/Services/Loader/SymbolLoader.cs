﻿using MatrixWeb.Extensions;
using MatrixWeb.Extensions.Data.Config;
using MatrixWeb.Extensions.Services;
using MatrixWeb.Extensions.Services.Translation;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace MatrixWeatherDisplay.Services;
public class SymbolLoader : IAsyncInitializable, IService {
    private const string s_directory = "Icons/Symbols";
    public ConfigLayout ConfigLayout { get; } = ConfigLayout.Empty;

    private static readonly IReadOnlyDictionary<string, char> s_files = new Dictionary<string, char>() {
        {"0.png", '0'},
        {"1.png", '1'},
        {"2.png", '2'},
        {"3.png", '3'},
        {"4.png", '4'},
        {"5.png", '5'},
        {"6.png", '6'},
        {"7.png", '7'},
        {"8.png", '8'},
        {"9.png", '9'},
        {"DegreeCelsius.png", '°'},
        {"Euro.png", '€'},
        {"cents.png", 'c'},
        {"Percent.png", '%'},
    };

    private readonly Dictionary<char, bool[,]> _symbols = new();

    private static async Task<bool[,]> LoadSymbolAsync(string path) {
        Image<Rgba32> image = await Image.LoadAsync<Rgba32>(path);

        bool[,] symbol = new bool[image.Width, image.Height];
        for (int y = 0; y < image.Height; y++) {
            for (int x = 0; x < image.Width; x++) {
                symbol[x, y] = image[x, y].A > 0;
            }
        }

        return symbol;
    }

    public async Task<InitResult> InitAsync() {
        foreach (KeyValuePair<string, char> symbol in s_files) {
            string file = symbol.Key;
            char character = symbol.Value;
            bool[,] symbolMatrix;
            try {
                symbolMatrix = await LoadSymbolAsync(Path.Combine(s_directory, file));
            } catch(Exception ex) {
                return InitResult.Critical(new Text(new TextElement(LanguageCode.EN, $"Couldn't load file '{file}': {ex.Message}"),
                                                    new TextElement(LanguageCode.DE, $"Konnte Datei nicht laden '{file}': {ex.Message}")));
            }

            _symbols.Add(character, symbolMatrix);
        }

        return InitResult.Success;
    }
    public void DrawNumber(Image<Rgb24> image, int value, int length, int x, int y, Color color) {
        if (value < 0) {
            return;
        }

        for (int i = 0; i < length; i++) {
            int digit = value % 10;

            int dx = (length - i - 1) * 4;

            DrawSymbol(image, x + dx, y, digit.ToString()[0], color);

            value /= 10;
        }
    }

    public void DrawSymbol(Image<Rgb24> image, int x, int y, char character, Color color) {

        bool[,] symbol = _symbols[character];

        for (int dx = 0; dx < symbol.GetLength(0); dx++) {
            for (int dy = 0; dy < symbol.GetLength(1); dy++) {
                if (symbol[dx, dy]) {
                    image[x + dx, y + dy] = color;
                }
            }
        }
    }
}