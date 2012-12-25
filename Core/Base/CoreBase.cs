using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BeatmapTool.DataAccess;
using System.IO;

namespace BeatmapTool.Core.Base
{
    public abstract class CoreBase
    {
        protected static Dictionary<string, string> __Setting = new Dictionary<string, string>(5);
        protected static LocalList __LocalList = null;
        protected static RankedList __RankedList = null;
        public static LocalList LocalList
        {
            get
            {
                if (__LocalList == null)
                    __LocalList = new LocalList();
                return __LocalList;
            }
            set { __LocalList = value; }
        }
        public static RankedList RankedList
        {
            get
            {
                if (__RankedList == null)
                    __RankedList = new RankedList();
                return __RankedList;
            }
            set { __RankedList = value; }
        }
        public static Dictionary<string, string> Setting
        {
            get { return __Setting; }
        }
        protected CoreBase() { }
        static CoreBase()
        {
            try
            {
                Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\osu!\shell\open\command");
                Setting["Path"] = Path.GetDirectoryName(rk.GetValue("").ToString().Split(new char[] { '"' }, StringSplitOptions.RemoveEmptyEntries)[0]) + "\\";
            }
            catch { Setting["Path"] = ""; }
        }
        public virtual byte[] Save() { throw new NotImplementedException(); }
        //public virtual bool Load(DataBlock data) { throw new NotImplementedException(); }
    }
}
