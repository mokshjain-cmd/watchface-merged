using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchBasic.WatchBin.Model;

namespace WatchBasic.WatchBin
{
    public class BinImage
    {
        public BinImage()
        {
            Bytes = new List<byte>();
            ValuePairs = new Dictionary<string, BinImageDetail>();
            ColorAddrs = new List<int>();
        }

        public BinImage(int addr)
        {
            Addr = addr;
            OffsetAddr = addr;
            Bytes = new List<byte>();
            ValuePairs = new Dictionary<string, BinImageDetail>();
            ColorAddrs = new List<int>();
            LocationLefts = new List<int>();
            LocationTops = new List<int>();
            GroupCodes = new List<string>();
            DisableAddrs= new List<int>();
        }

        public Dictionary<string, BinImageDetail> ValuePairs { get; set; }

        public List<byte>? Bytes { get; set; }

        public List<int> ColorAddrs { get; set; }

        public List<int> LocationLefts { get; set; }

        public List<int> LocationTops { get; set; }

        public List<int> DisableAddrs { get; set; }    

        public List<string>? GroupCodes { get; set; }


        /// <summary>
        /// 偏移位数
        /// </summary>
        public int? Count { get; set; }=0;

        //public int LastSize { get; set; } = 0;

        /// <summary>
        /// 下标
        /// </summary>
        public int? Addr { get; set; } = 0;

        /// <summary>
        /// 偏移地址
        /// </summary>
        public int OffsetAddr { get; set; }= 0;


        //public Color? Color { get; set; }  
        
        /// <summary>
        /// 用于图片地址
        /// </summary>
        public int CurrentAddr { get; set;} = 0;

        /// <summary>
        /// 4字节对齐获取地址
        /// </summary>
        /// <returns></returns>
        public  int SetImageAddr()
        {
            Addr = Addr + Count;
            while (Addr % 4 != 0)
            {
                Addr++;
                Bytes?.Add(0);
            }
            Count = 0;
            return (Addr??0);
        }

        public void AddSync(byte[] bytes)
        {
            Bytes?.AddRange(bytes);
            Addr += bytes.Count();
        }

    }

    public class BinImageDetail 
    {
        public BinImageDetail()
        {

            Addrs = new List<int>();
            HeadSizes = new List<int>();
        }

       public int ColorValue { get; set; }

       public int Num { get; set; }
       
       public PicCompress picCompress { get; set; }

       public List<int>? Addrs { get; set; }
       public List<int>? HeadSizes { get; set; }
    }
}
