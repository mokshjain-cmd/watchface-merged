using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchBasic.UIBasic;

namespace WatchUI.ViewModels
{
    public class AddSettingModel : BindableBase, IDialogAware
    {
        private WatchSetting? setting;
        public WatchSetting? Setting
        {
            get => setting;
            set => SetProperty(ref setting, value);
        }


        public event Action<IDialogResult>? RequestClose;

        public string Title => "";

        public DelegateCommand? ConfirmCommand { get; set; }

        public DelegateCommand? CloseCommand { get; set; }



        public bool CanCloseDialog()
        {
            return true;
            // throw new NotImplementedException();
        }

        public void OnDialogClosed()
        {

            // RequestClose.Invoke(new DialogResult(ButtonResult.OK));
            // throw new NotImplementedException();
        }



        public void OnDialogOpened(IDialogParameters parameters)
        {

            if (parameters.Count == 0)
                Setting = new WatchSetting();
            else
            {
                var setting = parameters.GetValue<WatchSetting>("Setting");
                Setting = new WatchSetting
                {
                    Height = setting.Height,
                    Width = setting.Width,
                    MaxValue = setting.MaxValue,
                    ProjectName = setting.ProjectName,
                    ThumbnailHeight = setting.ThumbnailHeight,
                    ThumbnailWidth = setting.ThumbnailWidth,
                    ThumbnailX = setting.ThumbnailX,
                    ThumbnailY = setting.ThumbnailY,
                };
            }

            ConfirmCommand = new DelegateCommand(() =>
            {
                if (Setting != null)
                {
                    if (string.IsNullOrEmpty(Setting.ProjectName))
                    {
                        throw new Exception("项目不能为空");
                    }
                    IDialogParameters parameters = new DialogParameters();
                    parameters.Add("Setting", Setting);
                    RequestClose?.Invoke(new DialogResult(ButtonResult.OK, parameters));
                }

            });
            CloseCommand = new DelegateCommand(() =>
            {
                RequestClose?.Invoke(new DialogResult(ButtonResult.No));
                //App.Current.Shutdown();
            });
            //DeleteCommand = new DelegateCommand(() =>
            //{

            //});
        }
    }
}
