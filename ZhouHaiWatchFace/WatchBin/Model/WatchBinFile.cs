using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchControlLibrary;

namespace WatchBin.Model
{
    public class WatchBinFile : Bin
    {
        public WatchBinFile()
        {
            BinThemes = new List<BinTheme>();
            RecordBases = new List<RecordBase>();
            RawData = new List<byte>();

        }
        public BinFileHeader? Header { get; set; }

        public List<BinTheme>? BinThemes { get; set; }

        public List<RecordBase>? RecordBases { get; set; }

        public List<byte> RawData { get; set; }

        public int CurrentSize => (Header?.GetBin().Count() ?? 0)
            + (BinThemes?.SelectMany(x => x.GetBin()).Count() ?? 0)
            + (RecordBases?.SelectMany(x => x.GetBin()).Count() ?? 0)
            + RawData.Count;

        public byte[] GetBin()
        {
            var bytes = new List<byte>();
            if (Header == null) throw new ArgumentNullException("header");
            bytes.AddRange(Header.GetBin());
            if (!BinThemes!.Any()) throw new ArgumentNullException("themes");
            bytes.AddRange(BinThemes!.SelectMany(x => x.GetBin()));
            if (!RecordBases!.Any()) throw new ArgumentNullException("recordBases");
            bytes.AddRange(RecordBases!.SelectMany(x => x.GetBin()));
            bytes.AddRange(RawData);
            bytes.AddRange(Enumerable.Range(0, 4 - bytes.Count % 4).Select(x => (byte)0)); //4字节对齐
            if (bytes.Count % 4096 == 0)
            {
                bytes.AddRange(Enumerable.Range(0, 4).Select(x => (byte)0));
            }
            return bytes.ToArray();
        }

        public Dictionary<string, BinCache> Cache = new Dictionary<string, BinCache>();

        public BinCache GetBinCache(byte[] bytes)
        {
            var md5 = MD5Helper.GetMD5Str(bytes);
            if (!Cache.ContainsKey(md5))
            {
                Cache.Add(md5, new BinCache
                {
                    Length = bytes.Length,
                    Addr = CurrentSize,
                });
                RawData.AddRange(bytes);
            }
            return Cache[md5];
        }

    }

    public class BinCache
    {

        public int? Addr { get; set; }

        public int? Length { get; set; }
    }
}
