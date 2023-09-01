using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

using MatrixWeatherDisplay.Data;
using MatrixWeatherDisplay.Services;
using MatrixWeb.Extensions;
using MatrixWeb.Extensions.Data.Config;
using MatrixWeb.Extensions.Services;
using MatrixWeb.Extensions.Services.Translation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace MatrixWeatherDisplay.DependencyInjection.Helper;
internal record struct ServiceInitializer(IServiceProvider Services, ImmutableArray<IInitializable> Initializables, ImmutableArray<IAsyncInitializable> AsyncInitializables) {
    public ILogger? Logger { private get; set; }

    public ServiceInitializer(IServiceProvider Services, IEnumerable<ServiceDescriptor> Initializables, IEnumerable<ServiceDescriptor> AsyncInitializables)
        : this(Services,
            Initializables.Select(x => x.GetService(Services)).OfType<IInitializable>().ToImmutableArray(),
            AsyncInitializables.Select(x => x.GetService(Services)).OfType<IAsyncInitializable>().ToImmutableArray()
          ) { }

    public async Task<bool> InitAllAsync() {
        ILogger<DisplayApplication>? newLogger = Services.GetService<ILogger<DisplayApplication>>();
        if (newLogger is not null) {
            Logger = newLogger;
        }

        foreach (IInitializable service in Initializables) {
            InitResult result = service.Init();
            if (HandleInitResult(service, result)) {
                return false;
            }
        }

        foreach (IAsyncInitializable service in AsyncInitializables) {
            InitResult result = await service.InitAsync();
            if (HandleInitResult(service, result)) {
                return false;
            }
        }

        return true;
    }

    public readonly IReadOnlyCollection<ConfigLayout> GetConfigLayouts() {
        var configLayouts = Initializables.Select(x => x.ConfigLayout).Concat(
                            AsyncInitializables.Select(x => x.ConfigLayout))
                            .Where(x => x != ConfigLayout.Empty && x.Keys.Length > 0)
                            .DistinctBy(x => x.Keys)
                            .ToImmutableArray();

        return configLayouts;
    }

    private readonly bool HandleInitResult(IInitializable service, InitResult result) => HandleInitResult(GetName(service), result);
    private readonly bool HandleInitResult(IAsyncInitializable service, InitResult result) => HandleInitResult(GetName(service), result);

    private readonly bool HandleInitResult(string name, InitResult result) {
        if (result == InitResult.Success) {
            return false;
        }

        if (result.ResultType == InitResultType.Warning) {
            Logger?.LogInformation("Warning initializing {serviceName}: {message}", name, result.Message.GetText(LanguageCode.EN));
            return false;
        }

        if (result.ResultType == InitResultType.Error) {
            Logger?.LogWarning("Error initializing {serviceName}: {message}", name, result.Message.GetText(LanguageCode.EN));
            return false;
        }

        if (result.ResultType == InitResultType.Critical) {
            Logger?.LogCritical("Critical error initializing {serviceName}: {message}", name, result.Message.GetText(LanguageCode.EN));
            Logger?.LogCritical("Stopping!");
            return true;
        }

        Logger?.LogInformation("Unknown result from initializing {serviceName}: {message}", name, result.Message.GetText(LanguageCode.EN));
        return false;
    }

    private static string GetName(IAsyncInitializable service) {
        return TryGetNameFromConfig(service.ConfigLayout, out string? name)
            ? name
            : service.GetType().Name;
    }

    private static string GetName(IInitializable service) {
        return TryGetNameFromConfig(service.ConfigLayout, out string? name)
            ? name
            : service.GetType().Name;
    }

    private static bool TryGetNameFromConfig(ConfigLayout configLayout, [NotNullWhen(true)] out string? name) {
        if (configLayout is not null && !string.IsNullOrWhiteSpace(configLayout.ConfigName)) {
            name = configLayout.ConfigName;
            return true;
        }

        name = null;
        return false;
    }
}
