using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchBasic.UIBasic;
using WatchBasic.WatchBin.Model;

namespace WatchBasic.WatchBin.DIY
{
    public sealed class HealthIndexCalorie_DIY : HealthIndex_DIY
    {
        public HealthIndexCalorie_DIY(IEnumerable<LayerGroup>? layerGroups, bool isChecked, int align) : base(layerGroups, isChecked, align)
        {
            Align = (WatchAlign)align;
        }
        public override void Init()
        {
            if (IsChecked) 
            {
                CanClick = true;
                HasBackground = HasVal("0401");
                HasProgressBar = HasVal("0402");
                HasText = HasVal("0403");
                HasUnit = HasVal("0404");
                HasNum = HasVal("0405");
                HasAnimation = HasVal("0409");
            }
           
        }
    }
}
