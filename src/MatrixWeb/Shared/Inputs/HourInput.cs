using MatrixWeb.Extensions.Services.Translation;
using Microsoft.AspNetCore.Components;
using Refit;

namespace MatrixWeb.Shared.Inputs;
public class HourInput : ExtendedInput {
    private static bool s_textIdsSet = false;

    private static int _hourText = -1;

    [Inject]
    public TextService TextService { get; set; }

    private static void InitTexts(TextService textService) {
        _hourText = textService.AddText(new TextElement(LanguageCode.EN, "Hours"), new TextElement(LanguageCode.DE, "Stunden"));

        s_textIdsSet = true;
    }
    protected override void OnParametersSet() {
        if (!s_textIdsSet) {
            InitTexts(TextService);
        }

        Text = TextService[_hourText];
        base.OnParametersSet();
    }
}
