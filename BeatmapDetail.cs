using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace BeatmapTool
{
    //[Serializable]
    public class BeatmapDetail
    {
        private string hash;
        private string difficulty;
        private string tags;
        private string bg;
        private string audio;

        public string Hash { get { return hash; } }
        public string Difficulty { get { return difficulty; } }
        public string Tags { get { return tags; } }
        public string BG { get { return bg; } }
        public string Audio { get { return audio; } }

        public BeatmapDetail(string path, string[] info = null)
        {
            tags = string.Empty;
            StreamReader sr = new StreamReader(path);
            do
            {
                string str = sr.ReadLine();
                if (str == "[General]")
                {
                    audio = Path.GetFileName(sr.ReadLine().Substring(15));
                    continue;
                }
                if (str == "[Metadata]")
                {
                    if (info != null)
                    {
                        info[0] = sr.ReadLine().Substring(6);
                        info[1] = sr.ReadLine().Substring(7);
                        info[2] = sr.ReadLine().Substring(8);
                    }
                    else
                    {
                        sr.ReadLine();
                        sr.ReadLine();
                        sr.ReadLine();
                    }
                    difficulty = sr.ReadLine().Substring(8);
                    while (!string.IsNullOrWhiteSpace((str = sr.ReadLine().Trim())))
                    {
                        if (info != null && str[0] == 'S') info[3] = str.Substring(7);
                        if (str[0] == 'T') tags = str.Substring(5);
                    }
                    continue;
                }
                if (str == "[Events]")
                {
                    string tmp = string.Empty;
                    do
                    {
                        tmp = sr.ReadLine();
                        if (string.IsNullOrWhiteSpace(tmp.Trim()))
                        {
                            if (bg == null)
                            {
                                bg = string.Empty;
                                sr.Close();
                                string osb = path.Substring(0, (path.LastIndexOf(" [") > -1) ? path.LastIndexOf(" [") : path.Length - 4) + ".osb";
                                if (File.Exists(osb))
                                {
                                    sr = new StreamReader(osb);
                                    while (!sr.EndOfStream)
                                    {
                                        tmp = sr.ReadLine();
                                        if (tmp == "[Events]")
                                            break;
                                    }
                                    if (!sr.EndOfStream)
                                    {
                                        tmp = sr.ReadLine();
                                        continue;
                                    }
                                }
                            }
                            sr.Close();
                            sr = new StreamReader(path);
                            hash = Core.GetHash(sr.BaseStream);
                            sr.Close();
                            return;
                        }
                    } while (tmp.Substring(0, 5) != "0,0,\"" && !sr.EndOfStream);
                    if (tmp.Substring(0, 5) == "0,0,\"")
                        tmp = tmp.Substring(5, tmp.Length - 6);
                    else tmp = string.Empty;
                    bg = tmp;
                    break;
                }
            } while (!sr.EndOfStream);
            sr.Close();
            sr = new StreamReader(path);
            hash = Core.GetHash(sr.BaseStream);
            sr.Close();
        }/*
        public BeatmapDetail(string path)
        {
            tags = "";            
            StreamReader sr = new StreamReader(path);
            do
            {
                string str = sr.ReadLine();
                if (str == "[General]")
                {
                    audio = Path.GetFileName(sr.ReadLine().Substring(15));
                    continue;
                }
                if (str == "[Metadata]")
                {
                    sr.ReadLine();
                    sr.ReadLine();
                    sr.ReadLine();
                    difficulty = sr.ReadLine().Substring(8);
                    while (!string.IsNullOrWhiteSpace((str = sr.ReadLine().Trim())))
                    {
                        if (str[0] == 'T') tags = str.Substring(5);
                    }
                    continue;
                }
                if (str == "[Events]")
                {
                    string tmp="";
                    do
                    {
                        tmp = sr.ReadLine();
                        if (string.IsNullOrWhiteSpace(tmp.Trim()))
                        {
                            if (bg == null)
                            {
                                bg = "";
                                sr.Close();
                                string osb = path.Substring(0, (path.LastIndexOf(" [") > -1) ? path.LastIndexOf(" [") : 0) + ".osb";
                                if (File.Exists(osb))
                                {
                                    sr = new StreamReader(osb);
                                    while (!sr.EndOfStream)
                                    {
                                        tmp = sr.ReadLine();
                                        if (tmp == "[Events]")
                                            break;
                                    }
                                    if (!sr.EndOfStream)
                                    {
                                        tmp = sr.ReadLine();
                                        continue;
                                    }
                                }
                            }
                            sr = new StreamReader(path);
                            hash = Core.GetHash(sr.BaseStream);
                            sr.Close();
                            return;
                        }
                    } while (tmp.Substring(0, 5) != "0,0,\"" && !sr.EndOfStream);
                    if (tmp.Substring(0, 5) == "0,0,\"")
                        tmp = tmp.Substring(5, tmp.Length - 6);
                    else tmp = "";
                    bg = tmp;
                    break;
                }
            } while (!sr.EndOfStream);
            sr.Close();
            sr = new StreamReader(path);
            hash = Core.GetHash(sr.BaseStream);
            sr.Close();
        }*/

        public override string ToString()
        {
            return hash;
        }
    }
}
