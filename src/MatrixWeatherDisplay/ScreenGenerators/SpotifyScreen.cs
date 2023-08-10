using MatrixWeatherDisplay.Data;
using MatrixWeatherDisplay.Data.Converter;
using MatrixWeatherDisplay.Screens;
using MatrixWeatherDisplay.Services;
using SpotifyAPI.Web;

namespace MatrixWeatherDisplay.ScreenGenerators;
public class SpotifyScreen : IScreenGenerator {
    private readonly SpotifyService _spotify;

    public string Name { get; } = "Spotify";

    public string Description { get; } = "Zeigt das Cover des Songs, den du aktuell abspielst. Wird übersprungen falls du momentan kein Spotify hörst.";

    public TimeSpan ScreenTime { get; set; } = TimeSpan.FromSeconds(10);


    public SpotifyScreen(SpotifyService spotify) {
        _spotify = spotify;
    }

    public async Task<Screen> GenerateImageAsync() {
        if(!_spotify.IsConnected) {
            return Screen.Empty;
        }

        Image<Rgb24>? cover = await GetCurrentCoverAsync();

        if(cover is null) {
            return Screen.Empty;
        }

        var image = new Image<Rgb24>(16, 16);

        ImageHelper.ScaleDown(cover, image);

        return new Screen(image, ScreenTime);
    }

    private async Task<Image<Rgb24>?> GetCurrentCoverAsync() {
        CurrentlyPlaying? status = await _spotify.GetCurrentStatusAsync();

        if (status is null || !status.IsPlaying) {
            return null;
        }

        Image<Rgb24>? cover = null;
        if (status.Item is FullTrack track) {
            cover = await _spotify.GetCoverAsync(track);
        }

        if (cover is null) {
            return null;
        }

        return cover;
    }
}
