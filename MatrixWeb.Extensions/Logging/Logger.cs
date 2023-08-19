using MatrixWeb.Extensions.Data;
using Microsoft.Extensions.Logging;

namespace MatrixWeb.Extensions.Logging;
public class Logger : ILogger {
    private readonly string _name;
    private readonly Wrapped<LogLevel> _minLogLevel;
    private static readonly object s_lock = new();

    public Logger(string name, Wrapped<LogLevel> minLogLevel) {
        _name = name;
        _minLogLevel = minLogLevel;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default!;

    public bool IsEnabled(LogLevel logLevel) => logLevel >= _minLogLevel;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) {
        if (!IsEnabled(logLevel))
            return;

        lock (s_lock) {
            Console.Write($"[{DateTime.Now:G}, {_name}] ");
            Console.ForegroundColor = GetColor(logLevel);
            Console.Write($"{logLevel}: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{formatter(state, exception)}");
        }
    }

    private static ConsoleColor GetColor(LogLevel logLevel) => logLevel switch {
        LogLevel.Trace => ConsoleColor.Gray,
        LogLevel.Debug => ConsoleColor.White,
        LogLevel.Information => ConsoleColor.Green,
        LogLevel.Warning => ConsoleColor.Yellow,
        LogLevel.Error => ConsoleColor.Red,
        LogLevel.Critical => ConsoleColor.Red,
        LogLevel.None => ConsoleColor.White,
        _ => ConsoleColor.White
    };


    // public static ILogger Create<T>() => new Logger(typeof(T).Name, new Wrapped<LogLevel>(LogLevel.Information));
}
