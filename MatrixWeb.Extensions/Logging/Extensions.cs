using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using System;

namespace MatrixWeb.Extensions.Logging;
public static class Extensions {
    public static void AddLogger(this IServiceCollection services) => services.AddLogging(builder => {
        builder.ClearProviders();
        builder.AddConfiguration();
        builder.Services.AddSingleton<ILoggerProvider, Provider>();
        builder.SetMinimumLevel(LogLevel.Trace);
    });

    public static void MoveLogger(this IServiceProvider from, IServiceCollection services) {
        var provider = from.GetService<ILoggerProvider>() ?? throw new InvalidOperationException("coulnd't get the logger provider");
        services.AddLogging(builder => {
            builder.ClearProviders();
            builder.AddConfiguration();
            builder.Services.AddSingleton(_ => provider);
        });
    }
}
