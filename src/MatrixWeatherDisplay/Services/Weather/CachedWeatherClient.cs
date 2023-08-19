using System.Diagnostics;

using MatrixWeatherDisplay.Data;
using MatrixWeb.Extensions;
using MatrixWeb.Extensions.Data;
using MatrixWeb.Extensions.Services;

namespace MatrixWeatherDisplay.Services.Weather;
public abstract class CachedWeatherClient : IInitializable, IService {
    private static readonly TicksTimeSpan s_updateFrequency = TicksTimeSpan.FromTimeSpan(TimeSpan.FromMinutes(5));

    private WeatherStatus? _currentWeather;
    private TicksTime _lastUpdate = TicksTime.MinValue;

    public abstract bool IsEnabled { get; protected set; }
    public abstract void Init();

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