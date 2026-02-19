using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchBasic.Event;
using WatchBasic.UIBasic;
using WatchUI.UI;

namespace WatchUI.ViewModels
{
    public class BaseWatch : NavigationViewModel
    {
        private readonly IEventAggregator? _eventAggregator;
        public DelegateCommand AddNumbarShowIndex { get; set; }
        public DelegateCommand SubNumbarShowIndex { get; set; }
        public DelegateCommand AddProgressbarShowIndex { get; set; }
        public DelegateCommand SubProgressbarShowIndex { get; set; }

        public BaseWatch(IContainerProvider containerProvider, IEventAggregator eventAggregator) : base(containerProvider)
        {

            _eventAggregator = eventAggregator;
            ShowNumCommnad = new DelegateCommand<string>((showNum) =>
            {
                if (Numbar != null)
                    Numbar.ShowNum = Convert.ToInt32(showNum);
            });
            _eventAggregator.GetEvent<RefreshEvent>().Subscribe((info) =>
            {
                if (Numbar != null && (bool)Numbar?.LayerGroups.Where(i => i.GroupCode == info.GroupCode).Any())
                {
                    int num = Numbar.ShowNum;
                    var align = Numbar.Align;
                    Numbar.InitPoint = new System.Drawing.Point(Numbar.LayerGroups.FirstOrDefault()!.Left, Numbar.LayerGroups.FirstOrDefault()!.Top);
                    Numbar.ShowNum = Numbar.LayerGroups.Count;
                    Numbar.Align = 0;
                    Numbar?.RestLocation(true);
                    Numbar?.UpdateColorDesc(info.ColorDesc);
                    Numbar.ShowNum = num;
                    Numbar.Align = align;
                    if (info.ColorDescs != null)
                    {
                        foreach (var item in Numbar.LayerGroups)
                        {
                            item.ShowLayer.ColorDescs = info.ColorDescs;
                            _eventAggregator.GetEvent<SetWatchGroupColorArrayEvent>().Publish(new SetWatchGroupColorArray
                            {
                                GroupCode = item.GroupCode,
                                ColorDescs = info.ColorDescs
                            });
                        }
                    }
                }
            });
        }

        public BaseWatch(IContainerProvider containerProvider) : base(containerProvider)
        {
        }

        private WatchShowItem? progressbar;

        public WatchShowItem? Progressbar
        {
            get => progressbar;
            set => SetProperty(ref progressbar, value);
        }

        public DelegateCommand<string>? ShowNumCommnad { get; set; }

        private WatchShowItem? numbar;

        public WatchShowItem? Numbar
        {
            get => numbar;
            set => SetProperty(ref numbar, value);
        }

        public override void SetWatchShowItem()
        {
            var list = _typeItem?.GetLayerGroupsByKey("数字")?.ToList();
            //int pointX = list.FirstOrDefault().Left;
            //if (Numbar != null) { pointX = Numbar.InitPoint.X; }
            Progressbar = new WatchShowItem(_typeItem?.GetLayerGroupsByKey("进度条")?.ToList(), _typeItem!.Refresh, _typeItem!.AsyncLocationItem);
            //Numbar = new WatchShowItem(_typeItem?.GetLayerGroupsByKey("数字")?.ToList(), _typeItem!.Refresh);
            Numbar = new WatchShowItem(list, _typeItem!.Refresh, _typeItem!.AsyncLocationItem);
            //Numbar.InitPoint.X = pointX;
            Numbar.ShowLayer(false);
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
            AddProgressbarShowIndex = new DelegateCommand(() => AddShowIndexs(Progressbar));
            SubProgressbarShowIndex = new DelegateCommand(() => SubShowIndexs(Progressbar));

        }
    }
}
