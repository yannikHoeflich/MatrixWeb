using MatrixWeatherDisplay.Data;

namespace MatrixWeatherDisplay.Services;
public class WeatherIconLoader {
    private const string s_directory = "Icons/Weather";

    private static readonly IReadOnlyDictionary<string, WeatherType> s_files = new Dictionary<string, WeatherType>() {
        {"Cloudy.gif", WeatherType.Clouds},
        {"Rain.gif", WeatherType.Rain},
        {"Sun.gif", WeatherType.ClearDay},
        {"ClearNight.gif", WeatherType.ClearNight},
        {"Thunder.gif", WeatherType.Thunderstorm},
        {"Snow.gif", WeatherType.Snow},
        {"Fog.gif", WeatherType.Fog},
        {"Drizzle.gif", WeatherType.Drizzle},
        {"CloudsWithSun.gif", WeatherType.PartlyCloudyDay},
        {"CloudsWithMoon.gif", WeatherType.PartlyCloudyNight},
    };

    private readonly Dictionary<WeatherType, Image<Rgb24>> _iconCash = new();

    private static async Task<Image<Rgb24>> LoadGifAsync(string path) => await Image.LoadAsync<Rgb24>(path);

    public async Task LoadGifsAsync() {
        foreach (KeyValuePair<string, WeatherType> item in s_files) {
            string file = item.Key;
            WeatherType weatherType = item.Value;

            Image<Rgb24> gif = await LoadGifAsync(Path.Combine(s_directory, file));

            _iconCash.Add(weatherType, gif);
        }
    }

    public Image<Rgb24> GetWeatherIconAsync(WeatherType weatherType) => _iconCash[weatherType].Clone();
}
