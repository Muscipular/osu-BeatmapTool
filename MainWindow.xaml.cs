using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
//using Microsoft.DirectX.AudioVideoPlayback;

namespace BeatmapTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        const string songUrid = "http://osu.ppy.sh/d/";
        const string songUri = "http://osu.ppy.sh/s/";
        const string songImg = "http://d.osu.ppy.sh/mt/";
        const string ListFile = "\\BeatmapListing.list";
        const string OsuFile = "\\osu!.exe";
        const string Mp3Url = "http://d.osu.ppy.sh/mp3/preview/";

        private static readonly string
            STR_BTNRELOAD,
            STR_MSGPICKOSUFOLDER,
            STR_MSGOSUNOEXIST,
            STR_UILOADLOCAL,
            STR_UILOADLOCALFIN,
            STR_MSGPICKOSUFOLDER2,
            STR_UPDATELIST,
            STR_DOWNLOADLIST,
            STR_UPDATELISTFIN,
            STR_MSGALERTDO,
            STR_MSGALERTDO1,
            STR_MSGALERTDO2,
            STR_MSGALERTDO3,
            STR_MSGALERTDO4,
            STR_MSGALERTDO5,
            STR_MSGALERTDO6,
            STR_MSGALERTDO7,
            STR_DELCFBEATMAP,
            STR_PROCESSING,//Processing
            STR_DELREP,
            STR_ALLDONE,
            STR_STOP,
            STR_RANKEDDATE,
            STR_GENRE,
            STR_SOURCE,
            STR_BEATMAPINFO,
            STR_DOWNLOADBEATMAP,
            STR_FILEDATE,
            STR_SONGPATH,
            STR_OPENFOLDER,
            STR_DELBEATMAP,
            STR_LOSTDIFF,
            STR_SKIN,
            STR_HITSOUND,
            STR_BACKUP,
            STR_MANAGEREP,
            STR_SAVEAUDIO,
            STR_REPLACEBG,
            STR_DELCHOOSE,
            STR_DOWNLOADWITHNV,
            STR_STORYBOARD,
            STR_BEATMAP,
            STR_MSGCHOOSEBGTITLE,
            STR_MSGCHOOSEBG,
            STR_MSGCHOOSEBG2,
            STR_MSGSAVELISTTITLE,
            STR_BTNSAVECFLIST,
            STR_MSGSAVECFLIST,
            STR_BTNSAVEALLLIST,
            STR_MSGSAVEALLLIST,
            STR_BTNSAVELOCALLIST,
            STR_MSGSAVELOCALLIST,
            STR_BTNSAVEOBJLIST,
            STR_MSGSAVEOBJLIST,
            STR_MSGSAVELISTCONDITION,
            STR_BTNSAVEAS,
            STR_BTNDELETEREPLAY
            ;
        static MainWindow()
        {
            STR_BTNDELETEREPLAY = "删除Replay";
            STR_BTNSAVEAS = "另存为...";
            STR_MSGSAVELISTCONDITION = "保存为简易样式？";
            STR_BTNSAVELOCALLIST = "保存本地列表";
            STR_MSGSAVELOCALLIST = "本地的Beatmap列表";
            STR_BTNSAVEOBJLIST = "保存缺少列表";
            STR_MSGSAVEOBJLIST = "缺少的Beatmap列表";
            STR_BTNSAVEALLLIST = "保存所有列表";
            STR_MSGSAVEALLLIST = "所有的Beatmap列表";
            STR_MSGSAVECFLIST = "重复的Beatmap列表";
            STR_BTNSAVECFLIST = "保存重复列表";
            STR_MSGSAVELISTTITLE = "保存列表";
            STR_MSGCHOOSEBG2 = "【是】把背景图片替换成黑色\n【否】把背景图片替换成灰色";
            STR_MSGCHOOSEBG = "替换指定的图片?";
            STR_MSGCHOOSEBGTITLE = "选择背景图像";
            STR_BEATMAP = "Beatmap";
            STR_DELCHOOSE = "删除选择";
            STR_REPLACEBG = "替换背景图";
            STR_SAVEAUDIO = "保存Mp3";
            STR_MANAGEREP = "查看Replay";
            STR_BACKUP = "备份模式";
            STR_HITSOUND = "音效";
            STR_SKIN = "皮肤";
            STR_LOSTDIFF = "缺少了{0}个难度!(点击查看)";
            STR_DELBEATMAP = "删除Beatmap";
            STR_OPENFOLDER = "打开目录";
            STR_SONGPATH = "所在目录:  ";
            STR_FILEDATE = "文件日期:  ";
            STR_DOWNLOADBEATMAP = "下载Beatmap";
            STR_BEATMAPINFO = "查看Beatmap";
            STR_SOURCE = "来源:  ";
            STR_GENRE = "类别:  ";
            STR_RANKEDDATE = "Ranked日期:  ";
            STR_STOP = "中止操作.";
            STR_ALLDONE = "所有操作完成.";
            STR_DELREP = "删除Replay中...";
            STR_PROCESSING = "正在处理:{0}...";
            STR_BTNRELOAD = "更新本地列表";
            STR_MSGPICKOSUFOLDER = "请先选择osu!.exe所在目录";
            STR_MSGPICKOSUFOLDER2 = "选择OSU!所在的目录";
            STR_MSGOSUNOEXIST = "osu!.exe不存在选择的目录下";
            STR_UILOADLOCAL = "读取本地Beatmap列表中...";
            STR_UILOADLOCALFIN = "读取本地Beatmap列表完成.";
            STR_UPDATELIST = "更新Beatmap列表中[{0}/{1}]...";
            STR_DOWNLOADLIST = "下载Beatmap列表中[{0}%]...";
            STR_UPDATELISTFIN = "更新Beatmap列表完成[{0}/{1}].";
            STR_MSGALERTDO = "是否以{0}备份模式执行以下操作？\n{1}{2}{3}{4}{5}{6}";
            STR_MSGALERTDO1 = "非";
            STR_MSGALERTDO2 = "　　删除重复Beatmap\n";
            STR_MSGALERTDO3 = "　　删除所有Replay\n";
            STR_MSGALERTDO4 = "　　删除Storyboard\n";
            STR_MSGALERTDO5 = "　　删除皮肤\n";
            STR_MSGALERTDO6 = "　　替换背景图\n";
            STR_MSGALERTDO7 = "　　删除音效\n";
            STR_DELCFBEATMAP = "删除重复的Beatmap中[{0}/{1}]...";
            STR_DOWNLOADWITHNV = "下载不带视频的版本";
            //STR_DOWNLOADWITHNV = "DownLoadWithout Video?";
            STR_STORYBOARD = "Storyboard";
        }

        UIBinding uiBinding;
        Dictionary<int, List<string>> searchKey;
        DownloadWin DlWindow;
        /*
        System.ComponentModel.SortDescription SDArtist;
        System.ComponentModel.SortDescription SDTitle;
        System.ComponentModel.SortDescription SDCreator;
        System.ComponentModel.SortDescription SDDifficulty;
        System.ComponentModel.SortDescription SDScore;
        */
        SongList localList = Core.LocalList;
        RankedList rankedList = Core.RankedList;
        RankedList objList = Core.ObjList;
        SongList cfList = Core.CfList;
        String osuPath { get { return Core.OsuPath; } set { Core.OsuPath = value; } }

        Thread threadlocal = null;
        Thread threaddowork = null;
        Thread threadget = null;
        Thread threadgetmp3 = null;
        Thread disposemp3 = null;

        Mp3Player player = null;
        Expander expCur = null;
        Expander expCur2 = null;
        Expander repExp = null;

        System.Drawing.Bitmap rpimg = null;
        System.Collections.ObjectModel.ObservableCollection<Replay> replays = null;

        //Microsoft.DirectX.AudioVideoPlayback.Audio player;

        bool playerFlag = false;

        public MainWindow()
        {
            InitializeComponent();
            if (Environment.CommandLine.ToLower().Contains("debug_mode"))
                Core.Debug = true;
            uiBinding = new UIBinding();
            DlWindow = new DownloadWin(uiBinding);
            searchKey = new Dictionary<int, List<string>>();
            searchKey[0] = new List<string>();
            searchKey[1] = new List<string>();
            searchKey[2] = new List<string>();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            osuPath = Environment.CurrentDirectory;
            tbxOsuPath.Text = osuPath;
            threadget = new Thread(LoadCacheList);
            threadget.Start();
            CheckOSUPath(true);
            height_max = Height + 80;
            height_min = Height;
        }

        private void SetBinding()
        {
            Binding binding = new Binding();
            binding.Source = uiBinding;
            binding.Path = new PropertyPath("IsNotWorking");
            binding.Mode = BindingMode.OneWay;
            BtnSetPath.SetBinding(Button.IsEnabledProperty, binding);
            tabReplay.SetBinding(TabItem.IsEnabledProperty, binding);


            procName.DataContext = uiBinding;
            Lb1.DataContext = localList;
            Lb2.DataContext = objList;
            Lb3.DataContext = rankedList;
            Lb4.DataContext = cfList;
            LGrid.DataContext = uiBinding;
            BtnStop.DataContext = uiBinding;
            BtnDoWork.DataContext = uiBinding;
            procBar.DataContext = uiBinding;
            BtnGetList.DataContext = uiBinding;
            cbxBackup.DataContext = uiBinding;
            cbxDelDu.DataContext = uiBinding;
            cbxDelHitSound.DataContext = uiBinding;
            cbxDelRep.DataContext = uiBinding;
            cbxDelSB.DataContext = uiBinding;
            cbxDelSkin.DataContext = uiBinding;
            cbxReBuild.DataContext = uiBinding;
            cbxReplaceBG.DataContext = uiBinding;
            cbxCache.DataContext = uiBinding;

            /*
            SDArtist = new System.ComponentModel.SortDescription("Artist", System.ComponentModel.ListSortDirection.Ascending);
            SDTitle = new System.ComponentModel.SortDescription("Title", System.ComponentModel.ListSortDirection.Ascending);
            SDCreator = new System.ComponentModel.SortDescription("Creator", System.ComponentModel.ListSortDirection.Ascending);
            SDDifficulty = new System.ComponentModel.SortDescription("Difficulty", System.ComponentModel.ListSortDirection.Ascending);
            SDScore = new System.ComponentModel.SortDescription("Score", System.ComponentModel.ListSortDirection.Ascending);

            /*
            LocalListBox.Items.SortDescriptions.Add(SDArtist);
            LocalListBox.Items.SortDescriptions.Add(SDTitle);
            LocalListBox.Items.SortDescriptions.Add(SDCreator);
            LocalListBox.Items.Filter += new Predicate<object>(ListFilter);

            ObjListBox.Items.SortDescriptions.Add(SDArtist);
            ObjListBox.Items.SortDescriptions.Add(SDTitle);
            ObjListBox.Items.SortDescriptions.Add(SDCreator);
            ObjListBox.Items.Filter += new Predicate<object>(ListFilter);

            AllListBox.Items.SortDescriptions.Add(SDArtist);
            AllListBox.Items.SortDescriptions.Add(SDTitle);
            AllListBox.Items.SortDescriptions.Add(SDCreator);
            AllListBox.Items.Filter += new Predicate<object>(ListFilter);

            CfListBox.Items.SortDescriptions.Add(SDArtist);
            CfListBox.Items.SortDescriptions.Add(SDTitle);
            CfListBox.Items.SortDescriptions.Add(SDCreator);
            CfListBox.Items.Filter += new Predicate<object>(ListFilter);

            RepListBox.Items.SortDescriptions.Add(SDArtist);
            RepListBox.Items.SortDescriptions.Add(SDTitle);
            RepListBox.Items.SortDescriptions.Add(SDCreator);
            RepListBox.Items.SortDescriptions.Add(SDDifficulty);
            RepListBox.Items.SortDescriptions.Add(SDScore);
            RepListBox.Items.Filter += new Predicate<object>(ListFilter2);
            
            LocalListBox.Items.Filter += new Predicate<object>(ListFilter);
            ObjListBox.Items.Filter += new Predicate<object>(ListFilter);
            AllListBox.Items.Filter += new Predicate<object>(ListFilter);
            CfListBox.Items.Filter += new Predicate<object>(ListFilter);
            RepListBox.Items.Filter += new Predicate<object>(ListFilter2);
            */
        }


        private void CheckOSUPath(bool flag)
        {
            if (File.Exists(Core.OsuPath + OsuFile))
            {
                tabList.IsEnabled = true;
                GboxDoWork.IsEnabled = true;
                GboxGetList.IsEnabled = true;
                GboxInfo.IsEnabled = true;
                GboxProc.IsEnabled = true;
                BtnSetPath.IsEnabled = false;
                OsuBtn.IsEnabled = true;
                tabAbout.IsEnabled = true;

                BtnSetPath.Content = STR_BTNRELOAD;
                BtnSetPath.Click -= new RoutedEventHandler(BtnSetPath_Click);
                BtnSetPath.Click += new RoutedEventHandler(BtnGetLocalList);
                SetBinding();
                if (threadlocal == null)
                {
                    threadlocal = new Thread(GetLocalList);
                    threadlocal.Start();
                }
            }
            else
            {
                MessageBox.Show(this, flag ? STR_MSGPICKOSUFOLDER : STR_MSGOSUNOEXIST, "!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GetLocalList()
        {
            uiBinding.IsWorking = true;
            uiBinding.ProcCur = 0;
            uiBinding.ProcMax = 100;
            uiBinding.ProcName = STR_UILOADLOCAL;
            Core.getLocalSongList();
            Core.GetObjList();
            uiBinding.ProcCur = uiBinding.ProcMax;
            uiBinding.ProcName = STR_UILOADLOCALFIN;
            uiBinding.IsWorking = false;
            threadlocal = null;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender.Equals(TopGrid) && e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            StopProcess();
            osuPath = null;
            DlWindow.Exit();
            App.Current.Shutdown();
        }

        private void BtnMin_Click(object sender, RoutedEventArgs e)
        {
            App.Current.MainWindow.WindowState = System.Windows.WindowState.Minimized;
        }

        private void tabControl1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
        private void BtnGetLocalList(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (threadlocal == null)
            {
                threadlocal = new Thread(new ThreadStart(GetLocalList));
                threadlocal.Start();
            }
        }
        private void BtnSetPath_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog f = new System.Windows.Forms.FolderBrowserDialog();
            f.Description = STR_MSGPICKOSUFOLDER2;
            f.ShowNewFolderButton = false;
            f.RootFolder = Environment.SpecialFolder.MyComputer;
            if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                tbxOsuPath.Text = f.SelectedPath;
                osuPath = f.SelectedPath;
                CheckOSUPath(false);
            }
        }
        private void BtnGetList_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (threadget == null)
            {
                threadget = new Thread(new ThreadStart(GetBeatmapList));
                threadget.Start();
            }
        }

        private void GetBeatmapList()
        {
            uiBinding.IsWorking = true;
            uiBinding.ProcCur = 0;
            Core.ReleaseAll();
            uiBinding.ProcMax = 100;
            if (uiBinding.IsReBuild)
                rankedList.Clear();
            if (uiBinding.IsCache)
            {
                uiBinding.IsCache = false;
                uiBinding.SetProcNameByFormat(STR_DOWNLOADLIST, uiBinding.ProcCur);
                Core.GetCacheList();
            }
            else
            {
                uiBinding.IsReBuild = false;
                uiBinding.SetProcNameByFormat(STR_UPDATELIST, 0, 0);
                Core.BeatmapListing();
                Core.GetObjList();
                uiBinding.ProcCur = uiBinding.ProcMax;
                uiBinding.SetProcNameByFormat(STR_UPDATELISTFIN);
                uiBinding.IsWorking = false;
            }
            threadget = null;
        }

        private void LoadCacheList()
        {
            if (File.Exists(Environment.CurrentDirectory + ListFile))
            {
                if (Core.RankedList.Lock())
                {
                    Core.RankedList.LoadList(Environment.CurrentDirectory + ListFile);
                    Core.RankedList.UnLock();
                }
            }
            threadget = null;
        }

        private void BtnDoWork_Click(object sender, RoutedEventArgs e)
        {
            if (uiBinding.IsDelDu || uiBinding.IsDelHitSound || uiBinding.IsDelRep || uiBinding.IsDelSb || uiBinding.IsDelSkin || uiBinding.IsReplaceBg)
            {
                if (MessageBox.Show(string.Format(STR_MSGALERTDO, uiBinding.IsBackup ? string.Empty : STR_MSGALERTDO1,
                    uiBinding.IsDelDu ? STR_MSGALERTDO2 : string.Empty,
                    uiBinding.IsDelRep ? STR_MSGALERTDO3 : string.Empty,
                    uiBinding.IsDelSb ? STR_MSGALERTDO4 : string.Empty,
                    uiBinding.IsDelSkin ? STR_MSGALERTDO5 : string.Empty,
                    uiBinding.IsReplaceBg ? STR_MSGALERTDO6 : string.Empty,
                    uiBinding.IsDelHitSound ? STR_MSGALERTDO7 : string.Empty
                    ), string.Empty, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    if (threaddowork == null)
                    {
                        threaddowork = new Thread(new ThreadStart(DoWork));
                        threaddowork.Start();
                    }
                }
            }
        }
        private void DoWork()
        {
            while (cfList.IsLock || localList.IsLock) { System.Threading.Thread.Sleep(2000); }
            cfList.Lock();
            localList.Lock();
            Core.ReleaseAll();
            uiBinding.ProcCur = 0;
            uiBinding.ProcMax = localList.Count + 10;
            uiBinding.IsWorking = true;
            uiBinding.ProcMax += (uiBinding.IsDelDu ? cfList.Count : 0);
            try
            {
                if (uiBinding.IsDelDu)
                {
                    int count = cfList.Count;
                    uiBinding.SetProcNameByFormat(STR_DELCFBEATMAP, uiBinding.ProcCur, count);
                    foreach (LocalSong song in cfList)
                    {
                        uiBinding.ProcCur++;
                        uiBinding.SetProcNameByFormat(STR_DELCFBEATMAP, uiBinding.ProcCur, count);
                        Core.RemoveSong(song);
                    }
                    cfList.Clear();
                }
                foreach (LocalSong song in localList)
                {
                    uiBinding.ProcCur++;
                    uiBinding.SetProcNameByFormat(STR_PROCESSING, song.ToString().Substring(0, ((song.ToString().Length > 20) ? 20 : song.ToString().Length)));
                    if (uiBinding.IsDelHitSound) Core.RemoveSound(song, uiBinding.IsBackup);
                    if (uiBinding.IsDelSb) Core.RemoveSb(song, false, uiBinding.IsBackup);
                    if (uiBinding.IsDelSkin) Core.RemoveSkin(song, true, uiBinding.IsBackup);
                    if (uiBinding.IsReplaceBg && rpimg != null) Core.ReplaceSongBg(song, rpimg, uiBinding.IsBackup);
                }
                if (uiBinding.IsDelRep)
                {
                    uiBinding.ProcName = STR_DELREP;
                    if (Directory.Exists(osuPath + "\\Data\\r"))
                    {
                        string[] files = Directory.GetFiles(osuPath + "\\Data\\r");
                        foreach (string file in files)
                        {
                            uiBinding.ProcCur++;
                            File.Delete(file);
                        }
                    }
                }
                uiBinding.ProcName = STR_ALLDONE;
                uiBinding.ProcCur = uiBinding.ProcMax;
            }
            catch (ThreadAbortException)
            {
                uiBinding.ProcName = STR_STOP;
                uiBinding.ProcCur = 0;
            }
            catch (Exception e)
            {
                if (Core.Debug) throw;
                if (e.Message == "Stop")
                {
                    uiBinding.ProcName = STR_STOP;
                    uiBinding.ProcCur = 0;
                }
                else
                    MessageBox.Show(e.Message);
            }
            finally
            {
                uiBinding.IsWorking = false;
                localList.UnLock();
                cfList.UnLock();
                threaddowork = null;
            }
        }
        private void BtnAbout_Click(object sender, RoutedEventArgs e)
        {
            About f = new About();
            f.Owner = this;
            f.ShowDialog();
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            StopProcess();
        }

        private void StopProcess()
        {
            if (uiBinding.IsWorking)
            {
                if (threaddowork != null)
                {
                    threaddowork.Abort();
                    threaddowork = null;
                    cfList.UnLock();
                    localList.UnLock();
                }
                if (threadget != null)
                {
                    threadget.Abort();
                    threadget = null;
                    objList.UnLock();
                    objList.Clear();
                    rankedList.UnLock();
                    rankedList.Clear();
                }
                if (threadlocal != null)
                {
                    threadlocal.Abort();
                    threadlocal = null;
                    cfList.UnLock();
                    cfList.Clear();
                    localList.UnLock();
                    objList.UnLock();
                    objList.Clear();
                    localList.Clear();
                }
                uiBinding.ProcCur = 0;
                uiBinding.ProcName = STR_STOP;
                uiBinding.IsWorking = false;
            }
        }
        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            try
            {
                e.Handled = true;
                Expander exp = (Expander)sender;
                if (expCur == null || expCur != exp)
                {
                    if (expCur != null)
                    {
                        //playerDispose();
                        /*disposemp3 = new Thread(new ThreadStart(playerDispose));
                        disposemp3.Start();*/
                        expCur.IsExpanded = false;
                        ((DockPanel)expCur.Content).Children.Clear();
                        cbFlag = 0;
                    }
                    switch (expCur2.Name)
                    {
                        case "expAll":
                            {
                                RankedSong song = CreateListingListExpanderContent(exp);
                                AllListBox.SelectedItem = song;
                                break;
                            }
                        case "expDu":
                            {
                                LocalSong song = CreateCfListExpanderContent(exp);
                                CfListBox.SelectedItem = song;
                                break;
                            }
                        case "expLocal":
                            {
                                LocalSong song = CreateLocalListExpanderContent(exp);
                                LocalListBox.SelectedItem = song;
                                break;
                            }
                        case "expObj":
                            {
                                RankedSong song = CreateListingListExpanderContent(exp);
                                ObjListBox.SelectedItem = song;
                                break;
                            }
                    }
                    expCur = exp;
                }
            }
            catch (Exception ex)
            {
                if (Core.Debug) throw;
                MessageBox.Show(ex.ToString());
            }
        }

        private void playerDispose()
        {
            try
            {
                if (threadgetmp3 != null)
                    threadgetmp3.Abort();
                if (player != null)
                {
                    //player.Close();
                    player.Dispose();
                }
                player = null;
                playerFlag = false;
                disposemp3 = null;
                /*
                if (threadgetmp3 != null)
                    threadgetmp3.Abort();
                if (psong != null)
                {
                    MediaPlayer.Stop();
                    psong.Dispose();
                    psong = null;
                }
                playerFlag = false;
                disposemp3 = null;*/
            }
            catch (ThreadAbortException) { }
            catch (Exception e)
            {
                if (Core.Debug) throw;
                MessageBox.Show(e.ToString());
            }

        }
        int cbFlag = 0;
        private RankedSong CreateListingListExpanderContent(Expander exp)
        {
            try
            {
                RankedSong song = exp.DataContext as RankedSong;
                DockPanel wp = (DockPanel)exp.Content;
                Image img = new Image();
                img.MouseLeftButtonDown += new MouseButtonEventHandler(ExpImage_MouseLeftButtonDown);
                DateTime date = song.Date;

                new Thread(new ThreadStart(() =>
                {
                    Uri uri = new Uri(songImg + song.Id);
                    System.Net.WebResponse response = null;
                    Stream s = null;
                    try
                    {
                        System.Net.WebRequest request = System.Net.WebRequest.Create(uri);
                        request.Method = "GET";
                        response = request.GetResponse();
                        s = response.GetResponseStream();
                        System.Drawing.Image imx = System.Drawing.Image.FromStream(s);
                        System.Drawing.Bitmap xx = new System.Drawing.Bitmap(imx);
                        IntPtr hb = xx.GetHbitmap();
                        Dispatcher.BeginInvoke((Action)(() =>
                        {
                            img.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hb, IntPtr.Zero, Int32Rect.Empty,
                            System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                        }));
                    }
                    catch (System.Net.WebException)
                    {
                        Dispatcher.BeginInvoke((Action)(() =>
                        {
                            img.Source = new BitmapImage(new Uri("img/3l.jpg", UriKind.Relative));
                        }));
                    }
                    catch (Exception)
                    {
                        if (Core.Debug) throw;
                        //MessageBox.Show("背景图片载入失败");
                        Dispatcher.BeginInvoke((Action)(() =>
                        {
                            img.Source = new BitmapImage(new Uri("img/3l.jpg", UriKind.Relative));
                        }));
                    }
                    finally
                    {
                        if (s != null)
                            s.Close();
                        if (response != null)
                            response.Close();
                    }
                })).Start();
                wp.Children.Add(img);
                StackPanel sp = new StackPanel();
                Label lb1 = new Label();
                lb1.Content = STR_RANKEDDATE + date.Year + "-" + date.Month + "-" + date.Day;
                sp.Children.Add(lb1);
                lb1 = new Label();
                lb1.Content = STR_GENRE + song.Genre + " (" + song.Language + ")";
                sp.Children.Add(lb1);
                lb1 = new Label();
                lb1.Content = STR_SOURCE + song.Source + " (Pack:  " + song.Pack + ")";
                sp.Children.Add(lb1);
                CheckBox cb = new CheckBox();
                cb.IsEnabled = song.Video;
                cb.Content = STR_DOWNLOADWITHNV;
                cb.Checked += new RoutedEventHandler(expCb_Checked);
                cb.Unchecked += new RoutedEventHandler(expCb_Checked);
                sp.Children.Add(cb);
                WrapPanel wp1 = new WrapPanel();
                wp1.Margin = new Thickness(0, 1, 0, 0);
                Button btn = new Button();
                btn.Content = STR_BEATMAPINFO;
                btn.Click += new RoutedEventHandler(expBtn_Click);
                wp1.Children.Add(btn);
                btn = new Button();
                btn.Click += new RoutedEventHandler(expBtn_Click);
                btn.Content = STR_DOWNLOADBEATMAP;
                wp1.Children.Add(btn);
                sp.Children.Add(wp1);

                wp.Children.Add(sp);
                return song;
            }
            catch (Exception e)
            {
                if (Core.Debug) throw;
                MessageBox.Show(e.ToString());
                return null;
            }
        }

        private LocalSong CreateCfListExpanderContent(Expander exp)
        {
            LocalSong song = (LocalSong)exp.DataContext;
            DockPanel wp = (DockPanel)exp.Content;
            Image img = new Image();
            img.MouseLeftButtonDown += new MouseButtonEventHandler(ExpImage_MouseLeftButtonDown);
            DateTime date = song.Date;
            string bg = song.BG;
            if (bg != "" && File.Exists(bg))
                img.Source = GetBgImg(song, bg);
            else
                img.Source = new BitmapImage(new Uri("img/3l.jpg", UriKind.RelativeOrAbsolute));
            wp.Children.Add(img);
            StackPanel sp = new StackPanel();
            Label lb1 = new Label();
            lb1.Content = STR_FILEDATE + date.Year + "-" + date.Month + "-" + date.Day + "  " + date.ToShortTimeString();
            sp.Children.Add(lb1);
            lb1 = new Label();
            lb1.Content = STR_SONGPATH + song.Path;
            lb1.ToolTip = song.Path;
            sp.Children.Add(lb1);
            wp.Children.Add(sp);
            WrapPanel wp1 = new WrapPanel();
            wp1.Margin = new Thickness(0, 1, 0, 1);
            wp1.Height = 10;
            sp.Children.Add(wp1);
            wp1 = new WrapPanel();
            wp1.Margin = new Thickness(0, 1, 0, 0);
            Button btn = new Button();
            btn.Content = STR_OPENFOLDER;
            btn.Click += new RoutedEventHandler(expBtn_Click);
            wp1.Children.Add(btn);
            sp.Children.Add(wp1);
            wp1 = new WrapPanel();
            wp1.Margin = new Thickness(0, 1, 0, 0);
            btn = new Button();
            btn.Content = STR_DELBEATMAP;
            btn.Click += new RoutedEventHandler(expBtn_Click);
            wp1.Children.Add(btn);
            sp.Children.Add(wp1);
            return song;
        }

        private LocalSong CreateLocalListExpanderContent(Expander exp)
        {
            LocalSong song = (LocalSong)exp.DataContext;
            DockPanel wp = (DockPanel)exp.Content;
            Image img = new Image();
            img.MouseLeftButtonDown += new MouseButtonEventHandler(ExpImage_MouseLeftButtonDown);
            DateTime date = song.Date;
            string bg = song.BG;
            if (bg != "" && File.Exists(bg))
                img.Source = GetBgImg(song, bg);
            else
                img.Source = new BitmapImage(new Uri("img/3l.jpg", UriKind.RelativeOrAbsolute));
            wp.Children.Add(img);
            StackPanel sp = new StackPanel();
            WrapPanel wpx = new WrapPanel();
            Label lb1 = new Label();
            lb1.Content = STR_FILEDATE + date.Year + "-" + date.Month + "-" + date.Day + "  " + date.ToShortTimeString();
            wpx.Children.Add(lb1);
            SongDetail song2 = rankedList[song.Id.ToString()];
            if (song2 != null && song.DifficultyCount < song2.DiffCount)
            {
                lb1 = new Label();
                lb1.Content = string.Format(STR_LOSTDIFF, song2.DiffCount - song.DifficultyCount);
                lb1.Cursor = Cursors.Hand;
                lb1.Margin = new Thickness(10, 0, 0, 0);
                lb1.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
                lb1.MouseLeftButtonUp += new MouseButtonEventHandler((object sender, MouseButtonEventArgs e) =>
                {
                    e.Handled = true;
                    ProcessStart(songUri + song.Id);
                });
                wpx.Children.Add(lb1);
            }
            sp.Children.Add(wpx);
            lb1 = new Label();
            lb1.Content = STR_SONGPATH + song.Path;
            lb1.ToolTip = song.Path;
            sp.Children.Add(lb1);
            wp.Children.Add(sp);
            WrapPanel wp1 = new WrapPanel();
            wp1.ClipToBounds = true;
            wp1.Margin = new Thickness(0, 1, 0, 1);
            CheckBox cb = new CheckBox();
            cb.Checked += new RoutedEventHandler(expCb_Checked);
            cb.Unchecked += new RoutedEventHandler(expCb_Checked);
            cb.Content = STR_STORYBOARD;
            wp1.Children.Add(cb);
            cb = new CheckBox();
            cb.Checked += new RoutedEventHandler(expCb_Checked);
            cb.Unchecked += new RoutedEventHandler(expCb_Checked);
            cb.Content = STR_SKIN;
            wp1.Children.Add(cb);
            cb = new CheckBox();
            cb.Checked += new RoutedEventHandler(expCb_Checked);
            cb.Unchecked += new RoutedEventHandler(expCb_Checked);
            cb.Content = STR_HITSOUND;
            wp1.Children.Add(cb);
            cb = new CheckBox();
            cb.Checked += new RoutedEventHandler(expCb_Checked);
            cb.Unchecked += new RoutedEventHandler(expCb_Checked);
            cb.Content = STR_BACKUP;
            cb.IsChecked = true;
            wp1.Children.Add(cb);
            cb = new CheckBox();
            cb.Checked += new RoutedEventHandler(expCb_Checked);
            cb.Unchecked += new RoutedEventHandler(expCb_Checked);
            cb.Content = STR_BEATMAP;
            wp1.Children.Add(cb);
            sp.Children.Add(wp1);
            wp1 = new WrapPanel();
            // new Thickness(1, 0, 0, 0);
            wp1.Margin = new Thickness(0, 1, 0, 0);
            Button btn = new Button();
            btn.Content = STR_BEATMAPINFO;
            if (song.Id < 99999999)
                btn.Click += new RoutedEventHandler(expBtn_Click);
            wp1.Children.Add(btn);
            btn = new Button();
            btn.Click += new RoutedEventHandler(expBtn_Click);
            btn.Content = STR_MANAGEREP;
            wp1.Children.Add(btn);
            btn = new Button();
            btn.Content = STR_SAVEAUDIO;
            btn.Click += new RoutedEventHandler(expBtn_Click);
            wp1.Children.Add(btn);
            sp.Children.Add(wp1);
            wp1 = new WrapPanel();
            wp1.Margin = new Thickness(0, 1, 0, 0);
            btn = new Button();
            btn.Content = STR_OPENFOLDER;
            btn.Click += new RoutedEventHandler(expBtn_Click);
            wp1.Children.Add(btn);
            btn = new Button();
            btn.Content = STR_REPLACEBG;
            btn.Click += new RoutedEventHandler(expBtn_Click);
            wp1.Children.Add(btn);
            btn = new Button();
            btn.Content = STR_DELCHOOSE;
            btn.Click += new RoutedEventHandler(expBtn_Click);
            wp1.Children.Add(btn);
            sp.Children.Add(wp1);
            return song;
        }

        void expCb_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb == null) return;
            e.Handled = true;
            string tmp = cb.Content.ToString();
            if (tmp == STR_DOWNLOADWITHNV) cbFlag += ((cb.IsChecked.Value) ? 1 : -1) * 1;
            if (tmp == STR_STORYBOARD) cbFlag += ((cb.IsChecked.Value) ? 1 : -1) * 2;
            if (tmp == STR_SKIN) cbFlag += ((cb.IsChecked.Value) ? 1 : -1) * 4;
            if (tmp == STR_HITSOUND) cbFlag += ((cb.IsChecked.Value) ? 1 : -1) * 8;
            if (tmp == STR_BACKUP) cbFlag += ((cb.IsChecked.Value) ? 1 : -1) * 16;
            if (tmp == STR_BEATMAP) cbFlag += ((cb.IsChecked.Value) ? 1 : -1) * 32;
        }

        private static BitmapImage GetBgImg(LocalSong song, string bg)
        {
            byte[] buffer = File.ReadAllBytes(bg);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = new MemoryStream(buffer);
            image.EndInit();
            return image;
        }

        private static BitmapImage GetBgImg(Replay rep)
        {
            byte[] buffer = File.ReadAllBytes(rep.Song.Path + "\\" + rep.BG);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = new MemoryStream(buffer);
            image.EndInit();
            return image;
        }

        void ProcessStart(string ProcInfo)
        {
            try
            {
                Process.Start(ProcInfo);
            }
            catch (Exception e)
            {
                if (Core.Debug) throw;
                MessageBox.Show(e.ToString());
            }
        }

        void expBtn_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            SongDetail song = expCur.DataContext as SongDetail;
            Button btn = sender as Button;
            if (btn == null) return;
            try
            {
                string tmp = btn.Content.ToString();
                if (tmp == STR_OPENFOLDER)
                    ProcessStart((song as LocalSong).Path);
                if (tmp == STR_DELCHOOSE)
                    this.expDoSelect(song as LocalSong);
                if (tmp == STR_DELCFBEATMAP)
                {
                    Core.RemoveSong(song as LocalSong);
                    cfList.Remove(song);
                }
                if (tmp == STR_BEATMAPINFO)
                    ProcessStart(songUri + song.Id);
                if (tmp == STR_DOWNLOADBEATMAP)
                {
                    DlWindow.Show(this);
                    DlWindow.AddDownload(song as RankedSong, (cbFlag & 1) == 1);
                }
                if (tmp == STR_SAVEAUDIO)
                    SaveAudio(song as LocalSong);
                if (tmp == STR_REPLACEBG)
                {
                    System.Drawing.Image img = GetReplaceBg();
                    if (img != null)
                    {
                        Core.ReplaceSongBg(song as LocalSong, img, ((cbFlag >> 4 & 1) == 1));
                        ((Image)((DockPanel)expCur.Content).Children[0]).Source = GetBgImg(song as LocalSong, (song as LocalSong).BG);
                    }
                }
                if (tmp == STR_MANAGEREP)
                    showReplay();
            }
            catch (Core.StopException) { }
            catch (Exception ex)
            {
                if (Core.Debug) throw;
                MessageBox.Show(ex.ToString());
            }
        }

        private System.Drawing.Bitmap GetReplaceBg()
        {
            System.Drawing.Bitmap img = null;
            switch (MessageBox.Show(this, STR_MSGCHOOSEBG, STR_MSGCHOOSEBGTITLE, MessageBoxButton.YesNoCancel,
                MessageBoxImage.Information, MessageBoxResult.Cancel))
            {
                case MessageBoxResult.Yes:
                    Microsoft.Win32.OpenFileDialog f = new Microsoft.Win32.OpenFileDialog();
                    f.Filter = "Image File|*.jpg;*.png;*.bmp";
                    f.Title = STR_MSGCHOOSEBGTITLE;
                    f.CheckFileExists = true;
                    f.CheckPathExists = true;

                    if (f.ShowDialog(this).Value)
                    {
                        img = new System.Drawing.Bitmap(new MemoryStream(File.ReadAllBytes(f.FileName)));
                    }
                    else
                    {
                        uiBinding.IsReplaceBg = false;
                    }
                    break;
                case MessageBoxResult.No:
                    {
                        System.Drawing.Color cc;
                        switch (MessageBox.Show(this, STR_MSGCHOOSEBG2, STR_MSGCHOOSEBGTITLE,
                            MessageBoxButton.YesNoCancel, MessageBoxImage.Information, MessageBoxResult.Cancel))
                        {
                            case MessageBoxResult.Yes:
                                cc = System.Drawing.Color.Black;
                                break;
                            case MessageBoxResult.No:
                                cc = System.Drawing.Color.Gray;
                                break;
                            default:
                                uiBinding.IsReplaceBg = false;
                                return null;
                        }
                        img = new System.Drawing.Bitmap(2, 2);
                        img.SetPixel(0, 0, cc);
                        img.SetPixel(0, 1, cc);
                        img.SetPixel(1, 0, cc);
                        img.SetPixel(1, 1, cc);
                        break;
                    }
                default:
                    uiBinding.IsReplaceBg = false;
                    return null;
            }
            return img;
        }
        public static void SaveAudio(LocalSong song)
        {
            foreach (string s in Core.GetMp3Files(song))
            {
                Microsoft.Win32.SaveFileDialog f = new Microsoft.Win32.SaveFileDialog();
                f.DefaultExt = System.IO.Path.GetExtension(s);
                f.Title = STR_SAVEAUDIO;
                f.FileName = System.IO.Path.GetFileName(s);
                f.OverwritePrompt = true;
                if (f.ShowDialog().Value)
                {
                    File.Copy(((LocalSong)song).Path + "\\" + s, f.FileName, true);
                }
            }
        }
        private void expDoSelect(LocalSong song)
        {
            if ((cbFlag >> 5 & 1) == 1 &&
                MessageBox.Show(STR_DELBEATMAP + ":" + song.ToString() + "?", "", MessageBoxButton.YesNo, MessageBoxImage.Warning,
                MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                Core.RemoveSong(song);
                localList.Remove(song);
                return;
            }
            bool backup = ((cbFlag >> 4 & 1) == 1);
            if ((cbFlag >> 3 & 1) == 1)
            {
                Core.RemoveSound(song, backup);
            }
            if ((cbFlag >> 2 & 1) == 1)
            {
                Core.RemoveSkin(song, true, backup);
            }
            if ((cbFlag >> 1 & 1) == 1)
            {
                Core.RemoveSb(song, false, backup);
            }
        }


        private void expList_Expanded(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Expander exp = sender as Expander;
            if (expCur2 == null || expCur2 != exp)
            {
                if (expCur != null)
                    expCur.IsExpanded = false;
                ListBox lx = null;
                SongList sl = null;
                switch (exp.Name)
                {
                    case "expAll":
                        sl = rankedList;
                        lx = AllListBox;
                        break;
                    case "expDu":
                        sl = cfList;
                        lx = CfListBox;
                        break;
                    case "expLocal":
                        sl = localList;
                        lx = LocalListBox;
                        break;
                    case "expObj":
                        sl = objList;
                        lx = ObjListBox;
                        break;
                }
                cbFlag = 0;
                searchKey[0].Clear();
                searchKey[1].Clear();
                searchKey[2].Clear();
                tbxSearch1.Text = "";
                tbxSearch2.Text = "";
                tbxSearch3.Text = "";
                tbxSearch4.Text = "";

                ListCollectionView lcv = new ListCollectionView(sl);
                lcv.CustomSort = new SongComarer();

                Binding b = new Binding();
                b.Source = lcv;
                b.XPath = "";
                b.Mode = BindingMode.TwoWay;
                lx.SetBinding(ListBox.ItemsSourceProperty, b);

                lx.Items.Filter -= new Predicate<object>(ListFilter);
                lx.Items.Filter += new Predicate<object>(ListFilter);

                if (expCur2 != null)
                {
                    switch (expCur2.Name)
                    {
                        case "expAll":
                            AllListBox.ItemsSource = null;
                            break;
                        case "expDu":
                            CfListBox.ItemsSource = null;
                            break;
                        case "expLocal":
                            LocalListBox.ItemsSource = null;
                            break;
                        case "expObj":
                            ObjListBox.ItemsSource = null;
                            break;
                    }

                    expCur2.IsExpanded = false;
                }
                expCur2 = exp;
            }
        }
        private bool ListFilter(object sender)
        {
            return Core.Search(searchKey, ((SongDetail)sender).ToString(Format.Search).ToLower());
        }
        private bool ListFilter2(object sender)
        {
            return Core.Search(searchKey, ((Replay)sender).ToString(Format.Search).ToLower());
        }

        private void tabList_Unselected(object sender, RoutedEventArgs e)
        {
            if (!tabList.IsSelected && expCur2 != null)
            {
                AllListBox.ItemsSource = null;
                CfListBox.ItemsSource = null;
                LocalListBox.ItemsSource = null;
                ObjListBox.ItemsSource = null;
                Core.ReleaseAll();

                disposemp3 = new Thread(new ThreadStart(playerDispose));
                disposemp3.Start();

                cbFlag = 0;
                searchKey[0].Clear();
                searchKey[1].Clear();
                searchKey[2].Clear();
                tbxSearch1.Text = "";
                tbxSearch2.Text = "";
                tbxSearch3.Text = "";
                tbxSearch4.Text = "";
                expCur2.IsExpanded = false;
                expCur2 = null;

            }
            if (tSearch != null)
            {
                tSearch.Abort();
                tSearch = null;
            }
            Slide(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flag">false=SlideUp;true=SlideDown</param>
        private void Slide(bool flag)
        {
            if (tSlide != null)
                tSlide.Abort();
            tSlide = new Thread(() =>
            {
                try
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        if (flag)
                            for (int i = 0; i < 20; i++)
                            {
                                this.Height += 12;
                                if (this.Height > height_max)
                                {
                                    this.Height = height_max;
                                    break;
                                }
                            }
                        else
                            for (int i = 0; i < 20; i++)
                            {
                                this.Height -= 12;
                                if (this.Height < height_min)
                                {
                                    this.Height = height_min;
                                    break;
                                }
                            }
                    }));
                }
                catch (ThreadAbortException)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        if (flag)
                            this.Height = height_max;
                        else
                            this.Height = height_min;
                    }));
                }
                catch { }
            });
            tSlide.Start();
        }
        double height_min, height_max;
        Thread tSlide;
        private void tabList_Selected(object sender, RoutedEventArgs e)
        {
            Slide(true);
        }

        double searchDelay = 0;
        Thread tSearch;
        private void tbxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (expCur != null && repExp == null)
                expCur.IsExpanded = false;
            if (repExp != null)
                repExp.IsExpanded = false;
            searchDelay = 500;
            if (tSearch == null)
            {
                tSearch = new Thread(() =>
                {
                    while (searchDelay > 0)
                    {
                        Thread.Sleep(50);
                        searchDelay -= 50;
                    }
                    Dispatcher.Invoke(new Action(() =>
                    {
                        try
                        {
                            if (repPanel.Visibility == System.Windows.Visibility.Visible)
                            {
                                searchKey = Core.GetKeys(tbxSearch5.Text);
                                RepListBox.Items.Filter -= new Predicate<object>(ListFilter2);
                                RepListBox.Items.Filter += new Predicate<object>(ListFilter2);
                                return;
                            }
                            switch (expCur2.Name)
                            {
                                case "expAll":
                                    searchKey = Core.GetKeys(tbxSearch4.Text);
                                    AllListBox.Items.Filter -= new Predicate<object>(ListFilter);
                                    AllListBox.Items.Filter += new Predicate<object>(ListFilter);
                                    break;
                                case "expDu":
                                    searchKey = Core.GetKeys(tbxSearch3.Text);
                                    CfListBox.Items.Filter -= new Predicate<object>(ListFilter);
                                    CfListBox.Items.Filter += new Predicate<object>(ListFilter);
                                    break;
                                case "expLocal":
                                    searchKey = Core.GetKeys(tbxSearch1.Text);
                                    LocalListBox.Items.Filter -= new Predicate<object>(ListFilter);
                                    LocalListBox.Items.Filter += new Predicate<object>(ListFilter);
                                    break;
                                case "expObj":
                                    searchKey = Core.GetKeys(tbxSearch2.Text);
                                    ObjListBox.Items.Filter -= new Predicate<object>(ListFilter);
                                    ObjListBox.Items.Filter += new Predicate<object>(ListFilter);
                                    break;
                            }
                        }
                        catch
                        {
                            if (Core.Debug)
                                throw;
                        }
                    }));
                    tSearch = null;
                });
                tSearch.IsBackground = true;
                tSearch.Start();
            }
        }

        Storyboard ExpHeaderSb;
        private void ExpHeaderGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            DockPanel ExpGd;
            TextBlock ExpTb;
            ExpGd = sender as DockPanel;
            if (ExpGd == null) return;
            ExpTb = ExpGd.Children[0] as TextBlock;
            if (ExpTb == null) return;
            if (e.RoutedEvent.Name == "MouseEnter")
            {
                if (ExpHeaderSb == null)
                {
                    Storyboard sb = new Storyboard();
                    ExpHeaderSb = sb;
                    ExpTb.Name = "tbname";
                    this.RegisterName(ExpTb.Name, ExpTb);
                    ThicknessAnimation da = new ThicknessAnimation();
                    if (ExpGd.ActualWidth - ExpTb.ActualWidth >= 0) return;
                    da.To = new Thickness(ExpGd.ActualWidth - ExpTb.ActualWidth, 0, 0, 0);
                    da.From = new Thickness(0);
                    da.AutoReverse = true;
                    da.Duration = new Duration(TimeSpan.FromSeconds(5));
                    da.RepeatBehavior = new RepeatBehavior(5);

                    ExpHeaderSb.Children.Add(da);
                    Storyboard.SetTargetName(da, ExpTb.Name);
                    Storyboard.SetTargetProperty(da, new PropertyPath((TextBlock.MarginProperty)));

                    ExpHeaderSb.Begin(this, true);
                }
            }
            if (e.RoutedEvent.Name == "MouseLeave")
            {
                if (ExpHeaderSb != null)
                {
                    ExpHeaderSb.Stop(this);
                    ExpHeaderSb.Remove(this);
                    ExpHeaderSb.Children.Clear();
                    ExpHeaderSb = null;
                    this.UnregisterName(ExpTb.Name);
                    ExpTb.Margin = new Thickness(0);
                }
            }
        }

        private void ExpImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (player == null && playerFlag == false && disposemp3 == null)
            //if (psong == null && playerFlag == false && disposemp3 == null)
            {
                playerFlag = true;
                SongDetail song = null;
                if (expCur != null)
                    song = expCur.DataContext as SongDetail;
                if (repExp != null)
                    song = (repExp.DataContext as Replay).Song;
                threadgetmp3 = new Thread(new ThreadStart(() =>
                {

                    {
                        try
                        {
                            if (player == null)
                            {
                                player = new Mp3Player();
                                if (song.GetType().Name == "LocalSong")
                                    //player = new Audio(((LocalSong)song).Path + "\\" + Core.GetMp3Files((LocalSong)song)[0], false);
                                    player.Open(((LocalSong)song).Path + "\\" + Core.GetMp3Files((LocalSong)song)[0]);
                                else
                                    //player = new Audio("http://osu.ppy.sh/mp3/preview/" + ((SongDetail)song).Id + ".mp3", false);
                                    player.Open(Mp3Url + ((SongDetail)song).Id + ".mp3");
                            }
                            player.Volume = -1500;
                            player.Play();
                        }
                        catch (ThreadAbortException) { }
                        catch (Exception ex)
                        {
                            if (Core.Debug) MessageBox.Show(ex.ToString());
                            //MessageBox.Show("BGM加载失败");
                            if (player != null)
                            {
                                player.Dispose();
                                player = null;
                            }
                        }
                    }
                    threadgetmp3 = null;
                }));
                threadgetmp3.Start();
                return;
            }
            if (!playerFlag)
            {
                if (player != null && disposemp3 == null)
                {
                    if (player.CurrentPosition == player.Duration)
                        player.CurrentPosition = 0;
                    player.Play();
                    playerFlag = true;
                }
            }
            else
            {
                if (player != null && disposemp3 == null)
                {
                    if (player.CurrentPosition != player.Duration)
                    {
                        player.Pause();
                        playerFlag = false;
                    }
                    else
                    {
                        if (player.Duration == 0) return;
                        player.CurrentPosition = 0;
                        player.Play();
                    }
                }
            }
        }
        /*
        private void newPlayer(object song)
        {
            try
            {
                if (player == null)
                {
                    //player = new MediaPlayer();
                    //Audio.f
                    if (song.GetType().Name == "LocalSong")
                        player.Open(new Uri(((LocalSong)song).Path + "\\" + Core.GetMp3Files((LocalSong)song)[0], UriKind.RelativeOrAbsolute));
                    //player = new Audio(((LocalSong)song).Path + "\\" + Core.GetMp3Files((LocalSong)song)[0], false);
                    else
                        player.Open(new Uri("http://osu.ppy.sh/mp3/preview/" + ((SongDetail)song).Id + ".mp3", UriKind.RelativeOrAbsolute));
                        //player = new Audio("http://osu.ppy.sh/mp3/preview/" + ((SongDetail)song).Id + ".mp3", false);
                }
                //player.Volume = -1500;
                player.Volume = 80;
                player.Play();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                if (player != null)
                {
                    player.Close();
                    player = null;
                    //player.Dispose();
                }
            }
        }*/

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            /*
            disposemp3 = new Thread(new ThreadStart(playerDispose));
            disposemp3.Start();
            e.Handled = true;
            if (player != null)
            {

                new Thread(new ThreadStart(() =>
                {
                    if (threadgetmp3 != null)
                        threadgetmp3.Abort();
                    Dispatcher.BeginInvoke(((Action)(() =>
                    {
                        if (player != null)
                        {
                            player.Close();
                            //player.Dispose();
                        }
                        player = null;
                    })));
                    playerFlag = false;
                    disposemp3 = null;
                })).Start();
            }*/
            if (player != null)
            {
                disposemp3 = new Thread(new ThreadStart(playerDispose));
                disposemp3.Start();
            }
        }

        private void cbxReplaceBG_Checked(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (uiBinding.IsReplaceBg) rpimg = GetReplaceBg();
            else rpimg = null;
        }

        private void OsuBtn_Click(object sender, RoutedEventArgs e)
        {
            ProcessStart(osuPath + "\\osu!.exe");
        }

        private void repPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            ProcessStart("http://www.muscipular.net/OSU/tid-185.html");
            e.Handled = true;
        }

        private void BtnSaveHtml_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;
            e.Handled = true;
            Microsoft.Win32.SaveFileDialog f = new Microsoft.Win32.SaveFileDialog();
            f.Title = STR_MSGSAVELISTTITLE;
            f.Filter = "HTML File|*.html";
            SongList list = null;
            string title = "";
            string tmp = btn.Content.ToString();
            if (tmp == STR_BTNSAVECFLIST)
            {
                list = GetSaveList(tbxSearch3, CfListBox);
                if (list == null) list = cfList;
                f.FileName = STR_MSGSAVECFLIST + ".html";
                title = STR_MSGSAVECFLIST;
            }
            if (tmp == STR_BTNSAVEALLLIST)
            {
                list = GetSaveList(tbxSearch4, AllListBox);
                if (list == null) list = rankedList;
                f.FileName = STR_MSGSAVEALLLIST + ".html";
                title = STR_MSGSAVEALLLIST;
            }
            if (tmp == STR_BTNSAVEOBJLIST)
            {
                list = GetSaveList(tbxSearch2, ObjListBox);
                if (list == null) list = objList;
                f.FileName = STR_MSGSAVEOBJLIST + ".html";
                title = STR_MSGSAVEOBJLIST;
            }
            if (tmp == STR_BTNSAVELOCALLIST)
            {
                list = GetSaveList(tbxSearch1, LocalListBox);
                if (list == null) list = localList;
                title = STR_MSGSAVELOCALLIST;
                f.FileName = STR_MSGSAVELOCALLIST + ".html";
            }
            if (f.ShowDialog(this).Value)
                if (MessageBox.Show(STR_MSGSAVELISTCONDITION, "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    Core.BuildHTML(list, f.FileName, title);
                else
                    Core.BuildHTML2(list, f.FileName, title);
        }

        private SongList GetSaveList(TextBox tbx, ListBox lxb)
        {
            SongList list = null;
            if (!string.IsNullOrWhiteSpace(tbx.Text.Trim()))
            {
                list = new SongList();
                foreach (SongDetail song in lxb.Items)
                {
                    list.Add(song);
                }
            }
            return list;
        }

        private void showReplay()
        {
            replays = new System.Collections.ObjectModel.ObservableCollection<Replay>();
            LocalSong curSong = null;
            if (expCur != null)
                curSong = expCur.DataContext as LocalSong;
            Dictionary<string, LocalSong> dicsong = new Dictionary<string, LocalSong>();
            Dictionary<string, BeatmapDetail> dicbeatmap = new Dictionary<string, BeatmapDetail>();
            if (curSong == null)
            {
                foreach (LocalSong song in Core.LocalList)
                {
                    foreach (BeatmapDetail beatmap in song.Difficulties)
                    {
                        dicbeatmap[beatmap.Hash.ToLower()] = beatmap;
                        dicsong[beatmap.Hash.ToLower()] = song;
                    }
                }
            }
            else
            {
                foreach (BeatmapDetail beatmap in curSong.Difficulties)
                {
                    dicbeatmap[beatmap.Hash.ToLower()] = beatmap;
                    dicsong[beatmap.Hash.ToLower()] = curSong;
                }
            }
            try
            {
                GetReplays(dicsong, dicbeatmap, true);
                if (curSong != null)
                    GetReplays(dicsong, dicbeatmap, false);
                ListCollectionView lcv = new ListCollectionView(replays);
                lcv.CustomSort = new ReplayComparer();

                Binding b = new Binding();
                b.Source = lcv;
                b.XPath = "";
                b.Mode = BindingMode.TwoWay;
                RepListBox.SetBinding(ListBox.ItemsSourceProperty, b);

                if (RepListBox.Items.Count > 0)
                {
                    RepListBox.SelectedIndex = 0;
                }
                repPanel.Visibility = Visibility.Visible;
                BtnAbout.IsEnabled = false;
            }
            catch (Exception e)
            {
                if (Core.Debug) throw;
                MessageBox.Show(e.ToString());
            }
        }

        private void GetReplays(Dictionary<string, LocalSong> dicsong, Dictionary<string, BeatmapDetail> dicbeatmap, bool flag)
        {
            string path;
            if (flag) path = @"\Data\r";
            else path = @"\Replays";
            if (Directory.Exists(Core.OsuPath + path))
            {
                string[] files = Directory.GetFiles(Core.OsuPath + path);
                foreach (string file in files)
                {
                    FileStream fs = new FileStream(file, FileMode.Open);
                    byte[] buffer = new byte[1024];
                    buffer.Initialize();
                    fs.Read(buffer, 0, 1024);
                    fs.Close();
                    string hash = string.Empty;
                    for (int i = 7; i < 39; i++)
                    {
                        hash += (char)buffer[i];
                    }
                    if (dicbeatmap.ContainsKey(hash.ToLower()))
                        replays.Add(new Replay(file, dicsong[hash.ToLower()], dicbeatmap[hash.ToLower()], buffer));
                }
            }
        }


        private void RepExpander_Expanded(object sender, RoutedEventArgs e)
        {
            Expander exp = sender as Expander;
            Replay rep = exp.DataContext as Replay;
            LocalSong song = rep.Song;
            StackPanel topPanel = exp.Content as StackPanel;
            topPanel.Orientation = Orientation.Vertical;
            StackPanel tp = new StackPanel();
            tp.Orientation = Orientation.Horizontal;
            StackPanel lp = new StackPanel();
            lp.Orientation = Orientation.Horizontal;

            Label lb = new Label();
            lb.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            lb.Content = lp;

            topPanel.Children.Add(tp);
            topPanel.Children.Add(lb);

            Image img = new Image();
            img.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            img.Cursor = Cursors.Hand;
            img.MouseLeftButtonDown += new MouseButtonEventHandler(ExpImage_MouseLeftButtonDown);
            if (rep.BG != "" && File.Exists(rep.Song.Path + "\\" + rep.BG))
                img.Source = GetBgImg(rep);
            else
                img.Source = new BitmapImage(new Uri("img/3l.jpg", UriKind.RelativeOrAbsolute));
            tp.Children.Add(img);

            Grid gd = new Grid();
            System.Windows.Media.FontFamily Arial = new System.Windows.Media.FontFamily("Arial");

            lb = new Label();
            lb.FontFamily = Arial;
            lb.FontSize = 24;
            lb.Content = "Score:  " + rep.Score;
            lb.FontWeight = System.Windows.FontWeight.FromOpenTypeWeight(200);
            lb.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            gd.Children.Add(lb);

            lb = new Label();
            lb.FontFamily = Arial;
            lb.FontSize = 14;
            lb.Content = "Mods(" + rep.Mod + ")";
            lb.Margin = new Thickness(4, 40, 0, 0);
            gd.Children.Add(lb);

            lb = new Label();
            lb.FontFamily = Arial;
            lb.FontSize = 14;
            lb.Content = "Date:  " + rep.Time;
            lb.Margin = new Thickness(4, 26, 0, 0);
            lb.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            gd.Children.Add(lb);


            lb = new Label();
            lb.FontFamily = Arial;
            lb.FontSize = 14;
            lb.Content = "300";
            lb.Margin = new Thickness(4, 54, 0, 0);
            gd.Children.Add(lb);
            lb = new Label();
            lb.FontFamily = Arial;
            lb.FontSize = 14;
            lb.Content = "x " + rep.Hit300;
            lb.Margin = new Thickness(35, 54, 0, 0);
            gd.Children.Add(lb);


            lb = new Label();
            lb.FontFamily = Arial;
            lb.FontSize = 14;
            lb.Content = "50 ";
            lb.Margin = new Thickness(154, 54, 0, 0);
            gd.Children.Add(lb);
            lb = new Label();
            lb.FontFamily = Arial;
            lb.FontSize = 14;
            lb.Content = "x " + rep.Hit50;
            lb.Margin = new Thickness(185, 54, 0, 0);
            gd.Children.Add(lb);

            lb = new Label();
            lb.FontFamily = Arial;
            lb.FontSize = 14;
            lb.Content = "100";
            lb.Margin = new Thickness(4, 68, 0, 0);
            gd.Children.Add(lb);
            lb = new Label();
            lb.FontFamily = Arial;
            lb.FontSize = 14;
            lb.Content = "x " + rep.Hit100;
            lb.Margin = new Thickness(35, 68, 0, 0);
            gd.Children.Add(lb);


            lb = new Label();
            lb.FontFamily = Arial;
            lb.FontSize = 14;
            lb.Content = "Miss";
            lb.Margin = new Thickness(154, 68, 0, 0);
            gd.Children.Add(lb);
            lb = new Label();
            lb.FontFamily = Arial;
            lb.FontSize = 14;
            lb.Content = "x " + rep.Miss;
            lb.Margin = new Thickness(185, 68, 0, 0);
            gd.Children.Add(lb);



            lb = new Label();
            lb.FontFamily = Arial;
            lb.FontSize = 14;
            lb.Content = "MaxCombo:  " + rep.MaxCombo;
            lb.Margin = new Thickness(4, 82, 0, 0);
            gd.Children.Add(lb);

            lb = new Label();
            lb.FontFamily = Arial;
            lb.FontSize = 14;
            lb.Content = "Accuracy:  " + rep.Acc;
            lb.Margin = new Thickness(154, 82, 0, 0);
            gd.Children.Add(lb);

            tp.Children.Add(gd);


            Button btn = new Button();
            btn.Content = STR_BTNSAVEAS;
            btn.Click += new RoutedEventHandler(repBtnClick);
            lp.Children.Add(btn);

            btn = new Button();
            btn.Content = STR_BTNDELETEREPLAY;
            btn.Click += new RoutedEventHandler(repBtnClick);
            lp.Children.Add(btn);

            if (repExp != null && repExp != exp)
            {
                repExp.IsExpanded = false;
                repExp = exp;
            }
            else
                if (repExp == null) repExp = exp;

            RepListBox.SelectedItem = exp.DataContext;
        }

        void repBtnClick(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            e.Handled = true;
            if (btn.Content.ToString() == STR_BTNSAVEAS)
            {
                SaveReplay();
            }
            if (btn.Content.ToString() == STR_BTNDELETEREPLAY)
            {
                RemoveReplay();
            }
        }

        private void RemoveReplay()
        {
            if (repExp == null) return;
            int index = RepListBox.SelectedIndex - 1;
            Replay rep = (Replay)repExp.DataContext;
            if (File.Exists(rep.Path))
            {
                repExp.IsExpanded = false;
                File.Delete(rep.Path);
                replays.Remove(rep);
            }
            if (replays.Count == 0) return;
            RepListBox.SelectedIndex = index;
        }

        private void SaveReplay()
        {
            if (replays.Count == 0) return;
            Replay rep = (Replay)repExp.DataContext;
            SaveFileDialog f = new SaveFileDialog();
            f.Title = STR_BTNSAVEAS;
            f.FileName = rep.ToString(Format.Save);
            f.DefaultExt = ".osr";
            f.AddExtension = true;
            f.Filter = "Replay|*.osr";
            f.ValidateNames = true;
            if (f.ShowDialog().Value)
            {
                File.Copy(rep.Path, f.FileName, true);
                File.SetLastWriteTime(f.FileName, DateTime.Parse(rep.Time));
                File.SetCreationTime(f.FileName, DateTime.Parse(rep.Time));
                File.SetLastAccessTime(f.FileName, DateTime.Parse(rep.Time));
            }
        }

        private void RepExpander_Collapsed(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Expander exp = sender as Expander;
            ((StackPanel)exp.Content).Children.Clear();
            if (player != null)
            {
                disposemp3 = new Thread(new ThreadStart(playerDispose));
                disposemp3.Start();
            }
        }

        private void tabReplay_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (uiBinding.IsWorking) return;
            tabList.IsSelected = false;
            tabMain.IsSelected = true;
            showReplay();
        }

        private void BtnClose2_Click(object sender, RoutedEventArgs e)
        {
            repPanel.Visibility = Visibility.Hidden;
            BtnAbout.IsEnabled = true;
            repExp = null;
            if (player != null)
            {
                disposemp3 = new Thread(new ThreadStart(playerDispose));
                disposemp3.Start();
            }
        }

        private void tabAbout_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DlWindow.Show(this);
            DlWindow.Focus();
            e.Handled = true;
            return;
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            this.Topmost = !this.Topmost;
        }
    }



    public class RankingConverter : IValueConverter
    {
        static BitmapImage XH = new BitmapImage(new Uri("/BeatmapTool;component/img/ranking-XH-small.png", UriKind.RelativeOrAbsolute));
        static BitmapImage X = new BitmapImage(new Uri("/BeatmapTool;component/img/ranking-X-small.png", UriKind.RelativeOrAbsolute));
        static BitmapImage SH = new BitmapImage(new Uri("/BeatmapTool;component/img/ranking-SH-small.png", UriKind.RelativeOrAbsolute));
        static BitmapImage S = new BitmapImage(new Uri("/BeatmapTool;component/img/ranking-S-small.png", UriKind.RelativeOrAbsolute));
        static BitmapImage A = new BitmapImage(new Uri("/BeatmapTool;component/img/ranking-A-small.png", UriKind.RelativeOrAbsolute));
        static BitmapImage B = new BitmapImage(new Uri("/BeatmapTool;component/img/ranking-B-small.png", UriKind.RelativeOrAbsolute));
        static BitmapImage C = new BitmapImage(new Uri("/BeatmapTool;component/img/ranking-C-small.png", UriKind.RelativeOrAbsolute));
        static BitmapImage D = new BitmapImage(new Uri("/BeatmapTool;component/img/ranking-D-small.png", UriKind.RelativeOrAbsolute));
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch (value.ToString().ToUpper())
            {
                case "XH": return XH;
                case "X": return X;
                case "SH": return SH;
                case "S": return S;
                case "A": return A;
                case "B": return B;
                case "C": return C;
                case "D": return D;
            }
            return null;
        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

}
