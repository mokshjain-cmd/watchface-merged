using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace WatchControlLibrary.Model
{
    public class WatchFace : Snapshoot
    {
        public WatchFace()
        {
            WatchStyles = new ObservableCollection<WatchStyle>();
        }
        private string? watchName;

        public string? WatchName
        {
            get { return watchName; }
            set { SetProperty(ref watchName, value); }
        }

        private string? watchCode;

        public string? WatchCode
        {
            get { return watchCode; }
            set { SetProperty(ref watchCode, value); }
        }


        private double? cornerX;

        public double? CornerX
        {
            get { return cornerX; }
            set { SetProperty(ref cornerX, value); }
        }

        private double? cornerY;

        public double? CornerY
        {
            get { return cornerY; }
            set { SetProperty(ref cornerY, value); }
        }

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
            set {
                    SetProperty(ref height, value); 
                }
        }

        //private void CalculateDivision(int oldValue, int value)
        //{
        //        if (double.TryParse(oldValue, out double oldNumber) &&
        //            double.TryParse(newValue, out double newNumber) &&
        //            oldNumber != 0)
        //        {
        //            DivisionResult = newNumber / oldNumber;
        //        }
        //        else
        //        {
        //            DivisionResult = null; // 设置为 null 以表示无效计算
        //        }
        //}

        private int thumbnailWidth;

        public int ThumbnailWidth
        {
            get { return thumbnailWidth; }
            set { SetProperty(ref thumbnailWidth, value); }
        }

        private int thumbnailHeight;

        public int ThumbnailHeight
        {
            get { return thumbnailHeight; }
            set { SetProperty(ref thumbnailHeight, value); }
        }

        private int corner;

        public int Corner
        {
            get { return corner; }
            set { SetProperty(ref corner, value); }
        }

        private string? deviceType;

        public string? DeviceType
        {
            get { return deviceType; }
            set { SetProperty(ref deviceType, value); }
        }

        private DateTime? createTime;

        public DateTime? CreateTime
        {
            get { return createTime; }
            set { SetProperty(ref createTime, value); }
        }

        private bool? isAlbum = false;
        public bool? IsAlbum
        {
            get { return isAlbum; }
            set { SetProperty(ref isAlbum, value); }
        }

        private int? colorBit;
            
        public int? ColorBit
        {
            get { return colorBit; }
            set { SetProperty(ref colorBit, value); }
        }


        public AlbumBackground? AlbumBackground { get; set; }
        //private string? designSketch;

        //public string? DesignSketch
        //{
        //    get { return designSketch; }
        //    set { SetProperty(ref designSketch, value); }
        //}

        //private string? thumbnail;

        //public string? Thumbnail
        //{
        //    get { return thumbnail; }
        //    set { SetProperty(ref thumbnail, value); }
        //}

        //private string? descriptionFile;

        //public string? DescriptionFile
        //{
        //    get { return descriptionFile; }
        //    set { SetProperty(ref descriptionFile, value); }
        //}



        [JsonIgnore]
        public string FolderName => $"{WatchCode}_{WatchName}_{DeviceType}";

        //[XmlIgnore]
        //public string XmlName => $"{Width}_{Height}";

        private ObservableCollection<WatchStyle>? watchStyles;

        public ObservableCollection<WatchStyle>? WatchStyles
        {
            get { return watchStyles; }
            set { SetProperty(ref watchStyles, value); }
        }




        public WatchfaceOut GetWatchfaceOut()
        {
            var faceOut = new WatchfaceOut
            {
                Name = this.WatchName,
                Width = this.Width,
                Height = this.Height,
                DeviceType = this.DeviceType,
                Id = Guid.NewGuid().ToString("N"),
            };
            var aod = this.WatchStyles!.FirstOrDefault(x => x.ScreenType == WatchScreen.dark);
            if (aod != null) 
            {
                this.WatchStyles!.Remove(aod);
                this.watchStyles!.Insert(1, aod);
            }

            if (this.WatchStyles != null && this.WatchStyles.Any())
            {
                foreach (var style in WatchStyles)
                {
                    var xmls = style.DragBindBases.Select(x => x.GetOutXml()).ToList();

                    var theme = new Theme
                    {
                        Name = OutXmlHelper.GetWatchElementName(),
                        Layout = xmls.Select(x => x.Layout).ToList(),
                        Type = style.ScreenType == WatchScreen.light ? "normal" : "AOD",
                        IsPhotoAlbumWatchface = style.ScreenType == WatchScreen.light && (this.IsAlbum ?? false),
                        StyleName = style.StyleName,
                    };
                    faceOut.Themes!.Add(theme);

                    Resources Resources = new Resources();
                    faceOut.ElementCache.Add(style.StyleName!, Resources);

                    foreach (var image in xmls.SelectMany(x => x.Images!))
                    {
                        if (string.IsNullOrWhiteSpace(image.Src)) 
                        {
                            throw new Exception("Image path cannot be empty");
                        }
                        image.Src = CommonHelper.OutPath(image.Src, this.FolderName);
                    }
                    var preview = new WatchImage
                    {
                        Name = OutXmlHelper.GetWatchElementName(),
                        Src = $@"_preview\{style.StyleName}.png",
                        IsPreview = true
                    };

                    theme.Preview = theme.Type != "AOD" ? $"@{preview.Name}" : null;
                    Resources.Images!.Add(preview);//预览图


                    if ((this.IsAlbum ?? false) && theme.Type != "AOD")
                    {
                        var background = new WatchImage
                        {
                            Name = OutXmlHelper.GetWatchElementName(),
                            Src = CommonHelper.OutPath(this.AlbumBackground!.BackgroundSource!, this.FolderName)
                        };
                        theme.Bg = $"@{background.Name}";
                        Resources.Images!.Add(background);//相册背景图

                    }
                    foreach (var image in xmls.SelectMany(x => x.ImageArrays!).SelectMany(x => x.Images))
                    {
                        if (string.IsNullOrWhiteSpace(image.Src))
                        {
                            throw new Exception("Image path cannot be empty");
                        }
                        image.Src = CommonHelper.OutPath(image.Src, this.FolderName);
                    }
                    Resources!.Translations.AddRange(xmls.SelectMany(x => x.Translations));
                    Resources!.Images!.AddRange(xmls.SelectMany(x => x.Images!));
                    Resources!.ImageArrays!.AddRange(xmls.SelectMany(x => x.ImageArrays!));
                    Resources!.DataItemImageValues!.AddRange(xmls.SelectMany(x => x.DataItemImageValues!));
                    Resources!.DataItemImageNumbers!.AddRange(xmls.SelectMany(x => x.DataItemImageNumbers!));
                    Resources!.DataItemImagePoints!.AddRange(xmls.SelectMany(x => x.DataItemPointers!));
                    Resources!.Widgets!.AddRange(xmls.SelectMany(x => x.Widgets!));
                    Resources!.Slots!.AddRange(xmls.SelectMany(x => x.Slots!));
                    Resources!.Sprites!.AddRange(xmls.SelectMany(x => x.Sprites!));
                    faceOut.Resources!.Translations.AddRange(Resources.Translations);
                    faceOut.Resources!.Images!.AddRange(Resources.Images);
                    faceOut.Resources!.ImageArrays!.AddRange(Resources.ImageArrays);
                    faceOut.Resources!.DataItemImageValues!.AddRange(Resources.DataItemImageValues);
                    faceOut.Resources!.DataItemImageNumbers!.AddRange(Resources.DataItemImageNumbers);
                    faceOut.Resources!.DataItemImagePoints!.AddRange(Resources.DataItemImagePoints);
                    faceOut.Resources!.Widgets!.AddRange(Resources.Widgets);
                    faceOut.Resources!.Slots!.AddRange(Resources.Slots);
                    faceOut.Resources!.Sprites!.AddRange(Resources.Sprites);

                }
            }

            //var aod = faceOut.Themes!.FirstOrDefault(x => x.Type == "AOD");
            //if (aod != null)
            //{
            //    faceOut.Themes!.Remove(aod);
            //    faceOut.Themes.Insert(1, aod);
            //}

            return faceOut;
        }
    }

    public class WatchStyle : Snapshoot
    {
        public WatchStyle()
        {
            this.Zh = new Watch()
            {
                Item = new MonitorItem(),
            };
            this.DragBindBases = new ObservableCollection<DragBindBase>();
            Zh.Item!.NumChanged += Item_NumChanged;
            Zh.Item!.TimeChanged += Item_TimeChanged;
        }

        public static int GetTimeAngle(int idx, DateTime dateTime)
        {
            switch (idx)
            {
                case 0:
                    return dateTime.Hour * 30 + dateTime.Minute / 2;

                case 1:
                    return dateTime.Minute * 6;

                case 2:
                    return dateTime.Second * 6;

                default:
                    return 0;
            }
        }

        private void Item_TimeChanged(DateTime time)
        {
            foreach (var item in TemplateBinds)
            {
                if (item is DragBindPoint point)
                {
                    point.Value = GetTimeAngle(point.ValueIndex, time);

                }
            }
        }

        private string? styleName;

        public string? StyleName
        {
            get { return styleName; }
            set { SetProperty(ref styleName, value); }
        }

        private ObservableCollection<DragBindBase>? dragBindBases;

        public ObservableCollection<DragBindBase>? DragBindBases
        {
            get { return dragBindBases; }
            set { SetProperty(ref dragBindBases, value); }
        }

        private ObservableCollection<DragBindBase> templateBinds;
        public ObservableCollection<DragBindBase> TemplateBinds
        {
            get => templateBinds;
            set => SetProperty(ref templateBinds, value);
        }


        private WatchScreen screenType;

        public WatchScreen ScreenType
        {
            get { return screenType; }
            set { SetProperty(ref screenType, value); }
        }

        private Watch zh;

        public Watch Zh
        {
            get { return zh; }
            set { SetProperty(ref zh, value); }
        }
        private void Item_NumChanged(string numName, int value)
        {
            string name = numName switch
            {
                "StepNum" => "Steps",
                //"HeartRateNum" => "Heart Rate",
                "CalorieNum" => "Calories",
                //"KwhNum" => "Battery",
                "StrengthNum" => "Exercise",
                _ => "error"
            };
            foreach (var item in TemplateBinds)
            {
                if (item.ItemName != null && item.ItemName.Contains(name))
                {
                    if (item is DragBindNums nums)
                        nums.CurrentNum = value;
                    if (item is DragBindProgress progress)
                    {
                        progress.CurrentNum = value;

                    }
                }
            }
        }
    }

    public enum WatchScreen
    {
        light,
        dark
    }
}
