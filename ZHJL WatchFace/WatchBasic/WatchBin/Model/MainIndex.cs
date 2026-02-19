using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WatchBasic.Tool;
using WatchBasic.UIBasic;
using WatchBasic.WatchBin.Model;

namespace WatchBasic.WatchBin.Model
{
    public class MainIndex : BaseData
    {
        public MainIndex(IEnumerable<LayerGroup>? layerGroups) : base(layerGroups)
        {
        }

        public override void Init()
        {
            IsBackground = true;
            IsThumbnail = true;
        }

        public bool IsThumbnail
        {
            get => Convert.ToBoolean(GetValue(0));
            set => SetValue(0, value);
        }
        public bool IsBackground
        {
            get => Convert.ToBoolean(GetValue(1));
            set => SetValue(1, value);
        }
    }

    /// <summary>
    /// 电池电量
    /// </summary>
    public class KWHIndex : BaseData
    {
        public KWHIndex(IEnumerable<LayerGroup>? layerGroups, int langCount, int Align) : base(layerGroups, langCount, Align)
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


        public bool HasText_CN
        {
            get => Convert.ToBoolean(GetValue(18));
            set => SetValue(18, value);
        }

        public bool HasText_EN
        {
            get => Convert.ToBoolean(GetValue(19));
            set => SetValue(19, value);
        }

        public bool HasText_CHT
        {
            get => Convert.ToBoolean(GetValue(20));
            set => SetValue(20, value);
        }

        public bool HasPicture
        {
            get => Convert.ToBoolean(GetValue(21));
            set => SetValue(21, value);
        }

        public bool HasNum
        {
            get => Convert.ToBoolean(GetValue(22));
            set => SetValue(22, value);
        }

        public bool HasPAH
        {
            get => Convert.ToBoolean(GetValue(23));
            set => SetValue(23, value);
        }

        public bool HasBlueTooth 
        {
            get => Convert.ToBoolean(GetValue(24));
            set => SetValue(24, value);
        }


        public override void Init()
        {
            CanClick = false;
            Align = (WatchAlign)base.Align;
            HasBackground = HasVal("0200");//背景
            HasProgressbar = HasVal("0201");//进度
            HasPicture = HasVal("0203");
            HasText_EN = HasVal("0202");
            if (LangCount == 2)
            {
                HasText_CHT = HasText_EN;
            }
            if (LangCount == 3)
            {
                HasText_CHT = HasText_EN;
                HasText_CN = HasText_EN;
            }
            HasPAH = HasVal("0204");
            HasNum = HasVal("0205");
            HasBlueTooth = HasVal("0102");
        }
    }

    /// <summary>
    /// 通用日期
    /// </summary>
    public class GeneralDateIndex : BaseData
    {
        public GeneralDateIndex(IEnumerable<LayerGroup>? layerGroups, int langCount, int align) : base(layerGroups, langCount, align)
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

        public bool HasWeek_CN
        {
            get => Convert.ToBoolean(GetValue(20));
            set => SetValue(20, value);
        }

        public bool HasWeek_EN
        {
            get => Convert.ToBoolean(GetValue(21));
            set => SetValue(21, value);
        }

        public bool HasWeek_CHT
        {
            get => Convert.ToBoolean(GetValue(22));
            set => SetValue(22, value);
        }



        public bool HasTimeSpan_CN
        {
            get => Convert.ToBoolean(GetValue(23));
            set => SetValue(23, value);
        }

        /// <summary>
        /// 时段（上下午）
        /// </summary>

        public bool HasTimeSpan_EN
        {
            get => Convert.ToBoolean(GetValue(24));
            set => SetValue(24, value);
        }

        public bool HasTimeSpan_CHT
        {
            get => Convert.ToBoolean(GetValue(25));
            set => SetValue(25, value);
        }



        public override void Init()
        {
            CanClick = false;
            HasBackground = HasVal("0300");
            HasMonthNum = HasVal("0301") || HasVal("0302");
            HasSeparate = HasVal("0303");
            HasDayNum = HasVal("0304") || HasVal("0305");
            HasWeek_EN = HasVal("0501");
            HasTimeSpan_EN = HasVal("0502");
            if (LangCount == 2)
            {
                HasWeek_CHT = HasWeek_EN;
                HasTimeSpan_CHT = HasTimeSpan_EN;
            }
            if (LangCount == 3)
            {
                HasWeek_CHT = HasWeek_EN;
                HasTimeSpan_CHT = HasTimeSpan_EN;

                HasWeek_CN = HasWeek_EN;
                HasTimeSpan_CN = HasTimeSpan_EN;

            }

        }
    }

    public sealed class HealthIndexStep : HealthIndex
    {
       
