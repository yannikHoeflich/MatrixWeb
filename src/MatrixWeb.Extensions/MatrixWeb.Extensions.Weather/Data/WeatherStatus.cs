namespace MatrixWeb.Extensions.Weather.Data;
public record struct WeatherStatus(
    WeatherType Weather,
    /// <summary>
    ///     in °C
    /// </summary>
    double Temperature,

    /// <summary>
    ///     in %
    /// </summary>
    double Humidity,

    /// <summary>
    ///     in Hectopascal
    /// </summary>
    double AirPressure,

    /// <summary>
    ///     in m
    /// </summary>
    double Visibility,

    /// <summary>
    ///     in m/s
    /// </summary>
    double WindSpeed
);