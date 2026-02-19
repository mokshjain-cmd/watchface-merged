using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using WatchBasic.UIBasic;
using WatchBasic.WatchBin.Model;
using WatchBasic.Language;
using WatchBasic.Event;

namespace WatchUI.UI
{
    /// <summary>
    /// 用于UI界面元素绑定(电量、心率、步数、卡路里)
    /// </summary>
    public class WatchShowItem : BindableBase
    {
        public Action? RefreshLayer;

        public Action? AsyncLocationItem;


        public WatchShowItem(List<LayerGroup>? layerGroups, Action? refreshLayer, Action? asyncLocationItem, bool noJustLocation = false)
        {
            this.NoJustLocation = noJustLocation;
            Init(layerGroups, refreshLayer, asyncLocationItem);
        }

        public Action<int>? PublishEvent;
        public virtual void Init(List<LayerGroup>? layerGroups, Action? refreshLayer, Action? asyncLocationItem)
        {
            LayerGroups = layerGroups;
            RefreshLayer = refreshLayer;
            AsyncLocationItem = asyncLocationItem;
            if (LayerGroups != null)
            {
                var group = LayerGroups.FirstOrDefault();
                var groups = LayerGroups?.Where(i => !i.LongShow).ToList();
                LayerNum = group?.LayerNum ?? 0;
                InitPoint = new Point(group?.Left ?? 0, group?.Top ?? 0);
                LayerWidth = group?.Width ?? 0;
                ShowNum = groups?.Count ?? 0;
                IsEnable = LayerNum > 0;
                ShowIndex = group == null ? 1 : group.ShowIndex + 1;
            }
        }

        public void RestLocation(bool asyncData)
        {
            if (LayerGroups != null)
            {
                var group = LayerGroups.FirstOrDefault();
                InitPoint = new Point(group?.Left ?? 0, group?.Top ?? 0);
                LayerWidth = group?.Width ?? 0;
                ShowLayer(asyncData);
            }
        }


        public void UpdateColorDesc(string? colorDesc)
        {
            if (LayerGroups != null)
                foreach (var group in LayerGroups)
                {
                    if (group.ShowLayer?.IsPng ?? false && (!(group.ShowLayer?.NotDye ?? false)))
                        group?.UpdateColorDesc(colorDesc);
                }
        }

        private IList<LayerGroup>? layerGroups;
        public IList<LayerGroup>? LayerGroups
        {
            get => layerGroups;
            set => SetProperty(ref layerGroups, value);
        }

        protected int LayerWidth;
        public Point InitPoint;

        /// <summary>
        /// 图层数量
        /// </summary>
        public int layerNum;
        public int LayerNum  // => LayerGroups?.FirstOrDefault()?.LayerNum ?? 0;
        {
            get => layerNum;
            set
            {
                SetProperty(ref layerNum, value);
                RaisePropertyChanged(nameof(BarDescr));
            }
        }

        /// <summary>
        /// 不调整坐标
        /// </summary>
        public bool NoJustLocation { get; set; }


        private bool isEnable;

        public bool IsEnable
        {
            get => isEnable;
            set
            {
                SetProperty(ref isEnable, value && LayerNum > 0);
            }
        }

        private int showIndex;
        public int ShowIndex
        {
            get => showIndex;
            set
            {
                SetProperty(ref showIndex, value);
                if (LayerGroups != null)
                {
                    foreach (var group in LayerGroups)
                    {
                        group.ShowIndex = showIndex - 1;
                    }
                    RefreshLayer?.Invoke();
                    AsyncLocationItem?.Invoke();
                }
                RaisePropertyChanged(nameof(BarDescr));
            }
        }

        public string BarDescr => $"{ShowIndex}/{LayerNum}";


        private int align;

        public virtual int Align
        {
            get => align;
            set
            {
                SetProperty(ref align, value);

                PublishEvent?.Invoke(value);
                RaisePropertyChanged(nameof(AlignTxt));
                var groups = LayerGroups?.Where(i => !i.LongShow).ToList();


                if (ShowNum != groups?.Count)
                    ShowLayer(false);
            }
        }

        public string AlignTxt => GetAlignTxt(Align);

        protected virtual string GetAlignTxt(int align)
        {
            var result = (WatchAlign)align;
            switch (result)
            {
                case WatchAlign.Full: return "占满";
                case WatchAlign.Left: return "左对齐";
                case WatchAlign.Center: return "居中";
                case WatchAlign.Right: return "右对齐";
                default: return "占满";
            }
        }


        /// <summary>
        /// 显示个数
        /// </summary>
        private int showNum;
        public int ShowNum
        {
            get { return showNum; }
            set
            {
                SetProperty(ref showNum, value);
                ShowLayer(false);
            }
        }

        protected virtual int GetLeft(WatchAlign align, int layerWidth, int showNum)
        {
            var groups = LayerGroups?.Where(i => !i.LongShow).ToList();
            if (align == WatchAlign.Left || align == WatchAlign.Full)
            {
                return InitPoint.X;
            }
            else if (align == WatchAlign.Center)
            {
                return layerWidth * ((groups?.Count ?? 0) - showNum) / 2 + InitPoint.X;
            }
            else if (align == WatchAlign.Right)
            {
                return layerWidth * ((groups?.Count ?? 0) - showNum) + InitPoint.X;
            }
            return 0;
        }

        public void IsShow(bool isShow)
        {
            if (LayerGroups != null)
                foreach (var i in LayerGroups)
                {
                    if (!i.LongShow)
                        i.IsShow = isShow;
                }
        }



