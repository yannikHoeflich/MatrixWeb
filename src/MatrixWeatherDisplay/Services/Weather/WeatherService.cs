namespace MatrixWeatherDisplay.Services.Weather;
public class WeatherService {
    private readonly Dictionary<WeatherProvider, CachedWeatherClient> _clients;

    public WeatherProvider WeatherProvider { get; set; } = WeatherProvider.OpenWeatherMap;

    public WeatherService(OpenWeatherMapClient openWeatherMapClient, WeatherApiClient weatherApiClient) {
        _clients = new() {
            {WeatherProvider.OpenWeatherMap, openWeatherMapClient},
            {WeatherProvider.WeatherApi, weatherApiClient}
        };
    }

    public async Task<WeatherStatus> GetWeatherAsync() {
        CachedWeatherClient provider = _clients[WeatherProvider];
        return await provider.GetWeatherAsync();
    }
}

public enum WeatherProvider{
    OpenWeatherMap,
    WeatherApi
}
