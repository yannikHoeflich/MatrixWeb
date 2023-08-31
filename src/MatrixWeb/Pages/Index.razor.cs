using MatrixWeb.Extensions.Services.Translation;

namespace MatrixWeb.Pages;
public partial class Index {
    private static bool s_textIdsSet = false;
    private static int s_nameId = -1;
    private static int s_descriptionId = -1;
    private static int s_screenTimeId = -1;
    private static int s_brightnessId = -1;
    private static int s_automaticId = -1;
    private static int s_showAllId = -1;

    protected override void OnInitialized() {
        base.OnInitialized();

        if (!s_textIdsSet) {
            InitTexts(textService);
        }
    }

    private static void InitTexts(TextService textService) {
        s_nameId = textService.AddText(new TextElement(LanguageCode.EN, "Name"), new TextElement(LanguageCode.DE, "Name"));
        s_descriptionId = textService.AddText(new TextElement(LanguageCode.EN, "Description"), new TextElement(LanguageCode.DE, "Beschreibung"));
        s_screenTimeId = textService.AddText(new TextElement(LanguageCode.EN, "Screen time in seconds"), new TextElement(LanguageCode.DE, "Anzeige Dauer in Sekunden"));
        s_brightnessId = textService.AddText(new TextElement(LanguageCode.EN, "Brightness"), new TextElement(LanguageCode.DE, "Helligkeit"));
        s_automaticId = textService.AddText(new TextElement(LanguageCode.EN, "Automatic"), new TextElement(LanguageCode.DE, "Automatisch"));
        s_showAllId = textService.AddText(new TextElement(LanguageCode.EN, "Show all"), new TextElement(LanguageCode.DE, "Alle anzeigen"));

        s_textIdsSet = true;
    }
}
