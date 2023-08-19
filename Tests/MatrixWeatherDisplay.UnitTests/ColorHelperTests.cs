using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MatrixWeatherDisplay.Data.Converter;
using MatrixWeb.Extensions.Services;
using SixLabors.ImageSharp;
using System;

namespace MatrixWeatherDisplay.UnitTests;
public class ColorHelperTests {
    private ColorHelper _colorHelper = new(new ConfigService());
    private Color _color;

    [Fact]
    public void Temperature() {
        _color = _colorHelper.MapTemperature(-20);
        Assert.Equal(Color.FromRgb(0, 0, 255), _color);

        _color = _colorHelper.MapTemperature(-10);
        Assert.Equal(Color.FromRgb(0, 0, 255), _color);

        _color = _colorHelper.MapTemperature(45);
        Assert.Equal(Color.FromRgb(255, 0, 0), _color);

        _color = _colorHelper.MapTemperature(55);
        Assert.Equal(Color.FromRgb(255, 0, 0), _color);
    }

    [Fact]
    public void Hour() {
        _color = _colorHelper.MapHour(0);
        Assert.Equal(Color.FromRgb(0, 0, 255), _color);

        _color = _colorHelper.MapHour(4);
        Assert.Equal(Color.FromRgb(255, 0, 255), _color);

        _color = _colorHelper.MapHour(8);
        Assert.Equal(Color.FromRgb(255, 0, 0), _color);

        _color = _colorHelper.MapHour(12);
        Assert.Equal(Color.FromRgb(255, 255, 0), _color);

        _color = _colorHelper.MapHour(16);
        Assert.Equal(Color.FromRgb(0, 255, 0), _color);

        _color = _colorHelper.MapHour(20);
        Assert.Equal(Color.FromRgb(0, 255, 255), _color);

        _color = _colorHelper.MapHour(24);
        Assert.Equal(Color.FromRgb(0, 0, 255), _color);
    }

    [Fact]
    public void Minute() {
        _color = _colorHelper.MapMinute(0);
        Assert.Equal(Color.FromRgb(255, 0, 0), _color);

        _color = _colorHelper.MapMinute(10);
        Assert.Equal(Color.FromRgb(255, 255, 0), _color);

        _color = _colorHelper.MapMinute(20);
        Assert.Equal(Color.FromRgb(0, 255, 0), _color);

        _color = _colorHelper.MapMinute(30);
        Assert.Equal(Color.FromRgb(0, 255, 255), _color);

        _color = _colorHelper.MapMinute(40);
        Assert.Equal(Color.FromRgb(0, 0, 255), _color);

        _color = _colorHelper.MapMinute(50);
        Assert.Equal(Color.FromRgb(255, 0, 255), _color);

        _color = _colorHelper.MapMinute(60);
        Assert.Equal(Color.FromRgb(255, 0, 0), _color);
    }

    [Fact]
    public void RoomHumidity() {
        _color = _colorHelper.MapRoomHumidity(50, 50);
        Assert.Equal(Color.FromRgb(0, 255, 0), _color);

        _color = _colorHelper.MapRoomHumidity(60, 50);
        Assert.Equal(Color.FromRgb(255, 0, 0), _color);

        _color = _colorHelper.MapRoomHumidity(55, 50);
        Assert.Equal(Color.FromRgb(255, 255, 0), _color);

        _color = _colorHelper.MapRoomHumidity(100, 100);
        Assert.Equal(Color.FromRgb(0, 255, 0), _color);

        _color = _colorHelper.MapRoomHumidity(90, 100);
        Assert.Equal(Color.FromRgb(0, 255, 0), _color);

        _color = _colorHelper.MapRoomHumidity(60, 20);
        Assert.Equal(Color.FromRgb(255, 0, 0), _color);
    }
}