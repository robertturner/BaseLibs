using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace BaseLibs.Types
{
    public class SpecialisedCallerCache<TBaseType, TDelegate> where TDelegate : class
    {
        static readonly ConcurrentDictionary<Type, TDelegate> creatorCache = new ConcurrentDictionary<Type, TDelegate>();

        public static TDelegate CallForType(Type specialised, Type specialisedClass, string specialisedMethodName)
        {
            return creatorCache.GetOrAdd(specialised, _ =>
            {
                if (!typeof(Delegate).IsAssignableFrom(typeof(TDelegate)))
                    ExThrowers.ThrowArgEx($"{nameof(TDelegate)} is not a delegate");
                if (!specialisedClass.IsGenericType)
                    ExThrowers.ThrowArgEx($"{nameof(specialisedClass)} is not a generic type type");
                if (specialisedClass.IsConstructedGenericType)
                    ExThrowers.ThrowArgEx($"{nameof(specialisedClass)} must be a non-constructed generic type");
                var callArgs = typeof(TDelegate).GetMethod("Invoke").GetParameters().Select(p => p.ParameterType).ToArray();
                return specialisedClass.MakeGenericType(specialised).GetMethod(specialisedMethodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static, null, callArgs, null).CreateCustomDelegate<TDelegate>();
            });
        }

    }
}
