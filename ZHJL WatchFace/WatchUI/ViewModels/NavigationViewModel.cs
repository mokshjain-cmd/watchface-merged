using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchBasic.UIBasic;
using WatchDB;
using WatchUI.UI;

namespace WatchUI.ViewModels
{
    public abstract class NavigationViewModel : BindableBase, INavigationAware
    {
        private readonly IContainerProvider containerProvider;
        protected readonly IEventAggregator eventAggregator;
        public NavigationViewModel(IContainerProvider containerProvider)
        {
            this.containerProvider = containerProvider;
            eventAggregator = containerProvider.Resolve<IEventAggregator>();
        }
        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public abstract void SetWatchShowItem();

        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
            //this._typeItem = navigationContext.Parameters["typeItem"] as WatchTypeItem;
            this._watchInfo = navigationContext.Parameters["watchInfo"] as WatchInfo;
            this._typeItem = navigationContext.Parameters["typeItem"] as BaseTypeItem;
            this._diyInfo= navigationContext.Parameters["watchInfo"] as WatchDB_DIY.WatchInfo_DIY;
            this.FontCode= navigationContext.Parameters["FontCode"] as string;
            SetWatchShowItem();
        }

        //public WatchTypeItem? _typeItem;
        public WatchInfo? _watchInfo;
        public WatchDB_DIY.WatchInfo_DIY _diyInfo { get; set; }
        public BaseTypeItem? _typeItem;
        public string? FontCode { get; set; }
        
        // 加
        //public DelegateCommand AddShowIndex { get; set; }
        //public DelegateCommand SubShowIndex { get; set; }

        public void AddShowIndexs(WatchShowItem watchShowItem)
        {
            if (watchShowItem.ShowIndex < watchShowItem.LayerNum)
            {
                watchShowItem.ShowIndex += 1;
            }
        }
        public void SubShowIndexs(WatchShowItem watchShowItem)
        {
            if (watchShowItem.ShowIndex > 1)
            {
                watchShowItem.ShowIndex -= 1;
            }
        }

    }
}
