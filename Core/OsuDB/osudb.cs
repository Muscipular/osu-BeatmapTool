using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeatmapTool.Core.OsuDB
{
    public class BeatmapInfo
    {
        public virtual enum BeamapStatus : byte
        {
            /// <summary>
            /// 未知待测
            /// </summary>
            Unknow_0 = 0,
            /// <summary>
            /// 未知待测
            /// </summary>
            Unknow_3 = 3,
            /// <summary>
            /// 未提交
            /// </summary>
            NotSubmit = 1,
            /// <summary>
            /// Unrank
            /// </summary>
            Unrank = 2,
            /// <summary>
            /// Rank
            /// </summary>
            Ranked = 4,
            /// <summary>
            /// Approved
            /// </summary>
            Approved = 5
        }
        /// <summary>
        /// 游戏模式
        /// </summary>
        public virtual enum PlayModes : byte
        {
            Osu,
            Taiko,
            CatchTheBeat
        }
        /// <summary>
        /// 得分评价
        /// </summary>
        public virtual enum Ranking : byte
        {
            /// <summary>
            /// 白金SS
            /// Hidden/FlashLight + SS
            /// </summary>
            XH,
            /// <summary>
            /// 白金S
            /// Hidden/FlashLight + S
            /// </summary>
            SH,
            /// <summary>
            /// SS
            /// Acc:100%
            /// </summary>
            X,
            /// <summary>
            /// S
            /// 300数量>90%且无miss且50数量少于1%
            /// </summary>
            S,
            /// <summary>
            /// A
            /// 1.300数量大于90%且未到达S条件
            /// 2.300数量大于80%且无miss
            /// </summary>
            A,
            /// <summary>
            /// A
            /// 1.300数量大于80%且未到达A条件
            /// 2.300数量大于70%且无miss
            /// </summary>
            B,
            /// <summary>
            /// A
            /// 1.300数量大于70%且未到达B条件
            /// 2.300数量大于60%且无miss
            /// </summary>
            C,
            /// <summary>
            /// 最低评价
            /// </summary>
            D,
            /// <summary>
            /// 智商>65535
            /// </summary>
            F,
            /// <summary>
            /// 智商>4294967295
            /// </summary>
            N
        }
        /// <summary>
        /// Timming集合
        /// </summary>
        public virtual List<TimmingPoint> Timmings { get; set; }
        /// <summary>
        /// 艺术家
        /// </summary>
        public virtual string Artist { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public virtual string Title { get; set; }
        /// <summary>
        /// 作者
        /// </summary>
        public virtual string Creator { get; set; }
        /// <summary>
        /// 难度
        /// </summary>
        public virtual string Version { get; set; }
        /// <summary>
        /// 难度
        /// </summary>
        public virtual string Difficulty { get { return Version; } set { Version = value; } }
        /// <summary>
        /// Mp3文件
        /// </summary>
        public virtual string AudioFile { get; set; }
        /// <summary>
        /// Md5哈希值
        /// </summary>
        public virtual string Md5Hash { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public virtual string FileName { get; set; }
        /// <summary>
        /// 当前状态
        /// </summary>
        public virtual BeamapStatus Status { get; set; }
        /// <summary>
        /// 点的数量
        /// </summary>
        public virtual ushort NoteCount { get; set; }
        /// <summary>
        /// 滑条数量
        /// </summary>
        public virtual ushort SliderCount { get; set; }
        /// <summary>
        /// 转盘数量
        /// </summary>
        public virtual ushort SpinnerCount { get; set; }
        /// <summary>
        /// 文件写入日期
        /// </summary>
        public virtual DateTime FileDate { get; set; }
        /// <summary>
        /// 圈大小系数(CS)
        /// </summary>
        public virtual byte CircleSize { get; set; }
        /// <summary>
        /// HP系数
        /// </summary>
        public virtual byte HPDrainRate { get; set; }
        /// <summary>
        /// 总体难度系数(OD)
        /// </summary>
        public virtual byte OverallDifficulty { get; set; }
        /// <summary>
        /// 滑条速率系数(SV)
        /// </summary>
        public virtual double SliderMultiplier { get; set; }
        /// <summary>
        /// 第一个note到最后一个note的总长度
        /// 单位：毫秒
        /// </summary>
        public virtual int DrunTime { get; set; }
        /// <summary>
        /// 最后一个的offset
        /// </summary>
        public virtual int LastNode { get; set; }
        /// <summary>
        /// 音频预览点
        /// 单位：毫秒
        /// </summary>
        public virtual int PreviewTime { get; set; }
        /// <summary>
        /// 歌曲Id
        /// </summary>
        public virtual int SongId { get; set; }
        /// <summary>
        /// osu文件的Id
        /// </summary>
        public virtual int BeatmapId { get; set; }
        /// <summary>
        /// 官网论坛帖子Id
        /// </summary>
        public virtual int ThreadId { get; set; }
        /// <summary>
        /// 显示的等分评价
        /// osu!模式
        /// </summary>
        public virtual Ranking Rank_osu { get; set; }
        /// <summary>
        /// 显示的等分评价
        /// 太鼓模式
        /// </summary>
        public virtual Ranking Rank_taiko { get; set; }
        /// <summary>
        /// 显示的等分评价
        /// 接水果模式
        /// </summary>
        public virtual Ranking Rank_ctb { get; set; }
        /// <summary>
        /// 本地offset
        /// </summary>
        public virtual short LocalOffset { get; set; }
        /// <summary>
        /// 堆叠系数
        /// </summary>
        public virtual float StackLeniency { get; set; }
        /// <summary>
        /// 游戏模式
        /// </summary>
        public virtual PlayModes Mode { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        public virtual string Source { get; set; }
        /// <summary>
        /// Tag
        /// </summary>
        public virtual string Tag { get; set; }
        /// <summary>
        /// 在线offset 会覆盖本地offset？
        /// </summary>
        public virtual short OnlineOffset { get; set; }
        /// <summary>
        /// 设置游戏开始时出现的标题样式
        /// </summary>
        public virtual string TitleFont { get; set; }
        /// <summary>
        /// 还没玩过一次
        /// </summary>
        public virtual bool IsNotPlayed { get; set; }
        /// <summary>
        /// 最后一次玩的时间
        /// </summary>
        public virtual DateTime LastPlayTime { get; set; }
        /// <summary>
        /// 是否为osz2格式
        /// </summary>
        public virtual bool IsOsz2 { get; set; }
        /// <summary>
        /// 文件夹路径
        /// </summary>
        public virtual string DirPath { get; set; }
        /// <summary>
        /// 最后一次检查时间
        /// </summary>
        public virtual DateTime LastCheckTime { get; set; }
    }
    public class TimmingPoint
    {
        public TimmingPoint()
        {
        }
        /// <summary>
        /// BPM
        /// 值大于等于0时为真实BPM，否则为绿线的系数
        /// </summary>
        public virtual double Bpm { get; set; }
        /// <summary>
        /// offset
        /// 单位毫秒
        /// </summary>
        public virtual double Offset { get; set; }
        /// <summary>
        /// 是否为红线
        /// </summary>
        public virtual bool IsTimmingPoint { get; set; }
    }
    public class OsuDbReader
    {
        protected class DBReader : System.IO.BinaryReader
        {
            public DBReader(string filePath)
                : base(new System.IO.FileStream(filePath, System.IO.FileMode.Open))
            {

            }
            public DBReader(System.IO.FileStream fileStream)
                : base(fileStream)
            {
            }
            public override string ReadString()
            {
                byte b = ReadByte();
                if (b == 11)
                    return base.ReadString();
                if (b == 0)
                    return string.Empty;
                throw new Exception("意外的标记");
            }
            public DateTime ReadDateTime()
            {
                return new DateTime(base.ReadInt64());
            }
        }
        public string DBPath { get; protected set; }
        //protected System.IO.BinaryReader DbReader { get; set; }
        public OsuDbReader(string path)
        {
            DBPath = path;
        }
        public int DbVersion { get; protected set; }
        public int FolderCount { get; protected set; }
        public int BeatmapCount { get; protected set; }
        public List<BeatmapInfo> ReadDB()
        {
            var list = new List<BeatmapInfo>();
            using (System.IO.FileStream fs = System.IO.File.OpenRead(DBPath))
            {
                using (DBReader DbReader = new DBReader(fs))
                {
                    DbVersion = DbReader.ReadInt32();
                    FolderCount = DbReader.ReadInt32();
                    BeatmapCount = DbReader.ReadInt32();
                    for (int i = 0; i < BeatmapCount; ++i)
                    {
                        list.Add(ReadBeatmapInfo(DbReader));
                    }
                }
            }
            return list;
        }

        protected BeatmapInfo ReadBeatmapInfo(DBReader DbReader)
        {
            BeatmapInfo song = new BeatmapInfo();
            song.Artist = DbReader.ReadString();
            song.Title = DbReader.ReadString();
            song.Creator = DbReader.ReadString();
            song.Version = DbReader.ReadString();
            song.AudioFile = DbReader.ReadString();
            song.Md5Hash = DbReader.ReadString();
            song.FileName = DbReader.ReadString();
            song.Status = (BeatmapInfo.BeamapStatus)DbReader.ReadByte();
            song.NoteCount = DbReader.ReadUInt16();
            song.SliderCount = DbReader.ReadUInt16();
            song.SpinnerCount = DbReader.ReadUInt16();
            song.FileDate = DbReader.ReadDateTime();
            song.CircleSize = DbReader.ReadByte();
            song.HPDrainRate = DbReader.ReadByte();
            song.OverallDifficulty = DbReader.ReadByte();
            song.SliderMultiplier = DbReader.ReadDouble();
            song.DrunTime = DbReader.ReadInt32();
            song.LastNode = DbReader.ReadInt32();
            song.PreviewTime = DbReader.ReadInt32();
            song.Timmings = ReadTimming(DbReader);
            song.SongId = DbReader.ReadInt32();
            song.BeatmapId = DbReader.ReadInt32();
            song.ThreadId = DbReader.ReadInt32();
            song.Rank_osu = (BeatmapInfo.Ranking)DbReader.ReadByte();
            song.Rank_taiko = (BeatmapInfo.Ranking)DbReader.ReadByte();
            song.Rank_ctb = (BeatmapInfo.Ranking)DbReader.ReadByte();
            song.LocalOffset = DbReader.ReadInt16();
            song.StackLeniency = DbReader.ReadSingle();
            song.Mode = (BeatmapInfo.PlayModes)DbReader.ReadByte();
            song.Source = DbReader.ReadString();
            song.Tag = DbReader.ReadString();
            song.OnlineOffset = DbReader.ReadInt16();
            song.TitleFont = DbReader.ReadString();
            song.IsNotPlayed = DbReader.ReadBoolean();
            song.LastPlayTime = DbReader.ReadDateTime();
            song.IsOsz2 = DbReader.ReadBoolean();
            song.DirPath = DbReader.ReadString();
            song.LastCheckTime = DbReader.ReadDateTime();
            return song;
        }

        protected List<TimmingPoint> ReadTimming(DBReader DbReader)
        {
            List<TimmingPoint> list = new List<TimmingPoint>();
            int count = DbReader.ReadInt32();
            for (int i = 0; i < count; ++i)
            {
                TimmingPoint timming = new TimmingPoint();
                timming.Bpm = DbReader.ReadDouble();
                timming.Offset = DbReader.ReadDouble();
                timming.IsTimmingPoint = DbReader.ReadBoolean();
                list.Add(timming);
            }
            return list;
        }
    }
    public class OsuDbWriter
    {
        protected class DBWriter : System.IO.BinaryWriter
        {
            public DBWriter(string filePath)
                : base(new System.IO.FileStream(filePath, System.IO.FileMode.Create))
            {

            }
            public DBWriter(System.IO.FileStream fileStream)
                : base(fileStream)
            {
            }
            public override void Write(string value)
            {
                if (value == null)
                {
                    base.Write((byte)0);
                    return;
                }
                base.Write((byte)11);
                base.Write(value);
            }
            public void Write(DateTime value)
            {
                base.Write(value.Ticks);
            }
        }
        public string DBPath { get; protected set; }
        //protected System.IO.BinaryReader DbReader { get; set; }
        public OsuDbWriter(string path)
        {
            DBPath = path;
        }
        public void WriteDB(int version, int folderCount, IList<BeatmapInfo> beatmaps)
        {
            using (System.IO.FileStream fs = System.IO.File.OpenWrite(DBPath))
            {
                using (DBWriter DbReader = new DBWriter(fs))
                {
                    DbReader.Write(version);
                    DbReader.Write(folderCount);
                    DbReader.Write(beatmaps.Count);
                    foreach (var i in beatmaps)
                    {
                        WriteBeatmapInfo(DbReader, i);
                    }
                }
            }
        }

        protected void WriteBeatmapInfo(DBWriter DbWriter, BeatmapInfo song)
        {
            DbWriter.Write(song.Artist);
            DbWriter.Write(song.Title);
            DbWriter.Write(song.Creator);
            DbWriter.Write(song.Version);
            DbWriter.Write(song.AudioFile);
            DbWriter.Write(song.Md5Hash);
            DbWriter.Write(song.FileName);
            DbWriter.Write((byte)song.Status);
            DbWriter.Write(song.NoteCount);
            DbWriter.Write(song.SliderCount);
            DbWriter.Write(song.SpinnerCount);
            DbWriter.Write(song.FileDate);
            DbWriter.Write(song.CircleSize);
            DbWriter.Write(song.HPDrainRate);
            DbWriter.Write(song.OverallDifficulty);
            DbWriter.Write(song.SliderMultiplier);
            DbWriter.Write(song.DrunTime);
            DbWriter.Write(song.LastNode);
            DbWriter.Write(song.PreviewTime);
            WriteTimming(DbWriter, song.Timmings);
            DbWriter.Write(song.SongId);
            DbWriter.Write(song.BeatmapId);
            DbWriter.Write(song.ThreadId);
            DbWriter.Write((byte)song.Rank_osu);
            DbWriter.Write((byte)song.Rank_taiko);
            DbWriter.Write((byte)song.Rank_ctb);
            DbWriter.Write(song.LocalOffset);
            DbWriter.Write(song.StackLeniency);
            DbWriter.Write((byte)song.Mode);
            DbWriter.Write(song.Source);
            DbWriter.Write(song.Tag);
            DbWriter.Write(song.OnlineOffset);
            DbWriter.Write(song.TitleFont);
            DbWriter.Write(song.IsNotPlayed);
            DbWriter.Write(song.LastPlayTime);
            DbWriter.Write(song.IsOsz2);
            DbWriter.Write(song.DirPath);
            DbWriter.Write(song.LastCheckTime);
        }

        protected void WriteTimming(DBWriter DbWriter, List<TimmingPoint> list)
        {
            DbWriter.Write(list.Count);
            foreach(var timming in list)
            {
                DbWriter.Write(timming.Bpm);
                DbWriter.Write(timming.Offset);
                DbWriter.Write(timming.IsTimmingPoint);
            }
        }
    }
}
