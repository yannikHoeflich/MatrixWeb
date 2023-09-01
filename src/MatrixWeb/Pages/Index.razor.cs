using MatrixWeb.Extensions.Services.Translation;

namespace MatrixWeb.Pages;
public partial class Index {
    private static bool s_textIdsSet = false;
    private static int s_nameText = -1;
    private static int s_descriptionText = -1;
    private static int s_screenTimeText = -1;
    private static int s_brightnessText = -1;
    private static int s_automaticText = -1;
    private static int s_showAllText = -1;

    protected override void OnInitialized() {
        base.OnInitialized();

        if (!s_textIdsSet) {
            InitTexts(textService);
        }
    }

    private static void InitTexts(TextService textService) {
        s_nameText = textService.AddText(new TextElement(LanguageCode.EN, "Name"), new TextElement(LanguageCode.DE, "Name"));
        s_descriptionText = textService.AddText(new TextElement(LanguageCode.EN, "Description"), new TextElement(LanguageCode.DE, "Beschreibung"));
        s_screenTimeText = textService.AddText(new TextElement(LanguageCode.EN, "Screen time in seconds"), new TextElement(LanguageCode.DE, "Anzeige Dauer in Sekunden"));
        s_brightnessText = textService.AddText(new TextElement(LanguageCode.EN, "Brightness"), new TextElement(LanguageCode.DE, "Helligkeit"));
        s_automaticText = textService.AddText(new TextElement(LanguageCode.EN, "Automatic"), new TextElement(LanguageCode.DE, "Automatisch"));
        s_showAllText = textService.AddText(new TextElement(LanguageCode.EN, "Show all"), new TextElement(LanguageCode.DE, "Alle anzeigen"));

        s_textIdsSet = true;
    }
}
