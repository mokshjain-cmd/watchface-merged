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
    /// 电池电量
    /// </summary>
    public class KWHIndex_DIY : BaseData
    {
        public KWHIndex_DIY(IEnumerable<LayerGroup>? layerGroups, bool isChecked, int Align) : base(layerGroups, isChecked, Align)
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


        public bool HasProgressbar
        {
            get => Convert.ToBoolean(GetValue(17));
            set => SetValue(17, value);
        }


        public bool HasText
        {
            get => Convert.ToBoolean(GetValue(18));
            set => SetValue(18, value);
        }

       

        public bool HasPicture
        {
            get => Convert.ToBoolean(GetValue(19));
            set => SetValue(19, value);
        }

        public bool HasNum
        {
            get => Convert.ToBoolean(GetValue(20));
            set => SetValue(20, value);
        }

        public bool HasPAH
        {
            get => Convert.ToBoolean(GetValue(21));
            set => SetValue(21, value);
        }
        public override void Init()
        {

            if (IsChecked)
            {
                CanClick = false;
                Align = (WatchAlign)base.Align;
                HasBackground = HasVal("0101");//背景
                HasProgressbar = HasVal("0102");//进度
                HasPicture = HasVal("0103");
                HasText = HasVal("0104");
                HasPAH = HasVal("0105");
                HasNum = HasVal("0106");
            }
        }
    }
}
