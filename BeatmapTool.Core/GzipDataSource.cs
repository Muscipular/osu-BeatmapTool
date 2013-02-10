using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Awesomium.Core.Data;

namespace BeatmapTool.Core
{
    public class GzipDataSource : AsyncDataSource
    {
        public string FilePath { get; private set; }

        private struct FileSize
        {
            public int Position { get; set; }
            public int Length { get; set; }
            public bool IsGziped { get; set; }
        }

        private readonly Dictionary<string, FileSize> _dataPak = new Dictionary<string, FileSize>();

        public GzipDataSource(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return;
            }
            FilePath = filePath;
            var fs = OpenFile();
            byte[] buffer = new byte[256];
            fs.Read(buffer, 0, 4);
            fs.Seek(BitConverter.ToInt32(buffer, 0), SeekOrigin.Begin);
            fs.Read(buffer, 0, 4);
            for (int i = BitConverter.ToInt32(buffer, 0); i > 0; i--)
            {
                fs.Read(buffer, 0, 4);
                var len = BitConverter.ToInt32(buffer, 0);
                if (buffer.Length < len)
                {
                    buffer = buffer.Length * 2 < len ? new byte[len] : new byte[buffer.Length * 2];
                }
                fs.Read(buffer, 0, len);
                var path = Encoding.UTF8.GetString(buffer, 0, len);
                fs.Read(buffer, 0, 4);
                int position = BitConverter.ToInt32(buffer, 0);
                fs.Read(buffer, 0, 4);
                int length = BitConverter.ToInt32(buffer, 0);
                var isGziped = IsGziped(path);
                _dataPak.Add(path, new FileSize { Position = position, Length = length, IsGziped = isGziped });
            }
        }

        private bool IsGziped(string path)
        {
            bool isGziped = false;
            switch ((Path.GetExtension(path) ?? "").Replace(".", "").ToLower())
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

        private FileStream OpenFile()
        {
            return File.Open(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        protected override void LoadResourceAsync(DataSourceRequest request)
        {
            var path = request.Path.ToLower();
            if (!(_dataPak.ContainsKey(path)))
            {
                SendResponse(request, new DataSourceResponse { Buffer = IntPtr.Zero, MimeType = request.MimeType, Size = 0 });
                return;
            }
            var fileInfo = _dataPak[path];
            var ms = new MemoryStream();
            using (var source = new MemoryStream())
            {
                byte[] buffer = new byte[4096];
                using (var fs = OpenFile())
                {
                    fs.Seek(fileInfo.Position, SeekOrigin.Begin);
                    int count, max = fileInfo.Length;
                    while ((count = fs.Read(buffer, 0, Math.Min(4096, max))) > 0)
                    {
                        source.Write(buffer, 0, count);
                        max -= count;
                        if (max == 0)
                        {
                            break;
                        }
                    }
                }
                source.Seek(0, SeekOrigin.Begin);
                using (var gzip = fileInfo.IsGziped ? (Stream)new GZipStream(source, CompressionMode.Decompress) : new BufferedStream(source))
                {
                    int count;
                    while ((count = gzip.Read(buffer, 0, 4096)) > 0)
                    {
                        ms.Write(buffer, 0, count);
                    }
                    ms.Flush();
                }
            }

            var data = ms.ToArray();
            ms.Dispose();
            unsafe
            {
                fixed (byte* pdata = data)
                {
                    IntPtr ptr = new IntPtr(pdata);
                    DataSourceResponse response = new DataSourceResponse
                    {
                        Buffer = ptr,
                        Size = (uint)data.Length,
                        MimeType = request.MimeType
                    };
                    SendResponse(request, response);
                }
            }
        }
    }
}
