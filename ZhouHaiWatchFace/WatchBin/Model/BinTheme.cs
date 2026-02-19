using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchBin.Model
{
    public class BinTheme : Bin
    {
        public BinTheme(string styleName)
        {
            BinRecordsTable = Enumerable.Range(0, 10).Select(x => new BinRecordsTable()).ToArray();
            StyleName = styleName;

        }
        public uint Background { get; set; }

        public uint PreviewImgDataAddr { get; set; }

        public BinRecordsTable[]? BinRecordsTable { get; set; }

        public byte[] GetBin()
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Background));
            bytes.AddRange(BitConverter.GetBytes(PreviewImgDataAddr));
            bytes.AddRange(BinRecordsTable!.Select(x => x.GetBin()).SelectMany(x => x));
            return bytes.ToArray();
        }


        public string StyleName { get; set; }//附加属性，不用序列化

    }

    public class BinRecordsTable : Bin
    {
        public uint RecordNum { get; set; }   // Number of records
        public uint Address { get; set; }      // Pointer to record_encoded_t

        public byte[] GetBin()
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(RecordNum));
            bytes.AddRange(BitConverter.GetBytes(Address));
            return bytes.ToArray();
        }
    }
}
