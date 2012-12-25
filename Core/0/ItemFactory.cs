using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BeatmapTool.Core.Base;
using BeatmapTool.DataAccess;

namespace BeatmapTool.Core
{
    public class ItemFactory
    {
        public static CoreBase CreateFromDataBlock(DataBlock data)
        {
            CoreBase item = null;
            switch (data.Type)
            {
                case DataType.Diff:
                    return new Diff(data);
                case DataType.Local:
                    break;
                case DataType.Ranked:
                    break;
                case DataType.Replay:
                    break;
                case DataType.Setting:
                    Setting.Setting.Add("ID", data.ReadString());
                    Setting.Setting.Add("PK", data.ReadLong().ToString());
                    return new Setting();
            }
            return item;
        }

    }
}
