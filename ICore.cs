using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeatmapTool
{
    public interface ICore
    {
        int Version { get; }
        void Run(Dictionary<string, object>[] args);
    }
}
