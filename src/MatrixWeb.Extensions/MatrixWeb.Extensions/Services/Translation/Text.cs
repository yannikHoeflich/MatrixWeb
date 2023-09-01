using System.Diagnostics.CodeAnalysis;

namespace MatrixWeb.Extensions.Services.Translation;
public record Text(params TextElement[] Texts) {
    public static readonly Text Empty = new(new TextElement(LanguageCode.EN, ""), new TextElement(LanguageCode.DE, ""));

    public string GetText(LanguageCode languageCode) {
        return TryGetText(languageCode, out string? text)
            ? text
            : string.Empty;
    }

    public bool TryGetText(LanguageCode language, [NotNullWhen(true)] out string? text) {
        if (Texts.TryFirst(x => x.Language == language, out TextElement? textElement) && textElement is not null) {
            text = textElement.Text;
            return true;
        }

        if (Texts.TryFirst(x => x.Language == LanguageCode.EN, out TextElement? textElementEnglish) && textElementEnglish is not null) {
            text = textElementEnglish.Text;
            return true;
        }

        text = null;
        return false;
    }
}
