using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BeatmapTool.Core;

namespace BeatmapTool.DataAccess
{
    static class ModelHelper
    {
        public static void Load(this Beatmap _this,Diff diff)
        {
            _this.ApproachRate = diff.ApproachRate;
            _this.Audio = diff.Audio;
            _this.AudioLeadIn = diff.AudioLeadIn;
            _this.Background = diff.Background;
            _this.Bpm = diff.Bpm;
            _this.CircleSize = diff.CircleSize;
            _this.Difficulty = diff.Difficulty;
            _this.DrainingTime = diff.DrainingTime;
            _this.Hash = diff.Md5Hash;
            _this.HPDrainRate = diff.HPDrainRate;
            _this.MaxCombo = diff.MaxCombo;
            _this.Mode = diff.Mode;
            _this.NodeCount = diff.NodeCount;
            _this.Ops = diff.Ops;
            _this.OverallDifficulty = diff.OverallDifficulty;
            _this.Path = diff.Path;
            _this.PreviewTime = diff.PreviewTime;
            _this.Sid = diff.Sid;
            _this.SliderCount = diff.SliderCount;
            _this.SliderMultiplier = diff.SliderMultiplier;
            _this.SliderTickRate = diff.SliderTickRate;
            _this.SpinnerCount = diff.SpinnerCount;
            _this.Tag = diff.Tag;
        }
    }
}
