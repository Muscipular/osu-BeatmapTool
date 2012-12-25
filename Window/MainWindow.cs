using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Reflection;
using BeatmapTool.DataAccess;
using BeatmapTool.Core;

namespace BeatmapTool
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if (this.IsGlassEnabled())
            {
                this.SetValue(Window.BackgroundProperty, null);
            }
            try
            {
                //LangSelector.ChangeLangENG();
                //SongDetail.Setting["Path"] = @"E:\osu\";
                ////getLocalSongList();
                //BeatmapTool.Core.OsuDB.OsuDbReader v = new Core.OsuDB.OsuDbReader("osu!.db");
                //var a = v.ReadDB();
                //BeatmapTool.Core.OsuDB.OsuDbWriter vv = new Core.OsuDB.OsuDbWriter("osu!-2.db");
                //vv.WriteDB(v.DbVersion, v.FolderCount, a);
                //var b = from s in a
                //        where (byte)s.Status == 0 || (byte)s.Status == 3
                //        //group s by s.long_1 into ss
                //        select s;
                //var c = b.ToArray();
                //var d = c.Max(e => (long)e.Key);
                //var e1 = c.Min(e => (long_)e.Key);
                Settings.Instance["a"] = "a";
                Settings.Instance.Save();
                s = new LocalSong(0, "");
            }
            catch { }
            //this.Opacity = 0.9; 
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        #region 基本
        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);
            osuIcon.StopAnimetion();
        }
        LocalSong s;
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            osuIcon.BeginAnimetion();

        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);
            if (this.WindowState == System.Windows.WindowState.Minimized)
            {
                osuIcon.StopAnimetion();
                return;
            }
            osuIcon.BeginAnimetion();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Style _style = null;
            if (this.IsGlassEnabled())
            {
                _style = (Style)Resources["GadgetStyle"];
            }
            else
                _style = (Style)Resources["GadgetStyle2"];
            this.Style = _style;
            if (this.IsActive)
                osuIcon.BeginAnimetion();
            //this.Opacity = 1;
        }
        #endregion

        private void StartOsu()
        {
        }

        static int tid = 99999999;
        private SongDetail getLocalSongDetail(string path)
        {
            try
            {
                int id = 0;
                if (int.TryParse(System.IO.Path.GetFileName(path).Split(new char[] { ' ', '_' })[0], out id))
                {
                    var s = localList[(long)id];
                    if (s != null && s.Path != path)
                        s = localList[path];
                    if (s != null && s.Date == Directory.GetLastWriteTime(path))
                        return null;
                    if (s != null)
                        localList.Remove(s);
                    return new LocalSong(id, path);
                }
                else
                {
                    foreach (string file in Directory.GetFiles(path))
                    {
                        if (Path.GetExtension(file).ToLower().Contains("osu"))
                        {
                            SongDetail s = localList[path];
                            if (s != null && s.Date == Directory.GetLastWriteTime(path))
                                return null;
                            return new LocalSong(++tid, path);
                        }
                    }
                }
                return null;
            }
            catch (System.Threading.ThreadAbortException) { return null; }
            catch (Exception e)
            {
                //throw;
                MessageBox.Show(e.ToString());
                return null;
            }
        }
        LocalList localList = new LocalList();
        public void getLocalSongList()
        {
            try
            {
                //DataOperator.Load(local: localList);
                string[] s1 = Directory.GetDirectories("E:\\osu\\Songs");//OsuPath + "\\Songs");
                foreach (string s in s1)
                {
                    if (Path.GetFileName(s).ToLower() == "failed") continue;
                    SongDetail song = getLocalSongDetail(s);
                    if (song != null && song.Id > 0)
                    {
                        localList.Add(song);
                        continue;
                    }
                    string[] s2 = Directory.GetDirectories(s);
                    foreach (string ss in s2)
                    {
                        song = getLocalSongDetail(ss);
                        if (song != null && song.Id > 0)
                        {
                            localList.Add(song);
                        }
                    }
                }
                localList.Sort();
                DateTime t1 = DateTime.Now, t2;
                //DataOperator.Save(local: localList);
                t2 = DateTime.Now;
                Debug.WriteLine((t2 - t1).TotalMilliseconds);
                localList.Clear();
                t1 = DateTime.Now;
                t2 = DateTime.Now;
                Debug.WriteLine((t2 - t1).TotalMilliseconds);
            }
            catch (System.Threading.ThreadAbortException) { }
        }

        void xxx(LocalSong s)
        {
            FileStream fs = File.OpenWrite("a");
            //DataOperator.Write(fs, s.Save());
            fs.Flush();
            fs.Close();
            fs.Dispose();
        }

        void eee()
        {
            byte[] b = File.ReadAllBytes("a");
            //DataBlock bb = new DataBlock(b, 4, b.Length - 4);
            //LocalSong l = new LocalSong(bb);
            //l.ToString();
        }
    }
}
