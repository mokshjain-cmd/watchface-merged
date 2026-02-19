using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;
using Control = System.Windows.Controls.Control;
using Image = System.Windows.Controls.Image;
using System.Collections.ObjectModel;
using WatchControlLibrary.Model;

namespace WatchControlLibrary
{

    public class DragNums : DragImageSource
    {
        static DragNums()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DragNums), new FrameworkPropertyMetadata(typeof(DragNums)));
        }

        public DragNums(int maxNum) : base()
        {
            this.MaxNum = maxNum;
        }
        public DragNums() : base()
        {

        }



        public int MaxNum
        {
            get { return (int)GetValue(MaxNumProperty); }
            set { SetValue(MaxNumProperty, value); }
        }
        // Using a DependencyProperty as the backing store for MaxNum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxNumProperty =
            DependencyProperty.Register("MaxNum", typeof(int), typeof(DragNums), new PropertyMetadata(0, ValueChanged));


        public int CurrentNum
        {
            get { return (int)GetValue(CurrentNumProperty); }
            set { SetValue(CurrentNumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentNum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentNumProperty =
            DependencyProperty.Register("CurrentNum", typeof(int), typeof(DragNums), new PropertyMetadata(0, ValueChanged));

        private static void ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                var group = (DragNums)d;
                if (group != null)
                {
                    group.SetSize();
                    group.LoadImages();
                    group?.GroupValueChanged?.Invoke();
                }
            }
        }


        public string? UnitSource
        {
            get { return (string?)GetValue(UnitSourceProperty); }
            set { SetValue(UnitSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UnitSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnitSourceProperty =
            DependencyProperty.Register("UnitSource", typeof(string), typeof(DragNums), new PropertyMetadata(string.Empty, ValueChanged));



        public string MinusSource
        {
            get { return (string)GetValue(MinusSourceProperty); }
            set { SetValue(MinusSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinusSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinusSourceProperty =
            DependencyProperty.Register("MinusSource", typeof(string), typeof(DragNums), new PropertyMetadata(string.Empty, ValueChanged));




        public string EmptySource
        {
            get { return (string)GetValue(EmptySourceProperty); }
            set { SetValue(EmptySourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EmptySource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EmptySourceProperty =
            DependencyProperty.Register("EmptySource", typeof(string), typeof(DragNums), new PropertyMetadata(string.Empty, ValueChanged));


        public override void LoadImages()
        {
            var canvas = GetTemplateChild("PART_Canvas") as Canvas;
            if (canvas != null)
            {
                canvas.Children.Clear();
                var left = 0.0;
                var top = 0.0;
                var numStr = CurrentNum.ToString();
                if (CurrentNum > MaxNum)
                {
                    numStr = MaxNum.ToString();
                }
                foreach (var i in numStr.ToCharArray())
                {
                    if (i >= '0' && i <= '9')
                    {
                        var idx = i - '0';
                        var imgPath = ImageSource?.Skip(idx).FirstOrDefault();
                        if (!string.IsNullOrWhiteSpace(imgPath)&&File.Exists(imgPath))
                        {
                            var image = new Image
                            {
                                Source = BitmapImageHelper.LoadFromUri(new Uri(CommonHelper.AbsolutePath(imgPath), UriKind.RelativeOrAbsolute)),
                                Width = this.SingleWidth,
                                Height = this.Height,
                            };
                          // var align= DraggableBehavior.GetAlign(this);
                           
                            canvas.Children.Add(image);
                            Canvas.SetLeft(image, left);
                            Canvas.SetTop(image, top);
                            left += SingleWidth;

                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(MinusSource) && File.Exists(MinusSource))
                        {
                            var bitmap = new Bitmap(MinusSource);
                            var image = new Image
                            {
                                Source = BitmapImageHelper.LoadFromUri(new Uri(CommonHelper.AbsolutePath(MinusSource), UriKind.RelativeOrAbsolute)),
                                Width = bitmap.Width,
                                Height = bitmap.Height,
                            };
                            canvas.Children.Add(image);
                            Canvas.SetLeft(image, left);
                            Canvas.SetTop(image, top);
                            left += bitmap.Width;
                        }
                    }

                }
                if (!string.IsNullOrWhiteSpace(UnitSource) && File.Exists(UnitSource))
                {
                    var bitmap = new Bitmap(UnitSource);
                    var image = new Image
                    {
                        Source = BitmapImageHelper.LoadFromUri(new Uri(CommonHelper.AbsolutePath(UnitSource), UriKind.RelativeOrAbsolute)),
                        Width=bitmap.Width,
                        Height = bitmap.Height,
                    };
                    canvas.Children.Add(image);
                    Canvas.SetLeft(image, left);
                    Canvas.SetTop(image, top);
                }
                DraggableBehavior.SetPostion(this);
                
            }
        }

        internal int SingleWidth;

        public override void SetSize()
        {
            if (ImageSource?.Any() ?? false)
            {
                var imgPath = ImageSource.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(imgPath))
                {
                    var bitmap = new Bitmap(imgPath);
                    SingleWidth = bitmap.Width;
                    this.Height = bitmap.Height;
                    this.Width = SingleWidth * (Math.Abs(CurrentNum).ToString()).Length;
                    if (CurrentNum < 0)
                    {
                        if (!string.IsNullOrWhiteSpace(MinusSource) && File.Exists(MinusSource))
                        {
                            bitmap = new Bitmap(MinusSource);
                            this.Width += bitmap.Width;
                            if (this.Height < bitmap.Height)
                            {
                                this.Height = bitmap.Height;
                            }
                        }
                    }
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

        public override void OnFolderSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                var group = (DragNums)d;
                if (group != null && !string.IsNullOrWhiteSpace(e.NewValue?.ToString()))
                {
                    group.SetSize();
                    group.LoadImages();
                }
            }
        }

       
    }

    public class DragBindNums : DragBindBase
    {
        //private int maxNum;
        private int currentNum;
        private string? unitSource;
        private ObservableCollection<string>? imageSource;
        private string? minusSource;
        private string? emptySource;

        public DragBindNums()
        {
        }

        public DragBindNums(int maxNum, int minNum, double current )
        {
            this.MaxNum = maxNum;
            this.MinNum = minNum;
            CurrentNum = (int)current;
        }



        public int CurrentNum
        {
            get { return currentNum; }
            set { SetProperty(ref currentNum, value); }
        }

        public string? UnitSource
        {
            get { return unitSource; }
            set { SetProperty(ref unitSource, value); }
        }

        public ObservableCollection<string>? ImageSource
        {
            get { return imageSource; }
            set { SetProperty(ref imageSource, value); }
        }

        public string? MinusSource
        {
            get { return minusSource; }
            set { SetProperty(ref minusSource, value); }
        }
        public string? EmptySource
        {
            get { return emptySource; }
            set { SetProperty(ref emptySource, value); }
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
            yield return UnitSource;
            yield return MinusSource;
            yield return EmptySource;
        }

        public override DragDataBaseXml GetOutXml()
        {
            if (VerifyNullNum)
            {
                // Check that we have 10 digit images + 1 empty/no-data image
                int totalImages = (this.ImageSource?.Count ?? 0) + (string.IsNullOrWhiteSpace(EmptySource) ? 0 : 1);
                if (totalImages != 11)
                {
                    throw new InvalidOperationException($"{this.DragName}的图片数量不正确，请检查是否添加无数据图片");
                }
            }
            var outXml = new DragDataBaseXml();
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
            var array = new ImageArray
            {
                Name = OutXmlHelper.GetWatchElementName(),
                Images = this.ImageSource?.Select(x => new WatchImage
                {
                    Src = x,
                }).ToList(),
            };
            if (EmptySource != null) array.Images!.Add(new WatchImage { Src = EmptySource });
            if (MinusSource != null) array.Images!.Add(new WatchImage { Src = MinusSource });
            outXml.ImageArrays!.Add(array);
            if (ItemName == "血压舒张压" || ItemName == "血压收缩压")
            {
                foreach (var item in DataItemTypeHelper.DataItemTypes2[ItemName])
                {
                    var dataItem = new DataItemImageNumber
                    {
                        Name = OutXmlHelper.GetWatchElementName(),
                        Source = item.ToString(),
                        Ref = $"@{array.Name}",
                        UnitIcon = unitIcon != null ? $"@{unitIcon.Name}" : "",
                        Align = this.Align.ToString(),
                        TotalDigits = DataItemTypeHelper.GetTotalDigits(ItemName),
                        RenderRule = "alwaysShow",
                    };
                    outXml!.DataItemImageNumbers!.Add(dataItem);
                    outXml!.Layout=new Layout
                    {
                        Ref = "@" + dataItem.Name,
                        X = (int)Left,
                        Y = (int)Top,
                    };
                }
            }
            else
            {
                var dataItem = new DataItemImageNumber
                {
                    Name = OutXmlHelper.GetWatchElementName(),
                    Source = DataItemTypeHelper.DataItemTypes[ItemName].ToString(),
                    Ref = $"@{array.Name}",
                    UnitIcon = unitIcon != null ? $"@{unitIcon.Name}" : "",
                    Align = this.Align.ToString(),
                    TotalDigits = DataItemTypeHelper.GetTotalDigits(ItemName),
                    RenderRule = "alwaysShow",
                };
                outXml!.DataItemImageNumbers!.Add(dataItem);
                outXml.Layout=new Layout
                {
                    Ref = "@" + dataItem.Name,
                    X = (int)Left,
                    Y = (int)Top,
                };
            }
            return outXml;
        }
    }


    public enum Align
    {
        left,
        center,
        right,
    }
}
