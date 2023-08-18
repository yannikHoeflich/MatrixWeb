using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MatrixWeatherDisplay.Services;
using MatrixWeb.Extensions;
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

    private async Task LogIfSkippedLastAsync(int skips, IScreenGenerator? screenGenerator) {
        if (skips > 0) {
            _logger.LogDebug("Skipping screen '{screenName}'", screenGenerator?.GetType().Name);
        }

        if (skips > ScreenGenerators.ScreenGeneratorCount) {
            _logger.LogInformation("Skipped all screen generators, waiting 30 Seconds to retry");
            await Extensions.SleepAsync(TimeSpan.FromSeconds(30), () => !_shouldRun);
        }
    }
}
