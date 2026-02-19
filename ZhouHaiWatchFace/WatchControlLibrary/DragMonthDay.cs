using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.IO;
using System.Drawing;
using System.Collections.ObjectModel;
using WatchControlLibrary.Model;

namespace WatchControlLibrary
{
    public class DragMonthDay : DragImageSource
    {
        static DragMonthDay()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DragMonthDay), new FrameworkPropertyMetadata(typeof(DragMonthDay)));
        }

        public bool? LeadingZero
        {
            get { return (bool?)GetValue(LeadingZeroProperty); }
            set { SetValue(LeadingZeroProperty, value); }
        }
        // Using a DependencyProperty as the backing store for LeadingZero.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeadingZeroProperty =
            DependencyProperty.Register("LeadingZero", typeof(bool?), typeof(DragMonthDay), new PropertyMetadata(false, ValueOnChanged));

        public DateTime? SetDateTime
        {
            get { return (DateTime?)GetValue(SetDateTimeProperty); }
            set { SetValue(SetDateTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SetDateTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SetDateTimeProperty =
            DependencyProperty.Register("SetDateTime", typeof(DateTime?), typeof(DragMonthDay), new PropertyMetadata(default(DateTime?), ValueOnChanged));

        private static void ValueOnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                var group = (DragMonthDay)d;
                if (group != null)
                {
                    group.SetSize();
                    group.LoadImages();
                    group.GroupValueChanged?.Invoke();
                }
            }
        }


        public string? UnitIcon
        {
            get { return (string?)GetValue(UnitIconProperty); }
            set { SetValue(UnitIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UnitIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnitIconProperty =
            DependencyProperty.Register("UnitIcon", typeof(string), typeof(DragMonthDay), new PropertyMetadata(string.Empty, ValueOnChanged));


        public string? MonthIcon
        {
            get { return (string?)GetValue(MonthIconProperty); }
            set { SetValue(MonthIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MonthIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MonthIconProperty =
            DependencyProperty.Register("MonthIcon", typeof(string), typeof(DragMonthDay), new PropertyMetadata(string.Empty, ValueOnChanged));




        public string? DayIcon
        {
            get { return (string?)GetValue(DayIconProperty); }
            set { SetValue(DayIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DayIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DayIconProperty =
            DependencyProperty.Register("DayIcon", typeof(string), typeof(DragMonthDay), new PropertyMetadata(string.Empty, ValueOnChanged));


        public MonthDayModeEnum MonthDayMode
        {
            get { return (MonthDayModeEnum)GetValue(MonthDayModeProperty); }
            set { SetValue(MonthDayModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MonthDayMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MonthDayModeProperty =
            DependencyProperty.Register("MonthDayMode", typeof(MonthDayModeEnum), typeof(DragMonthDay), new PropertyMetadata(default(MonthDayModeEnum), ValueOnChanged));


        public DragMonthDay() : base()
        {

        }

        int SingleWidth;

        void SetSize(string? iconPath)
        {
            if (!string.IsNullOrWhiteSpace(iconPath) && File.Exists(iconPath))
            {
                var bitmap = new Bitmap(iconPath);
                this.Width += bitmap.Width;
                if (bitmap.Height > this.Height)
                {
                    this.Height = bitmap.Height;
                }
            }
        }

        public Dictionary<string, string?> signTable => new Dictionary<string, string?>
        {
            {DragDateTimeHelper.iconStr,UnitIcon  },
            {DragDateTimeHelper.monthStr,MonthIcon  },
            {DragDateTimeHelper.dayStr,DayIcon  },

        };

        public override void SetSize()
        {
            if (ImageSource?.Any() ?? false)
            {
                var str = DragDateTimeHelper.GetMothDay(LeadingZero ?? false, this.MonthDayMode, !string.IsNullOrWhiteSpace(UnitIcon), !string.IsNullOrWhiteSpace(MonthIcon), !string.IsNullOrEmpty(DayIcon), SetDateTime ?? default(DateTime));
                var imgPath = ImageSource.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(imgPath) && File.Exists(imgPath))
                {
                    var bitmap = new Bitmap(imgPath);
                    SingleWidth = bitmap.Width;
                    this.Height = bitmap.Height;
                    this.Width = SingleWidth * str.Replace(DragDateTimeHelper.iconStr, "")
                                                  .Replace(DragDateTimeHelper.monthStr, "")
                                                  .Replace(DragDateTimeHelper.dayStr, "").Length;
                    if (MonthDayMode == MonthDayModeEnum.split)
                    {
                        SetSize(UnitIcon);
                    }
                    else if (MonthDayMode == MonthDayModeEnum.unit)
                    {
                        var list = new string?[] { MonthIcon, DayIcon };
                        foreach (var item in list)
                        {
                            SetSize(item);
                        }
                    }
                }
            }
        }

        public override void LoadImages()
        {
            var canvas = GetTemplateChild("PART_Canvas") as Canvas;
            if (canvas != null && SetDateTime != null)
            {
                canvas.Children.Clear();
                var left = 0.0;
                var top = 0.0;
                var datatimestr = DragDateTimeHelper.GetMothDay(LeadingZero ?? false, this.MonthDayMode, !string.IsNullOrWhiteSpace(UnitIcon), !string.IsNullOrWhiteSpace(MonthIcon), !string.IsNullOrEmpty(DayIcon), SetDateTime ?? default(DateTime));
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
                        var icon = signTable[i.ToString()];
                        if (!string.IsNullOrWhiteSpace(icon) && File.Exists(icon))
                        {
                            var bitmap = new Bitmap(icon);
                            var image = new System.Windows.Controls.Image
                            {
                                Source = BitmapImageHelper.LoadFromUri(new Uri(CommonHelper.AbsolutePath(icon), UriKind.RelativeOrAbsolute)),
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
                DraggableBehavior.SetPostion(this);
                
            }
        }

        public override void OnFolderSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                var group = (DragMonthDay)d;
                if (group != null && !string.IsNullOrWhiteSpace(e.NewValue?.ToString()))
                {
                    group.SetSize();
                    group.LoadImages();
                }
            }
        }


    }

    public enum MonthDayModeEnum
    {
        split,
        unit,
    }

    public class DragBindMonthDay : DragBindBase
    {
        private bool? leadingZero;
        private DateTime? setDateTime;
        private string? unitIcon;
        private string? monthIcon;
        private string? dayIcon;
        private MonthDayModeEnum? monthDayMode;
        private ObservableCollection<string>? imageSource;
        public bool? LeadingZero
        {
            get { return leadingZero; }
            set { SetProperty(ref leadingZero, value); }
        }

        public DateTime? SetDateTime
        {
            get { return setDateTime; }
            set { SetProperty(ref setDateTime, value); }
        }

        public string? UnitIcon
        {
            get { return unitIcon; }
            set { SetProperty(ref unitIcon, value); }
        }

        public string? MonthIcon
        {
            get { return monthIcon; }
            set { SetProperty(ref monthIcon, value); }
        }

        public string? DayIcon
        {
            get { return dayIcon; }
            set { SetProperty(ref dayIcon, value); }
        }

        public MonthDayModeEnum? MonthDayMode
        {
            get { return monthDayMode; }
            set { SetProperty(ref monthDayMode, value); }
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
            yield return UnitIcon;
            yield return MonthIcon;
            yield return DayIcon;
        }

        public override DragDataBaseXml GetOutXml()
        {
            var outXml = new DragDataBaseXml();
            WatchImage unitIcon = null, monthIcon = null, dayIcon = null;
            if (!string.IsNullOrWhiteSpace(UnitIcon))
            {
                unitIcon = new WatchImage
                {
                    Name = OutXmlHelper.GetWatchElementName(),
                    Src = UnitIcon,
                };
            }
            if (!string.IsNullOrWhiteSpace(MonthIcon))
            {
                monthIcon = new WatchImage
                {
                    Name = OutXmlHelper.GetWatchElementName(),
                    Src = MonthIcon,
                };
            }
            if (!string.IsNullOrWhiteSpace(DayIcon))
            {
                dayIcon = new WatchImage
                {
                    Name = OutXmlHelper.GetWatchElementName(),
                    Src = DayIcon,
                };
            }
            if (MonthDayMode == MonthDayModeEnum.split)
            {
                if (unitIcon != null)
                    outXml!.Images!.Add(unitIcon!);
            }
            else if (MonthDayMode == MonthDayModeEnum.unit)
            {
                if (monthIcon != null)
                    outXml!.Images!.Add(monthIcon!);
                if (dayIcon != null)
                    outXml!.Images!.Add(dayIcon!);
            }
            var array = new ImageArray
            {
                Name = OutXmlHelper.GetWatchElementName(),
                Images = ImageSource?.Select(x => new WatchImage
                {
                    Src = x,
                }).ToList(),

            };
            outXml.ImageArrays.Add(array);
            var monthItem = new DataItemImageNumber
            {
                Name = OutXmlHelper.GetWatchElementName(),
                Align = Align.ToString(),
                Source = DataItemTypeHelper.DataItemTypes["月"].ToString(),
                Ref = $"@{array.Name}",
                TotalDigits = DataItemTypeHelper.GetTotalDigits("月"),
                LeadingZero = LeadingZero ?? false,

            };
            outXml!.DataItemImageNumbers!.Add(monthItem);
            var dayItem = new DataItemImageNumber
            {
                Name = OutXmlHelper.GetWatchElementName(),
                Align = Align.ToString(),
                Source = DataItemTypeHelper.DataItemTypes["日"].ToString(),
                Ref = $"@{array.Name}",
                TotalDigits = DataItemTypeHelper.GetTotalDigits("日"),
                LeadingZero = LeadingZero ?? false,
            };
            outXml!.DataItemImageNumbers!.Add(dayItem);
            Widget widget = null;
            if (MonthDayMode == MonthDayModeEnum.split)
            {
                widget = new Widget
                {
                    Name = OutXmlHelper.GetWatchElementName(),
                    FlexDirection = "row",
                    JustifyContent = "flex-start",
                    AlignItems = "center",
                    //Width = (int)this.Width,
                    Width = System.Drawing.Image.FromFile(imageSource[0]).Width*4,
                    Height = (int)this.Height,
                    Items = new List<Item>
                   {
                      new Item{ Ref=$"@{monthItem.Name}", X=(int)this.Left,Y=(int)this.Top },
                      new Item{ Ref=$"{(unitIcon!=null?$"@{unitIcon.Name}":"")}", X=(int)this.Left,Y=(int)this.Top },
                      new Item{ Ref=$"@{dayItem.Name}", X=(int)this.Left,Y=(int)this.Top },
                   },
                };
                if (!string.IsNullOrEmpty(UnitIcon)) widget.Width += System.Drawing.Image.FromFile(UnitIcon).Width;
                outXml.Widgets!.Add(widget);
            }
            else
            {
                widget = new Widget
                {
                    Name = OutXmlHelper.GetWatchElementName(),
                    FlexDirection = "row",
                    JustifyContent = "flex-start",
                    AlignItems = "center",
                    //Width = (int)this.Width,
                    Width = System.Drawing.Image.FromFile(imageSource[0]).Width * 4,
                    Height = (int)this.Height,
                    Items = new List<Item>
                   {
                      new Item{ Ref=$"@{monthItem.Name}", X=(int)this.Left,Y=(int)this.Top },
                      new Item{ Ref=$"{(monthIcon!=null?$"@{monthIcon.Name}":"")}", X=(int)this.Left,Y=(int)this.Top },
                      new Item{ Ref=$"@{dayItem.Name}", X=(int)this.Left,Y=(int)this.Top },
                      new Item{ Ref=$"{(dayIcon!=null?$"@{dayIcon.Name}":"")}", X=(int)this.Left,Y=(int)this.Top },
                   },
                };
                if (!string.IsNullOrEmpty(DayIcon)) widget.Width += System.Drawing.Image.FromFile(DayIcon).Width;
                if (!string.IsNullOrEmpty(MonthIcon)) widget.Width += System.Drawing.Image.FromFile(MonthIcon).Width;
                outXml.Widgets!.Add(widget);
            }
            outXml!.Layout = new Layout
            {
                Ref = $"@{widget.Name}",
                X = GetLeft(),
                Y = (int)this.Top,
            };


            return outXml;
        }
    }

}
