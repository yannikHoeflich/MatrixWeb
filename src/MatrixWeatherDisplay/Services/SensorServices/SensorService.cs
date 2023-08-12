using MatrixWeatherDisplay.Logging;
using Microsoft.Extensions.Logging;
using USM.Devices;

namespace MatrixWeatherDisplay.Services.SensorServices;
public class SensorService
{
    private readonly List<SensorDevice> _devices = new();

    private readonly ILogger _logger = Logger.Create<SensorService>();

    public async Task ScanAsync()
    {
        _logger.LogInformation("Searching for sensor devices");
        while (_devices.Count == 0)
        {
            _logger.LogDebug("Scanning . . .");
            await foreach (IDevice device in Device.ScanAsync())
            {
                if (device is not SensorDevice sensorDevice)
                {
                    continue;
                }

                if (_devices.Exists(x => x.Id == sensorDevice.Id))
                {
                    continue;
                }

                await sensorDevice.InitAsync();
                _devices.Add(sensorDevice);
            }
        }

        _logger.LogInformation("Found {deviceCount} devices", _devices.Count);
    }

    public SensorDevice? GetDeviceBySuffix(string suffix) => _devices.Find(x => x.Suffix == suffix);

    public async Task<double?> GetValueBySuffixAsync(string suffix)
    {
        SensorDevice? device = GetDeviceBySuffix(suffix);
        if (device is null)
        {
            return null;
        }

        try
        {
            return await device.GetValueAsync(TimeSpan.FromSeconds(1));
        }
        catch
        {
            _logger.LogWarning("Value request timed out");
            return null;
        }
    }
}
