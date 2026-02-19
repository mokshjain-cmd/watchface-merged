using Mapster.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WatchControlLibrary.Model;

namespace WatchControlLibrary
{
    public class DragSingleDigit : DragImageSource
    {
        public DragSingleDigit() : base() { }

        public DateTime? SetDateTime
        {
            get { return (DateTime?)GetValue(SetDateTimeProperty); }
            set { SetValue(SetDateTimeProperty, value); CurrentNum = null; }
        }

        // Using a DependencyProperty as the backing store for SetDateTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SetDateTimeProperty =
            DependencyProperty.Register("SetDateTime", typeof(DateTime?), typeof(DragSingleDigit), new PropertyMetadata(default(DateTime?), ValueOnChanged));

        public string? DateTimeFormat
        {
            get { return (string?)GetValue(DateTimeFormatProperty); }
            set { SetValue(DateTimeFormatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DateTimeFormat.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DateTimeFormatProperty =
            DependencyProperty.Register("DateTimeFormat", typeof(string), typeof(DragSingleDigit), new PropertyMetadata(string.Empty, ValueOnChanged));



        public int? CurrentNum
        {
            get { return (int?)GetValue(CurrentNumProperty); }
            set { SetValue(CurrentNumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentNum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentNumProperty =
            DependencyProperty.Register("CurrentNum", typeof(int?), typeof(DragSingleDigit), new PropertyMetadata(0, ValueOnChanged));


        private static void ValueOnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                var group = (DragSingleDigit)d;
                if (group != null)
                {
                    group.SetSize();
                    group.LoadImages();
                }
            }
        }

        public override void SetSize()
        {
            if (ImageSource?.Any() ?? false)
            {
                var imgPath = ImageSource.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(imgPath) && File.Exists(imgPath))
                {
                    var bitmap = new Bitmap(imgPath);
                    this.Width = bitmap.Width;
                    this.Height = bitmap.Height;
                }
            }
        }

        public override void LoadImages()
        {
            var canvas = GetTemplateChild("PART_Canvas") as Canvas;
            if (canvas != null && !string.IsNullOrWhiteSpace(DateTimeFormat) /*&& SetDateTime != null*/)
            {
                canvas.Children.Clear();
                //var left = 0.0;
                //var top = 0.0;
                int index;
                if (SetDateTime != null)   // 时间
                {
                    int num = DateTimeFormat switch
                    {
                        string s when s.Contains("小时")  => SetDateTime.Value.Hour,
                        string s when s.Contains('年') => SetDateTime.Value.Year,
                        string s when s.Contains('分') => SetDateTime.Value.Minute,
                        string s when s.Contains('秒') => SetDateTime.Value.Second,
                        string s when s.Contains('月') => SetDateTime.Value.Month,
                        string s when s.Contains('日') => SetDateTime.Value.Day,
                        _ => throw new Exception("未知类型")
                    };
                    index = GetSingleNum(num, DateTimeFormat);
                }
                else  // 数字
                {
                    index = GetSingleNum(CurrentNum!.Value, DateTimeFormat);
                }
                var imgPath = ImageSource[index];
                if (!string.IsNullOrWhiteSpace(imgPath) && File.Exists(imgPath))
                {
                    var image = new System.Windows.Controls.Image
                    {
                        Source = BitmapImageHelper.LoadFromUri(new Uri(CommonHelper.AbsolutePath(imgPath), UriKind.RelativeOrAbsolute)),
                        Width = this.Width,
                        Height = this.Height,
                    };
                    canvas.Children.Add(image);
                    Canvas.SetLeft(image, 0);
                    Canvas.SetTop(image, 0);
                }
            }
        }

        private int GetSingleNum(int num, string dateTimeFormat)
        {
            return dateTimeFormat switch
            {
                string s when s.Contains('万') => num / 10000,
                string s when s.Contains('千') => (num / 1000) % 10,
                string s when s.Contains('百') => (num / 100) % 10,
                string s when s.Contains('十') => (num / 10) % 10,
                string s when s.Contains('个') => num  % 10,
                _ => throw new Exception("未知错误")
            };
        }

        public override void OnFolderSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                var group = (DragSingleDigit)d;
                if (group != null && !string.IsNullOrWhiteSpace(e.NewValue?.ToString()))
                {
                    group.SetSize();
                    group.LoadImages();
                }
            }
        }

       
    }

    public class DragBindSingleDigit : DragBindBase
    {
        private string? dateTimeFormat;
        private DateTime? setDateTime;
        private int? currentNum=0;
        private ObservableCollection<string>? imageSource;
        public string? DateTimeFormat
        {
            get { return dateTimeFormat; }
            set { SetProperty(ref dateTimeFormat, value); }
        }

        public ObservableCollection<string>? ImageSource
        {
            get { return imageSource; }
            set { SetProperty(ref imageSource, value); }
        }

        public DateTime? SetDateTime
        {
            get { return setDateTime; }
            set { SetProperty(ref setDateTime, value); }
        }

        public int? CurrentNum
        {
            get { return currentNum; }
            set { SetProperty(ref currentNum, value); }
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

            var dataItem = new DataItemImageValues
            {
                Name = OutXmlHelper.GetWatchElementName(),
                Source = DataItemTypeHelper.DataItemTypes[ItemName].ToString(),
                Ref = $"@{array.Name}",
            };
            outXml!.DataItemImageValues!.Add(dataItem);

            Layout layout = new Layout
            {
                Ref = $"@{dataItem.Name}",
                X = (int)this.Left,
                Y = (int)this.Top,
            };
            outXml!.Layout = layout;
            return outXml;
        }
    }

    public class IsShowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
                return value is null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
