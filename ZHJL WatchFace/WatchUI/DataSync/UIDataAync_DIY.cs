using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchBasic.UIBasic;
using WatchDB;
using WatchDB_DIY;
using WatchUI.UI;

namespace WatchUI.DataSync
{
    public class UIDataAync_DIY
    {
        public static void AsyncWatchTypeItem(WatchTypeItem? watchTypeItem, WatchInfo_DIY? watchInfo, Action? saveData)
        {
            var dataItem = watchInfo?.WatchTypes!.Where(i => i.WatchTypeName == watchTypeItem?.WatchType).FirstOrDefault();
            if (dataItem != null)
            {
                AsyncWatchGroups(dataItem, watchTypeItem?.ShowGroups?.ToList());
            }
            else
            {
                dataItem = new WatchType() { WatchTypeName = watchTypeItem?.WatchType };
                dataItem.WatchGroups = watchTypeItem?.ShowGroups?.Select(i => new WatchGroup
                {
                    GroupCode = i.GroupCode,
                    Left = i.Left,
                    Top = i.Top,
                    ColorDesc = ((LayerTool.AllowColorGroup().Contains(i.GroupCode)) && (i.ShowLayer?.IsPng ?? false)) ? i.ShowLayer?.ColorDesc : String.Empty,
                }).ToList();
                watchInfo?.WatchTypes?.Add(dataItem);

            }
            saveData?.Invoke();

        }

        public static void AsyncWatchGroups(WatchType? watchType, List<LayerGroup>? layerGroups)
        {
            if (layerGroups != null)
            {
                foreach (var i in layerGroups)
                {
                    var datagroup = watchType?.WatchGroups?.Where(w => w.GroupCode == i.GroupCode).FirstOrDefault();
                    if (datagroup == null)
                    {
                        datagroup = new WatchGroup()
                        {
                            GroupCode = i.GroupCode,
                            Left = i.Left,
                            Top = i.Top,
                            ColorDesc = ((LayerTool.AllowColorGroup().Contains(i.GroupCode)) && (i.ShowLayer?.IsPng ?? false)) ? i.ShowLayer?.ColorDesc : String.Empty,
                        };
                        watchType?.WatchGroups?.Add(datagroup);
                    }
                    else
                    {
                        i.SetLocation(datagroup.Left, datagroup.Top);
                        if (i.ShowLayer != null)
                        {
                            if (!string.IsNullOrEmpty(datagroup.ColorDesc))
                            {
                                if (i.ShowLayer.IsPng ?? false && (!i.ShowLayer.NotDye))
                                {
                                    //i.ShowLayer.ColorDesc = datagroup.ColorDesc;
                                    foreach (var item in i.Layers)
                                    {
                                        item.ColorDesc = datagroup.ColorDesc;
                                    }
                                }

                            }
                            else
                            {
                                if ((LayerTool.AllowColorGroup().Contains(i.GroupCode)) && (i.ShowLayer.IsPng ?? false) && (!i.ShowLayer.NotDye))
                                {
                                    datagroup.ColorDesc = i.ShowLayer.ColorDesc;
                                }
                            }
                        }


                    }
                }

            }


        }

        public static void AsyncWatchTime(List<TimeImage>? items, WatchInfo_DIY? watchInfo, Action? saveData)
        {
            if (items != null)
            {
                foreach (var i in items)
                {
                    var dataItem = watchInfo?.WatchTimes?.Where(w => w.TimeCode == i.TimeCode).FirstOrDefault();
                    if (dataItem == null)
                    {
                        dataItem = new WatchTime() { TimeCode = i.TimeCode };
                        dataItem.TimeGroups = i.ShowGroups?.Select(l => new TimeGroup
                        {
                            GroupCode = l.GroupCode,
                            Left = l.Left,
                            Top = l.Top,
                            ColorDesc = l.ShowLayer?.ColorDesc,
                        }).ToList();

                        watchInfo?.WatchTimes?.Add(dataItem);
                    }
                    else
                    {

                        if (dataItem?.TimeGroups != null)
                        {
                            AsyncTimeGroups(dataItem, i.ShowGroups?.ToList());

                        }
                    }

                }
                saveData?.Invoke();


            }
        }

        public static void AsyncTimeGroups(WatchTime? watchTime, List<LayerGroup>? layerGroups)
        {
            if (layerGroups != null)
            {
                foreach (var i in layerGroups)
                {
                    var datagroup = watchTime?.TimeGroups?.Where(w => w.GroupCode == i.GroupCode).FirstOrDefault();
                    if (datagroup == null)
                    {
                        datagroup = new TimeGroup()
                        {
                            GroupCode = i.GroupCode,
                            Left = i.Left,
                            Top = i.Top,
                            ColorDesc = i.ShowLayer?.ColorDesc,
                        };
                        watchTime?.TimeGroups?.Add(datagroup);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(datagroup.ColorDesc) && (i?.ShowLayer?.IsPng ?? false))
                        {
                            datagroup.ColorDesc = i.ShowLayer.ColorDesc;
                        }
                        else
                        {
                            i.UpdateColorDesc(datagroup.ColorDesc);
                        }
                        i.SetLocation(datagroup.Left, datagroup.Top);

                    }

                }
                var needless = watchTime?.TimeGroups?.Select(i => i.GroupCode).Except(layerGroups.Where(i => i.Layers.Count > 0).Select(i => i.GroupCode));
                if (needless.Any())
                {
                    foreach (var i in needless)
                    {
                        watchTime?.TimeGroups?.Remove(watchTime?.TimeGroups?.Where(i => needless.Contains(i.GroupCode)).FirstOrDefault());
                    }
                }


            }
        }


        public static void AsyncLocation(IEnumerable<LocationUI> locationUIs, IEnumerable<WatchTypeItem> watchTypeItems, WatchInfo_DIY? watchInfo, decimal ratio, Action<string?, string?, bool> updateDefalut)
        {
            if (watchInfo?.WatchLocations?.Any() ?? false)
            {
                foreach (var l in watchInfo.WatchLocations!)
                {
                    LocationUI location = new LocationUI
                    {
                        Left = l.Left,
                        Top = l.Top,
                        LocationName = l.LocationName,
                    };
                    var items = from d in l.LocationDetails
                                join t in watchInfo.WatchTypes! on d.WatchTypeId equals t.ID
                                join w in watchTypeItems on t.WatchTypeName equals w.WatchType
                                select new WatchTypeLocationItem(w?.ShowGroups!, location.Left, location.Top, updateDefalut)
                                {
                                    WatchTitle = w.WatchTitle,
                                    DesignSketch = w.DesignSketch,
                                    Code = w.Code,
                                    LocationName = l.LocationName,
                                    WatchType = w?.WatchType,
                                    IsDefault = d.IsDefalut == 1 ? true : false,
                                };
                    foreach (var i in items)
                    {
                        foreach (var s in i.ShowGroups.Where(i => LayerTool.GetHeartRateNullCode(true).Contains(i.GroupCode)))
                        {
                            s.IsShow = false;
                        }
                        i.SetLayerRatio(ratio);
                        location.WatchTypeLocationItems!.Add(i);
                        if (i.IsDefault)
                            location.SelectItem = i;
                    }

                    var Uis = locationUIs as ObservableCollection<LocationUI>;
                    Uis.Add(location);
                }
            }

        }


    }
}
