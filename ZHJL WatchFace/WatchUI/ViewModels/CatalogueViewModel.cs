using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using WatchBasic.UIBasic;


namespace WatchUI.ViewModels
{
    public class CatalogueViewModel : BindableBase, IDialogAware
    {
        public CatalogueViewModel()
        {
            Canccel = new DelegateCommand(() =>
            {
                RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
            });
            Confim = new DelegateCommand(() =>
            {
                if (SelectedCatalogue == null)
                {
                    MessageBox.Show("请选择目录");
                    return;
                }
                CatalogueClosed();
            });
            SearchCommand = new DelegateCommand<string>((str) =>
            {
                Catalogues = new ObservableCollection<PreviewModel>(Catalogues.Where(i => i.Title.Contains(str)));
            });
        }
        public DelegateCommand Canccel { get; set; }
        public DelegateCommand Confim { get; set; }

        public DelegateCommand<string> SearchCommand { get; set; }
        public string Title => "";

        public event Action<IDialogResult> RequestClose;
        public IEnumerable<string> Folders { get; set; }
        private ObservableCollection<PreviewModel> catalogues;

        public ObservableCollection<PreviewModel> Catalogues
        {
            get { return catalogues; }
            set { SetProperty(ref catalogues, value); }
        }

        private PreviewModel? selectedCatalogue;

        public PreviewModel? SelectedCatalogue
        {
            get { return selectedCatalogue; }
            set { SetProperty(ref selectedCatalogue, value); }
        }

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
                {"FolderPath", SelectedCatalogue.FolderPath }
            };
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK, pairs));
        }
        public void OnDialogOpened(IDialogParameters parameters)
        {
            Folders = parameters.GetValue<IEnumerable<string>>("Folders");
            var title = parameters.GetValue<string>("Title");
            GetCatalogueList(title);
        }
        void GetCatalogueList(string title)
        {
            Catalogues = new ObservableCollection<PreviewModel>();
            foreach (var item in Folders)
            {
                var a = LayerTool.GetSimpleLayerGroups(item);
                var b = a.FirstOrDefault()?.Layers?.FirstOrDefault();
                Catalogues.Add(new PreviewModel { FolderPath = item, Title = Path.GetFileName(item), BitmapSource = b.BitmapSource });
            }
            SelectedCatalogue = Catalogues?.Where(i => i.Title == title)?.FirstOrDefault();
        }
       
    }
    
}
