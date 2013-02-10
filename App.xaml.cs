using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows;

namespace BeatmapTool
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private const string CoreDllFileName = "BeatmapTool.Core.dll";
        private const string PakFileName = "BeatmapTool.Core.pak";
        private Assembly CoreAssembly { get; set; }
        private ICore Core { get; set; }
        public App()
        {
            try
            {
                var core = File.ReadAllBytes(CoreDllFileName);
                this.CoreAssembly = Assembly.Load(core);
                this.Core = (ICore)CoreAssembly.CreateInstance("BeatmapTool.Core.Main");
                CheckLastVersionAsync();
                this.Startup += (sender, e) => Core.Run(null);
            }
            catch (Exception)
            {
                MessageBox.Show("error load main!");
                this.Shutdown(-1);
            }
        }

        public void CheckLastVersionAsync()
        {
            WebClient webClient = new WebClient();
            webClient.DownloadStringAsync(new Uri("http://beatmaptool.googlecode.com/files/version.json"));
            webClient.DownloadStringCompleted += (sender, e) =>
            {
                if (e.Cancelled || e.Error != null || string.IsNullOrWhiteSpace(e.Result))
                {
                    return;
                }
                try
                {
                    var lastVersion = Newtonsoft.Json.JsonConvert.DeserializeObject<CoreVersion>(e.Result);
                    if (Core.Version >= lastVersion.Version)
                    {
                        return;
                    }
                    MessageBoxResult result = MessageBoxResult.Cancel;
                    Dispatcher.Invoke(new Action(() =>
                    {
                        result = MessageBox.Show(MainWindow, "是否更新到新版本？\n" + lastVersion.Desciption, "提示", MessageBoxButton.YesNo);
                    }));
                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                        case MessageBoxResult.OK:
                            UpdateToVersion(lastVersion);
                            break;
                    }
                }
                catch (Exception)
                {
                    return;
                }
                finally
                {
                    ((WebClient)sender).Dispose();
                }
            };
        }

        private void UpdateToVersion(CoreVersion version)
        {
            using (WebClient webClient = new WebClient())
            {
                for (int i = 0; i < version.Files.Length; i++)
                {
                    string tmpFile = version.Files[i] + ".tmp";
                    try
                    {
                        webClient.DownloadFile(new Uri(version.Url[i]), tmpFile);
                    }
                    catch (Exception)
                    {
                        foreach (var file in version.Files)
                        {
                            if (File.Exists(file + ".tmp"))
                            {
                                File.Delete(file + ".tmp");
                            }
                        }
                        return;
                    }
                }
                foreach (var file in version.Files)
                {
                    if (File.Exists(file))
                    {
                        File.Delete(file);
                    }
                    File.Move(file + ".tmp", file);
                }
            }
            Process.Start(Process.GetCurrentProcess().MainModule.FileName);
            Shutdown(0);
        }
    }
}
