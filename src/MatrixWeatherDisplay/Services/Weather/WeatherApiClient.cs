using MatrixWeatherDisplay.Data;
using MatrixWeatherDisplay.Logging;
using MatrixWeb.Extensions;
using MatrixWeb.Extensions.Data;
using MatrixWeb.Extensions.Services;
using Microsoft.Extensions.Logging;
using NETWeatherAPI;
using NETWeatherAPI.Entities;

namespace MatrixWeatherDisplay.Services.Weather;
public class WeatherApiClient : CachedWeatherClient, IInitializable {
    private readonly ILogger _logger = Logger.Create<WeatherApiClient>();
    private WeatherAPIClient? _weatherAPIClient;
    private string? _cityName;

    private readonly ConfigService _configService;

    public override bool IsEnabled { get; protected set; } = false;

    public WeatherApiClient(ConfigService configService) {
        _configService = configService;
    }

    public override void Init() {
        Config? config = _configService.GetConfig("weather-api");
        if (config is null) {
            IsEnabled = false;
            return;
        }

        if (!config.TryGetString("api-key", out string? apiKey) || !config.TryGetString("city", out _cityName)) {
            IsEnabled = false;
            return;
        }

        _weatherAPIClient = new WeatherAPIClient(apiKey);
        IsEnabled = true;
    }


    protected override async Task<WeatherStatus> UpdateWeather() {
        if (_weatherAPIClient is null || _cityName is null) {
            throw new InvalidOperationException("The service 'WeatherApiClient' should be initialized and get all values through the config, to be used!");
        }

        _logger.LogDebug("Updating Weather");
        RealtimeRequestEntity request = new RealtimeRequestEntity().WithCityName(_cityName);
        RealtimeResponseEntity response = await _weatherAPIClient.Realtime.GetCurrentAsync(request);

        CurrentEntity currentWeather = response.Current;
        WeatherType weatherType = ToWeatherType((int)currentWeather.Condition.Code, response.Current.IsDay);
        return new WeatherStatus(weatherType, response.Current.TemperatureC, response.Current.Humidity);
    }

    private static WeatherType ToWeatherType(int id, bool isDay) {
        id -= 1000;
        return id switch {
            0 => isDay ? WeatherType.ClearDay : WeatherType.ClearNight,
            3 => isDay ? WeatherType.PartlyCloudyDay : WeatherType.PartlyCloudyNight,
            6 or 9 => WeatherType.Clouds,
            30 => WeatherType.Fog,
            63 or 66 or 69 or 72 or 87 => WeatherType.Clouds,
            114 or 117 => WeatherType.Snow,
            135 or 147 => WeatherType.Fog,
            <= 171 => WeatherType.Drizzle,
            <= 201 => WeatherType.Rain,
            <= 225 => WeatherType.Snow,
            <= 246 => WeatherType.Rain,
            <= 258 => WeatherType.Snow,
            <= 264 => WeatherType.Rain,
            <= 282 => WeatherType.Thunderstorm,
            _ => isDay ? WeatherType.ClearDay : WeatherType.ClearNight
        };
    }
}
