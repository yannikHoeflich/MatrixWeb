using System.Collections.Concurrent;

using MatrixWeb.Extensions;
using MatrixWeb.Extensions.Data;
using MatrixWeb.Extensions.Data.Config;
using MatrixWeb.Extensions.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MatrixWeb.Extensions.Logging;
public class Provider : ILoggerProvider, IInitializable {
    private const string s_configName = "logger";
    private const string s_logLevelName = "min-log-level";

    private readonly ConfigService _configService;

    private bool _disposedValue;
    private readonly ConcurrentDictionary<string, Logger> _loggers = new(StringComparer.OrdinalIgnoreCase);

    private readonly Wrapped<LogLevel> _minLogLevel = new(LogLevel.None);

    public bool IsEnabled { get; } = true;


    public ConfigLayout ConfigLayout { get; } = new() {
        ConfigName = s_configName,
        Keys = new ConfigKey[] {
            new ConfigKey(s_logLevelName, typeof(string)),
        }
    };

    public Provider(ConfigService configService) {
        _configService = configService;
    }

    public InitResult Init() {
        RawConfig config = _configService.GetOrCreateConfig(s_configName);

        if(config.TryGetInt(s_logLevelName, out int logLevelInt)) {
            _minLogLevel.Value = (LogLevel)logLevelInt;
            return InitResult.Success;
        }

        if(config.TryGetString(s_logLevelName, out string? logLevelStr) && logLevelStr is not null && Enum.TryParse(logLevelStr, out LogLevel logLevel)){
            _minLogLevel.Value = logLevel;
        } else {
            _minLogLevel.Value = LogLevel.Information;
            config.Set(s_logLevelName, _minLogLevel);
        }

        return InitResult.Success;
    }

    public ILogger CreateLogger(string categoryName) {
        return _disposedValue
            ? throw new ObjectDisposedException(GetType().FullName)
            : (ILogger)_loggers.GetOrAdd(categoryName, name => new Logger(name.Split('.')[^1], _minLogLevel));
    }

    public ILogger CreateLogger<T>() => CreateLogger(typeof(T).Name);

    protected virtual void Dispose(bool disposing) {
        if (!_disposedValue) {
            if (disposing) {
                // cleanup managed
                _loggers.Clear();
            }
            // cleanup unmanaged

            _disposedValue = true;
        }
    }

    public void Dispose() {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}