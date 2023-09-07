using System.Diagnostics;

using MatrixWeb.Extensions.Data;
using MatrixWeb.Extensions.Data.Config;
using MatrixWeb.Extensions.Services;
using MatrixWeb.Extensions.Weather.Data;

namespace MatrixWeb.Extensions.Weather.Services;
public abstract class CachedWeatherClient : IInitializable, IService {
    private static readonly TicksTimeSpan s_updateFrequency = TicksTimeSpan.FromTimeSpan(TimeSpan.FromMinutes(5));

    private WeatherStatus? _currentWeather;
    private TicksTime _lastUpdate = TicksTime.MinValue;

    public abstract bool IsEnabled { get; protected set; }

    public abstract InitResult Init();

    protected abstract Task<WeatherStatus> UpdateWeather();
    public abstract string Name { get; }

    public abstract ConfigLayout ConfigLayout { get; }
    private async Task PrivateUpdateWeatherAsync() {
        WeatherStatus newWeather = await UpdateWeather();

        _lastUpdate = TicksTime.Now;

        if(newWeather == default) {
            return;
        }

        _currentWeather = newWeather;
    }

    public async Task<WeatherStatus> GetWeatherAsync() {
        if (TicksTime.Now - _lastUpdate >= s_updateFrequency || _currentWeather is null) {
            await PrivateUpdateWeatherAsync();
        }

        return _currentWeather ?? throw new UnreachableException("Current weather should always has a value as this point");
    }
}