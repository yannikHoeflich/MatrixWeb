using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace MatrixWeb.Extensions.Data;
public class Config : IDictionary<string, string?> {
    private delegate bool DataGetter<T>(string input, IFormatProvider? formatProvider, out T output);
    private readonly Dictionary<string, string?> _data = new();

    public bool TryGetString(string key, out string? value) {
        if (_data.ContainsKey(key)) {
            value = _data[key];
            return true;
        }

        value = default;
        return false;
    }

    public bool TryGetInt(string key, out int value) => Get(key, int.TryParse, out value);
    public bool TryGetLong(string key, out long value) => Get(key, long.TryParse, out value);
    public bool TryGetDouble(string key, out double value) => Get(key, double.TryParse, out value);
    public bool TryGetGuid(string key, out Guid value) => Get(key, Guid.TryParse, out value);

    private bool Get<T>(string key, DataGetter<T> func, out T? value) {
        value = default;
        return _data.ContainsKey(key) && _data[key] is not null && func(_data[key], CultureInfo.InvariantCulture, out value);
    }

    public void Set(string key, object value) => _data[key] = value.ToString();


    ICollection<string> IDictionary<string, string?>.Keys => ((IDictionary<string, string?>)_data).Keys;
    ICollection<string?> IDictionary<string, string?>.Values => ((IDictionary<string, string?>)_data).Values;
    int ICollection<KeyValuePair<string, string?>>.Count => ((ICollection<KeyValuePair<string, string>>)_data).Count;
    bool ICollection<KeyValuePair<string, string?>>.IsReadOnly => ((ICollection<KeyValuePair<string, string>>)_data).IsReadOnly;

    string? IDictionary<string, string?>.this[string key] { get => _data[key]; set => _data[key] = value; }
    void IDictionary<string, string?>.Add(string key, string? value) => ((IDictionary<string, string?>)_data).Add(key, value);
    bool IDictionary<string, string?>.ContainsKey(string key) => ((IDictionary<string, string?>)_data).ContainsKey(key);
    bool IDictionary<string, string?>.Remove(string key) => ((IDictionary<string, string?>)_data).Remove(key);
    bool IDictionary<string, string?>.TryGetValue(string key, [MaybeNullWhen(false)] out string? value) => ((IDictionary<string, string?>)_data).TryGetValue(key, out value);
    void ICollection<KeyValuePair<string, string?>>.Add(KeyValuePair<string, string?> item) => ((ICollection<KeyValuePair<string, string?>>)_data).Add(item);
    void ICollection<KeyValuePair<string, string?>>.Clear() => ((ICollection<KeyValuePair<string, string?>>)_data).Clear();
    bool ICollection<KeyValuePair<string, string?>>.Contains(KeyValuePair<string, string?> item) => ((ICollection<KeyValuePair<string, string?>>)_data).Contains(item);
    void ICollection<KeyValuePair<string, string?>>.CopyTo(KeyValuePair<string, string?>[] array, int arrayIndex) => ((ICollection<KeyValuePair<string, string?>>)_data).CopyTo(array, arrayIndex);
    bool ICollection<KeyValuePair<string, string?>>.Remove(KeyValuePair<string, string?> item) => ((ICollection<KeyValuePair<string, string?>>)_data).Remove(item);
    IEnumerator<KeyValuePair<string, string?>> IEnumerable<KeyValuePair<string, string?>>.GetEnumerator() => ((IEnumerable<KeyValuePair<string, string?>>)_data).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_data).GetEnumerator();
}
