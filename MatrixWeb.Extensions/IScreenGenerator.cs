using MatrixWeb.Extensions.Data;

namespace MatrixWeb.Extensions;
public interface IScreenGenerator : IEnableable{
    public string Name { get; }
    public string Description { get; }
    public bool RequiresInternet { get; }
    public TimeSpan ScreenTime { get; set; }

    public Task<Screen> GenerateImageAsync();

}
