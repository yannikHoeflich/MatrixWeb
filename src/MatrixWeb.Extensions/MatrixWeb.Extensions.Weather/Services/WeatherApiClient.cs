﻿using MatrixWeb.Extensions.Data;
using MatrixWeb.Extensions.Data.Config;
using MatrixWeb.Extensions.Services;
using MatrixWeb.Extensions.Weather.Data;
using Microsoft.Extensions.Logging;
using NETWeatherAPI;
using NETWeatherAPI.Entities;

namespace MatrixWeb.Extensions.Weather.Services;
public class WeatherApiClient : CachedWeatherClient, IInitializable {
    private const string s_configName = "weather-api";
    private const string s_apiKeyName = "api-key";
    private const string s_cityName = "city";

    private readonly ILogger _logger;
    private WeatherAPIClient? _weatherAPIClient;
    private string? _cityName;

    private readonly ConfigService _configService;

    public override bool IsEnabled { get; protected set; } = false;


    public override string Name => "Weather Api";
    public override ConfigLayout ConfigLayout { get; } = new() {
        ConfigName = s_configName,
        Keys = new ConfigKey[] {
            new(s_apiKeyName, typeof(string)),
            new(s_cityName, typeof(string))
        }
    };

    public WeatherApiClient(ConfigService configService, ILogger<WeatherApiClient> logger) {
        _configService = configService;
        _logger = logger;
    }

    public override InitResult Init() {
        RawConfig? config = _configService.GetConfig(s_configName);
        if (config is null) {
            IsEnabled = false;
            return InitResult.NoConfig();
        }

        if (!config.TryGetString(s_apiKeyName, out string? apiKey) || !config.TryGetString(s_cityName, out _cityName)) {
            IsEnabled = false;
            return InitResult.NoConfigElements(s_apiKeyName, s_cityName);
        }

        _weatherAPIClient = new WeatherAPIClient(apiKey);
        IsEnabled = true;
        return InitResult.Success;
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
        return new WeatherStatus(weatherType, response.Current.TemperatureC, response.Current.Humidity, response.Current.PressureMB, response.Current.VisibilityKm * 1000, response.Current.WindKPH / 3.6);
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
