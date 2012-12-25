using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;

namespace BeatmapTool.Core
{
    public sealed class Settings
    {
        static Dictionary<string, string> dic = new Dictionary<string, string>();
        public static Settings Instance { get; private set; }
        public Version Version
        {
            get { return new Version(this["Version"]); }
            private set { this["Version"] = value.ToString(); }
        }
        static Version _version;
        public Version CurrentVersion
        {
            get { return _version; }
        }
        static Settings()
        {
            _version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            Instance = new Settings();
            Instance.Load();
        }
        private Settings()
        {
        }
        public bool Load()
        {
            using (DataAccess.dbEntities db = new DataAccess.dbEntities())
            {
                try
                {
                    var result = from s in db.Setting select s;
                    foreach (var o in result)
                    {
                        this[o.Key] = o.Value;
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool Save()
        {
            using (DataAccess.dbEntities db = new DataAccess.dbEntities())
            {
                using (System.Transactions.TransactionScope trans = new System.Transactions.TransactionScope())
                {
                    db.Connection.Open();
                    try
                    {
                        Dictionary<string, string> d = new Dictionary<string, string>(dic);
                        var result = from s in db.Setting select s;
                        foreach (var o in result)
                        {
                            if (d.ContainsKey(o.Key))
                            {
                                o.Value = d[o.Key];
                                d.Remove(o.Key);
                            }
                        }
                        foreach (var o in d)
                        {
                            var ent = db.Setting.CreateObject();
                            ent.Key = o.Key;
                            ent.Value = o.Value;
                            db.Setting.AddObject(ent);
                        }
                        db.SaveChanges();
                        trans.Complete();
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
        }
        public string this[string key]
        {
            get
            {
                if (dic.ContainsKey(key))
                    return dic[key];
                return null;
            }
            set
            {
                if (dic.ContainsKey(key))
                    dic[key] = value;
                else
                    dic.Add(key, value);
            }
        }
    }
}
