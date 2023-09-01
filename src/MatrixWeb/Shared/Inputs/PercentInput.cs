using MatrixWeb.Extensions.Services.Translation;
using Microsoft.AspNetCore.Components;

namespace MatrixWeb.Shared.Inputs;
public class PercentInput : ExtendedInput {
    private static bool s_textIdsSet = false;

    private static int _percentText = -1;

    [Inject]
    public TextService TextService { get; set; }

    private static void InitTexts(TextService textService) {
        _percentText = textService.AddText(new TextElement(LanguageCode.EN, "%"), new TextElement(LanguageCode.DE, "%"));

        s_textIdsSet = true;
    }
    protected override void OnParametersSet() {
        if (!s_textIdsSet) {
            InitTexts(TextService);
        }

        Text = TextService[_percentText];
        Multiplier = 100;
        base.OnParametersSet();
    }
}
