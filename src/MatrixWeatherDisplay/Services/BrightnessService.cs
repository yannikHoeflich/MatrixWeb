using MatrixWeatherDisplay.Data;
using MatrixWeb.Extensions.Services;

namespace MatrixWeatherDisplay.Services;
public class BrightnessService : IService {
    private const double s_u = 15;
    private const double s_o = 3;

    public double TimeShift { get; set; }

    public double GeneralBrightness { get; set; } = 0.75;

    internal bool AutoBrightness { get; set; } = true;

    public BrightnessPair GetBrightness(double newBrightness) => GetBrightness(TimeOnly.FromDateTime(DateTime.Now), newBrightness);

    public BrightnessPair GetBrightness(TimeOnly time, double newBrightness) {
        if (!AutoBrightness) {
            return new BrightnessPair(newBrightness, newBrightness * GeneralBrightness);
        }

        double x = (time.TotalHours() + TimeShift) % 24;
        double factor = BrightnessFunction(x, s_u, s_o);

        if (factor > 1) {
            factor = 1;
        }

        if (factor < 0.01) {
            factor = 0.01;
        }

        double brightness = factor * GeneralBrightness;

        return new BrightnessPair(factor, brightness);
    }

    private double BrightnessFunction(double x, double u, double o) {
        double upper = x - u;
        upper *= upper;

        double lower = 2 * o * o;

        double exponent = -(upper / lower);

        double result = Math.Pow(Math.E, exponent);
        return result;
    }
}
