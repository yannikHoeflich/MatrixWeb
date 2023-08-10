using MatrixWeatherDisplay.Screens;

namespace MatrixWeatherDisplay.DependencyInjection.ScreenGeneratorCollections;
internal class ScreenGeneratorProvider : IScreenGeneratorProvider {
    private readonly IScreenGenerator[] _screenGenerators;
    private int _currentIndex = 0;

    public int ScreenGeneratorCount => _screenGenerators.Length;

    public ScreenGeneratorProvider(IScreenGenerator[] screenGenerators) {
        _screenGenerators = screenGenerators;
    }

    public void Reset() {
        _currentIndex = 0;
    }

    public IScreenGenerator? GetNextScreenGenerator() {
        if (_screenGenerators.Length == 0)
            return null;

        if (_currentIndex >= _screenGenerators.Length)
            _currentIndex = 0;

        return _screenGenerators[_currentIndex++];
    }

    public T? GetScreenGenerator<T>() where T: IScreenGenerator {
        foreach (IScreenGenerator screenGenerator in _screenGenerators) {
            if(screenGenerator is T match) {
                return match;
            }
        }

        return default;
    }

    public IReadOnlyCollection<IScreenGenerator> GetScreenGenerators() 
        => _screenGenerators;
}
