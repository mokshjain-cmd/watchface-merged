using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WatchBasic.Event;
using WatchBasic.Language;
using WatchBasic.LZO;
using WatchBasic.UIBasic;
using WatchBasic;
using WatchUI.CreateBin;
using WatchUI.UI;
using WatchUI.ViewModels;
using WatchDB;
using System.Windows;
using Ionic.Zip;
using WatchBasic.Tool;
using Microsoft.EntityFrameworkCore;
using WatchUI.DataSync;
using WatchJieLi.Common;
using System.Drawing;
using System.Drawing.Imaging;
using WatchBasic.WatchBin;
using static Emgu.CV.DepthAI.Camera;

namespace WatchJieLi.ViewModels
{
    public class MainViewModel : BaseMainView
    {
        #region WatchPoperty

        private LayerGroup? selectDimensions;

        public LayerGroup? SelectDimensions
        {
            get { return selectDimensions; }
            set { SetProperty(ref selectDimensions, value); }
        }

        private WatchTypeItem? selectTypeItem;

        public WatchTypeItem? SelectTypeItem
        {
            get => selectTypeItem;
            set
            {
                SetProperty(ref selectTypeItem, value);
            }
        }

        private ObservableCollection<WatchTypeItem>? typeItems;

        public ObservableCollection<WatchTypeItem>? TypeItems
        {
            get => typeItems;

            set => SetProperty(ref typeItems, value);
        }


        private ObservableCollection<LayerGroupCheck>? dimensionsGroup;

        public ObservableCollection<LayerGroupCheck>? DimensionsGroup
        {
            get { return dimensionsGroup; }
            set { SetProperty(ref dimensionsGroup, value); }
        }

        private List<Layer>? backgroundLayers;

        public List<Layer>? BackgroundLayers
        {
            get { return backgroundLayers; }
            set { SetProperty(ref backgroundLayers, value); }
        }


        private PointerImage? pointer;

        public PointerImage? Pointer
        {
            get { return pointer; }
            set
            {
                SetProperty(ref pointer, value);
                ClearPointerOrTime(value);
            }
        }

        private bool isAlbum;

        public bool IsAlbum
        {
            get => isAlbum;
            set
            {
                SetProperty(ref isAlbum, value);
                if (baseSetting != null)
                {
                    baseSetting.IsAlbum = IsAlbum;
                    baseSetting?.Save();
                }

            }
        }

        private bool isColor888;

        public bool IsColor888
        {
            get { return isColor888; }
            set
            {
                SetProperty(ref isColor888, value);
                CommonDefintion.IsColor888 = IsColor888;
                if (baseSetting != null)
                {
                    baseSetting.IsColor888 = IsColor888;
                    baseSetting?.Save();
                }
            }
        }


        #endregion

        private WatchDataContext? _dataContext;
        public WatchInfo? WatchInfo;

        public void SaveData()
        {
            _dataContext?.SaveChanges();
        }

        BaseSetting baseSetting { get; set; }


        public byte[] convertBitmapTo565(Bitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            byte[] bytes = new byte[width * height * 2];

            int index = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var color = bitmap.GetPixel(x, y);
                    int r = color.R;
                    int g = color.G;
                    int b = color.B;
                    int value = ((r & 0xF8) << 8) | ((g & 0xFC) << 3) | (b >> 3);
                    bytes[index++] = (byte)((value >> 8) & 0xFF);
                    bytes[index++] = (byte)(value & 0xFF);
                }
            }

