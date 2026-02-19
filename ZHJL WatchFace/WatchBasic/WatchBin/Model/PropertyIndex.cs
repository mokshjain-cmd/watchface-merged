using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchBasic.WatchBin.Model
{
    public class PropertyIndex : Attribute
    {
        public PropertyIndex(int index)
        {
            this.Index = index;
        }

        public int Index { get; set; }
    }

    /// <summary>
    /// 所占字节数
    /// </summary>
    public class ByteNum : Attribute
    {
        public int Num { get; set; }
        public ByteNum(int num)
        {
            this.Num = num;
        }
    }
}
