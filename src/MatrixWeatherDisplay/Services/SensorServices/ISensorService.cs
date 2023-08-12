﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using USM.Devices;

namespace MatrixWeatherDisplay.Services.SensorServices;
public interface ISensorService : IEnableable {
    public string SensorSuffix { get; }
    public SensorService SensorService { get; }
}

public static class ISensorServiceExtensions {
    public static async Task<double> GetValueAsync(this ISensorService sensorService) {
        SensorDevice? device = sensorService.GetSensorDevice();
        return device is not null
            ? await device.GetValueAsync()
            : double.NaN;
    }

    public static SensorDevice? GetSensorDevice(this ISensorService sensorService) => sensorService.SensorService.GetDeviceBySuffix(sensorService.SensorSuffix);
}