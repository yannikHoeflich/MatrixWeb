using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MatrixWeatherDisplay.Data.Converter;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using Xunit;

namespace MatrixWeatherDisplay.UnitTests;
public class ImageHelperTests {
    readonly Image<Rgb24> _image;
    readonly Random _random = new(420);

    public ImageHelperTests() {
        _image = new Image<Rgb24>(16, 16);

        byte[] color = new byte[3];
        for(int y = 0; y < _image.Height; y++) {
            for(int x = 0; x < _image.Width; x++) {
                _random.NextBytes(color);
                _image[x, y] = Color.FromRgb(color[0], color[1], color[2]);
            }
        }
    }

    [Fact]
    public void SetColor() {
        var color = Color.FromRgb(255, 0, 0);
        Rgb24 testPixelColor = color.ToPixel<Rgb24>();
        Rgb24 testPixelBlack = Color.Black.ToPixel<Rgb24>();
        ImageHelper.SetColor(_image, color);

        for (int y = 0; y < _image.Height; y++) {
            for (int x = 0; x < _image.Width; x++) {
                Rgb24 pixel = _image[x, y];
                if(pixel != testPixelColor && pixel != testPixelBlack) {
                    Assert.Fail($"Each pixel must be either {color} or black");
                }
            }
        }
    }
}
