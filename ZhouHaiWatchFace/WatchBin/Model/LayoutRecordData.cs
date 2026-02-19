using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchControlLibrary.Model;

namespace WatchBin.Model
{
    public class LayoutRecordData : Bin
    {
        public uint ResUid { get; set; }        // 资源的 UID
        public ushort X { get; set; }           // 资源在屏幕上的 X 坐标
        public ushort Y { get; set; }           // 资源在屏幕上的 Y 坐标

        /// <summary>
        /// 渲染 res_uid 时的参数。
        /// 当 res_uid 是 slot_type 时，parameter 的 bit[7:0] 用于识别要渲染的 widget。
        /// 当 res_uid 是其他类型时，parameter 保留。
        /// </summary>
        public uint Parameter { get; set; }     // 渲染参数
        public ushort Reserved1 { get; set; }   // 保留字段1
        public ushort Reserved { get; set; }     // 保留字段

        public byte[] GetBin()
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(ResUid));
            bytes.AddRange(BitConverter.GetBytes(X));
            bytes.AddRange(BitConverter.GetBytes(Y));
            bytes.AddRange(BitConverter.GetBytes(Parameter));
            bytes.AddRange(BitConverter.GetBytes(Reserved1));
            bytes.AddRange(BitConverter.GetBytes(Reserved));
            return bytes.ToArray();
        }

        public static LayoutRecordData GetLayoutRecordData(Layout layout)
        {
            return new LayoutRecordData
            {
                ResUid = BinCommonHelper.UidRelationTable[layout!.Ref!.Replace("@", "")],
                X = (ushort)layout.X,
                Y = (ushort)layout.Y,
            };

        }

       

    }
}
