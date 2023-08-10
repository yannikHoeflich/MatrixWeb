namespace MatrixWeatherDisplay;
public class Wrapped<T> where T : struct {
    public static implicit operator Wrapped<T>(T val) => new(val);
    public static implicit operator T(Wrapped<T> wrapped) => wrapped.Value;

    public T Value { get; set; }

    public Wrapped(T value) {
        Value = value;
    }
}
