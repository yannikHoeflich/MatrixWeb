namespace MatrixWeatherDisplay.Data;
public record Screen(Image<Rgb24> Image, TimeSpan ScreenTime) {

    public static Screen Empty => new(new Image<Rgb24>(1, 1), new TimeSpan(0));
}