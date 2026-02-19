using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchBasic.UIBasic
{
    /// <summary>
    /// 指针
    /// </summary>
    public class PointerImage : BaseTypeItem
    {
        public PointerImage(List<LayerGroup>? layers, string? path)
        {
            PointerCode = Path.GetFileNameWithoutExtension(path)?.Split('_').FirstOrDefault();
            LayerGroups = layers;
            var showList = layers?.Where(i => !i.GroupName!.Contains("辅助文件")).ToList();
            if (showList != null)
                ShowGroups = new ObservableCollection<LayerGroup>(showList);
            DesignSketch = layers?.Where(i => i.GroupName!.Contains("辅助文件")).FirstOrDefault()?.GetLayerByName("效果.png");
            Overlayer = layers?.Where(i => i.GroupName!.Contains("辅助文件")).FirstOrDefault()?.GetLayerByName("叠加.png");
            CoordinateImage = layers?.Where(i => i.GroupName!.Contains("辅助文件")).FirstOrDefault()?.GetLayerByName("坐标");
            HourLayerGroup = GetLayerGroupsByKey("时针")?.ToList().FirstOrDefault();
            MinLayerGroup = GetLayerGroupsByKey("分针")?.ToList().FirstOrDefault();
            SecondLayerGroup = GetLayerGroupsByKey("秒针")?.ToList().FirstOrDefault();
            WatchType = "Pointer";
        }




        public bool HourEnable => HourLayerGroup == null ? false : true;
        public bool MinEnable => MinLayerGroup == null ? false : true;
        public bool SecondEnable => SecondLayerGroup == null ? false : true;
        public Layer? Overlayer { get; set; }

        public Layer? StyleLayer { get; set; }

        public Layer? CoordinateImage { get; set; }

        public string? PointerCode { get; set; }
        public List<LayerGroup>? LayerGroups { get; set; }

        private LayerGroup? secondLayerGroup;
        public LayerGroup? SecondLayerGroup
        {
            get => secondLayerGroup;
            set { SetProperty(ref secondLayerGroup, value); }
        }
        private LayerGroup? hourLayerGroup;
        public LayerGroup? HourLayerGroup
        {
            get => hourLayerGroup;
            set { SetProperty(ref hourLayerGroup, value); }
        }
        private LayerGroup? minLayerGroup;
        public LayerGroup? MinLayerGroup
        {
            get => minLayerGroup;
            set { SetProperty(ref minLayerGroup, value); }
        }

        private int secondAngle;
        public int SecondAngle
        {
            get => secondAngle;
            set
            {
                SetProperty(ref secondAngle, value);
                SetAngle(SecondLayerGroup, value);

            }
        }
        private int hourAngle;
        public int HourAngle
        {
            get => hourAngle;
            set
            {
                SetProperty(ref hourAngle, value);
                SetAngle(HourLayerGroup, value);

            }
        }
        private int minAngle;
        public int MinAngle
        {
            get => minAngle;
            set
            {
                SetProperty(ref minAngle, value);
                SetAngle(MinLayerGroup, value);
            }
        }

        void SetAngle(LayerGroup? layerGroup, int angle)
        {
            if (layerGroup != null)
            {
                layerGroup.SetAngle(angle);
            }
        }
    }
}
