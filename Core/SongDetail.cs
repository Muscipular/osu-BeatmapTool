using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BeatmapTool.DataAccess;

namespace BeatmapTool.Core
{
    public abstract class SongDetail : Base, IComparable<SongDetail>
    {
        public virtual string Artist { get; set; }
        public virtual string Audio { get; set; }
        public virtual string Creator { get; set; }
        public virtual int? CreatorId { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual int DiffCount { get; set; }
        public virtual Genre? Genre { get; set; }
        public virtual Language? Lang { get; set; }
        public virtual string Pack { get; protected set; }
        public abstract string Path { get; set; }
        public virtual int? Id { get; set; }
        public virtual string Source { get; set; }
        public virtual string Tag { get; set; }
        public virtual string Title { get; set; }
        public abstract SongType Type { get; }
        public abstract bool HaveVideo { get; set; }
        public abstract string Background { get; set; }
        public virtual BeatmapFlag Flag { get; set; }
        public int? Tid { get; set; }
        List<Diff> _Diffs;
        public List<Diff> Diffs
        {
            get
            {
                if (_Diffs == null)
                {
                    _Diffs = new List<Diff>();
                    AddOrRefeshObject(this);
                }
                return _Diffs;
            }
            protected set
            {
                _Diffs = value;
            }
        }
        protected override void Release()
        {
            Diffs = null;
        }
        protected SongDetail() { }
        protected SongDetail(BeatmapSet song)
        {
            if (song.Type.GetValueOrDefault(0) != (int)this.Type)
                throw new ArgumentException("参数应该为: " + this.Type.ToString());
            Artist = song.Artist;
            Audio = song.Audio;
            Creator = song.Creator;
            CreatorId = song.CreatorId;
            Date = song.Date.GetValueOrDefault();
            DiffCount = song.DiffCount ?? 0;
            Genre = song.Genre == null ? (Genre?)null : (Genre)song.Genre;
            Lang = song.Genre == null ? (Language?)null : (Language)song.Lang;
            Pack = song.Pack;
            Path = song.Path;
            Id = song.Sid;
            Source = song.Source;
            Tag = song.Tag;
            Title = song.Title;
            HaveVideo = song.Video.GetValueOrDefault().To<bool>().Value;
            Flag = (BeatmapFlag)song.Flag.GetValueOrDefault();
            Tid = song.Tid;
        }

        public int CompareTo(SongDetail other)
        {
            return this.Id.GetValueOrDefault(0) - other.Id.GetValueOrDefault(0);
        }
        public override string ToString()
        {
            string str = string.Empty;
            if (!string.IsNullOrWhiteSpace(Artist))
                str += Artist + " - ";
            str = string.Format("{0}{1} (By {2})", str, Title, Creator);
            return str;
        }
        public abstract string ToString(Format format);
    }
}
