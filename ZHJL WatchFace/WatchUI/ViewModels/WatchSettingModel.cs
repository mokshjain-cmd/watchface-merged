using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchBasic.UIBasic;

namespace WatchUI.ViewModels
{
    public class WatchSettingModel : BindableBase, IDialogAware
    {
        private readonly IEventAggregator _eventaggregator;
        public WatchSettingModel(IEventAggregator eventaggregator)
        {
            _eventaggregator = eventaggregator;
            Cancel = new DelegateCommand(() =>
            {
                RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
            });
            Confim = new DelegateCommand(() =>
            {
                if (Setting != null)
                {
                    var str = JsonConvert.SerializeObject(Setting);
                    File.WriteAllText("WatchSetting.json", str);
                }
                DialogParameters pairs = new DialogParameters
            {
                {"Setting",  this.Setting}
            };
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK, pairs));

            });
        }
        public DelegateCommand Cancel { get; set; }
        public DelegateCommand Confim { get; set; }
        public string? Title { get; set; }

        public event Action<IDialogResult>? RequestClose;

        public Action<string?, string?, bool>? updateDefault;

        public bool CanCloseDialog()
        {
            return true;
        }

        private WatchSetting? setting;

        public WatchSetting? Setting
        {
            get { return setting; }
            set
            {
                SetProperty(ref setting, value);

            }
        }



        public void OnDialogOpened(IDialogParameters parameters)
        {
            Title = "";

            Setting = parameters.GetValue<WatchSetting>("Setting");


        }

        public void OnDialogClosed()
        {

        }
    }
}
