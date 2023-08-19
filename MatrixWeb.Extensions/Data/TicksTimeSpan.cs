namespace MatrixWeatherDisplay.Data.Extensions;
public readonly struct TicksTimeSpan : IEquatable<TicksTimeSpan> {
    public long Ticks { get; }
    public double Seconds => Ticks / 1000.0;
    public double Minutes => Seconds / 60;
    public double Hours => Minutes / 60;
    public double Days => Hours / 24;

    public static explicit operator TimeSpan(TicksTimeSpan ticksTime) => TimeSpan.FromMilliseconds(ticksTime.Ticks);
    public static explicit operator TicksTimeSpan(TimeSpan timespan) => FromTimeSpan(timespan);

    public static TicksTimeSpan operator +(TicksTimeSpan left, TicksTimeSpan right) => new(left.Ticks + right.Ticks);
    public static TicksTimeSpan operator -(TicksTimeSpan left, TicksTimeSpan right) => new(left.Ticks - right.Ticks);

    public static bool operator >(TicksTimeSpan left, TicksTimeSpan right) => left.Ticks > right.Ticks;
    public static bool operator <(TicksTimeSpan left, TicksTimeSpan right) => left.Ticks < right.Ticks;

    public static bool operator ==(TicksTimeSpan left, TicksTimeSpan right) => left.Equals(right);
    public static bool operator !=(TicksTimeSpan left, TicksTimeSpan right) => !(left == right);

    public static bool operator <= (TicksTimeSpan left, TicksTimeSpan right) => (left.Ticks <= right.Ticks);
    public static bool operator >= (TicksTimeSpan left, TicksTimeSpan right) => (left.Ticks <= right.Ticks);

    public TicksTimeSpan() : this(Environment.TickCount64) { }

    public TicksTimeSpan(TicksTimeSpan ticksTime) : this(ticksTime.Ticks) { }
    public TicksTimeSpan(long ticks) {
        Ticks = ticks;
    }

    public override bool Equals(object? obj) => obj is TicksTimeSpan span && Equals(span);
    public bool Equals(TicksTimeSpan other) => Ticks == other.Ticks;
    public override int GetHashCode() => HashCode.Combine(Ticks);

    public static TicksTimeSpan FromTimeSpan(TimeSpan timeSpan) => new((long)timeSpan.TotalMilliseconds);
    public override string? ToString() => Ticks.ToString();
}
