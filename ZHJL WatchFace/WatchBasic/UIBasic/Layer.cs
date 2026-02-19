using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using WatchBasic.Tool;

namespace WatchBasic.UIBasic
{
    /// <summary>
    /// 图层
    /// </summary>
    public class Layer : BindableBase
    {
        public int Id { get; set; }
        private decimal ratio;
        public decimal Ratio
        {
            get => ratio;
            set
            {
                SetProperty(ref ratio, value);
                RaisePropertyChanged(nameof(RatioWidth));
                RaisePropertyChanged(nameof(RatioHeight));
                RaisePropertyChanged(nameof(RatioTop));
                RaisePropertyChanged(nameof(RatioLeft));
                RaisePropertyChanged(nameof(CenterX));
                RaisePropertyChanged(nameof(CenterY));
            }
        }  // 缩放比例
        public decimal RatioWidth => Width * Ratio; // 缩放后高度
        public decimal RatioHeight => Height * Ratio; // 缩放后高度
        public decimal RatioTop => (Top - AbsoluteTop) * Ratio + AbsoluteTop * Ratio; // 缩放后相对高度
        public decimal RatioLeft => (Left - AbsoluteLeft) * Ratio + AbsoluteLeft * Ratio; // 缩放后相对Left
        private int width;

        public int Width
        {
            get { return width; }
            set { SetProperty(ref width, value); }
        }

        private int height;

        public int Height
        {
            get { return height; }
            set => SetProperty(ref height, value);
        }

        //相对高度
        private int top;

        public int Top
        {
            get { return top + absoluteTop; }
            set { SetProperty(ref top, value); RaisePropertyChanged(nameof(RatioTop)); }
        }

        //相对Left
        private int left;

        public int Left
        {
            get { return left + absoluteLeft; }
            set { SetProperty(ref left, value); RaisePropertyChanged(nameof(RatioLeft)); }
        }

        //偏移Top
        private int absoluteTop;

        public int AbsoluteTop
        {
            get { return absoluteTop; }
            set { SetProperty(ref absoluteTop, value); RaisePropertyChanged(nameof(Top)); }
        }

        //偏移Left
        private int absoluteLeft;

        public int AbsoluteLeft
        {
            get { return absoluteLeft; }
            set { SetProperty(ref absoluteLeft, value); RaisePropertyChanged(nameof(Left)); }
        }

        public bool NotBitmapSource { get; set; }

        /// <summary>
        /// 不填色
        /// </summary>
        public bool NotDye { get; set; }

        private string? imgPath;

        public string? ImgPath
        {
            get { return imgPath; }
            set
            {
                SetProperty(ref imgPath, value);
                if (ImgPath != null && File.Exists(ImgPath)&& BitmapSource==null&&(!NotBitmapSource))
                {
                    var bitmap = new Bitmap(ImgPath);
                   // BitmapSource = BitmapSourceConvert.ToBitmapSource(bitmap);
                    BitmapSource = BitmapSourceConvert.ToBitmapImage(bitmap);
                    RaisePropertyChanged(nameof(BitmapSource));
                    if ((IsPng??false) && (!value!.Contains("效果")) && (!value.Contains("坐标"))&&(!NotDye))
                    {
                        if (!value!.Contains("1301") && !value!.Contains("1302") && !value!.Contains("1303"))//指针
                        {
                            try
                            {
                                IsInitColor = true;
                                ColorDesc = ImageUtils.GetMainColor(bitmap);
                                IsInitColor = false;
                            }
                            catch (Exception ex)
                            {

                                throw ex;
                            }
                            
                        }
                    }
                    bitmap.Dispose();
                }

            }
        }

        bool IsInitColor { get; set; }

        private string? colorDesc;

        public string? ColorDesc
        {
            get { return colorDesc; }
            set
            {
                SetProperty(ref colorDesc, value);
                if (value != null)
                {
                    if (IsPng??false)
                    {
                        var tempcolor = ColorTranslator.FromHtml(value);
                        if (!IsInitColor&&BitmapSource!=null) 
                        {
                            var bitmap = new Bitmap(ImgPath);
                            bitmap = ImageUtils.LayerColored(bitmap, tempcolor);
                            // BitmapSource = BitmapSourceConvert.ToBitmapSource(bitmap);
                            BitmapSource= BitmapSourceConvert.ToBitmapImage(bitmap);
                            RaisePropertyChanged(nameof(BitmapSource));
                            bitmap.Dispose();
                        }
                       
                    }

                }

            }
        }

        private BitmapImage? bitmapSource;

        public BitmapImage? BitmapSource
        {
            get { return bitmapSource; }
            set
            {
                SetProperty(ref bitmapSource, value);

            }
        }

        /// <summary>
        /// 是否PNG
        /// </summary>
        public bool? IsPng => Path.GetExtension(ImgPath ?? "").ToUpper() == ".PNG";

        public string? LayerName => Path.GetFileNameWithoutExtension(ImgPath ?? "");
        public string? Code=> Path.GetFileNameWithoutExtension(ImgPath ?? "").Split("_").Last();
       


        /// <summary>
        /// 图层顺序
        /// </summary>
        private int? layerIndex;

        public int? LayerIndex
        {
            get => layerIndex;
            set => SetProperty(ref layerIndex, value);
        }

        public decimal CenterX => RatioWidth / 2;

        public decimal CenterY => RatioHeight / 2;

        private int angle;
        public int Angle
        {
            get { return angle; }
            set { SetProperty(ref angle, value); }
        }

        //public string?[] ColorDescs { get; set; } = new string?[6] {"Green","Yellow", "Orange","Red","Purple", "Maroon" };
        private string?[] colorDescs = new string?[6];
        public string?[] ColorDescs
        {
            get => colorDescs;
            set
            {
                SetProperty(ref colorDescs, value);
            }
        }
    }
}
