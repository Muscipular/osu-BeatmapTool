﻿using System.Windows.Input;
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
            webControl.ProcessCreated += OnWebLoad;
            webControl.SelectionChanged += (sender, args) =>
            {
                if (webControl.FocusedElementType != FocusedElementType.EditableContent
                    && webControl.FocusedElementType != FocusedElementType.TextInput)
                {
                    
                }
            };
            webControl.PreviewKeyDown += (sender, args) => args.Handled = webControl.FocusedElementType != FocusedElementType.EditableContent
                && webControl.FocusedElementType != FocusedElementType.TextInput;
            webControl.ShowContextMenu += (sender, args) => args.Handled = !args.Info.IsEditable;
            webControl.Source = new Uri("asset://local/web/index.html");
        }

        private void OnWebLoad(object sender, EventArgs e)
        {
            dynamic jsCommon = (JSObject)webControl.CreateGlobalJavascriptObject("$APP");
            dynamic window = (JSObject)webControl.CreateGlobalJavascriptObject("$APP_Window");
            jsCommon.window = new JSValue(window);
            window.width = (JavascriptMethodEventHandler)WindowWidth;
            window.height = (JavascriptMethodEventHandler)WindowHeight;
            window.drag = (JavascriptMethodEventHandler)WindowDrag;
            jsCommon.close = (JavascriptMethodEventHandler)AppClose;
            ((IDisposable)window).Dispose();
            ((IDisposable)jsCommon).Dispose();
        }

        private void WindowDrag(object sender, JavascriptMethodEventArgs e)
        {
            e.Result = JSValue.Undefined;
            //            if (Mouse.LeftButton == MouseButtonState.Pressed)
            //            {
            //                MouseEventHandler mouseEventHandler = (o, args) =>
            //                {
            //                    this.OnMouseMove(args);
            //                };
            //                Mouse.AddPreviewMouseMoveHandler(webControl, mouseEventHandler);
            //                this.DragMove();
            //            }
        }

        private void AppClose(object sender, JavascriptMethodEventArgs e)
        {
            e.Result = JSValue.Undefined;
            this.Close();
        }

        private void WindowHeight(object sender, JavascriptMethodEventArgs e)
        {
            e.Result = new JSValue(this.Height);
            if (e.Arguments.Length == 0)
            {
                return;
            }
            if (e.Arguments[0].IsNumber)
            {
                e.Result = new JSValue(this.Height = (float)e.Arguments[0]);
            }
        }

        private void WindowWidth(object sender, JavascriptMethodEventArgs e)
        {
            e.Result = new JSValue(this.Width);
            if (e.Arguments.Length == 0)
            {
                return;
            }
            if (e.Arguments[0].IsNumber)
            {
                e.Result = new JSValue(this.Width = (float)e.Arguments[0]);
            }
        }
    }
}