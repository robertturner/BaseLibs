using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibs.Collections
{
    public class ReadOnlyObservableDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        readonly ObservableDictionary<TKey, TValue> dictionary;
        public ReadOnlyObservableDictionary(ObservableDictionary<TKey, TValue> dictionary)
        {
            this.dictionary = dictionary;
            ((INotifyCollectionChanged)dictionary).CollectionChanged += ReadOnlyObservableDictionary_CollectionChanged;
            ((INotifyPropertyChanged)dictionary).PropertyChanged += ReadOnlyObservableDictionary_PropertyChanged;
        }

        private void ReadOnlyObservableDictionary_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }

        void ReadOnlyObservableDictionary_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(sender, e);
        }

        public TValue this[TKey key] => dictionary[key];

        public IEnumerable<TKey> Keys => dictionary.Keys;

        public IEnumerable<TValue> Values => dictionary.Values;

        public int Count => dictionary.Count;

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public bool ContainsKey(TKey key) => dictionary.ContainsKey(key);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => ((IReadOnlyDictionary<TKey, TValue>)dictionary).GetEnumerator();

        public bool TryGetValue(TKey key, out TValue value) => dictionary.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)dictionary).GetEnumerator();
    }
}
