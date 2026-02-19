using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchBasic.Event;
using WatchUI.UI;

namespace WatchUI.ViewModels
{
    public class KwhWatch : BaseWatch
    {
        public KwhWatch(IContainerProvider containerProvider, IEventAggregator eventAggregator) : base(containerProvider, eventAggregator)
        {

        }
        public override void SetWatchShowItem()
        {
            base.SetWatchShowItem();
            Numbar = new WatchShowItem(_typeItem?.GetLayerGroupsByKey("数字")?.ToList(), _typeItem!.Refresh, _typeItem!.AsyncLocationItem, false);
            var pah = _typeItem?.GetLayerGroupsByKey("百分号")?.ToList()?.FirstOrDefault();
            if (pah != null)
            {
                pah.LongShow = true;
                Numbar.LayerGroups?.Add(pah);
                // Numbar.LayerGroups = Numbar.LayerGroups?.OrderBy(i => i.GroupCode).ToList();
                // Numbar.ShowNum += 1;
            }
            Numbar.PublishEvent += (align) =>
            {
                eventAggregator.GetEvent<WatchTypeAlignEvent>().Publish(new WatchTypeAlignasync
                {
                    //WatchType=(_typeItem as WatchTypeItem).WatchType,
                    WatchType = _typeItem.WatchType,
                    Align = align
                });
            };
            var dataType = _watchInfo?.WatchTypes?.Where(i => i.WatchTypeName == _typeItem.WatchType).FirstOrDefault();//数据同步
            if (dataType != null)
            {
                Numbar.Align = dataType.Align;
            }
            AddNumbarShowIndex = new DelegateCommand(() => AddShowIndexs(Numbar));
            SubNumbarShowIndex = new DelegateCommand(() => SubShowIndexs(Numbar));
            //var blueBoothGroups = _typeItem?.InitGroups?.Where(i => i.GroupCode == "0102")?.FirstOrDefault();
            //_typeItem.ShowGroups.Add(blueBoothGroups);
            BlueTooth = new WatchShowItem(_typeItem?.InitGroups?.Where(i => i.GroupCode == "0102")?.ToList(), _typeItem!.Refresh, _typeItem!.AsyncLocationItem, false);
        }

        /// <summary>
        /// 蓝牙
        /// </summary>
        private WatchShowItem? blueTooth;

        public WatchShowItem? BlueTooth
        {
            get => blueTooth;
            set => SetProperty(ref blueTooth, value);
        }

        private bool isConncented;

        public bool IsConncented
        {
            get => isConncented;
            set
            {
                SetProperty(ref isConncented, value);
                if (BlueTooth != null && BlueTooth?.LayerGroups != null)
                {
                    foreach (var info in BlueTooth?.LayerGroups!)
                    {
                        if (IsConncented)
                        {
                            info.ShowIndex += 1;
                        }
                        else
                        {
                            if (info.ShowIndex != 0)
                            {
                                info.ShowIndex -= 1;
                            }
                        }
                    }
                    BlueTooth.RefreshLayer?.Invoke();
                }


            }
        }


    }
}
