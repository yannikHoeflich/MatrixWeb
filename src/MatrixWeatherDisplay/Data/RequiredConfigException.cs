using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using System;

namespace MatrixWeatherDisplay.Data;
public class RequiredConfigException : Exception {

    public RequiredConfigException(string sender, params string[] requiredParts) 
        : this($"The Service '{sender}' requires following parts to work: {string.Join(',', requiredParts)}") { }

    public RequiredConfigException() {
    }

    public RequiredConfigException(string? message) : base(message) {
    }

    public RequiredConfigException(string? message, Exception? innerException) : base(message, innerException) {
    }

    protected RequiredConfigException(SerializationInfo info, StreamingContext context) : base(info, context) {
    }
}
