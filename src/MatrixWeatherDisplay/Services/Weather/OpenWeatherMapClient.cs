using MatrixWeatherDisplay.Data;
using MatrixWeatherDisplay.Logging;
using Microsoft.Extensions.Logging;
using OpenWeatherMap.NetClient.Models;

namespace MatrixWeatherDisplay.Services.Weather;
public class OpenWeatherMapClient : CachedWeatherClient {
    private readonly OpenWeatherMap.NetClient.OpenWeatherMapClient _client;
    private readonly double _latitude;
    private readonly double _longitude;

    private readonly ILogger _logger = Logger.Create<OpenWeatherMapClient>();

    public OpenWeatherMapClient(string apiKey, double lat, double lon) {
        _client = new OpenWeatherMap.NetClient.OpenWeatherMapClient(apiKey);
        _latitude = lat;
        _longitude = lon;
    }

    protected override async Task<WeatherStatus> UpdateWeather() {
        Func<Task<CurrentWeather?>> getWeatherFunction = () => _client.CurrentWeather.GetByCoordinatesAsync(_latitude, _longitude);
        CurrentWeather? response = await Extensions.Retry(getWeatherFunction, 5, _logger);

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
