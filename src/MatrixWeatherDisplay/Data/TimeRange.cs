using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace MatrixWeatherDisplay.Data;
public readonly struct TimeRange : IEquatable<TimeRange> {
    public static readonly TimeRange Never = new(TimeRangeStatus.Never);
    public static readonly TimeRange Always = new(TimeRangeStatus.Always);

    public static bool operator ==(TimeRange left, TimeRange right) => left.Equals(right);
    public static bool operator !=(TimeRange left, TimeRange right) => !(left == right);

    public TimeOnly Start { get; }
    public TimeOnly End { get; }

    private readonly TimeRangeStatus _status = 0;

    public TimeRange(TimeOnly start, TimeOnly end) : this(TimeRangeStatus.Normal) {
        Start = start;
        End = end;
    }

    private TimeRange(TimeRangeStatus status) {
        _status = status;
    }

    public bool Equals(TimeRange other) => (_status !=  TimeRangeStatus.Normal && _status == other._status) || (_status == TimeRangeStatus.Normal && Start.Equals(other.Start) && End.Equals(other.End));
    public override bool Equals(object? obj) => obj is TimeRange range && Equals(range);
    public override int GetHashCode() => HashCode.Combine(Start, End, _status);
    public override string? ToString() => _status switch {
        TimeRangeStatus.Never => "Never",
        TimeRangeStatus.Always => "Always",
        TimeRangeStatus.Normal or _ => $"{Start} - {End}",
    };


    private enum TimeRangeStatus {
        Normal,
        Never,
        Always
    }
}