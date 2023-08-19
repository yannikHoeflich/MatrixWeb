using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace MatrixWeb.Extensions.Data;
public class Wrapped<T> where T: struct {
    public static implicit operator T(Wrapped<T> wrapped) => wrapped.Value;

    public T Value { get; set; }

    public Wrapped() { }
    public Wrapped(T value) {
            Value = value;
    }

    public override bool Equals(object? obj) => Value.Equals(obj);
    public override int GetHashCode() => Value.GetHashCode();
    public override string? ToString() => Value.ToString();
}
