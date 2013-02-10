using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace GzipPacker
{
    class Program
    {
        private const int BufferSize = 4096;

        static int Main(string[] args)
        {
            if (args.Length != 3)
            {
                return 1;
            }
            var pak = args[0];
            var input = args[1];
            var output = args[2];

            if (string.IsNullOrEmpty(pak))
            {
                return 1;
            }
            if (string.IsNullOrEmpty(input))
            {
                return 1;
            }
            if (string.IsNullOrEmpty(output))
            {
                return 1;
            }
            if (!Directory.Exists(output))
            {
                return 2;
            }
            if (!Directory.Exists(input))
            {
                return 3;
            }
            try
            {
                if (!input.EndsWith("\\"))
                {
                    input = input + "\\";
                }
                FileStream fs = File.OpenWrite(output + (output.EndsWith("\\") ? "" : "\\") + pak);
                fs.Write(new byte[4], 0, 4);
                List<Tuple<string, int, int>> fileInfo = new List<Tuple<string, int, int>>();
                PackDir(fileInfo, fs, input, input);
                int position = (int)fs.Position;
                fs.Write(BitConverter.GetBytes(fileInfo.Count), 0, 4);
                foreach (var info in fileInfo)
                {
                    var buffer = Encoding.UTF8.GetBytes(info.Item1);
                    fs.Write(BitConverter.GetBytes(buffer.Length), 0, 4);
                    fs.Write(buffer, 0, buffer.Length);
                    fs.Write(BitConverter.GetBytes(info.Item2), 0, 4);
                    fs.Write(BitConverter.GetBytes(info.Item3), 0, 4);
                }
                fs.Seek(0, SeekOrigin.Begin);
                fs.Write(BitConverter.GetBytes(position), 0, 4);
                fs.Flush();
                fs.Dispose();
            }
            catch (Exception)
            {
                return -1;
            }
            return 0;
        }

        private static void PackDir(List<Tuple<string, int, int>> fileInfo, Stream stream, string dir, string root)
        {
            foreach (var entry in Directory.GetDirectories(dir))
            {
                PackDir(fileInfo, stream, entry, root);
            }
            foreach (var file in Directory.GetFiles(dir))
            {
                PackFile(fileInfo, stream, file, root);
            }
        }

        private static void PackFile(List<Tuple<string, int, int>> fileInfo, Stream stream, string file, string root)
        {
            int position = (int)stream.Position;
            int length;
            var isGziped = IsGziped(file);
            using (var fs = File.OpenRead(file))
            {
                length = isGziped ? Gzip(stream, fs) : NonGzip(stream, fs);
            }
            var depPath = file.Substring(root.Length).ToLower().Replace('\\', '/');
            fileInfo.Add(new Tuple<string, int, int>(depPath, position, length));
        }

        private static bool IsGziped(string file)
        {
            bool isGziped = false;
            switch ((Path.GetExtension(file) ?? "").Replace(".", "").ToLower())
            {
                case "html":
                case "json":
                case "htm":
                case "js":
                case "text":
                case "txt":
                case "css":
                    isGziped = true;
                    break;
            }
            return isGziped;
        }

        private static int NonGzip(Stream stream, FileStream fs)
        {
            int length = 0;
            int count;
            byte[] buffer = new byte[BufferSize];
            while ((count = fs.Read(buffer, 0, BufferSize)) > 0)
            {
                length += count;
                stream.Write(buffer, 0, count);
            }
            stream.Flush();
            return length;
        }

        private static int Gzip(Stream stream, FileStream fs)
        {
            int length = 0;
            using (var ms = new MemoryStream())
            {
                byte[] buffer = new byte[BufferSize];
                int count;
                using (var gzip = new GZipStream(ms, CompressionMode.Compress, true))
                {
                    while ((count = fs.Read(buffer, 0, BufferSize)) > 0)
                    {
                        gzip.Write(buffer, 0, count);
                    }
                    gzip.Flush();
                }
                ms.Seek(0, SeekOrigin.Begin);
                while ((count = ms.Read(buffer, 0, BufferSize)) > 0)
                {
                    length += count;
                    stream.Write(buffer, 0, count);
                }
                stream.Flush();
            }
            return length;
        }
    }
}
