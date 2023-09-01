using MatrixWeb.Extensions.Services.Translation;

namespace MatrixWeb.Extensions;
public record struct InitResult(InitResultType ResultType, Text Message) {
    public static readonly InitResult Success = new(InitResultType.Success, Text.Empty);

    public static InitResult Warning(Text message) => new(InitResultType.Warning, message);
    public static InitResult Error(Text message) => new(InitResultType.Error, message);
    public static InitResult Critical(Text message) => new(InitResultType.Critical, message);

    public static InitResult NoConfig() => Error(new Text(
        new TextElement( LanguageCode.EN , "There is no config for this service, even thought its required"),
        new TextElement( LanguageCode.DE , "Es gibt keine configuration für diesen Service, obwohl dieser benötigt wird")
        ));
    public static InitResult NoConfigElements(params string[] required) => Error(new Text(
        new TextElement(LanguageCode.EN, $"The config needs to contain: {string.Join(", ", required)}"),
        new TextElement(LanguageCode.DE, $"Die Konfiguration muss folgendes enthalten: {string.Join(", ", required)}")
        ));
}

public enum InitResultType {
    Success,
    Warning,
    Error,
    Critical
}