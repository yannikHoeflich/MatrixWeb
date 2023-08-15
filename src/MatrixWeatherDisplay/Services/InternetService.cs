using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

using MatrixWeatherDisplay.Data.Extensions;
using System;

namespace MatrixWeatherDisplay.Services;
public class InternetService {
    private static readonly TicksTimeSpan s_updateFrequency = TicksTimeSpan.FromTimeSpan(TimeSpan.FromMinutes(1));
    
    private TicksTime _lastCheckTime;
    private bool _lastCheckResult;


    private readonly Ping _pingSender = new();

    public async Task<bool> HasInternetConnection() {
        if(TicksTime.Now - _lastCheckTime > s_updateFrequency) {
            await Update();
        }

        return _lastCheckResult;
    }

    private async Task Update() {
        _lastCheckResult = await Ping();
        _lastCheckTime = TicksTime.Now;
    }

    private async Task<bool> Ping() {
        try {
            PingReply result = await _pingSender.SendPingAsync("google.com", 500);
            return result.Status == IPStatus.Success;
        } catch {
            return false;
        }
    }
}
