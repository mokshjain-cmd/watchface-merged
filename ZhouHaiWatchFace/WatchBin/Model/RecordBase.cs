using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchBin.Model
{
    public class RecordBase : Bin
    {
        internal RecordBase()
        {
        }

        public RecordBase(byte type, bool builtin, byte colorGroup, uint id, string name, string styleName)
        {
            Uid = SetUid(type, builtin, colorGroup, id);
            BinCommonHelper.UidRelationTable.Add(name, Uid);
            this.Name = name;
            StyleName = styleName;
        }
        public uint Uid { get; set; }
        public uint Flags { get; set; }
        public uint DataAddress { get; set; }
        public uint DataLength { get; set; }

        public string Name { get; set; }//附加属性，不序列化

        public string StyleName {  get; set; }//附加属性，不序列化





        public uint SetUid(byte type, bool builtin, byte colorGroup, uint id)
        {
            // 确保参数在有效范围内
            if (type > 255) throw new ArgumentOutOfRangeException(nameof(type), "type must be between 0 and 255.");
            if (colorGroup > 15) throw new ArgumentOutOfRangeException(nameof(colorGroup), "colorGroup must be between 0 and 15.");
            if (id > 524287) throw new ArgumentOutOfRangeException(nameof(id), "id must be between 0 and 524287.");
            // 将参数组合成一个 uint
            return (uint)((type & 0xFF) << 24) | (id & 0xFFFFFF);
            // return (uint)(type << 24 | (builtin ? 1 : 0) << 23 | colorGroup << 19 | id);

        }

        public uint GetRecordType() 
        {
            return (Uid >> 24) & 0xFF;
        }

        public uint GetIndex() 
        {
            return Uid & 0x7FFFF; // 0x7FFFF 是 19 位的掩码
        }

        public byte[] GetBin()
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Uid));
            bytes.AddRange(BitConverter.GetBytes(Flags));
            bytes.AddRange(BitConverter.GetBytes(DataAddress));
            bytes.AddRange(BitConverter.GetBytes(DataLength));
            return bytes.ToArray();
        }
    }
}
