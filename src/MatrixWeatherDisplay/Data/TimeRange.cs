using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace MatrixWeatherDisplay.Data;
public record struct TimeRange(TimeOnly Start, TimeOnly End);