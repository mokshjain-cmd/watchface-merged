using Prism.Events;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchBasic.Event;
using WatchUI.UI;
using WatchUI.ViewModels;

namespace WatchUI.ViewModels
{
    public class HeartRateWatch : BaseWatch
    {
        public HeartRateWatch(IContainerProvider containerProvider, IEventAggregator eventAggregator) : base(containerProvider, eventAggregator)
        {
            eventAggregator.GetEvent<RefreshEvent>().Subscribe((info) =>
            {
                if (info.GroupCode == "0804")
                {
                    if (NullNum != null)
                    {
                        NullNum.UpdateColorDesc(info.ColorDesc);
                    }
                }
            });
        }

        private bool isOpen;

        public bool IsOpen
        {
            get => isOpen;
            set
            {
                SetProperty(ref isOpen, value);
                OpenMethod();
            }
        }

        void OpenMethod()
        {
            if (IsOpen)
            {
                if (NullNum != null)
                {
                    NullNum.IsShow(false);
                }

                if (Numbar != null)
                {
                    // Numbar.IsShow(true);
                    Numbar.ShowLayer(false);
                    Numbar.IsEnable = true;
                    
                }

            }
            else
            {
                if (NullNum != null)
                {
                    NullNum.IsShow(true);
                }
                if (Numbar != null)
                {
                    Numbar.IsShow(false);
                    Numbar.IsEnable = false;

                }
            }
            _typeItem!.AsyncLocationItem();
            _typeItem!.Refresh();
        }


        private bool textBlockEnable;

        public bool TextBlockEnable
        {
            get => textBlockEnable;
            set { SetProperty(ref textBlockEnable, value); }
        }

        private WatchShowItem? nullNum;

        public WatchShowItem? NullNum
        {
            get => nullNum;
            set => SetProperty(ref nullNum, value);
        }

        public override void SetWatchShowItem()
        {
            base.SetWatchShowItem();
            NullNum = new WatchShowItem(_typeItem?.GetLayerGroupsByKey("无数据")?.ToList(), _typeItem!.Refresh, _typeItem!.AsyncLocationItem);
            IsOpen = false;
            eventAggregator.GetEvent<HeartRateRefreshEvent>().Subscribe(() =>
            {
                
                if (NullNum.LayerGroups != null)
                {
                   
                    foreach (var group in NullNum.LayerGroups)
                    {
                        group.SetLocation(Numbar.InitPoint.X, Numbar.InitPoint.Y, true);
                    }
                }

                OpenMethod();
            });
            TextBlockEnable = (Numbar.LayerNum == 0 ? false : true) && (Numbar.IsEnable == true ? true : true);

        }

    }
}
