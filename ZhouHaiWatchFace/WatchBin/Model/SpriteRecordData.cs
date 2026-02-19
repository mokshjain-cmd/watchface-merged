using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchControlLibrary.Model;

namespace WatchBin.Model
{
    //动画帧
    public class SpriteRecordData : Bin
    {
        public uint UidImageArray { get; set; }
        public byte ImgNum { get; set; }

        public byte Flags { get; set; }

        public ushort DelayMs { get; set; }

        public uint RepeatCount { get; set; }

        public SpriteFrameInfo[]? SpriteFrameInfos { get; set; }

        public byte[] GetBin()
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(UidImageArray));
            bytes.Add(ImgNum);
            bytes.Add(Flags);
            bytes.AddRange(BitConverter.GetBytes(DelayMs));
            bytes.AddRange(BitConverter.GetBytes(RepeatCount));
            //if (SpriteFrameInfos != null) 
            //{
            //    bytes.AddRange(SpriteFrameInfos.SelectMany(x => x.GetBin()));
            //}

            return bytes.ToArray();
        }

        public static SpriteRecordData GetSpriteRecordData(Sprite sprite)
        {
            var data = new SpriteRecordData();
            data.UidImageArray = BinCommonHelper.UidRelationTable[sprite.Ref.Replace("@", "")];
            data.ImgNum = (byte)(sprite.ImageCount ?? 0);
            data.Flags = 0;
            data.DelayMs = (ushort)sprite.Interval;
            data.RepeatCount = (uint)sprite.RepeatCount;
            data.SpriteFrameInfos = sprite.ImageCount == null ? null : Enumerable.Range(0, sprite.ImageCount.Value).Select(x => new SpriteFrameInfo()).ToArray();
            return data;
        }
    }

    public class SpriteFrameInfo : Bin
    {
        //alpha off_x off_y 
        public uint value { get; set; }

        public ushort delay_ms { get; set; }
        public ushort reserved { get; set; }

        public byte[] GetBin()
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(value));
            bytes.AddRange(BitConverter.GetBytes(delay_ms));
            bytes.AddRange(BitConverter.GetBytes(reserved));
            return bytes.ToArray();
        }
    }
}
