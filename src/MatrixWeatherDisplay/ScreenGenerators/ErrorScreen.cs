using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MatrixWeatherDisplay.Data;
using MatrixWeatherDisplay.Screens;
using MatrixWeatherDisplay.Services;
using MatrixWeatherDisplay.Services.IconLoader;
using MatrixWeb.Extensions;
using MatrixWeb.Extensions.Data;
using MatrixWeb.Extensions.Services.Translation;
using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace MatrixWeatherDisplay.ScreenGenerators;
public class ErrorScreen : IScreenGenerator {
    private readonly InternetService _internetService;
    private readonly ErrorIconLoader _errorIconLoader;
    public Text Name { get; } = new Text(new TextElement(LanguageCode.EN, "Error message"), new TextElement(LanguageCode.DE, "Fehler meldungen"));
    public Text Description { get; } = new Text(
        new TextElement(LanguageCode.EN, "If something does not work (e.g. no internet connection) this screen shows it."), 
        new TextElement(LanguageCode.DE, "Falls etwas nicht funktioniert (z.B. keine Internetverbindung) wird das angezeigt.")
        );

    public TimeSpan ScreenTime { get; set; } = TimeSpan.FromSeconds(5);
    public bool IsEnabled { get; } = true;

    public bool RequiresInternet => false;

    public ErrorScreen(InternetService internetService, ErrorIconLoader errorIconLoader) {
        _internetService = internetService;
        _errorIconLoader = errorIconLoader;
    }


    public async Task<Screen> GenerateImageAsync() { 
        if(await _internetService.HasInternetConnectionAsync()) {
            return Screen.Empty;
        }

        Image<Rgb24> image = _errorIconLoader.GetIconAsync(ErrorType.NoInternet);
        return new Screen(image, ScreenTime);
    }
}
