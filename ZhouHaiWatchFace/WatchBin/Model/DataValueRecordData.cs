using MiniExcelLibs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchBin.BinAnalyze;
using WatchControlLibrary;
using WatchControlLibrary.Model;

namespace WatchBin.Model
{
    public class DataValueRecordData : Bin
    {
        public ushort SourceId { get; set; }
        /// <summary>
        /// How many digits this data has. Not used for pointer, progress bar etc.
        /// bit[7:4] means decimal fraction digits
        /// bit[3:0] total digits.
        /// </summary>
        public byte Digits { get; set; }
        /// <summary>
        /// bit[1:0]: align mode
        /// bit2: enable leading zero,
        /// bit3: enable trailing zero,
        /// bit[7:4]: style_flag(enum DataStyle)
        /// </summary>
        private byte Flags { get; set; }
        /// <summary>
        /// bit[0]: support recolor
        /// bit[8:1]: render rule
        /// </summary>
        private ushort Flags1 { get; set; }

        public ushort Parameter { get; set; }

        public DataValueStyle? StyleValue { get; set; }

        public void SetFlags(byte align, bool leading, bool trailing, byte style)
        {
            if (align > 3)
                throw new Exception("键值对项: 错误的对齐方式");
            //if (leading && trailing) align = 0;
            int temp = (style & 0xF) << 4;
            if (trailing) temp |= 0x1 << 3;
            if (leading) temp |= 0x1 << 2;
            temp |= align & 0x3;
            Flags = (byte)temp;
        }
        public void SetFlags1(bool recolor, byte render)
        {
            Flags1 = (byte)((render & 0x7F) << 1);
            if (recolor) Flags1 |= 0x1;

        }

        public byte GetAlign(string align)
        {

            return align switch
            {
                "left" => 1,
                "right" => 0,
                "center" => 2,
                _ => throw new Exception("对齐未知类型"),
            };

        }

        public ushort GetSourceId(string source)
        {
            var source2 = (int)Enum.Parse(typeof(DataOriginDataSource), source, ignoreCase: true);
            return (ushort)source2;
        }

        public void SetDigits(int totalDigits, int decimalDigits)
        {
            var temp = totalDigits & 0xF;
            temp |= (decimalDigits & 0xf) << 4;
            this.Digits = (byte)temp;
        }

        public static DataValueRecordData GetDataValueRecordData(DataItemImageNumber dataItem, RecordBase record)
        {
            DataValueRecordData recordData = new DataValueRecordData();
            recordData.SourceId = recordData.GetSourceId(dataItem.Source!);
            recordData.SetDigits(dataItem.TotalDigits, dataItem.DecimalDigits);
            recordData.SetFlags(recordData.GetAlign(dataItem.Align!), dataItem.LeadingZero, dataItem.TrailingZero, (byte)DataStyle.DATA_STYLE_IMAGE_NUMBER);
            recordData.SetFlags1(false, (byte)RenderRule.ALWAYS_SHOW);
            recordData.Parameter = (ushort)dataItem.Parameter;
            recordData.StyleValue = new StyleImageNumber
            {
                UidImageArray = BinCommonHelper.UidRelationTable[dataItem.Ref!.Replace("@", "")],
                DecimalOffsetX = (sbyte)dataItem.DecimalOffsetX,
                Rotation = (sbyte)dataItem.Rotation,
                Space = (sbyte)dataItem.Space,
                UidUnitIcon = string.IsNullOrWhiteSpace(dataItem.UnitIcon) ? 0 : BinCommonHelper.UidRelationTable[dataItem.UnitIcon!.Replace("@", "")]
            };
            return recordData;
        }

        public static DataValueRecordData GetDataValueRecordData(DataItemImageValues dataItem, RecordBase record)
        {
            DataValueRecordData recordData = new DataValueRecordData();
            recordData.SourceId = recordData.GetSourceId(dataItem.Source!);
            recordData.SetDigits(0, 0);
            recordData.SetFlags(0, false, false, (byte)DataStyle.DATA_STYLE_IMAGE_VALUES);
            if (dataItem.Source == "miscIsPM")
            {
                recordData.SetFlags1(false, (byte)RenderRule.HIDE_WHEN_UNIT_MISMATCH);
            }
            else
            {
                recordData.SetFlags1(false, (byte)RenderRule.ALWAYS_SHOW);
            }
            recordData.Parameter = (ushort)1000;
            recordData.StyleValue = new StyleImageValues
            {
                UidImageArray = BinCommonHelper.UidRelationTable[dataItem.Ref!.Replace("@", "")],
                Rotation = (sbyte)dataItem.Rotation,
            };
            var imageValues = (StyleImageValues)recordData.StyleValue;
            imageValues.SetFlags(false, dataItem.Params != null, false);
            if (dataItem.Params?.Any() ?? false)
            {
                //  dataItem.Source
                imageValues.PosValueTable = dataItem.Params.Select(o =>
                {
                    return Tuple.Create<int?, int?, int?>(null, null, o.Value);
                }).ToArray();

            }

            return recordData;
        }

