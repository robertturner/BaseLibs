using BaseLibs.Collections;
using BaseLibs.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibs.Tuples
{
    public static class Type_Extensions
    {
        static readonly MethodInfo[] creatorsConstructorCache = typeof(ValueTuple).GetMethods().Where(m => m.IsStatic && m.Name == nameof(ValueTuple.Create)).ToArray();
        static readonly ConcurrentDictionary<Type[], ConstructorInvoker> creators = new ConcurrentDictionary<Type[], ConstructorInvoker>();

        public static ConstructorInvoker AsValueTupleCreator(this Type[] types)
        {
            types.ThrowIfNull(nameof(types));
            if (types.Length >= creatorsConstructorCache.Length)
                ExThrowers.ThrowArgEx($"Unable to create ValueTuple type with more than {creatorsConstructorCache.Length} args");

            return creators.GetOrAdd(types, t =>
            {
                var invoker = creatorsConstructorCache[t.Length].MakeGenericMethod(t).DelegateForMethod();
                return objs => invoker(null, objs);
            });
        }

        static readonly Type[] valueTupleTypes = new[] {
            typeof(ValueTuple<>),
            typeof(ValueTuple<,>),
            typeof(ValueTuple<,,>),
            typeof(ValueTuple<,,,>),
            typeof(ValueTuple<,,,,>),
            typeof(ValueTuple<,,,,,>),
            typeof(ValueTuple<,,,,,,>),
            typeof(ValueTuple<,,,,,,,>)
        };
        public static Type AsValueTupleType(this Type[] types)
        {
            types.ThrowIfNull(nameof(types));
            if (types.Length < 1)
                ExThrowers.ThrowArgEx("Unable to create ValueTuple with less than one type");
            if (types.Length > creatorsConstructorCache.Length)
                ExThrowers.ThrowArgEx($"Unable to create ValueTuple type with more than {valueTupleTypes.Length} args");
            return valueTupleTypes[types.Length - 1].MakeGenericType(types);
        }

        public static bool TryGetValueTupleTypes(this Type type, out Type[] types)
        {
            if (type == null)
                ExThrowers.ThrowArgNull(nameof(type));
            types = null;
            if (!type.FullName.StartsWith("System.ValueTuple`", StringComparison.Ordinal))
                return false;
            types = type.GetGenericArguments();
            return true;
        }

        static readonly object getterLock = new object();
        static readonly Dictionary<Type, ValueTupleGetValuesInvoker> getters = new Dictionary<Type, ValueTupleGetValuesInvoker>();

        public static ValueTupleGetValuesInvoker ValueTupleValuesGetter(this Type type)
        {
            lock (getterLock)
            {
                return getters.GetOrSet(type, () =>
                {
                    if (!type.IsGenericType || !type.Name.StartsWith("ValueTuple`"))
                        ExThrowers.ThrowArgEx($"{nameof(type)} is not ValueTuple type");
                    var fields = type.GetFields();

                    var instParam = Expression.Parameter(typeof(object), "instance");
                    var instVT = Expression.Convert(instParam, type);

                    var fieldExpressions = fields.Select((f, i) =>
                    {
                        Expression fieldExpr = Expression.Field(instVT, f);
                        if (f.FieldType == typeof(object))
                            return fieldExpr;
                        return Expression.Convert(fieldExpr, typeof(object));
                    });
                    var arrayExpr = Expression.NewArrayInit(typeof(object), fieldExpressions.ToArray());
                    var expr = Expression.Lambda(typeof(ValueTupleGetValuesInvoker), arrayExpr, instParam);
                    return (ValueTupleGetValuesInvoker)expr.Compile();
                });
            }
        }

        public static ValueTupleGetValuesInvoker ValuesGetter<T1>(this ValueTuple<T1> vt) => ValueTupleValuesGetter(typeof(ValueTuple<T1>));
        public static ValueTupleGetValuesInvoker ValuesGetter<T1, T2>(this ValueTuple<T1, T2> vt) => ValueTupleValuesGetter(typeof(ValueTuple<T1, T2>));
        public static ValueTupleGetValuesInvoker ValuesGetter<T1, T2, T3>(this ValueTuple<T1, T2, T3> vt) => ValueTupleValuesGetter(typeof(ValueTuple<T1, T2, T3>));
        public static ValueTupleGetValuesInvoker ValuesGetter<T1, T2, T3, T4>(this ValueTuple<T1, T2, T3, T4> vt) => ValueTupleValuesGetter(typeof(ValueTuple<T1, T2, T3, T4>));
        public static ValueTupleGetValuesInvoker ValuesGetter<T1, T2, T3, T4, T5>(this ValueTuple<T1, T2, T3, T4, T5> vt) => ValueTupleValuesGetter(typeof(ValueTuple<T1, T2, T3, T4, T5>));
        public static ValueTupleGetValuesInvoker ValuesGetter<T1, T2, T3, T4, T5, T6>(this ValueTuple<T1, T2, T3, T4, T5, T6> vt) => ValueTupleValuesGetter(typeof(ValueTuple<T1, T2, T3, T4, T5, T6>));
        public static ValueTupleGetValuesInvoker ValuesGetter<T1, T2, T3, T4, T5, T6, T7>(this ValueTuple<T1, T2, T3, T4, T5, T6, T7> vt) => ValueTupleValuesGetter(typeof(ValueTuple<T1, T2, T3, T4, T5, T6, T7>));

    }
}
