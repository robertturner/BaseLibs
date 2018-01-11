using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibs.Enums
{
    public static class Enum_Extensions
    {
        static System.Collections.Concurrent.ConcurrentDictionary<Type, Dictionary<Enum, string>> cachedDescriptions = new System.Collections.Concurrent.ConcurrentDictionary<Type, Dictionary<Enum, string>>();

        public static string GetDescription<T>(this T value) where T : struct, IConvertible
        {
            var t = value.GetType();
            if (!t.IsEnum)
                throw new ArgumentException("T must be enum type");
            var e = (Enum)((object)value);
            if (!cachedDescriptions.TryGetValue(t, out Dictionary<Enum, string> defs))
            {
                var vals = Enum.GetValues(t).Cast<Enum>();
                defs = vals.ToDictionary(v => v, v =>
                {
                    var str = v.ToString();
                    var f = t.GetField(str);
                    var attr = f.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>();
                    return (attr != null) ? attr.Description : str;
                });
                cachedDescriptions[t] = defs;
            }
            return defs[e];
        }
    }
}
