using System.Reflection;

using MatrixWeatherDisplay.Data.Converter;
using MatrixWeatherDisplay.DependencyInjection.ScreenGeneratorCollections;
using MatrixWeatherDisplay.Screens;
using MatrixWeatherDisplay.Services;
using MatrixWeatherDisplay.Services.IconLoader;
using MatrixWeatherDisplay.Services.SensorServices;
using MatrixWeb.Extensions;
using MatrixWeb.Extensions.Logging;
using MatrixWeb.Extensions.Services;
using MatrixWeb.Extensions.Weather.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

namespace MatrixWeatherDisplay.DependencyInjection;
public partial class DisplayApplicationBuilder {
    public IServiceCollection Services { get; private set; }

    public DisplayApplicationBuilder() {
        Services = new ServiceCollection();

        Services.AddSingleton<ConfigService>();
        Services.AddLogger();

        Services.AddSingleton<BrightnessService>();
        Services.AddSingleton<DeviceService>();

        Services.AddSingleton<ILoggerProvider, Provider>();
    }

    public void AddScreenGenerator<T>() where T : class, IScreenGenerator => Services.AddSingleton<IScreenGenerator, T>();
    public void AddScreenGenerator(Type t) {
        if (!typeof(IScreenGenerator).IsAssignableFrom(t) || !t.IsClass) {
            throw new InvalidOperationException("Every screen generator must implement the IScreenGenerator interface and must be a class.");
        }

        Services.AddSingleton(typeof(IScreenGenerator), t);
    }

    public void AddDefaultServices() {
        Services.AddSingleton<ColorHelper>();
        Services.AddSingleton<SymbolLoader>();
        Services.AddSingleton<SensorService>();
        Services.AddSingleton<RoomHumidityService>();
        Services.AddSingleton<OpenWeatherMapClient>();
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

        ServiceDescriptor[] initServices = Services.GetServiceTypesWithInterface<IInitializable>().ToArray();
        ServiceDescriptor[] initAsyncServices = Services.GetServiceTypesWithInterface<IAsyncInitializable>().ToArray();

        var app = new DisplayApplication(serviceProvider, screenGeneratorProvider) {
            Initializables = initServices,
            AsyncInitializables = initAsyncServices
        };

        return app;
    }
}
