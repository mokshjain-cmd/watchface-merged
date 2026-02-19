using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchBasic.UIBasic;

namespace WatchBasic.WatchBin.Model
{
    public abstract class BaseData
    {
        protected Action? InitData;
        public BaseData(IEnumerable<LayerGroup>? layerGroups)
        {
            LayerGroups = layerGroups;
            Init();
        }
        public BaseData(IEnumerable<LayerGroup>? layerGroups,int langCount, int align)
        {
            LayerGroups = layerGroups;
            Align = align;
            LangCount=langCount;
            Init();

        }

        public BaseData(IEnumerable<LayerGroup>? layerGroups, int align, Action? initData)
        {
            LayerGroups = layerGroups;

            Align = align;
            InitData = initData;
            Init();

        }

        public BaseData(IEnumerable<LayerGroup>? layerGroups, bool isChecked, int align)
        {
            LayerGroups = layerGroups;
            IsChecked = isChecked;
            Align = align;
            Init();

        }

        public IEnumerable<LayerGroup>? LayerGroups { get; set; }

        /// <summary>
        /// 语言数量
        /// </summary>
        public int LangCount { get; set; }

        public int Align { get; set; }

        public bool IsChecked { get; set; }

        public bool HasVal(string? groupCode)
        {
            var group = LayerGroups?.Where(i => i.GroupCode == groupCode)?.FirstOrDefault();
            return group == null ? false : group?.LayerNum > 0;
        }




        public uint Value { get; set; }

        public abstract void Init();


        public static uint SetIntVal(uint val, int offset, bool value)
        {
            if (value)
            {
                val = Convert.ToUInt32(val | Convert.ToUInt32(1 << offset));
            }
            else
            {
                var num = Convert.ToBoolean((val >> (offset)) & 1) ? 1 : 0;
                val = Convert.ToUInt32(val ^ (num << offset));
            }
            return val;
        }

        public void SetValue(int offset, bool value)
        {
            if (value)
            {
                Value = Convert.ToUInt32(Value | (uint)(1 << offset));
            }
            else
            {
                var num = Convert.ToBoolean((Value >> (offset)) & 1) ? 1 : 0;
                Value = Convert.ToUInt32(Value ^ (num << offset));
            }

        }
        public uint GetValue(int offset)
        {
            return Convert.ToUInt32((Value >> (offset)) & 1);
        }

        public static uint GetValue(int Val, int offset)
        {
            return Convert.ToUInt32((Val >> (offset)) & 1);
        }

    }

    public enum WatchAlign
    {
        Full = 0,
        Left = 1,
        Center = 2,
        Right = 3,
    }
}
