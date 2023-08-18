using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MatrixWeatherDisplay.Data;
using MatrixWeb.Extensions.Services.Loader;
using System;

namespace MatrixWeatherDisplay.Services.IconLoader;
public class ErrorIconLoader : IconLoader<ErrorType> {
    protected override string p_directory { get; } = "Icons/Errors";

    protected override Dictionary<string, ErrorType> p_files { get; } = new Dictionary<string, ErrorType>() {
        {"NoInternet.gif", ErrorType.NoInternet}
    };
}

public enum ErrorType {
    NoInternet
}