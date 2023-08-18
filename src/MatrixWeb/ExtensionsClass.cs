using System.Globalization;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Server.IIS.Core;

namespace MatrixWeb;
public static class ExtensionsClass {
    public static bool ToDouble(this ChangeEventArgs eventArgs, out double value) {
        value = 0;

        if (eventArgs.Value is string str) {
            if (!double.TryParse(str, CultureInfo.InvariantCulture, out value)) {
                return false;
            }
        } else {
            if (eventArgs.Value is not double d) {
                return false;
            }

            value = d;
        }

        return true;
    }

    public static bool ToInt(this ChangeEventArgs eventArgs, out int value) {
        value = 0;

        if (eventArgs.Value is string str) {
            if (!int.TryParse(str, CultureInfo.InvariantCulture, out value)) {
                return false;
            }
        } else {
            if (eventArgs.Value is not int i) {
                return false;
            }

            value = i;
        }

        return true;
    }

    public static string ToInputString(this double value) => value.ToString(CultureInfo.InvariantCulture);

    public static string ToInputString(this double value, int decimals) {
        double multiplier = Math.Pow(10, decimals);
        value = Math.Round(value * multiplier) / multiplier;

        return ToInputString(value);
    }

    public static string ToInputString(this double value, int decimals, double multiplier) => ToInputString(value * multiplier, decimals);

    public static double MinMax(this double value, double min, double max) {
        if (value < min) {
            return min;
        }

        if (value > max) {
            return max;
        }

        return value;
    }
}
