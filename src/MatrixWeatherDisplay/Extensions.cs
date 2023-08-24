using MatrixWeatherDisplay.Data.Converter;
using MatrixWeb.Extensions;
using MatrixWeb.Extensions.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MatrixWeatherDisplay;
public static class Extensions {
    public static async Task<byte[]> ToGifBytesAsync<T>(this Image<T> image) where T : unmanaged, IPixel<T> {
        ImageHelper.AdjustFrameLengths(image);
        var stream = new MemoryStream();
        await image.SaveAsGifAsync(stream);
        return stream.ToArray();
    }



    public static double TotalHours(this DateTime dateTime) => TotalHours(TimeOnly.FromDateTime(dateTime));
    public static double TotalHours(this TimeOnly dateTime) {
        double hours = dateTime.Hour;
        hours += dateTime.Minute / 60.0;
        hours += dateTime.Second / 60.0 / 60;

        return hours;
    }

    public static double MinutesOfHour(this DateTime dateTime) => MinutesOfHour(TimeOnly.FromDateTime(dateTime));
    public static double MinutesOfHour(this TimeOnly dateTime) {
        double minutes = dateTime.Minute;
        minutes += dateTime.Second / 60.0;

        return minutes;
    }

    public static void MoveServiceTo<T>(this IServiceProvider from, IServiceCollection to) where T : class {
        T? service = from.GetService<T>() ?? throw new InvalidOperationException("The requested service is not registered");
        to.AddSingleton(_ => service);
    }

    public static void MoveServiceTo<TService, TImplementation>(this IServiceProvider from, IServiceCollection to) where TService: class where TImplementation : class, TService {
        TImplementation? service = from.GetService<TImplementation>() ?? throw new InvalidOperationException("The requested service is not registered");
        to.AddSingleton<TService, TImplementation>(_ => service);
    }

    public static async Task SleepAsync(TimeSpan timeSpan, Func<bool> endFunc) {
        TicksTime ticks = TicksTime.Now;
        var ticksToWait = TicksTimeSpan.FromTimeSpan(timeSpan);

        await SleepUntilAsync(new Wrapped<TicksTime>(ticks + ticksToWait), endFunc);
    }

    public static async Task SleepUntilAsync(Wrapped<TicksTime> tickCount, Func<bool> endFunc) {
        while (TicksTime.Now < tickCount && !endFunc()) {
            await Task.Delay(1);
        }
    }

    public static async Task<bool> TimeoutAfterAsync(this Task task, TimeSpan timeout) {
        using var timeoutCancellationTokenSource = new CancellationTokenSource();
        Task completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
        if (completedTask == task) {
            timeoutCancellationTokenSource.Cancel();
            await task;  // Very important in order to propagate exceptions
            return true;
        } else {
            return false;
        }
    }

    public static async Task<TResult?> TimeoutAfterAsync<TResult>(this Task<TResult> task, TimeSpan timeout) {

        using var timeoutCancellationTokenSource = new CancellationTokenSource();
        Task completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
        if (completedTask == task) {
            timeoutCancellationTokenSource.Cancel();
            return await task;  // Very important in order to propagate exceptions
        } else {
            return default;
        }
    }

    public static IEnumerable<ServiceDescriptor> GetServiceTypesWithInterface<T>(this IServiceCollection services)
        => services.OfType<ServiceDescriptor>().Where(x => typeof(T).IsAssignableFrom(x.ImplementationType));
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
