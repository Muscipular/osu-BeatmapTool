using System;
using System.Collections;
using System.Collections.Generic;

namespace BeatmapTool
{
    //[Serializable]
    public class SongDetail : IComparable<SongDetail>
    {
        protected int id;
        protected string title;
        protected string artist;
        protected string creator;
        protected string source;
        protected DateTime date;
        protected int diffCount;

        public SongDetail() { }
        public SongDetail(int id, string title, string artist, string creator, string path)
        {
            this.id = id;
            this.title = title;
            this.artist = artist;
            this.creator = creator;
        }

        public int Id { get { return id; } }
        public string Title { get { return title; } }
        public string Artist { get { return artist; } }
        public string Creator { get { return creator; } }
        public string Source { get { return source; } }
        public DateTime Date { get { return date; } }
        public int DiffCount { get { return diffCount; } }

        public int CompareTo(SongDetail other)
        {
            return this.Id - other.Id;
        }
        public override string ToString()
        {
            string str = string.Empty;
            if (!string.IsNullOrWhiteSpace(artist)) str += artist + " - ";
            str = string.Format("{0}{1} (By {2})", str, title, creator);
            return str;
        }
        public virtual string ToString(Format format)
        {
            return "";
        }
    }

    public enum Format { None, All, Replay, Normal, Search, Download, Save, ReplaySearch }
}
