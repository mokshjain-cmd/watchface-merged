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
using WatchBasic.Language;
using WatchBasic.UIBasic;
using WatchUI.CreateBin;
using ZHAPI;

namespace WatchUI.ViewModels
{
    //internal class EjectPageViewModel
    //{
    //}
    public class EjectPageViewModel : BindableBase, IDialogAware
    {
        public EjectPageViewModel()
        {
            Canccel = new DelegateCommand(() =>
            {
                RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
            });
            Confim = new DelegateCommand<System.Collections.IList>((c) =>
            {
                var list = c.Cast<PreviewModel>;
                ChooseCatalogues = list.Invoke();
                CatalogueClosed();
            });

            CancelUpLoadCommand = new DelegateCommand<string>(async (str) =>
            {
                await WatchAPI.CancelUploadAsync(str, Convert.ToInt32(deviceType));
                GetCatalogueList(deviceType);
                MessageBox.Show("取消成功");
            });
        }
        public DelegateCommand Canccel { get; set; }
        public DelegateCommand<System.Collections.IList> Confim { get; set; }

        public DelegateCommand<string> CancelUpLoadCommand { get; set; }

        //public DelegateCommand<string> SearchCommand { get; set; }
        public string Title => "";

        public event Action<IDialogResult> RequestClose;
        public IEnumerable<string> Folders { get; set; }
        public IEnumerable<PreviewModel> ChooseCatalogues { get; set; }
        private ObservableCollection<PreviewModel> catalogues;
        public ObservableCollection<PreviewModel> Catalogues
        {
            get { return catalogues; }
            set { SetProperty(ref catalogues, value); }
        }

        //private CatalogueModel? selectedCatalogue;

        //public CatalogueModel? SelectedCatalogue
        //{
        //    get { return selectedCatalogue; }
        //    set { SetProperty(ref selectedCatalogue, value); }
        //}

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }
        void CatalogueClosed()
        {
            DialogParameters pairs = new DialogParameters
            {
                {"Choose", ChooseCatalogues }
            };
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK, pairs));
        }

        string deviceType { get; set; }
        string projectName { get; set; }
        public void OnDialogOpened(IDialogParameters parameters)
        {
            Folders = parameters.GetValue<IEnumerable<string>>("Folders");
            var title = parameters.GetValue<string>("Title");
            deviceType = parameters.GetValue<string>("deviceType");
            projectName = parameters.GetValue<string>("projectName");
            GetCatalogueList(deviceType);
        }
        void GetCatalogueList(string deviceType)
        {
            Catalogues = new ObservableCollection<PreviewModel>();
            var Ids = WatchAPI.SearchWatch(deviceType);
            var bathInfo = Folders.Select(i => new
            {
                Sub = Directory.GetDirectories(i).Select(j => new
                {
                    FolderPath = i,
                    Lang = Path.GetFileName(j),
                })
            }).SelectMany(i => i.Sub);
            LanguageFactory factory = new LanguageFactory(projectName);
            var list = from p in
                       (from item in bathInfo
                        select new
                        {
                            FolderPath = item.FolderPath,
                            Title = Path.GetFileName(item.FolderPath),
                            WatchId = WatchTool.GetWatchId(factory, item.Lang, Path.GetFileName(item.FolderPath)),
                            Lang = item.Lang,
                        })
                       join id in Ids on p.WatchId equals id into temp
                       from t in temp.DefaultIfEmpty()
                       select new PreviewModel
                       {
                           FolderPath = p.FolderPath,
                           BitmapSource = LayerTool.GetBatchLayerGroups(p.FolderPath)?.BitmapSource,
                           Title = p.Title,
                           WatchId = p.WatchId,
                           IsUpload = !string.IsNullOrEmpty(t),
                           Lang = p.Lang,
                           Layer= LayerTool.GetBatchLayerGroups(p.FolderPath),
                       };

            Catalogues.AddRange(new ObservableCollection<PreviewModel>(list));

        }

    }
}
