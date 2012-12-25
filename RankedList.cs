using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace BeatmapTool
{
    //[Serializable]
    public class RankedList : SongList
    {
        int[] last;
        public RankedList()
        {
            last = new int[6] { 0, 0, 0, 0, 0, 0 };
            list = new List<SongDetail>();
        }
        public override void Clear()
        {
            last = new int[6] { 0, 0, 0, 0, 0, 0 };
            list.Clear();
            OnListReset();
            OnPropertyChanged();
        }
        public RankedList CloneTo(RankedList objList)
        {/*
            //RankedList obj = new RankedList();
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.Clone));
            bf.Serialize(ms, this.list);
            ms.Seek(0, SeekOrigin.Begin);
            objList.list = (List<SongDetail>)bf.Deserialize(ms);
            ms.Close();*/
            objList.Clear();
            objList.AddRange(this.list.ToArray());
            objList.OnListReset();
            //objList.OnListAdd(this.list.ToArray());
            //((System.Windows.Data.CollectionView)System.Windows.Data.CollectionViewSource.GetDefaultView(objList)).Refresh();
            return objList;
        }
        public int[] Last { get { return last; } set { this.last = value; } }
        public void SaveList(string path)
        {
            byte[] x = null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(ms))
                {
                    StringBuilder app =new StringBuilder();
                    sw.WriteLine("Beatmap List V3.1");
                    sw.WriteLine(this.Last[0].ToString());
                    sw.WriteLine(this.Last[1].ToString());
                    sw.WriteLine(this.Last[2].ToString());
                    foreach (RankedSong song in this)
                    {
                        if (song.Approved == "")
                        {
                            sw.WriteLine(song.ToString(Format.All));
                            sw.WriteLine(song.Video ? "1" : "0");
                            sw.WriteLine(song.DiffCount);
                        }
                        else
                        {
                            app.AppendLine(song.ToString(Format.All));
                            app.AppendLine(song.Video ? "1" : "0");
                            app.AppendLine(song.DiffCount.ToString());
                        }
                    }
                    sw.WriteLine("[Approved]");
                    sw.WriteLine(this.Last[3].ToString());
                    sw.WriteLine(this.Last[4].ToString());
                    sw.WriteLine(this.Last[5].ToString());
                    sw.Write(app);
                    sw.Flush();
                }
                x = Core.Compress(ms.ToArray());
            }
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                fs.Write(x, 0, x.Length);
                fs.Flush();
            }
        }
        public void LoadList(string path)
        {
            if (!File.Exists(path)) return;
            byte[] buffer = new byte[4096];
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    int n = 0;
                    while ((n = fs.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        ms.Write(buffer, 0, n);
                    }
                    buffer = ms.ToArray();
                }
            }
            LoadList(buffer);
        }

        public void LoadList(byte[] buffer)
        {
            int i = 0;
            string[] sd = new string[11];
            try
            {
                using (StreamReader sr = new StreamReader(new MemoryStream(Core.Decompress(buffer))))
                {
                    bool isApp = false;
                    string ver = sr.ReadLine();
                    if (ver != "Beatmap List V3.1")
                    {
                        throw new Exception("载入列表缓存失败.");
                    }
                    this.Last[0] = int.Parse(sr.ReadLine());
                    this.Last[1] = int.Parse(sr.ReadLine());
                    this.Last[2] = int.Parse(sr.ReadLine());
                    sd.Initialize();
                    do
                    {
                        for (i = 0; i < 11; i++)
                        {
                            sd[i] = sr.ReadLine();
                            if (i == 0 && sd[0] == "[Approved]")
                            {
                                this.Last[3] = int.Parse(sr.ReadLine());
                                this.Last[4] = int.Parse(sr.ReadLine());
                                this.Last[5] = int.Parse(sr.ReadLine());
                                sd[i] = sr.ReadLine();
                                isApp = true;
                            }
                        }
                        RankedSong song = new RankedSong(int.Parse(sd[0]), sd[1], sd[2], sd[3], (sd[4] == "" ? DateTime.MinValue : DateTime.Parse(sd[4])), sd[5], sd[6], sd[7], (sd[9] == "1") ? true : false, sd[8], isApp, int.Parse(sd[10]));
                        this.Add(song);
                    } while (!sr.EndOfStream);
                }
            }
            catch
            {
                if (Core.Debug) throw;
                this.Clear();
                System.Windows.Forms.MessageBox.Show("载入列表缓存失败");
            }
            finally
            {
                this.Sort();
            }
        }

    }
}
