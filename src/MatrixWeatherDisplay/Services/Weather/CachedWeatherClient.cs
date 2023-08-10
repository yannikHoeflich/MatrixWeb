using System.Diagnostics;

using MatrixWeatherDisplay.Data.Extensions;

namespace MatrixWeatherDisplay.Services.Weather;
public abstract class CachedWeatherClient {
    private static readonly TicksTimeSpan s_updateFrequency = TicksTimeSpan.FromTimeSpan(TimeSpan.FromMinutes(5));

    private WeatherStatus? _currentWeather;
    private TicksTime _lastUpdate = TicksTime.MinValue;

    protected abstract Task<WeatherStatus> UpdateWeather();

    private async Task PrivateUpdateWeather() {
        WeatherStatus newWeather = await UpdateWeather();

        _lastUpdate = TicksTime.Now;
        _currentWeather = newWeather;
    }

    public async Task<WeatherStatus> GetWeatherAsync() {
        if (TicksTime.Now - _lastUpdate >= s_updateFrequency || _currentWeather is null) {
            await PrivateUpdateWeather();
        }

        return _currentWeather ?? throw new UnreachableException("Current weather should always has a value as this point");
    }
}