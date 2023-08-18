using MatrixWeatherDisplay.Data;
using MatrixWeatherDisplay.Logging;
using MatrixWeb.Extensions.Services;
using Microsoft.Extensions.Logging;
using UnitsNet.Units;
using USM.Devices;

namespace MatrixWeatherDisplay.Services;
public class DeviceService: IService {
    private readonly List<MatrixDevice> _devices = new();

    public BrightnessPair Brightness { get; private set; }
    public bool AutoBrightness { 
        get => _brightnessService.AutoBrightness;
        set {
            _brightnessService.AutoBrightness = value;
            _ = UpdateBrightnessAsync();
        } 
    }

    private readonly ILogger _logger = Logger.Create<DeviceService>();
    private readonly BrightnessService _brightnessService;

    public DeviceService(BrightnessService autoBrightness) {
        _brightnessService = autoBrightness;
        Brightness = new BrightnessPair(1, _brightnessService.GeneralBrightness);
    }
    

    public async Task ScanAsync() {
        _logger.LogInformation("Searching for matrix devices");
        while (_devices.Count == 0) {
            _logger.LogDebug("Scanning . . .");
            await foreach (IDevice device in Device.ScanAsync()) {
                if (device is not MatrixDevice matrixDevice) {
                    continue;
                }

                if (_devices.Exists(x => x.Id == matrixDevice.Id)) {
                    continue;
                }

                await matrixDevice.InitAsync();
                await matrixDevice.SendBrightnessAsync(Brightness.Real);
                _devices.Add(matrixDevice);
            }
        }

        _logger.LogInformation("Found {deviceCount} devices", _devices.Count);
    }

    public async Task SendGifAsync(byte[] gifBytes) {
        if (AutoBrightness) {
            await UpdateBrightnessAsync();
        }

        await PerformForAll(d => d.SendGifAsync(gifBytes));
    }

    public async Task SetBrightnessAsync(double newBrightness) {
        BrightnessPair brightness = _brightnessService.GetBrightness(newBrightness);
        Brightness = brightness;
        await PerformForAll(d => d.SendBrightnessAsync(brightness.Real));
    }

    public async Task UpdateBrightnessAsync() {
        BrightnessPair brightness = _brightnessService.GetBrightness(Brightness.Display);
        Brightness = brightness;
        await PerformForAll(d => d.SendBrightnessAsync(brightness.Real));
    }

    private async Task PerformForAll(Func<MatrixDevice, Task> func) => await Task.WhenAll(_devices.Select(func));
}
