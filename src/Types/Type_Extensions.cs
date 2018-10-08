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
