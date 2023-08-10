using MatrixWeatherDisplay.Data;

namespace MatrixWeatherDisplay.Services;
public class BrightnessService {
    public double GeneralBrightness { get; set; } = 0.75;

    private const double s_a = -(99.0 / 138236000),
                         s_b = (7821.0 / 96765200),
                         s_c = -(3510837.0 / 967652000),
                         s_d = (15060573.0 / 241913000),
                         s_e = -(1226907.0 / 4319875),
                         s_f = (1.0 / 100);

    internal bool AutoBrightness { get; set; } = true;

    public BrightnessPair GetBrightness(double newBrightness) => GetBrightness(TimeOnly.FromDateTime(DateTime.Now), newBrightness);

    public BrightnessPair GetBrightness(TimeOnly time, double newBrightness) {
        if (!AutoBrightness) {
            return new BrightnessPair(newBrightness, newBrightness * GeneralBrightness);
        }

        double x = time.TotalHours();
        double factor = PolynomialFunction(x, s_a, s_b, s_c, s_d, s_e, s_f);

        if (factor > 1) {
            factor = 1;
        }

        if (factor < 0.01) {
            factor = 0.01;
        }

        double brightness = factor * GeneralBrightness;

        return new BrightnessPair(factor, brightness);
    }


    private static double PolynomialFunction(double x, params double[] values)
        => values.Reverse().Select((a, i) => a * Math.Pow(x, i)).Sum();
}
