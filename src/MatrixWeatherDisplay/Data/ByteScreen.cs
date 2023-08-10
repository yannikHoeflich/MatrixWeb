namespace MatrixWeatherDisplay.Data;
internal record ByteScreen(byte[] Image, TimeSpan ScreenTime) {
    public static async Task<ByteScreen> FromScreenAsync(Screen screen) {
        byte[] bytes = await screen.Image.ToGifBytesAsync();

        return new ByteScreen(bytes, screen.ScreenTime);
    }
}