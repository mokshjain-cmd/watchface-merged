using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchBasic.UIBasic;


namespace WatchUI.ViewModels
{
    public class ThumbnailViewModel : BindableBase, IDialogAware
    {
        public ThumbnailViewModel()
        {
            Confim = new DelegateCommand(() =>
            {
                OnDialogClosed();
            });

        }
        public DelegateCommand Confim { get; set; }

        private WatchBackgroundItem? backgroundItem;

        public WatchBackgroundItem? BackgroundItem
        {
            get => backgroundItem;
            set => SetProperty(ref backgroundItem, value);
        }
        public string Title { get; set; }

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
           return true;
        }

        private ObservableCollection<Layer> thumbnails;

        public ObservableCollection<Layer> Thumbnails
        {
            get { return thumbnails; }
            set { SetProperty(ref thumbnails,value); }
        }
        public void OnDialogClosed()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
        }

        private string error;

        public string Error
        {
            get { return error; }
            set { SetProperty(ref error,value); }
        }


        public void OnDialogOpened(IDialogParameters parameters)
        {
            Title = "";
            BackgroundItem = parameters.GetValue<WatchBackgroundItem>("BackgroundItem");
            var Setting = parameters.GetValue<WatchSetting>("Setting");
            var group = BackgroundItem?.LayerGroups?.Where(i => i.GroupCode == "辅助文件").FirstOrDefault();
            if (group != null) 
            {
                Thumbnails=new ObservableCollection<Layer>(group.GetLayersByName("缩略"));
                if (Setting != null) 
                {
                    foreach (var layer in Thumbnails)
                    {
                        if (layer.Width != Setting.ThumbnailWidth||layer.Height!=Setting.ThumbnailHeight) 
                        {
                            Error = "缩略尺寸错误，请检查";
                        }
                }
                }
               
            }


            
        }
    }
}
