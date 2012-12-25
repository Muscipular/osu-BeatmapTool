using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace BeatmapTool
{
    public class TextBox : System.Windows.Controls.TextBox
    {
        Object<int> tick = -1;
        Object<int> Tick { get { lock (tick) { return tick; } } }
        public event Action TextChanged_Delay;
        public TextBox()
            : base()
        {
        }

        protected override void OnTextChanged(System.Windows.Controls.TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            if (Tick.Value <= 0)
            {
                Thread timer = new Thread(() =>
                  {
                      Tick.Value = 5;
                      while (Tick.Value > 0) { Thread.Sleep(100); Tick.Value--; }
                      timer = null;
                      Dispatcher.Invoke(new Action(OnTextChanged_Delay));
                  });
                timer.Start();
            }
            Tick.Value = 5;
        }

        protected virtual void OnTextChanged_Delay()
        {
            Action act = TextChanged_Delay;
            if (act != null)
                TextChanged_Delay();
        }
    }
}
