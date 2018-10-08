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
            var fieldType = f.FieldType;

            var instParam = Expression.Parameter(typeof(object), "instance");
            var callInstParam = f.IsStatic ? null :
                ((parentType == typeof(object)) ? (Expression)instParam : 
                (parentType.IsValueType ? Expression.Unbox(instParam, parentType) : Expression.Convert(instParam, parentType)));

            var fieldExpr = Expression.Field(callInstParam, f);
            var body = (fieldType == typeof(object)) ? (Expression)fieldExpr : 
                (fieldType.IsValueType ? Expression.Convert(fieldExpr, typeof(object)) : Expression.Convert(fieldExpr, typeof(object)));

            var expr = Expression.Lambda(typeof(MemberGetter), body, instParam);
            return (MemberGetter)expr.Compile();
        }

        public static MemberSetter DelegateForSetField(this FieldInfo f)
        {
            var parentType = f.DeclaringType;
            var fieldType = f.FieldType;

            var instParam = Expression.Parameter(typeof(object), "instance");
            var valueParam = Expression.Parameter(typeof(object), "value");
            
            var callInstParam = f.IsStatic ? null :
                ((parentType == typeof(object)) ? (Expression)instParam : 
                (parentType.IsValueType ? (Expression)Expression.Unbox(instParam, parentType) : Expression.Convert(instParam, parentType)));

            var fieldExpr = Expression.Field(callInstParam, f);
            var argExpr = (fieldType == typeof(object)) ? (Expression)valueParam : 
                (fieldType.IsValueType ? Expression.Unbox(valueParam, fieldType) : Expression.Convert(valueParam, fieldType));

            var body = Expression.Block(Expression.Assign(fieldExpr, argExpr), instParam);

            var expr = Expression.Lambda(typeof(MemberSetter), body, instParam, valueParam);
            return (MemberSetter)expr.Compile();
        }
    }
}
