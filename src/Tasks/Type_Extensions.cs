using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibs.Tasks
{
    public static class Type_Extensions
    {
        public static TaskCompletionSourceGeneric AsTCSGeneric(this Type type)
        {
            return new TaskCompletionSourceGeneric(type);
        }
    }
}
