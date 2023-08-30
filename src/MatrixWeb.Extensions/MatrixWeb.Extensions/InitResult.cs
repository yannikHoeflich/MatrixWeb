using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace MatrixWeb.Extensions;
public record struct InitResult(InitResultType ResultType, string Message) {
    public static readonly InitResult Success = new(InitResultType.Success, "");

    public static InitResult Warning(string message) => new(InitResultType.Warning, message);
    public static InitResult Error(string message) => new(InitResultType.Error, message);
    public static InitResult Critical(string message) => new(InitResultType.Critical, message);

    public static InitResult NoConfig() => new (InitResultType.Error, "There is no config for this service, even thought its required");
    public static InitResult NoConfigElements(params string[] required) => new (InitResultType.Error, $"The config needs to contain: {string.Join(", ", required)}");
}

public enum InitResultType {
    Success,
    Warning,
    Error,
    Critical
}