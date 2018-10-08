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
        static class DescCache<T> where T : Enum
        {
            static DescCache()
            {
                var t = typeof(T);
                var vals = Enum.GetValues(t).Cast<Enum>();
                CachedDescription = vals.ToDictionary(v => v, v =>
                {
                    var str = v.ToString();
                    var f = t.GetField(str);
                    var attr = f.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>();
                    return (attr != null) ? attr.Description : str;
                });
            }
            public static Dictionary<Enum, string> CachedDescription { get; }
        }

        public static string GetDescription<T>(this T value) where T : Enum => DescCache<T>.CachedDescription[value];
    }
}