        public HealthIndexStep(IEnumerable<LayerGroup>? layerGroups, int langCount, int align) : base(layerGroups, langCount, align)
        {
            Align = (WatchAlign)align;
        }
        public override void Init()
        {
            CanClick = false;
            
            HasBackground = HasVal("0700");
            HasProgressBar = HasVal("0701");
            HasText_EN = HasVal("0702");
            HasUnit_EN = HasVal("0703");
            if (LangCount == 2)
            {
                HasText_CHT = HasText_EN;
                HasUnit_CHT = HasUnit_EN;
            }
            if (LangCount == 3)
            {
                HasText_CHT = HasText_EN;
                HasUnit_CHT = HasUnit_EN;

                HasText_CN = HasText_EN;
                HasUnit_CN = HasUnit_EN;
            }

            HasNum = HasVal("0705");
            HasAnimation = HasVal("0710");
        }
    }

    public sealed class HealthIndexCalorie : HealthIndex
    {
       
        public HealthIndexCalorie(IEnumerable<LayerGroup>? layerGroups, int langCount, int align) : base(layerGroups, langCount, align)
        {
            Align = (WatchAlign)align;
        }
        public override void Init()
        {
            CanClick = false;
            HasBackground = HasVal("0900");
            HasProgressBar = HasVal("0901");
            HasText_EN = HasVal("0902");
            HasUnit_EN = HasVal("0903");
            if (LangCount == 2)
            {
                HasText_CHT = HasText_EN;
                HasUnit_CHT = HasUnit_EN;
            }
            if (LangCount == 3)
            {
                HasText_CHT = HasText_EN;
                HasUnit_CHT = HasUnit_EN;

                HasText_CN = HasText_EN;
                HasUnit_CN = HasUnit_EN;
            }
            HasNum = HasVal("0905");
            HasAnimation = HasVal("0909");
        }
    }

    /// <summary>
    /// 步数、卡路里
    /// </summary>
    public abstract class HealthIndex : BaseData
    {

        public HealthIndex(IEnumerable<LayerGroup>? layerGroups, int langCount, int align) : base(layerGroups, langCount, align)
        {

        }
        public HealthIndex(IEnumerable<LayerGroup>? layerGroups, int align, Action? initData) : base(layerGroups, align, initData)
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
        /// 进度条
        /// </summary>

        public bool HasProgressBar
        {
            get => Convert.ToBoolean(GetValue(17));
            set => SetValue(17, value);
        }


        public bool HasText_CN
        {
            get => Convert.ToBoolean(GetValue(18));
            set => SetValue(18, value);
        }

        /// <summary>
        /// 文字
        /// </summary>
        public bool HasText_EN
        {
            get => Convert.ToBoolean(GetValue(19));
            set => SetValue(19, value);
        }

        public bool HasText_CHT
        {
            get => Convert.ToBoolean(GetValue(20));
            set => SetValue(20, value);
        }



        /// <summary>
        /// 数值
        /// </summary>
        public bool HasNum
        {
            get => Convert.ToBoolean(GetValue(21));
            set => SetValue(21, value);
        }


        public bool HasUnit_CN
        {
            get => Convert.ToBoolean(GetValue(22));
            set => SetValue(22, value);
        }

        /// <summary>
        /// 单位
        /// </summary>
        public bool HasUnit_EN
        {
            get => Convert.ToBoolean(GetValue(23));
            set => SetValue(23, value);
        }

        public bool HasUnit_CHT
        {
            get => Convert.ToBoolean(GetValue(24));
            set => SetValue(24, value);
        }

        /// <summary>
        /// 动图
        /// </summary>

        public bool HasAnimation
        {
            get => Convert.ToBoolean(GetValue(25));
            set => SetValue(25, value);
        }

        //public override void Init()
        //{
        //    Align = (WatchAlign)base.Align;
        //    InitData?.Invoke(LayerGroups, this);
        //}
    }

    public class HeartRateIndex : BaseData
    {
        public HeartRateIndex(IEnumerable<LayerGroup>? layerGroups, int langCount, int align) : base(layerGroups, langCount, align)
        {
        }

        public bool CanClick
        {
            get => Convert.ToBoolean(GetValue(0));
            set => SetValue(0, value);
        }

