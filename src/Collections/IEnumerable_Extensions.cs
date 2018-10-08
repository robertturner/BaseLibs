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

        public static async Task ForEach<T>(this IEnumerable<T> source, Func<T, Task> action)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            foreach (T element in source)
                await action(element);
        }

        public static async Task ForEach<T>(this IEnumerable<T> source, Func<T, int, Task> action)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            int index = 0;
            foreach (T element in source)
                await action(element, index++);
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source) => new HashSet<T>(source);

        public static bool NullOrEmpty<T>(this IEnumerable<T> source) => source == null || !source.Any();
    }
}
