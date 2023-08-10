using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using System;

namespace MatrixWeatherDisplay.Data;
public record struct BrightnessPair(double Visible, double Real);