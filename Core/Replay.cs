using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace BeatmapTool.Core
{
    public class Replay
    {
        LocalSong song;
        Diff beatmap;
        string player;
        uint score;
        uint hit300;
        uint hit100;
        uint hit50;
        uint miss;
        uint maxCombo;
        double acc;
        string path;
        string mod;
        DateTime time;
        string ranking;

        public string Hit300 { get { return hit300.ToString(); } }
        public string Hit100 { get { return hit100.ToString(); } }
        public string Hit50 { get { return hit50.ToString(); } }
        public string Miss { get { return miss.ToString(); } }
        public string Acc { get { return acc.ToString("P4"); } }
        public string MaxCombo { get { return maxCombo.ToString() + "x"; } }
        public string Path { get { return path; } }
        public string Mod { get { return mod; } }
        public string Player { get { return player; } }
        public string Time { get { return time.ToString(); } }
        public string SongInfo { get { return song.ToString(Format.Normal); } }
        public string Title { get { return song.Title; } }
        public string Artist { get { return song.Artist; } }
        public string Creator { get { return song.Creator; } }
        public LocalSong Song { get { return song; } }
        public string Difficulty { get { return beatmap.Difficulty; } }
        public string BG { get { return beatmap.Background; } }
        public string Score { get { return score.ToString(); } }
        public string Ranking { get { return ranking; } }

        public override string ToString()
        {
            return string.Format("{2}\nPlayer:{1} diff:[{0}]", beatmap.Difficulty, player, song.ToString());
            //return string.Format("{3}\nPlayer:{1} diff:[{0}] Score:{2}", beatmap.Difficulty, player, score, song.ToString());
        }
        public string ToString(Format format)
        {
            switch (format)
            {
                case Format.Search: return string.Format("{0} {1} {2} {3} {4}", Player, Difficulty, Mod, Player, SongInfo);
                case Format.Save: return string.Format("{0} - {1} - {2} [{3}] ({4}-{5}-{6}) Osu.osr", Player, song.Artist, song.Title, Difficulty, time.Year, time.Month, time.Day);
                default: return ToString();
            }
        }

        public Replay() { }
        public Replay(string path, LocalSong song, Diff beatmap, byte[] buffer)
        {
            this.path = path;
            this.song = song;
            this.beatmap = beatmap;
            int i = 0;
            int n = buffer[40];
            player = "";
            for (i = 41; i < 41 + n; i++)
            {
                player += (char)buffer[i];
            }
            i += 34;
            hit300 = (uint)(buffer[i++] | buffer[i++] << 8);
            hit100 = (uint)(buffer[i++] | buffer[i++] << 8);
            hit50 = (uint)(buffer[i++] | buffer[i++] << 8);
            i += 4;
            miss = (uint)(buffer[i++] | buffer[i++] << 8);
            score = (uint)(buffer[i++] | buffer[i++] << 8 | buffer[i++] << 16 | buffer[i++] << 24);
            maxCombo = (uint)(buffer[i++] | buffer[i++] << 8);
            i++;
            /*
                0000 none   0000 0000 0000 0000
                0010 so     0000 0000 0001 0000
                0400 nv     0000 0100 0000 0000
                8000 relax  1000 0000 0000 0000
                0020 ap     0000 0000 0010 0000
                0008 auto   0000 0000 0000 1000
                1000 hr     0001 0000 0000 0000
                2000 sd     0010 0000 0000 0000
                4000 dt     0100 0000 0000 0000
                0800 hid    0000 1000 0000 0000
                0004 fl     0000 0000 0000 0100
                0200 easy   0000 0010 0000 0000
                0100 nf     0000 0001 0000 0000
                0001 ht     0000 0000 0000 0001
            */
            StringBuilder mod = new StringBuilder();/*
            if ((buffer[i] & 0x04) != 0) mod += "+NoVideo";
            if ((buffer[i] & 0x02) != 0) mod += ((mod == "") ? "" : "+") + "Easy";
            if ((buffer[i] & 0x01) != 0) mod += ((mod == "") ? "" : "+") + "NoFail";
            if ((buffer[i + 1] & 0x01) != 0) mod += ((mod == "") ? "" : "+") + "HaftTime";
            if ((buffer[i] & 0x10) != 0) mod += ((mod == "") ? "" : "+") + "HardRock";
            if ((buffer[i] & 0x20) != 0) mod += ((mod == "") ? "" : "+") + "SuddenDeath";
            if ((buffer[i] & 0x40) != 0) mod += ((mod == "") ? "" : "+") + "DoubleTime";
            if ((buffer[i] & 0x08) != 0) mod += ((mod == "") ? "" : "+") + "Hidden";
            if ((buffer[i + 1] & 0x04) != 0) mod += ((mod == "") ? "" : "+") + "FlashLight";
            if ((buffer[i + 1] & 0x10) != 0) mod += ((mod == "") ? "" : "+") + "SpunOut";
            if ((buffer[i] & 0x80) != 0) mod += ((mod == "") ? "" : "+") + "Relax";
            if ((buffer[i + 1] & 0x20) != 0) mod += ((mod == "") ? "" : "+") + "AutoPilot";
            if ((buffer[i + 1] & 0x08) != 0) mod += ((mod == "") ? "" : "+") + "Auto";
            //if (mod != "") mod = "(" + mod + ")";
            //else mod = "(+None)";*/
            if ((buffer[i] & 0x04) != 0) mod.Append("+NoVideo");
            if ((buffer[i] & 0x02) != 0) mod.Append("+Easy");
            if ((buffer[i] & 0x01) != 0) mod.Append("+NoFail");
            if ((buffer[i + 1] & 0x01) != 0) mod.Append("+HaftTime");
            if ((buffer[i] & 0x10) != 0) mod.Append("+HardRock");
            if ((buffer[i] & 0x20) != 0) mod.Append("+SuddenDeath");
            if ((buffer[i] & 0x40) != 0) mod.Append("+DoubleTime");
            if ((buffer[i] & 0x08) != 0) mod.Append("+Hidden");
            if ((buffer[i + 1] & 0x04) != 0) mod.Append("+FlashLight");
            if ((buffer[i + 1] & 0x10) != 0) mod.Append("+SpunOut");
            if ((buffer[i] & 0x80) != 0) mod.Append("+Relax");
            if ((buffer[i + 1] & 0x20) != 0) mod.Append("+AutoPilot");
            if ((buffer[i + 1] & 0x08) != 0) mod.Append("+Auto");
            if (mod.Length == 0) this.mod = "+None";
            else this.mod = mod.ToString();
            i += 4;
            if (buffer[i++] == 0x0B)
            {
                time = File.GetLastWriteTime(path);
            }
            else
                time = DateTime.FromBinary((buffer[i++] | buffer[i++] << 8 | buffer[i++] << 16 | buffer[i++] << 24
                    | buffer[i++] << 32 | buffer[i++] << 40 | buffer[i++] << 48 | buffer[i++] << 56));
            acc = ((double)hit300) / (hit300 + hit100 + hit50 + miss);
            if (acc == 1)
            {
                if (Mod.Contains("Hidden") || Mod.Contains("FlashLight")) ranking = "XH";
                else ranking = "SH";
            }
            else if (acc >= 0.9)
            {
                if (((double)hit50) / (hit300 + hit100 + hit50 + miss) > 0.01 || miss != 0) ranking = "A";
                else if (Mod.Contains("Hidden") || Mod.Contains("FlashLight")) ranking = "X";
                else ranking = "S";
            }
            else if (acc >= 0.8 && miss == 0) ranking = "A";
            else if (acc >= 0.7 && miss == 0 || acc >= 0.8) ranking = "B";
            else if (acc >= 0.6) ranking = "C";
            else ranking = "D";

            acc = (hit100 * 2.0 + hit300 * 6.0 + hit50) / ((hit100 + hit300 + hit50 + miss) * 6.0);
        }

    }
    public class ReplayComparer : System.Collections.IComparer, IComparer<Replay>
    {
        public int Compare(object x, object y)
        {
            Replay a = x as Replay;
            Replay b = y as Replay;
            if (a == null || b == null)
            {
                throw new FormatException("输入格式错误");
            }
            return Compare(a, b);
        }

        public int Compare(Replay x, Replay y)
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
            if (!string.Equals(x.Difficulty, y.Difficulty))
            {
                resualt = x.Difficulty.CompareTo(y.Difficulty);
                return resualt;
            }
            if (!string.Equals(x.Score, y.Score))
            {
                resualt = x.Score.CompareTo(y.Score);
                return resualt;
            }

            return resualt;
        }
    }
}
