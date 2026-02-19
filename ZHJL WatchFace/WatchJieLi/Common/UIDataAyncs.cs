using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchBasic.UIBasic;
using WatchDB;

namespace WatchJieLi.Common
{
    public class UIDataAyncs
    {
        public static void AsyncWatchTypeItem(BaseTypeItem? watchTypeItem, WatchInfo? watchInfo, Action? saveData)
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
                                if (i.ShowLayer.IsPng??false   && (!i.ShowLayer.NotDye))
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
                                if ((LayerTool.AllowColorGroup().Contains(i.GroupCode)) && (i.ShowLayer.IsPng??false) && (!i.ShowLayer.NotDye))
                                {
                                    datagroup.ColorDesc = i.ShowLayer.ColorDesc;
                                }
                            }
                        }


                    }
                }

            }


        }





    }
}
