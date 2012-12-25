using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeatmapTool
{
    [Flags]
    public enum SongListType
    {
        NotSet = 0,
        Ranked = 1,
        Local = 2,
        Dirty = 4,
        NeedDownload = 8
    }


    //public enum SortType
    //{
    //    ASC = 0,
    //    DESC = 1
    //}

    //public enum DataType
    //{
    //    Ranked = 0,
    //    Local,
    //    Diff,
    //    Replay,
    //    Setting
    //}

    [Flags]
    public enum HitObjectType
    {
        Node = 1,
        Slider = 2,
        NewCombo = 4,
        Spinner = 8
    }

    public enum Format
    {
        None,
        All,
        Replay,
        Normal,
        Search,
        Download,
        Save,
        ReplaySearch
    }

    [Flags]
    public enum Mods
    {
        Autoplay = 0x800,
        DoubleTime = 0x40,
        Easy = 2,
        Flashlight = 0x400,
        HalfTime = 0x100,
        HardRock = 0x10,
        Hidden = 8,
        LastMod = 0x4000,
        NoFail = 1,
        None = 0,
        NoVideo = 4,
        Relax = 0x80,
        Relax2 = 0x2000,
        SpunOut = 0x1000,
        SuddenDeath = 0x20
    }
}
