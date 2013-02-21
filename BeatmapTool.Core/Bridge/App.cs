using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Awesomium.Core;

namespace BeatmapTool.Core.Bridge
{
    public class App
    {
        public void HashString(object sender, JavascriptMethodEventArgs e)
        {
            if (e.Arguments == null || e.Arguments.Length == 0)
            {
                e.Result = JSValue.Null;
                return;
            }
            try
            {
                e.Result = new JSValue(Helpers.CommonHelper.GetHash(e.Arguments[0].ToString()));
            }
            catch (Exception)
            {
                e.Result = JSValue.Null;
            }
        }

        public void HashFile(object sender, JavascriptMethodEventArgs e)
        {
            if (e.Arguments == null || e.Arguments.Length == 0)
            {
                e.Result = JSValue.Null;
                return;
            }
            try
            {
                e.Result = new JSValue(Helpers.CommonHelper.GetHash(File.OpenRead(e.Arguments[0].ToString())));
            }
            catch (Exception)
            {
                e.Result = JSValue.Null;
            }
        }

        public void Exit(object sender, JavascriptMethodEventArgs e)
        {
            e.Result = JSValue.Undefined;
            Application.Current.Shutdown();
        }
    }
}
