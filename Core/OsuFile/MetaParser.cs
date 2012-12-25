using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.IO;
using BeatmapTool.Utils;

namespace BeatmapTool.Core
{
    public static class MetaHelper
    {
        public static Nullable<T> To<T>(this IConvertible IConvertible) where T : struct, IConvertible
        {
            try
            {
                return (T)Convert.ChangeType(IConvertible, typeof(T));
            }
            catch
            {
                return null;
            }
        }
        public static Nullable<T> To<T>(this IConvertible IConvertible, IFormatProvider IFormatProvider) where T : struct, IConvertible
        {
            try
            {
                return (T)Convert.ChangeType(IConvertible, typeof(T), IFormatProvider);
            }
            catch
            {
                return null;
            }
        }
    }
    public class MetaParser
    {
        public readonly static string[] GeneralKeys = 
        {
            "AudioFilename",
            "AudioLeadIn",
            "PreviewTime",
            "Mode"
        };

        public static string[] MetadataKeys = 
        {
            "Title",
            "Artist",
            "Creator",
            "Version",
            "Source",
            "Tags"
        };

        public static string[] DifficultyKeys = 
        {
            "HPDrainRate",
            "CircleSize",
            "OverallDifficulty",
            "ApproachRate",
            "SliderMultiplier",
            "SliderTickRate"
        };

        public static string[] OtherKeys =
        {
            "NodeCount",
            "SliderCount",
            "SpinnerCount",
            "MaxCombo",
            "DrainingTime",
            "Ops",
            "Hash",
            "Background",
            "Bpm"
        };

        private static string[] creator_changedname = { "Echo49", "Echo" };
        private static string[] specialTitle = 
        {
            "DISCO★PRINCE",/*==>*/ "DISCO PRINCE", 
            "Angry Video Game Nerd Theme",/*==>*/ "Angry Video Game Nerd Theme [MATURE CONTENT]" 
        };

        private static string replace(string String, string[] ReplaceList)
        {
            for (int i = 0, max = ReplaceList.Length; i < max; i += 2)
                if (String == ReplaceList[i])
                    String = ReplaceList[i + 1];
            return String;
        }

        public static SortedList<string, string> Parse(string path)
        {
            SortedList<string, string> result = Init();
            Parse(path, result);
            result["Creator"] = replace(result["Creator"], creator_changedname);
            result["Title"] = replace(result["Title"], specialTitle);
            using (Stream sr = File.OpenRead(path))
            {
                result["Hash"] = Helper.GetHash(sr);
            }
            return result;
        }

        private static SortedList<string, string> Init()
        {
            SortedList<string, string> result = new SortedList<string, string>(32);
            foreach (string s in DifficultyKeys)
                result[s] = string.Empty;
            foreach (string s in GeneralKeys)
                result[s] = string.Empty;
            foreach (string s in MetadataKeys)
                result[s] = string.Empty;
            foreach (string s in OtherKeys)
                result[s] = string.Empty;
            result.TrimExcess();
            return result;
        }

        private static void Parse(string path, SortedList<string, string> result)
        {
            Func<string, string[]> ParseFunction = null;
            Func<StreamReader, decimal, decimal, string[]> ParseObject = null;
            StreamReader sr = null;
            sr = File.OpenText(path);
            string tmpStr = null;
            string[] tmpStrArr = null;
            while (true)
            {
                if (sr.EndOfStream)
                    break;
                tmpStr = sr.ReadLine().Trim();
                if (string.IsNullOrWhiteSpace(tmpStr))
                    continue;
                if (tmpStr.Length > 1 && tmpStr[0] == '[')
                {
                    tmpStr = tmpStr.Substring(1, tmpStr.Length - 2);
                    switch (tmpStr)
                    {
                        case "General":
                            ParseFunction = ParseGeneral;
                            continue;
                        case "Metadata":
                            ParseFunction = ParseMetadata;
                            continue;
                        case "Difficulty":
                            ParseFunction = ParseDifficulty;
                            continue;
                        case "Events":
                            ParseFunction = ParseBackGround;
                            continue;
                        case "TimingPoints":
                            ParseFunction = ParseTiming;
                            continue;
                        case "HitObjects":
                            ParseFunction = null;
                            ParseObject = MetaParser.ParseObject;
                            break;
                        default:
                            ParseFunction = null;
                            ParseObject = null;
                            continue;
                    }
                }
                if (tmpStr.Length > 2 && tmpStr[0] == '/' && tmpStr[1] == '/')
                    continue;
                if (ParseFunction != null)
                {
                    tmpStrArr = ParseFunction(tmpStr);
                    if (tmpStrArr == null)
                        continue;
                    if (string.IsNullOrWhiteSpace(result[tmpStrArr[0]]))
                        result[tmpStrArr[0]] = tmpStrArr[1];
                    continue;
                }
                if (ParseObject != null)
                {
                    tmpStrArr = ParseObject(sr, decimal.Parse(result["SliderMultiplier"]), decimal.Parse(result["SliderTickRate"]));
                    if (tmpStrArr == null)
                        continue;
                    result["Node"] = tmpStrArr[0];
                    result["Slider"] = tmpStrArr[1];
                    result["Spinner"] = tmpStrArr[2];
                    result["Combo"] = tmpStrArr[3];
                    result["Draining Time"] = tmpStrArr[4];
                    result["Ops"] = tmpStrArr[5];
                }
            }
            sr.Close();
            sr.Dispose();
        }

