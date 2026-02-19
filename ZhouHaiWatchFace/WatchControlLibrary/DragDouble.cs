using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;
using System.IO;
using System.Collections.ObjectModel;
using WatchControlLibrary.Model;

namespace WatchControlLibrary
{
    public class DragDouble : DragImageSource
    {
        static DragDouble()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DragDouble), new FrameworkPropertyMetadata(typeof(DragDouble)));
        }

        public DragDouble(int maxNum) : base()
        {
            this.MaxNum = maxNum;
        }
        public DragDouble() : base()
        {

        }

        //小数点保留位数
        public int DecimalPlaces { get; set; }

        public double MaxNum
        {
            get { return (double)GetValue(MaxNumProperty); }
            set { SetValue(MaxNumProperty, value); }
        }
        // Using a DependencyProperty as the backing store for MaxNum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxNumProperty =
            DependencyProperty.Register("MaxNum", typeof(double), typeof(DragDouble), new PropertyMetadata(0.0, ValueChanged));


        public double CurrentNum
        {
            get { return (double)GetValue(CurrentNumProperty); }
            set { SetValue(CurrentNumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentNum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentNumProperty =
            DependencyProperty.Register("CurrentNum", typeof(double), typeof(DragDouble), new PropertyMetadata(0.0, ValueChanged));

        private static void ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                var group = (DragDouble)d;
                if (group != null)
                {
                    group.SetSize();
                    group.LoadImages();
                    group.GroupValueChanged?.Invoke();
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
            DependencyProperty.Register("UnitSource", typeof(string), typeof(DragDouble), new PropertyMetadata(string.Empty, ValueChanged));


        public string PointSource
        {
            get { return (string)GetValue(PointSourceProperty); }
            set { SetValue(PointSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PointSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PointSourceProperty =
            DependencyProperty.Register("PointSource", typeof(string), typeof(DragDouble), new PropertyMetadata(string.Empty, ValueChanged));



        public string EmptySource
        {
            get { return (string)GetValue(EmptySourceProperty); }
            set { SetValue(EmptySourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EmptySource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EmptySourceProperty =
            DependencyProperty.Register("EmptySource", typeof(string), typeof(DragDouble), new PropertyMetadata(string.Empty, ValueChanged));

        public bool TrailingZero
        {
            get { return (bool)GetValue(TrailingZeroProperty); }
            set { SetValue(TrailingZeroProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TrailingZero.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TrailingZeroProperty =
            DependencyProperty.Register("TrailingZero", typeof(bool), typeof(DragDouble), new PropertyMetadata(false, ValueChanged));



        public int DecimalOffsetX
        {
            get { return (int)GetValue(DecimalOffsetXProperty); }
            set { SetValue(DecimalOffsetXProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DecimalOffsetX.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DecimalOffsetXProperty =
            DependencyProperty.Register("DecimalOffsetX", typeof(int), typeof(DragDouble), new PropertyMetadata(0, ValueChanged));


        string GetNumStr(bool trailingZero, double currentNum, int decimalPlaces)
        {
            if (trailingZero)
            {
                return Math.Round(currentNum, decimalPlaces).ToString($"0.{new string('0', decimalPlaces)}");
            }
            else
            {
                return Math.Round(currentNum, 1).ToString("0.#");
            }
        }
        public override void LoadImages()
        {
            var canvas = GetTemplateChild("PART_Canvas") as Canvas;
            if (canvas != null)
            {
                canvas.Children.Clear();
                var left = 0.0;
                var top = 0.0;
                var numStr = GetNumStr(TrailingZero, CurrentNum, DecimalPlaces);

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
                        if (!string.IsNullOrWhiteSpace(imgPath) && File.Exists(imgPath))
                        {
                            var image = new System.Windows.Controls.Image
                            {
                                Source = BitmapImageHelper.LoadFromUri(new Uri(CommonHelper.AbsolutePath(imgPath), UriKind.RelativeOrAbsolute)),
                                Width = this.SingleWidth,
                                Height = this.Height,
                            };
                            canvas.Children.Add(image);
                            Canvas.SetLeft(image, left);
                            Canvas.SetTop(image, top);
                            left += SingleWidth;

                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(PointSource) && File.Exists(PointSource))
                        {
                            var bitmap = new Bitmap(PointSource);
                            var image = new System.Windows.Controls.Image
                            {
                                Source = BitmapImageHelper.LoadFromUri(new Uri(CommonHelper.AbsolutePath(PointSource), UriKind.RelativeOrAbsolute)),
                                Width = bitmap.Width,
                                Height = bitmap.Height,
                            };
                            canvas.Children.Add(image);
                            if (DecimalOffsetX != 0)
                            {
                                left += DecimalOffsetX;
                            }
                            Canvas.SetLeft(image, left);
                            Canvas.SetTop(image, top);
                            left += bitmap.Width;
                        }
                    }

                }
                if (!string.IsNullOrWhiteSpace(UnitSource) && File.Exists(UnitSource))
                {
                    var image = new System.Windows.Controls.Image
                    {
                        Source = BitmapImageHelper.LoadFromUri(new Uri(CommonHelper.AbsolutePath(UnitSource), UriKind.RelativeOrAbsolute))
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
                var numStr = GetNumStr(TrailingZero, CurrentNum, DecimalPlaces);
                if (CurrentNum > MaxNum)
                {
                    numStr = MaxNum.ToString();
                }
                var imgPath = ImageSource.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(imgPath))
                {
                    var bitmap = new Bitmap(imgPath);
                    SingleWidth = bitmap.Width;
                    this.Height = bitmap.Height;
                    this.Width = SingleWidth * (numStr.Replace(".", "")).Length;

                    if (numStr.Contains('.') && !string.IsNullOrWhiteSpace(PointSource) && File.Exists(PointSource))
                    {
                        bitmap = new Bitmap(PointSource);
                        this.Width += bitmap.Width;
                        if (this.Height < bitmap.Height)
                        {
                            this.Height = bitmap.Height;
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
                    if (DecimalOffsetX != 0)
                    {
                        double temp = this.Width + DecimalOffsetX;
                        if (temp >= 0) this.Width = temp;
                    }

                }
            }
        }


        public override void OnFolderSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                var group = (DragDouble)d;
                if (group != null && !string.IsNullOrWhiteSpace(e.NewValue?.ToString()))
                {
                    group.SetSize();
                    group.LoadImages();
                }
            }
        }



    }

    public class DragBindDouble : DragBindBase
    {
        //private double maxNum;
        private double currentNum;
        private string? unitSource;
        private ObservableCollection<string>? imageSource;
        private string? pointSource;
        private string? emptySource;
        private int decimalOffsetX;
        private bool trailingZero;
        public int DecimalPlaces { get; set; } = 1;
        public DragBindDouble()
        {
        }

        public DragBindDouble(double maxNum, double minNum, double current)
        {
            this.MaxNum = maxNum;
            this.MinNum = minNum;
            this.CurrentNum = current;
        }



        public double CurrentNum
        {
            get { return Math.Round(currentNum, DecimalPlaces); }
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

        public string? PointSource
        {
            get { return pointSource; }
            set { SetProperty(ref pointSource, value); }
        }
        public string? EmptySource
        {
            get { return emptySource; }
            set { SetProperty(ref emptySource, value); }
        }
        
        public int DecimalOffsetX
        {
            get { return decimalOffsetX; }
            set { SetProperty(ref decimalOffsetX, value); }
        }

        public bool TrailingZero
        {
            get { return trailingZero; }
            set { SetProperty(ref trailingZero, value); }
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
            yield return PointSource;
            yield return EmptySource;
        }
        public override DragDataBaseXml GetOutXml()
        {
            if (VerifyNullNum && this.ImageSource?.Count != 11)
            {
                throw new InvalidOperationException($"{this.DragName} has incorrect image count, please check if no-data image is added");
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
            if (PointSource != null) array.Images!.Add(new WatchImage { Src = PointSource });
            outXml!.ImageArrays!.Add(array);
            var dataItem = new DataItemImageNumber
            {
                Name = OutXmlHelper.GetWatchElementName(),
                Source = DataItemTypeHelper.DataItemTypes[ItemName].ToString(),
                Ref = $"@{array.Name}",
                UnitIcon = unitIcon != null ? $"@{unitIcon.Name}" : "",
                Align = this.Align.ToString(),
                TotalDigits = DataItemTypeHelper.GetTotalDigits(ItemName),
                DecimalDigits = ItemName== "Step Distance" ? 2:1,
                DecimalOffsetX = DecimalOffsetX,
                TrailingZero = TrailingZero,
                RenderRule = "alwaysShow",
            };
            outXml!.DataItemImageNumbers!.Add(dataItem);
            outXml!.Layout = new Layout
            {
                Ref = "@" + dataItem.Name,
                X = (int)Left,
                Y = (int)Top,
            };
            return outXml;
        }
    }

}
