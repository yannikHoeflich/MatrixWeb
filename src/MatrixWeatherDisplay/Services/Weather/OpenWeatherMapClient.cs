using MatrixWeatherDisplay.Data;
using MatrixWeatherDisplay.Logging;
using MatrixWeb.Extensions.Data;
using MatrixWeb.Extensions.Services;
using Microsoft.Extensions.Logging;
using OpenWeatherMap.NetClient.Models;

namespace MatrixWeatherDisplay.Services.Weather;
public class OpenWeatherMapClient : CachedWeatherClient {
    private OpenWeatherMap.NetClient.OpenWeatherMapClient? _client;
    private double _latitude;
    private double _longitude;

    private readonly ILogger _logger = Logger.Create<OpenWeatherMapClient>();

    private readonly ConfigService _configService;

    public override bool IsEnabled { get; protected set;}

    public OpenWeatherMapClient(ConfigService configService) {
        _configService = configService;
    }

    public override void Init() {
        Config? config = _configService.GetConfig("open-weather-map");
        if(config is null) {
            IsEnabled = false;
            return;
        }

        if(!config.TryGetString("api-key", out string? apiKey) || apiKey is null) {
            IsEnabled = false;
            return;
        }

        _client = new OpenWeatherMap.NetClient.OpenWeatherMapClient(apiKey);

        config.TryGetDouble("latitude", out _latitude);
        config.TryGetDouble("longitude", out _longitude);
        IsEnabled = true;
    }

    protected override async Task<WeatherStatus> UpdateWeather() {
        if (_client is null) {
            throw new InvalidOperationException("The service 'OpenWeatherMap' should be initialized and get all values through the config, to be used!");
        }


        Func<Task<CurrentWeather?>> getWeatherFunction = () => _client.CurrentWeather.GetByCoordinatesAsync(_latitude, _longitude);
        CurrentWeather? response = await Extensions.RetryAsync(getWeatherFunction, 5, _logger);

        if(response is null ) {
            return default;
        }

        WeatherType weatherType = ToWeatherType(response.WeatherConditionId, IsDay(response.Sunrise, response.Sunset));

        return new WeatherStatus(weatherType, response.Temperature.DegreesCelsius, response.Humidity.Percent);
    }

    private static bool IsDay(DateTimeOffset sunrise, DateTimeOffset sunset) {
        DateTimeOffset now = DateTimeOffset.Now;
        return now > sunrise && now < sunset;
    }

    public static WeatherType ToWeatherType(int id, bool isDay) {
        int main = id / 100;
        return main switch {
            2 => WeatherType.Thunderstorm,
            3 => WeatherType.Drizzle,
            5 => WeatherType.Rain,
            6 => WeatherType.Snow,
            7 => WeatherType.Fog,
            8 => Clouds(id, isDay),
            _ => WeatherType.Clouds
        };
    }

    private static WeatherType Clouds(int id, bool isDay) {
        int amount = id % 10;

        return amount switch {
            >= 4 => WeatherType.Clouds,
            >= 2 => isDay ? WeatherType.PartlyCloudyDay : WeatherType.PartlyCloudyNight,
            _ => isDay ? WeatherType.ClearDay : WeatherType.Clouds
        };
    }
}
