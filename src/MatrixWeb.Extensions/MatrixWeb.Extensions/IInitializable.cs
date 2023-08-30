using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MatrixWeb.Extensions.Data.Config;
using System;

namespace MatrixWeb.Extensions;
public interface IInitializable : IEnableable{
    public ConfigLayout ConfigLayout { get; }
    public InitResult Init();
}
