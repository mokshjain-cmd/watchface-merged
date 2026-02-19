using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchBasic.UIBasic
{
    /// <summary>
    /// 预览图展示图层
    /// </summary>
    public class WatchTypeLocationItem : WatchTypeItem
    {

        public Action<string?, string?, bool>? UpdateDefault { get; set; }
        public string? LocationName { get; set; }

        public WatchTypeLocationItem(ObservableCollection<LayerGroup> showGroups, int aleft, int atop, Action<string?, string?, bool>? updateDefault)
        {
            this.UpdateDefault = updateDefault;
            this.ShowGroups = new ObservableCollection<LayerGroup>(LayerTool.CopyLayerGroups(showGroups));
            SetAbsoulteLocation(aleft, atop);

        }

        public void SetAbsoulteLocation(int left, int top)
        {
            if (ShowGroups != null)
            {
                foreach (var i in ShowGroups)
                {
                    if (i.LayerNum > 0)
                        i.SetAbsoulteLocation(left, top);
                }

            }
        }



        private bool isDefault;

        public bool IsDefault
        {
            get => isDefault;
            set
            {
                SetProperty(ref isDefault, value);
                if (value)
                    UpdateDefault?.Invoke(LocationName, WatchType, value);

            }
        }




        public Action? RefreshLayer;

    }
}