        public static DataValueRecordData GetDataValueRecordData(DataItemPointer dataItem, RecordBase record)
        {
            DataValueRecordData recordData = new DataValueRecordData();
            recordData.SourceId = recordData.GetSourceId(dataItem.Source!);
            recordData.SetDigits(0, 0);
            recordData.SetFlags(0, false, false, (byte)DataStyle.DATA_STYLE_POINTER);
            recordData.SetFlags1(false, (byte)RenderRule.ALWAYS_SHOW);
            recordData.Parameter = (ushort)1000;
            recordData.StyleValue = new StylePointer
            {
                UidImg = BinCommonHelper.UidRelationTable[dataItem.Ref!.Replace("@", "")],
                AngleRange = (short)(dataItem.AngleRange * 10),
                AngleStart = (short)(dataItem.AngleStart * 10),
                CenterX = (short)dataItem.PivotX,
                CenterY = (short)dataItem.PivotY,
                ValueRange = BinCommonHelper.GetQ24(dataItem.ValueRange),
                ValueStart = BinCommonHelper.GetQ24(dataItem.ValueStart),
            };
            var point = (StylePointer)recordData.StyleValue;
            point.SetFlags(false, false);
            return recordData;
        }




        public byte[] GetBin()
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(SourceId));
            bytes.Add(Digits);
            bytes.Add(Flags);
            bytes.AddRange(BitConverter.GetBytes(Flags1));

            switch (StyleValue)
            {
                case StyleImageNumber number:
                    bytes.AddRange(BitConverter.GetBytes(Parameter));
                    bytes.AddRange(BitConverter.GetBytes(number.UidImageArray));
                    bytes.Add((byte)number.DecimalOffsetX);
                    bytes.Add((byte)number.Space);
                    bytes.AddRange(BitConverter.GetBytes(number.Rotation));
                    bytes.AddRange(BitConverter.GetBytes(number.UidUnitIcon));
                    break;
                case StyleImageValues values:
                    bytes.AddRange(BitConverter.GetBytes((ushort)0));//Parameter
                    bytes.AddRange(BitConverter.GetBytes(values.UidImageArray));
                    bytes.Add(values.Reserved);
                    bytes.Add(values.Flags);
                    bytes.AddRange(BitConverter.GetBytes(values.Rotation));
                    if (values.PosValueTable?.Length > 0)
                        foreach ((int? x, int? y, int? value) in values.PosValueTable)
                        {
                            if (x is not null) bytes.AddRange(BitConverter.GetBytes(x.Value));
                            if (y is not null) bytes.AddRange(BitConverter.GetBytes(y.Value));
                            if (value is not null) bytes.AddRange(BitConverter.GetBytes(value.Value));
                        }
                    break;
                case StylePointer pointer:
                    bytes.AddRange(BitConverter.GetBytes(Parameter));
                    bytes.AddRange(BitConverter.GetBytes(pointer.UidImg));
                    bytes.AddRange(BitConverter.GetBytes(pointer.ValueStart));
                    bytes.AddRange(BitConverter.GetBytes(pointer.ValueRange));
                    bytes.AddRange(BitConverter.GetBytes(pointer.CenterX));
                    bytes.AddRange(BitConverter.GetBytes(pointer.CenterY));
                    bytes.AddRange(BitConverter.GetBytes(pointer.AngleStart));
                    bytes.AddRange(BitConverter.GetBytes(pointer.AngleRange));
                    bytes.AddRange(BitConverter.GetBytes(pointer.Flags));
                    break;
                default:
                    break;
            }
            return bytes.ToArray();
        }
    }
    public abstract class DataValueStyle { };
    public class StyleImageNumber : DataValueStyle
    {
        public uint UidImageArray { get; set; } /* ID of image array to represent data */
        public sbyte DecimalOffsetX { get; set; } /* decimal point image offset */
        public sbyte Space { get; set; } /* space between images in pixel(before rotation if any). */
        public short Rotation { get; set; } /* All images rotate certain angle, the angle is in 0.1degree unit. */
        public uint UidUnitIcon { get; set; } /* uid of unit icon if exist, 0 if not used. */


    }

    public class StyleImageValues : DataValueStyle
    {
        public uint UidImageArray { get; set; }
        public byte Reserved { get; set; }
        /// <summary>
        /// bit0: has_position_table
        /// bit1: has_value_table
        /// bit2: has invalid value image
        /// </summary>
        public byte Flags { get; set; }
        public short Rotation { get; set; }

        public Tuple<int?, int?, int?>[]? PosValueTable;  // 生成bin的时候根据是否为空添加数据

        public void SetFlags(bool hasPostionTable, bool hasValueTable, bool hasInvalidValueImage)
        {
            var temp = (hasPostionTable ? 1 : 0);
            temp |= (hasValueTable ? 1 : 0) << 1;
            temp |= (hasInvalidValueImage ? 1 : 0) << 2;
            this.Flags = (byte)temp;

        }

    }
    public class StylePointer : DataValueStyle
    {
        public uint UidImg { get; set; }
        public int ValueStart { get; set; }
        public int ValueRange { get; set; }
        public short CenterX { get; set; }
        public short CenterY { get; set; }
        public short AngleStart { get; set; }
        public short AngleRange { get; set; }
        /// <summary>
        /// bit2：whether value_start is data source
        /// bit3: whether value_range is data source
        /// </summary>
        public uint Flags { get; set; }

        public void SetFlags(bool start, bool range)
        {
            var temp = ((start ? 1 : 0) & 0x1 << 2) | ((range ? 1 : 0) & 0x1 << 3);
            Flags = (uint)temp;
        }



    }


}
