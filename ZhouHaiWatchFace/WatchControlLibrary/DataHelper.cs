using MiniExcelLibs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WatchControlLibrary;
using WatchControlLibrary.Model;
using MessageBox = System.Windows.MessageBox;

namespace WatchControlLibrary
{
    public  class DataHelper
    {
        public static IEnumerable<WatchSetting>? WatchSettings => MiniExcel.Query<WatchSetting>($@"{AppDomain.CurrentDomain.BaseDirectory}\data\WatchFace.xlsx");

        
        
        


        public static WatchSetting? GetWatchSettingByItemName(string itemName)
        {
            return WatchSettings?.FirstOrDefault(x => x.ItemName == itemName);
        }

        public static DragBindBase? GetDragBindBase(string folderName, WatchSetting? setting, string? dragName, IEnumerable<string>? itemSource)
        {
            if (setting.ControlType == "DragImage")
            {
                itemSource = itemSource.Select(x => ImageCache.GetImage(folderName, x)).ToList();
            }
            else
            {
                if ((setting?.ControlType == "DragSwitch" || setting?.ControlType == "DragAMPM") && itemSource.Count() < 2)
                {
                    MessageBox.Show("Please select 2 or more images");
                    return null;
                }
                
                itemSource = itemSource.Select(x => ImageCache.GetEzipImage(folderName, x)).ToList();
            }

            return setting?.ControlType switch
            {
                "DragNormalDateTime" => new DragBindNormalDateTime { DragName = dragName, ImageSource = new ObservableCollection<string>(itemSource), ElementType = setting.BindMonitorType, DateTimeFormat = setting.ItemName,ItemName=setting.ItemName },
                "DragSingleDigit" => new DragBindSingleDigit { DragName = dragName, ImageSource = new ObservableCollection<string>(itemSource), ElementType = setting.BindMonitorType, DateTimeFormat = setting.ItemName, ItemName = setting.ItemName },
                "DragMonthDay" => new DragBindMonthDay { DragName = dragName, ImageSource = new ObservableCollection<string>(itemSource), ElementType = setting.BindMonitorType, ItemName = setting.ItemName,MonthDayMode= MonthDayModeEnum.split,LeadingZero=true },
                "DragWeek" => new DragBindWeek { DragName = dragName, ImageSource = new ObservableCollection<string>(itemSource), ElementType = setting.BindMonitorType, DateTimeType = DateTimeTypes[setting.ItemName], ItemName = setting.ItemName },
                "DragNums" => new DragBindNums(System.Convert.ToInt32(setting.MaxNum ?? 0), System.Convert.ToInt32(setting.MinNum ?? 0), setting.Default ?? 0) { DragName = dragName, ImageSource = new ObservableCollection<string>(itemSource), ElementType = setting.BindMonitorType, ItemName = setting.ItemName,DefaultNum=setting.Default },
                "DragProgress" => new DragBindProgress(System.Convert.ToInt32(setting.MaxNum ?? 0), System.Convert.ToInt32(setting.MinNum ?? 0), setting.Default ?? 0) { DragName = dragName, ImageSource = new ObservableCollection<string>(itemSource), ElementType = setting.BindMonitorType, ItemName = setting.ItemName,DefaultNum=setting.Default,TargetValue= System.Convert.ToInt32(setting.TargetValue ?? 0) },
                "DragSwitch" => new DragBindSwitch { DragName = dragName, OpenSource = itemSource.FirstOrDefault(), CloseSource = itemSource.ElementAt(1), ElementType = setting.BindMonitorType, ItemName = setting.ItemName },
                "DragAMPM" => new DragBindAMPM { DragName = dragName, AMSource = itemSource.FirstOrDefault(), PMSource = itemSource.ElementAt(1), ElementType = setting.BindMonitorType, ItemName = setting.ItemName },
                "DragDouble" => new DragBindDouble(setting.MaxNum ?? 0, setting.MinNum ?? 0, setting.Default ?? 0)
                { DragName = dragName, ImageSource = new ObservableCollection<string>(itemSource), ElementType = setting.BindMonitorType, 
                  ItemName = setting.ItemName,DefaultNum= setting.Default,
                  DecimalPlaces=GetDecimalPlaces(setting.ItemName),
                },
                "DragKeyValue" => new DragBindKeyValue(StaticData.ToolKeyValuesFactory(setting.ItemName), itemSource) { DragName = dragName, /*ImageSource = new ObservableCollection<string>(itemSource),*/ ElementType = setting.BindMonitorType, ItemName = setting.ItemName,CurrentNum=System.Convert.ToInt32(setting.Default??0),DefaultNum=setting.Default },
                "DragImage" => new DragBindImage { DragName = dragName, Source = itemSource.FirstOrDefault(), ItemName = setting.ItemName },
                //"DragPoint" => new DragBindPoint(itemSource.FirstOrDefault()) { DragName = dragName,Source = itemSource.FirstOrDefault(), ItemName = setting.ItemName },
                "DragPoint" => new DragBindPoint(itemSource.FirstOrDefault()) { DragName = dragName, ItemName = setting.ItemName },
                "DragAnimFrame" => new DragBindAnimFrame { DragName = dragName, ImageSource = new ObservableCollection<string>(itemSource), ItemName = setting.ItemName, ElementType = setting.BindMonitorType, RepeatCount = 1, FrameRate = 30 },
                _ => throw new Exception("Unknown type")
            };


        }

        public static int GetDecimalPlaces(string itemName) 
        {
            return itemName switch 
            {
                "Sleep Duration" or "Step Distance" => 2,
                _=>1,
            };
        }

        public static Dictionary<string, DateTimeType> DateTimeTypes => new Dictionary<string, DateTimeType>
        {
            {"Week",DateTimeType.Week },
            {"Month (Normal Fill)",DateTimeType.Month },
            {"Day (Normal Fill)",DateTimeType.Day },
            {"Hour (Normal Fill)",DateTimeType.Hour },
        };

    }


    public class WatchSetting
    {
        public string? ItemName { get; set; }

        public string? ControlType { get; set; }

        public BindMonitorType? BindMonitorType { get; set; }

        public double? MaxNum { get; set; }

        public double? MinNum { get; set; }

        public double? Default { get; set; }

        public double? TargetValue { get; set;}
    
    }
}
