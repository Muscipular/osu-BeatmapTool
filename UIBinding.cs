using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.Windows.Threading;

namespace BeatmapTool
{
    public class UIBinding : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private int procCur = 0;
        private int procMax = 100;
        private string procName;
        private bool isWorking = false;
        private bool isReBuild = false;
        private bool isCache = false;
        private bool isDelDu = false;
        private bool isDelRep = false;
        private bool isReplaceBg = false;
        private bool isDelSkin = false;
        private bool isDelSb = false;
        private bool isDelHitSound = false;
        private bool isBackup = true;
        private int procCur2 = 0; 
        private int procMax2 = 100;
        private string procName2 = string.Empty;
        private bool isDownload = false;
        private int waittime = 0;

        public UIBinding()
        {
            Core.uiBinding = this;
            procName = "空闲";
        }

        public string PassWord
        {
            get { return Core.PassWord; ; }
            set
            {
                Core.PassWord = value;
                OnPropertyChanged("PassWord");
            }
        }
        public string UserName
        {
            get { return Core.UserName; }
            set
            {
                Core.UserName = value;
                OnPropertyChanged("UserName");
            }
        }
        public int WaitTime
        {
            get { return waittime; }
            set
            {
                waittime = value;
                OnPropertyChanged("Waittime");
            }
        }
        public bool IsBackup
        {
            get { return isBackup; }
            set
            {
                isBackup = value;
                OnPropertyChanged("IsBackup");
            }
        }
        public bool IsDelHitSound
        {
            get { return isDelHitSound; }
            set
            {
                isDelHitSound = value;
                OnPropertyChanged("IsDelHitSound");
            }
        }
        public bool IsDelSb
        {
            get { return isDelSb; }
            set
            {
                isDelSb = value;
                OnPropertyChanged("IsDelSb");
            }
        }
        public bool IsDelSkin
        {
            get { return isDelSkin; }
            set
            {
                isDelSkin = value;
                OnPropertyChanged("IsDelSkin");
            }
        }
        public bool IsReplaceBg
        {
            get { return isReplaceBg; }
            set
            {
                isReplaceBg = value;
                OnPropertyChanged("IsReplaceBg");
            }
        }        
        public bool IsDelRep
        {
            get { return isDelRep; }
            set
            {
                isDelRep = value;
                OnPropertyChanged("IsDelRep");
            }
        }
        public bool IsDelDu
        {
            get { return isDelDu; }
            set
            {
                isDelDu = value;
                OnPropertyChanged("IsDelDu");
            }
        }
        public bool IsReBuild
        {
            get { return isReBuild; }
            set
            {
                isReBuild = value;
                OnPropertyChanged("IsReBuild");
            }
        }
        public bool IsCache
        {
            get { return isCache; }
            set
            {
                isCache = value;
                IsReBuild = value;
                OnPropertyChanged("IsCache");
                OnPropertyChanged("IsNotCache");
            }
        }
        public bool IsNotCache
        {
            get { return !isCache; }
        }
        public int ProcCur
        {
            get { return procCur; }
            set
            {
                procCur = value;
                OnPropertyChanged("ProcCur");
            }
        }
        public int ProcMax
        {
            get { return procMax; }
            set
            {
                procMax = value;
                OnPropertyChanged("ProcMax");
            }
        }
        public string ProcName
        {
            get { return procName; }
            set 
            {
                procName = value;
                OnPropertyChanged("ProcName");
            }
        }
        public bool IsWorking
        {
            get { return isWorking; }
            set
            {
                isWorking = value;
                OnPropertyChanged("IsWorking");
                OnPropertyChanged("IsNotWorking");
            }
        }
        public bool IsNotWorking
        {
            get { return !isWorking; }
        }
        public int ProcCur2
        {
            get { return procCur2; }
            set
            {
                procCur2 = value;
                OnPropertyChanged("ProcCur2");
            }
        }
        public int ProcMax2
        {
            get { return procMax2; }
            set
            {
                procMax2 = value;
                OnPropertyChanged("ProcMax2");
            }
        }
        public string ProcName2
        {
            get { return procName2; }
            set 
            {
                procName2 = value;
                OnPropertyChanged("ProcName2");
            }
        }
        public bool IsDownload
        {
            get { return isDownload; }
            set
            {
                isDownload = value;
                OnPropertyChanged("IsDownload");
                OnPropertyChanged("IsNotDownload");
            }
        }
        public bool IsNotDownload
        {
            get { return !isDownload; }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public void SetProcNameByFormat(string format)
        {
            ProcName = string.Format(format, procCur, procMax);
            //return procName;
        }

        public void SetProcNameByFormat(string format, params object[] objs)
        {
            ProcName = string.Format(format, objs);
            //return procName;
        }
        /*
        public void SetProcName2ByFormat(string format)
        {
            ProcName2 = string.Format(format, procCur2, procMax2);
            //return procName2;
        }

        public void SetProcName2ByFormat(string format, params object[] objs)
        {
            ProcName2 = string.Format(format, objs);
            //return procName2;
        }*/
    }
    
}
