namespace MatrixWeb.Extensions.Data;
public readonly struct TicksTime : IEquatable<TicksTime> {
    public static TicksTime MaxValue { get; } = new TicksTime(long.MaxValue);
    public static TicksTime MinValue { get; } = new TicksTime(long.MinValue);
    public static TicksTime Zero { get; } = new TicksTime(0);

    public long Ticks { get; }

    public static TicksTimeSpan operator -(TicksTime left, TicksTime right) => new(left.Ticks - right.Ticks);
    public static TicksTime operator +(TicksTime left, TicksTimeSpan right) => new(left.Ticks + right.Ticks);
    public static bool operator >(TicksTime left, TicksTime right) => left.Ticks > right.Ticks;
    public static bool operator <(TicksTime left, TicksTime right) => left.Ticks < right.Ticks;
    public static bool operator ==(TicksTime left, TicksTime right) => left.Equals(right);
    public static bool operator !=(TicksTime left, TicksTime right) => !(left == right);

    public TicksTime() : this(Environment.TickCount64) { }

    public TicksTime(TicksTime ticksTime) : this(ticksTime.Ticks){ }
    public TicksTime(long ticks) {
        Ticks = ticks;
    }

    public static TicksTime Now => new(Environment.TickCount64);


    public override bool Equals(object? obj) => obj is TicksTime time && Equals(time);
    public bool Equals(TicksTime other) => Ticks == other.Ticks;
    public override int GetHashCode() => HashCode.Combine(Ticks);
}
