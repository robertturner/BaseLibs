using BaseLibs.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibs.Collections
{
    public static class Array_Extensions
    {
        public static MemberGetterIndexed GetMemberGetterDelegate(this Type arrayType)
        {
            if (!arrayType.IsArray)
                ExThrowers.ThrowArgEx($"{nameof(arrayType)} is not an array");
            var elementType = arrayType.GetElementType();

            var instParam = Expression.Parameter(typeof(object), "instance");
            var idxParam = Expression.Parameter(typeof(object), "index");

            var callInstParam = Expression.Convert(instParam, arrayType);
            var callIdxParam = Expression.Convert(idxParam, typeof(int));
            var arrayIndex = Expression.ArrayIndex(callInstParam, callIdxParam);
            var body = elementType.IsValueType ? (Expression)Expression.Convert(arrayIndex, typeof(object)) : arrayIndex;
            var expr = Expression.Lambda(typeof(MemberGetterIndexed), body, instParam, idxParam);
            return (MemberGetterIndexed)expr.Compile();
        }

    }
}
