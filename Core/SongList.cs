using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Linq;

namespace BeatmapTool.Core
{
    public class SongList : INotifyPropertyChanged, INotifyCollectionChanged,
        IEnumerable<SongDetail>, IList<SongDetail>, IList
    {
        protected SongListType _Type = SongListType.NotSet;
        protected int[] _RankLast = null;
        protected int[] _AppLast = null;
        public SongListType Type
        {
            get { return _Type; }
        }
        public int[] RankLast
        {
            get { return _RankLast; }
            set { _RankLast = value; }
        }
        public int[] AppLast
        {
            get { return _AppLast; }
            set { _AppLast = value; }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        protected List<SongDetail> list;
        public SongList(IEnumerable<SongDetail> songList, int[] rankLast = null, int[] appLast = null)
        {
            _AppLast = appLast;
            if (appLast == null)
                _AppLast = new int[] { -1, -1, -1 };
            _RankLast = rankLast;
            if (_RankLast == null)
                _RankLast = new int[] { -1, -1, -1 };
            list = new List<SongDetail>(songList);
        }
        public SongList()
        {
            _AppLast = new int[] { -1, -1, -1 };
            _RankLast = new int[] { -1, -1, -1 };
            list = new List<SongDetail>();
        }
        ~SongList()
        {
            list.Clear();
            list.TrimExcess();
            list = null;
            Release();
        }
        public void Add(SongDetail song)
        {
            list.Add(song);
            OnListAdd(song);
            OnPropertyChanged();
        }
        public void AddRange(SongDetail[] songs)
        {
            list.AddRange(songs);
            OnListAdd(songs);
            OnPropertyChanged();
        }
        public void Sort()
        {
            IComparer<SongDetail> cp = new SongIdComparer();
            list.Sort(cp);
            list.TrimExcess();
        }
        public bool Remove(SongDetail song)
        {
            SongDetail song2 = this[(long)song.Id];
            int index = list.IndexOf(song2);
            if (index < 0 || song2 == null)
                return false;
            OnListRemove(song2, index);
            list.Remove(song2);
            OnPropertyChanged();
            return true;
        }
        public void RemoveAt(int index)
        {
            SongDetail song = list[index];
            OnListRemove(song, index);
            list.RemoveAt(index);
            OnPropertyChanged();
        }
        public void Clear()
        {
            for (int i = 0; i < 3; i++)
                _AppLast[i] = _RankLast[i] = 0;
            list.Clear();
            list.TrimExcess();
            OnListReset();
            OnPropertyChanged();
        }
        public int Count
        {
            get { return list.Count; }
        }
        public SongDetail this[int index]
        {
            get { return list[index]; }
            set { throw new NotSupportedException(); }
        }
        public SongDetail this[long index]
        {
            get
            {
                return Search((int)(index), 0, this.Count - 1);
            }
        }
        public SongDetail this[string path]
        {
            get
            {
                var o = from s in list.AsParallel() where s.Path == path select s;
                return o.FirstOrDefault();
            }
        }
        protected SongDetail Search(int id, int a, int b)
        {
            if (a < 0 || b < 0 || b >= this.Count) return null;
            if (a == b) if (list[a].Id == id) return list[a]; else return null;
            int mid = (a + b) / 2;
            if (this[mid].Id < id)
            {
                if (mid == a)
                    if (list[b].Id == id) return this[b];
                    else return null;
                else return Search(id, mid, b);
            }
            else
            {
                if (this[mid].Id == id) return this[mid];
                else return Search(id, a, mid);
            }
        }
        public SongDetail[] ToArray()
        {
            return list.ToArray();
        }
        public IEnumerator<SongDetail> GetEnumerator()
        {
            return list.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        protected void OnPropertyChanged()
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs("Count"));
            }
        }
        protected void OnListAdd(SongDetail song)
        {
            NotifyCollectionChangedEventHandler h = CollectionChanged;
            if (h != null)
            {
                NotifyCollectionChangedEventArgs x = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, song);
                h(this, x);
            }
        }
        protected void OnListAdd(SongDetail[] songs)
        {
            NotifyCollectionChangedEventHandler h = CollectionChanged;
            if (h != null)
            {
                NotifyCollectionChangedEventArgs x = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, songs);
                h(this, x);
            }
        }

        protected void OnListRemove(SongDetail song, int index)
        {
            NotifyCollectionChangedEventHandler h = CollectionChanged;
            if (h != null)
            {
                NotifyCollectionChangedEventArgs x = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, song, index);
                h(this, x);
            }
        }

        protected void OnListReset()
        {
            NotifyCollectionChangedEventHandler h = CollectionChanged;
            if (h != null)
            {
                NotifyCollectionChangedEventArgs x = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                h(this, x);
            }
        }

        public void Release()
        {
            CollectionChanged = null;
            //System.Windows.Data.CollectionViewSource a = new System.Windows.Data.CollectionViewSource();
            //a.Source = this;
        }


        public override string ToString()
        {
            return this.Count.ToString();
        }

        public int IndexOf(SongDetail item)
        {
            return list.IndexOf(item);
        }

        public void Insert(int index, SongDetail item)
        {
            throw new NotSupportedException();
        }

        public bool Contains(SongDetail item)
        {
            return list.Contains(item);
        }

        public void CopyTo(SongDetail[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public int Add(object value)
        {
            if (!(value is SongDetail))
                return -1;
            return (list as IList).Add(value);
        }

        public bool Contains(object value)
        {
            if (!(value is SongDetail))
                return false;
            return list.Contains((SongDetail)value);
        }

        public int IndexOf(object value)
        {
            SongDetail song = value as SongDetail;
            return IndexOf(song);
        }

        public void Insert(int index, object value)
        {
            throw new NotSupportedException();
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public void Remove(object value)
        {
            (list as IList).Remove(value);
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this[index] = (SongDetail)value;
            }
        }

        public void CopyTo(Array array, int index)
        {
            (list as IList).CopyTo(array, index);
        }

        public bool IsSynchronized
        {
            get { return (list as IList).IsSynchronized; }
        }

        public object SyncRoot
        {
            get { return (list as IList).SyncRoot; }
        }
    }
}