        private static string[] ParseString(string str, string[] keys)
        {
            if (string.IsNullOrWhiteSpace(str))
                return null;
            int i = str.IndexOf(':');
            if (i < 0)
                return null;
            string tmp = str.Substring(0, i).Trim();
            string tmp2 = null;
            string tmp3 = str.Substring(i + 1).Trim();
            foreach (string key in keys)
            {
                //"Title","Artist","Creator",
                if (key == tmp)
                {
                    tmp2 = key;
                    break;
                }
            }
            if (tmp2 == null)
                return null;
            return new string[] { tmp2, tmp3 };
        }

        private static string[] ParseBackGround(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return null;
            int i = str.IndexOf("0,0,\"");
            if (i < 0)
                return null;
            return new string[] { "Background", str.Substring(5, str.Length - 6) };
        }

        private static string[] ParseTiming(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return null;
            string[] tmp = str.Split(',');
            decimal i = 0;
            if (!decimal.TryParse(tmp[1], out i))
                return null;
            if (i < 0)
                return null;
            i = 60000 / i;
            return new string[] { "Bpm", i.ToString("n") };
        }

        private static string[] ParseObject(StreamReader sr, decimal SliderMultiplier, decimal SliderTickRate)
        {
            int combo = 0;
            int? beginTime = null;
            int? endTime = null;
            string last = null;
            int node = 0;
            int slider = 0;
            int spinner = 0;
            while (!sr.EndOfStream)
            {
                int itmp = 0;
                decimal dtmp = 0;
                string str = sr.ReadLine();
                if (string.IsNullOrWhiteSpace(str))
                    continue;
                if (str.Length > 2 && str[0] == '/' && str[1] == '/')
                    continue;
                string[] tmp = str.Split(',');
                if (tmp.Length < 5)
                    continue;
                if (!int.TryParse(tmp[3], out itmp))
                    continue;
                HitObjectType type = (HitObjectType)itmp;
                if (type.HasFlag(HitObjectType.Node))
                    node++;
                if (type.HasFlag(HitObjectType.Slider))
                {
                    slider++;
                    itmp = tmp.Length - 1;
                    while (tmp[itmp].Contains('|') || tmp[itmp].Contains(':'))  //定位重复次数
                        itmp--;
                    dtmp = decimal.Parse(tmp[itmp]);    //长度
                    itmp = int.Parse(tmp[itmp - 1]);    //重复次数
                    combo += (1 + (int)decimal.Ceiling(dtmp / (SliderMultiplier * 100) * SliderTickRate - 1)) * itmp + 1;
                }
                if (type.HasFlag(HitObjectType.Spinner))
                    spinner++;
                if (!beginTime.HasValue)
                    beginTime = int.Parse(tmp[2]);
                last = tmp[2];
            }
            if (last != null)
                endTime = int.Parse(last);
            else
            {
                endTime = 0;
                beginTime = 0;
            }
            combo += spinner;
            combo += node;
            decimal draining = (endTime.Value - beginTime.Value) / (decimal)1000;
            if (draining <= 0)
                draining = 1;
            decimal d = (node + slider + spinner) / draining;
            return new string[] { 
                node.ToString(),    //node数量
                slider.ToString(),      //slider数量
                spinner.ToString(),     //转盘数量
                combo.ToString(),   //maxcombo
                draining.ToString("0"),     //持续时间
                d.ToString("n")     //平均密度
            };
        }

        private static string[] ParseGeneral(string str)
        {
            return ParseString(str, GeneralKeys);
        }

        private static string[] ParseMetadata(string str)
        {
            return ParseString(str, MetadataKeys);
        }

        private static string[] ParseDifficulty(string str)
        {
            return ParseString(str, DifficultyKeys);
        }
    }
}
