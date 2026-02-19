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
    public class TimeWatch : NavigationViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        public DelegateCommand AddNumbarShowIndex { get; set; }
        public DelegateCommand SubNumbarShowIndex { get; set; }
        public TimeWatch(IContainerProvider containerProvider, IEventAggregator eventAggregator) : base(containerProvider)
        {
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<RefreshEvent>().Subscribe((info) =>
            {
                if (Numbar != null && (Numbar.LayerGroups?.Where(i => i.GroupCode == info.GroupCode).Any() ?? false))
                {
                    Numbar.LayerGroups.FirstOrDefault(i => i.GroupCode == info.GroupCode).UpdateColorDesc(info.ColorDesc);
                }
            });

        }


        private WatchShowItem? numbar;
        public WatchShowItem? Numbar
        {
            get => numbar;
            set => SetProperty(ref numbar, value);
        }

        public override void SetWatchShowItem()
        {
            Numbar = new WatchShowItem(_typeItem?.GetLayerGroupsByKey("数字")?.ToList(), _typeItem!.Refresh,_typeItem!.AsyncLocationItem, true);
            var split = _typeItem?.GetLayerGroupsByKey("分隔")?.ToList()?.FirstOrDefault();
            if (split != null)
            {
                Numbar.LayerGroups?.Add(split);
                Numbar.LayerGroups = Numbar.LayerGroups?.OrderBy(i => i.GroupCode).ToList();
                Numbar.ShowNum += 1;
            }

            AddNumbarShowIndex = new DelegateCommand(() => AddShowIndexs(Numbar));
            SubNumbarShowIndex = new DelegateCommand(() => SubShowIndexs(Numbar));
        }
    }
}
