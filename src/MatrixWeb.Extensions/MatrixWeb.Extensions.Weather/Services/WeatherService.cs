using MatrixWeb.Extensions.Data.Config;
using MatrixWeb.Extensions.Services;
using MatrixWeb.Extensions.Weather.Data;

namespace MatrixWeb.Extensions.Weather.Services;
public class WeatherService : IService, IInitializable {
    private readonly Dictionary<WeatherProvider, CachedWeatherClient> _clients;

    public WeatherProvider WeatherProvider { get; set; } = WeatherProvider.OpenWeatherMap;

    public bool IsEnabled => _clients.Any(c => c.Value.IsEnabled);

    public ConfigLayout ConfigLayout { get; } = ConfigLayout.Empty;

    public WeatherService(OpenWeatherMapClient openWeatherMapClient, WeatherApiClient weatherApiClient) {
        _clients = new() {
            {WeatherProvider.OpenWeatherMap, openWeatherMapClient},
            {WeatherProvider.WeatherApi, weatherApiClient}
        };
    }

    public InitResult Init() {
        WeatherProvider = _clients.FirstOrDefault(x => x.Value.IsEnabled).Key;
        return InitResult.Success;
    }

    public async Task<WeatherStatus> GetWeatherAsync() {
        CachedWeatherClient provider = _clients[WeatherProvider];

        if (!provider.IsEnabled) {
            return new WeatherStatus();
        }

        return await provider.GetWeatherAsync();
    }

    public bool IsProviderEnabled(WeatherProvider weatherProvider) => _clients[weatherProvider].IsEnabled;
}

public enum WeatherProvider {
    OpenWeatherMap,
    WeatherApi
}
