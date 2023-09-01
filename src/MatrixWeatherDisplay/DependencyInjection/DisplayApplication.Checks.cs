using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MatrixWeatherDisplay.Services;
using MatrixWeb.Extensions;
using MatrixWeb.Extensions.Data.Config;
using MatrixWeb.Extensions.Services.Translation;
using Microsoft.Extensions.Logging;
using System;

namespace MatrixWeatherDisplay.DependencyInjection;
public partial class DisplayApplication {
    private static async Task<bool> ShouldScreenGeneratorSkipAsync(InternetService? internetService, IScreenGenerator? screenGenerator)
        => screenGenerator is null ||
           screenGenerator.ScreenTime <= TimeSpan.Zero ||
           !screenGenerator.IsEnabled ||
           await HasInternetErrorAsync(screenGenerator, internetService);

    private static async Task<bool> HasInternetErrorAsync(IScreenGenerator screenGenerator, InternetService? internetService) {
        return internetService is not null &&
               screenGenerator.RequiresInternet &&
               !await internetService.HasInternetConnection();
    }

    private async Task<bool> LogIfSkippedLastAsync(int skips, IScreenGenerator? screenGenerator) {
        if (skips > 0) {
            _logger.LogDebug("Skipping screen '{screenName}'", screenGenerator?.GetType().Name);
        }

        if (skips > ScreenGenerators.ScreenGeneratorCount) {
            _logger.LogInformation("Skipped all screen generators, waiting 30 Seconds to retry");
            await Extensions.SleepAsync(TimeSpan.FromSeconds(30), () => !_shouldRun);
            return true;
        }

        return false;
    }


    private bool HandleInitResult(IInitializable service, InitResult result) => HandleInitResult(GetName(service), result);
    private bool HandleInitResult(IAsyncInitializable service, InitResult result) => HandleInitResult(GetName(service), result);

    private bool HandleInitResult(string name, InitResult result) {
        if (result == InitResult.Success) {
            return false;
        }

        if (result.ResultType == InitResultType.Warning) {
            _logger.LogInformation("Warning initializing {serviceName}: {message}", name, result.Message.GetText(LanguageCode.EN));
            return false;
        }
        if (result.ResultType == InitResultType.Error) {
            _logger.LogWarning("Error initializing {serviceName}: {message}", name, result.Message.GetText(LanguageCode.EN));
            return false;
        }

        if (result.ResultType == InitResultType.Critical) {
            _logger.LogCritical("Critical error initializing {serviceName}: {message}", name, result.Message.GetText(LanguageCode.EN));
            _logger.LogCritical("Stopping!");
            _ = StopAsync();
            return true;
        }

        _logger.LogInformation("Unknown result from initializing {serviceName}: {message}", name, result.Message.GetText(LanguageCode.EN));
        return false;
    }

    private string GetName(IAsyncInitializable service) {
        return TryGetNameFromConfig(service.ConfigLayout, out string? name)
            ? name
            : service.GetType().Name;
    }

    private string GetName(IInitializable service) {
        return TryGetNameFromConfig(service.ConfigLayout, out string? name) 
            ? name 
            : service.GetType().Name;
    }

    private bool TryGetNameFromConfig(ConfigLayout configLayout, [NotNullWhen(true)] out string? name) {
        if(configLayout is not null && !string.IsNullOrWhiteSpace(configLayout.ConfigName)) {
            name = configLayout.ConfigName;
            return true;
        }

        name = null;
        return false;
    }
}
