using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeatmapTool
{
    public class EventArgs<T> : EventArgs
    {
        public T Args { get; protected set; }
        public EventArgs(T args)
        {
            Args = args;
        }
    }
}
