using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Linq;

namespace BeatmapTool
{
    //[Serializable]
    public class SongList :  INotifyPropertyChanged, INotifyCollectionChanged,
        IEnumerable<SongDetail>, IList<SongDetail>,IList
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        protected List<SongDetail> list;
        protected bool islock;
        public SongList()
        {
            islock = false;
            list = new List<SongDetail>();
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
        }
        public bool Remove(SongDetail song)
        {
            SongDetail song2 = this[song.Id.ToString()];
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
        public bool Lock()
        {
            if (islock) return false;
            islock = true;
            return true;
        }
        public bool UnLock()
        {
            islock = false;
            return true;
        }
        public virtual void Clear()
        {
            list.Clear();
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
            set { throw new NotImplementedException(); }
        }
        public SongDetail this[string index]
        {
            get
            {
                /*int i=int.Parse(index);
                var xxx = from s in list where s.Id == i select s;
                if (xxx.Count() > 0)
                    return xxx.ElementAt(0);
                return null;*/
                return search(int.Parse(index), 0, this.Count - 1);
            }
        }
        public bool IsLock { get { return islock; } }
        protected SongDetail search(int id, int a, int b)
        {
            if (a < 0 || b < 0 || b >= this.Count) return null;
            if (a == b) if (list[a].Id == id) return list[a]; else return null;
            int mid = (a + b) / 2;
            if (this[mid].Id < id)
            {
                if (mid == a)
                    if (list[b].Id == id) return this[b];
                    else return null;
                else return search(id, mid, b);
            }
            else
            {
                if (this[mid].Id == id) return this[mid];
                else return search(id, a, mid);
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
                NotifyCollectionChangedEventArgs x = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,song);
                h(this, x);
            }
        }
        protected void OnListAdd(SongDetail[] songs)
        {
            NotifyCollectionChangedEventHandler h = CollectionChanged;
            if (h != null)
            {
                NotifyCollectionChangedEventArgs x = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,songs);
                h(this, x);
            }
        }

        protected void OnListRemove(SongDetail song,int index)
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
            throw new NotImplementedException();
        }


        public bool Contains(SongDetail item)
        {
            return list.Contains(item);
        }

        public void CopyTo(SongDetail[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool IsReadOnly
        {
            get { return false; }
        }


        public int Add(object value)
        {
            return (list as IList).Add(value);
        }

        public bool Contains(object value)
        {
            return list.Contains((SongDetail)value);
        }

        public int IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
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

    public class SongIdComparer : IComparer<SongDetail>
    {
        public int Compare(SongDetail x, SongDetail y)
        {
            return x.Id - y.Id;
        }
    }

    public class SongComarer : IComparer, IComparer<SongDetail>
    {

        public int Compare(object x, object y)
        {
            SongDetail a = x as SongDetail;
            SongDetail b = y as SongDetail;
            if (a == null || b == null)
            {
                throw new FormatException("输入格式错误");
            }
            return Compare(a, b);
        }

        public int Compare(SongDetail x, SongDetail y)
        {
            int resualt = 0;
            if (!string.Equals(x.Artist, y.Artist))
            {
                resualt = x.Artist.CompareTo(y.Artist);
                return resualt;
            }
            if (!string.Equals(x.Title, y.Title))
            {
                resualt = x.Title.CompareTo(y.Title);
                return resualt;
            }
            if (!string.Equals(x.Creator, y.Creator))
            {
                resualt = x.Creator.CompareTo(y.Creator);
                return resualt;
            }
            
            return resualt;
        }
    }
}
