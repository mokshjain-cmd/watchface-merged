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
using WatchBasic;
using WatchUI.CreateBin;
using WatchUI.UI;

namespace WatchUI.ViewModels
{
    public class InformationViewModel : BindableBase, IDialogAware
    {
        public InformationViewModel()
        {
            Confim = new DelegateCommand(() =>
            {
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
            });
        }
        public DelegateCommand Confim { get; set; }
        private string message;

        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        private string[] textArray;

        public string[] TextArray
        {
            get { return textArray; }
            set { SetProperty(ref textArray, value); }
        }

        private ObservableCollection<WatchDesc>? watchDescs;

        public ObservableCollection<WatchDesc>? WatchDescs
        {
            get { return watchDescs; }
            set { SetProperty(ref watchDescs, value); }
        }
        public string Title { get; set; }
        public string Desc { get; set; }

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            Title = "";
            if (parameters.GetValue<string>("FolderPath") != null)
            {
                WatchDescs = new ObservableCollection<WatchDesc>();
                Message = parameters.GetValue<string>("FolderPath") + "\\辅助文件\\表盘信息.txt";
                //TextArray = ReadTxTContent(Message + "\\表盘信息.txt").ToArray();
                var watchDescs = parameters.GetValue<ObservableCollection<WatchDesc>>("WatchDescs");
                foreach (var watchDesc in watchDescs)
                {
                    WatchDescs.Add(new WatchDesc
                    {
                        Language = watchDesc.Language,
                        Description = watchDesc.Description.Split(":")[1],
                        WatchName = watchDesc.WatchName.Split(":")[1],
                    });
                }
                string res = ReadLast(Message);
                if (res != string.Empty)
                {
                    Desc = res.Split(":")[1];
                }
            }
        }


        private string ReadLast(string path)
        {

            if (File.Exists(path))
            {
                var str = File.ReadAllLines(path);
                if (str.Contains("#=====表盘分类====="))
                {
                    return str.Where(i => i != "").Last();
                }
            }
            return  string.Empty;
        }
    }
}
