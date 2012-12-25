using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace BeatmapTool.Core
{
    public class SongIdComparer : IComparer<SongDetail>
    {
        public int Compare(SongDetail x, SongDetail y)
        {
            return x.CompareTo(y);
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
