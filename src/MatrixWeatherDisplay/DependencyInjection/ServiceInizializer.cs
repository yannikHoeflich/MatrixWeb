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

    public void Init<T>() where T : IInitializable => Init<T>(service => service.Init());
    public void Init<T>(Action<T> action) {
        T? service = _services.GetService<T>();
        if (service is null) {
            return;
        }

        action(service);
    }

    public void Init(ServiceDescriptor t) {
        var serviceRaw = _services.GetService(t.ServiceType);
        if (serviceRaw is not IInitializable service) {
            throw new InvalidOperationException("The type must implement the 'IInitializable' interface to be initializable by this method");
        }

        service.Init();
    }

    public async Task InitAsync<T>() where T : IAsyncInitializable => await InitAsync<T>(service => service.InitAsync());
    public async Task InitAsync<T>(Func<T, Task> action) {
        T? service = _services.GetService<T>();
        if (service is null) {
            return;
        }

        await action(service);
    }

    public async Task InitAsync(ServiceDescriptor t) {
        if (_services.GetService(t.ServiceType) is not IAsyncInitializable service) {
            throw new InvalidOperationException("The type must implement the 'IAsyncInitializable' interface to be initializable by this method");
        }

        await service.InitAsync();
    }
}
