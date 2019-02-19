using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibs
{
    public static class object_Extensions
    {
        static public bool EqualsByReference<T>(T x, T y)
        {
            if (x is ValueType)
                return EqualityComparer<T>.Default.Equals(x, y); // avoids boxing
            return object.ReferenceEquals(x, y);
        }
    }
}
