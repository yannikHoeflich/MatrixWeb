using MatrixWeatherDisplay;
using MatrixWeatherDisplay.DependencyInjection;
using MatrixWeatherDisplay.ScreenGenerators;
using MatrixWeatherDisplay.Screens;
using MatrixWeatherDisplay.Services;
using MatrixWeb.Extensions.Logging;
using MatrixWeb.Extensions.Services;
using MatrixWeb.Extensions.Weather.Services;
using MatrixWeb.Services;

namespace MatrixWeb;
public static class Program {
    public static async Task Main(string[] args) {
        var displayBuilder = new DisplayApplicationBuilder();
        displayBuilder.AddDefaultServices();

        displayBuilder.AddScreenGenerator<WeatherScreen>();
        displayBuilder.AddScreenGenerator<TemperatureScreen>();
        displayBuilder.AddScreenGenerator<TimeScreen>();
        displayBuilder.AddScreenGenerator<SpotifyScreen>();
        displayBuilder.AddScreenGenerator<ErrorScreen>();

        displayBuilder.AddExtensions();

        DisplayApplication displayApp = displayBuilder.Build();

        if(!await displayApp.InitDefaultServicesAsync()) {
            return;
        }

        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        displayApp.Services.MoveServiceTo<ConfigService>(builder.Services);
        displayApp.Services.MoveLogger(builder.Services);

        builder.Services.AddSingleton((p) => new DisplayService(displayApp, p.GetService<ILogger<DisplayService>>()));

        displayApp.Services.MoveServiceTo<SpotifyService>(builder.Services);
        displayApp.Services.MoveServiceTo<DeviceService>(builder.Services);
        displayApp.Services.MoveServiceTo<WeatherService>(builder.Services);
        displayApp.Services.MoveServiceTo<BrightnessService>(builder.Services);

        // Add services to the container.
        builder.Services.AddSingleton(_ => displayApp.ScreenGenerators);

        builder.Services.AddControllers();
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();

        WebApplication app = builder.Build();

        app.Services.GetService<DisplayService>()?.Start();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment()) {
            app.UseExceptionHandler("/Error");
        }


        app.UseStaticFiles();

        app.UseRouting();

        app.MapBlazorHub();
        app.MapControllers();
        app.MapFallbackToPage("/_Host");

        app.Run();
    }
}
