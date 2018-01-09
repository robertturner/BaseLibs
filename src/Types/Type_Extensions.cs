using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibs.Types
{
    public static class Type_Extensions
    {

        static readonly MethodInfo[] ValueTupleCreatorsCache = typeof(ValueTuple).GetMethods().Where(m => m.IsStatic && m.Name == nameof(ValueTuple.Create)).ToArray();

        public static ConstructorInvoker AsValueTupleCreator(this Type[] types)
        {
            if (types == null)
                throw new ArgumentNullException(nameof(types));

            if (types.Length >= ValueTupleCreatorsCache.Length)
                throw new ArgumentException($"Unable to create ValueTuple with {ValueTupleCreatorsCache.Length} or more args");

            var invoker = ValueTupleCreatorsCache[types.Length].MakeGenericMethod(types).DelegateForMethod();
            return objs => invoker(null, objs);
        }

    }
}
