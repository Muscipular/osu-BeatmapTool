using Awesomium.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Awesomium.Core.Data;


namespace BeatmapTool.Core
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            webControl.WebSession = WebCore.CreateWebSession(new WebPreferences
            {
                UniversalAccessFromFileURL = true,
                CanScriptsAccessClipboard = true,
                CanScriptsCloseWindows = true,
                CanScriptsOpenWindows = true,
                WebSecurity = false,
                DefaultEncoding = "utf-8",
            });
            var dataPakSource = new GzipDataSource("BeatmapTool.Core.pak");
            webControl.WebSession.AddDataSource("local", dataPakSource);
            webControl.WebSession.AddDataSource("localfile", new DirectoryDataSource("./session"));
            webControl.ProcessCreated += OnWebLoad;
            //            webControl.PreviewKeyDown += (sender, args) => args.Handled = webControl.FocusedElementType != FocusedElementType.EditableContent
            //                && webControl.FocusedElementType != FocusedElementType.TextInput;
            webControl.ShowContextMenu += (sender, args) => args.Handled = !args.Info.IsEditable;
            webControl.Source = new Uri("asset://local/web/index.html");
        }

        private void OnWebLoad(object sender, EventArgs e)
        {
            var app = new Bridge.App();
            var win = new Bridge.Win(this);
            dynamic jsCommon = (JSObject)webControl.CreateGlobalJavascriptObject("$APP");
            dynamic window = (JSObject)webControl.CreateGlobalJavascriptObject("$APP_Window");
            jsCommon.window = new JSValue(window);
            window.width = (JavascriptMethodEventHandler)win.Width;
            window.height = (JavascriptMethodEventHandler)win.Height;
            jsCommon.close = (JavascriptMethodEventHandler)app.Exit;
            window.alert = (JavascriptMethodEventHandler)win.Alert;
            jsCommon.hashString = (JavascriptMethodEventHandler) app.HashString;
            jsCommon.hashFile = (JavascriptMethodEventHandler) app.HashFile;
            ((IDisposable)window).Dispose();
            ((IDisposable)jsCommon).Dispose();
        }
    }
}
