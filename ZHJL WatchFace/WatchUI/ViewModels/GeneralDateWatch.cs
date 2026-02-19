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
    public class GeneralDateWatch : NavigationViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        public DelegateCommand AddWeekbarShowIndex { get; set; }
        public DelegateCommand SubWeekbarShowIndex { get; set; }
        public DelegateCommand AddDatebarShowIndex { get; set; }
        public DelegateCommand SubDatebarShowIndex { get; set; }
        public GeneralDateWatch(IContainerProvider containerProvider, IEventAggregator eventAggregator) : base(containerProvider)
        {
            _eventAggregator = eventAggregator;
        }

        public override void SetWatchShowItem()
        {
            Weekbar = new WatchShowItem(_typeItem?.GetLayerGroupsByKey("星期")?.ToList(), _typeItem!.Refresh,_typeItem!.AsyncLocationItem);
            //Weekbar.LayerNum = 7;
           // Datebar = new WatchShowItem(_typeItem?.GetLayerGroupsByKey("数字")?.ToList(), _typeItem!.Refresh);
            Monthbar = new WatchShowItem(_typeItem?.GetLayerGroupsByKey("(月)")?.ToList(), _typeItem!.Refresh, _typeItem!.AsyncLocationItem);
            Daybar = new WatchShowItem(_typeItem?.GetLayerGroupsByKey("(日)")?.ToList(), _typeItem!.Refresh, _typeItem!.AsyncLocationItem);

            //var split = _typeItem?.GetLayerGroupsByKey("分隔")?.ToList()?.FirstOrDefault();
            //if (split != null)
            //{
            //    Datebar.LayerGroups?.Add(split);
            //    Datebar.LayerGroups = Datebar.LayerGroups?.OrderBy(i => i.GroupCode).ToList();
            //    Datebar.ShowNum += 1;
            //}
            TimeSpanbar = new WatchShowItem(_typeItem?.GetLayerGroupsByKey("上下午")?.ToList(), _typeItem!.Refresh, _typeItem!.AsyncLocationItem);

            AddWeekbarShowIndex = new DelegateCommand(() => AddShowIndexs(Weekbar));
            SubWeekbarShowIndex = new DelegateCommand(() => SubShowIndexs(Weekbar));
            AddDatebarShowIndex = new DelegateCommand(() =>
            {
                if (Monthbar.IsEnable)
                {
                    AddShowIndexs(Monthbar);

                }
                if (Daybar.IsEnable)
                {

                    AddShowIndexs(Daybar);
                }
                if (ShowIndex < LayerNum)
                {
                    ShowIndex += 1;
                    RaisePropertyChanged(nameof(BarDescr));
                }

            });
            SubDatebarShowIndex = new DelegateCommand(() =>
            {
                if (Monthbar.IsEnable)
                {
                    SubShowIndexs(Monthbar);
                }
                if (Daybar.IsEnable)
                {
                    SubShowIndexs(Daybar);
                }
                if (ShowIndex > 1)
                {
                    ShowIndex -= 1;
                    RaisePropertyChanged(nameof(BarDescr));
                }

            });
            IsPM = false;
            TimeWay = false;
            _eventAggregator.GetEvent<RefreshEvent>().Subscribe((info) =>
            {
                if (Monthbar?.LayerGroups?.Where(i => i.GroupCode == info.GroupCode).Any() ?? false) 
                {
                    Monthbar?.RestLocation(true);
                    Monthbar?.UpdateColorDesc(info.ColorDesc);
                }
                if (Weekbar?.LayerGroups.Where(i => i.GroupCode == info.GroupCode).Any() ?? false) 
                {
                    Weekbar?.RestLocation(true);
                    Weekbar?.UpdateColorDesc(info.ColorDesc);
                }
                if (Daybar?.LayerGroups?.Where(i => i.GroupCode == info.GroupCode).Any() ?? false) 
                {
                    Daybar?.RestLocation(true);
                    Daybar?.UpdateColorDesc(info.ColorDesc);
                }
                
            });
        }

        private bool isPM;

        public bool IsPM
        {
            get => isPM;
            set
            {
                SetProperty(ref isPM, value);
                if (TimeSpanbar != null && TimeSpanbar?.LayerGroups != null)
                {
                    foreach (var info in TimeSpanbar?.LayerGroups!)
                    {
                        if (IsPM)
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
                    TimeSpanbar.RefreshLayer?.Invoke();
                }


            }
        }

        private bool timeWay;
        /// <summary>
        /// 小时制
        /// </summary>
        public bool TimeWay
        {
            get { return timeWay; }
            set 
            {
                SetProperty(ref timeWay,value);
                if (value) 
                {
                    TimeSpanbar!.IsShow(false);
                }
                else 
                {
                    TimeSpanbar!.IsShow(true);
                }
                _typeItem!.Refresh();
            }
        }




        private int showIndex;

        public int ShowIndex
        {
            get { return showIndex; }
            set
            {
                SetProperty(ref showIndex, value);
                if (Monthbar != null)
                    Monthbar.ShowIndex = value;
                if (Daybar != null)
                    Daybar.ShowIndex = value;

                RaisePropertyChanged(nameof(BarDescr));
            }
        }

        public bool IsEnable => (Monthbar?.IsEnable ?? false) || (Daybar?.IsEnable ?? false);
        public int LayerNum => IsEnable ? (Monthbar!.IsEnable ? Monthbar.LayerNum : Daybar!.LayerNum) : 0;

        public string BarDescr => IsEnable ? (Monthbar!.IsEnable ? Monthbar.BarDescr : Daybar!.BarDescr) : "1/0";


        void RefreshInfo() 
        {
            RaisePropertyChanged(nameof(IsEnable));
            RaisePropertyChanged(nameof(LayerNum));
            RaisePropertyChanged(nameof(BarDescr));
            ShowIndex = 1;
        }

        //<Slider Minimum = "1" Maximum="{Binding  Datebar.LayerNum}" Value="{Binding Datebar.ShowIndex}" Width="120" DockPanel.Dock="Right" VerticalAlignment="Center" Margin="5 0 0 0" IsEnabled="{Binding Datebar.IsEnable}"/>
        //           <TextBlock VerticalAlignment = "Center" Margin="0 0 0 0"  Text="{Binding Datebar.BarDescr}"  DockPanel.Dock="Right"/>


        private WatchShowItem? weekbar;

        public WatchShowItem? Weekbar
        {
            get => weekbar;
            set => SetProperty(ref weekbar, value);
        }


        //private WatchShowItem? datebar;

        //public WatchShowItem? Datebar
        //{
        //    get => datebar;
        //    set => SetProperty(ref datebar, value);
        //}

        private WatchShowItem? monthbar;

        public WatchShowItem? Monthbar
        {
            get => monthbar;
            set
            {
                SetProperty(ref monthbar, value);
                RefreshInfo();
            }
        }

        private WatchShowItem? daybar;

        public WatchShowItem? Daybar
        {
            get => daybar;
            set
            {
                SetProperty(ref daybar, value);
                RefreshInfo();
            }
        }

        /// <summary>
        /// 上下午
        /// </summary>
        private WatchShowItem? timeSpanbar;

        public WatchShowItem? TimeSpanbar
        {
            get => timeSpanbar;
            set => SetProperty(ref timeSpanbar, value);
        }

    }
}
