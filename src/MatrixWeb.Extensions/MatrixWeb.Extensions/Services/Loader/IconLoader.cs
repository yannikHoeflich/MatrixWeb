﻿using MatrixWeb.Extensions;
using MatrixWeb.Extensions.Data.Config;

namespace MatrixWeb.Extensions.Services.Loader;
public abstract class IconLoader<T> : IAsyncInitializable, IService where T : struct {
    protected abstract string p_directory { get; }

    protected abstract Dictionary<string, T> p_files { get; }

    private readonly Dictionary<T, Image<Rgb24>> _iconCash = new();

    public ConfigLayout ConfigLayout { get; } = ConfigLayout.Empty;

    private static async Task<Image<Rgb24>> LoadGifAsync(string path) => await Image.LoadAsync<Rgb24>(path);

    public Task InitAsync() => LoadGifsAsync();

    public async Task LoadGifsAsync() {
        foreach (KeyValuePair<string, T> item in p_files) {
            string file = item.Key;
            T name = item.Value;

            Image<Rgb24> gif = await LoadGifAsync(Path.Combine(p_directory, file));

            _iconCash.Add(name, gif);
        }
    }

    public Image<Rgb24> GetIconAsync(T name) => _iconCash[name].Clone();
}
