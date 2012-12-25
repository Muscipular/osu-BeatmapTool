using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeatmapTool.Core
{
    public enum Language
    {
        Unkown = 0,
        CN = 1
    }

    public enum Genre
    {
        Unkown = 0,
        Anime = 1
    }

    public enum SongType
    {
        Local = 1,
        Ranked = 2
    }

    public enum BeatmapFlag
    {
        Unranked = 0,
        NotSubmit = 1,
        Ranked = 2,
        Approved = 3,
    }
}
