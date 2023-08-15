using MatrixWeatherDisplay.DependencyInjection.ScreenGeneratorCollections;
using MatrixWeatherDisplay.Logging;
using MatrixWeatherDisplay.Screens;
using MatrixWeatherDisplay.Services;
using MatrixWeatherDisplay.Services.IconLoader;
using MatrixWeatherDisplay.Services.SensorServices;
using MatrixWeatherDisplay.Services.Weather;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MatrixWeatherDisplay.DependencyInjection;
public class DisplayApplicationBuilder {
    public IServiceCollection Services { get; private set; }

    public DisplayApplicationBuilder() {
        Services = new ServiceCollection();

        Services.AddSingleton<BrightnessService>();
        Services.AddSingleton<DeviceService>();

        Services.AddSingleton<ILoggerProvider, Provider>();
    }

    public void AddScreenGenerator<T>() where T : class, IScreenGenerator => Services.AddSingleton<IScreenGenerator, T>();

    public void AddDefaultServices() {
        Services.AddSingleton<ConfigService>();
        Services.AddSingleton<SymbolLoader>();
        Services.AddSingleton<SensorService>();
        Services.AddSingleton<RoomHumidityService>();
        Services.AddSingleton<OpenWeatherMapClient>();
        Services.AddSingleton<GasPriceService>();
        Services.AddSingleton<SpotifyService>();
        Services.AddSingleton<InternetService>();

        Services.AddSingleton<WeatherApiClient>();
        Services.AddSingleton<WeatherService>();

        Services.AddSingleton<WeatherIconLoader>();
        Services.AddSingleton<ErrorIconLoader>();
    }

    public DisplayApplication Build() {
        ServiceProvider serviceProvider = Services.BuildServiceProvider();

        IEnumerable<IScreenGenerator> screenGenerators = serviceProvider.GetServices<IScreenGenerator>();
        var screenGeneratorProvider = new ScreenGeneratorProvider(screenGenerators.ToArray());

        return new DisplayApplication(serviceProvider, screenGeneratorProvider);
    }
}
