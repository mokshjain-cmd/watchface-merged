using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using WatchBasic;
using WatchBasic.UIBasic;

namespace WatchUI.ViewModels
{
    public class SettingsViewModel : BindableBase, IDialogAware
    {
        public SettingsViewModel(IDialogService dialogAware)
        {
            this._dialogAware = dialogAware;
        }
        IDialogService _dialogAware;
        private WatchSetting? setting;
        public WatchSetting? Setting
        {
            get => setting;
            set => SetProperty(ref setting, value);
        }

        private ObservableCollection<WatchSetting>? settings;
        public ObservableCollection<WatchSetting>? Settings
        {
            get => settings;
            set => SetProperty(ref settings, value);
        }

        public event Action<IDialogResult> RequestClose;


        public string Title => "";

        public DelegateCommand? SelectCommand { get; set; }

        public DelegateCommand? CloseCommand { get; set; }

        public DelegateCommand<WatchSetting>? DeleteCommand { get; set; }

        public DelegateCommand? AlterProjectCommnad { get; set; }
        public DelegateCommand? AddProjectCommnad { get; set; }

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

        void Save()
        {
            if (Settings != null)
            {
                var str = JsonConvert.SerializeObject(Settings);
                File.WriteAllText("WatchSetting.json", str);
            }
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            if (!File.Exists("WatchSetting.json"))
            {
                throw new Exception("未找到表盘项目文件");
            }
            var str = File.ReadAllText("WatchSetting.json");
            Settings = JsonConvert.DeserializeObject<ObservableCollection<WatchSetting>>(str);
            SelectCommand = new DelegateCommand(() =>
            {
                if (Setting != null)
                {
                    CommonDefintion.Setting = Setting;
                    RequestClose.Invoke(new DialogResult(ButtonResult.OK, new DialogParameters { { "Setting", Setting } }));
                }
                else
                {
                    MessageBox.Show("请选择项目");
                }
            });
            CloseCommand = new DelegateCommand(() =>
            {
                RequestClose.Invoke(new DialogResult(ButtonResult.No));
                //App.Current.Shutdown();
            });
            DeleteCommand = new DelegateCommand<WatchSetting>((setting) =>
            {
                if (setting != null)
                {
                    Settings?.Remove(setting);
                    Save();
                }

            });
            AddProjectCommnad = new DelegateCommand(() =>
            {
                IDialogParameters pairs = new DialogParameters();

                _dialogAware?.ShowDialog("AddSetting", pairs, args =>
                {
                    if (args.Result == ButtonResult.OK)
                    {
                        var setting = args.Parameters.GetValue<WatchSetting>("Setting");
                        Settings?.Add(setting);
                        Save();


                    }
                });
            });
            AlterProjectCommnad = new DelegateCommand(() =>
            {
                if (Setting != null)
                {
                    IDialogParameters pairs = new DialogParameters
                    {
                        { "Setting", Setting }
                    };
                    _dialogAware?.ShowDialog("AddSetting", pairs, args =>
                    {
                        if (args.Result == ButtonResult.OK)
                        {
                            var setting = args.Parameters.GetValue<WatchSetting>("Setting");
                            Setting.Height = setting.Height;
                            Setting.Width = setting.Width;
                            Setting.MaxValue = setting.MaxValue;
                            Setting.ProjectName = setting.ProjectName;
                            Setting.ThumbnailHeight = setting.ThumbnailHeight;
                            Setting.ThumbnailWidth = setting.ThumbnailWidth;
                            Setting.ThumbnailX = setting.ThumbnailX;
                            Setting.ThumbnailY = setting.ThumbnailY;
                            Save();
                        }
                    });

                }
                else
                {
                    MessageBox.Show("请选择项目");
                }
            });

        }
    }
}
