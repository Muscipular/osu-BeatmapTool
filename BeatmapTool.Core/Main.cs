using System.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Awesomium.Core;

namespace BeatmapTool.Core
{
    public sealed class Main : ICore
    {
        static Main()
        {
            WebCore.Initialize(new WebConfig { LogLevel = LogLevel.Verbose, ManagedConsole = false });
        }

        public int Version
        {
            get { return 1; }
        }

        public void Run(Dictionary<string, object>[] args)
        {
            var win = new MainWindow();
            Application.Current.MainWindow = win;
            win.Show();
            Application.Current.Exit += Current_Exit;
        }

        void Current_Exit(object sender, ExitEventArgs e)
        {
            WebCore.Shutdown();
        }
    }
}
