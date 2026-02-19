using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchBasic.UIBasic
{
    public class TimeImage : BaseTypeItem
    {
        public TimeImage(List<LayerGroup> layers, string? timeCode)
        {
            var showList = layers.Where(i => !i.GroupName!.Contains("辅助文件")).ToList();
            LayerGroups = layers;
            ShowGroups = new ObservableCollection<LayerGroup>(showList);
            DesignSketch = layers?.Where(i => i.GroupName!.Contains("辅助文件")).FirstOrDefault()?.GetLayerByName("效果");
            Overlayer = layers?.Where(i => i.GroupName!.Contains("辅助文件")).FirstOrDefault()?.GetLayerByName("叠加");
            CoordinateImage = layers?.Where(i => i.GroupName!.Contains("辅助文件")).FirstOrDefault()?.GetLayerByName("坐标");
            WatchType = "Time";
            TimeCode = Path.GetFileNameWithoutExtension(timeCode)?.Split('_').FirstOrDefault();
            LocationName = Path.GetFileNameWithoutExtension(timeCode)?.Split('_').Skip(1).Take(1).FirstOrDefault();
            FontName = Path.GetFileNameWithoutExtension(timeCode)?.Split('_').Last();
            TimeLocationInfos = new List<TimeLocationInfo>();
        }


        public Layer? Overlayer { get; set; }

      
        public Layer? CoordinateImage { get; set; }

        public List<LayerGroup>? LayerGroups { get; set; }
        public string? TimeCode { get; set; }

        public string? LocationName { get; set; }

        public string? FontName { get; set; }

        public IEnumerable<TimeLocationInfo> TimeLocationInfos { get; set; }

    }

    public class TimeLocationInfo
    {
        public string? GroupCode { get;set; }
        /// <summary>
        /// bin文件写入坐标
        /// </summary>
        public int ColorAddr { get; set; }

        /// <summary>
        /// bin文件写入坐标
        /// </summary>
        public int LeftAddr { get; set; }

        /// <summary>
        /// bin文件写入坐标
        /// </summary>
        public int TopAddr { get; set; }

        public int  DisableAddr { get; set; }

    }
}
