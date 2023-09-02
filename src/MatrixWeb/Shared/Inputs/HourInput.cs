using MatrixWeb.Extensions.Services.Translation;
using Microsoft.AspNetCore.Components;
using Refit;

namespace MatrixWeb.Shared.Inputs;
public class HourInput : ExtendedInput {
    private static bool s_textIdsSet = false;

    private static int s_hourText = -1;

    [Inject]
    public required TextService TextService { get; set; }

    private static void InitTexts(TextService textService) {
        s_hourText = textService.AddText(new TextElement(LanguageCode.EN, "Hours"), new TextElement(LanguageCode.DE, "Stunden"));

        s_textIdsSet = true;
    }
    protected override void OnParametersSet() {
        if (!s_textIdsSet) {
            InitTexts(TextService);
        }

        Text = TextService[s_hourText];
        base.OnParametersSet();
    }

    public static void Reset() {
        s_textIdsSet = false;
    }
}
