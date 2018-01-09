using System;
using System.Collections.Generic;

namespace BaseLibs.Collections
{
    public static class Dictionary_Extensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default(TValue))
        {
            if (dictionary == null)
                return defaultValue;
            return dictionary.TryGetValue(key, out TValue value) ? value : defaultValue;
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default(TValue))
        {
            if (dictionary == null)
                return defaultValue;
            return dictionary.TryGetValue(key, out TValue value) ? value : defaultValue;
        }

        public static TValue GetOrSet<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> valueGetter, bool setIfNull = true)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));
            if (valueGetter == null)
                throw new ArgumentNullException(nameof(valueGetter));
            if (dictionary.TryGetValue(key, out TValue value))
                return value;
            value = valueGetter();
            if (setIfNull || value != null)
                dictionary.Add(key, value);
            return value;
        }
        public static TValue GetOrSet<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> valueGetter, bool setIfNull = true)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));
            if (valueGetter == null)
                throw new ArgumentNullException(nameof(valueGetter));
            if (dictionary.TryGetValue(key, out TValue value))
                return value;
            value = valueGetter(key);
            if (setIfNull || value != null)
                dictionary.Add(key, value);
            return value;
        }

        public static bool AddIfNoKey<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> valProvider)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));
            if (!dictionary.ContainsKey(key))
                dictionary[key] = valProvider();
            else
                return false;
            return true;
        }
    }
}
