﻿using MatrixWeatherDisplay.Data;
using MatrixWeatherDisplay.Logging;
using Microsoft.Extensions.Logging;
using NETWeatherAPI;
using NETWeatherAPI.Entities;

namespace MatrixWeatherDisplay.Services.Weather;
public class WeatherApiClient : CachedWeatherClient {
    private readonly ILogger _logger = Logger.Create<WeatherApiClient>();
    private readonly WeatherAPIClient _weatherAPIClient;
    private readonly string _cityName;
    public WeatherApiClient(string apiKey, string cityName){
        _weatherAPIClient = new WeatherAPIClient(apiKey);
        _cityName = cityName;
    }

    protected override async Task<WeatherStatus> UpdateWeather() {
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
