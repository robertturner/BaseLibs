using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibs.Collections
{
    public static class IEnumerable_Extensions
    {
        public static IEnumerable<T> SingleItemAsEnumerable<T>(this T item)
        {
            yield return item;
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            foreach (T element in source)
                action(element);
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            int index = 0;
            foreach (T element in source)
                action(element, index++);
        }

        public static Task<T[]> WhenAll<T>(this IEnumerable<Task<T>> source)
        {
            return Task.WhenAll(source);
        }
    }
}
