using MatrixWeatherDisplay.Data;
using MatrixWeatherDisplay.Data.Converter;
using MatrixWeatherDisplay.Screens;
using MatrixWeatherDisplay.Services;
using MatrixWeatherDisplay.Services.SensorServices;
using MatrixWeatherDisplay.Services.Weather;

namespace MatrixWeatherDisplay.ScreenGenerators;
public class RoomHumidityScreen : IScreenGenerator {
    private readonly RoomHumidityService _roomHumidityService;
    private readonly SymbolLoader _symbolLoader;
    private readonly WeatherService _weatherService;

    public string Name { get; } = "Raum Luftfeuchtigkeit";

    public string Description { get; } = "Zeigt die aktuelle Luftfeuchtigkeit im Raum an";

    public TimeSpan ScreenTime { get; set; } = TimeSpan.FromSeconds(1);

    public bool IsEnabled => _weatherService.IsEnabled && _roomHumidityService.IsEnabled;

    public RoomHumidityScreen(RoomHumidityService roomHumidityService, SymbolLoader symbolLoader, WeatherService weatherService) {
        _roomHumidityService = roomHumidityService;
        _symbolLoader = symbolLoader;
        _weatherService = weatherService;
    }

    public async Task<Screen> GenerateImageAsync() {
        double? value = await _roomHumidityService.GetValueAsync();
        if(value is null) {
            return Screen.Empty;
        }

        WeatherStatus currentWeather = await _weatherService.GetWeatherAsync();

        Color color = ColorHelper.MapRoomHumidity(value.Value, currentWeather.Humidity);

        var image = new Image<Rgb24>(16, 16);
        _symbolLoader.DrawNumber(image, (int)value.Value, 2, 1, 4, color);
        _symbolLoader.DrawSymbol(image, 10, 4, '%', color);

        return new Screen(image, ScreenTime);
    }
}
