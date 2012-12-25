using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Xml;
using System.Threading;

namespace BeatmapTool
{
    public class Core
    {
        private static String osuPath;
        private static SongList localList = new SongList();
        private static RankedList rankedList = new RankedList();
        private static RankedList objList = new RankedList();
        private static SongList cfList = new SongList();
        private static CookieContainer Cookies = null;
        private static string username = string.Empty, password = string.Empty;
        private static UIBinding _uiBinding = null;
        private static bool debug = false;
        private static MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

        private static readonly string STR_UPDATELIST;
        private static readonly string STR_DOWNLOADLISTFIN;
        private static readonly string STR_DOWNLOADLIST;
        private static readonly string STR_UISTOP;
        private static readonly string STR_STOP;
        private static readonly string STR_REPLACEBGFAILED;
        private static readonly string STR_PATH;
        private static readonly string STR_FILE;
        private static readonly string STR_MSG;
        private static readonly string STR_DELETEFAILED;
        private static readonly string STR_DELSKINFAILED;
        private static readonly string STR_DELSBFAILED;
        private static readonly string STR_DELHITSOUNDFAILED;
        static Core()
        {
            STR_UPDATELIST = "更新Beatmap列表中[{0}/{1}]...";
            STR_DOWNLOADLISTFIN = "下载Beatmap列表完成[100%].";
            STR_UISTOP = "操作中止.";
            STR_DOWNLOADLIST = "下载Beatmap列表中[{0}%]...";
            STR_STOP = "中断操作?";
            STR_REPLACEBGFAILED = "替换背景失败!";
            STR_PATH="目录";
            STR_FILE="文件";
            STR_MSG = "信息";
            STR_DELETEFAILED = "删除失败!";
            STR_DELSKINFAILED = "删除皮肤失败!";
            STR_DELSBFAILED = "删除SB失败!";
            STR_DELHITSOUNDFAILED = "删除音效失败!";
        }

        public static bool Debug
        {
            get { return debug; }
            set { debug = value; }
        }
        public static UIBinding uiBinding {
            get { return _uiBinding; }
            set { _uiBinding = value; }
        }
        public static String OsuPath
        {
            get { return osuPath; }
            set { osuPath = value; }
        }
        public static SongList LocalList
        {
            get { return localList; }
            set { localList = value; }
        }
        public static RankedList RankedList
        {
            get { return rankedList; }
            set { rankedList = value; }
        }
        public static RankedList ObjList
        {
            get { return objList; }
            set { objList = value; }
        }
        public static SongList CfList
        {
            get { return cfList; }
            set { cfList = value; }
        }
        public static String UserName
        {
            get { return username; }
            set { username = value; }
        }
        public static String PassWord
        {
            get { return password; }
            set { password = value; }
        }

