using MatrixWeatherDisplay;
using MatrixWeatherDisplay.DependencyInjection;
using MatrixWeatherDisplay.Logging;
using MatrixWeatherDisplay.ScreenGenerators;
using MatrixWeatherDisplay.Screens;
using MatrixWeatherDisplay.Services;
using MatrixWeatherDisplay.Services.Weather;
using MatrixWeb.Services;

namespace MatrixWeb;
public static class Program {
    public static async Task Main(string[] args) {
        var displayBuilder = new DisplayApplicationBuilder();
        displayBuilder.AddDefaultServices();

        displayBuilder.AddScreenGenerator<WeatherScreen>();
        displayBuilder.AddScreenGenerator<TemperatureScreen>();
        displayBuilder.AddScreenGenerator<TimeScreen>();
        displayBuilder.AddScreenGenerator<GasPriceScreen>();
        displayBuilder.AddScreenGenerator<RoomHumidityScreen>();
        displayBuilder.AddScreenGenerator<SpotifyScreen>();
        displayBuilder.AddScreenGenerator<ErrorScreen>();

        DisplayApplication displayApp = displayBuilder.Build();

        await displayApp.InitDefaultServicesAsync();

        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        builder.Services.AddSingleton(_ => new DisplayService(displayApp));


        builder.Logging.ClearProviders();
        builder.Logging.AddProvider(new Provider());

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
