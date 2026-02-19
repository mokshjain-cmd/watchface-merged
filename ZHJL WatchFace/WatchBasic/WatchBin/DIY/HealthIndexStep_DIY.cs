using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchBasic.UIBasic;
using WatchBasic.WatchBin.Model;

namespace WatchBasic.WatchBin.DIY
{
    public sealed class HealthIndexStep_DIY : HealthIndex_DIY
    {

        public HealthIndexStep_DIY(IEnumerable<LayerGroup>? layerGroups, bool isChecked, int align) : base(layerGroups, isChecked, align)
        {
            Align = (WatchAlign)align;
        }
        public override void Init()
        {
            if (IsChecked) 
            {
                CanClick = false;
                HasBackground = HasVal("0301");
                HasProgressBar = HasVal("0302");
                HasText = HasVal("0303");
                HasNum = HasVal("0304");
                HasAnimation = HasVal("0309");
            }
            
        }
    }
}
