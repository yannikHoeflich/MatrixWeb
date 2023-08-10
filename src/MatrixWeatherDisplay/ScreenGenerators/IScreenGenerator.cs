using MatrixWeatherDisplay.Data;
using MatrixWeatherDisplay.Data.Converter;

namespace MatrixWeatherDisplay.Screens;
public interface IScreenGenerator {
    public string Name { get; }
    public string Description { get; }
    public TimeSpan ScreenTime { get; set; }

    public Task<Screen> GenerateImageAsync();

    internal async Task<ByteScreen> GenerateScreenAsync(bool turnRed) {
        Screen screen = await GenerateImageAsync();
        if (turnRed) {
            ImageHelper.SetColor(screen.Image, Color.FromRgb(255, 0, 0));
        }

        return await ByteScreen.FromScreenAsync(screen);
    }
}
