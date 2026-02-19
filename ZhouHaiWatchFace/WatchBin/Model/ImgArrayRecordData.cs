using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchBin.BinAnalyze;
using WatchControlLibrary;
using WatchControlLibrary.Model;

namespace WatchBin.Model
{
    public class ImgArrayRecordData : Bin
    {
        public ImgArrayRecordData()
        {
            Ezips = new List<byte[]>();
            ImageLen = new List<uint>();
        }
        public byte Fmt { get; set; }
        public byte ImgNumber { get; set; }
        public ushort Flags { get; set; }
        public ushort ImgWidth { get; set; }
        public ushort ImgHeight { get; set; }
        public uint DataLen { get; set; }
        public List<uint> ImageLen { get; set; }
        public List<byte[]> Ezips { get; set; }
        public byte[] GetBin()
        {
            var bytes = new List<byte>();
            bytes.Add(Fmt);
            bytes.Add(ImgNumber);
            bytes.AddRange(BitConverter.GetBytes(Flags));
            bytes.AddRange(BitConverter.GetBytes(ImgWidth));
            bytes.AddRange(BitConverter.GetBytes(ImgHeight));
            bytes.AddRange(BitConverter.GetBytes(DataLen));
            if (ImageLen?.Count > 0)
                foreach (var item in ImageLen)
                    bytes.AddRange(BitConverter.GetBytes(item));
            if (Ezips?.Count > 0)
                bytes.AddRange(Ezips.SelectMany(x => x));

            return bytes.ToArray();
        }

        public void SetFlags(int compression, bool recolor = false, bool hasImgInfo = true)
        {

            int temp = recolor ? 1 : 0;
            temp |= (hasImgInfo ? 1 : 0) << 1;
            temp |= (compression) << 2;
            Flags = (ushort)temp;
        }

        public static ImgArrayRecordData GetImgArrayRecordData(string watchName, ImageArray array,int? bit)
        {
            ImgArrayRecordData data = new ImgArrayRecordData();
            data.Fmt = 40;
            data.ImgNumber = (byte)array.Images!.Count;
            data.SetFlags((int)Compression.N65A, hasImgInfo: data.ImgNumber != 0);
            
            // Use current working directory instead of CommonHelper.CurrentPath
            var firstImageSrc = array.Images.FirstOrDefault()!.Src!;
            var src = Path.Combine(Directory.GetCurrentDirectory(), firstImageSrc);
            
            Console.WriteLine($"[DEBUG] Processing ImageArray with {array.Images.Count} images");
            Console.WriteLine($"[DEBUG] First image path: {src}");
            Console.WriteLine($"[DEBUG] File exists: {System.IO.File.Exists(src)}");
            
            if (!System.IO.File.Exists(src))
            {
                throw new FileNotFoundException($"First image file not found: {src}");
            }
            
            var image = Image.FromFile(src);
            data.ImgWidth = (ushort)image.Width;
            data.ImgHeight = (ushort)image.Height;
            
            foreach (var item in array.Images)
            {
                src = Path.Combine(Directory.GetCurrentDirectory(), item.Src!);
                Console.WriteLine($"[DEBUG] Processing image: {src}");
                Console.WriteLine($"[DEBUG] File exists: {System.IO.File.Exists(src)}");
                
                if (!System.IO.File.Exists(src))
                {
                    throw new FileNotFoundException($"Image file not found: {src}");
                }
                
                var bytes = BinCommonHelper.CommandEzip(src, $"{CommonHelper.EzipBin(watchName)}", false, bit);
                data.Ezips.Add(bytes);
                data.ImageLen.Add((uint)bytes.Length);
            }
            data.DataLen = (uint)(data.ImageLen.SelectMany(x => BitConverter.GetBytes(x)).Count() + data.Ezips.SelectMany(x => x).Count());
            return data;
        }
    }
}
