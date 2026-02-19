using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchBasic.Event;

namespace WatchBasic.UIBasic
{
    public class BaseTypeItem : BindableBase
    {

        private Layer? designSketch;

        public Layer? DesignSketch
        {
            get => designSketch;
            set
            {
                SetProperty(ref designSketch, value);
                RaisePropertyChanged(nameof(IsVaild));
            }
        }

        public IEventAggregator? _eventaggregator;

        public bool IsVaild => DesignSketch != null;

        private ObservableCollection<LayerGroup>? showGroups;

        public ObservableCollection<LayerGroup>? ShowGroups
        {
            get => showGroups;
            set
            {
                SetProperty(ref showGroups, value);
                RaisePropertyChanged(nameof(Layers));
            }
        }


        public void AsyncLocationItem()
        {

            _eventaggregator?.GetEvent<WacthTypeLocationGroupsEvent>().Publish(new WacthTypeLocationGroupsAsync
            {
                WatchType = this.WatchType,
                LayerGroups = this.ShowGroups

            });
        }

        public void Refresh()
        {
            RaisePropertyChanged(nameof(Layers));
        }

        public IEnumerable<Layer?>? Layers => ShowGroups?.Where(i => i.Layers!.Any()).OrderBy(i => i.GroupCode).Select(i => i?.ShowLayer);

        public IEnumerable<LayerGroup>? GetLayerGroupsByKey(string key)
        {
            return ShowGroups?.Where(i => i.GroupName!.Contains(key) && i.LayerNum > 0);
        }

        public void SetLayerRatio(decimal ratio)
        {
            if (DesignSketch != null)
                DesignSketch.Ratio = ratio;
            foreach (var layergroup in ShowGroups ?? new ObservableCollection<LayerGroup>())
            {
                layergroup.SetLayerRatio(ratio);
            }
        }

        //public void SetLocation(int left ,int top,string? colorDesc=null) 
        //{
        //    foreach (var layergroup in ShowGroups ?? new ObservableCollection<LayerGroup>())
        //    {
        //        layergroup.SetLocation(left,top);
        //        if (string.IsNullOrWhiteSpace(colorDesc)) 
        //        {
        //            layergroup.UpdateColorDesc(colorDesc);
        //        }
        //    }
        //}

        public IEnumerable<LayerGroup>? InitGroups { get; set; }

        private string? watchType;

        public string? WatchType
        {
            get => watchType;
            set => SetProperty(ref watchType, value);
        }
    }
}
