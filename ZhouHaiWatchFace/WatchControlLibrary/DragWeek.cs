using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Collections.ObjectModel;
using System.IO;
using WatchControlLibrary.Model;

namespace WatchControlLibrary
{
    public class DragWeek : DragImageSource
    {
        static DragWeek()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DragWeek), new FrameworkPropertyMetadata(typeof(DragWeek)));
        }

        public DateTime? SetDateTime
        {
            get { return (DateTime?)GetValue(SetDateTimeProperty); }
            set { SetValue(SetDateTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SetDateTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SetDateTimeProperty =
            DependencyProperty.Register("SetDateTime", typeof(DateTime?), typeof(DragWeek), new PropertyMetadata(default(DateTime?), ValueOnChanged));




        public DateTimeType DateTimeType
        {
            get { return (DateTimeType)GetValue(DateTimeTypeProperty); }
            set { SetValue(DateTimeTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DateTimeType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DateTimeTypeProperty =
            DependencyProperty.Register("DateTimeType", typeof(DateTimeType), typeof(DragWeek), new PropertyMetadata(DateTimeType.Month, ValueOnChanged));



        private static void ValueOnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                var group = (DragWeek)d;
                if (group != null)
                {
                    group.SetSize();
                    group.LoadImages();
                }
            }
        }


        public DragWeek() : base()
        {

        }

        public override void SetSize()
        {
            if (ImageSource?.Any() ?? false)
            {
                var imgPath = ImageSource.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(imgPath))
                {
                    var bitmap = new Bitmap(imgPath);
                    this.Height = bitmap.Height;
                    this.Width = bitmap.Width; ;
                }
            }
        }

        public override void LoadImages()
        {
            var canvas = GetTemplateChild("PART_Canvas") as Canvas;
            if (canvas != null && SetDateTime != null)
            {
                canvas.Children.Clear();
                var index = GetIndex(DateTimeType)??0;
                if ((ImageSource?.Any() ?? false))
                {
                    var imgPath = ImageSource?.Skip(index).FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(imgPath)&&File.Exists(imgPath))
                    {
                        var image = new System.Windows.Controls.Image
                        {
                            Source = BitmapImageHelper.LoadFromUri(new Uri(CommonHelper.AbsolutePath(imgPath), UriKind.RelativeOrAbsolute)),
                            Width = this.Width,
                            Height = this.Height,
                        };
                        canvas.Children.Add(image);

                    }
                }
            }
        }

        public override void OnFolderSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                var group = (DragWeek)d;
                if (group != null && !string.IsNullOrWhiteSpace(e.NewValue?.ToString()))
                {
                    group.SetSize();
                    group.LoadImages();
                }
            }
        }
        public int? GetIndex(DateTimeType dateTimeType)
        {
            if (SetDateTime != null)
            {
                return dateTimeType switch
                {
                    DateTimeType.Month => SetDateTime.Value.Month - 1,
                    DateTimeType.Day => SetDateTime.Value.Day - 1,
                    DateTimeType.Hour => SetDateTime.Value.Hour,
                    DateTimeType.Minute => SetDateTime.Value.Minute,
                    DateTimeType.Second => SetDateTime.Value.Second,
                    DateTimeType.Week => (int)SetDateTime!.Value.DayOfWeek,
                    _ => throw new Exception("不支持的时间类型")
                };
            }
            return null;
        }

        
    }

    public class DragBindWeek : DragBindBase
    {
        private DateTime? setDateTime;
        private ObservableCollection<string>? imageSource;

        public DateTime? SetDateTime
        {
            get { return setDateTime; }
            set { SetProperty(ref setDateTime, value); }
        }

        public ObservableCollection<string>? ImageSource
        {
            get { return imageSource; }
            set { SetProperty(ref imageSource, value); }
        }

        private DateTimeType dateTimeType;

        public DateTimeType DateTimeType
        {
            get { return dateTimeType; }
            set { SetProperty(ref dateTimeType, value); }
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
                Params = DataItemTypeHelper.GetParams(ItemName),
            };
            outXml!.DataItemImageValues!.Add(dataItem);
            outXml!.Layout= new Layout
            {
                Ref = "@" + dataItem.Name,
                X = GetLeft(),
                Y = (int)Top,
            };
            return outXml;
        }
    }

    public enum DateTimeType
    {
        Month,
        Day,
        Hour,
        Minute,
        Second,
        Week,
    }

    

}