            return bytes;
        }


      
        public MainViewModel(IRegionManager regionManager, IDialogService dialogAware, IEventAggregator eventAggregator) : base(regionManager, dialogAware, eventAggregator)
        {

           
                        //var bytes = DIYSerializeTools.GetPng565Bytes(new Bitmap("C:\\Users\\LeungChee\\Desktop\\新建文件夹\\11.png"));
                        //File.WriteAllBytes("test.bin", bytes);
                        baseSetting = new BaseSetting("aa");
            IsAlbum = baseSetting.IsAlbum ?? false;
            IsColor888 = baseSetting.IsColor888 ?? false;
            SettingPathCommand = new DelegateCommand(SetBasePath);
            ChooseBatchPathCommand = new DelegateCommand(ChooseBatchPath);
            SetLocationLayerGroupCommand = new DelegateCommand<LayerGroup>(SetLocationLayerGroup);
            OpenCatalogueCommand = new DelegateCommand(OpenCatalogue);
            NavigateCommand = new DelegateCommand<BaseTypeItem>(Navigation);
            RefreshWatchCommand = new DelegateCommand(() => RefreshWatch(FolderPath));
            GenerateBinCommand = new DelegateCommand(GenerateBin);
            WatchSettingCommand = new DelegateCommand(WatchSetting);
            GenrateBatchBinCommand = new DelegateCommand(async () =>
            {
                if (string.IsNullOrWhiteSpace(BatchPath))
                {
                    MessageBox.Show("无导出路径");
                    return;
                }
                _eventaggregator?.GetEvent<ShowProgressBar>().Publish(true);
                await Task.Run(() =>
                {
                    GenrateBatchBin();
                });
                _eventaggregator?.GetEvent<ShowProgressBar>().Publish(false);
                MessageBox.Show("导出完成");
            });
            SetRatio = new DelegateCommand(SetRatios);
            RrefreshPathCommand = new DelegateCommand(RefreshPath);
            OpenInformationCommand = new DelegateCommand(() =>
            {
                IDialogParameters parameters = new DialogParameters();
                parameters.Add("FolderPath", FolderPath);
                parameters.Add("WatchDescs", WatchDescs);
                dialogAware.ShowDialog("Information", parameters, null);
            });
            OpenThumbnailCommand = new DelegateCommand(() =>   // 打开缩略图
            {
                IDialogParameters parameters = new DialogParameters();
                parameters.Add("BackgroundItem", BackgroundItem);
                parameters.Add("Setting", CommonDefintion.Setting);

                dialogAware.ShowDialog("Thumbnail", parameters, null);
            });

            //数据同步
            _eventaggregator.GetEvent<WatchItemLayerPointEvent>().Subscribe((e) =>
            {
                var group = WatchInfo?.WatchTypes?.SelectMany(i => i.WatchGroups!).Where(i => i.GroupCode == e.GroupCode).FirstOrDefault();
                if (group != null)
                {
                    group.Left = e.Left;
                    group.Top = e.Top;
                    group.ColorDesc = (LayerTool.AllowColorGroup().Contains(e.GroupCode)) ? e.ColorDesc : String.Empty;
                    SaveData();
                }
            });

            _eventaggregator.GetEvent<WatchTypeAlignEvent>().Subscribe((e) =>
            {
                var item = WatchInfo?.WatchTypes?.Where(i => i.WatchTypeName == e.WatchType).FirstOrDefault();
                if (item != null)
                {
                    item.Align = e.Align;
                    SaveData();
                }
            });

            _eventaggregator.GetEvent<WatchGroupEvent>().Subscribe((e) =>
            {
                var group = WatchInfo?.WatchTypes?.SelectMany(i => i.WatchGroups!).Where(i => i.GroupCode == e.GroupCode).FirstOrDefault();
                if (group != null)
                {
                    group.Top = e.Top;
                    group.Left = e.Left;
                }
                SaveData();

            });

            _eventaggregator?.GetEvent<TimeGroupEvent>().Subscribe((e) =>
            {
                var group = WatchInfo?.WatchTypes?.SelectMany(i => i.WatchGroups!).Where(i => i.GroupCode == e.GroupCode).FirstOrDefault();
                if (group != null)
                {
                    group.ColorDesc = e.ColorDesc;
                }
                SaveData();
            });

            _eventaggregator?.GetEvent<WatchGroupColorDescEvent>().Subscribe((e) =>
            {
                var group = WatchInfo?.WatchTypes?.SelectMany(i => i.WatchGroups!).Where(i => i.GroupCode == e.GroupCode).FirstOrDefault();
                if (group != null)
                {
                    group.ColorDesc = e.ColorDesc;
                }
                SaveData();
            });

            RefreshPath();
        }


        #region 命令
        public DelegateCommand<BaseTypeItem> NavigateCommand { get; set; }

        public DelegateCommand? CreateLocationCommnad { get; set; }
        public DelegateCommand<LayerGroup>? SetLocationLayerGroupCommand { get; set; }
        //public DelegateCommand? SetItemLocationCommand { get; set; }
        public DelegateCommand? OpenCatalogueCommand { get; set; }
        public DelegateCommand? RefreshWatchCommand { get; set; }

        public DelegateCommand? GenerateBinCommand { get; set; }

        public DelegateCommand? GenrateBatchBinCommand { get; set; }
        public DelegateCommand? SetRatio { get; set; }
        public DelegateCommand? OpenInformationCommand { get; set; }
        public DelegateCommand? OpenThumbnailCommand { get; set; }
        public DelegateCommand? RrefreshPathCommand { get; set; }

        #endregion


        void ZipFile(string zipName, List<string> files)
        {
            ZipFile zip = new ZipFile(zipName);
            foreach (string file in files)
            {
                zip.AddFile(file, "");
            }
            zip.Save();

        }

        void GenerateBin(WatchInfo watchInfo, string path, ProgressInfo progressInfo, string desc, LanguageFactory factory)
        {
            if (watchInfo != null)
            {
                var layergroups = GetLayerGroups(path, true);
                LayerTool.CheckUnit(layergroups, watchInfo);
                if (progressInfo != null)
                {
                    progressInfo.ProImgPath = layergroups?.FirstOrDefault(i => i.GroupCode!.Contains("辅助文件"))?.GetLayerByName("效果")?.ImgPath;
                }
                var watchDescs = WatchTool.GetWatchDesc(path);

                foreach (var i in layergroups)
                {
                    if (LayerTool.GetLanguageCode().Contains(i.GroupCode))
                    {
                        i.LanguageCount = watchDescs.Count;
                    }
                }
                var nowstr = DateTime.Now.ToString("yyyy-MM-dd_hh_mm_ss");
                var error_txt = $@"{BatchPath}\\error.txt";
                var layer_error = $@"{BatchPath}\\layer_error.txt";
                foreach (var watchdesc in watchDescs)
                {
                    try
                    {
                        var savePath = $"{BatchPath}\\{desc}\\{nowstr}\\";

                        if (!Directory.Exists(savePath))
                        {
                            Directory.CreateDirectory(savePath);
                        }
                        var watchIdstr = CommonDefintion.GetWatchId(watchInfo, watchdesc.Language);
                        var watchId = System.Text.ASCIIEncoding.UTF8.GetBytes(watchIdstr).ToList();
                        var horbinList = GetBin(layergroups, watchDescs, factory, desc, error_txt, watchdesc, watchInfo, watchId,IsAlbum).ToList();
                        if (horbinList == null) { continue; }
                        var effects = layergroups?.Where(i => i.GroupCode!.Contains("辅助文件")).FirstOrDefault()?.GetLayersByName($"效果").ToList();
                        var effect = effects[factory.GetLayerOrder(watchdesc.Language)];
                        var extendName = (effect?.IsPng ?? false) ? ".png" : ".bmp";
                        var effectfile = $"{savePath}img_effect{extendName}";
                        //效果图
                        if (effect != null)
                        {
                            File.Copy(effect?.ImgPath!, effectfile, true);
                        }
                        var thumbnails = layergroups?.Where(i => i.GroupCode!.Contains("辅助文件")).FirstOrDefault()?.GetLayersByName($"缩略").ToList();
                        var thumbnail = thumbnails[factory.GetLayerOrder(watchdesc.Language)];
                        //extendName = (thumbnail?.IsPng ?? false) ? ".png" : ".bmp";
                        var thumbnailfile = $"{savePath}img_thumbnail.png";
                        //缩略图
                        if (thumbnail != null)
                        {
                            var newthumbnail = DIYX1BinTool.GetThumnail(thumbnail?.ImgPath!, CommonDefintion.Setting.ProjectName);
                            newthumbnail.Save(thumbnailfile);
                            //File.Copy(thumbnail?.ImgPath!, thumbnailfile, true);
                        }
                        if (IsAlbum)
                        {
                            List<byte> horbinData = new List<byte>() { 2 };
                            horbinData.AddRange(Enumerable.Range(0, 91).Select(i => (byte)0));
                            horbinData.AddRange(CommonDefintion.Setting.ThumbnailX.GetBytes16());
                            horbinData.AddRange(CommonDefintion.Setting.ThumbnailY.GetBytes16());
                            horbinData.AddRange(thumbnail.Width.GetBytes16());
                            horbinData.AddRange(thumbnail.Height.GetBytes16());
                            horbinList.InsertRange(0, horbinData);
                        }
                        var horbin = horbinList.ToArray();
                        Console.WriteLine($"[GUI] Uncompressed bin size: {horbin.Length} bytes ({horbin.Length / 1024.0:F2} KB)");
                        if (!IsAlbum)
                        {
                            horbin = LZOHelper.mergeDialFile(horbin);//lzo
                            Console.WriteLine($"[GUI] Compressed bin size: {horbin.Length} bytes ({horbin.Length / 1024.0:F2} KB)");
                        }
                        var binfile = $"{savePath}hor.bin";
                        File.WriteAllBytes(binfile, horbin);
                        var verfile = $"{savePath}ver.bin";
                        File.WriteAllBytes(verfile, horbin);
                        var overlayerFile = string.Empty;
                        var bgFile = string.Empty;
                        List<string> zipFiles = new List<string>();

                        if (progressInfo != null)
                        {
                            progressInfo.ProBinNum = horbin.Length;
                        }
                       
                        if (IsAlbum)
                        {
                            var overlayers = layergroups?.Where(i => i.GroupCode!.Contains("辅助文件")).FirstOrDefault()?.GetLayersByName($"叠加").ToList();
                            var overlayer = overlayers[factory.GetLayerOrder(watchdesc.Language)];
                            var overExtendName = (overlayer?.IsPng ?? false) ? ".png" : ".bmp";
                            overlayerFile = $"{savePath}img_zdy_text{overExtendName}";
                            //叠加图img_zdy_bg.png
                            if (overlayer != null)
                            {
                                File.Copy(overlayer?.ImgPath!, overlayerFile, true);
                            }
                            zipFiles.Add(overlayerFile);
                            //zip.AddFile(overlayerFile, "");

                            var bg = layergroups?.Where(i => i.GroupCode == "0101").FirstOrDefault()?.Layers?.FirstOrDefault();
                            if (bg != null)
                            {
                                var bgExtendName = (bg?.IsPng ?? false) ? ".png" : ".bmp";
                                bgFile = $"{savePath}img_zdy_bg{bgExtendName}";
                                File.Copy(bg?.ImgPath!, bgFile, true);
                                // zip.AddFile(bgFile, "");
                            }
                            zipFiles.Add(bgFile);

                        }
                        var zipName = $"{savePath}{watchIdstr}_{watchdesc.Language}_{watchInfo.WatchCode}_(默认风格-0)_{DateTime.Now.ToString("yyyy_MM_dd")} {DateTime.Now.ToString("HH_mm_ss")}_v16.8_2.zip";
                        var size = horbin.Length / 1024;
                        if (horbin.Length % 1024 > 0)
                        {
                            size += 1;
                        }
                        var watchAppDes = CommonDefintion.WriteWatchInfo(watchInfo, watchdesc, size);
                        var watchAppDesFile = $"{savePath}config.properties";
                        File.WriteAllLines(watchAppDesFile, watchAppDes);
                        zipFiles.Add(binfile);
                        zipFiles.Add(effectfile);
                        zipFiles.Add(thumbnailfile);
                        zipFiles.Add(watchAppDesFile);
                        zipFiles.Add(verfile);
                        ZipFile(zipName, zipFiles);
                        File.Delete(binfile);
                        File.Delete(effectfile);
                        File.Delete(thumbnailfile);
                        File.Delete(verfile);
                        File.Delete(watchAppDesFile);
                        if (File.Exists(bgFile))
                        {
                            File.Delete(bgFile);
                        }
                        if (File.Exists(overlayerFile))
                        {
                            File.Delete(overlayerFile);
                        }
                    }

                    catch (Exception ex)
                    {

                        MessageBox.Show(ex.Message);
                        var code = Marshal.GetLastWin32Error();
                        File.AppendAllText(layer_error, $"{desc}:{ex.Message},code={code} \n\r");
                        continue;
                    }
                }
            }
        }


        byte[] GetBin(List<LayerGroup> layergroups, List<WatchDesc> watchDescs, LanguageFactory factory, string desc, string error_txt, WatchDesc watchdesc, WatchInfo watchInfo, List<byte> watchId,bool isAlbum)
        {

            BinHelpers binHelper = new BinHelpers();
            var colorInfo = new List<byte>();
            var bin = binHelper.GetBinBytes(layergroups, watchDescs.Count(), watchInfo, colorInfo, isAlbum).ToList();
            List<byte> horbin = new List<byte>();

            // var watchId= System.Text.ASCIIEncoding.UTF8.GetBytes(watchIdstr);
            var watchName = System.Text.ASCIIEncoding.UTF8.GetBytes(watchdesc?.WatchName!.Split(":")[1]);
            var len = watchName.Length;

            var arrayCount = 60 - 1 - watchName.Count();//1为表盘名长度
            if (arrayCount < 0)
                throw new Exception("表盘名长度超出限制");
            var remain = Enumerable.Range(0, arrayCount).Select(i => (byte)0).ToList();//表盘名长度60不足补0
            horbin.AddRange(watchId);
            horbin.Add((byte)len);
            horbin.AddRange(watchName);
            horbin.AddRange(remain);
            horbin.Add(0);//版本号
            horbin.AddRange(100.GetBytes32());//块1地址
            var firstSzie = bin.Count();
            var secondAddr = 100 + firstSzie;
            while (secondAddr % 4 != 0)
            {
                secondAddr++;
                bin.Add(0);
            }//4K对齐
            horbin.AddRange(bin.Count().GetBytes32());//块1大小
            horbin.AddRange(secondAddr.GetBytes32());//块2地址

            if (IsAlbum) //相册表盘
            {

                horbin.AddRange(0.GetBytes32());//块2大小没确定，填0
                horbin.AddRange(Enumerable.Range(0, 3).Select(i => (byte)0).ToArray());//保留3
                horbin.AddRange(bin);//块1
                horbin.InsertRange(0, colorInfo);
                var extrCount = (CommonDefintion.Setting.Width * CommonDefintion.Setting.Height * 3) + (CommonDefintion.Setting.ThumbnailHeight * CommonDefintion.Setting.ThumbnailWidth * 3);
                if (!IsColor888 && (horbin.Count+ extrCount) > CommonDefintion.Setting.MaxValue * 1024)
                {
                    MessageBox.Show($"{desc}文件过大,超出了{(horbin.Count + extrCount) - CommonDefintion.Setting.MaxValue * 1024}K");
                    File.AppendAllText(error_txt, $"{desc}\n\r");
                    return null;
                    // continue;
                }
                if (IsColor888 && horbin.Count > (1.5) * 1024 * 1024)
                {
                    MessageBox.Show($"{desc}文件过大,超出了{horbin.Count - (1.5 * 1024 * 1024)}K");
                    File.AppendAllText(error_txt, $"{desc}\n\r");
                    return null;
                    // continue;
                }
            }
            else
            {
                var backgroundBin = binHelper.GetBackgroundBinBytes(layergroups, factory.GetLayerOrder(watchdesc.Language), watchInfo, CommonDefintion.Setting);
                horbin.AddRange(backgroundBin.Count().GetBytes32());//块2大小
                horbin.AddRange(Enumerable.Range(0, 3).Select(i => (byte)0).ToArray());//保留3
                horbin.AddRange(bin);
                horbin.AddRange(backgroundBin);
                horbin.AddRange(LZOHelper.GetOther(horbin.ToArray()));
                if (!IsColor888 && horbin.Count > CommonDefintion.Setting.MaxValue * 1024)
                {
                    MessageBox.Show($"{desc}文件过大,超出了{horbin.Count - (CommonDefintion.Setting.MaxValue * 1024)}K");
                    File.AppendAllText(error_txt, $"{desc}\n\r");
                    return null;
                    // continue;
                }
                if (IsColor888 && horbin.Count > (1.5) * 1024 * 1024)
                {
                    MessageBox.Show($"{desc}文件过大,超出了{horbin.Count - (1.5 * 1024 * 1024)}K");
                    File.AppendAllText(error_txt, $"{desc}\n\r");
                    return null;
                    // continue;
                }
            }
            return horbin.ToArray();
        }


        void GenerateBin(string path, int folderNum, ProgressInfo? progressInfo)
        {
            if (progressInfo != null)
            {
                progressInfo.NowNum = folderNum;
            }
            var desc = Path.GetFileName(path);
            var watchInfo = GetWatchInfo(desc, path);
            LanguageFactory factory = new LanguageFactory(CommonDefintion.Setting.ProjectName);
            GenerateBin(watchInfo, path, progressInfo, desc, factory);
            if (progressInfo != null)
            {
                progressInfo.ProValue = (int)((folderNum * 1.00 / Folders.Count()) * 100);
            }

        }

        public void GenrateBatchBin()
        {
            if (string.IsNullOrWhiteSpace(BatchPath))
            {
                MessageBox.Show("无导出路径");
                return;
            }
            if (Folders != null)
            {
                if (Folders?.Count() <= 0)
                {
                    MessageBox.Show("无表盘生成");
                    return;
                }
                ProValues = new ProgressInfo();
                ProValues.AllNum = Folders?.Count() ?? 0;
                int FolderNum = 0;

                var error_txt = $@"{BatchPath}\\error.txt";
                var layer_error = $@"{BatchPath}\\layer_error.txt";
                if (File.Exists(error_txt))
                {
                    File.Delete(error_txt);
                }
                if (File.Exists(layer_error))
                {
                    File.Delete(layer_error);
                }
                foreach (var path in Folders)
                {
                    FolderNum++;
                    GenerateBin(path, FolderNum, ProValues);
                }

            }

        }

        /// <summary>
        /// 生成BIN
        /// </summary>
        public void GenerateBin()
        {
            if (string.IsNullOrEmpty(FolderPath))
            {
                MessageBox.Show("请选择目录");
                return;
            }

            if (string.IsNullOrWhiteSpace(BatchPath))
            {
                MessageBox.Show("无导出路径");
                return;
            }
            GenerateBin(FolderPath, 0, null);
            MessageBox.Show("生成成功");
        }



        private void RefreshWatch(string? path)
        {
            if (path == null) return;
            Ratio = 1;  // 缩放比例默认为1
            LoadImages(path);
            DimensionsGroup = new ObservableCollection<LayerGroupCheck>();
            foreach (var item in TypeItems!)
            {
                Navigation(item);
            }
            SetDimensionsGroup(Pointer?.ShowGroups);
            SetDimensionsGroup(DimensionsGroup);
            SetRatios();

        }

        WatchInfo? GetWatchInfo(string watchCode, string? path)
        {
            _dataContext = new WatchDataContext(path);
            _dataContext.Database.EnsureCreated();
            //var watchInfo = _dataContext?.WatchInfos?.Include(i => i.WatchTypes!).ThenInclude(i => i.WatchGroups!)
            //              .FirstOrDefault();
            //_dataContext?.WatchInfos?.Remove(watchInfo);
            //_dataContext.SaveChanges();
            var watchInfo = _dataContext?.WatchInfos?.Include(i => i.WatchTypes!).ThenInclude(i => i.WatchGroups!)
                           .FirstOrDefault();
            if (watchInfo != null && watchInfo.WatchCode != watchCode)//默认改名了
            {
                watchInfo.WatchCode = watchCode;
                _dataContext?.SaveChanges();
            }
            return watchInfo;
        }

        private void LoadImages(string? path)
        {

            #region 
            WatchCode = Path.GetFileName(path);
            if (WatchCode != null)
            {
                WatchInfo = GetWatchInfo(WatchCode, path);
                if (WatchInfo == null)
                {
                    WatchInfo = new WatchInfo { WatchCode = WatchCode };
                    _dataContext?.WatchInfos.Add(WatchInfo);
                }
            }
            Shape = WatchInfo?.Shape;
            Dpi = WatchInfo?.Dpi;
            Type = WatchInfo?.Type;
            Author = WatchInfo?.Author;
            Code = WatchInfo?.Code;
            //背景
            var allGroups = GetLayerGroups(path);
            LayerTool.CheckLayerConsistency(allGroups);
            WatchDescs = new ObservableCollection<WatchDesc>(WatchTool.GetWatchDesc(path!)!);
            // LayerTool.CheckLayerGroup(allGroups, WatchDescs.Count);
            BackgroundItem = new WatchBackgroundItem(allGroups);
            BackgroundLayers = allGroups?.FirstOrDefault(i => i.GroupCode == "0101")?.Layers?.ToList();
            //辅助文件
            //allGroups?.Where(i => i.GroupCode == "辅助文件")?.FirstOrDefault();

            //复杂
            TypeItems = new ObservableCollection<WatchTypeItem>();

            var ComplexCodeHeadList = LayerTool.ComplexCodeHead();
            foreach (var item in ComplexCodeHeadList)
            {
                //check
                var watchTypeItem = new WatchTypeItem(allGroups, item);
                if (watchTypeItem != null)
                {
                    UIDataAyncs.AsyncWatchTypeItem(watchTypeItem, WatchInfo, SaveData);//数据同步
                    TypeItems.Add(watchTypeItem);
                }
            }

            // 指针
            var pointerGroups = allGroups?.Where(i => i.GroupCode!.StartsWith("13"));
            Pointer = new PointerImage(pointerGroups?.ToList(), "13");
            if (Pointer != null)
            {
                UIDataAyncs.AsyncWatchTypeItem(Pointer, WatchInfo, SaveData);//数据同步
            }
            LanguageControl = new LanguageControl(TypeItems, BackgroundItem, Pointer, WatchDescs!.Count);
            LanguageControl?.SwithcLanguage(CommonDefintion.Setting.ProjectName, "英文");
            #endregion
        }

        private void OpenCatalogue()
        {
            if (Folders == null) return;
            DialogParameters pairs = new DialogParameters
            {
                {"Folders", Folders },
                {"Title", WatchCode}
            };

            _dialogAware?.ShowDialog("Catalogue", pairs, args =>
            {
                if (args.Result == ButtonResult.OK)
                {
                    FolderPath = args.Parameters.GetValue<string>("FolderPath");
                    RefreshWatch(FolderPath);

                }
            });
        }


        private void Navigation(BaseTypeItem? e)
        {
            if (e == null) return;
            _regionManager?.Regions[$"{e?.WatchType}Content"].RequestNavigate(e?.WatchType, new NavigationParameters
                     {
                         {"typeItem", e},
                         {"watchInfo",WatchInfo }

                      });
            SetDimensionsGroup(e?.ShowGroups);
        }


        public void SetLocationLayerGroup(LayerGroup g)
        {

            if (g == null || (g != null && !g.IsEnable)) return;

            IDialogParameters parameters = new DialogParameters();

            var showGroup = TypeItems?.SelectMany(i => i.ShowGroups!).Where(i => i.GroupCode == g?.GroupCode).FirstOrDefault();
            showGroup ??= Pointer?.ShowGroups?.Where(i => i.GroupCode == g?.GroupCode).FirstOrDefault();

            parameters.Add("ShowGroup", showGroup);
            parameters.Add("Group", g);
            parameters.Add("Coordinate", BackgroundItem?.CoordinateImage);

            _dialogAware?.ShowDialog("EditLayer", parameters, args =>
            {
                if (args.Result == ButtonResult.OK)
                {
                    var groupCode = args.Parameters.GetValue<string>("GroupCode");
                    DimensionsGroup = new ObservableCollection<LayerGroupCheck>();
                    foreach (var item in TypeItems)
                    {
                        SetDimensionsGroup(item.ShowGroups);
                    }
                    SetDimensionsGroup(Pointer?.ShowGroups);
                    SetDimensionsGroup(DimensionsGroup);
                    asyncGroupDB(DimensionsGroup);

                }
            });
        }

        /// <summary>
        /// 编辑Group与数据库同步
        /// </summary>
        void asyncGroupDB(ObservableCollection<LayerGroupCheck>? groups)
        {
            var groupDB = WatchInfo?.WatchTypes?.SelectMany(i => i.WatchGroups!);
            if (groupDB != null)
            {
                foreach (var g in groupDB)
                {
                    var layerGroup = groups.FirstOrDefault(i => i.GroupCode == g.GroupCode);
                    if (layerGroup != null)
                    {
                        layerGroup.SetLocationNoRefresh(g.Left, g.Top);
                    }
                }


            }

        }

        public void SetDimensionsGroup(ObservableCollection<LayerGroup>? typeItem)
        {

            var list = typeItem?.Where(i => i.LayerNum > 0).OrderBy(i => i.GroupCode).CopyLayerGroupChecks();
            DimensionsGroup?.AddRange(list);

        }

        void SetDimensionsGroup(ObservableCollection<LayerGroupCheck>? groups)
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
        void ClearPointerOrTime(BaseTypeItem? baseTypeItem)
        {
            if (baseTypeItem == null) return;
            if (baseTypeItem is PointerImage)
            {
                DimensionsGroup = null;
            }
            else
            {
                Pointer = null;
            }
        }
        private void SetRatios()
        {
            //if (Ratio <= 0.2M)
            //{
            //    Ratio = 0.2M;
            //}
            //if (Ratio > 2)
            //{
            //    Ratio = 2;
            //}

            if (BackgroundLayers != null)
            {
                foreach (var item in BackgroundLayers)
                {
                    item.Ratio = Ratio;
                }
            }
            if (BackgroundItem != null)
            {
                BackgroundItem.DesignSketch.Ratio = Ratio;
                //BackgroundItem.OverlayImage.Radio = Ratio;
            }

            if (Pointer != null)
            {
                if (Pointer.LayerGroups != null)
                    foreach (var item in Pointer.LayerGroups)
                    {
                        item.SetLayerRatio(Ratio);
                    }
            }

            if (TypeItems != null)
            {
                foreach (var item in TypeItems)
                {
                    item.SetLayerRatio(Ratio);
                }
            }
        }
    }

    public class BaseSetting
    {
        public BaseSetting()
        {

        }
        public BaseSetting(string aa)
        {
            if (File.Exists("basesetting.json"))
            {
                var temp = JsonConvert.DeserializeObject<BaseSetting>(File.ReadAllText("basesetting.json"));
                this.IsAlbum = temp?.IsAlbum;
                this.IsColor888 = temp?.IsColor888;
            }
        }

        public bool? IsAlbum { get; set; }

        public bool? IsColor888 { get; set; }

        public void Save()
        {
            var str = JsonConvert.SerializeObject(this);
            File.WriteAllText("basesetting.json", str);
        }
    }
}
