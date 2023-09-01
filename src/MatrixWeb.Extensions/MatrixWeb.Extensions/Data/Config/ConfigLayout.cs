using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace MatrixWeb.Extensions.Data.Config;
public sealed class ConfigLayout : IEquatable<ConfigLayout?> {
    public required string ConfigName { get; init; }
    public required ConfigKey[] Keys { get; init; }

    public static readonly ConfigLayout Empty = new() {
        ConfigName = "",
        Keys = Array.Empty<ConfigKey>()
    };

    public override bool Equals(object? obj) => Equals(obj as ConfigLayout);
    public bool Equals(ConfigLayout? other) => other is not null && ConfigName == other.ConfigName && EqualityComparer<ConfigKey[]>.Default.Equals(Keys, other.Keys);
    public static bool operator ==(ConfigLayout? left, ConfigLayout? right) => EqualityComparer<ConfigLayout>.Default.Equals(left, right);
    public static bool operator !=(ConfigLayout? left, ConfigLayout? right) => !(left == right);

    public override int GetHashCode() => HashCode.Combine(ConfigName, Keys);
}
