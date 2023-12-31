﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MatrixWeatherDisplay.Data.Converter;
using MatrixWeb.Extensions.Services;
using SixLabors.ImageSharp;
using System;

namespace MatrixWeatherDisplay.UnitTests;
public class ColorHelperTests {
    private readonly ColorHelper _colorHelper = new(new ConfigService());
    private Color _color;

    [Fact]
    public void Temperature() {
        _color = ColorHelper.MapTemperature(-20);
        Assert.Equal(Color.FromRgb(0, 0, 255), _color);

        _color = ColorHelper.MapTemperature(-10);
        Assert.Equal(Color.FromRgb(0, 0, 255), _color);

        _color = ColorHelper.MapTemperature(45);
        Assert.Equal(Color.FromRgb(255, 0, 0), _color);

        _color = ColorHelper.MapTemperature(55);
        Assert.Equal(Color.FromRgb(255, 0, 0), _color);
    }

    [Fact]
    public void Hour() {
        _color = ColorHelper.MapHour(0);
        Assert.Equal(Color.FromRgb(0, 0, 255), _color);

        _color = ColorHelper.MapHour(4);
        Assert.Equal(Color.FromRgb(255, 0, 255), _color);

        _color = ColorHelper.MapHour(8);
        Assert.Equal(Color.FromRgb(255, 0, 0), _color);

        _color = ColorHelper.MapHour(12);
        Assert.Equal(Color.FromRgb(255, 255, 0), _color);

        _color = ColorHelper.MapHour(16);
        Assert.Equal(Color.FromRgb(0, 255, 0), _color);

        _color = ColorHelper.MapHour(20);
        Assert.Equal(Color.FromRgb(0, 255, 255), _color);

        _color = ColorHelper.MapHour(24);
        Assert.Equal(Color.FromRgb(0, 0, 255), _color);
    }

    [Fact]
    public void Minute() {
        _color = ColorHelper.MapMinute(0);
        Assert.Equal(Color.FromRgb(255, 0, 0), _color);

        _color = ColorHelper.MapMinute(10);
        Assert.Equal(Color.FromRgb(255, 255, 0), _color);

        _color = ColorHelper.MapMinute(20);
        Assert.Equal(Color.FromRgb(0, 255, 0), _color);

        _color = ColorHelper.MapMinute(30);
        Assert.Equal(Color.FromRgb(0, 255, 255), _color);

        _color = ColorHelper.MapMinute(40);
        Assert.Equal(Color.FromRgb(0, 0, 255), _color);

        _color = ColorHelper.MapMinute(50);
        Assert.Equal(Color.FromRgb(255, 0, 255), _color);

        _color = ColorHelper.MapMinute(60);
        Assert.Equal(Color.FromRgb(255, 0, 0), _color);
    }
}