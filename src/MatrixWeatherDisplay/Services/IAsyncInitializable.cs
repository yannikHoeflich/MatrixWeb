using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace MatrixWeatherDisplay.Services;
internal interface IAsyncInitializable {
    public Task InitAsync();
}
