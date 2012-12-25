using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeatmapTool.Core.Base
{
    public class CoreData<TKey, TValue> : IDictionary<TKey, TValue>
    {
        Dictionary<object, Object<TValue>> _Keys = new Dictionary<object, Object<TValue>>();
        Dictionary<int, Object<TValue>> _Values = new Dictionary<int, Object<TValue>>();

        #region IDictionary<TKey,TValue> 成员

        public void Add(TKey key, TValue value)
        {
            if (_Keys.ContainsKey(key))
            {
                _Keys[key].Value = value;
                return;
            }
            Object<TValue> obj = null;
            if (!_Values.TryGetValue(value.GetHashCode(), out obj))
            {
                obj = new Object<TValue>(value);
                _Values.Add(value.GetHashCode(), obj);
            }
            _Keys.Add(key, obj);
        }

        public bool ContainsKey(TKey key)
        {
            return _Keys.ContainsKey(key);
        }

        public bool ContainsKey(object key)
        {
            return _Keys.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get { throw new NotSupportedException(); }
        }

        public bool Remove(TKey key)
        {
            var x = _Keys[key];
            bool result = _Keys.Remove(key);
            _Values.Remove(x.Value.GetHashCode());
            return result;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            Object<TValue> v = new Object<TValue>();
            bool result = _Keys.TryGetValue(key, out v);
            value = v.Value;
            return result;
        }

        public ICollection<TValue> Values
        {
            get
            {
                ICollection<TValue> ic = new List<TValue>();
                foreach (TValue v in _Values.Values)
                {
                    ic.Add(v);
                }
                return ic;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                return _Keys[key].Value;
            }
            set
            {
                var tmp = _Keys[key];
                _Values.Remove(_Keys[key].Value.GetHashCode());
                if (!_Values.ContainsKey(value.GetHashCode()))
                    _Values.Add(value.GetHashCode(), tmp);
                tmp = value;
            }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> 成员

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _Keys.Clear();
            _Values.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _Keys.ContainsKey(item.Key) && _Keys.ContainsValue(item.Value);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return _Values.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> 成员

        System.Collections.Generic.IEnumerator<KeyValuePair<TKey, TValue>> System.Collections.Generic.IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IEnumerable 成员

        public System.Collections.IEnumerator GetEnumerator()
        {
            return _Values.GetEnumerator();
        }

        #endregion
    }
}
