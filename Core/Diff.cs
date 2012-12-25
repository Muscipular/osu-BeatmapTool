using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using BeatmapTool.DataAccess;
using System.Collections.Specialized;
using System.Linq;

namespace BeatmapTool.Core
{
    public class Diff
    {
        protected string _Path;
        public SongDetail Song { get; set; }
        public string Artist { get { return Song == null ? null : Song.Artist; } }
        public string Title { get { return Song == null ? null : Song.Title; } }
        public string Creator { get { return Song == null ? null : Song.Creator; } }
        public int? CreatorId { get { return Song == null ? null : Song.CreatorId; } }
        public int? Sid { get { return Song == null ? null : Song.Id; } }
        public int? Tid { get { return Song == null ? null : Song.Tid; } }
        public int? Id { get; set; }
        public string Md5Hash { get; set; }
        public int? OverallDifficulty { get; set; }
        public int? ApproachRate { get; set; }
        public string Audio { get; set; }
        public int? AudioLeadIn { get; set; }
        public string Background { get; set; }
        public double? Bpm { get; set; }
        public int? CircleSize { get; set; }
        public string Difficulty { get; set; }
        public int? DrainingTime { get; set; }
        public int? HPDrainRate { get; set; }
        public int? MaxCombo { get; set; }
        public int? Mode { get; set; }
        public int? NodeCount { get; set; }
        public double? Ops { get; set; }
        public string Path
        {
            get
            {
                if (Song.Type == SongType.Local)
                    return _Path;
                if (Id.HasValue)
                    return string.Concat("http://osu.ppy.sh/b/", Id);
                if (Song.Id.HasValue)
                    return string.Concat("http://osu.ppy.sh/s/", this.Sid);
                return null;
            }
            set
            {
                if (Song.Type == SongType.Local)
                {
                    _Path = value;
                    return;
                }
                throw new NotSupportedException();
            }
        }
        public int? PreviewTime { get; set; }
        public int? SliderCount { get; set; }
        public double? SliderMultiplier { get; set; }
        public double? SliderTickRate { get; set; }
        public int? SpinnerCount { get; set; }
        public string Tag { get; set; }

        public Diff(string path, SongDetail song)
        {
            this.Song = song;
            var meta = MetaParser.Parse(path);
            this.Md5Hash = meta["Hash"];
            this.OverallDifficulty = meta["OverallDifficulty"].To<int>();
            this.ApproachRate = meta["ApproachRate"].To<int>() ?? this.OverallDifficulty;
            this.Audio = meta["AudioFilename"];
            this.AudioLeadIn = meta["AudioLeadIn"].To<int>();
            this.Background = meta["Background"];
            this.Bpm = meta["Bpm"].To<double>();
            this.CircleSize = meta["CircleSize"].To<int>();
            this.Difficulty = meta["Version"];
            this.DrainingTime = meta["DrainingTime"].To<int>();
            this.HPDrainRate = meta["HPDrainRate"].To<int>();
            this.MaxCombo = meta["MaxCombo"].To<int>();
            this.Mode = meta["Mode"].To<int>();
            this.NodeCount = meta["NodeCount"].To<int>();
            this.Ops = meta["Ops"].To<double>();
            //this.Path = meta["Path"];
            this.PreviewTime = meta["PreviewTime"].To<int>();
            this.SliderCount = meta["SliderCount"].To<int>();
            this.SliderMultiplier = meta["SliderMultiplier"].To<double>();
            this.SliderTickRate = meta["SliderTickRate"].To<double>();
            this.SpinnerCount = meta["SpinnerCount"].To<int>();
            this.Tag = meta["Tags"];
        }

        public Diff(Beatmap beatmap, SongDetail song)
        {
            this.Song = song;
            this.Md5Hash = beatmap.Hash;
            this.OverallDifficulty = beatmap.OverallDifficulty;
            this.ApproachRate = beatmap.ApproachRate;
            this.Audio = beatmap.Audio;
            this.AudioLeadIn = beatmap.AudioLeadIn;
            this.Background = beatmap.Background;
            this.Bpm = beatmap.Bpm;
            this.CircleSize = beatmap.CircleSize;
            this.Difficulty = beatmap.Difficulty;
            this.DrainingTime = beatmap.DrainingTime;
            this.HPDrainRate = beatmap.HPDrainRate;
            this.MaxCombo = beatmap.MaxCombo;
            this.Mode = beatmap.Mode;
            this.NodeCount = beatmap.NodeCount;
            this.Ops = beatmap.Ops;
            this.Path = beatmap.Path;
            this.PreviewTime = beatmap.PreviewTime;
            this.SliderCount = beatmap.SliderCount;
            this.SliderMultiplier = beatmap.SliderMultiplier;
            this.SliderTickRate = beatmap.SliderTickRate;
            this.SpinnerCount = beatmap.SpinnerCount;
            this.Tag = beatmap.Tag;
            this.Id = beatmap.Bid;
        }

        public override string ToString()
        {
            return Md5Hash;
        }

        public string ToString(Format format)
        {
            return null;
        }
    }
}
