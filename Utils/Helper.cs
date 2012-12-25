using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace BeatmapTool.Utils
{
    public class Helper
    {
        static MD5CryptoServiceProvider _MD5Provider = new MD5CryptoServiceProvider();
        public static string GetHash(Stream stream)
        {
            byte[] hash = _MD5Provider.ComputeHash(stream);
            StringBuilder Hash = new StringBuilder();
            foreach (byte b in hash)
            {
                Hash.AppendFormat("{0:X2}", b);
            }
            return Hash.ToString();
        }
    }
}
namespace BeatmapTool
{
    public static class WindowHelper
    {
        public static bool IsGlassEnabled(this System.Windows.Window window)
        {
            return Microsoft.Windows.Shell.SystemParameters2.Current.IsGlassEnabled;
        }
    }
}