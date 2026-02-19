using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchBin.BinAnalyze;
using WatchControlLibrary;
using WatchControlLibrary.Model;

namespace WatchBin.Model
{
    public class ImgRecordData : Bin
    {
        public byte Fmt { get; set; }             // 图像格式
        public byte Flags { get; set; }               // 标志位
        public ushort Reserved { get; set; }          // 保留字段
        public ushort ImgWidth { get; set; }          // 图像宽度
        public ushort ImgHeight { get; set; }         // 图像高度
        public uint DataLength { get; set; }          // 数据总长度
        public byte[]? EzipData { get; set; }  //直接用Ezip生成的数据

        public string Name { set; get; }
        // public EzipCompressed? EzipCompressed { get; set; }

        public byte[] GetBin()
        {
            var bytes = new List<byte>();
            bytes.Add(Fmt);
            bytes.Add(Flags);
            bytes.AddRange(BitConverter.GetBytes(Reserved));
            bytes.AddRange(BitConverter.GetBytes(ImgWidth));
            bytes.AddRange(BitConverter.GetBytes(ImgHeight));
            bytes.AddRange(BitConverter.GetBytes(DataLength));
            if (EzipData == null) throw new ArgumentNullException(nameof(EzipData));
            bytes.AddRange(EzipData);
            return bytes.ToArray();
        }

        public byte SetFlags(bool recolorEnabled, byte compressionMethod)
        {
            // 确保 compressionMethod 在有效范围内
            if (compressionMethod > 3)
            {
                throw new ArgumentOutOfRangeException(nameof(compressionMethod), "Compression method must be 0-3.");
            }
            // 清除现有的 flags
            var flags = 0;
            // 设置重色启用状态
            if (recolorEnabled)
            {
                flags |= 0x01; // 设置 bit0
            }
            // 设置压缩方法
            flags |= (byte)((compressionMethod & 0x03) << 2); // 设置 bit[4:2]
            return (byte)flags;
        }

        public static ImgRecordData GetImgRecordData(WatchImage image, string watchName, int? bit)
        {
            ImgRecordData img = new ImgRecordData();
            // Use current directory (set to JSON file directory) + relative path
            var src = image.Src!.TrimStart('.', '\\', '/').Replace(@"/", @"\");
            img.Name = image.Name!;
            Image init = Image.FromFile(src);
            img.ImgWidth = (ushort)init.Width;
            img.ImgHeight = (ushort)init.Height;

            if (image.IsPointer) 
            {
                img.Fmt = (byte)(4);//直接用16位
                img.Flags = 0;
                var bytes = BinCommonHelper.CommandPointer16(src); //直接用16位
                img.EzipData = bytes;
                img.DataLength = (uint)bytes.Length;
            }
            else  //527TODO
            {
                img.Fmt = 40;//Png
                img.Flags = 12;
                var bytes = BinCommonHelper.CommandEzip(src, $"{CommonHelper.EzipBin(watchName)}",image.IsPointer, bit);
                img.EzipData = bytes;
                img.DataLength = (uint)bytes.Length;
            }

            return img;
        }

       

    }

    public class EzipCompressed : Bin
    {
        public uint LenOrig { get; set; }                        // 原始数据长度
        public uint AlignSize { get; set; }                       // 对齐大小
        public byte[]? AlignData { get; set; }                      // 对齐数据
        public byte[]? Data { get; set; }                         // ezip 压缩数据

        public byte[] GetBin()
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(LenOrig));
            bytes.AddRange(BitConverter.GetBytes(AlignSize));
            if (AlignData?.Length > 0)
                bytes.AddRange(AlignData);
            if (Data?.Length > 0)
                bytes.AddRange(Data);
            return bytes.ToArray();
        }
    }
}
