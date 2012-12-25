using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using BeatmapTool.DataAccess;

namespace BeatmapTool.Core
{
    public class RankedSong : SongDetail
    {
        protected int _DiffCount = -1;
        public override string Background
        {
            get
            {
                return string.Concat("http://d.osu.ppy.sh/mt/", this.Id);
            }
            set { throw new NotSupportedException("不支持写入"); }
        }
        public override string Audio
        {
            get
            {
                return string.Concat("http://d.osu.ppy.sh/mp3/preview/", this.Id);
            }
            set { throw new NotSupportedException("不支持写入"); }
        }
        public override string Path
        {
            get { return string.Concat("http://osu.ppy.sh/s/", this.Id.Value); }
            set { throw new NotSupportedException("不支持写入"); }
        }
        public RankedSong(BeatmapSet song)
            : base(song)
        {
        }
        //public RankedSong(DataBlock data)
        //{
        //    Load(data);
        //}

        public RankedSong(int id, string title, string artist, string creator, DateTime date, Genre genre, Language language,
            string pack, bool isHaveVideo, string source, bool isApproved, int diffCount)
        {
            this.Id = id;
            this.Title = title;
            this.Artist = artist;
            this.Creator = creator;
            this.Pack = pack;
            this.Genre = genre;
            this.Lang = language;
            this.Date = date;
            this.Source = source;
            this.Flag = isApproved ? BeatmapFlag.Approved : BeatmapFlag.Ranked;
            this._DiffCount = diffCount;
        }

        public override string ToString(Format format)
        {
            switch (format)
            {
                //case Format.Download: return _Id + " " + (string.IsNullOrWhiteSpace(_Artist) ? "" : _Artist + " - ") + _Title;
                //case Format.All: return string.Format("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}\n{8}", _Id, _Title, _Artist, _Creator, _Date.ToShortDateString(), _Genre.ToString(), _Lang.ToString(), _Pack, _Source);
                //case Format.Search: return string.Format("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}\n{8}\n{9}", _Id, _Title, _Artist, _Creator, _Date.ToShortDateString().Replace('/', '-'), _Genre.ToString(), _Lang.ToString(), _Pack, _Source, _IsAPP ? " Approved" : "");
                default: return this.ToString();
            }
        }

        public override SongType Type
        {
            get { return SongType.Ranked; }
        }
        protected bool _HaveVideo;
        public override bool HaveVideo
        {
            get { return _HaveVideo; }
            set { _HaveVideo = value; }
        }
    }
}
