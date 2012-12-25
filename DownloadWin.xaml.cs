using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
using System.Diagnostics;

namespace BeatmapTool
{
    /// <summary>
    /// DownloadWin.xaml 的交互逻辑
    /// </summary>
    public partial class DownloadWin : Window
    {
        private UIBinding uiBinding;
        private System.Collections.ObjectModel.ObservableCollection<Dictionary<string, object>> queue = new System.Collections.ObjectModel.ObservableCollection<Dictionary<string, object>>();
        //private List<Dictionary<string, object>> queue = new List<Dictionary<string, object>>();
        private Thread thread;

        private static readonly string STOP;
        private static readonly string OPEN;
        private static readonly string RETRY;
        private static readonly string REMOVE;
        private static readonly string START;
        private static readonly string STR_LOGINFAILED;
        private static readonly string STR_DOWNLOADFAILED;
        private static readonly string STR_LOGINTIPS;
        private static readonly string STR_CHOOSEFOLDER;
        private static readonly string STR_CHOOSEFOLDERMSG;
        private static readonly string STR_TBTITLE;
        private static readonly string STR_BTNHIDE;
        private static readonly string STR_TBID;
        private static readonly string STR_TBWAIT;
        private static readonly string STR_TBWAIT2;
        private static readonly string STR_TBPWD;
        private static readonly string STR_BTNLOGIN;
        private static readonly string STR_BTNLOGOUT;
        private static readonly string STR_TBDOWNLOADFOLDER;
        private static readonly string STR_BTNCHOOSEFOLDER;

        static DownloadWin()
        {
            STOP = "停止";
            OPEN = "打开";
            RETRY = "重试";
            REMOVE = "移除";
            START = "开始";
            STR_LOGINFAILED = "登陆失败!";
            STR_DOWNLOADFAILED = "下载失败.";
            STR_LOGINTIPS = "请先登陆";
            STR_CHOOSEFOLDER = "选择下载目录？";
            STR_CHOOSEFOLDERMSG = "选择下载到的文件夹";
            STR_TBTITLE = "Beatmap下载队列";
            STR_BTNHIDE = "隐藏";
            STR_TBID = "用户名:";
            STR_TBWAIT = "等待时间:";
            STR_TBWAIT2 = "秒";
            STR_TBPWD = "密码:";
            STR_BTNLOGIN = "登陆";
            STR_BTNLOGOUT = "登出";
            STR_TBDOWNLOADFOLDER = "下载目录:";
            STR_BTNCHOOSEFOLDER = "选择目录";
       }

        public DownloadWin()
        {
            InitializeComponent();            
        }

        public DownloadWin(UIBinding uiBinding)
        {
            InitializeComponent();
            this.uiBinding = uiBinding;
            tb1.DataContext = uiBinding;
            tb2.DataContext = uiBinding;
            tbx1.DataContext = uiBinding;
            QueueList.ItemsSource = queue;
            tbTitle.Text = STR_TBTITLE;
            btnHide.ToolTip = STR_BTNHIDE;
            tbId.Text = tbId2.Text = STR_TBID;
            tbWait.Text = STR_TBWAIT;
            tbWait2.Text = STR_TBWAIT2;
            tbPwd.Text = STR_TBPWD;
            btnLogin.Content = STR_BTNLOGIN;
            btnLogout.Content = STR_BTNLOGOUT;
            tbDownloadFolder.Text = STR_TBDOWNLOADFOLDER;
            btnChooseFolder.Content = STR_BTNCHOOSEFOLDER;
            //.DataContext = uiBinding;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            this.Hide();
            this.Owner.Focus();
        }

        public void AddDownload(RankedSong song,bool flag)
        {
            foreach (Dictionary<string, object> tmp in queue)
            {
                if (tmp["song"].Equals(song)) return;
            }
            Dictionary<string, object> item = new Dictionary<string, object>();
            item.Add("song", song);
            item.Add("flag", flag);
            queue.Add(item);
            if (thread == null)
            {
                thread = new Thread(new ThreadStart(Download));
                thread.Start();
            }
        }

