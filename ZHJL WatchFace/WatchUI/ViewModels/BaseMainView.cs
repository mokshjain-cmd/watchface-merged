using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WatchBasic;
using WatchBasic.Event;
using WatchBasic.UIBasic;
using WatchUI.CreateBin;
using WatchUI.UI;

namespace WatchUI.ViewModels
{
    public class BaseMainView : BindableBase
    {
        public BaseMainView(IRegionManager regionManager, IDialogService dialogAware, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _dialogAware = dialogAware;
            _eventaggregator = eventAggregator;
            WatchDescs = new ObservableCollection<WatchDesc>();
            //List<WatchSetting> watchSettings = new List<WatchSetting>() 
            //{
            //   new WatchSetting{ ProjectName="E27",Width=390, Height=450,ThumbnailWidth=262,ThumbnailHeight=303,ThumbnailX=64,ThumbnailY=74},
            //   new WatchSetting{ ProjectName="M65A",Width=240,Height=280,ThumbnailWidth=156, ThumbnailHeight=182,ThumbnailX=42,ThumbnailY=38},
            //   new WatchSetting{ ProjectName="N019",Width=466,Height=466,ThumbnailWidth=310,ThumbnailHeight=310,ThumbnailX=78,ThumbnailY=78}
            //};
            //var json= JsonConvert.SerializeObject(watchSettings);
            //File.WriteAllText("WatchSetting.json", json);
            //if (File.Exists("WatchSetting.json"))
            //{
            //    var str = File.ReadAllText("WatchSetting.json");
            //    Setting = JsonConvert.DeserializeObject<WatchBasic.UIBasic.WatchSetting>(str);
            //}

            //Setting = CommonDefintion.Setting;
        }
        #region property
        //public string ProjectName = "DIY";

        private string? basePath;

        public string? BasePath
        {
            get => basePath;
            set => SetProperty(ref basePath, value);
        }

        private decimal ratio;
        public decimal Ratio  // 定义缩放比例
        {
            get => ratio;
            set => SetProperty(ref ratio, value);
        }

        private string? watchCode;

        public string? WatchCode
        {
            get { return watchCode; }
            set { SetProperty(ref watchCode, value); }
        }

        private string? shape;

        public string? Shape
        {
            get { return shape; }
            set { SetProperty(ref shape, value); }
        }

        private string? dpi;

        public string? Dpi
        {
            get { return dpi; }
            set { SetProperty(ref dpi, value); }
        }

        private string? type;

        public string? Type
        {
            get { return type; }
            set { SetProperty(ref type, value); }
        }

        private string? author;

        public string? Author
        {
            get { return author; }
            set { SetProperty(ref author, value); }
        }

        private string? code;

        public string? Code
        {
            get { return code; }
            set { SetProperty(ref code, value); }
        }

        private ObservableCollection<WatchDesc>? watchDescs;

        public ObservableCollection<WatchDesc>? WatchDescs
        {
            get { return watchDescs; }
            set { SetProperty(ref watchDescs, value); }
        }

        private ObservableCollection<string>? errors;

        public ObservableCollection<string>? Errors
        {
            get { return errors; }
            set { SetProperty(ref errors, value); }
        }

        private ProgressInfo? proValues;
        public ProgressInfo? ProValues  // 进度条
        {
            get { return proValues; }
            set { SetProperty(ref proValues, value); }
        }


        private LanguageControl? languageControl;

        public LanguageControl? LanguageControl
        {
            get { return languageControl; }
            set { SetProperty(ref languageControl, value); }
        }

        private WatchDesc? selectDesc;

        public WatchDesc? SelectDesc
        {
            get { return selectDesc; }
            set
            {
                SetProperty(ref selectDesc, value);
                if (value != null)
                    LanguageControl?.SwithcLanguage(CommonDefintion.Setting.ProjectName, value?.Language, CommonDefintion.Setting.ProjectName == "DIY");
            }
        }

        public IEnumerable<string>? Folders { get; set; }

        private string? folderPath;
        public string? FolderPath
        {
            get { return folderPath; }
            set { SetProperty(ref folderPath, value); }
        }

        private string? batchPath;

        public string? BatchPath
        {
            get { return batchPath; }
            set { SetProperty(ref batchPath, value); }
        }

        private WatchBackgroundItem? backgroundItem;

