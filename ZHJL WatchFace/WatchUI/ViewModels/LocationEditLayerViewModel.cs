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
using System.Windows;
using WatchBasic.UIBasic;
using WatchBasic.Event;
using WatchUI.UI;

namespace WatchUI.ViewModels
{


    public class LocationEditLayerViewModel : BindableBase, IDialogAware
    {
        private readonly IEventAggregator _eventaggregator;
        private readonly IDialogService _dialogAware;
        public LocationEditLayerViewModel(IEventAggregator eventaggregator, IDialogService dialogAware)
        {
            _eventaggregator = eventaggregator;
            Canccel = new DelegateCommand(() =>
            {
                RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
            });
            Confim = new DelegateCommand(() =>
            {
                SaveData();
            });
            AddLocatonItemCommand = new DelegateCommand(AddLocatonItem);
            SubLocatonItemCommand = new DelegateCommand<WatchTypeLocationItem>((w) =>
            {
                if (w == null) return;
                UI.WatchTypeLocationItems.Remove(w);
            });
            _dialogAware = dialogAware;
        }
        public DelegateCommand Canccel { get; set; }
        public DelegateCommand Confim { get; set; }
        public DelegateCommand AddLocatonItemCommand { get; set; }
        public DelegateCommand<WatchTypeLocationItem> SubLocatonItemCommand { get; set; }
        public string? Title { get; set; }

        public event Action<IDialogResult>? RequestClose;

        public void AddLocatonItem()
        {
            //if (SelectLocationUI == null) return;
            if (!(TypeItems?.Any() ?? false)) return;

            IDialogParameters parameters = new DialogParameters();
            parameters.Add("TypeItems", TypeItems);
            parameters.Add("UI", UI);
            parameters.Add("updateDefault", updateDefault);
            _dialogAware.ShowDialog("ChooseItem", parameters, args =>
            {
                if (args.Result == ButtonResult.OK)
                {
                    //todo
                    //DimensionsGroup = new List<LayerGroupCheck>();
                    //foreach (var item in TypeItems)
                    //{
                    //    SetDimensionsGroup(item.ShowGroups);
                    //}
                    //SetDimensionsGroup(DimensionsGroup);

                }
            });
        }

        public bool CanCloseDialog()
        {
            return true;
        }


        void SaveData()
        {

            DialogParameters pairs = new DialogParameters();

            pairs.Add("UI", UI);
            pairs.Add("add", Add);
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK, pairs));
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

        public Layer coordinate;
        public Layer Coordinate
        {
            get { return coordinate; }
            set { SetProperty(ref coordinate, value); }
        }

        public List<string> locationNames;
        public List<string> LocationNames
        {
            get { return locationNames; }
            set { SetProperty(ref locationNames, value); }
        }
        public bool isEnable;
        public bool IsEnable
        {
            get { return isEnable; }
            set
            {
                SetProperty(ref isEnable, value);
            }
        }

        private ObservableCollection<WatchTypeItem>? typeItems;

        public ObservableCollection<WatchTypeItem>? TypeItems
        {
            get => typeItems;
            set => SetProperty(ref typeItems, value);
        }
        bool Add;
        public Action<string?, string?, bool> updateDefault;
        public void OnDialogOpened(IDialogParameters parameters)
        {
            Title = "";
            Coordinate = parameters.GetValue<Layer>("Coordinate");
            LocationNames = parameters.GetValue<List<string>>("UiNames");
            TypeItems = parameters.GetValue<ObservableCollection<WatchTypeItem>>("TypeItems");
            updateDefault = parameters.GetValue<Action<string?, string?, bool>>("updateDefault");
            UI = parameters.GetValue<LocationUI>("UI");
            if (UI == null)
            {
                UI = new LocationUI();
                UI.LocationName = LocationNames[0];//默认选中
                Add = true;
                IsEnable = true;
            }
            else
            {
                LocationNames.Add(UI.LocationName);
                IsEnable = false;
            }




        }

        public void OnDialogClosed()
        {

        }
    }
}
