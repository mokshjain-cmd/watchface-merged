using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchBasic.UIBasic
{
    /// <summary>
    /// 电量、通用日期、心率、步数、卡路里
    /// </summary>
    public class WatchTypeItem : BaseTypeItem
    {
        public WatchTypeItem()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layers"></param>
        /// <param name="groupCode"></param>

        public WatchTypeItem(IEnumerable<LayerGroup>? layers, string groupCode, IEventAggregator eventAggregator)
        {
            this._eventaggregator = eventAggregator;
            this.InitGroups = layers;
            var showList = layers?.Where(i => i.GroupCode!.StartsWith(groupCode)).Where(i => !i.GroupName!.Contains("辅助文件")).ToList();
            Code = groupCode;
            if (showList != null)
                ShowGroups = new ObservableCollection<LayerGroup>(showList);
            DesignSketch = layers?.Where(i => i.GroupCode!.StartsWith(groupCode) && i.GroupName!.Contains("辅助文件")).FirstOrDefault()?.GetLayerByName("效果.png");
            WatchTitle = GetTitleDiy(groupCode);
            WatchType = GetTypeTxtDiy(groupCode);
        }

        public WatchTypeItem(IEnumerable<LayerGroup>? layers, string groupCode)
        {
            this.InitGroups = layers;
            var showList = layers?.Where(i => i.GroupCode!.StartsWith(groupCode)).Where(i => !i.GroupName!.Contains("辅助文件")).ToList();
            Code = groupCode;
            if (groupCode == "03")
            {
                var otherList = layers?.Where(i => i.GroupCode!.StartsWith("05") || i.GroupCode!.StartsWith("04")).Where(i => !i.GroupName!.Contains("辅助文件")).ToList();
                if (otherList != null)
                    showList?.AddRange(otherList);
              
            }
            if (groupCode == "02")
            {
                var bluetooth = layers?.Where(i => i.GroupCode!.StartsWith("0102"));
                if (bluetooth != null)
                    showList?.AddRange(bluetooth);
            }
            if (showList != null)
                ShowGroups = new ObservableCollection<LayerGroup>(showList.OrderBy(i => i.GroupCode));
            DesignSketch = layers?.Where(i => i.GroupCode!.StartsWith(groupCode) && i.GroupName!.Contains("辅助文件")).FirstOrDefault()?.GetLayerByName("效果.png");
            WatchTitle = GetTitle(groupCode);
            WatchType = GetTypeTxt(groupCode);
        }

        string GetTitleDiy(string code)
        {
            return code switch
            {
                "01" => "电量",
                "02" => "通用日期",
                "03" => "步数",
                "04" => "卡路里",
                "05" => "心率",
                _ => string.Empty,
            };
        }

        string GetTypeTxtDiy(string code)
        {
            return code switch
            {
                "01" => "Kwh",
                "02" => "GeneralDate",
                "03" => "Step",
                "04" => "Calorie",
                "05" => "HeartRate",
                _ => string.Empty,
            };
        }



        public string? Code { get; set; }

        string GetTitle(string code)
        {
            return code switch
            {
                "02" => "电量",
                "03" => "通用日期",
                "06" => "时间",
                "07" => "步数",
                "09" => "卡路里",
                "08" => "心率",
                _ => string.Empty,
            };
        }

        string GetTypeTxt(string code)
        {
            switch (code)
            {
                case "02": return "Kwh";
                case "03": return "GeneralDate";
                case "07": return "Step";
                case "09": return "Calorie";
                case "08": return "HeartRate";
                case "06": return "Time";
                default: return string.Empty;
            }
        }


        private string? watchTitle;

        public string? WatchTitle
        {
            get => watchTitle;
            set => SetProperty(ref watchTitle, value);
        }

    }
}
