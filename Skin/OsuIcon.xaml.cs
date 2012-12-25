using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace BeatmapTool
{
	/// <summary>
	/// OsuIcon.xaml 的交互逻辑
	/// </summary>
	public partial class OsuIcon : UserControl
    {
        Storyboard Storyboard1;
		public OsuIcon()
		{
			this.InitializeComponent();
            this.Loaded += new RoutedEventHandler(OsuIcon_Loaded);
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.Fant);
            Storyboard1 = TryFindResource("Storyboard1") as Storyboard;
		}

        private void OsuIcon_Loaded(object sender, RoutedEventArgs e)
        {
            //BeginAnimetion();
        }

        public void StopAnimetion()
        {
            if (Storyboard1 == null)
                return;
            Storyboard1.Stop();
        }

        public void BeginAnimetion()
        {
            if (Storyboard1 == null)
                return;
            Storyboard1.Begin();
        }
	}
}