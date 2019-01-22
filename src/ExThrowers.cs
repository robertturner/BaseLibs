using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibs
{
    internal static class ExThrowers
    {
        public static void ThrowIfNull<T>(this T val, string name) where T : class
        {
            if (val == null)
                throw new ArgumentNullException(name);
        }

        public static void ThrowArgNull(string name) => throw new ArgumentNullException(name);

        public static void ThrowArgEx(string message) => throw new ArgumentException(message);

        public static void ThrowTimeoutEx() => throw new TimeoutException("The operation timed out");
    }
}
