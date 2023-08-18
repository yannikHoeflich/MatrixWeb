using System.Reflection;

using MatrixWeatherDisplay.Data.Converter;
using MatrixWeatherDisplay.DependencyInjection.ScreenGeneratorCollections;
using MatrixWeatherDisplay.Logging;
using MatrixWeatherDisplay.Screens;
using MatrixWeatherDisplay.Services;
using MatrixWeatherDisplay.Services.IconLoader;
using MatrixWeatherDisplay.Services.SensorServices;
using MatrixWeatherDisplay.Services.Weather;
using MatrixWeb.Extensions;
using MatrixWeb.Extensions.Services;
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
        Services.AddSingleton<ColorHelper>();
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

    public void AddExtensions() {

    }

    public DisplayApplication Build() {
        ServiceProvider serviceProvider = Services.BuildServiceProvider();

        IEnumerable<IScreenGenerator> screenGenerators = serviceProvider.GetServices<IScreenGenerator>();
        var screenGeneratorProvider = new ScreenGeneratorProvider(screenGenerators.ToArray());

        Type[] initServices = Services.GetServiceTypesWithInterface<IInitializable>().ToArray();
        Type[] initAsyncServices = Services.GetServiceTypesWithInterface<IInitializable>().ToArray();

        var app = new DisplayApplication(serviceProvider, screenGeneratorProvider) {
            Initializables = initServices,
            AsyncInitializables = initAsyncServices
        };

        return app;
    }
}
