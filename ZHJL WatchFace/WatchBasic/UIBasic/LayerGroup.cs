using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchBasic.UIBasic
{
    /// <summary>
    /// Layer文件夹
    /// </summary>
    public class LayerGroup : BindableBase
    {

        public Action<int, int>? SetLocationCallBack;
        public LayerGroup()
        {
            Layers = new List<Layer>();
        }



        /// <summary>
        /// 对应的文件夹编号
        /// </summary>
        private string? groupCode;

        public string? GroupCode
        {
            get => groupCode;
            set => SetProperty(ref groupCode, value);
        }

        /// <summary>
        /// 文件夹名
        /// </summary>
        private string? groupName;

        public string? GroupName
        {
            get => groupName;
            set => SetProperty(ref groupName, value);
        }

        private IList<Layer>? layers;

        public IList<Layer>? Layers
        {
            get => layers;
            set => SetProperty(ref layers, value);
        }

        /// <summary>
        /// 语言个数
        /// </summary>
        public int LanguageCount { get; set; } = 1;

        /// <summary>
        /// 显示哪种语言
        /// </summary>
        public int LanguagePage { get; set; } = 1;


        /// <summary>
        /// 显示的序号
        /// </summary>
        private int showIndex;
        public int ShowIndex
        {
            get => showIndex;
            set
            {
                SetProperty(ref showIndex, value);
                RaisePropertyChanged(nameof(ShowLayer));
            }
        }

        /// <summary>
        /// 单位图标使用（电量百分比）
        /// </summary>
        public bool LongShow { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        private bool isShow;
        public bool IsShow
        {
            get => isShow;
            set
            {
                SetProperty(ref isShow, value);

            }
        }

        /// <summary>
        /// 默认图层
        /// </summary>
        Layer? DefaultLayer => (Layers?.Any() ?? false) ? Layers.FirstOrDefault() : null;

        public int Width => DefaultLayer?.Width ?? 0;
        public int Height => DefaultLayer?.Height ?? 0;
        public int Top => DefaultLayer?.Top ?? 0;
        public int Left => DefaultLayer?.Left ?? 0;

        public int AbsoluteLeft => DefaultLayer?.AbsoluteLeft ?? 0;
        public int AbsoluteTop => DefaultLayer?.AbsoluteTop ?? 0;

        public int LayerNum => (Layers?.Count() / LanguageCount) ?? 0;

        public int LanguageIndex => LayerNum * (LanguagePage - 1);

        //图层数据同步
        public Action<string?, int, int>? asyncGroupLayer { get; set; }
        public Action<string?, string?>? asyncGroupLayerColor { get; set; }

        /// <summary>
        /// 显示Layer 
        /// </summary>
       // public Layer? ShowLayer => (Layers?.Any() ?? false) && IsShow ? Layers[ShowIndex > LayerNum - 1 ? LayerNum - 1 : ShowIndex] : null;

        public Layer? ShowLayer
        {
            get
            {
                var index = ShowIndex + LanguageIndex;
                var trueNum = LayerNum + LanguageIndex - 1;
                return (Layers?.Any() ?? false) && IsShow ? Layers[index > trueNum ? trueNum : index] : null;
            }
        }

        public void SetLocationNoRefresh(int left, int top)
        {
            Layers?.ToList().ForEach(layer =>
            {
                layer.Left = left;
                layer.Top = top;

            });
        }

        public void SetLocation(int left, int top, bool isAsync = false)
        {
            SetLocationNoRefresh(left, top);
            if (isAsync)
                asyncGroupLayer?.Invoke(GroupCode, Left, Top);

            RaisePropertyChanged(nameof(ShowLayer));
        }

        public void UpdateColorDesc(string? colorDesc)
        {
            Layers?.ToList().ForEach(layer =>
            {
                if ((!layer.NotDye) && (layer.IsPng ?? false))
                {
                    layer.ColorDesc = colorDesc;
                    asyncGroupLayerColor?.Invoke(GroupCode, colorDesc);
                }

            });
        }

        public void SetLayerRatio(decimal ratio)
        {
            Layers?.ToList().ForEach(layer =>
            {
                layer.Ratio = ratio;

            });
        }

        //public void SetColor(System.Windows.Media.Color color)
        //{
        //    Layers?.ToList().ForEach(layer =>
        //    {
        //        layer.LayerColor = color;

        //    });
        //}

        public void SetAbsoulteLocation(int left, int top)
        {
            Layers?.ToList().ForEach(layer =>
            {
                layer.AbsoluteLeft = left;
                layer.AbsoluteTop = top;
            });
            RaisePropertyChanged(nameof(ShowLayer));
        }


        public void SetAngle(int angle)
        {
            Layers?.ToList().ForEach(layer =>
            {
                layer.Angle = angle;
            });
            RaisePropertyChanged(nameof(ShowLayer));
        }

        public Layer? GetLayerByName(string name)
        {
            return Layers?.Where(i => i.ImgPath!.Contains(name)).FirstOrDefault();
        }

        public Layer? GetLayerByName(string name, int index)
        {
            var templist = from layer in Layers
                           let first = Path.GetFileNameWithoutExtension(layer.ImgPath).Split('_').First()
                           let second = Path.GetFileNameWithoutExtension(layer.ImgPath).Split('_').Length == 2 ? Path.GetFileNameWithoutExtension(layer.ImgPath).Split('_').Last() : "error"
                           select new
                           {
                               layer = layer,
                               first = first,
                               second = second
                           };
            return templist.Where(i => i.first.Contains(name) && i.second == index.ToString()).FirstOrDefault()?.layer;


        }
        public IList<Layer>? GetLayersByName(string name)
        {
            return Layers?.Where(i => i.ImgPath!.Contains(name)).ToList();
        }

        private bool isEnable;
        public bool IsEnable
        {
            get => isEnable;
            set => SetProperty(ref isEnable, value);
        }

    }
}
