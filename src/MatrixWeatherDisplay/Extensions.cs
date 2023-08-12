using MatrixWeatherDisplay.Data.Converter;
using MatrixWeatherDisplay.Data.Extensions;
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

    public static double MinutesOfHour(this DateTime dateTime) => MinutesOfHour(TimeOnly.FromDateTime(dateTime));

    public static double TotalHours(this TimeOnly dateTime) {
        double hours = dateTime.Hour;
        hours += dateTime.Minute / 60.0;
        hours += dateTime.Second / 60.0 / 60;

        return hours;
    }

    public static double MinutesOfHour(this TimeOnly dateTime) {
        double minutes = dateTime.Minute;
        minutes += dateTime.Second / 60.0;

        return minutes;
    }

    public static void MoveServiceTo<T>(this IServiceProvider from, IServiceCollection to) where T : class {
        T? service = from.GetService<T>() ?? throw new InvalidOperationException("The requested service is not registered");
        to.AddSingleton(_ => service);
    }

    public static async Task Sleep(TimeSpan timeSpan, Func<bool> endFunc) {
        TicksTime ticks = TicksTime.Now;
        var ticksToWait = TicksTimeSpan.FromTimeSpan(timeSpan);

        await SleepUntil(ticks + ticksToWait, endFunc);
    }

    public static async Task SleepUntil(Wrapped<TicksTime> tickCount, Func<bool> endFunc) {
        while (TicksTime.Now < tickCount && !endFunc()) {
            await Task.Delay(1);
        }
    }

    public static async Task<bool> TimeoutAfter(this Task task, TimeSpan timeout) {
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

    public static async Task<TResult?> TimeoutAfter<TResult>(this Task<TResult> task, TimeSpan timeout) {

        using var timeoutCancellationTokenSource = new CancellationTokenSource();
        Task completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
        if (completedTask == task) {
            timeoutCancellationTokenSource.Cancel();
            return await task;  // Very important in order to propagate exceptions
        } else {
            return default;
        }
    }

    public static async Task<TResult?> Retry<TResult>(Func<Task<TResult?>> createTask, int maxRetries, ILogger logger) {
        TResult? response = default;
        Exception? exception = null;
        int retries = 0;

        while (retries < maxRetries) {
            try {
                response = await createTask();
                break;
            } catch (Exception ex) {
                exception = ex;
                retries++;
            }
        }

        if (retries >= maxRetries) {
            logger.LogError("Couldn't get the data status in 5 tries!");
            logger.LogError("Last Exception: {exception}", exception);
            return default;
        }

        if (retries > 0) {
            logger.LogWarning("{retries} tries needed to get the current data", retries + 1);
        }


        return response;
    }
}
