using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Awesomium.Core;

namespace BeatmapTool.Core.Bridge
{
    public class Win
    {
        private readonly Window _window;
        public Win(Window window)
        {
            this._window = window;
        }

        public void Alert(object sender, JavascriptMethodEventArgs e)
        {
            try
            {
                string caption = e.Arguments.Length >= 2 ? e.Arguments[0].ToString() : "";
                string message = e.Arguments.Length >= 2 ? e.Arguments[1].ToString() : e.Arguments.Length == 1 ? e.Arguments[0].ToString() : "";
                e.Result = new JSValue((int)(MessageBox.Show(_window, message, caption)));
            }
            catch (Exception)
            {
                e.Result = JSValue.Undefined;
            }
        }

        public void Width(object sender, JavascriptMethodEventArgs e)
        {
            e.Result = new JSValue(_window.Width);
            if (e.Arguments.Length == 0)
            {
                return;
            }
            if (e.Arguments[0].IsNumber)
            {
                e.Result = new JSValue(_window.Width = (float)e.Arguments[0]);
            }
        }

        public void Height(object sender, JavascriptMethodEventArgs e)
        {
            e.Result = new JSValue(_window.Height);
            if (e.Arguments.Length == 0)
            {
                return;
            }
            if (e.Arguments[0].IsNumber)
            {
                e.Result = new JSValue(_window.Height = (float)e.Arguments[0]);
            }
        }
    }
}
