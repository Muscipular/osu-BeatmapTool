using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using BeatmapTool.DataAccess;

namespace BeatmapTool.Core
{
    public class LocalSong : SongDetail
    {
        private string[] _Backgrounds;
        protected string _Path;

        //public override string Tag
        //{
        //    get
        //    {
        //        if (_Tag == null)
        //            _Tag = getTags();
        //        return _Tag;
        //    }
        //}
        //public override string Audio
        //{
        //    get
        //    {
        //        Random rnd = new Random();
        //        return string.Concat(Setting["Path"], _Path, "\\", _Diffs[rnd.Next(0, _Diffs.Count)].Audio);
        //    }
        //}
        //public override string Path
        //{
        //    get
        //    {
        //        return string.Concat(Setting["Path"], _Path);
        //    }
        //}
        //public override string Background
        //{
        //    get
        //    {
        //        if (Backgrounds.Length == 0)
        //            return string.Empty;
        //        Random rnd = new Random();
        //        return string.Concat(Setting["Path"], _Path, "\\", Backgrounds[rnd.Next(0, Backgrounds.Length)]);
        //    }
        //}

        public string[] Backgrounds
        {
            get
            {
                if (_Backgrounds != null)
                    return _Backgrounds;
                List<string> bgs = new List<string>();
                foreach (Diff diff in Diffs)
                {
                    if (!string.IsNullOrWhiteSpace(diff.Background) && !bgs.Contains(diff.Background))
                        bgs.Add(diff.Background);
                }
                _Backgrounds = bgs.ToArray();
                return _Backgrounds;
                //return bgs.ToArray();
            }
        }

        public LocalSong(BeatmapSet song) : base(song) { }
        public LocalSong(int id, string path)
        {
            Init(id, path);
            //ReferenceTransaction(trans);
            if (!FillData())
                throw new Exception("读取Beatmap错误");
        }

        //public LocalSong(DataBlock data)
        //{
        //    Load(data);
        //}

        private void Init(int id, string path)
        {
            //_Tag = string.Empty;
            //_Diffs = new List<Diff>();
            this.Id = id;
            this._Path = @"E:\osu\Songs\Z-Songs\19753 Horie Yui - Sakura";
            foreach (string file in Directory.GetFiles(_Path))
            {
                if (System.IO.Path.GetExtension(file).ToLower() == ".osu")
                {
                    Diff beatmap = null;
                    beatmap = new Diff(file, this);
                    Diffs.Add(beatmap);
                }
            }


            //_Tag = string.Empty;
            //_Tag = getTags();
            //foreach (Diff diff in _Diffs)
            //{
            //    //diff.SetTags(_Tag);
            //}
            //_Date = Directory.GetLastWriteTime(path);
            //if (id > 99999999)
            //{
            //    //var xxx = from s in list where s.Artist == this.Artist && s.Creator == this.Creator && s.Title == this.Title select s;
            //    //if (xxx.Count() > 0)
            //    //{
            //    //    this._Id = xxx.ElementAt(0).Id;
            //    //}
            //}
        }

        private string getDiffsName()
        {
            //string str="";
            StringBuilder sb = new StringBuilder();
            //foreach (Diff b in _Diffs)
            //{
            //    sb.AppendLine(b.Difficulty);
            //}
            return sb.ToString();
        }

        public override string ToString(Format format)
        {
            switch (format)
            {
                //case Format.Search:
                //case Format.All:
                //    return string.Format("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}\n{8}", _Id, _Title, _Artist, _Creator, _Source, Tag, _Path, getDiffsName(), _Date.ToLongDateString());
                //case Format.ReplaySearch: return string.Format("{0}{1}{2}{3}", _Id, _Artist, _Title, _Creator);
                //case Format.Normal: return string.Format("{0} - {1}", _Artist, _Title);
                //case Format.Replay: 
                //case Format.None:
                default: return this.ToString();
            }
        }

        public void Reload()
        {
        }

        public bool ReloadFromFile()
        {
            //Init(this._Id, this._Path);
            return FillData();
        }

