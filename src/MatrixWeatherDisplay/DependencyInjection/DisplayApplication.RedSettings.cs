using MatrixWeatherDisplay.Data;
using MatrixWeatherDisplay.Services;

namespace MatrixWeatherDisplay.DependencyInjection;
public partial class DisplayApplication {
    public class RedSettings {
        private static readonly TimeSpan s_step = TimeSpan.FromDays(1.0 / 2_000);
        private static readonly TimeOnly s_midnight = new(0, 0);
        public double RedThreshold { get; set; }

        private readonly BrightnessService _brightnessService;

        public RedSettings(BrightnessService brightnessService) {
            _brightnessService = brightnessService;
        }

        public bool ShouldBeRed(BrightnessPair brightness) => brightness.Display <= RedThreshold;

        public TimeRange GetRedTimeRange() {
            if(RedThreshold == 0) {
                return new TimeRange(s_midnight, s_midnight);
            }

            bool autoBrightnessActivated = _brightnessService.AutoBrightness;
            _brightnessService.AutoBrightness = true;
            try {
                TimeOnly currentTime = s_midnight;
                do {
                    currentTime = AddTime(currentTime);
                } while (ShouldBeRed(_brightnessService.GetBrightness(currentTime, 1)) && currentTime > s_midnight);

                if (currentTime == s_midnight) {
                    return new TimeRange(s_midnight, s_midnight);
                }

                TimeOnly end = currentTime;
                while (!ShouldBeRed(_brightnessService.GetBrightness(currentTime, 1)) && currentTime > s_midnight) {
                    currentTime = AddTime(currentTime);
                }

                TimeOnly start = currentTime;
                return new TimeRange(start, end);
            } finally {
                _brightnessService.AutoBrightness = autoBrightnessActivated;
            }
        }

        private static TimeOnly AddTime(TimeOnly time) => time.Add(s_step);
    }
}
