using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchBasic.UIBasic;
using WatchBasic.WatchBin.Model;

namespace WatchBasic.WatchBin.DIY
{
    public class TimePointerIndex_DIY : BaseData
    {
        public TimePointerIndex_DIY(IEnumerable<LayerGroup>? layerGroups, int langCount, int align) : base(layerGroups, langCount, align)
        {

        }

        public override void Init()
        {
            HasHourPointer = HasVal("3101");
            HasMinutePointer = HasVal("3102");
            HasSecondPointer = HasVal("3103");


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