        public WatchBackgroundItem? BackgroundItem
        {
            get => backgroundItem;
            set => SetProperty(ref backgroundItem, value);
        }

        //public WatchBasic.UIBasic.WatchSetting? Setting { get; set; }

        #endregion

        #region prism
        public IRegionManager? _regionManager;
        public IDialogService? _dialogAware;
        public IEventAggregator? _eventaggregator;
        #endregion

        #region Method
        public List<LayerGroup> GetLayerGroups(string? path, bool notBitmapSource=false)
        {
            
            if (path == null)
            {
                return new List<LayerGroup>();
            }
           
            var list = path.GetLayerGroups(notBitmapSource);
           
            foreach (var i in list)
            {
                i.asyncGroupLayer = (groupcode, left, top) =>
                {
                    _eventaggregator?.GetEvent<WatchGroupEvent>().Publish(new WatchGroupasync
                    {
                        GroupCode = groupcode,
                        Left = left,
                        Top = top,
                    });
                }; 

                i.asyncGroupLayerColor = (groupcode, colorDesc) =>
                {
                    _eventaggregator?.GetEvent<WatchGroupColorDescEvent>().Publish(new WatchGroupColorDesc
                    {
                        GroupCode = groupcode,
                        ColorDesc = colorDesc,
                    });
                };

            }
            return list.ToList();
        }

        public void SetLayerGroupsAsync(List<LayerGroup> layerGroups)
        {
            foreach (var i in layerGroups)
            {
                i.asyncGroupLayer = (groupcode, left, top) =>
                {
                    _eventaggregator?.GetEvent<WatchGroupEvent>().Publish(new WatchGroupasync
                    {
                        GroupCode = groupcode,
                        Left = left,
                        Top = top,
                    });
                };

                i.asyncGroupLayerColor = (groupcode, colorDesc) =>
                {
                    _eventaggregator?.GetEvent<WatchGroupColorDescEvent>().Publish(new WatchGroupColorDesc
                    {
                        GroupCode = groupcode,
                        ColorDesc = colorDesc,
                    });
                };

            }
        }

        public void RefreshPath()
        {
            if (File.Exists("Load.txt")) //读取记录
            {
                var temp = File.ReadAllText("Load.txt");
                if (Directory.Exists(temp))
                {
                    BasePath = temp;
                    Folders = from floder in Directory.GetDirectories(BasePath).Select(i => i).Where(i => Path.GetFileNameWithoutExtension(i).Split('_').Count() == 7)
                              select floder;
                }
            }
        }

        public void SetBasePath()
        {
            using var folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                BasePath = folderBrowserDialog.SelectedPath;
                File.WriteAllText("Load.txt", BasePath);
                Folders = from floder in Directory.GetDirectories(BasePath).Select(i => i).Where(i => Path.GetFileNameWithoutExtension(i).Split('_').Count() == 7)
                          select floder;
                //LoadImages();
            }
        }

        public void ChooseBatchPath()
        {
            using var folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var path = folderBrowserDialog.SelectedPath;
                BatchPath = path;
                return;

            }
        }


        public void SetDimensionsGroupEnable(List<LayerGroupCheck>? groups, bool IsDiy)
        {
            if (groups == null) return;
            var codes = LayerTool.NoEditGroup();
            Errors = new ObservableCollection<string>(LayerTool.CheckLayerGroup(groups, WatchDescs!.Count));
            foreach (LayerGroupCheck group in groups)
            {
                if (codes.Contains(group.GroupCode!))
                {
                    group.IsEnable = false;
                }
                else
                {
                    group.IsEnable = true;
                }

            }
        }


        public void WatchSetting()
        {
            IDialogParameters parameters = new DialogParameters();
            parameters.Add("Setting", CommonDefintion.Setting);

            _dialogAware?.ShowDialog("WatchSetting", parameters, args =>
            {
                if (args.Result == ButtonResult.OK)
                {
                    CommonDefintion.Setting = args.Parameters.GetValue<WatchSetting>("Setting");

                }
            });

        }

        #endregion

        #region command
        public DelegateCommand? SettingPathCommand { get; set; }
        public DelegateCommand? ChooseBatchPathCommand { get; set; }
        public DelegateCommand? WatchSettingCommand { get; set; }
        
        #endregion

    }
}
