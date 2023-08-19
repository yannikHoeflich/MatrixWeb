using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MatrixWeb.Extensions;
using MatrixWeb.Extensions.Data;
using MatrixWeb.Extensions.Services;
using SixLabors.ImageSharp;
using System;

namespace TestExtensions;
public class TestService : IService, IInitializable {
    private const string s_configName = "test-extension";
    private const string s_minRedName = "min-red";

    private readonly ConfigService _configService;

    private byte _minRed = 0;

    public bool IsEnabled { get;  } = true;

    public TestService(ConfigService configService) {
        _configService = configService;
    }

    public Color GetColor() {
        byte[] color = new byte[3];
        Random.Shared.NextBytes(color);
        return Color.FromRgb(Math.Max(color[0], _minRed), color[1], color[2]);
    }

    public void Init() {
        Config config = _configService.GetConfig(s_configName) ?? _configService.CreateConfig(s_configName);

        if(config.TryGetByte(s_minRedName, out byte minRed)) {
            _minRed = minRed;
        } else {
            config.Set(s_minRedName, _minRed);
        }
    }
}
