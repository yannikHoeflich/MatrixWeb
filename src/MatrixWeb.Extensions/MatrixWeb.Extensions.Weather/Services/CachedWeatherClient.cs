using System.Diagnostics;

using MatrixWeb.Extensions.Data;
using MatrixWeb.Extensions.Data.Config;
using MatrixWeb.Extensions.Services;
using MatrixWeb.Extensions.Weather.Data;

namespace MatrixWeb.Extensions.Weather.Services;
public abstract class CachedWeatherClient : IInitializable, IService
{
    private static readonly TicksTimeSpan s_updateFrequency = TicksTimeSpan.FromTimeSpan(TimeSpan.FromMinutes(5));

    private WeatherStatus? _currentWeather;
    private TicksTime _lastUpdate = TicksTime.MinValue;

    public abstract bool IsEnabled { get; protected set; }

    public abstract void Init();

    protected abstract Task<WeatherStatus> UpdateWeather();

    public ConfigLayout ConfigLayout { get; } = ConfigLayout.Empty;
    private async Task PrivateUpdateWeather()
    {
        WeatherStatus newWeather = await UpdateWeather();

        _lastUpdate = TicksTime.Now;
        _currentWeather = newWeather;
    }

    public async Task<WeatherStatus> GetWeatherAsync()
    {
        if (TicksTime.Now - _lastUpdate >= s_updateFrequency || _currentWeather is null)
        {
            await PrivateUpdateWeather();
        }

        return _currentWeather ?? throw new UnreachableException("Current weather should always has a value as this point");
    }
}