using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace BeatmapTool
{
  static  class LangSelector
    {
      public static void ChangeLangENG()
      {
          ResourceDictionary rd = Application.Current.Resources.MergedDictionaries[0];
          rd.Source = new Uri("Lang\\Lang-EN.xaml", UriKind.Relative);
      }
      public static void ChangeLangCHN()
      {
          ResourceDictionary rd = Application.Current.Resources.MergedDictionaries[0];
          rd.Source = new Uri("Lang\\Lang-CN.xaml", UriKind.Relative);
      }
    }
}
