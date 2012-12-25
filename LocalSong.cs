using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace BeatmapTool
{
    //[Serializable]
    public class LocalSong : SongDetail
    {
        //private string bg;
        private string tags;
        private string path;
        List<BeatmapDetail> beatmaps;
        private static string[] creator_changedname = { "Echo49", "Echo" };
        private static string[] specialTitle = { "DISCO★PRINCE", "DISCO PRINCE","Angry Video Game Nerd Theme", "Angry Video Game Nerd Theme [MATURE CONTENT]" };

        public string Path { get { return path; } }
        public string Tags { get { return tags; } }
        public int DifficultyCount { get { return beatmaps.Count; } }
        public BeatmapDetail[] Difficulties { get { return beatmaps.ToArray(); } }
        public string BG
        {
            get
            {
                List<string> bgs = Core.GetBg(this);
                if (bgs.Count == 0) return string.Empty;
                Random rnd = new Random(DateTime.Now.Millisecond*id);
                return path + "\\" + bgs[rnd.Next(0, bgs.Count)];
                //return (beatmaps[0].BG == "") ? "" : path + "\\" + beatmaps[0].BG;
            }
        }

        public LocalSong() {}
        public LocalSong(int id,string path)
        {
            Init(id, path);
        }

        private void Init(int id, string path)
        {
            tags = string.Empty;
            beatmaps = new List<BeatmapDetail>();
            this.id = id;
            this.path = path;
            foreach (string file in Directory.GetFiles(path))
            {
                if (System.IO.Path.GetExtension(file).ToLower() == ".osu")
                {
                    diffCount++;
                    BeatmapDetail beatmap = null;
                    if (string.IsNullOrWhiteSpace(creator))
                    {
                        string[] info = new string[4] { string.Empty, string.Empty, string.Empty, string.Empty };
                        beatmap = new BeatmapDetail(file, info);
                        title = info[0];
                        artist = info[1];
                        creator = info[2];
                        source = info[3];
                    }
                    else
                    {
                        beatmap = new BeatmapDetail(file);
                    }
                    beatmaps.Add(beatmap);
                }
            }

            replace(ref title, specialTitle);
            replace(ref creator, creator_changedname);

            tags = getTags();
            date = Directory.GetLastWriteTime(path);
            if (id > 99999999)
            {
                RankedList list = Core.RankedList;
                //foreach (RankedSong s in list)
                //{
                //    if (s.Artist == this.artist && s.Creator == this.creator && s.Title == this.title)
                //    {
                //        this.id = s.Id;
                //        break;
                //    }
                //}
                var xxx = from s in list where s.Artist == this.Artist && s.Creator == this.Creator && s.Title == this.Title select s;
                if (xxx.Count() > 0)
                {
                    this.id = xxx.ElementAt(0).Id;
                }
            }
        }

        private void replace(ref string obj, string[] list)
        {
            for (int i = 0; i < list.Length; i += 2)
                if (obj == list[i])
                    obj = list[i + 1];
        }

        private string getDiffsName()
        {
            //string str="";
            StringBuilder sb = new StringBuilder();
            foreach (BeatmapDetail b in beatmaps)
            {
                sb.AppendLine(b.Difficulty);
            }
            return sb.ToString();
        }

        public override string ToString(Format format)
        {
            switch (format)
            {
                case Format.Search:
                case Format.All:
                    return string.Format("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}\n{8}",id, title, artist, creator, source, tags, path, getDiffsName(), date.ToLongDateString());
                case Format.ReplaySearch: return string.Format("{0}{1}{2}{3}",id, artist, title, creator);
                case Format.Normal: return string.Format("{0} - {1}", artist, title);
                //case Format.Replay: 
                //case Format.None:
                default: return this.ToString();
            }
        }

        public void Reload()
        {
            Init(this.id, this.path);
        }

        private string getTags()
        {
            List<string> taglist = new List<string>();
            foreach (BeatmapDetail beatmap in Difficulties)
            {
                if (!taglist.Contains(beatmap.Tags))
                    taglist.Add(beatmap.Tags);
            }
            List<string> result = new List<string>();
            foreach (string tags in taglist)
            {
                foreach (string tag in tags.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string tStr = tag.Trim();
                    if (!string.IsNullOrWhiteSpace(tStr) && !result.Contains(tStr))
                        result.Add(tStr);
                }
            }
            RankedSong song = new RankedSong();
            if (song != null && song.DiffCount > DiffCount)
            {
                result.Add("缺失难度LostDiff");
            }
            return string.Concat(result).Trim();
        }

    }

}
