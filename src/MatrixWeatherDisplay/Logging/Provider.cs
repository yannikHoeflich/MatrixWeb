using System.Collections.Concurrent;

using Microsoft.Extensions.Logging;

namespace MatrixWeatherDisplay.Logging;
public class Provider : ILoggerProvider {
    private bool _disposedValue;

    private readonly ConcurrentDictionary<string, Logger> _loggers =  new(StringComparer.OrdinalIgnoreCase);
    public ILogger CreateLogger(string categoryName) {
        if(_disposedValue)
            throw new ObjectDisposedException(GetType().FullName);

        return _loggers.GetOrAdd(categoryName, name => new Logger(name));
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
