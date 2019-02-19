using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibs.Types
{
    public static class ConstructorInfo_Extensions
    {
        public static Func<object> DelegateForConstructorNoArgs(this ConstructorInfo c)
        {
            var parentType = c.DeclaringType;
            if (c.GetParameters().Length > 0)
                ExThrowers.ThrowArgEx("This method is only for constructors with no arguments");

            Expression call = Expression.New(c);
            if (parentType.IsValueType)
                call = Expression.Convert(call, typeof(object));

            var expr = Expression.Lambda(typeof(Func<object>), call);
            return (Func<object>)expr.Compile();
        }
        public static Func<T> DelegateForConstructorNoArgs<T>(this ConstructorInfo c)
        {
            var parentType = c.DeclaringType;
            if (!typeof(T).IsAssignableFrom(parentType))
                ExThrowers.ThrowArgEx($"Can't assign parent type ({parentType}) to T ({typeof(T)})");
            if (c.GetParameters().Length > 0)
                ExThrowers.ThrowArgEx("This method is only for constructors with no arguments");

            Expression call = Expression.New(c);
            if (parentType.IsValueType)
                call = Expression.Convert(call, typeof(T));

            var expr = Expression.Lambda(typeof(Func<T>), call);
            return (Func<T>)expr.Compile();
        }

        public static ConstructorInvoker DelegateForConstructor(this ConstructorInfo c)
        {
            var parentType = c.DeclaringType;

            var ctorParams = c.GetParameters().Select(p => p.ParameterType).ToArray();

            var argsParam = Expression.Parameter(typeof(object[]), "args");

            var indexedParams = ctorParams.Select((arg, i) =>
            {
                Expression argExpr = Expression.ArrayAccess(argsParam, Expression.Constant(i));
                if (arg == typeof(object))
                    return argExpr;
                return Expression.Convert(argExpr, arg);
            }).ToArray();

            Expression createInst;
            if (ctorParams.Length > 0)
                createInst = Expression.New(c, indexedParams);
            else
                createInst = Expression.New(c);

            Expression<Action<object[]>> callArgEx = a => ExThrowers.ThrowArgEx($"Number of supplied arguments ({a.Length}) does not match number of constructor arguments ({ctorParams.Length})");
            var checkerExpr = Expression.IfThen(Expression.NotEqual(Expression.ArrayLength(argsParam), Expression.Constant(ctorParams.Length)),
                Expression.Invoke(callArgEx, argsParam));

            createInst = Expression.Block(checkerExpr, createInst);

            if (parentType.IsValueType)
                createInst = Expression.Convert(createInst, typeof(object));

            var expr = Expression.Lambda(typeof(ConstructorInvoker), createInst, argsParam);
            return (ConstructorInvoker)expr.Compile();
        }

        public static ConstructorInvoker<T> DelegateForConstructor<T>(this ConstructorInfo c)
        {
            var parentType = c.DeclaringType;
            if (!typeof(T).IsAssignableFrom(parentType))
                ExThrowers.ThrowArgEx($"Can't assign parent type ({parentType}) to T ({typeof(T)})");

            var ctorParams = c.GetParameters().Select(p => p.ParameterType).ToArray();

            var argsParam = Expression.Parameter(typeof(object[]), "args");

            var indexedParams = ctorParams.Select((arg, i) =>
            {
                Expression argExpr = Expression.ArrayAccess(argsParam, Expression.Constant(i));
                if (arg == typeof(object))
                    return argExpr;
                return Expression.Convert(argExpr, arg);
            }).ToArray();

            Expression createInst;
            if (ctorParams.Length > 0)
                createInst = Expression.New(c, indexedParams);
            else
                createInst = Expression.New(c);

            Expression<Action<object[]>> callArgEx = a => ExThrowers.ThrowArgEx($"Number of supplied arguments ({a.Length}) does not match number of constructor arguments ({ctorParams.Length})");
            var checkerExpr = Expression.IfThen(Expression.NotEqual(Expression.ArrayLength(argsParam), Expression.Constant(ctorParams.Length)),
                Expression.Invoke(callArgEx, argsParam));

            createInst = Expression.Block(checkerExpr, createInst);

            if (parentType.IsValueType)
                createInst = Expression.Convert(createInst, typeof(T));

            var expr = Expression.Lambda(typeof(ConstructorInvoker<T>), createInst, argsParam);
            return (ConstructorInvoker<T>)expr.Compile();
        }

    }
}
