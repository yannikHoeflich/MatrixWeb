using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MatrixWeb.Extensions.Data.Config;
using System;

namespace MatrixWeb.Extensions;
public interface IAsyncInitializable {
    public ConfigLayout ConfigLayout { get; }
    public Task<InitResult> InitAsync();
}