        public virtual void ShowLayer(bool asyncData)
        {
            IsShow(false);
            var left = GetLeft((WatchAlign)Align, LayerWidth, ShowNum);
            var groups = LayerGroups?.Where(i => !i.LongShow).ToList();
            for (var i = 0; i < ShowNum; i++)
            {
                if (i > groups.Count - 1)
                {
                    break;
                }
                if (groups?[i]?.Layers?.Count <= 0)
                {
                    continue;
                }
                groups![i].IsShow = true;
                if (!NoJustLocation)
                {
                    groups[i].SetLocation(left, InitPoint.Y, asyncData);
                    left += groups[i].Width;
                }
            }
            var showLayer = LayerGroups?.Where(i => i.LongShow)?.FirstOrDefault();
            if (showLayer != null)
            {
                showLayer.SetLocation(left, InitPoint.Y, asyncData);
            }
            AsyncLocationItem?.Invoke();
            RefreshLayer?.Invoke();
        }

    }



    /// <summary>
    /// UI方位绑定
    /// </summary>
    public class LocationUI : BindableBase
    {

        public LocationUI()
        {
            WatchTypeLocationItems = new ObservableCollection<WatchTypeLocationItem>();
        }

        private string? locationName;

        public string? LocationName
        {
            get => locationName;
            set => SetProperty(ref locationName, value);
        }


        private bool isChecked;

        public bool IsChecked
        {
            get => isChecked;
            set
            {
                SetProperty(ref isChecked, value);
                RefreshLayer?.Invoke();
                AsyncIsChecked?.Invoke(LocationName, value);
            }
        }

        private int left;
        public int Left
        {
            get => left;
            set
            {
                SetProperty(ref left, value);
                UpdateItemLocation();
            }
        }

        private int top;
        public int Top
        {
            get => top;
            set
            {
                SetProperty(ref top, value);
                UpdateItemLocation();
            }
        }

        public Action<string?, bool>? AsyncIsChecked;

        private ObservableCollection<WatchTypeLocationItem>? watchTypeLocationItems;

        public ObservableCollection<WatchTypeLocationItem>? WatchTypeLocationItems
        {
            get => watchTypeLocationItems;
            set
            {
                SetProperty(ref watchTypeLocationItems, value);
            }
        }

        public Action? RefreshLayer { get; set; }

        private WatchTypeLocationItem? selectItem;

        public WatchTypeLocationItem? SelectItem
        {
            get => selectItem;
            set
            {

                SetProperty(ref selectItem, value);

            }
        }

        void UpdateItemLocation()
        {
            foreach (var item in WatchTypeLocationItems!)
            {
                item.SetAbsoulteLocation(Left, Top);
                item.Refresh();
            }

        }

        public Action<string?, string?, int, int>? asyncLocationLayerGroup;


    }






    public class LanguageControl : BindableBase
    {

        public LanguageControl(ObservableCollection<WatchTypeItem>? typeItems, WatchBackgroundItem? backgroundItem, PointerImage pointerImage, int LanguageCount)
        {
            TypeItems = typeItems;
            WatchBackgroundItem = backgroundItem;
            this.LanguageCount = LanguageCount;
            PointerImage = pointerImage;
          
        }

        public LanguageControl(ObservableCollection<WatchTypeItem>? typeItems, WatchBackgroundItem? backgroundItem, PointerImage pointerImage, TimeImage timeImage, int LanguageCount)
        {
            TypeItems = typeItems;
            WatchBackgroundItem = backgroundItem;
            this.LanguageCount = LanguageCount;
            PointerImage = pointerImage;
          
        }

      

        private ObservableCollection<WatchTypeItem>? typeItems;
        public ObservableCollection<WatchTypeItem>? TypeItems
        {
            get => typeItems;
            set => SetProperty(ref typeItems, value);
        }

        private WatchBackgroundItem? watchBackgroundItem;

        public WatchBackgroundItem? WatchBackgroundItem
        {
            get { return watchBackgroundItem; }
            set { SetProperty(ref watchBackgroundItem, value); }
        }

        public PointerImage PointerImage { get; set; }

        public int LanguageCount { get; set; }




        public void SwithcLanguage(string projectName, string? language, bool IsDiy = false)
        {
            var codes = LayerTool.GetLanguageCode();
            LanguageFactory factory = new LanguageFactory(projectName);
            var order = factory.GetLayerOrder(language) + 1;

            var groups = TypeItems?.SelectMany(i => i?.ShowGroups!)?.Where(i => codes.Contains(i.GroupCode!) && i.Layers!.Any());
            if (groups != null)
                foreach (var group in groups)
                {
                    group.LanguageCount = LanguageCount;
                    group.LanguagePage = order;
                }
            if (TypeItems != null)
                foreach (var item in TypeItems)
                {
                    item.Refresh();
                }
            WatchBackgroundItem?.SetLanguage(factory.GetLayerOrder(language));
        }

    }



    /// <summary>
    /// 进度条页面信息
    /// </summary>
    public class ProgressInfo : BindableBase
    {
        private int proValue;
        public int ProValue  // 进度条的值
        {
            get { return proValue; }
            set { SetProperty(ref proValue, value); }
        }
        private int allNum;
        public int AllNum  // 总数量
        {
            get { return allNum; }
            set { SetProperty(ref allNum, value); }
        }
        private int nowNum;
        public int NowNum  // 正在加载的
        {
            get { return nowNum; }
            set { SetProperty(ref nowNum, value); }
        }
        private string? proBitmapSource;
        public string? ProImgPath // 进度条弹窗图片
        {
            get { return proBitmapSource; }
            set
            {
                SetProperty(ref proBitmapSource, value);
            }
        }
        private int? proBinNum;
        public int? ProBinNum // Bin文件字节数组长度
        {
            get { return proBinNum; }
            set
            {
                SetProperty(ref proBinNum, value);
            }
        }
    }
}
