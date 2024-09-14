using MatrixWeatherDisplay.Data.Converter;
using MatrixWeb.Extensions.Data;
using SixLabors.ImageSharp;

namespace MatrixWeatherDisplay.Data;
internal record ByteScreen(byte[] Image, TimeSpan ScreenTime) {
    public static async Task<ByteScreen> FromScreenAsync(Screen screen, bool turnRed) {
        if (turnRed) {
            ImageHelper.SetColor(screen.Image, Color.FromRgb(255, 0, 0));
        }

        byte[] bytes = await screen.Image.ToGifBytesAsync();

        return new ByteScreen(bytes, screen.ScreenTime);
    }
}