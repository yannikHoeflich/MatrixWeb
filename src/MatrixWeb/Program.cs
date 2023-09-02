using MatrixWeatherDisplay;
using MatrixWeatherDisplay.DependencyInjection;
using MatrixWeatherDisplay.ScreenGenerators;
using MatrixWeatherDisplay.Screens;
using MatrixWeatherDisplay.Services;
using MatrixWeb.Extensions.Logging;
using MatrixWeb.Extensions.Services;
using MatrixWeb.Extensions.Services.Translation;
using MatrixWeb.Extensions.Weather.Services;
using MatrixWeb.Pages;
using MatrixWeb.Services;
using MatrixWeb.Shared.Inputs;

namespace MatrixWeb;
public static class Program {
    private static DisplayApplication? s_displayApp;
    private static WebApplication? s_webApp;

    public static async Task Main(string[] args) {
        var displayBuilder = new DisplayApplicationBuilder();
        displayBuilder.AddDefaultServices();

        displayBuilder.AddScreenGenerator<WeatherScreen>();
        displayBuilder.AddScreenGenerator<TemperatureScreen>();
        displayBuilder.AddScreenGenerator<TimeScreen>();
        displayBuilder.AddScreenGenerator<SpotifyScreen>();
        displayBuilder.AddScreenGenerator<ErrorScreen>();

        displayBuilder.AddExtensions();

        s_displayApp = displayBuilder.Build();

        if(!await s_displayApp.InitDefaultServicesAsync()) {
            return;
        }

        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        s_displayApp.Services.MoveServiceTo<ConfigService>(builder.Services);
        s_displayApp.Services.MoveServiceTo<TextService>(builder.Services);
        s_displayApp.Services.MoveLogger(builder.Services);

        builder.Services.AddSingleton((p) => new DisplayService(s_displayApp, p.GetRequiredService<ILogger<DisplayService>>()));

        s_displayApp.Services.MoveServiceTo<SpotifyService>(builder.Services);
        s_displayApp.Services.MoveServiceTo<DeviceService>(builder.Services);
        s_displayApp.Services.MoveServiceTo<WeatherService>(builder.Services);
        s_displayApp.Services.MoveServiceTo<BrightnessService>(builder.Services);

        // Add services to the container.
        builder.Services.AddSingleton(_ => s_displayApp.ScreenGenerators);

        builder.Services.AddControllers();
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();

        s_webApp = builder.Build();

        ResetAll();
        s_webApp.Services.GetService<DisplayService>()?.Start();

        // Configure the HTTP request pipeline.
        if (!s_webApp.Environment.IsDevelopment()) {
            s_webApp.UseExceptionHandler("/Error");
        }


        s_webApp.UseStaticFiles();

        s_webApp.UseRouting();

        s_webApp.MapBlazorHub();
        s_webApp.MapControllers();
        s_webApp.MapFallbackToPage("/_Host");

        s_webApp.Run();
    }

    public static async Task RestartAsync() {
        if (s_displayApp is not null) {
            await s_displayApp.StopAsync();
        }

        if (s_webApp is not null) {
            await s_webApp.DisposeAsync();
        }

        ResetAll();

        _ = Main(Array.Empty<string>());
    }

    public static void ResetAll() {
        Settings.Reset();
        Pages.Index.Reset();

        SecondInput.Reset();
        HourInput.Reset();
        PercentInput.Reset();
    }
}
