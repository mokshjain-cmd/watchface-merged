using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using WatchControlLibrary.Model;

namespace WatchBin.Model
{
    public class WidgetRecordData : Bin
    {

        public byte[] Name { get; set; } = new byte[32];
        public uint Background { get; set; }  // uid or color
        public uint UidPreviewImg { get; set; }  // uid for image or color
        public byte RecordCount { get; set; }
        public byte GroupType { get; set; }
        /// <summary>
        /// BIT0: widget uses color as background.
        /// BIT1: enable click to jump to app.
        /// BIT2: edit box image UID exists in optional data
        /// BIT3: icon image UID exists in optional data.
        /// BIT[11:4]: the app_id of the app to jump to.
        /// BIT12: widget width and height in optional data.
        /// BIT13: parameter(uint32_t) of the app to jump exists in optional data.
        /// BIT14: enable auto layout, 0:enable 1:disable
        /// BIT15: additional flags(uint32_t) exists in optional data.
        /// </summary>
        private ushort Flags { get; set; }
        public Tuple<ushort, ushort, uint>[]? Records { get; set; }
        public List<uint>? OptionalData { get; set; } = new List<uint>();


        public void SetFlags(bool useColor, bool toApp, bool editImage, bool iconImage, uint appid, bool size, bool para, bool auto, bool flags)
        {
            int temp = 0;
            if (useColor) temp |= 0x1;
            if (toApp) temp |= 1 << 1;
            if (editImage) temp |= 1 << 2;
            if (iconImage) temp |= 1 << 3;
            temp |= (int)appid << 4;
            if (size) temp |= 1 << 12;
            if (para) temp |= 1 << 13;
            if (auto) temp |= 1 << 14;
            if (flags) temp |= 1 << 15;
            Flags = (ushort)temp;
        }
        public byte[] GetBin()
        {
            var bytes = new List<byte>();
            bytes.AddRange(Name);
            bytes.AddRange(BitConverter.GetBytes(Background));
            bytes.AddRange(BitConverter.GetBytes(UidPreviewImg));
            bytes.Add(RecordCount);
            bytes.Add(GroupType);
            bytes.AddRange(BitConverter.GetBytes(Flags));
            if (Records?.Length > 0)
                foreach ((ushort x, ushort y, uint uid) in Records)
                {
                    bytes.AddRange(BitConverter.GetBytes(x));
                    bytes.AddRange(BitConverter.GetBytes(y));
                    bytes.AddRange(BitConverter.GetBytes(uid));
                }
            if (OptionalData?.Count > 0)
                foreach (uint data in OptionalData)
                    bytes.AddRange(BitConverter.GetBytes(data));
            return bytes.ToArray();
        }


        public static WidgetRecordData GetWidgetRecordData(Widget widget, string name = "")
        {
            var nameBytes = string.IsNullOrWhiteSpace(name) ? Encoding.ASCII.GetBytes(widget.Name!) : Encoding.ASCII.GetBytes(name);
            Array.Resize(ref nameBytes, 32);
            WidgetRecordData widgetRecordData = new WidgetRecordData
            {
                Name = nameBytes,
                Background = 0,
                UidPreviewImg = 0,
                RecordCount = (byte)widget.Items!.Count,
                GroupType = 0,//TODO
                Records = widget.Items.Where(x => !string.IsNullOrWhiteSpace(x.Ref))
                .Select(x => Tuple.Create<ushort, ushort, uint>(string.IsNullOrWhiteSpace(widget.FlexDirection) ? (ushort)x.X : (ushort)0, string.IsNullOrWhiteSpace(widget.FlexDirection) ? (ushort)x.Y : (ushort)0, BinCommonHelper.UidRelationTable[x.Ref!.Replace("@", "")])).ToArray(),

            };

            //if (!string.IsNullOrWhiteSpace(widget.EditBox)) 
            //{
            //    widgetRecordData.Background = BinCommonHelper.UidRelationTable[widget.EditBox.Replace("@", "")];
            //}
            if (!string.IsNullOrWhiteSpace(widget.Preview))
            {
                widgetRecordData.UidPreviewImg = BinCommonHelper.UidRelationTable[widget.Preview.Replace("@", "")];
            }



            int temp = 0;
            if (false) temp |= 0x0;
            if (!string.IsNullOrWhiteSpace(widget.JumpApp) && widget.JumpApp != "无")
            {
                temp |= 0x1;
                var appid = BinCommonHelper.GetAppId(widget.JumpApp!);
                temp |= 1 << 1;
                temp |= (int)appid << 4;

            }
            if (!string.IsNullOrWhiteSpace(widget.EditBox))
            {
                temp |= 1 << 2;
                widgetRecordData.OptionalData!.Add(BinCommonHelper.UidRelationTable[widget.EditBox!.Replace("@", "")]);
            }

            if (false) temp |= 1 << 3;//icon 

            if (false) temp |= 1 << 13; // parameter
            if ((!string.IsNullOrWhiteSpace(widget.JumpApp) && widget.JumpApp != "无") || !string.IsNullOrEmpty(widget.FlexDirection))//layout
            {
                temp |= 1 << 12; //size
                widgetRecordData.OptionalData!.Add((uint)((widget.Width << 16) | widget.Height));
                if (!string.IsNullOrEmpty(widget.FlexDirection))
                {
                    temp |= 1 << 14;
                    var opt = widget.FlexDirection == "row" ? 0 : 1;
                    opt |= BinCommonHelper.GetWidgetLayoutFlag(widget.JustifyContent!) << 2;
                    opt |= BinCommonHelper.GetWidgetLayoutFlag(string.IsNullOrWhiteSpace(widget.AlignContent) ? widget.AlignItems! : widget.AlignContent) << 5;
                    opt |= BinCommonHelper.GetWidgetLayoutFlag(widget.AlignItems!) << 8;
                    opt |= widget.Gap << 11;
                    widgetRecordData.OptionalData!.Add((uint)opt);
                }
            }
            else
            {
                temp |= 0 << 12; //size
                temp |= 0 << 14;
            }
            if (false) temp |= 1 << 15;//flags
            widgetRecordData.Flags = (ushort)temp;
            return widgetRecordData;
        }
    }
}
