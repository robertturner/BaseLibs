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
            var parentType = p.DeclaringType;
            var m = p.GetMethod;
            var type = p.PropertyType;

            var instParam = Expression.Parameter(typeof(object), "instance");
            var callInstParam = m.IsStatic ? null :
                ((parentType == typeof(object)) ? (Expression)instParam : Expression.Convert(instParam, parentType));

            var call = Expression.Call(callInstParam, m);
            var body = (type == typeof(object)) ? (Expression)call : Expression.Convert(call, typeof(object));
            var expr = Expression.Lambda(typeof(MemberGetter), body, instParam);
            return (MemberGetter)expr.Compile();
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
    }
}
