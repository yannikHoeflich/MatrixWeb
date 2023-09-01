using MatrixWeatherDisplay.Data;
using MatrixWeb.Extensions;
using MatrixWeb.Extensions.Data;
using MatrixWeb.Extensions.Data.Config;
using MatrixWeb.Extensions.Services;
using Microsoft.Extensions.Logging;
using SpotifyAPI.Web;

namespace MatrixWeatherDisplay.Services;
public class SpotifyService : IInitializable, IService {
    private const string s_configName = "spotify";
    private const string s_clientIdName = "client-id";
    private const string s_clientSecretName = "client-secret";

    private readonly static ICollection<string> s_spotifyScopes = new List<string> { Scopes.UserReadCurrentlyPlaying };

    private readonly ConfigService _configService;

    private SpotifyClient? _client;

    private string? _clientId;
    private string? _clientSecret;

    private readonly HttpClient _httpClient = new();

    private readonly ILogger _logger;

    public bool IsConnected => _client is not null;

    public bool HasClientKeys => _clientId is not null && _clientSecret is not null;
    public bool IsEnabled => HasClientKeys && _client is not null;

    public ConfigLayout ConfigLayout { get; } = new() {
        ConfigName = s_configName,
        Keys = new ConfigKey[] {
            new ConfigKey(s_clientIdName, typeof(string)),
            new ConfigKey(s_clientSecretName, typeof(string))
        }
    };

    public SpotifyService(ConfigService configService, ILogger<SpotifyService> logger) {
        _configService = configService;
        _logger = logger;
    }

    public InitResult Init() {
        RawConfig? config = _configService.GetConfig(s_configName);

        if(config is null) {
            return InitResult.NoConfig();
        }

        config.TryGetString(s_clientIdName, out _clientId);
        config.TryGetString(s_clientSecretName, out _clientSecret);

        return HasClientKeys
            ? InitResult.Success
            : InitResult.NoConfigElements(s_clientIdName, s_clientSecretName);
    }

    public string GetSpotifyUrl(string baseUrl) {
        if(_clientId is null) {
            throw new InvalidOperationException("The service 'SpotifyService' should be initialized and get all values through the config, to be used!");
        }

        var request = new LoginRequest(new Uri(baseUrl), _clientId, LoginRequest.ResponseType.Code) {
            Scope = s_spotifyScopes
        };

        return request.ToUri().ToString();
    }

    public async Task AddTokenAsync(string code, string url) {
        if (_clientId is null || _clientSecret is null) {
            throw new InvalidOperationException("The service 'SpotifyService' should be initialized and get all values through the config, to be used!");
        }

        AuthorizationCodeTokenResponse tokenResponse = await new OAuthClient().RequestToken(
          new AuthorizationCodeTokenRequest(
            _clientId, _clientSecret, code, new Uri(url)
          )
        );


        SpotifyClientConfig config = SpotifyClientConfig
                        .CreateDefault()
                        .WithAuthenticator(new AuthorizationCodeAuthenticator(_clientId, _clientSecret, tokenResponse));

        config.HTTPClient.SetRequestTimeout(TimeSpan.FromMilliseconds(500));

        _client = new SpotifyClient(config);
    }

    public void Logout() {
        _client = null;
    }


    public async Task<CurrentlyPlaying?> GetCurrentStatusAsync() {
        if(_client is null) {
            return null;
        }

        Func<Task<CurrentlyPlaying?>> requestFunc = async () => await _client.Player.GetCurrentlyPlaying(new PlayerCurrentlyPlayingRequest());
        CurrentlyPlaying? response = await requestFunc.RetryAsync(5, _logger);
        return response;
    }

    public Task<Image<Rgb24>?> GetCoverAsync(FullTrack fullTrack) => GetCoverAsync(fullTrack.Album);

    public async Task<Image<Rgb24>?> GetCoverAsync(SimpleAlbum album) {
        SpotifyAPI.Web.Image? smallest = album.Images.MinBy(x => x.Width + x.Height);
        if(smallest is null) {
            return null;
        }

        byte[] imageBytes = await _httpClient.GetByteArrayAsync(smallest.Url);

        try {
            return SixLabors.ImageSharp.Image.Load<Rgb24>(imageBytes);
        } catch {
            return null;
        }
    }
}
