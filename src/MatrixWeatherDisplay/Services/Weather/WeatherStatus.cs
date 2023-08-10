using MatrixWeatherDisplay.Data;

namespace MatrixWeatherDisplay.Services.Weather;
public record struct WeatherStatus(WeatherType Weather, double Temperature, double Humidity);