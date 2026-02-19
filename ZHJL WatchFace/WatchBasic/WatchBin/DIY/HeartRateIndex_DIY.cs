using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchBasic.Tool;
using WatchBasic.UIBasic;
using WatchBasic.WatchBin.Model;

namespace WatchBasic.WatchBin.DIY
{
    public class HeartRateIndex_DIY : BaseData
    {
        public HeartRateIndex_DIY(IEnumerable<LayerGroup>? layerGroups, bool isChecked, int align) : base(layerGroups, isChecked, align)
        {
        }

        public bool CanClick
        {
            get => Convert.ToBoolean(GetValue(0));
            set => SetValue(0, value);
        }

        public WatchAlign Align
        {
            get
            {
                var array = Value.GetBytes32();
                return (WatchAlign)array[1];
            }
            set
            {
                var array = Value.GetBytes32();
                array[1] = (byte)value;
                Value = array.GetInt32();
            }
        }

        public bool HasBackground
        {
            get => Convert.ToBoolean(GetValue(16));
            set => SetValue(16, value);
        }

        public bool HasText
        {
            get => Convert.ToBoolean(GetValue(17));
            set => SetValue(17, value);
        }

        /// <summary>
        /// 数值
        /// </summary>
        public bool HasNum
        {
            get => Convert.ToBoolean(GetValue(18));
            set => SetValue(18, value);
        }


        /// <summary>
        /// 无数据
        /// </summary>
        public bool HasNull
        {
            get => Convert.ToBoolean(GetValue(19));
            set => SetValue(19, value);
        }


        public bool HasUnit
        {
            get => Convert.ToBoolean(GetValue(20));
            set => SetValue(20, value);
        }
        
        /// <summary>
        /// 动图
        /// </summary>

        public bool HasAnimation
        {
            get => Convert.ToBoolean(GetValue(21));
            set => SetValue(21, value);
        }

        public override void Init()
        {
            if (IsChecked)
            {
                CanClick = true;
                Align = (WatchAlign)base.Align;
                HasBackground = HasVal("0501");
                HasText = HasVal("0502");
                HasUnit = HasVal("0503");
                HasNull = HasVal("0504");
                HasNum = HasVal("0505");
                HasAnimation = HasVal("0508");
            }
        }
    }
}
