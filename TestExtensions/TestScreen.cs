using MatrixWeb.Extensions;
using MatrixWeb.Extensions.Data;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace TestExtensions;
public class TestScreen : IScreenGenerator {
    private readonly TestService _service;

    public string Name { get; } = "Test Screen";

    public string Description { get; } = "Just a test";

    public bool RequiresInternet { get; } = false;

    public TimeSpan ScreenTime { get; set; } = TimeSpan.FromSeconds(5);

    public bool IsEnabled { get; } = true;

    public TestScreen(TestService service) {
        _service = service;
    }

    public Task<Screen> GenerateImageAsync() {
        Color color = _service.GetColor();
        var img = new Image<Rgb24>(16, 16, color);
        return Task.FromResult(new Screen(img, ScreenTime));
    }
}
