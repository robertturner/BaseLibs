using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibs.Types
{
    public static class FieldInfo_Extensions
    {
        public static MemberGetter DelegateForGetField(this FieldInfo f)
        {
            var parentType = f.DeclaringType;
            var type = f.FieldType;

            var instParam = Expression.Parameter(typeof(object), "instance");
            var callInstParam = f.IsStatic ? null :
                ((parentType == typeof(object)) ? (Expression)instParam : Expression.Convert(instParam, parentType));

            var fieldExpr = Expression.Field(callInstParam, f);
            var body = (type == typeof(object)) ? (Expression)fieldExpr : Expression.Convert(fieldExpr, typeof(object));

            var expr = Expression.Lambda(typeof(MemberGetter), body, instParam);
            return (MemberGetter)expr.Compile();
        }

        public static MemberSetter DelegateForSetField(this FieldInfo f)
        {
            var parentType = f.DeclaringType;
            var type = f.FieldType;

            var instParam = Expression.Parameter(typeof(object), "instance");
            var valueParam = Expression.Parameter(typeof(object), "value");
            var callInstParam = f.IsStatic ? null :
                ((parentType == typeof(object)) ? (Expression)instParam : Expression.Convert(instParam, parentType));

            var fieldExpr = Expression.Field(callInstParam, f);
            var argExpr = (type == typeof(object)) ? (Expression)valueParam : Expression.Convert(valueParam, type);
            var body = Expression.Assign(fieldExpr, argExpr);

            var expr = Expression.Lambda(typeof(MemberSetter), body, instParam, valueParam);
            return (MemberSetter)expr.Compile();
        }
    }
}
