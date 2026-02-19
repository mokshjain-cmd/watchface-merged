using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchBasic.Tool;

namespace WatchBasic.ZHPicRSA.Model
{
    /// <summary>
    /// BMP行描述
    /// </summary>
    public class BmpRow
    {
        public BmpRow()
        {
            BmpCompresses = new List<BmpCompress>();
        }
        public List<BmpCompress>? BmpCompresses { get; set; }
    }

    /// <summary>
    /// BMP压缩
    /// </summary>
    public class BmpCompress
    {
        /// <summary>
        /// 个数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Color Value
        /// </summary>
        public int Color { get; set; }

       

        /// <summary>
        /// Color+Count
        /// </summary>
        // public uint CompressVal => Color;

        public byte[] CompressBytes
        {
            get
            {
                List<byte> bytes=new List<byte>();
                if (CommonDefintion.IsColor888) 
                {

                    bytes = Color.GetBytes(3).ToList();

                }
                else 
                {
                    bytes = Color.GetBytes16().ToList();
                }
               
                bytes.Add((byte)Count);
                return bytes.ToArray();
            }

        }
    }
}
