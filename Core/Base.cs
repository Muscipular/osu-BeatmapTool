using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace BeatmapTool.Core
{
    public abstract class Base
    {
        private static ConcurrentDictionary<WeakReference, byte> _List;
        static Base()
        {
            _List = new ConcurrentDictionary<WeakReference, byte>();
            Task t = new Task(ReleaseObject, TaskCreationOptions.LongRunning);
            t.Start();
        }
        public static bool AutoRelease { get; set; }
        protected abstract void Release();
        private static void ReleaseObject()
        {
            while (true)
            {
                Thread.Sleep(10000);
                foreach (var a in _List.Keys.ToArray())
                {
                    if (!AutoRelease)
                        break;
                    if ((_List[a] >>= 1) == 0)
                    {
                        byte o;
                        if (!AutoRelease)
                            break;
                        _List.TryRemove(a, out o);
                        if (a.IsAlive)
                        {
                            var ox = a.Target as Base;
                            if (ox != null)
                                ox.Release();
                        }
                    }
                }
            }
        }
        protected static void AddOrRefeshObject(Base obj)
        {
            _List.AddOrUpdate(new WeakReference(obj), 0x3f, (a, b) => { return 0x3f; });
        }
    }
}
