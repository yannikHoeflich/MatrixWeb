using MatrixWeatherDisplay.Logging;
using Microsoft.Extensions.Logging;
using SpotifyAPI.Web;

namespace MatrixWeatherDisplay.Services;
public class SpotifyService {
    private readonly static ICollection<string> s_spotifyScopes = new List<string> { Scopes.UserReadCurrentlyPlaying };

    private SpotifyClient? _client;

    private readonly string _clientId;
    private readonly string _clientSecret;

    private readonly HttpClient _httpClient = new();

    private readonly ILogger _logger = Logger.Create<SpotifyService>();

    public bool IsConnected => _client is not null;

    public SpotifyService(string clientId, string accessToken) {
        _clientId = clientId;
        _clientSecret = accessToken;
    }

    public string GetSpotifyUrl(string baseUrl) {
        var request = new LoginRequest(new Uri(baseUrl), _clientId, LoginRequest.ResponseType.Code) {
            Scope = s_spotifyScopes
        };

        return request.ToUri().ToString();
    }

    public async Task AddToken(string code, string url) {

        AuthorizationCodeTokenResponse tokenResponse = await new OAuthClient().RequestToken(
          new AuthorizationCodeTokenRequest(
            _clientId, _clientSecret, code, new Uri(url)
          )
        );


        SpotifyClientConfig config = SpotifyClientConfig
                        .CreateDefault()
                        .WithAuthenticator(new AuthorizationCodeAuthenticator(_clientId, _clientSecret, tokenResponse));


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
        CurrentlyPlaying? response = await Extensions.Retry(requestFunc, 5, _logger);
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
