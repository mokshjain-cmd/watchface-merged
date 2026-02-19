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
using System.Net.WebSockets;
using Newtonsoft.Json.Linq;

namespace WatchControlLibrary
{
    public class DragNormalDateTime : DragImageSource
    {
        static DragNormalDateTime()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DragNormalDateTime), new FrameworkPropertyMetadata(typeof(DragNormalDateTime)));
        }

        public bool? LeadingZero
        {
            get { return (bool?)GetValue(LeadingZeroProperty); }
            set { SetValue(LeadingZeroProperty, value); }
        }
        // Using a DependencyProperty as the backing store for LeadingZero.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeadingZeroProperty =
            DependencyProperty.Register("LeadingZero", typeof(bool?), typeof(DragNormalDateTime), new PropertyMetadata(false, ValueOnChanged));

        public DateTime? SetDateTime
        {
            get { return (DateTime?)GetValue(SetDateTimeProperty); }
            set { SetValue(SetDateTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SetDateTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SetDateTimeProperty =
            DependencyProperty.Register("SetDateTime", typeof(DateTime?), typeof(DragNormalDateTime), new PropertyMetadata(default(DateTime?), ValueOnChanged));

        private static void ValueOnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                var group = (DragNormalDateTime)d;
                if (group != null)
                {
                    group.SetSize();
                    group.LoadImages();
                    group?.GroupValueChanged?.Invoke();
                }
            }
        }
        public string? DateTimeFormat
        {
            get { return (string?)GetValue(DateTimeFormatProperty); }
            set { SetValue(DateTimeFormatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DateTimeFormat.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DateTimeFormatProperty =
            DependencyProperty.Register("DateTimeFormat", typeof(string), typeof(DragNormalDateTime), new PropertyMetadata(string.Empty, ValueOnChanged));

        public string? UnitIcon
        {
            get { return (string?)GetValue(UnitIconProperty); }
            set { SetValue(UnitIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UnitIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnitIconProperty =
            DependencyProperty.Register("UnitIcon", typeof(string), typeof(DragNormalDateTime), new PropertyMetadata(string.Empty, ValueOnChanged));


        public DragNormalDateTime() : base()
        {

        }

        int SingleWidth;
        public override void SetSize()
        {
            if (ImageSource?.Any() ?? false)
            {
                var str = DragDateTimeHelper.GetDateTime(DateTimeFormat!, LeadingZero ?? false, !string.IsNullOrWhiteSpace(UnitIcon), SetDateTime ?? default(DateTime));
                var imgPath = ImageSource.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(imgPath) && File.Exists(imgPath))
                {
                    var bitmap = new Bitmap(imgPath);
                    SingleWidth = bitmap.Width;
                    this.Height = bitmap.Height;
                    this.Width = SingleWidth * str.Replace(DragDateTimeHelper.iconStr, "").Length;
                    if (!string.IsNullOrWhiteSpace(UnitIcon) && File.Exists(UnitIcon))
                    {
                        bitmap = new Bitmap(UnitIcon);
                        this.Width += bitmap.Width;
                        if (bitmap.Height > this.Height)
                        {
                            this.Height = bitmap.Height;
                        }
                    }
                }
            }
        }

        public override void LoadImages()
        {
            var canvas = GetTemplateChild("PART_Canvas") as Canvas;
            if (canvas != null && !string.IsNullOrWhiteSpace(DateTimeFormat) && SetDateTime != null)
            {
                canvas.Children.Clear();
                var left = 0.0;
                var top = 0.0;
                var datatimestr = DragDateTimeHelper.GetDateTime(DateTimeFormat!, LeadingZero ?? false, !string.IsNullOrWhiteSpace(UnitIcon), SetDateTime ?? default(DateTime));

                foreach (var i in datatimestr.ToCharArray())
                {
                    if (i >= '0' && i <= '9')
                    {
                        var idx = i - '0';
                        var imgPath = ImageSource?.Skip(idx).FirstOrDefault();
                        if (!string.IsNullOrWhiteSpace(imgPath) && File.Exists(imgPath))
                        {

                            var image = new System.Windows.Controls.Image
                            {
                                Source = BitmapImageHelper.LoadFromUri(new Uri($"{CommonHelper.AbsolutePath(imgPath)}", UriKind.RelativeOrAbsolute)),
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
                        if (!string.IsNullOrWhiteSpace(UnitIcon) && File.Exists(UnitIcon))
                        {
                            var bitmap = new Bitmap(UnitIcon);
                            var image = new System.Windows.Controls.Image
                            {
                                Source = BitmapImageHelper.LoadFromUri(new Uri(CommonHelper.AbsolutePath(UnitIcon), UriKind.RelativeOrAbsolute)),
                                Width = bitmap.Width,
                                Height = bitmap.Height,
                            };
                            canvas.Children.Add(image);
                            Canvas.SetLeft(image, left);
                            Canvas.SetTop(image, top);
                            left += image.Width;
                        }
                    }
                }
                DraggableBehavior.SetPostion(this);
                
            }
        }

        public override void OnFolderSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                var group = (DragNormalDateTime)d;
                if (group != null && !string.IsNullOrWhiteSpace(e.NewValue?.ToString()))
                {

                    group.SetSize();
                    group.LoadImages();
                }
            }
        }


    }

    public class DragBindNormalDateTime : DragBindBase
    {
        private string? dateTimeFormat;
        private string? unitIcon;
        private ObservableCollection<string>? imageSource;
        private DateTime? setDateTime;
        private bool leadingZero;
       
        public string? DateTimeFormat
        {
            get { return dateTimeFormat; }
            set { SetProperty(ref dateTimeFormat, value); }
        }

        public string? UnitIcon
        {
            get { return unitIcon; }
            set { SetProperty(ref unitIcon, value); }
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

        public bool LeadingZero
        {
            get { return leadingZero; }
            set { SetProperty(ref leadingZero, value); }
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
            yield return UnitIcon;
        }
        public override DragDataBaseXml GetOutXml()
        {
            var outXml = new DragDataBaseXml();
            WatchImage? unitIcon = null;
            if (!string.IsNullOrWhiteSpace(UnitIcon))
            {
                unitIcon = new WatchImage
                {
                    Src = UnitIcon,
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
            outXml!.ImageArrays!.Add(array);
            if (ItemName == "Time") 
            {
                var hourItem = new DataItemImageNumber
                {
                    Name = OutXmlHelper.GetWatchElementName(),
                    Source = DataItemTypeHelper.DataItemTypes["Hour"].ToString(),
                    Ref = $"@{array.Name}",
                    LeadingZero = LeadingZero,
                    Align = this.Align.ToString(),
                    TotalDigits = DataItemTypeHelper.GetTotalDigits(ItemName),
                };
                outXml!.DataItemImageNumbers!.Add(hourItem);
                var minItem = new DataItemImageNumber
                {
                    Name = OutXmlHelper.GetWatchElementName(),
                    Source = DataItemTypeHelper.DataItemTypes["Minute"].ToString(),
                    Ref = $"@{array.Name}",
                    LeadingZero = true,
                    Align = this.Align.ToString(),
                    TotalDigits = DataItemTypeHelper.GetTotalDigits(ItemName),
                };
                outXml!.DataItemImageNumbers!.Add(minItem);
                Widget widget = new Widget 
                {
                   Name=OutXmlHelper.GetWatchElementName(),
                   FlexDirection= "row",
                   JustifyContent= "flex-start",
                   AlignItems= "center",
                   Width = System.Drawing.Image.FromFile(imageSource[0]).Width * 4,
                   Height =(int)this.Height, 
                   Items=new List<Item> 
                   {
                      new Item{ Ref=$"@{hourItem.Name}", X=(int)this.Left,Y=(int)this.Top },
                      new Item{ Ref=$"{(unitIcon!=null?$"@{unitIcon.Name}":"")}", X=(int)this.Left,Y=(int)this.Top },
                      new Item{ Ref=$"@{minItem.Name}", X=(int)this.Left,Y=(int)this.Top },
                   },
                };
                if (!string.IsNullOrEmpty(UnitIcon)) widget.Width += System.Drawing.Image.FromFile(UnitIcon).Width;
                outXml.Widgets!.Add(widget);
                outXml!.Layout=new Layout 
                {
                    Ref=$"@{widget.Name}",
                    X= GetLeft(),
                    Y =(int)this.Top,
                };
            }
            else 
            {
                var dataItem = new DataItemImageNumber
                {
                    Name = OutXmlHelper.GetWatchElementName(),
                    Source = DataItemTypeHelper.DataItemTypes[ItemName].ToString(),
                    Ref = $"@{array.Name}",
                    UnitIcon = unitIcon != null ? $"@{unitIcon.Name}" : "",
                    LeadingZero = LeadingZero,
                    Align = this.Align.ToString(),
                    TotalDigits = DataItemTypeHelper.GetTotalDigits(ItemName),
                };
                outXml!.DataItemImageNumbers!.Add(dataItem);
                Layout layout = new Layout
                {
                    Ref = $"@{dataItem.Name}",
                    X = (int)this.Left,
                    Y = (int)this.Top,
                };
                outXml!.Layout=layout;
            }
            return outXml;
        }

       
       

    }
}
