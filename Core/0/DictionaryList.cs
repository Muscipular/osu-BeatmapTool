using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace BeatmapTool
{
    public class DictionaryList<T, T2> : IEnumerable<DictionaryObject<T, T2>>
    {
        protected List<Object<T>> _Column = new List<Object<T>>();
        protected List<Object<T2>> _SortType = new List<Object<T2>>();

        public virtual DictionaryObject<T, T2> this[int index]
        {
            get
            {
                return new DictionaryObject<T, T2>(_Column[index], _SortType[index]);
            }
            set
            {
                _Column[index].Value = value.Key;
                _SortType[index].Value = value.Value;
            }
        }

        public T2 this[T Column]
        {
            get
            {
                return _SortType[_Column.IndexOf(Column)].Value;
            }
            set
            {
                int index = _Column.IndexOf(Column);
                if (index > -1)
                {
                    _SortType[index].Value = value;
                    return;
                }
                _Column.Add(Column);
                _SortType.Add(value);
            }
        }

        public int Count
        {
            get { return _Column.Count; }
        }

        public void Add(T Column, T2 Type)
        {
            if (Contains(Column))
                throw new ArgumentException("已存在键\"" + Column + "\"");
            _Column.Add(Column);
            _SortType.Add(Type);
        }

        public void Add(DictionaryObject<T, T2> condition)
        {
            if (Contains(condition))
                throw new ArgumentException("已存在键\"" + condition.Key + "\"");
            _Column.Add(condition.Key);
            _SortType.Add(condition.Value);
        }

        private bool Contains(DictionaryObject<T, T2> condition)
        {
            return Contains(condition.Key);
        }

        protected bool Contains(T column)
        {
            foreach (Object<T> str in _Column)
            {
                if (str.Value.Equals(column))
                    return true;
            }
            return false;
        }

        public override string ToString()
        {
            if (Count == 0)
                return "";
            StringBuilder sb = new StringBuilder(" order by");
            for (int i = 0; i < Count; i++)
            {
                sb.AppendFormat(" {0} {1},", _Column[i].Value, _SortType[i].ToString());
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        #region IEnumerable<SortCondition> 成员

        public IEnumerator<DictionaryObject<T, T2>> GetEnumerator()
        {
            return new Enumerator(_Column, _SortType, this);
        }

        #endregion

        #region IEnumerable 成员

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public class Enumerator : IEnumerator<DictionaryObject<T, T2>>
        {
            IEnumerator<Object<T>> _x;
            List<Object<T>> _c;
            List<Object<T2>> _t;
            DictionaryList<T, T2> _l;
            internal Enumerator(List<Object<T>> c, List<Object<T2>> t, DictionaryList<T, T2> l)
            {
                _c = c;
                _t = t;
                _x = c.GetEnumerator();
                _l = l;
            }

            #region IEnumerator<SortCondition> 成员

            public DictionaryObject<T, T2> Current
            {
                get
                {
                    if (_x.Current == null)
                        return null;
                    return _l[_c.IndexOf(_x.Current)];
                }
            }

            #endregion

            #region IDisposable 成员

            public void Dispose()
            {
                //throw new NotImplementedException();
            }

            #endregion

            #region IEnumerator 成员

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                return _x.MoveNext();
            }

            public void Reset()
            {
                _x.Reset();
            }

            #endregion
        }
        #endregion
    }

    public class DictionaryObject<T, T2>
    {
        protected Object<T> _Column = new Object<T>();
        protected Object<T2> _SortTypre = new Object<T2>();
        public T2 Value
        {
            get
            {
                return _SortTypre.Value;
            }
            set
            {
                _SortTypre.Value = value;
            }
        }
        public T Key
        {
            get
            {
                return _Column.Value;//.ToString();
            }
            set
            {
                _Column.Value = value;
            }
        }
        public DictionaryObject(T Column, T2 Type)
        {
            _Column = Column;
            _SortTypre = Type;
        }
        public DictionaryObject(Object<T> Column, Object<T2> Type)
        {
            _Column = Column;
            _SortTypre = Type;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(' ');
            sb.Append(Key);
            sb.Append(' ');
            sb.Append(_SortTypre.ToString());
            return sb.ToString();
        }
    }
}
