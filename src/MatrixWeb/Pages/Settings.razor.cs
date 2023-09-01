using MatrixWeb.Extensions.Services.Translation;

namespace MatrixWeb.Pages;
public partial class Settings {
    private static bool s_textIdsSet = false;

    private static int s_saveText = -1;
    private static int s_configText = -1;
    private static int s_displayText = -1;
    private static int s_generalBrightnessText = -1;
    private static int s_timeShiftText = -1;
    private static int s_redThresholdText = -1;
    private static int s_alwaysRedText = -1;
    private static int s_neverRedText = -1;
    private static int s_redBetweenText = -1;
    private static int s_andText = -1;
    private static int s_spotifyText = -1;
    private static int s_spotifyConnectedText = -1;
    private static int s_connectWithSpotifyText = -1;
    private static int s_weatherText = -1;
    private static int s_weatherProviderText = -1;
    private static int s_displayControllerText = -1;
    private static int s_restartDisplayControllerText = -1;

    protected override void OnInitialized() {
        base.OnInitialized();
        PageOnInitialized();

        if (!s_textIdsSet) {
            InitTexts(textService);
        }
    }

    private static void InitTexts(TextService textService) {
        s_saveText = textService.AddText(new TextElement(LanguageCode.EN, "Save"), new TextElement(LanguageCode.DE, "Speichern"));
        s_configText = textService.AddText(new TextElement(LanguageCode.EN, "Config"), new TextElement(LanguageCode.DE, "Configuration"));
        s_displayText = textService.AddText(new TextElement(LanguageCode.EN, "Display"), new TextElement(LanguageCode.DE, "Bildschirm"));
        s_generalBrightnessText = textService.AddText(new TextElement(LanguageCode.EN, "General Brightness"), new TextElement(LanguageCode.DE, "Generelle Helligkeit"));
        s_timeShiftText = textService.AddText(new TextElement(LanguageCode.EN, "Brightness time shift"), new TextElement(LanguageCode.DE, "Helligkeits Zeit Verschiebung"));
        s_redThresholdText = textService.AddText(new TextElement(LanguageCode.EN, "Red threshold"), new TextElement(LanguageCode.DE, "Rot schwelle"));
        s_alwaysRedText = textService.AddText(new TextElement(LanguageCode.EN, "Always red"), new TextElement(LanguageCode.DE, "Immer rot"));
        s_neverRedText = textService.AddText(new TextElement(LanguageCode.EN, "Never red"), new TextElement(LanguageCode.DE, "Nie rot"));
        s_redBetweenText = textService.AddText(new TextElement(LanguageCode.EN, "Red between"), new TextElement(LanguageCode.DE, "Rot zwischen"));
        s_andText = textService.AddText(new TextElement(LanguageCode.EN, "and"), new TextElement(LanguageCode.DE, "und"));
        s_spotifyText = textService.AddText(new TextElement(LanguageCode.EN, "Spotify"), new TextElement(LanguageCode.DE, "Spotify"));
        s_spotifyConnectedText = textService.AddText(new TextElement(LanguageCode.EN, "Spotify connected"), new TextElement(LanguageCode.DE, "Spotify verbunden"));
        s_connectWithSpotifyText = textService.AddText(new TextElement(LanguageCode.EN, "Connect with Spotify"), new TextElement(LanguageCode.DE, "Mit Spotify verbinden"));
        s_weatherText = textService.AddText(new TextElement(LanguageCode.EN, "Weather"), new TextElement(LanguageCode.DE, "Wetter"));
        s_weatherProviderText = textService.AddText(new TextElement(LanguageCode.EN, "Weather provider"), new TextElement(LanguageCode.DE, "Wetter Anbieter"));
        s_displayControllerText = textService.AddText(new TextElement(LanguageCode.EN, "Display Controller"), new TextElement(LanguageCode.DE, "Bildschirm Controller"));
        s_restartDisplayControllerText = textService.AddText(new TextElement(LanguageCode.EN, "Restart Display Controller"), new TextElement(LanguageCode.DE, "Bildschirm Controller neustarten"));

        s_textIdsSet = true;
    }
}
