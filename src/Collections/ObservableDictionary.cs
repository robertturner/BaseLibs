﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibs.Collections
{
    public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        readonly IDictionary<TKey, TValue> dictionary;

		public event NotifyCollectionChangedEventHandler CollectionChanged = (sender, args) => { };

		/// <summary>Event raised when a property on the collection changes.</summary>
		public event PropertyChangedEventHandler PropertyChanged = (sender, args) => { };

		/// <summary>
		/// Initializes an instance of the class.
		/// </summary>
		public ObservableDictionary()
			: this(new Dictionary<TKey, TValue>())
		{ }

		/// <summary>
		/// Initializes an instance of the class using another dictionary as 
		/// the key/value store.
		/// </summary>
		public ObservableDictionary(IDictionary<TKey, TValue> dictionary) => this.dictionary = dictionary;

		void AddWithNotification(KeyValuePair<TKey, TValue> item) => AddWithNotification(item.Key, item.Value);

		void AddWithNotification(TKey key, TValue value)
		{
			dictionary.Add(key, value);
			CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
				new KeyValuePair<TKey, TValue>(key, value)));
			PropertyChanged(this, new PropertyChangedEventArgs(nameof(Count)));
			PropertyChanged(this, new PropertyChangedEventArgs(nameof(Keys)));
			PropertyChanged(this, new PropertyChangedEventArgs(nameof(Values)));
		}

		bool RemoveWithNotification(TKey key)
		{
			if (dictionary.TryGetValue(key, out TValue value) && dictionary.Remove(key))
			{
				CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
					new KeyValuePair<TKey, TValue>(key, value)));
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(Count)));
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(Keys)));
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(Values)));
				return true;
			}
			return false;
		}

		void UpdateWithNotification(TKey key, TValue value)
		{
			if (dictionary.TryGetValue(key, out TValue existing))
			{
				dictionary[key] = value;
				CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
					new KeyValuePair<TKey, TValue>(key, value),
					new KeyValuePair<TKey, TValue>(key, existing)));
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(Values)));
			}
			else
				AddWithNotification(key, value);
		}

		/// <summary>
		/// Allows derived classes to raise custom property changed events.
		/// </summary>
		protected void RaisePropertyChanged(PropertyChangedEventArgs args) => PropertyChanged(this, args);

		#region IDictionary<TKey,TValue> Members

		/// <summary>
		/// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2" />.
		/// </summary>
		/// <param name="key">The object to use as the key of the element to add.</param>
		/// <param name="value">The object to use as the value of the element to add.</param>
		public void Add(TKey key, TValue value) => AddWithNotification(key, value);

		/// <summary>
		/// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the specified key.
		/// </summary>
		/// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2" />.</param>
		/// <returns>
		/// true if the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the key; otherwise, false.
		/// </returns>
		public bool ContainsKey(TKey key) => dictionary.ContainsKey(key);

		public void Clear()
		{
			dictionary.Clear();
			CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			PropertyChanged(this, new PropertyChangedEventArgs(nameof(Count)));
			PropertyChanged(this, new PropertyChangedEventArgs(nameof(Keys)));
			PropertyChanged(this, new PropertyChangedEventArgs(nameof(Values)));
		}

		/// <summary>
		/// Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2" />.
		/// </summary>
		/// <returns>An <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" />.</returns>
		public ICollection<TKey> Keys => dictionary.Keys;

		/// <summary>
		/// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2" />.
		/// </summary>
		/// <param name="key">The key of the element to remove.</param>
		/// <returns>
		/// true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key" /> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2" />.
		/// </returns>
		public bool Remove(TKey key) => RemoveWithNotification(key);

		/// <summary>
		/// Gets the value associated with the specified key.
		/// </summary>
		/// <param name="key">The key whose value to get.</param>
		/// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value" /> parameter. This parameter is passed uninitialized.</param>
		/// <returns>
		/// true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the specified key; otherwise, false.
		/// </returns>
		public bool TryGetValue(TKey key, out TValue value) => dictionary.TryGetValue(key, out value);

		/// <summary>
		/// Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2" />.
		/// </summary>
		/// <returns>An <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" />.</returns>
		public ICollection<TValue> Values => dictionary.Values;

		/// <summary>
		/// Gets or sets the element with the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public TValue this[TKey key]
		{
			get => dictionary[key];
			set => UpdateWithNotification(key, value);
		}

		public bool TryMoveValue(TKey currentKey, TKey newKey)
		{
			if (dictionary.TryGetValue(currentKey, out TValue value))
			{
				dictionary[newKey] = value;
				CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, new KeyValuePair<TKey, TValue>(newKey, value), new KeyValuePair<TKey, TValue>(currentKey, value)));
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(Keys)));
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(Values)));
				return true;
			}
			return false;
		}

		#endregion

		#region ICollection<KeyValuePair<TKey,TValue>> Members

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) => AddWithNotification(item);

		void ICollection<KeyValuePair<TKey, TValue>>.Clear()
		{
			((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Clear();
			CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			PropertyChanged(this, new PropertyChangedEventArgs(nameof(Count)));
			PropertyChanged(this, new PropertyChangedEventArgs(nameof(Keys)));
			PropertyChanged(this, new PropertyChangedEventArgs(nameof(Values)));
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Contains(item);

		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).CopyTo(array, arrayIndex);

		bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).IsReadOnly;

		IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => dictionary.Keys;

		IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => dictionary.Values;

		public int Count => dictionary.Count;

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) => RemoveWithNotification(item.Key);

		#endregion

		#region IEnumerable<KeyValuePair<TKey,TValue>> Members

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).GetEnumerator();

		#endregion
	}
}
