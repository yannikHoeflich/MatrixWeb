using MatrixWeb.Extensions.Data;
using MatrixWeb.Extensions.Data.Config;
using MatrixWeb.Extensions.Services;
using MatrixWeb.Extensions.Weather.Data;
using Microsoft.Extensions.Logging;
using OpenWeatherMap.NetClient.Models;

namespace MatrixWeb.Extensions.Weather.Services;
public class OpenWeatherMapClient : CachedWeatherClient {
    private const string s_configName = "open-weather-map";
    private const string s_apiKeyName = "api-key";
    private const string s_latitudeName = "latitude";
    private const string s_longitudeName = "longitude";

    private OpenWeatherMap.NetClient.OpenWeatherMapClient? _client;
    private double _latitude;
    private double _longitude;

    private readonly ILogger _logger;

    private readonly ConfigService _configService;

    public override bool IsEnabled { get; protected set; }

    public override ConfigLayout ConfigLayout { get; } = new() {
        ConfigName = s_configName,
        Keys = new ConfigKey[] {
            new(s_apiKeyName, typeof(string)),
            new(s_latitudeName, typeof(double)),
            new(s_longitudeName, typeof(double)),
        }
    };

    public OpenWeatherMapClient(ConfigService configService, ILogger<OpenWeatherMapClient> logger) {
        _configService = configService;
        _logger = logger;
    }

    public override InitResult Init() {
        RawConfig? config = _configService.GetConfig(s_configName);
        if (config is null) {
            IsEnabled = false;
            return InitResult.NoConfig();
        }

        if (!config.TryGetString(s_apiKeyName, out string? apiKey) || apiKey is null) {
            IsEnabled = false;
            return InitResult.NoConfigElements(s_apiKeyName);
        }

        _client = new OpenWeatherMap.NetClient.OpenWeatherMapClient(apiKey);

        config.TryGetDouble(s_latitudeName, out _latitude);
        config.TryGetDouble(s_longitudeName, out _longitude);
        IsEnabled = true;
        return InitResult.Success;
    }

    protected override async Task<WeatherStatus> UpdateWeather() {
        if (_client is null) {
            throw new InvalidOperationException("The service 'OpenWeatherMap' should be initialized and get all values through the config, to be used!");
        }


        Func<Task<CurrentWeather?>> getWeatherFunction = () => _client.CurrentWeather.GetByCoordinatesAsync(_latitude, _longitude);
        CurrentWeather? response = await getWeatherFunction.RetryAsync(5, _logger);

        if (response is null) {
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
