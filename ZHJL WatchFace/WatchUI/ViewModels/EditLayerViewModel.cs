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
using WatchDB;
using WatchBasic.Event;

namespace WatchUI.ViewModels
{
    public class EditLayerViewModel : BindableBase, IDialogAware
    {
        private readonly IEventAggregator _eventaggregator;
        public EditLayerViewModel(IEventAggregator eventaggregator)
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

        }
        public DelegateCommand Canccel { get; set; }
        public DelegateCommand Confim { get; set; }
        public string? Title { get; set; }

        public event Action<IDialogResult>? RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }

        void SetImageLocation(LayerGroup? group)
        {
            if (ShowGroup != null)
            {
                ShowGroup.SetLocation(group?.ShowLayer?.Left ?? 0, group?.ShowLayer?.Top ?? 0, true);
                if (group?.ShowLayer?.ColorDesc != null && ShowGroup.ShowLayer != null&&(ShowGroup.ShowLayer.IsPng??false)&&(!ShowGroup.ShowLayer.NotDye))
                    ShowGroup.ShowLayer.ColorDesc = group.ShowLayer.ColorDesc;
                if (!(group?.GroupName?.Contains("时间") ?? false))
                {
                    _eventaggregator.GetEvent<WatchItemLayerPointEvent>().Publish(new WatchItemLayerPointasync
                    {
                        PointerCode = TimeCode,
                        GroupCode = group?.GroupCode,
                        Left = group?.Left ?? 0,
                        Top = group?.Top ?? 0,
                        ColorDesc = (group?.ShowLayer?.IsPng ?? false) ? group?.ShowLayer?.ColorDesc : String.Empty,
                    });
                }
                else
                {
                    _eventaggregator.GetEvent<TimeGroupEvent>().Publish(new TimeGroupasync
                    {
                        TimeCode = TimeCode,
                        GroupCode = group?.GroupCode,
                        Left = group?.Left ?? 0,
                        Top = group?.Top ?? 0,
                        ColorDesc = (group?.ShowLayer?.IsPng ?? false) ? group?.ShowLayer?.ColorDesc : String.Empty,
                    });
                }
            }
            _eventaggregator.GetEvent<RefreshEvent>().Publish(new RefreshEventInfo { GroupCode = group?.GroupCode, ColorDesc = group?.ShowLayer?.ColorDesc,IsDiy=IsDiy });
            if (group.GroupCode == LayerTool.GetHeartRateNumCode(IsDiy))
            {
                _eventaggregator.GetEvent<HeartRateRefreshEvent>().Publish();
            }
        }

        void SaveData()
        {
            SetImageLocation(Group);
            DialogParameters pairs = new DialogParameters
            {
                {"WathcType",  this.WathcType},
                {"GroupCode",this.Group.GroupCode }
            };
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK, pairs));
        }

        private LayerGroup? group;

        public LayerGroup? Group
        {
            get { return group; }
            set
            {
                SetProperty(ref group, value);
                RaisePropertyChanged(nameof(IsColorShow));
            }
        }

        public LayerGroup? ShowGroup { get; set; }

        private string? imgPath;

        public string? ImgPath
        {
            get { return imgPath; }
            set { SetProperty(ref imgPath, value); }
        }

        //private string? size;
        //public string? Size
        //{
        //    get => size;
        //    set { SetProperty(ref size, value); }
        //}

        public bool IsDiy { get; set; }
        public string? TimeCode { get; set; }

        public string? WathcType;

        private string? widthColor;
        public string? WidthColor
        {
            get => widthColor;
            set { SetProperty(ref widthColor, value); }
        }

        public bool IsColorShow => (Group?.ShowLayer?.IsPng ?? false)&&(!Group?.ShowLayer?.NotDye ?? false) && (LayerTool.AllowColorGroup().Contains(Group.GroupCode));

        //public Action? SetLocationFinished;


        public void OnDialogOpened(IDialogParameters parameters)
        {
            Title = "";
            Group = parameters.GetValue<LayerGroup>("Group");
            //typeItem = parameters.GetValue<ObservableCollection<LayerGroup>>("TypeItem");
            ShowGroup = parameters.GetValue<LayerGroup>("ShowGroup");
            var coordinateLayer = parameters.GetValue<Layer>("Coordinate");
            //SetLocationFinished= parameters.GetValue<Action>("SetLocationFinished");
            ImgPath = coordinateLayer?.ImgPath;
            //Size = $"{Group.ShowLayer?.Width},{Group.ShowLayer?.Height}";
            WidthColor = (Group.ShowLayer?.Width % 2) == 0 ? "#DD000000" : "Red";
            WathcType = parameters.GetValue<string>("WathcType");
            TimeCode = parameters.GetValue<string>("TimeCode");
            IsDiy= parameters.GetValue<bool>("IsDiy");
            RaisePropertyChanged(nameof(IsColorShow));
        }

        public void OnDialogClosed()
        {

        }
    }
}
