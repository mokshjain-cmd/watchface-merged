using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WatchControlLibrary.Model;
using Image = System.Windows.Controls.Image;

namespace WatchControlLibrary
{
    public class DragProgress : DragImageSource
    {
        static DragProgress()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DragProgress), new FrameworkPropertyMetadata(typeof(DragProgress)));
        }

        public DragProgress(int maxNum) : base()
        {
            this.MaxNum = maxNum;
        }
        public DragProgress() : base()
        {

        }

        public int MaxNum
        {
            get { return (int)GetValue(MaxNumProperty); }
            set { SetValue(MaxNumProperty, value); }
        }
        // Using a DependencyProperty as the backing store for MaxNum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxNumProperty =
            DependencyProperty.Register("MaxNum", typeof(int), typeof(DragProgress), new PropertyMetadata(0, SetSizeChanged));
        private static void SetSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                var group = (DragProgress)d;
                if (group != null)
                {
                    group.SetSize();
                    group.LoadImages();
                }
            }
        }

        public int CurrentNum
        {
            get { return (int)GetValue(CurrentNumProperty); }
            set { SetValue(CurrentNumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentNum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentNumProperty =
            DependencyProperty.Register("CurrentNum", typeof(int), typeof(DragProgress), new PropertyMetadata(0, ValueChanged));




        public int RatioNum
        {
            get { return (int)GetValue(RatioNumProperty); }
            set { SetValue(RatioNumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RatioNum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RatioNumProperty =
            DependencyProperty.Register("RatioNum", typeof(int), typeof(DragProgress), new PropertyMetadata(0, ValueChanged));



        public int TargetValue
        {
            get { return (int)GetValue(TargetValueProperty); }
            set { SetValue(TargetValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TargetValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetValueProperty =
            DependencyProperty.Register("TargetValue", typeof(int), typeof(DragProgress), new PropertyMetadata(0, ValueChanged));



        public bool FillType  // true 普通填充  false 数字填充
        {
            get { return (bool)GetValue(FillTypeProperty); }
            set { SetValue(FillTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FillType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FillTypeProperty =
            DependencyProperty.Register("FillType", typeof(bool), typeof(DragProgress), new PropertyMetadata(true, ValueChanged));


        public string? UnitSource
        {
            get { return (string?)GetValue(UnitSourceProperty); }
            set { SetValue(UnitSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UnitSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnitSourceProperty =
            DependencyProperty.Register("UnitSource", typeof(string), typeof(DragProgress), new PropertyMetadata(string.Empty, ValueChanged));

        private static void ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                var group = (DragProgress)d;
                if (group != null)
                {
                    group.SetSize();
                    group.LoadImages();
                    group?.GroupValueChanged?.Invoke();
                }
            }
        }

        public override void LoadImages()
        {
            var canvas = GetTemplateChild("PART_Canvas") as Canvas;
            if (canvas != null)
            {
                canvas.Children.Clear();

                if (ImageSource?.Any() ?? false)
                {
                    if (TargetValue == 0)
                    {
                        var image = new Image
                        {
                            Source = BitmapImageHelper.LoadFromUri(new Uri(CommonHelper.AbsolutePath(ImageSource[0]), UriKind.RelativeOrAbsolute)),
                            Width = this.SingleWidth,
                            Height = this.Height,
                        };
                        canvas.Children.Add(image);
                        return;
                    }
                    if (FillType) //true 普通填充
                    {
                        var count = ImageSource.Count()-1;
                        var per = (decimal)TargetValue / count;
                        var idx = (decimal)CurrentNum / per;
                        int roundedIdx = (int)(Math.Floor(idx));
                        var imgPath = ImageSource?.Skip(roundedIdx).FirstOrDefault();
                        if (CurrentNum >= TargetValue) 
                        {
                            imgPath = ImageSource[ImageSource.Count - 1];
                        }
                        if (!string.IsNullOrWhiteSpace(imgPath) && File.Exists(imgPath))
                        {
                            var image = new Image
                            {
                                Source = BitmapImageHelper.LoadFromUri(new Uri(CommonHelper.AbsolutePath(imgPath), UriKind.RelativeOrAbsolute)),
                                Width = this.Width,
                                Height = this.Height,
                            };
                            canvas.Children.Add(image);
                        }
                    }
                    else  // false 数字填充
                    {
                        var left = 0;
                        var num = ((int)(CurrentNum * 1.0 / TargetValue * 100)).ToString();
                        foreach (var i in num.ToCharArray())
                        {
                            if (i >= '0' && i <= '9')
                            {
                                var idx = i - '0';
                                var imgPath = ImageSource?.Skip(idx).FirstOrDefault();
                                if (!string.IsNullOrWhiteSpace(imgPath) && File.Exists(imgPath))
                                {
                                    var image = new Image
                                    {
                                        Source = BitmapImageHelper.LoadFromUri(new Uri(CommonHelper.AbsolutePath(imgPath), UriKind.RelativeOrAbsolute)),
                                        Width = this.SingleWidth,
                                        Height = this.Height,
                                    };
                                    canvas.Children.Add(image);
                                    Canvas.SetLeft(image, left);
                                    Canvas.SetTop(image, 0);
                                    left += SingleWidth;

                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(UnitSource) && File.Exists(UnitSource))
                        {
                            var bitmap = new Bitmap(UnitSource);
                            var image = new Image
                            {
                                Source = BitmapImageHelper.LoadFromUri(new Uri(CommonHelper.AbsolutePath(UnitSource), UriKind.RelativeOrAbsolute)),
                                Width = bitmap.Width,
                                Height = bitmap.Height,
                            };
                            canvas.Children.Add(image);
                            Canvas.SetLeft(image, left);
                            Canvas.SetTop(image, 0);
                        }
                    }
                }
                DraggableBehavior.SetPostion(this);
                
            }
        }

        internal int SingleWidth;

        public override void SetSize()
        {
            //if (ImageSource?.Any() ?? false)
            //{
            //    var imgPath = ImageSource.FirstOrDefault();
            //    if (!string.IsNullOrWhiteSpace(imgPath))
            //    {
            //        var bitmap = new Bitmap(imgPath);
            //        this.Height = bitmap.Height;
            //        this.Width = bitmap.Width; ;
            //        SingleWidth = bitmap.Width;
            //    }
            //}
            if (ImageSource?.Any() ?? false)
            {
                var imgPath = ImageSource.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(imgPath))
                {
                    var bitmap = new Bitmap(imgPath);
                    SingleWidth = bitmap.Width;
                    this.Height = bitmap.Height;
                    if (FillType)
                    {
                        this.Width = bitmap.Width;
                    }
                    else
                    {

                        this.Width = TargetValue == 0 ? bitmap.Width : SingleWidth * (Math.Abs((int)(CurrentNum * 1.0 / TargetValue * 100)).ToString()).Length;
                        if (!string.IsNullOrWhiteSpace(UnitSource) && File.Exists(UnitSource))
                        {
                            bitmap = new Bitmap(UnitSource);
                            this.Width += bitmap.Width;
                            if (this.Height < bitmap.Height)
                            {
                                this.Height = bitmap.Height;
                            }
                        }
                    }

                }
            }
        }


        public override void OnFolderSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                var group = (DragProgress)d;
                if (group != null && !string.IsNullOrWhiteSpace(e.NewValue?.ToString()))
                {

                    group.SetSize();
                    group.LoadImages();
                }
            }
        }
    }

    public class DragBindProgress : DragBindBase
    {
        private int maxNum;
        private int currentNum;
        private int targetValue;
        private bool fillType = true;
        private string unitSource;
        private ObservableCollection<string>? imageSource;

        public DragBindProgress()
        {
        }

        public DragBindProgress(int maxNum, int minNum, double current)
        {
            this.MaxNum = maxNum;
            this.MinNum = minNum;
            CurrentNum = (int)current;
        }

        bool isUpdating = false;//避免无限递归
        public int CurrentNum
        {
            get { return currentNum; }
            set
            {
                SetProperty(ref currentNum, value);
                if (TargetValue > 0 && !isUpdating) //普通填充
                {
                    isUpdating = true;
                    RatioNum = (int)(((decimal)CurrentNum / TargetValue)*100 );
                    isUpdating = false;
                }
            }
        }

        private int ratioNum;

        public int RatioNum
        {
            get { return ratioNum; }
            set
            {
                SetProperty(ref ratioNum, value);
                if (!isUpdating)
                {
                    isUpdating = true;
                    CurrentNum = (int)(RatioNum * 0.01 * TargetValue);
                    isUpdating = false;
                }
            }
        }


        public int TargetValue
        {
            get { return targetValue; }
            set { if (value > MaxNum && MaxNum != 0) value = (int)MaxNum; SetProperty(ref targetValue, value); }
        }
        public string? UnitSource
        {
            get { return unitSource; }
            set { SetProperty(ref unitSource, value); }
        }
        public bool FillType
        {
            get { return fillType; }
            set { SetProperty(ref fillType, value); }
        }
        public ObservableCollection<string>? ImageSource
        {
            get { return imageSource; }
            set { SetProperty(ref imageSource, value); }
        }

        public override IEnumerable<string?>? GetAllImages()
        {
            if (ImageSource != null && ImageSource.Any())
            {
                foreach (var image in ImageSource)
                {
                    yield return image;
                }
            }
        }

        public override DragDataBaseXml GetOutXml()
        {
            if (VerifyNullNum && this.ImageSource?.Count != 11)
            {
                throw new InvalidOperationException($"{this.DragName}的图片数量不正确，请检查是否添加无数据图片");
            }
            var outXml = new DragDataBaseXml();
            var array = new ImageArray
            {
                Name = OutXmlHelper.GetWatchElementName(),
                Images = this.ImageSource?.Select(x => new WatchImage
                {
                    Src = x,
                }).ToList(),
            };
            outXml!.ImageArrays!.Add(array);
            int interval = 100 / (ImageSource.Count() - 1);
            var itemName=string.Empty;
            if (this.FillType) 
            {
                var dataItem = new DataItemImageValues
                {
                    Name = OutXmlHelper.GetWatchElementName(),
                    Source = DataItemTypeHelper.DataItemTypes[ItemName].ToString(),
                    Ref = $"@{array.Name}",
                    Params = Enumerable.Range(0, imageSource.Count()).Select(i => new Param { Value = i * interval }).ToList(),

                };
                outXml!.DataItemImageValues!.Add(dataItem);
                itemName = dataItem.Name;
               
            }
            else 
            {
                WatchImage? unitIcon = null;
                if (!string.IsNullOrWhiteSpace(UnitSource))
                {
                    unitIcon = new WatchImage
                    {
                        Src = UnitSource,
                        Name = OutXmlHelper.GetWatchElementName(),
                    };
                }
                if (unitIcon != null)
                {
                    outXml!.Images!.Add(unitIcon);
                }
                var dataItem = new DataItemImageNumber
                {
                    Name = OutXmlHelper.GetWatchElementName(),
                    Source = DataItemTypeHelper.DataItemTypes[ItemName].ToString(),
                    Ref = $"@{array.Name}",
                    Align = this.Align.ToString(),
                    TotalDigits = DataItemTypeHelper.GetTotalDigits(ItemName),
                    RenderRule = "alwaysShow",
                    UnitIcon = unitIcon != null ? $"@{unitIcon.Name}" : "",

                };
                outXml!.DataItemImageNumbers!.Add(dataItem);
                itemName = dataItem.Name;
            }
            outXml!.Layout = new Layout
            {
                Ref = "@" + itemName,
                X = FillType?GetLeft():(int)Left,
                Y = (int)Top,
            };

            return outXml;
        }
    }

}
