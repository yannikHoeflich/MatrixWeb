using MatrixWeatherDisplay.Screens;

namespace MatrixWeatherDisplay.DependencyInjection.ScreenGeneratorCollections;
public interface IScreenGeneratorProvider {
    IScreenGenerator? GetNextScreenGenerator();

    public T? GetScreenGenerator<T>() where T : IScreenGenerator;
    public IReadOnlyCollection<IScreenGenerator> GetScreenGenerators();
    public void Reset();
    public int ScreenGeneratorCount { get; }
}