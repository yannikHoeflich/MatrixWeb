using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MatrixWeatherDisplay.Services;
using System;

namespace MatrixWeatherDisplay.Data;
public class RedSettings {
    private static readonly int s_steps = 2000;
    private static readonly TimeSpan s_step = TimeSpan.FromDays(1.0 / s_steps);
    private static readonly TimeOnly s_midnight = new(0, 0);
    public double RedThreshold { get; set; }

    private readonly BrightnessService _brightnessService;

    public RedSettings(BrightnessService brightnessService) {
        _brightnessService = brightnessService;
    }

    public bool ShouldBeRed(BrightnessPair brightness) => brightness.Display <= RedThreshold;

    public TimeRange GetRedTimeRange() {
        if (RedThreshold <= BrightnessService.MinBrightness) {
            return TimeRange.Never;
        }

        bool autoBrightnessActivated = _brightnessService.AutoBrightness;
        _brightnessService.AutoBrightness = true;

        double timeShift = _brightnessService.TimeShift;
        _brightnessService.TimeShift = 0;
        try {
            TimeOnly currentTime = s_midnight;

            int counter = 0;
            while (ShouldBeRed(_brightnessService.GetBrightness(currentTime, 1)) && counter < s_steps) {
                currentTime = AddTime(currentTime);
                counter++;
            }

            if(counter >= s_steps) {
                return TimeRange.Always;
            }

            TimeOnly end = currentTime;
            counter = 0;
            while (!ShouldBeRed(_brightnessService.GetBrightness(currentTime, 1)) && counter < s_steps) {
                currentTime = AddTime(currentTime);
                counter++;
            }

            if (counter >= s_steps) {
                return TimeRange.Always;
            }

            TimeOnly start = currentTime;

            return new TimeRange(start.AddHours(timeShift), end.AddHours(timeShift));
        } finally {
            _brightnessService.AutoBrightness = autoBrightnessActivated;
            _brightnessService.TimeShift = timeShift;
        }
    }

    private static TimeOnly AddTime(TimeOnly time) => time.Add(s_step);
}