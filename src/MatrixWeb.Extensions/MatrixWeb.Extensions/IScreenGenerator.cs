using MatrixWeb.Extensions.Data;
using MatrixWeb.Extensions.Services.Translation;

namespace MatrixWeb.Extensions;
public interface IScreenGenerator : IEnableable{
    public Text Name { get; }
    public Text Description { get; }
    public bool RequiresInternet { get; }
    public TimeSpan ScreenTime { get; set; }

    public Task<Screen> GenerateImageAsync();

}
