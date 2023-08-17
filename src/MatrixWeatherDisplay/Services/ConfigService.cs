using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using MatrixWeatherDisplay.Data;
using System;

namespace MatrixWeatherDisplay.Services;
public class ConfigService{
    private const string s_configFile = "config.json";

    private Dictionary<string, Config?>? _configs;

    public async Task InitAsync() {
        if (!File.Exists(s_configFile)) {
            File.WriteAllText(s_configFile, "{\n\n}");
        }

        using FileStream stream = File.OpenRead(s_configFile);
        _configs = await JsonSerializer.DeserializeAsync<Dictionary<string, Config?>>(stream);
    }

    public Config? GetConfig(string name) {
        if(_configs is null) {
            return null;
        }

        if(!_configs.TryGetValue(name, out Config? config)) {
            return null;
        }

        return config;
    }

    public Config CreateConfig(string name) { 
        var newConfig = new Config();
        _configs.Add(name, newConfig);

        return newConfig;
    }

    public async Task SaveAsync() {
        FileStream fileStream = File.Create(s_configFile);
        await JsonSerializer.SerializeAsync(fileStream, _configs);
    }
}
