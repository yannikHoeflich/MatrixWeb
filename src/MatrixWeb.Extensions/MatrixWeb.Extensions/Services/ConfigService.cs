﻿using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using MatrixWeb.Extensions.Data;
using MatrixWeb.Extensions.Data.Config;
using System;

namespace MatrixWeb.Extensions.Services;
public class ConfigService: IService{
    private const string s_configFile = "config.json";

    private Dictionary<string, RawConfig>? _configs;

    public IReadOnlyCollection<ConfigLayout> Layouts { get; private set; } = new List<ConfigLayout>(0);

    public async Task InitAsync(IReadOnlyCollection<ConfigLayout> layouts) {
        Layouts = layouts;

        if (!File.Exists(s_configFile)) {
            File.WriteAllText(s_configFile, "{\n\n}");
        }

        using FileStream stream = File.OpenRead(s_configFile);
        if (stream.Length > 0) {
            _configs = await JsonSerializer.DeserializeAsync<Dictionary<string, RawConfig>>(stream);
        }

        _configs ??= new Dictionary<string, RawConfig>();
    }

    public RawConfig GetOrCreateConfig(string name) => GetConfig(name) ?? CreateConfig(name);

    public RawConfig? GetConfig(string name) {
        return _configs is not null && _configs.TryGetValue(name, out RawConfig? config) 
            ? config 
            : null;
    }

    public RawConfig CreateConfig(string name) { 
        var newConfig = new RawConfig();

        if(_configs is null) {
            throw new InvalidOperationException("Initialize the 'Config Service' before using it");
        }

        _configs.Add(name, newConfig);

        return newConfig;
    }

    public async Task SaveAsync() {
        var configsToSave = _configs?.Where(x => x.Value.Any()).ToDictionary(x => x.Key, x => x.Value);
        using FileStream fileStream = File.Create(s_configFile);
        await JsonSerializer.SerializeAsync(fileStream, configsToSave, new JsonSerializerOptions() { WriteIndented = true});
    }
}
