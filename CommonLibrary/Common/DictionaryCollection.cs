using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Common
{
    public class DictionaryCollection<TKey, TValue>
    {
        private Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();
        private List<TValue> _list = new List<TValue>();

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get { return _dictionary[key]; }
        }

        public TValue this[int index]
        {
            get { return _list[index]; }
        }

        public void Add(TKey key, TValue value)
        {
            _dictionary.Add(key, value);
            _list.Add(value);
        }

        public List<TValue> Values
        {
            get { return _list; }
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public int IndexOf(TValue value)
        {
            return _list.IndexOf(value);
        }
    }
}
