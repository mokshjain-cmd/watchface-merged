using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WatchControlLibrary.Model;

namespace WatchBin.Model
{
    public class SlotRecordData : Bin
    {
        // For slot-type (see below) 3, count is always zero.
        public byte WidgetsCount { get; set; }
        public byte Reserved { get; set; }

        // Flags field
        // Using a ushort to match the uint16_t type in C
        public ushort Flags { get; set; }

        // Union for uid array and appWidget struct
        // Using a List to represent dynamic array
        public List<uint>? UidWidgets { get; set; } = new List<uint>();

        public byte[] GetBin()
        {
            var bytes = new List<byte>();
            bytes.Add(WidgetsCount);
            bytes.Add(Reserved);
            bytes.AddRange(BitConverter.GetBytes(Flags));
            bytes.AddRange(UidWidgets!.Select(x => BitConverter.GetBytes(x)).SelectMany(x => x));
            bytes.AddRange(PosTables.Select(x => x.GetBin()).SelectMany(x => x));
            return bytes.ToArray();
        }


        public void SetFlags(int posTableCount, int slotType)
        {
            var temp = 0;
            temp |= posTableCount > 0 ? 1 : 0;
            temp |= slotType << 4;
            temp |= posTableCount << 8;
            this.Flags = (ushort)temp;
        }

        public List<PosTable> PosTables { get; set; } = new List<PosTable>();

        public static SlotRecordData? GetSlotRecordData(Slot slot)
        {
            var slotData = new SlotRecordData();
            slotData.WidgetsCount = (byte)slot.Items!.Count();
            slotData.SetFlags(slot.Positions!.Count, 0);
            slotData.UidWidgets = slot.Items!.Select(x => BinCommonHelper.UidRelationTable[x.Ref!.Replace("@", "")]).ToList();
            if (slot.Positions.Count > 0)
            {
                slotData.PosTables = slot.Positions.Select(x => new PosTable
                {
                    X = x.X,
                    Y = x.Y,
                }).ToList();
            }
            return slotData;
        }
    }

    public class PosTable : Bin
    {
        public int X { get; set; }

        public int Y { get; set; }

        public byte[] GetBin()
        {
            var temp = 0;
            temp |= X;
            temp |= (Y << 10);
            return BitConverter.GetBytes((uint)temp);
        }
    }
}
