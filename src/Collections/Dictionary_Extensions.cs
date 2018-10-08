using System;
using System.Collections.Generic;
using System.Linq;

namespace BaseLibs.Collections
{
    public static class Dictionary_Extensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> dictionary, TKey key, TValue defaultValue = default)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));
            if (dictionary is IReadOnlyDictionary<TKey, TValue> rdict)
                return rdict.TryGetValue(key, out TValue value) ? value : defaultValue;
            if (dictionary is IDictionary<TKey, TValue> idict)
                return idict.TryGetValue(key, out TValue value) ? value : defaultValue;
            var match = dictionary.Where(kvp => kvp.Key.Equals(key));
            return match.Any() ? match.First().Value : defaultValue;
        }

        public static TParsed ParseValueOrDefault<TKey, TValue, TParsed>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, TParsed defaultValue = default)
        {
            if (dictionary == null)
                return defaultValue;
            if (dictionary.TryGetValue(key, out TValue value))
            {
                var con = System.ComponentModel.TypeDescriptor.GetConverter(typeof(TParsed));
                if (con != null && con.IsValid(value))
                    return (TParsed)con.ConvertFrom(value);
            }
            return defaultValue;
        }

        public static void UpdateOrRemove<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, bool update, TValue value)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));
            if (update)
                dictionary[key] = value;
            else
                dictionary.Remove(key);
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

        public static TValue GetOrThrow<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> dictionary, TKey key, Func<Exception> exceptionCreator)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));
            if (exceptionCreator == null)
                throw new ArgumentNullException(nameof(exceptionCreator));
            if (dictionary is IReadOnlyDictionary<TKey, TValue> rdict && rdict.TryGetValue(key, out TValue rdvalue))
                return rdvalue;
            if (dictionary is IDictionary<TKey, TValue> idict && idict.TryGetValue(key, out TValue idvalue))
                return idvalue;
            var match = dictionary.Where(kvp => kvp.Key.Equals(key));
            if (match.Any())
                return match.First().Value;
            throw exceptionCreator();
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
