using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebRock.Utils.Monad;

namespace WebRock.Utils.Extensions
{
	public static class DictionaryExtensions
	{
		public static Maybe<TVal> GetValue<TKey, TVal>(this IDictionary<TKey, TVal> d, TKey k)
		{
			if ((object)k == null)
			{
				return (Maybe<TVal>)Maybe.Nothing;
			}
			else
			{
				TVal val;
				return !d.TryGetValue(k, out val) ? (Maybe<TVal>)Maybe.Nothing : Maybe.Just<TVal>(val);
			}
		}

		public static TVal GetValueFailingVerbose<TKey, TVal>(this IDictionary<TKey, TVal> d, TKey k)
		{
			TVal val;
			if (d.TryGetValue(k, out val))
				return val;
			throw new KeyNotFoundException(StringExtensions.Fmt("Key {0} was not found in map", new object[1]
      {
        (object) k
      }));
		}

		public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> valueRetreiver)
		{
			TValue obj;
			if (!dictionary.TryGetValue(key, out obj))
			{
				obj = valueRetreiver(key);
				dictionary.Add(key, obj);
			}
			return obj;
		}

		public static IDictionary<TKey, TValue> WithDefaultValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TValue defaultValue)
		{
			return (IDictionary<TKey, TValue>)new DictionaryExtensions.DefaultDictionary<TKey, TValue>(dictionary, defaultValue);
		}

		private class DefaultDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
		{
			private readonly IDictionary<TKey, TValue> _dictionary;
			private readonly TValue _defaultValue;

			public int Count
			{
				get
				{
					return this._dictionary.Count;
				}
			}

			public bool IsReadOnly
			{
				get
				{
					return this._dictionary.IsReadOnly;
				}
			}

			public TValue this[TKey key]
			{
				get
				{
					return MaybeExtension.GetOrDefault<TValue>(DictionaryExtensions.GetValue<TKey, TValue>(this._dictionary, key), this._defaultValue);
				}
				set
				{
					this._dictionary[key] = value;
				}
			}

			public ICollection<TKey> Keys
			{
				get
				{
					return this._dictionary.Keys;
				}
			}

			public ICollection<TValue> Values
			{
				get
				{
					return this._dictionary.Values;
				}
			}

			public DefaultDictionary(IDictionary<TKey, TValue> dictionary, TValue defaultValue)
			{
				this._dictionary = dictionary;
				this._defaultValue = defaultValue;
			}

			public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
			{
				return this._dictionary.GetEnumerator();
			}

			public void Add(KeyValuePair<TKey, TValue> item)
			{
				this._dictionary.Add(item);
			}

			public void Clear()
			{
				this._dictionary.Clear();
			}

			public bool Contains(KeyValuePair<TKey, TValue> item)
			{
				return this._dictionary.Contains(item);
			}

			public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
			{
				this._dictionary.CopyTo(array, arrayIndex);
			}

			public bool Remove(KeyValuePair<TKey, TValue> item)
			{
				return this._dictionary.Remove(item);
			}

			public bool ContainsKey(TKey key)
			{
				return this._dictionary.ContainsKey(key);
			}

			public void Add(TKey key, TValue value)
			{
				this._dictionary.Add(key, value);
			}

			public bool Remove(TKey key)
			{
				return this._dictionary.Remove(key);
			}

			public bool TryGetValue(TKey key, out TValue value)
			{
				value = this[key];
				return true;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return (IEnumerator)this.GetEnumerator();
			}
		}
	}
}
