using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibs.Tuples
{
    public static class ParameterInfo_Extensions
    {

        public static bool TryGetValueTupleTypes(this ParameterInfo pi, out Type[] argTypes, out string[] argNames)
        {
            if (pi == null)
                ExThrowers.ThrowArgNull(nameof(pi));
            var type = pi.ParameterType;
            argTypes = null;
            argNames = null;
            if (!type.FullName.StartsWith("System.ValueTuple`", StringComparison.Ordinal))
                return false;
            argTypes = type.GetGenericArguments();
            var attr = pi.GetCustomAttribute<TupleElementNamesAttribute>();
            if (attr != null)
                argNames = attr.TransformNames.ToArray();
            else
                argNames = type.GetFields().Select(f => f.Name).ToArray();
            return true;
        }

    }
}
