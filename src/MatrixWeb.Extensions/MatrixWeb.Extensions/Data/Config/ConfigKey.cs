namespace MatrixWeb.Extensions.Data.Config;

public record class ConfigKey(string Key, Type Type) {
    public static ConfigKey Create<T>(string key) => new(key, typeof(T));
}
