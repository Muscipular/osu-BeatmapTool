using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace BeatmapTool.Core
{
    public class RankedList : SongList
    {
        //int[] last;
        public RankedList()
            : base()
        {
            //last = new int[6] { 0, 0, 0, 0, 0, 0 };
            //list = new List<SongDetail>();
        }
        //public override void Clear()
        //{
        //    last = new int[6] { 0, 0, 0, 0, 0, 0 };
        //    list.Clear();
        //    list.TrimExcess();
        //    OnListReset();
        //    OnPropertyChanged();
        //}
        //public void CloneTo(RankedList objList)
        //{
        //    objList.Clear();
        //    objList.AddRange(this.list.ToArray());
        //    objList.OnListReset();
        //}
        //public int[] Last { get { return last; } set { this.last = value; } }
    }
}
