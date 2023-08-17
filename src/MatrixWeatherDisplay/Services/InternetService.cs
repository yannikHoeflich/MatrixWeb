﻿using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using MatrixWeatherDisplay.Data;
using MatrixWeatherDisplay.Data.Extensions;
using System;

namespace MatrixWeatherDisplay.Services;
public class InternetService : IInitializable {
    private const string s_configName = "connection";
    private const string s_updateFrequencyName = "check-frequency";
    private const string s_timeoutName = "timeout";
    private const string s_hostToPingName = "host-to-ping";

    private TicksTimeSpan _updateFrequency = TicksTimeSpan.FromTimeSpan(TimeSpan.FromMinutes(1));
    private int _timeout = 500;
    private string _hostToPing = "google.com";

    private readonly ConfigService _configService;

    private TicksTime _lastCheckTime;
    private bool _lastCheckResult;


    private readonly Ping _pingSender = new();

    public bool IsEnabled => true;


    public InternetService(ConfigService configService) {
        _configService = configService;
    }

    public void Init() {
        var config = _configService.GetConfig(s_configName);
        if(config is null) {
            config = _configService.CreateConfig(s_configName);
        }

        if(config.TryGetDouble(s_updateFrequencyName, out double checkFrequency)) {
            _updateFrequency = TicksTimeSpan.FromTimeSpan(TimeSpan.FromMinutes(checkFrequency));
        } else {
            config.Set(s_updateFrequencyName, _updateFrequency);
        }

        if(config.TryGetInt(s_timeoutName, out int timeout)) {
            _timeout = timeout;
        } else {
            config.Set(s_timeoutName,  _timeout);
        }

        if(config.TryGetString(s_hostToPingName, out string? hostToPing) && hostToPing is not null) {
            _hostToPing = hostToPing;
        } else {
            config.Set(s_hostToPingName,  _hostToPing);
        }
    }

    public async Task<bool> HasInternetConnection() {
        if(TicksTime.Now - _lastCheckTime > _updateFrequency) {
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
            PingReply result = await _pingSender.SendPingAsync(_hostToPing, _timeout);
            return result.Status == IPStatus.Success;
        } catch {
            return false;
        }
    }

}