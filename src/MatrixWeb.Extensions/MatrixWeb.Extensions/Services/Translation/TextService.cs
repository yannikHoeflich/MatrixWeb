using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MatrixWeb.Extensions.Data;
using MatrixWeb.Extensions.Data.Config;
using System;

namespace MatrixWeb.Extensions.Services.Translation;
public class TextService : IService, IInitializable {
    public const string NoTextAvailableText = "ERROR: No translation or english text available!";

    private const string s_configName = "language";
    private const string s_languageCodeName = "code";

    public string this[int id] => GetText(id);

    private readonly Dictionary<int, Text> _texts = new();
    private int _count = 0;
    private LanguageCode _languageCode;

    private readonly ConfigService _configService;
    public ConfigLayout ConfigLayout { get; } = new ConfigLayout() {
        ConfigName = s_configName,
        Keys = new ConfigKey[] {
            new(s_languageCodeName, typeof(string))
        }
    };

    public bool IsEnabled => true;

    public TextService(ConfigService configService) {
        _configService = configService;
    }

    public InitResult Init() {
        RawConfig config = _configService.GetOrCreateConfig(ConfigLayout.ConfigName);
        if (config.TryGetString(s_languageCodeName, out string? language) && Enum.TryParse(language.ToUpper(), out LanguageCode languageCode)) {
            _languageCode = languageCode;
            return InitResult.Success;
        }

        _languageCode = LanguageCode.EN;
        return InitResult.Success;
    }

    public int AddText(params TextElement[] textElements) {
        int id = _count;
        _count++;
        _texts.Add(id, new Text(textElements));

        return id;
    }

    public string GetText(int id) {
        return _texts.TryGetValue(id, out Text? textCollection) && textCollection.TryGetText(_languageCode, out string? text)
            ? text
            : NoTextAvailableText;
    }

    public string GetTranslation(Text texts) {
        return texts.TryGetText(_languageCode, out string? text) 
            ? text 
            : NoTextAvailableText;
    }
}
