using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace MatrixWeatherDisplay.Services;
public interface IEnableable {
    public bool IsEnabled { get; }
}