        public void Login(string username,string password)
        {
            thread = new System.Threading.Thread(new System.Threading.ThreadStart(() =>
            {
                if (Core.Login(uiBinding.UserName, uiBinding.PassWord))
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        wp1.Visibility = System.Windows.Visibility.Collapsed;
                        wp2.Visibility = System.Windows.Visibility.Visible;
                    }));
                    login = true;
                }
                else
                {
                    login = false;
                    MessageBox.Show(STR_LOGINFAILED);
                }
                Dispatcher.Invoke(new Action(() =>
                {
                    btnLogin.IsEnabled = true;
                }));
                thread = null;
            }));
            thread.IsBackground = true;
            thread.Start();
        }

        public void Show(Window Owner)
        {
            if (tb3.Text == "")
            {
                if (MessageBox.Show(STR_CHOOSEFOLDER, "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    SelectFolder();
                }
                else
                {
                    tb3.Text = Core.OsuPath + @"\Songs\";
                    path = tb3.Text;
                }
            }
            this.Owner = Owner;
            this.Show();
        }
        string path="";
        bool login = false;
        private void Download()
        {
            Dictionary<string, object> item = GetFirst();
            if (item == null || this.path == "")
            {
                thread = null;
                return;
            }
            Thread.Sleep((uiBinding.WaitTime + 5) * 1000);
            SetBinding(item, false, STOP);
            RankedSong song = item["song"] as RankedSong;
            string path = this.path;
            switch (Core.Download(song, "http://osu.ppy.sh/d/" + song.Id + ((bool)item["flag"] ? "n" : ""), ref path))
            {
                case Core.DownLoadState.Done:
                    item.Add("path", path);
                    SetBinding(item, true, OPEN);
                    thread = new Thread(new ThreadStart(Download));
                    thread.Start();
                    return;
                case Core.DownLoadState.Error:
                case Core.DownLoadState.Failed:
                    if (wp2.Visibility != System.Windows.Visibility.Collapsed)
                    {
                        SetBinding(item, true, RETRY);
                        thread = new Thread(new ThreadStart(Download));
                        thread.Start();
                    }
                    MessageBox.Show(STR_DOWNLOADFAILED);
                    return;
                case Core.DownLoadState.LoginFailed:
                    SetBinding(item, true, RETRY);
                    thread = null;
                    if (!login)
                        MessageBox.Show(STR_LOGINTIPS);
                    else
                        MessageBox.Show(STR_LOGINFAILED);
                    return;
                case Core.DownLoadState.Wait:
                    Thread.Sleep((uiBinding.WaitTime + 5) * 1000);
                    thread = new Thread(new ThreadStart(Download));
                    thread.Start();
                    return;
                case Core.DownLoadState.Success:
                default:
                    thread = null;
                    return;
            }
        }

        private void SetBinding(Dictionary<string, object> item,bool flag,string info)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                Grid gd = item["ui"] as Grid;
                foreach (UIElement ui in gd.Children)
                {
                    switch (ui.GetType().Name)
                    {
                        case "ProgressBar":
                            ProgressBar pb = ui as ProgressBar;
                            if (!flag)
                            {
                                uiBinding.ProcCur2 = 0;
                                uiBinding.ProcMax2 = 1000;
                            }
                            pb.SetBinding(ProgressBar.ValueProperty, GetBinding("ProcCur2", flag));
                            pb.SetBinding(ProgressBar.MaximumProperty, GetBinding("ProcMax2", flag));
                            break;
                        case "Label":
                            Label lb = ui as Label;
                            if (!flag) uiBinding.ProcName2 = lb.Content.ToString();
                            lb.SetBinding(Label.ToolTipProperty, GetBinding("ProcName2", flag));
                            lb.SetBinding(Label.ContentProperty, GetBinding("ProcName2", flag));
                            break;
                        case "Button":
                            Button btn = ui as Button;
                            btn.Content = info;
                            break;
                        default:
                            break;
                    }
                }
            }));
        }
        public void Exit()
        {
            if (thread != null)
                thread.Abort();
            thread = null;
        }
        private Binding GetBinding(string path,bool flag)
        {
            Binding binding = new Binding();
            binding.Source = uiBinding;
            if (flag) binding.Mode = BindingMode.OneTime;
            else binding.Mode = BindingMode.OneWay;
            binding.Path = new PropertyPath(path);
            return binding;
        }
        private Dictionary<string, object> GetFirst()
        {
            Dictionary<string, object> item = null;
            try
            {
                item = queue.First(new Func<Dictionary<string, object>, bool>(func1));
            }
            catch(Exception)
            {
                if (Core.Debug) throw;
                item = null;
            }
            return item;
        }
        private bool func1(Dictionary<string, object> obj)
        {
            bool resualt = false;
            while (!obj.ContainsKey("ui"))
            {
                Thread.Sleep(500);
            }
            Dispatcher.Invoke(new Action(() =>
            {
                string str = ((obj["ui"] as Grid).Children[2] as Button).Content.ToString();
                if (str != OPEN && str != RETRY)
                    resualt = true;
            }));
            return resualt;
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            uiBinding.PassWord = pwd.Password;
            if (uiBinding.PassWord.Length < 6 || uiBinding.UserName.Length < 6)
                return;
            btnLogin.IsEnabled = false;
            Login(uiBinding.UserName, uiBinding.PassWord);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Core.Logout();
            uiBinding.PassWord = "";
            login = false;
            wp1.Visibility = System.Windows.Visibility.Visible;
            wp2.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            SelectFolder();
        }

        private void SelectFolder()
        {
            System.Windows.Forms.FolderBrowserDialog f = new System.Windows.Forms.FolderBrowserDialog();
            f.ShowNewFolderButton = false;
            f.Description = STR_CHOOSEFOLDERMSG;
            if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                tb3.Text = f.SelectedPath + "\\";
                path = f.SelectedPath + "\\";
            }
            else
            {
                tb3.Text = Core.OsuPath + @"\Songs\";
                path = tb3.Text;
            }
            f.Dispose();
        }
        static SolidColorBrush Brush1 = new SolidColorBrush(Colors.AntiqueWhite);//pbBrush
        static SolidColorBrush Brush2 = new SolidColorBrush(Colors.SlateBlue);//lb
        static SolidColorBrush Brush3 = new SolidColorBrush(Color.FromArgb(80, 252, 252, 252));//btn
        static SolidColorBrush Brush4 = new SolidColorBrush(Colors.BlueViolet);//btn
        private void StackPanel_Loaded(object sender, RoutedEventArgs e)
        {
            Grid sp = sender as Grid;
            Dictionary<string, object> item = sp.DataContext as Dictionary<string, object>;
            if (!item.ContainsKey("ui"))
            {
                item.Add("ui", sp);
                RankedSong song = item["song"] as RankedSong;
                ProgressBar pb = new ProgressBar();
                //pb.Width = sp.Width;
                pb.Background = Brush1;
                sp.Children.Add(pb);
                Label lb = new Label();
                lb.FontSize = 12;
                lb.Foreground = Brush2;
                lb.ToolTip = song.ToString(Format.Download);
                lb.Padding = new Thickness(6);
                lb.Margin = new Thickness(4, 0, 38, 0);
                lb.Content = lb.ToolTip;
                sp.Children.Add(lb);
                Button btn = new Button();
                btn.Content = REMOVE;
                btn.Background = Brush3;
                btn.BorderThickness = new Thickness(0);
                btn.Foreground = Brush4;
                //btn.BorderBrush = new SolidColorBrush(Color.FromArgb(100, 252, 252, 252));
                btn.Margin = new Thickness(0, 3, 3, 3);
                btn.Focusable = false;
                btn.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                btn.Height = 24;
                btn.Width = 38;
                btn.Cursor = Cursors.Hand;
                btn.Click += new RoutedEventHandler(btn_Click);
                sp.Children.Add(btn);
            }
            else
            {
                Grid obj = item["ui"] as Grid;
                UIElement[] l = new UIElement[obj.Children.Count];
                for (int i = 0; i < obj.Children.Count; i++)
                {
                    l[i] = obj.Children[i];
                }
                obj.Children.Clear();
                for (int i = 0; i < l.Length; i++)
                {
                    sp.Children.Add(l[i]);
                }
                item["ui"] = sp;
            }
        }
        //int index=0;
        void btn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string tmp = btn.Content.ToString();
            if (tmp == STOP)
            {
                if (thread != null)
                {
                    thread.Abort();
                    thread = null;
                }
                int index = queue.IndexOf(btn.DataContext as Dictionary<string, object>);
                queue.Remove(btn.DataContext as Dictionary<string, object>);
                if (queue.Count > index)
                {
                    ((queue[index]["ui"] as Grid).Children[2] as Button).Content = START;
                }
            }
            if (tmp == REMOVE)
            {
                queue.Remove(btn.DataContext as Dictionary<string, object>);
            }
            if (tmp == OPEN)
            {
                Process.Start((btn.DataContext as Dictionary<string, object>)["path"].ToString());
                queue.Remove(btn.DataContext as Dictionary<string, object>);
            }
            if (tmp == RETRY || tmp == START)
            {
                btn.Content = STOP;
                if (thread == null)
                {
                    thread = new Thread(new ThreadStart(Download));
                    thread.Start();
                }
            }
        }
    }
}
