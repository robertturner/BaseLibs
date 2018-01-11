using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public static Delegate BuildDynamicHandler(this Type delegateType, Func<object[], object> func)
        {
            var invokeMethod = delegateType.GetMethod(nameof(func.Invoke));
            var returnType = invokeMethod.ReturnType;
            var parameters = invokeMethod.GetParameters().Select(parm => Expression.Parameter(parm.ParameterType, parm.Name)).ToArray();

            var instance = (func.Target == null) ? null : Expression.Constant(func.Target);

            var convertedParameters = parameters.Select(parm =>
            {
                if (parm.Type == typeof(object))
                    return parm;
                return (Expression)Expression.Convert(parm, typeof(object));
            }).ToArray();
            Expression call = Expression.Call(instance, func.Method, Expression.NewArrayInit(typeof(object), convertedParameters));
            if (returnType != typeof(void) && returnType != typeof(object))
                call = Expression.Convert(call, invokeMethod.ReturnType);
            var expr = Expression.Lambda(delegateType, call, parameters);
            return expr.Compile();
        }

    }
}
