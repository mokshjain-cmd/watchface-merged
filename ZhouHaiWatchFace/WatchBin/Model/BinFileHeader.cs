using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace WatchBin.Model
{
    
    public class BinFileHeader : Bin
    {

        public uint MagicWord { get; set; } = 0x1234A55A;
        public Version? Version { get; set; }

        public byte ColorGroupCount { get; set; }

        /// <summary>
        /// 无用
        /// </summary>
        public byte[] Reserved { get; set; } = new byte[3];

        public byte ThemeCount { get; set; }

        public byte ColorCount { get; set; }

        public ushort Flags { get; set; }

        public uint PreviewImgDataAddr { get; set; }

        public uint Reserved1 { get; set; }

        public byte[]? Id { get; set; }

        public byte[]? Name { get; set; }


        public byte[] GetBin()
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(this.MagicWord));
            bytes.AddRange(Version.GetBin());
            bytes.Add(ColorGroupCount);
            bytes.AddRange(Reserved);
            bytes.Add(ThemeCount);
            bytes.Add(ColorCount);
            bytes.AddRange(BitConverter.GetBytes(this.Flags));
            bytes.AddRange(BitConverter.GetBytes(this.PreviewImgDataAddr));
            bytes.AddRange(BitConverter.GetBytes(this.Reserved1));
            bytes.AddRange(Id);
            bytes.AddRange(Name);
            return bytes.ToArray();
        }
        public void SetFlags(bool isAlbum = false, bool isAod = false, bool isSku = false, int powerLevel = 0)
        {
            ushort flag = 0;//置0
            flag |= (ushort)(isAlbum?1:0);
            flag |= (ushort)((isAlbum ? 1 : 0) << 1);
            flag |= (ushort)((isAod ? 1 : 0) << 2);
            flag |= (ushort)((isAod ? 1 : 0) << 3);
            flag |= (ushort)((isSku ? 1 : 0) << 4);
            // 设置新的功耗等级
            flag |= (ushort)(powerLevel << 5);  // 设置新值
            this.Flags = flag;
        }

    }

    public class Version : Bin
    {
        public uint Watchface { get; }
        public uint Editor { get; }
        public uint Generator { get; }
        public uint Protocol { get; }
        public uint Firmware { get; }
        //watchface='1.0.0', editor='0.0.0', generator='0.0.0', protocol='0.8.0', firmware='1.0.0'
        public Version(string watchface = "1.0.0", string editor = "0.0.0", string generator = "0.0.0", string protocol = "0.8.0", string firmware = "1.0.0")
        {
            Watchface = ConvertToHex(watchface);
            Editor = ConvertToHex(editor);
            Generator = ConvertToHex(generator);
            Protocol = ConvertToHex(protocol);
            Firmware = ConvertToHex(firmware);
        }

        private uint ConvertToHex(string versionStr)
        {
            string[] parts = versionStr.Split('.');
            if (parts.Length != 3)
            {
                throw new ArgumentException("Version string must be in the format 'major.minor.patch'");
            }

            int major = int.Parse(parts[0]);
            int minor = int.Parse(parts[1]);
            int patch = int.Parse(parts[2]);

            if (major > 0xFFFF || minor > 0xFF || patch > 0xFF)
            {
                throw new ArgumentException("Version components out of range");
            }

            return (uint)(major << 16 | minor << 8 | patch);
        }

        public override string ToString()
        {
            return $"Version(watchface=0x{Watchface:X8}, editor=0x{Editor:X8}, generator=0x{Generator:X8}, protocol=0x{Protocol:X8}, firmware=0x{Firmware:X8})";
        }

        public byte[] GetBin()
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(this.Watchface));
            bytes.AddRange(BitConverter.GetBytes(this.Editor));
            bytes.AddRange(BitConverter.GetBytes(this.Generator));
            bytes.AddRange(BitConverter.GetBytes(this.Protocol));
            bytes.AddRange(BitConverter.GetBytes(this.Firmware));
            return bytes.ToArray();
        }
    }

   

    

   

}
