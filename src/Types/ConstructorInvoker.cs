using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibs.Types
{
    public delegate object ConstructorInvoker(params object[] parameters);

    public delegate T ConstructorInvoker<T>(params object[] parameters);
}
