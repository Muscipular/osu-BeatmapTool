using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BeatmapTool.Core.Helpers
{
    public class CommonHelper
    {
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
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] hash = md5.ComputeHash(stream);
            StringBuilder Hash = new StringBuilder();
            foreach (byte b in hash)
            {
                Hash.AppendFormat("{0:X2}", b);
            }
            md5.Dispose();
            return Hash.ToString();
        }

        public static string GetHash(string text)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(text));
            StringBuilder Hash = new StringBuilder();
            foreach (byte b in hash)
            {
                Hash.AppendFormat("{0:X2}", b);
            }
            md5.Dispose();
            return Hash.ToString();
        }

    }
}
