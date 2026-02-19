using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchBasic.UIBasic;


namespace WatchBasic.WatchBin.Model
{
    public class DIYTimeIndex : BaseData
    {
        /// <summary>
        /// 小时十位
        /// </summary>
        public bool HasHourTens
        {
            get => Convert.ToBoolean(GetValue(15));
            set => SetValue(15, value);
        }

        /// <summary>
        /// 小时个位
        /// </summary>
        public bool HasHourUint
        {
            get => Convert.ToBoolean(GetValue(14));
            set => SetValue(14, value);
        }

        /// <summary>
        /// 小时分割符
        /// </summary>
        public bool HasHourSplit
        {
            get => Convert.ToBoolean(GetValue(13));
            set => SetValue(13, value);
        }

        /// <summary>
        /// 分钟十位
        /// </summary>

        public bool HasMinuteTens
        {
            get => Convert.ToBoolean(GetValue(12));
            set => SetValue(12, value);
        }
        /// <summary>
        /// 分钟个位
        /// </summary>
        public bool HasMinuteUint
        {
            get => Convert.ToBoolean(GetValue(11));
            set => SetValue(11, value);
        }

        /// <summary>
        /// 分钟分割符
        /// </summary>
        public bool HasMinuteSplit
        {
            get => Convert.ToBoolean(GetValue(10));
            set => SetValue(10, value);
        }

        /// <summary>
        /// 秒十位
        /// </summary>
        public bool HasSecondTens
        {
            get => Convert.ToBoolean(GetValue(9));
            set => SetValue(9, value);
        }
        /// <summary>
        /// 秒个位
        /// </summary>
        public bool HasSecondUnit
        {
            get => Convert.ToBoolean(GetValue(8));
            set => SetValue(8, value);
        }

        public DIYTimeIndex(IEnumerable<LayerGroup>? layerGroups) : base(layerGroups)
        {
        }

        public override void Init()
        {
            HasHourTens = HasVal("3001");
            HasHourUint = HasVal("3002");
            HasHourSplit = HasVal("3003");
            HasMinuteTens = HasVal("3004");
            HasMinuteUint = HasVal("3005");
            HasMinuteSplit = HasVal("3006");
            HasSecondTens = HasVal("3007");
            HasSecondUnit = HasVal("3008");
        }
    }

    public class DIYTime
    {
        [PropertyIndex(1)]
        public DIYX1? HourTens { get; set; }
        [PropertyIndex(2)]
        public DIYX1? HourUnit { get; set; }
        [PropertyIndex(3)]
        public DIYX1? HourSplit { get; set; }
        [PropertyIndex(4)]
        public DIYX1? MinuteTens { get; set; }
        [PropertyIndex(5)]
        public DIYX1? MinuteUnit { get; set; }
        [PropertyIndex(6)]
        public DIYX1? MinuteSplit { get; set; }
        [PropertyIndex(7)]
        public DIYX1? SecondTens { get; set; }
        [PropertyIndex(8)]
        public DIYX1? SecondUnit { get; set; }

        public byte[] Serialize()
        {
            return SerializeTool.SerializeObject(this);
        }

    }
}
