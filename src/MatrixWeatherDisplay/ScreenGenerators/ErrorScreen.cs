using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MatrixWeatherDisplay.Data;
using MatrixWeatherDisplay.Screens;
using MatrixWeatherDisplay.Services;
using MatrixWeatherDisplay.Services.IconLoader;
using System;

namespace MatrixWeatherDisplay.ScreenGenerators;
public class ErrorScreen : IScreenGenerator {
    private readonly InternetService _internetService;
    private readonly ErrorIconLoader _errorIconLoader;
    public string Name { get; } = "Fehler meldungen";
    public string Description { get; } = "Falls etwas nicht funktioniert (z.B. keine Internetverbindung) wird das angezeigt.";
    public TimeSpan ScreenTime { get; set; } = TimeSpan.FromSeconds(5);
    public bool IsEnabled { get; } = true;

    public bool NeedsInternet => false;

    public ErrorScreen(InternetService internetService, ErrorIconLoader errorIconLoader) {
        _internetService = internetService;
        _errorIconLoader = errorIconLoader;
    }


    public async Task<Screen> GenerateImageAsync() { 
        if(await _internetService.HasInternetConnection()) {
            return Screen.Empty;
        }

        Image<Rgb24> image = _errorIconLoader.GetIconAsync(ErrorType.NoInternet);
        return new Screen(image, ScreenTime);
    }
}
