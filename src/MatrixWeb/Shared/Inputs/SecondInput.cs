﻿using MatrixWeb.Extensions.Services.Translation;
using Microsoft.AspNetCore.Components;

namespace MatrixWeb.Shared.Inputs;
public class SecondInput : ExtendedInput {
    private static bool s_textIdsSet = false;

    private static int _secondsText = -1;

    [Inject]
    public TextService TextService { get; set; }

    private static void InitTexts(TextService textService) {
        _secondsText = textService.AddText(new TextElement(LanguageCode.EN, "Sec"), new TextElement(LanguageCode.DE, "Sek."));

        s_textIdsSet = true;
    }

    protected override void OnParametersSet() {
        if (!s_textIdsSet) {
            InitTexts(TextService);
        }

        Text = TextService[_secondsText];
        base.OnParametersSet();
    }
}
