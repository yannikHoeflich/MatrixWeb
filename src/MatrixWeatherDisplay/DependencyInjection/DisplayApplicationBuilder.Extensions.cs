using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MatrixWeb.Extensions;
using MatrixWeb.Extensions.Services;
using McMaster.NETCore.Plugins;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MatrixWeatherDisplay.DependencyInjection;
public partial class DisplayApplicationBuilder {
    private const string s_extensionFolderName = "extensions";

    public void AddExtensions() {
        string pluginsDir = Path.Combine(AppContext.BaseDirectory, s_extensionFolderName);

        if (!Directory.Exists(pluginsDir)) {
            Directory.CreateDirectory(pluginsDir);
            return;
        }

        List<PluginLoader> loaders = LoadLoaders(pluginsDir);
        LoadTypesFromLoaders(loaders);
    }

    private void LoadTypesFromLoaders(List<PluginLoader> loaders) {
        foreach (PluginLoader loader in loaders) {
            GetTypes(loader, out IEnumerable<Type> serviceTypes, out IEnumerable<Type> screenGeneratorTypes);

            foreach (Type type in serviceTypes) {
                Services.AddSingleton(type);
            }

            foreach (Type type in screenGeneratorTypes) {
                AddScreenGenerator(type);
            }
        }
    }

    private static void GetTypes(PluginLoader loader, out IEnumerable<Type> serviceTypes, out IEnumerable<Type> screenGeneratorTypes) {
        IEnumerable<Type> pluginTypes = loader.LoadDefaultAssembly().GetTypes().Where(t => !t.IsAbstract && !t.IsInterface);
        serviceTypes = pluginTypes.Where(t => typeof(IService).IsAssignableFrom(t));
        screenGeneratorTypes = pluginTypes.Where(t => typeof(IService).IsAssignableFrom(t));
    }

    private static List<PluginLoader> LoadLoaders(string pluginsDir) {
        var loaders = new List<PluginLoader>();
        foreach (string dir in Directory.GetDirectories(pluginsDir)) {
            string dirName = Path.GetFileName(dir);
            string pluginDll = Path.Combine(dir, dirName + ".dll");
            if (File.Exists(pluginDll)) {
                var loader = PluginLoader.CreateFromAssemblyFile(pluginDll, sharedTypes: new[] { typeof(IService), typeof(IScreenGenerator) });
                loaders.Add(loader);
            }
        }

        return loaders;
    }
}
