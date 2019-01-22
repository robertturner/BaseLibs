using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibs.Types
{
    public static class MethodInfo_Extensions
    {
        #region Delegates
        public static MethodInvoker DelegateForMethod(this MethodInfo m)
        {
            var parentType = m.DeclaringType;
            var retType = m.ReturnType;
            var args = m.GetParameters().Select(p => p.ParameterType).ToArray();

            var instParam = Expression.Parameter(typeof(object), "instance");
            var argsParam = Expression.Parameter(typeof(object[]), "args");

            var indexedParams = args.Select((arg, i) =>
            {
                Expression argExpr = Expression.ArrayAccess(argsParam, Expression.Constant(i));
                if (arg == typeof(object))
                    return argExpr;
                return Expression.Convert(argExpr, arg);
            }).ToArray();

            Expression call = m.IsStatic ? Expression.Call(m, indexedParams) :
                Expression.Call((parentType == typeof(object)) ? (Expression)instParam : Expression.Convert(instParam, parentType), m, indexedParams);

            Expression<Action<object[]>> callArgEx = a => ExThrowers.ThrowArgEx($"Number of supplied arguments ({a.Length}) does not match number of method arguments ({args.Length})");
            var checkerExpr = Expression.IfThen(Expression.NotEqual(Expression.ArrayLength(argsParam), Expression.Constant(args.Length)),
                Expression.Invoke(callArgEx, argsParam));
            
            var checkedCall = Expression.Block(checkerExpr, call);

            if (retType == typeof(void))
                call = Expression.Block(checkedCall, Expression.Constant(null, typeof(object))); // Return null
            else if (retType.IsValueType)
                call = Expression.Convert(checkedCall, typeof(object)); // Box
            else
                call = checkedCall;

            var expr = Expression.Lambda(typeof(MethodInvoker), call, instParam, argsParam);
            return (MethodInvoker)expr.Compile();
        }

        public static T CreateCustomDelegate<T>(this MethodInfo m) where T : class
        {
            var delType = typeof(T);
            if (!typeof(Delegate).IsAssignableFrom(delType))
                ExThrowers.ThrowArgEx("T is not Delegate type");
            var delInvokeM = delType.GetMethod("Invoke");
            var delParams = delInvokeM.GetParameters();
            var parentType = m.DeclaringType;
            var retType = m.ReturnType;
            var args = m.GetParameters().Select(p => p.ParameterType).ToArray();
            if (delParams.Length != (args.Length + (m.IsStatic ? 0 : 1)))
                ExThrowers.ThrowArgEx("Number of Delegate arguments does not match method");

            var argsParams = delParams.Select(p => Expression.Parameter(p.ParameterType, p.Name)).ToArray();

            var callParams = m.IsStatic ? argsParams : argsParams.Skip(1);
            var callArgs = callParams.Select((arg, i) =>
            {
                if (arg.Type == args[i])
                    return (Expression)arg;
                return Expression.Convert(arg, args[i]);
            });

            var callInstParam = m.IsStatic ? null :
                ((parentType == typeof(object)) ? (Expression)argsParams[0] : Expression.Convert(argsParams[0], parentType));

            Expression call = Expression.Call(callInstParam, m, callArgs.ToArray());

            if (retType == typeof(void))
            {
                if (delInvokeM.ReturnType != typeof(void))
                    call = Expression.Block(call, Expression.Default(delInvokeM.ReturnType)); // Return default(ReturnType)
            }
            else if (retType != delInvokeM.ReturnType)
            {
                if (delInvokeM.ReturnType != typeof(void) && (retType.IsValueType || delInvokeM.ReturnType.IsValueType))
                    call = Expression.Convert(call, typeof(object));
            }
            var expr = Expression.Lambda(delType, call, argsParams);
            return expr.Compile() as T;
        }
        #endregion

        #region Attributes
        public static IEnumerable<Attribute> Attributes(this ICustomAttributeProvider provider, params Type[] attributeTypes)
        {
            var hasTypes = attributeTypes != null && attributeTypes.Length > 0;
            return provider.GetCustomAttributes(true).Cast<Attribute>()
                .Where(attr => !hasTypes || attributeTypes.Any(at =>
                {
                    var type = attr.GetType();
                    return at == type || at.IsSubclassOf(type);
                }));
        }

        public static IEnumerable<T> Attributes<T>(this ICustomAttributeProvider provider) where T : Attribute
        {
            return provider.GetCustomAttributes(typeof(T), true).Cast<T>().ToList();
        }

        public static Attribute Attribute(this ICustomAttributeProvider provider, Type attributeType)
        {
            return provider.Attributes(attributeType).FirstOrDefault();
        }

        public static T Attribute<T>(this ICustomAttributeProvider provider) where T : Attribute
        {
            return provider.Attributes<T>().FirstOrDefault();
        }

        public static bool HasAttribute(this ICustomAttributeProvider provider, Type attributeType)
        {
            return provider.Attribute(attributeType) != null;
        }

        public static bool HasAttribute<T>(this ICustomAttributeProvider provider) where T : Attribute
        {
            return provider.HasAttribute(typeof(T));
        }
        #endregion
    }
}
