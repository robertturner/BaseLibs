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
                var vals = Enum.GetValues(t);
                CachedDescription = new Dictionary<Enum, string>(vals.Length);
                DescriptionToEnum = new Dictionary<string, Enum>(vals.Length);
                foreach (var val in vals)
                {
                    var enumVal = (Enum)val;
                    var str = val.ToString();
                    var f = t.GetField(str);
                    var attr = f.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>();
                    var desc = (attr != null) ? attr.Description : str;
                    CachedDescription[enumVal] = desc;
                    DescriptionToEnum[desc] = enumVal;
                }
            }
            public static Dictionary<Enum, string> CachedDescription { get; }
            public static Dictionary<string, Enum> DescriptionToEnum { get; }
        }

        public static string GetDescription<T>(this T value) where T : Enum => DescCache<T>.CachedDescription[value];

        public static T? TryParseFromDescription<T>(string enumDescription) where T : struct, Enum
        {
            if (DescCache<T>.DescriptionToEnum.TryGetValue(enumDescription, out Enum val))
                return (T)val;
            return null;
        }
    }
}