        private bool FillData()
        {
            return true;
        }

        private string getTags()
        {
            List<string> taglist = new List<string>();
            //foreach (Diff beatmap in _Diffs)
            //{
            //    if (!taglist.Contains(beatmap.Tag))
            //        taglist.Add(beatmap.Tag);
            //}
            List<string> result = new List<string>();
            foreach (string tags in taglist)
            {
                foreach (string tag in tags.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string tStr = tag.Trim();
                    if (!string.IsNullOrWhiteSpace(tStr) && !result.Contains(tStr))
                    {
                        result.Add(" ");
                        result.Add(tStr);
                    }
                }
            }
            //RankedSong song = new RankedSong();
            //if (song != null && song.DiffCount > DiffCount)
            //{
            //    result.Add(" ");
            //    result.Add("缺失难度LostDiff");
            //}
            //if (result.Count > 1)
            //    result.RemoveAt(0);
            return string.Concat(result);
        }

        //public override bool Load(DataBlock data)
        //{
        //    _Id = data.ReadInt();
        //    _Date = DateTime.FromBinary(data.ReadLong());
        //    _Genre = (Genre)data.ReadInt();
        //    _IsAPP = data.ReadInt() == 1;
        //    _Lang = (Language)data.ReadInt();
        //    _Pack = data.ReadString();
        //    _Path = data.ReadString();
        //    _Source = data.ReadString();
        //    _Tag = null;
        //    data.ReadString();
        //    int diffcount = data.ReadInt();
        //    _Diffs = new List<Diff>(diffcount);
        //    for (int i = 0; i < diffcount; i++)
        //    {
        //        _Diffs.Add(new Diff(data.ReadBlock()));
        //    }
        //    _Title = _Diffs[0].Title;
        //    _Artist = _Diffs[0].Artist;
        //    _Creator = _Diffs[0].Creator;
        //    _Source = _Diffs[0].Source;
        //    _Tag = _Diffs[0].Tags;
        //    return true;
        //}

        //public override byte[] Save()
        //{
        //    MemoryStream ms = new MemoryStream();
        //    byte[] buffer = null;
        //    buffer = DataOperator.i2b(0);
        //    DataOperator.Write(ms, buffer);
        //    buffer = DataOperator.i2b((int)this._Id);
        //    DataOperator.Write(ms, buffer);
        //    buffer = DataOperator.l2b(this._Date.ToBinary());
        //    DataOperator.Write(ms, buffer);
        //    buffer = DataOperator.i2b((int)this._Genre);
        //    DataOperator.Write(ms, buffer);
        //    buffer = DataOperator.i2b(this._IsAPP ? 1 : 0);
        //    DataOperator.Write(ms, buffer);
        //    buffer = DataOperator.i2b((int)this._Lang);
        //    DataOperator.Write(ms, buffer);
        //    buffer = DataOperator.s2b(this._Pack);
        //    DataOperator.Write(ms, buffer);
        //    buffer = DataOperator.s2b(this._Path);
        //    DataOperator.Write(ms, buffer);
        //    buffer = DataOperator.s2b(this._Source);
        //    DataOperator.Write(ms, buffer);
        //    _Tag = null;
        //    buffer = DataOperator.s2b(this._Tag);
        //    DataOperator.Write(ms, buffer);
        //    buffer = DataOperator.i2b(this.DiffCount);
        //    DataOperator.Write(ms, buffer);
        //    foreach (Diff d in _Diffs)
        //    {
        //        buffer = d.Save();
        //        DataOperator.Write(ms, buffer);
        //    }
        //    ms.Seek(0, SeekOrigin.Begin);
        //    buffer = DataOperator.i2b((int)ms.Length - 4);
        //    DataOperator.Write(ms, buffer);
        //    buffer = ms.ToArray();
        //    ms.Close();
        //    ms.Dispose();
        //    return buffer;
        //}

        public override string Path
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override SongType Type
        {
            get { throw new NotImplementedException(); }
        }

        public override bool HaveVideo
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override string Background
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
