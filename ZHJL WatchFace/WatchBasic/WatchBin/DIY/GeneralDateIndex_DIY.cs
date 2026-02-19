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
    /// <summary>
    /// 通用日期
    /// </summary>
    public class GeneralDateIndex_DIY : BaseData
    {
        public GeneralDateIndex_DIY(IEnumerable<LayerGroup>? layerGroups, bool isChecked, int align) : base(layerGroups, isChecked, align)
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



        /// <summary>
        /// 月数值
        /// </summary>

        public bool HasMonthNum
        {
            get => Convert.ToBoolean(GetValue(17));
            set => SetValue(17, value);
        }


        /// <summary>
        /// 分隔符
        /// </summary>

        public bool HasSeparate
        {
            get => Convert.ToBoolean(GetValue(18));
            set => SetValue(18, value);
        }

        /// <summary>
        /// 日数值
        /// </summary>

        public bool HasDayNum
        {
            get => Convert.ToBoolean(GetValue(19));
            set => SetValue(19, value);
        }

        /// <summary>
        /// 星期
        /// </summary>

        public bool HasWeek
        {
            get => Convert.ToBoolean(GetValue(20));
            set => SetValue(20, value);
        }


        public bool HasTimeSpan
        {
            get => Convert.ToBoolean(GetValue(21));
            set => SetValue(21, value);
        }
        public override void Init()
        {
            if (IsChecked)
            {
                CanClick = false;
                HasBackground = HasVal("0201");
                HasMonthNum = HasVal("0202") || HasVal("0203");
                HasSeparate = HasVal("0204");
                HasDayNum = HasVal("0205") || HasVal("0206");
                HasWeek = HasVal("0207");
                HasTimeSpan = HasVal("0208");
            }
        }
    }
}
