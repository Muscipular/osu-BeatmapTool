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
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private void Title_Drap(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;
            osuIcon.StopAnimetion();
            //this.Opacity = 0.9;
            this.DragMove();
            //this.Opacity = 1;
            osuIcon.BeginAnimetion();
        }

        private void OsuIcon_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                StartOsu();
            else
                ToString();
            e.Handled = true;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox x = (sender as ComboBox);
            if (x.SelectedIndex == 1)
                LangSelector.ChangeLangCHN();
            if (x.SelectedIndex == 0)
                LangSelector.ChangeLangENG();
        }
    }
}
