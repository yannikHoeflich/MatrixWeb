using MatrixWeatherDisplay.Data;

namespace MatrixWeatherDisplay.Services.IconLoader;
public class WeatherIconLoader : IconLoader<WeatherType>
{
    protected override string p_directory { get; } = "Icons/Weather";

    protected override Dictionary<string, WeatherType> p_files { get; } = new Dictionary<string, WeatherType>() {
        {"Cloudy.gif", WeatherType.Clouds},
        {"Rain.gif", WeatherType.Rain},
        {"Sun.gif", WeatherType.ClearDay},
        {"ClearNight.gif", WeatherType.ClearNight},
        {"Thunder.gif", WeatherType.Thunderstorm},
        {"Snow.gif", WeatherType.Snow},
        {"Fog.gif", WeatherType.Fog},
        {"Drizzle.gif", WeatherType.Drizzle},
        {"CloudsWithSun.gif", WeatherType.PartlyCloudyDay},
        {"CloudsWithMoon.gif", WeatherType.PartlyCloudyNight},
    };

}
