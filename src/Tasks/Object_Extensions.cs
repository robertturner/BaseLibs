using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibs.Tasks
{
    public static class Object_Extensions
    {
        public static Task AsGenericTaskResult(this object result, Type type)
        {
            var tcs = new TaskCompletionSourceGeneric(type);
            tcs.SetResult(result);
            return tcs.Task;
        }
    }
}
