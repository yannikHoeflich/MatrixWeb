using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace MatrixWeb.Extensions;
public interface IInitializable : IEnableable{
    public void Init();
}
