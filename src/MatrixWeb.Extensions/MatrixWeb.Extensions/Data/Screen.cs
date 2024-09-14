using System.Drawing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace MatrixWeb.Extensions.Data;
public record Screen(Image<Rgb24> Image, TimeSpan ScreenTime) {
    public static Screen Empty => new(new Image<Rgb24>(1, 1), new TimeSpan(0));
}