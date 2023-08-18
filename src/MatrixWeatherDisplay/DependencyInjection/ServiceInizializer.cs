using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MatrixWeb.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MatrixWeatherDisplay.DependencyInjection;
internal class ServiceInitializer {
    private readonly IServiceProvider _services;

    public ServiceInitializer(IServiceProvider services) {
        _services = services;
    }

    public async Task InitAsync<T>() where T : IAsyncInitializable => await InitAsync<T>(service => service.InitAsync());
    public async Task InitAsync<T>(Func<T, Task> action) {
        T? service = _services.GetService<T>();
        if (service is null) {
            return;
        }

        await action(service);
    }

    public void Init<T>() where T : IInitializable => Init<T>(service => service.Init());
    public void Init<T>(Action<T> action) {
        T? service = _services.GetService<T>();
        if (service is null) {
            return;
        }

        action(service);
    }

    public void Init(Type t) {
        if (_services.GetService(t) is not IInitializable service) {
            return;
        }

        service.Init();
    }

    public async Task InitAsync(Type t) {
        if (_services.GetService(t) is not IAsyncInitializable service) {
            return;
        }

        await service.InitAsync();
    }
}
