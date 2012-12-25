using System;
using System.Collections.Generic;
using System.Text;

namespace BeatmapTool
{
    //[Serializable]
    public class RankedSong : SongDetail
    {
        private bool video;
        private string pack;
        private string genre;
        private string language;
        private bool approved;

        public bool Video { get { return video; } }
        public string Pack { get { return pack; } }
        public string Genre { get { return genre; } }
        public string Language { get { return language; } }
        public string Approved { get { return approved ? "Approved" : ""; } }

        public Format a
        {
            get { return Format.All; }
        }

        public RankedSong() { }
        public RankedSong(int id, string title, string artist, string creator,DateTime date, string genre, string language,
            string pack, bool video, string source, bool approved,int diffCount)
        {
            this.id = id;
            this.title = title;
            this.artist = artist;
            this.creator = creator;
            this.video = video;
            this.pack = pack;
            this.genre = genre;
            this.language = language;
            this.date = date;
            this.source = source;
            this.approved = approved;
            this.diffCount = diffCount;
        }

        public override string ToString(Format format)
        {
            switch (format)
            {
                case Format.Download: return id + " " + (string.IsNullOrWhiteSpace(artist) ? "" : artist + " - ") + title;
                case Format.All: return string.Format("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}\n{8}", id, title, artist, creator, date.ToShortDateString(), genre, language, pack, source);
                case Format.Search: return string.Format("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}\n{8}\n{9}", id, title, artist, creator, date.ToShortDateString().Replace('/', '-'), genre, language, pack, source, Approved);
                default: return this.ToString();
            }
        }

    }
}
