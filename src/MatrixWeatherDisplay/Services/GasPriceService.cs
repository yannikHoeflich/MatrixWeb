using MatrixWeatherDisplay.Data;
using MatrixWeatherDisplay.Data.Extensions;
using MatrixWeatherDisplay.Logging;
using Microsoft.Extensions.Logging;
using Tankerkoenig.Net;
using Tankerkoenig.Net.Data;
using Tankerkoenig.Net.Results;

namespace MatrixWeatherDisplay.Services;
public partial class GasPriceService : IInitializable {
    private const int s_defaultDaysToSave = 14;

    private int _daysToSave = s_defaultDaysToSave;

    private readonly ConfigService _configService;

    private static readonly TicksTimeSpan s_updateFrequency = TicksTimeSpan.FromTimeSpan(TimeSpan.FromMinutes(5.1));
    private TankerkoenigClient _client;

    private double _price;
    private TicksTime _lastUpdate;

    private readonly ILogger _logger = Logger.Create<GasPriceService>();

    private MinMax[] _minMaxValues = new MinMax[s_defaultDaysToSave];

    public double MaxPrice => _minMaxValues.Where(x => x != default).Max(x => x.Max);
    public double MinPrice => _minMaxValues.Where(x => x != default).Min(x => x.Min);

    public bool IsEnabled {get; private set;}

    public GasPriceService(ConfigService configService) {
        _configService = configService;
    }

    public void Init() {
        var config = _configService.GetConfig("tanker-koenig");

        if (config is null || !config.TryGetString("api-key", out string? apiKey) || apiKey is null) {
            IsEnabled = false;
            return;
        }

        if (config.TryGetInt("days-to-save", out int daysToSave)) {
            _daysToSave = daysToSave;
            _minMaxValues = new MinMax[_daysToSave];
        }

        _client = new TankerkoenigClient(apiKey);
        IsEnabled = true;
    }

    private async Task UpdatePrice(double lat, double lon) {
        _logger.LogDebug("Updating Gas Price");
        Result<IReadOnlyList<Station>> stationsResult = await _client.ListStationsAsync(lat, lon, 3);
        if (!stationsResult.TryGetValue(out IReadOnlyList<Station>? stations) || stations is null) {
            return;
        }

        Station? cheapest = stations.MinBy(x => x.E10);
        if (cheapest is null) {
            return;
        }

        _price = cheapest.E10;
        _lastUpdate = TicksTime.Now;

        UpdateMinMaxPrice();
    }

    private void UpdateMinMaxPrice() {
        var now = DateTime.Now;
        int index = (int)(now - DateTime.UnixEpoch).TotalDays % _daysToSave;

        if (_minMaxValues[index] == default || _minMaxValues[index].Date != DateOnly.FromDateTime(now)) {
            _minMaxValues[index] = new MinMax() {
                Min = _price,
                Max = _price,
                Date = DateOnly.FromDateTime(now)
            };
            _logger.LogDebug("Current -  min: {Min} | max: {Max}", MinPrice, MaxPrice);
            return;
        }

        if (_minMaxValues[index].Min > _price) {
            _minMaxValues[index].Min = _price;
        }

        if (_minMaxValues[index].Max < _price) {
            _minMaxValues[index].Max = _price;
        }

        _logger.LogDebug("Current -  min: {Min} | max: {Max}", MinPrice, MaxPrice);
    }

    public async Task<double> GetCheapestPriceAsync(double lat, double lon) {
        if (TicksTime.Now - _lastUpdate > s_updateFrequency) {
            await UpdatePrice(lat, lon);
        }

        return _price;
    }
}