        public static void Logout()
        {
            Cookies = null;
        }
        public enum DownLoadState { Success, Failed, LoginFailed, Done, Wait, Error };
        public static bool Login(string username, string password)
        {
            if (username.Length < 3 || password.Length < 6) return false;
            Cookies = new CookieContainer();
            HttpWebRequest req = WebRequest.Create("http://osu.ppy.sh/p/download") as HttpWebRequest;
            string sid = string.Empty;
            //req.CookieContainer = Cookies;
            try
            {
                using (HttpWebResponse res = req.GetResponse() as HttpWebResponse)
                {
                    using (StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.UTF8))
                    {
                        string str = sr.ReadToEnd();
                        Regex reg = new Regex("<input type=\"hidden\" name=\"sid\" value=\"([a-zA-Z0-9]+)\" />");
                        sid = reg.Match(str).Result("$1");
                    }
                    if (string.IsNullOrWhiteSpace(sid))
                    {
                        Cookies = null;
                        return false;
                    }
                }

                req = HttpWebRequest.Create("http://osu.ppy.sh/forum/ucp.php?mode=login") as HttpWebRequest;
                req.CookieContainer = Cookies;
                req.AllowAutoRedirect = true;
                string param = string.Format("username={0}&password={1}&autologin=on&redirect=%2F&sid={2}&login=login",
                    WebUtility.HtmlEncode(username), WebUtility.HtmlEncode(password)
                    //System.Web.HttpUtility.HtmlEncode(username), System.Web.HttpUtility.HtmlEncode(password)
                    , sid);
                byte[] postBytes = Encoding.UTF8.GetBytes(param);
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.ContentLength = postBytes.Length;
                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(postBytes, 0, postBytes.Length);
                }
                using (HttpWebResponse res = req.GetResponse() as HttpWebResponse)
                {
                    if (res.ResponseUri.ToString() == "http://osu.ppy.sh/forum/ucp.php?mode=login")
                        return false;
                    Cookies.Add(res.Cookies);
                }
            }
            catch
            {
                if (debug) throw;
                return false;
            }
            return true;
        }
        public static DownLoadState Download(RankedSong song, string url, ref string path)
        {
            try
            {
                HttpWebRequest req = HttpWebRequest.Create(url) as HttpWebRequest;
                req.AllowAutoRedirect = true;
                if (Cookies == null && !Login(username, password))
                {
                    return DownLoadState.LoginFailed;
                }
                req.CookieContainer = Cookies;
                req.Method = "GET";
                //req.BeginGetResponse
                using (HttpWebResponse res = req.GetResponse() as HttpWebResponse)
                {
                    if (!res.Headers.ToString().ToLower().Contains("application"))
                    {
                        using (StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.UTF8))
                        {
                            string str=sr.ReadToEnd();
                            try
                            {
                                string tstr = "<span id='seconds'>";
                                int a = str.IndexOf(tstr) + tstr.Length;
                                int b = str.IndexOf("</span>", a);
                                if (uiBinding.WaitTime != 0) return DownLoadState.Wait;
                                uiBinding.WaitTime = int.Parse(str.Substring(a, b - a)) + 60;
                                Thread thread = new Thread(new ThreadStart(() =>
                                {
                                    while (uiBinding.WaitTime > 0 && OsuPath != null)
                                    {
                                        System.Threading.Thread.Sleep(1000);
                                        uiBinding.WaitTime--;
                                    }
                                }));
                                thread.IsBackground = true;
                                thread.Start();
                            }
                            catch
                            {
                                if (debug) throw;
                                //uiBinding.ProcName2 = "下载的Beatmap不存在或者其他原因..";
                                return DownLoadState.Error;
                            }
                        }
                        return DownLoadState.Wait;
                    }
                    long totalBytes = res.ContentLength;
                    uiBinding.ProcMax2 = 1000;
                    using (Stream sResp = res.GetResponseStream())
                    {
                        /*if (!Directory.Exists(path.Substring(0, path.LastIndexOf('\\'))))
                            Directory.CreateDirectory(path.Substring(0, path.LastIndexOf('\\')));*/
                        string filename = "";
                        foreach (string key in res.Headers.AllKeys)
                        {
                            if (key.ToLower() == "Content-Disposition".ToLower())
                            {
                                filename = res.Headers[key];
                                Regex x = new Regex("\"(.*?)\"");
                                if (x.IsMatch(filename))
                                    filename = x.Match(filename).Result("$1");
                            }
                        }
                        if (string.IsNullOrWhiteSpace(filename))
                            filename = song.Id + " - " + song.Title + ".osz";
                        path += filename;
                        using (Stream sFile = new FileStream(path, FileMode.Create))
                        {
                            long totalDownloadBytes = 0;
                            byte[] bs = new byte[8192];
                            int size = sResp.Read(bs, 0, bs.Length);
                            while (size > 0)
                            {
                                totalDownloadBytes += size;
                                sFile.Write(bs, 0, size);
                                uiBinding.ProcCur2 = (int)(totalDownloadBytes * 1000 / totalBytes);
                                uiBinding.ProcName2 = "(" + (uiBinding.ProcCur2 / 10.0).ToString("F1") + "％)" + filename;
                                size = sResp.Read(bs, 0, bs.Length);
                            }
                            if (totalBytes != totalDownloadBytes)
                                return DownLoadState.Error;
                        }
                    }
                }
                uiBinding.ProcCur2 = uiBinding.ProcMax2;
                return DownLoadState.Done;
            }
            catch
            {
                //MessageBox.Show(e.ToString());
                if (debug) throw;
                return DownLoadState.Failed;
            }
        }/*
        public static DownLoadState Download(RankedSong song,ref string path,UIBinding uiBinding)
        {
            System.Windows.Forms.WebBrowser wb = new System.Windows.Forms.WebBrowser();
            WebClient wc = new WebClient();
            //wc.BaseAddress = "";
            string[] json = wc.DownloadString("http://www.muscipular.net/osudb.php?id=" + song.Id).Split(new char[] { '"', '{', '(', '}', ')', ':', ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (json.Length < 5) return DownLoadState.Failed;
            
            wb.AllowWebBrowserDrop = false;
            wb.IsWebBrowserContextMenuEnabled = false;
            wb.ScriptErrorsSuppressed = true;
            wb.WebBrowserShortcutsEnabled = false;
            wb.Navigate("http://www.mediafire.com/?" + json[4]);
            new System.Threading.Thread(new System.Threading.ThreadStart(() =>
            {
                System.Threading.Thread.Sleep(12000);
                wb.Invoke(new Action(() =>
                {
                    foreach (System.Windows.Forms.HtmlElement link in wb.Document.Links)
                    {
                        if (link.InnerText == "Click here to start download from MediaFire.." && link.GetAttribute("href").Contains("download") && link.GetAttribute("href").Contains(".osz"))
                        {
                            MessageBox.Show(wb.Document.Cookie);
                            MessageBox.Show(link.GetAttribute("href"));
                            return;
                        }
                    }
                }));
            })).Start();

            return DownLoadState.Done;
        }*/
        public static byte[] Compress(byte[] data)
        {
            byte[] buffer = null;
            using (MemoryStream stream = new MemoryStream())
            {
                using (GZipStream gZipStream = new GZipStream(stream, CompressionMode.Compress))
                {
                    gZipStream.Write(data, 0, data.Length);
                }
                buffer = stream.ToArray();
            }
            return buffer;
        }
        public static byte[] Decompress(byte[] data)
        {
            byte[] buffer = null;
            using (MemoryStream stream = new MemoryStream())
            {
                using (GZipStream gZipStream = new GZipStream(new MemoryStream(data), CompressionMode.Decompress))
                {
                    buffer = new byte[4096];
                    int n;
                    while ((n = gZipStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        stream.Write(buffer, 0, n);
                    }
                }
                buffer = stream.ToArray();
            }
            return buffer;
        }
        public static string GetHash(Stream stream)
        {
            byte[] hash = md5.ComputeHash(stream);
            StringBuilder Hash = new StringBuilder();            
            foreach (byte b in hash)
            {
                Hash.AppendFormat("{0:X2}", b);
            }
            //md5.Dispose();
            return Hash.ToString();
        }
        public static Dictionary<int, List<string>> GetKeys(string key)
        {
            key = key.ToLower();
            string[] splitString = { "&&", "||", "!&&" };
            Dictionary<int, List<string>> keys = new Dictionary<int, List<string>>();
            keys[0] = new List<string>();
            keys[1] = new List<string>();
            keys[2] = new List<string>();
            int start = 0;
            int type = 0;
            for (int i = 0; i < key.Length; i++)
            {

                int tmp = 0;
                int tmp2 = key.Length;
                int j = 0;
                //int type = 0;
                for (; j < 3; j++)
                {
                    tmp = key.IndexOf(splitString[j], i);
                    if (tmp > -1 && tmp < tmp2)
                    {
                        tmp2 = tmp;// +splitString[j].Length;
                        type = j;
                    }
                }
                //type = type;
                if (i == 0)
                {
                    if (type == 1) keys[1].Add(key.Substring(start, tmp2 - start));
                    else keys[0].Add(key.Substring(start, tmp2 - start));
                }
                else keys[type].Add(key.Substring(start, tmp2 - start));
                start = tmp2 + splitString[type].Length;
                i = start - 1;
            }
            return keys;
        }
        public static bool Search(Dictionary<int, List<string>> keys, string source)
        {
            source = source.ToLower();
            bool flag = true;
            foreach (string s in keys[0])
            {
                if (s == "") continue;
                flag = source.Contains(s);
                if (!flag)
                    return flag;
            }
            foreach (string s in keys[1])
            {
                flag = source.Contains(s) || flag;
                if (!flag)
                    return flag;
            }
            foreach (string s in keys[2])
            {
                flag = (!source.Contains(s)) && flag;
                if (!flag)
                    return flag;
            }
            return flag;
        }


        public static void ReleaseAll()
        {
            rankedList.Release();
            localList.Release();
            cfList.Release();
            objList.Release();
        }
        public static void GetObjList()
        {
            while (rankedList.IsLock) { System.Threading.Thread.Sleep(200); }
            objList.Lock();
            rankedList.CloneTo(objList);

            foreach (SongDetail song in localList)
            {
                objList.Remove(song);
            }
            objList.UnLock();
        }
        public static void BuildHTML(SongList list, string path, string title)
        {
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine("<html><head><title>{0}</title><script src='http://ajax.googleapis.com/ajax/libs/jquery/1.2.6/jquery.min.js' type='text/javascript'></script></head><body><ul>", title);
                if (list.Count > 0 && list[0].GetType() == typeof(RankedSong))
                {
                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        SongDetail song = list[i];
                        sw.WriteLine("<li><a href=\"http://osu.ppy.sh/s/{0}\" target=\"_blank\">{1} - {2}</a></li>", song.Id, song.Artist, song.Title);
                    }
                }
                else
                {
                    foreach (LocalSong song in list)
                    {
                        sw.WriteLine("<li><a href=\"file://{0}\" target=\"_blank\">{1} - {2}</a>        官网链接:<a href=\"http://osu.ppy.sh/s/{3}\" target=\"_blank\">传送门</a></li>", song.Path, song.Artist, song.Title, song.Id);
                    }
                }
                sw.WriteLine("</ul>\n</body>\n</html>");
                sw.Flush();
                //BuildXML(list, path);
            }
        }
        public static void BuildHTML2(SongList list, string path, string title)
        {
            using (StreamWriter sw = File.CreateText(path))
            {
                using (StreamReader sr = new StreamReader(new MemoryStream(Decompress(Properties.Resources.t_html))))
                {
                    sw.WriteLine(sr.ReadLine());
                    sr.ReadLine();
                    sw.WriteLine(title);
                    sw.WriteLine(sr.ReadLine());
                    sr.ReadLine();
                    sw.WriteLine(string.Format("'{0}'", Path.GetFileName(path)));
                    sw.WriteLine(sr.ReadLine());
                    sr.ReadLine();
                    sw.WriteLine(title);
                    sw.WriteLine(sr.ReadLine());
                }
                sw.Flush();
                BuildXML(list, path);
            }
        }
        public static void BuildXML(SongList list, string path)
        {
            System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
            xml.AppendChild(xml.CreateNode(XmlNodeType.XmlDeclaration, "", ""));

            XmlElement root = xml.CreateElement("Songs");
            if (list.Count > 0 && list[0].GetType() == typeof(LocalSong)) root.SetAttribute("Type", "0");
            for (int i = list.Count - 1; i >= 0; i--)
            {
                SongDetail song = list[i];
                XmlElement sn = xml.CreateElement("Song");
                if (song.GetType() == typeof(RankedSong))
                {
                    sn.SetAttribute("Id", song.Id.ToString());
                }
                else
                {
                    if (song.Id < 99999999)
                        sn.SetAttribute("Id", song.Id.ToString());
                    else
                        sn.SetAttribute("Id", "0");
                    sn.SetAttribute("Path", ((LocalSong)song).Path);
                }
                sn.SetAttribute("Title", song.Title);
                sn.SetAttribute("Artist", song.Artist);
                sn.SetAttribute("Creator", song.Creator);
                root.AppendChild(sn);
            }
            xml.AppendChild(root);
            xml.Save(path + ".xml");
        }
        public static void CheckLocalList()
        {
            while (cfList.IsLock || localList.IsLock) { System.Threading.Thread.Sleep(200); }
            cfList.Lock();
            localList.Lock();
            cfList.Clear();
            for (int i = 0; i < localList.Count; ++i)
            {
                for (int j = i + 1; j < localList.Count; ++j)
                {
                    if (localList[i].Id == localList[j].Id)
                    {
                        if (localList[i].Date > localList[j].Date)
                        {
                            cfList.Add(localList[j]);
                            localList.RemoveAt(j);
                            j--;
                        }
                        else
                        {
                            cfList.Add(localList[i]);
                            localList.RemoveAt(i);
                            j--;
                        }
                    }
                    else
                        break;
                }
            }
            localList.UnLock();
            cfList.UnLock();
        }
        static int tid = 99999999;
        private static SongDetail getLocalSongDetail(string path)
        {
            try
            {
                int id = 0;
                if (int.TryParse(System.IO.Path.GetFileName(path).Split(new char[] { ' ', '_' })[0], out id))
                    return new LocalSong(id, path);
                else
                {
                    foreach (string file in Directory.GetFiles(path))
                    {
                        if (Path.GetExtension(file).ToLower().Contains("osu"))
                            return new LocalSong(++tid, path);
                    }
                }
                return null;
            }
            catch (System.Threading.ThreadAbortException) { return null; }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return null;
            }
        }
        public static void getLocalSongList()
        {
            try
            {
                while (localList.IsLock) { System.Threading.Thread.Sleep(200); }
                localList.Lock();
                localList.Clear();
                string[] s1 = Directory.GetDirectories(OsuPath + "\\Songs");
                uiBinding.ProcMax += s1.Length;
                foreach (string s in s1)
                {
                    if (Path.GetFileName(s).ToLower() == "failed") continue;
                    SongDetail song = getLocalSongDetail(s);
                    if (song != null && song.Id > 0)
                    {
                        uiBinding.ProcCur++;
                        localList.Add(song);
                        continue;
                    }
                    string[] s2 = Directory.GetDirectories(s);
                    uiBinding.ProcMax += s2.Length;
                    foreach (string ss in s2)
                    {
                        song = getLocalSongDetail(ss);
                        if (song != null && song.Id > 0)
                        {
                            uiBinding.ProcCur++;
                            localList.Add(song);
                        }
                    }
                }
                localList.Sort();
                localList.UnLock();
                CheckLocalList();
            }
            catch (System.Threading.ThreadAbortException) { }
        }


        private static int getIndex(string html)
        {
            //Regex reg = new Regex(@">(\d+)</a>.*?page=2.>Next", RegexOptions.Compiled);
            //string a = reg.Matches(html).Result("$1");
            Regex reg = new Regex("page=\\d+.>(\\d+)</a>", RegexOptions.Compiled);
            MatchCollection a = reg.Matches(html);
            return int.Parse(a[a.Count-1].Result("$1"));
        }
        private static string getHtml(string url)
        {
            try
            {
                string tmp = string.Empty;
                using (WebClient wc = new WebClient())
                {
                    wc.Encoding = Encoding.UTF8;
                    tmp = wc.DownloadString(url);
                }
                return tmp;
            }
            catch (WebException e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
            catch (System.Threading.ThreadAbortException)
            {
                return null;
            }
            catch
            {
                if (debug) throw;
                return null;
            }
        }
        private static Dictionary<string, object> searchKeyWord(string html)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            int a = 0, b = 0, x = 0;
            string[] diff = new string[] { "easy", "normal", "hard", "insane", "hard-t", "insane-t" };
            a = html.IndexOf("expandRow(");/*
            a = html.IndexOf("onclick='load(", a);
            int x =  html.IndexOf("videoicon", a);
            //if (x - a > -1 && x - a < 100) result["video"] = true;
            //else result["video"] = false;
            result["video"] = html.Contains("videoicon");
            b = a + 14;
            a = html.IndexOf(')', a);
            result["id"] = html.Substring(b, a - b);*/
            b = a + 10;
            a = html.IndexOf(')', a);
            result["id"] = html.Substring(b, a - b);
            a = html.IndexOf("onclick='load(", a);
            result["video"] = html.Contains("videoicon");
            a = html.IndexOf(result["id"] + "'>", a);
            b = a + ((string)result["id"]).Length + 2;
            a = html.IndexOf('<', a);
            result["title"] = html.Substring(b, a - b);
            x = html.IndexOf("src'>(", a);
            a = html.IndexOf(")'>", a);
            b = a + 3;
            a = html.IndexOf('<', a);
            result["artist"] = html.Substring(b, a - b);
            if (x - a > -1 && x - a < 100) //result["source"] = "0";
            {
                b = x + 6;
                a = html.IndexOf(')', x);
                result["source"] = html.Substring(b, a - b);
            }
            else result["source"] = "";
            a = html.IndexOf("href", a);
            a = html.IndexOf('>', a);
            b = a + 1;
            a = html.IndexOf('<', a);
            result["creator"] = html.Substring(b, a - b);
            int count = 0;
            foreach (string dif in diff)
            {
                do
                {
                    x = html.IndexOf(dif, a);
                    if (x > 0)
                    {
                        a = x + dif.Length;
                        count++;
                    }
                    else break;
                } while (true);
            }
            result["diffCount"] = count;
            a = html.IndexOf(")'>", a);
            b = a + 3;
            a = html.IndexOf("<", a);

            if (a == b) result["date"] = DateTime.MinValue;
            else result["date"] = DateTime.Parse(html.Substring(b, a - b));

            a = html.IndexOf("</center>", a);
            a = html.IndexOf("</center>", a);
            x = html.IndexOf("tick.png", a);
            if (x > 0)
            {
                a = html.IndexOf("<br />", a);
                b = a + 6;
                a = html.IndexOf("</a>", a);
                result["pack"] = html.Substring(b, a - b);
            }
            else result["pack"] = "";
            a = html.IndexOf("href", a);
            a = html.IndexOf('>', a);
            b = a + 1;
            a = html.IndexOf('<', a);
            result["genre"] = html.Substring(b, a - b);
            a = html.IndexOf("href", a);
            a = html.IndexOf('>', a);
            b = a + 1;
            a = html.IndexOf('<', a);
            result["language"] = html.Substring(b, a - b);
            return result;
        }

        private static Dictionary<string, object>[] searchKeyWords(string html)
        {
            Dictionary<string, object>[] result = new Dictionary<string, object>[20];
            result.Initialize();
            int a, b;
            a = html.IndexOf("beatmapListing");
            a = html.IndexOf("/tr>", a) + 4;
            for (int i = 0; i < 20; i++)
            {
                try
                {
                    a = html.IndexOf("<tr", a);
                    b = a + 4;
                    a = html.IndexOf("</tr>", a);
                    result[i] = searchKeyWord(html.Substring(b, a - b));
                    a = html.IndexOf("<tr", a);
                    a = html.IndexOf("</tr>", a);
                }
                catch
                {
                    return result;
                }
            }
            return result;
        }
        private static SongDetail[] getSongsDetails(string html, int[] last = null, bool is_app = false)
        {
            SongDetail[] songs = new RankedSong[20];
            Dictionary<string, object>[] KeyWords = searchKeyWords(html);
            for (int i = 0, a = is_app ? 3 : 0; i < 20; i++)
            {
                int id = 0;
                try
                {
                    id = int.Parse((string)KeyWords[i]["id"]);
                    if (last != null && i < 3)
                    {
                        if (((RankedList)rankedList).Last[i + a] == 0) ((RankedList)rankedList).Last[i + a] = -1 * id;
                        else last[i + a] = id;
                    }
                    switch (id)
                    {
                        case 104: KeyWords[i]["title"] = "パリジョナ大作戦"; break;
                        case 106: KeyWords[i]["title"] = "安心感"; break;
                        case 93: KeyWords[i]["title"] = "Shichiten Hakki ☆ Shijou Shugi"; break;
                    }
                }
                catch (System.Threading.ThreadAbortException) { }
                catch
                {
                    if (debug) throw;
                    return songs;
                }
                songs[i] = new RankedSong(id, (string)KeyWords[i]["title"], (string)KeyWords[i]["artist"], (string)KeyWords[i]["creator"], (DateTime)KeyWords[i]["date"], (string)KeyWords[i]["genre"], (string)KeyWords[i]["language"], (string)KeyWords[i]["pack"], (bool)KeyWords[i]["video"], (string)KeyWords[i]["source"], is_app, (int)KeyWords[i]["diffCount"]);
            }
            return songs;
        }
        private static bool addSongToList(RankedList list, SongDetail[] songs)
        {
            foreach (SongDetail str in songs)
            {
                if (str == null) return false;
                if (list.Last[0] > 0 && list.Last[3] > 0)
                    if (str.Id == list.Last[0] || str.Id == list.Last[1] || str.Id == list.Last[2] ||
                        str.Id == list.Last[3] || str.Id == list.Last[4] || str.Id == list.Last[5])
                        return false;
                if (str.Id != 0)
                    list.Add(str);
            }            
            return true;
        }
        public static void BeatmapListing()
        {
            while (rankedList.IsLock) { System.Threading.Thread.Sleep(200); }
            rankedList.Lock();
            int[] last = new int[6] { 0, 0, 0, 0, 0, 0 };
            string url = "http://osu.ppy.sh/p/beatmaplist?l=1&r=0&q=&g=0&la=0&s=4&o=1&page=";
            string html = getHtml(url + "1");
            if (html == null)
            {
                rankedList.UnLock();
                return;
            }
            int index = getIndex(html);
            uiBinding.ProcMax = index;
            uiBinding.ProcCur = 1;
            uiBinding.SetProcNameByFormat(STR_UPDATELIST);// +uiBinding.ProcCur + "/" + uiBinding.ProcMax + "}";
            if (addSongToList(rankedList, getSongsDetails(html, last, false)))
            {
                for (int i = 2; i <= index; i++)
                {
                    uiBinding.ProcCur = i;
                    uiBinding.SetProcNameByFormat(STR_UPDATELIST);
                    //uiBinding.ProcName = STR_UPDATELIST + uiBinding.ProcCur + "/" + uiBinding.ProcMax + "}";
                    html = getHtml(url + i.ToString());
                    if (html == null)
                    {
                        rankedList.UnLock();
                        return;
                    }
                    if (!addSongToList(rankedList, getSongsDetails(html)))
                    {
                        break;
                    }
                }
            }
            url = "http://osu.ppy.sh/p/beatmaplist?l=1&r=6&q=&g=0&la=0&s=4&o=1&m=0&page=";
            html = getHtml(url + "1");
            if (html == null)
            {
                rankedList.UnLock();
                return;
            }
            index = getIndex(html);
            uiBinding.ProcMax += index;
            uiBinding.ProcCur += 1;
            uiBinding.SetProcNameByFormat(STR_UPDATELIST);
            //uiBinding.ProcName = STR_UPDATELIST + uiBinding.ProcCur + "/" + uiBinding.ProcMax + "}";
            if (addSongToList(rankedList, getSongsDetails(html, last, true)))
            {
                for (int i = 2; i <= index; i++)
                {
                    uiBinding.ProcCur++;
                    uiBinding.SetProcNameByFormat(STR_UPDATELIST);
                    //uiBinding.ProcName = STR_UPDATELIST + uiBinding.ProcCur + "/" + uiBinding.ProcMax + "}";
                    html = getHtml(url + i.ToString());
                    if (html == null)
                    {
                        rankedList.UnLock();
                        return;
                    }
                    if (!addSongToList(rankedList, getSongsDetails(html)))
                    {
                        break;
                    }
                }
            }
            for (int i = 0; i < 6; i++)
            {
                if (rankedList.Last[i] < 0)
                    rankedList.Last[i] *= -1;
                else
                    rankedList.Last[i] = last[i];
            }
            rankedList.Sort();
            rankedList.UnLock();
            rankedList.SaveList(Environment.CurrentDirectory + "\\BeatmapListing.list");
        }

        public static void GetCacheList()
        {
            using (WebClient wc = new WebClient())
            {
                wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
                wc.DownloadDataCompleted += new DownloadDataCompletedEventHandler(wc_DownloadDataCompleted);
                wc.DownloadDataAsync(new Uri("http://www.muscipular.net/ranked.list"));
            }
            return;
        }

        static void wc_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            if (!e.Cancelled && e.Error == null)
            {
                rankedList.LoadList(e.Result);
                Core.RankedList.SaveList(Environment.CurrentDirectory + "\\BeatmapListing.list");
                GetObjList();
            }
            uiBinding.ProcCur = uiBinding.ProcMax;
            if (!e.Cancelled)
            {
                uiBinding.ProcName = STR_DOWNLOADLISTFIN;
            }
            else
                uiBinding.ProcName = STR_UISTOP;
            uiBinding.IsWorking = false;
            (sender as WebClient).Dispose();
        }

        static void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (uiBinding.IsNotWorking)
            {
                (sender as WebClient).CancelAsync();
                return;
            }
            uiBinding.ProcCur = e.ProgressPercentage;
            uiBinding.SetProcNameByFormat(STR_DOWNLOADLIST, uiBinding.ProcCur);
        }

        #region static string[] skinfile1 = new string[]{ //Skin文件
        static string[] skinFile = new string[]{
            "hit",              //note 0-8
            "followpoint",      //note
            "sliderb",
            "spinner",
            "fruit",
            "cursor",
            "approachcircle",
            "reversearrow",
            "star",

            "default",          //other 9-end
            "comboburst",
            "score",
            "count",
            "go",
            "ranking",
            "ready",
            "scorebar",
            "section",          //section
            "selection"   //mod
        };
        #endregion
        
        public static string[] GetMp3Files(LocalSong song)
        {
            List<string> mp3 = new List<string>();
            foreach (BeatmapDetail beatmap in song.Difficulties)
            {
                string tmp = beatmap.Audio;
                if (!mp3.Contains(tmp))
                    mp3.Add(tmp);
            }
            return mp3.ToArray();
        }
        public static List<string> GetBg(LocalSong song)
        {
            List<string> bg = new List<string>();
            foreach (BeatmapDetail beatmap in song.Difficulties)
            {
                string tmp = beatmap.BG;
                if (tmp != "" && !bg.Contains(tmp))
                    bg.Add(tmp);
            }
            return bg;
        }

        public static string GetPathX(LocalSong song)
        {
            return song.Path.Substring(OsuPath.Length+1);
        }
        private static void BackUp(string src, LocalSong song, bool isDir = false)
        {
            string dest = song.Path + "\\backup\\" + Path.GetFileName(src);
            string backupPath = song.Path + "\\backup";
            if (!Directory.Exists(backupPath))
                Directory.CreateDirectory(backupPath);
            if (isDir)
            {
                List<string> bgs = new List<string>();
                foreach (string bg in GetBg(song))
                {
                    if (!bg.Contains("\\") && !bg.Contains("/")) { bgs.Add(bg); continue; }
                    string[] a = bg.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
                    bgs.Add(a[a.Length - 1]);
                }
                MoveDir(src, dest, bgs);
            }
            else if (!File.Exists(dest) && File.Exists(src))
                File.Move(src, dest);
            else File.Delete(src);
        }
        private static bool MoveDir(string src, string dest, List<string> bgs)
        {
            if (!Directory.Exists(src)) return true;
            if (!Directory.Exists(dest)) Directory.CreateDirectory(dest);
            bool f = false;
            foreach (string file in Directory.GetFiles(src))
            {
                bool flag = true;
                foreach (string s in bgs)
                {
                    if (s == Path.GetFileName(file))
                    {
                        flag = false;
                        f = true;
                    }
                }
                if (flag)
                {
                    if (!File.Exists(dest + "\\" + Path.GetFileName(file)) && File.Exists(file))
                        File.Move(file, dest + "\\" + Path.GetFileName(file));
                    else File.Delete(file);
                }
            }
            foreach (string dir in Directory.GetDirectories(src))
            {
                if (MoveDir(dir, dest + "\\" + Path.GetFileName(dir), bgs)) f = true;
            }
            if (!f && Directory.Exists(src)) Directory.Delete(src, true);
            return f;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="song"></param>
        /// <param name="mode">true=删除所有;false=删除Note</param>
        /// <param name="backup"></param>
        public static void RemoveSkin(LocalSong song,bool mode,bool backup)
        {
            int max = 9;
            string[] files = Directory.GetFiles(song.Path);
            if (mode) max = skinFile.Length;
            foreach (string file in files)
            {
                if (Path.GetExtension(file).ToLower() == ".png")
                {
                    string filename = Path.GetFileNameWithoutExtension(file).ToLower();
                    try
                    {
                        for (int i = 0; i < max; i++)
                        {
                            if (filename.Contains(skinFile[i]))
                            {
                                if (backup)
                                    BackUp(file, song);
                                else
                                    File.Delete(file);
                                break;
                            }
                        }
                    }
                    catch (System.Threading.ThreadAbortException) { throw; }
                    catch (Exception e)
                    {
                        if (debug) throw;
                        MessageBox.Show(string.Format("{0}\n{4}: {1}\n{5}:{2}\n{6}:{3}", song.ToString(), song.Path, file, e.Message, STR_PATH, STR_FILE, STR_MSG), STR_DELSKINFAILED);
                        if (MessageBox.Show(STR_STOP, "", MessageBoxButton.YesNo) == MessageBoxResult.Yes) throw new StopException();
                    }
                }
            }
        }
        public static void RemoveSound(LocalSong song,bool backup)
        {
            string[] files = Directory.GetFiles(song.Path);
            string[] mp3 = GetMp3Files(song);
            foreach (string file in files)
            {
                try
                {
                    string ext = Path.GetExtension(file).ToLower();
                    if (ext == ".wav" || ext == ".mp3")
                    {
                        string filename = Path.GetFileNameWithoutExtension(file).ToLower();
                        bool flag = false;
                        foreach (string mp3file in mp3)
                        {
                            if (filename.Contains(Path.GetFileNameWithoutExtension(mp3file).ToLower()))
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (flag) continue;
                        if (backup)
                            BackUp(file, song);
                        else
                            File.Delete(file);
                    }
                }
                catch (System.Threading.ThreadAbortException) { throw; }
                catch (Exception e)
                {
                    if (debug) throw;
                    MessageBox.Show(string.Format("{0}\n{4}: {1}\n{5}:{2}\n{6}:{3}", song.ToString(), song.Path, file, e.Message, STR_PATH, STR_FILE, STR_MSG), STR_DELHITSOUNDFAILED);
                    if (MessageBox.Show(STR_STOP, "", MessageBoxButton.YesNo) == MessageBoxResult.Yes) throw new StopException();
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="song"></param>
        /// <param name="mode">true=包括bg;false=不包括bg</param>
        /// <param name="backup"></param>
        public static void RemoveSb(LocalSong song, bool mode, bool backup)
        {
            string[] files = Directory.GetFiles(song.Path);
            string[] dirs = Directory.GetDirectories(song.Path);
            List<string> bg = new List<string>();
            if (!mode)
            {
                bg = GetBg(song);
            }
            foreach (string file in files)
            {
                try
                {
                    string ext = Path.GetExtension(file).ToLower();
                    if (ext == ".flv" || ext == ".avi")
                    {
                        if (backup)
                            BackUp(file, song);
                        else
                            File.Delete(file);
                    }
                    if (ext == ".png" || ext == ".jpg" || ext == ".bmp")
                    {
                        string filename = Path.GetFileNameWithoutExtension(file).ToLower();
                        bool flag = false;
                        for (int i = 0; i < bg.Count; i++)
                        {
                            if (filename.Contains(Path.GetFileNameWithoutExtension(bg[i]).ToLower()))
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (flag) continue;
                        for (int i = 0; i < skinFile.Length; i++)
                        {
                            if (filename.Contains(skinFile[i]))
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (flag) continue;
                        if (backup)
                            BackUp(file, song);
                        else
                            File.Delete(file);
                    }
                }
                catch (System.Threading.ThreadAbortException) { throw; }
                catch (Exception e)
                {
                    if (debug) throw;
                    MessageBox.Show(string.Format("{0}\n{4}: {1}\n{5}:{2}\n{6}:{3}", song.ToString(), song.Path, file, e.Message, STR_PATH, STR_FILE, STR_MSG), STR_DELSBFAILED);
                    if (MessageBox.Show(STR_STOP, "", MessageBoxButton.YesNo) == MessageBoxResult.Yes) throw new StopException();
                }
            }
            foreach (string dir in dirs)
            {
                try
                {
                    if (Path.GetFileName(dir) == "backup") continue;
                    if (backup)
                        BackUp(dir, song, true);
                    else
                        Directory.Delete(dir, true);
                }
                catch (System.Threading.ThreadAbortException) { throw; }
                catch (Exception e)
                {
                    if (debug) throw;
                    MessageBox.Show(string.Format("{0}\n{4}: {1}\n{5}:{2}\n{6}:{3}", song.ToString(), song.Path, dir, e.Message, STR_PATH, STR_FILE, STR_MSG), STR_DELSBFAILED);
                    if (MessageBox.Show(STR_STOP, "", MessageBoxButton.YesNo) == MessageBoxResult.Yes) throw new StopException();
                }
            }
        }
        public static void RemoveSong(LocalSong song)
        {
            try
            {
                Directory.Delete(song.Path, true);
            }
            catch (System.Threading.ThreadAbortException) { throw; }
            catch (Exception e)
            {
                if (debug) throw;
                MessageBox.Show(string.Format("{0}\n{3}: {1}\n{4}:{2}", song.ToString(), song.Path, e.Message, STR_PATH, STR_MSG), STR_DELETEFAILED);
                if (MessageBox.Show(STR_STOP, "", MessageBoxButton.YesNo) == MessageBoxResult.Yes) throw new StopException();
            }
        }
        public static void ReplaceSongBg(LocalSong song,System.Drawing.Image img,bool backup)
        {
            try
            {
                string[] bgs = GetBg(song).ToArray();
                if (bgs.Length == 0)
                {
                    string[] f = Directory.GetFiles(song.Path);
                    string file = "";
                    foreach (string s in f)
                    {
                        if (Path.GetExtension(s).ToLower() == ".osu")
                        {
                            file = s.Substring(0, (s.LastIndexOf(" [") > -1) ? s.LastIndexOf(" [") : s.Length - 4) + ".osb";
                            break;
                        }
                    }
                    string tmp = "";
                    if (File.Exists(file))
                    {
                        tmp = File.ReadAllText(file);
                        int i = tmp.IndexOf("[Events]");
                        if (i > -1)
                            tmp = tmp.Insert(i + 8, "\r\n0,0,\"Custom_BG.jpg\"\r\n");
                        else
                            tmp = tmp.Insert(0, "[Events]\r\n0,0,\"Custom_BG.jpg\"\r\n");
                    }
                    else
                        tmp = "[Events]\r\n0,0,\"Custom_BG.jpg\"\r\n";
                    using (StreamWriter sw = new StreamWriter(file, false))
                    {
                        sw.Write(tmp);
                        sw.Flush();
                    }
                    song.Reload();
                    bgs = new string[] { "Custom_BG.jpg" };
                }

                foreach (string bg in bgs)
                {
                    if (bg.Contains("/") || bg.Contains("\\"))
                    {
                        string tmp = song.Path + "\\backup\\" + bg;
                        tmp = Path.GetDirectoryName(tmp);
                        if (!Directory.Exists(tmp)) Directory.CreateDirectory(tmp);
                        tmp = song.Path + "\\" + bg;
                        tmp = Path.GetDirectoryName(tmp);
                        if (!Directory.Exists(tmp)) Directory.CreateDirectory(tmp);
                    }
                    if (backup)
                    {
                        if (File.Exists(song.Path + "\\" + bg))
                        {
                            if (!Directory.Exists(song.Path + "\\backup")) Directory.CreateDirectory(song.Path + "\\backup");
                            if (!File.Exists(song.Path + "\\backup\\" + bg) && File.Exists(song.Path + "\\" + bg))
                                File.Move(song.Path + "\\" + bg, song.Path + "\\backup\\" + bg);
                        }
                    }
                    using (FileStream fs = File.Create(song.Path + "\\" + bg))
                    {
                        if (img.Size.Height * img.Size.Width < 10)
                            img.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
                        else
                            img.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
                        fs.Flush();
                    }
                }
            }
            catch (System.Threading.ThreadAbortException) { throw; }
            catch (Exception e)
            {
                if (debug) throw;
                MessageBox.Show(string.Format("{0}\n{3}: {1}\n{4}:{2}", song.ToString(), song.Path, e.Message, STR_PATH, STR_MSG), STR_REPLACEBGFAILED);
                if (MessageBox.Show(STR_STOP, "", MessageBoxButton.YesNo) == MessageBoxResult.Yes) throw new StopException();
            }
        }

        public class StopException : ApplicationException
        {
            public StopException() : base() { }
            public StopException(string message) : base(message) { }
        }
    }
}
