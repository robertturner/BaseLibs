using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibs.Tasks
{
    public static class Exception_Extensions
    {
        public static Task AsGenericTaskException(this Exception exception, Type type)
        {
            var tcs = TaskCompletionSourceGeneric.Create(type);
            tcs.SetException(exception);
            return tcs.Task;
        }
    }
}
