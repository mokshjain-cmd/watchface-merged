using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchBasic.UIBasic;
using WatchUI.UI;

namespace WatchUI.ViewModels
{

    public class ChooseItemModel : BindableBase, IDialogAware
    {
        private readonly IEventAggregator _eventaggregator;
        public ChooseItemModel(IEventAggregator eventaggregator)
        {
            _eventaggregator = eventaggregator;
            Canccel = new DelegateCommand(() =>
            {
                RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
            });
            Confim = new DelegateCommand<System.Collections.IList>((o) =>
            {
                var list = o.Cast<WatchTypeItem>;
                foreach (var item in list.Invoke())
                {
                    UI!.WatchTypeLocationItems!.Add(new WatchTypeLocationItem(item?.ShowGroups!, UI.Left, UI.Top, updateDefault)
                    {
                        WatchTitle = item?.WatchTitle,
                        DesignSketch = item?.DesignSketch,
                        Code = item?.Code,
                        WatchType = item?.WatchType,
                        LocationName = UI.LocationName,
                    });
                }
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
                // SaveData();
            });
        }
        public DelegateCommand Canccel { get; set; }
        public DelegateCommand<System.Collections.IList> Confim { get; set; }
        public string? Title { get; set; }

        public event Action<IDialogResult>? RequestClose;

        public Action<string?, string?, bool>? updateDefault;

        public bool CanCloseDialog()
        {
            return true;
        }

        void SetImageLocation(LocationUI? uI)
        {
            //if (ShowGroup != null)
            //{
            //    ShowGroup.SetLocation(uI?.ShowLayer?.Left ?? 0, uI?.ShowLayer?.Top ?? 0, true);
            //    //if (group?.ShowLayer?.ColorDesc != null && ShowGroup.ShowLayer != null)
            //    //    ShowGroup.ShowLayer.ColorDesc = group.ShowLayer.ColorDesc;
            //    //_eventaggregator.GetEvent<WatchItemLayerPointEvent>().Publish(new WatchItemLayerPointasync
            //    //{
            //    //    GroupCode = group?.GroupCode,
            //    //    Left = group?.Left ?? 0,
            //    //    Top = group?.Top ?? 0,
            //    //    ColorDesc = (group?.ShowLayer?.IsPng ?? false) ? group?.ShowLayer?.ColorDesc : String.Empty,
            //    //});
            //}
            //_eventaggregator.GetEvent<RefreshEvent>().Publish(new RefreshEventInfo { GroupCode = uI?.GroupCode, ColorDesc = uI?.ShowLayer?.ColorDesc });
            ////if (group?.GroupCode == "0804")
            ////{
            ////    _eventaggregator.GetEvent<HeartRateRefreshEvent>().Publish();
            ////}
        }

        void SaveData()
        {
            //if (SelectItem != null) 
            //{
            //    UI.WatchTypeLocationItems.Add(new WatchTypeLocationItem 
            //    {
            //      WatchTitle=SelectItem.WatchTitle,
            //      DesignSketch=SelectItem.DesignSketch,
            //      Code=SelectItem.Code,
            //      ShowGroups=SelectItem.ShowGroups,
            //      WatchType=SelectItem.WatchType,
            //      LocationName=UI.LocationName,

            //    });
            //}
            // SetImageLocation(uI);//todo
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
        }

        private LocationUI? uI;

        public LocationUI? UI
        {
            get { return uI; }
            set
            {
                SetProperty(ref uI, value);

            }
        }


        private ObservableCollection<WatchTypeItem>? typeItems;

        public ObservableCollection<WatchTypeItem>? TypeItems
        {
            get => typeItems;
            set => SetProperty(ref typeItems, value);
        }

        private WatchTypeItem selectItem;

        public WatchTypeItem SelectItem
        {
            get { return selectItem; }
            set { SetProperty(ref selectItem, value); }
        }
        public void OnDialogOpened(IDialogParameters parameters)
        {
            Title = "";
            var tempItems = parameters.GetValue<ObservableCollection<WatchTypeItem>>("TypeItems");
            updateDefault = parameters.GetValue<Action<string?, string?, bool>>("updateDefault");
            UI = parameters.GetValue<LocationUI>("UI");
            TypeItems = new ObservableCollection<WatchTypeItem>(tempItems.Where(i => !UI.WatchTypeLocationItems.Select(i => i.WatchTitle).Contains(i.WatchTitle)));

        }

        public void OnDialogClosed()
        {

        }
    }
}
