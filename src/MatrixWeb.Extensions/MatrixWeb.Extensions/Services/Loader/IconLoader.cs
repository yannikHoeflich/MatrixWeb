using MatrixWeb.Extensions;
using MatrixWeb.Extensions.Data.Config;
using MatrixWeb.Extensions.Services.Translation;

namespace MatrixWeb.Extensions.Services.Loader;
public abstract class IconLoader<T> : IAsyncInitializable, IService where T : struct {
    protected abstract string p_directory { get; }

    protected abstract Dictionary<string, T> p_files { get; }

    private readonly Dictionary<T, Image<Rgb24>> _iconCash = new();

    public ConfigLayout ConfigLayout { get; } = ConfigLayout.Empty;

    private static async Task<Image<Rgb24>> LoadGifAsync(string path) => await Image.LoadAsync<Rgb24>(path);

    public async Task<InitResult> InitAsync() {
        foreach (KeyValuePair<string, T> item in p_files) {
            string file = item.Key;
            T name = item.Value;

            Image<Rgb24> gif;
            try {
                gif = await LoadGifAsync(Path.Combine(p_directory, file));
            } catch(Exception ex) {
                return InitResult.Critical(new Text(new TextElement(LanguageCode.EN, $"Couldn't load file '{file}': {ex.Message}"),
                                                    new TextElement(LanguageCode.DE, $"Konnte Datei nicht laden '{file}': {ex.Message}")));
            }

            _iconCash.Add(name, gif);
        }

        return InitResult.Success;
    }
    public Image<Rgb24> GetIconAsync(T name) => _iconCash[name].Clone();
}
