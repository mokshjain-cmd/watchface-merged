using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchBasic.WatchBin.Model
{

    public class DIYBase
    {
        public DIYBase()
        {
            Addrs = new List<int>();
            HeadSizes = new List<int>();
        }
        public virtual int Addr { get; set; }

        public virtual int HeadSize { get; set; }

        public List<int> Addrs { get; set; }

        public List<int> HeadSizes { get; set; }

        public virtual int Num { get; set; }

        public virtual PicCompress PicCompress { get; set; }
    }
    public class DIYX0 : DIYBase
    {
        [PropertyIndex(1)]
        [ByteNum(2)]
        public int Width { get; set; }

        [PropertyIndex(2)]
        [ByteNum(2)]
        public int Height { get; set; }

        [PropertyIndex(3)]
        [ByteNum(2)]
        public override int Num { get { return base.Num; } set { base.Num = value; } }

        /// <summary>
        /// 压缩方式
        /// </summary>
        [PropertyIndex(4)]
        [ByteNum(1)]
        public PicCompress PicCompress { get; set; }

        [PropertyIndex(5)]
        [ByteNum(4)]
        public override int Addr { get { return base.Addr; } set { base.Addr = value; } }

        [PropertyIndex(6)]
        [ByteNum(4)]
        public override int HeadSize { get { return base.HeadSize; } set { base.HeadSize = value; } }

    }

    public class DIYX1 : DIYBase
    {

        public string? Code { get; set; }

        [PropertyIndex(1)]
        [ByteNum(1)]
        public int Key { get; set; }

        [PropertyIndex(2)]
        [ByteNum(2)]
        public int X { get; set; }

        [PropertyIndex(3)]
        [ByteNum(2)]
        public int Y { get; set; }

        [PropertyIndex(4)]
        [ByteNum(2)]
        public int Width { get; set; }

        [PropertyIndex(5)]
        [ByteNum(2)]  
        public int Height { get; set; }

        [PropertyIndex(6)]
        [ByteNum(1)]
        public override int Num { get { return base.Num; } set { base.Num = value; } }

        /// <summary>
        /// 压缩方式
        /// </summary>
        [PropertyIndex(7)]
        [ByteNum(1)]
        public override PicCompress PicCompress { get { return base.PicCompress; } set { base.PicCompress = value; } }

        [PropertyIndex(8)]
        [ByteNum(4)]
        public override int Addr { get { return base.Addr; } set { base.Addr = value; } }

        /// <summary>
        /// 大小（参照深圳文档）Todo
        /// </summary>
        [PropertyIndex(9)]
        [ByteNum(4)]
        public override int HeadSize { get { return base.HeadSize; } set { base.HeadSize = value; } }

        /// <summary>
        /// 颜色值
        /// </summary>
        [PropertyIndex(10)]
        [ByteNum(3)]
        public int ColorValue { get; set; }

        /// <summary>
        /// 保留
        /// </summary>
        [PropertyIndex(11)]
        [ByteNum(2)]
        public int Reserve { get; set; }


    }

    /// <summary>
    /// 图片压缩 Todo
    /// </summary>
    public enum PicCompress
    {
        bmp1 = 0,
        bmp2 = 1,
        png1 = 2,//rgba
        png2 = 3,//a+color
    }

}
