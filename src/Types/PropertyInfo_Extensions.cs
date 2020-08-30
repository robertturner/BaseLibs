using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibs.Types
{
    public static class PropertyInfo_Extensions
    {
        public static MemberGetter DelegateForGetProperty(this PropertyInfo p)
        {
            if (p.GetIndexParameters().Length > 0)
                ExThrowers.ThrowArgEx("Property is index type");
            var parentType = p.DeclaringType;
            var m = p.GetMethod;
            var type = p.PropertyType;

            var instParam = Expression.Parameter(typeof(object), "instance");
            var callInstParam = m.IsStatic ? null :
                ((parentType == typeof(object)) ? (Expression)instParam : Expression.Convert(instParam, parentType));

            var call = Expression.Call(callInstParam, m);
            var body = type.IsValueType ? (Expression)Expression.Convert(call, typeof(object)) : call;
            var expr = Expression.Lambda(typeof(MemberGetter), body, instParam);
            return (MemberGetter)expr.Compile();
        }
        public static MemberGetter<T> DelegateForGetProperty<T>(this PropertyInfo p)
        {
            if (p.GetIndexParameters().Length > 0)
                ExThrowers.ThrowArgEx("Property is index type");
            var parentType = p.DeclaringType;
            var m = p.GetMethod;
            var type = p.PropertyType;
            var genType = typeof(T);
            if (!genType.IsAssignableFrom(type))
                ExThrowers.ThrowArgEx($"Generic argument ({genType}) cannot be assigned from property type ({type})");

            var instParam = Expression.Parameter(typeof(object), "instance");
            var callInstParam = m.IsStatic ? null :
                ((parentType == typeof(object)) ? (Expression)instParam : Expression.Convert(instParam, parentType));

            var call = Expression.Call(callInstParam, m);
            var body = type.IsValueType ? (Expression)Expression.Convert(call, genType) : call;
            var expr = Expression.Lambda(typeof(MemberGetter<T>), body, instParam);
            return (MemberGetter<T>)expr.Compile();
        }

        public static MemberGetterIndexed DelegateForGetPropertyIndex(this PropertyInfo p)
        {
            var idxParams = p.GetIndexParameters();
            if (idxParams.Length != 1)
                ExThrowers.ThrowArgEx("Property is not index type");
            var parentType = p.DeclaringType;
            var m = p.GetMethod;
            var type = p.PropertyType;
            var idxType = idxParams[0].ParameterType;

            var instParam = Expression.Parameter(typeof(object), "instance");
            var idxParam = Expression.Parameter(typeof(object), "index");
            var callInstParam = m.IsStatic ? null :
                ((parentType == typeof(object)) ? (Expression)instParam : Expression.Convert(instParam, parentType));
            var callIdxParam = (idxType == typeof(object)) ? (Expression)idxParam : Expression.Convert(idxParam, idxType);

            var call = Expression.Call(callInstParam, m, callIdxParam);
            var body = type.IsValueType ? (Expression)Expression.Convert(call, typeof(object)) : call;
            var expr = Expression.Lambda(typeof(MemberGetterIndexed), body, instParam, idxParam);
            return (MemberGetterIndexed)expr.Compile();
        }
        public static MemberGetterIndexed<T> DelegateForGetPropertyIndex<T>(this PropertyInfo p)
        {
            var idxParams = p.GetIndexParameters();
            if (idxParams.Length != 1)
                ExThrowers.ThrowArgEx("Property is not index type");
            var parentType = p.DeclaringType;
            var m = p.GetMethod;
            var type = p.PropertyType;
            var idxType = idxParams[0].ParameterType;
            var genType = typeof(T);
            if (!genType.IsAssignableFrom(type))
                ExThrowers.ThrowArgEx($"Generic argument ({genType}) cannot be assigned from property type ({type})");

            var instParam = Expression.Parameter(typeof(object), "instance");
            var idxParam = Expression.Parameter(typeof(object), "index");
            var callInstParam = m.IsStatic ? null :
                ((parentType == typeof(object)) ? (Expression)instParam : Expression.Convert(instParam, parentType));
            var callIdxParam = (idxType == typeof(object)) ? (Expression)idxParam : Expression.Convert(idxParam, idxType);

            var call = Expression.Call(callInstParam, m, callIdxParam);
            var body = type.IsValueType ? (Expression)Expression.Convert(call, genType) : call;
            var expr = Expression.Lambda(typeof(MemberGetterIndexed<T>), body, instParam, idxParam);
            return (MemberGetterIndexed<T>)expr.Compile();
        }

        public static MemberSetter DelegateForSetProperty(this PropertyInfo p)
        {
            var parentType = p.DeclaringType;
            var type = p.PropertyType;
            var m = p.SetMethod;

            var instParam = Expression.Parameter(typeof(object), "instance");
            var valueParam = Expression.Parameter(typeof(object), "value");
            var callInstParam = m.IsStatic ? null :
                ((parentType == typeof(object)) ? (Expression)instParam : 
                (parentType.IsValueType ? Expression.Unbox(instParam, parentType) : Expression.Convert(instParam, parentType)));

            var argExpr = (type == typeof(object)) ? (Expression)valueParam : Expression.Convert(valueParam, type);

            var body = Expression.Block(Expression.Call(callInstParam, m, argExpr), instParam);

            var expr = Expression.Lambda(typeof(MemberSetter), body, instParam, valueParam);
            return (MemberSetter)expr.Compile();
        }

        public static Expression ExpressionForSetProperty(this PropertyInfo p)
        {
            var parentType = p.DeclaringType;
            var type = p.PropertyType;
            var m = p.SetMethod;

            var instParam = Expression.Parameter(typeof(object), "instance");
            var valueParam = Expression.Parameter(typeof(object), "value");
            var callInstParam = m.IsStatic ? null :
                ((parentType == typeof(object)) ? (Expression)instParam :
                (parentType.IsValueType ? Expression.Unbox(instParam, parentType) : Expression.Convert(instParam, parentType)));

            var argExpr = (type == typeof(object)) ? (Expression)valueParam : Expression.Convert(valueParam, type);

            var body = Expression.Block(Expression.Call(callInstParam, m, argExpr), instParam);

            var expr = Expression.Lambda(typeof(MemberSetter), body, instParam, valueParam);
            return expr;
        }
    }
}