        public WatchAlign Align
        {
            get { var array = Value.GetBytes32();
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


        public bool HasText_CN
        {
            get => Convert.ToBoolean(GetValue(17));
            set => SetValue(17, value);
        }
        /// <summary>
        /// 文字
        /// </summary>
        public bool HasText_EN
        {
            get => Convert.ToBoolean(GetValue(18));
            set => SetValue(18, value);
        }

        public bool HasText_CHT
        {
            get => Convert.ToBoolean(GetValue(19));
            set => SetValue(19, value);
        }



        /// <summary>
        /// 数值
        /// </summary>
        public bool HasNum
        {
            get => Convert.ToBoolean(GetValue(20));
            set => SetValue(20, value);
        }


        /// <summary>
        /// 无数据
        /// </summary>
        public bool HasNull
        {
            get => Convert.ToBoolean(GetValue(21));
            set => SetValue(21, value);
        }


        public bool HasUnit_CN
        {
            get => Convert.ToBoolean(GetValue(22));
            set => SetValue(22, value);
        }
        /// <summary>
        /// 单位
        /// </summary>
        public bool HasUnit_EN
        {
            get => Convert.ToBoolean(GetValue(23));
            set => SetValue(23, value);
        }

        public bool HasUnit_CHT
        {
            get => Convert.ToBoolean(GetValue(24));
            set => SetValue(24, value);
        }



        /// <summary>
        /// 动图
        /// </summary>

        public bool HasAnimation
        {
            get => Convert.ToBoolean(GetValue(25));
            set => SetValue(25, value);
        }


        public override void Init()
        {
            CanClick = false;
            Align = (WatchAlign)base.Align;
            HasBackground = HasVal("0800");
            HasText_EN = HasVal("0801");
            HasUnit_EN = HasVal("0802");
            if (LangCount == 2)
            {
                HasText_CHT = HasText_EN;
                HasUnit_CHT = HasUnit_EN;
            }
            if (LangCount == 3)
            {
                HasText_CHT = HasText_EN;
                HasUnit_CHT = HasUnit_EN;

                HasText_CN = HasText_EN;
                HasUnit_CN = HasUnit_EN;
            }
            HasNull = HasVal("0803");
            HasNum = HasVal("0804");
            HasAnimation = HasVal("0807");

        }
    }

    public class TimeIndex : BaseData
    {
        public TimeIndex(IEnumerable<LayerGroup>? layerGroups, int langCount, int align) : base(layerGroups, langCount, align)
        {

        }

        public override void Init()
        {
            HasHourTens = HasVal("0601");
            HasHourUint = HasVal("0602");
            HasHourSplit = HasVal("0603");
            HasMinuteTens = HasVal("0604");
            HasMinuteUint = HasVal("0605");
            HasMinuteSplit = HasVal("0606");
            HasSecondTens = HasVal("0607");
            HasSecondUnit = HasVal("0608");
        }

        /// <summary>
        /// 小时十位
        /// </summary>
        public bool HasHourTens
        {
            get => Convert.ToBoolean(GetValue(0));
            set => SetValue(0, value);
        }

        /// <summary>
        /// 小时个位
        /// </summary>
        public bool HasHourUint
        {
            get => Convert.ToBoolean(GetValue(1));
            set => SetValue(1, value);
        }

        /// <summary>
        /// 小时分割符
        /// </summary>
        public bool HasHourSplit
        {
            get => Convert.ToBoolean(GetValue(2));
            set => SetValue(2, value);
        }

        /// <summary>
        /// 分钟十位
        /// </summary>

        public bool HasMinuteTens
        {
            get => Convert.ToBoolean(GetValue(3));
            set => SetValue(3, value);
        }
        /// <summary>
        /// 分钟个位
        /// </summary>
        public bool HasMinuteUint
        {
            get => Convert.ToBoolean(GetValue(4));
            set => SetValue(4, value);
        }

        /// <summary>
        /// 分钟分割符
        /// </summary>
        public bool HasMinuteSplit
        {
            get => Convert.ToBoolean(GetValue(5));
            set => SetValue(5, value);
        }

        /// <summary>
        /// 秒十位
        /// </summary>
        public bool HasSecondTens
        {
            get => Convert.ToBoolean(GetValue(6));
            set => SetValue(6, value);
        }
        /// <summary>
        /// 秒个位
        /// </summary>
        public bool HasSecondUnit
        {
            get => Convert.ToBoolean(GetValue(7));
            set => SetValue(7, value);
        }


    }

    public class TimePointerIndex : BaseData
    {
        public TimePointerIndex(IEnumerable<LayerGroup>? layerGroups, int langCount, int align) : base(layerGroups, langCount, align)
        {

        }

        public override void Init()
        {
            HasHourPointer = HasVal("1301");
            HasMinutePointer = HasVal("1302");
            HasSecondPointer = HasVal("1303");


        }
        public bool HasHourPointer
        {
            get => Convert.ToBoolean(GetValue(0));
            set => SetValue(0, value);
        }
        public bool HasMinutePointer
        {
            get => Convert.ToBoolean(GetValue(1));
            set => SetValue(1, value);
        }
        public bool HasSecondPointer
        {
            get => Convert.ToBoolean(GetValue(2));
            set => SetValue(2, value);
        }



    }


}
