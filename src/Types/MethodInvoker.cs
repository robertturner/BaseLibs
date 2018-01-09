using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibs.Types
{
    public delegate object MethodInvoker(object instance, params object[] args);

    public delegate object MethodInvoker<TInst>(TInst instance, params object[] args);
}
