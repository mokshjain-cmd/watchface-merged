using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Xml.Serialization;
using WatchControlLibrary.Model;
using System.Windows;
using MessageBox = System.Windows.MessageBox;
using Size = System.Windows.Size;
using System.Windows.Forms;
using Brushes = System.Windows.Media.Brushes;
using Point = System.Windows.Point;
using System.Xml;
using System.Configuration;

namespace WatchControlLibrary
{
    public class CommonHelper
    {
        public static string AppPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ZHWatch");
        public static string AddBtnBackgroundPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "AddBtnBackground.png");

        public static string CurrentPath(string watchName) => Path.Combine(RootPath(watchName), "resources");

        public static string RootPath(string watchName) => Path.Combine(AppPath, watchName);

        public static string GetJsonPath(string watchName) => Path.Combine(CurrentPath(watchName), "watchFace.json");

        public static string GetXmlPath(string watchName) => Path.Combine(CurrentPath(watchName), "watchFace.xml");

        public static string Preview(string watchName) => Path.Combine(CurrentPath(watchName), "_preview");

        public static string Widget(string watchName) => Path.Combine(CurrentPath(watchName), "_widget");

        public static string Ezip(string watchName) => Path.Combine(CurrentPath(watchName), "ezip");

        public static string AbsolutePath(string relative) => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relative.TrimStart('.', '\\', '/'));

        public static string OutPutPath(string watchName) => Path.Combine(RootPath(watchName), "output");

        public static string OutPutDescriptFile(string watchName) => Path.Combine(OutPutPath(watchName), "description.xml");
        public static string OutPutDescriptJson(string watchName) => Path.Combine(CurrentPath(watchName), "description.json");

        public static string OutPutPathPreview(string watchName) => Path.Combine(RootPath(watchName), "output", "preview");

        public static string OutPutPathPreviewCoverImage(string watchName) => Path.Combine(OutPutPathPreview(watchName), "cover.jpg");
        public static string OutPutPathPreviewIconImage(string watchName) => Path.Combine(OutPutPathPreview(watchName), "icon_small.jpg");

        public static string WatchBin(string watchName) => Path.Combine(RootPath(watchName), "WatchBin");

        public static string EzipBin(string watchName) => Path.Combine(RootPath(watchName), "EzipBin");



        public static string Resource => "resource.bin";

        public static readonly string? Version = ConfigurationManager.AppSettings["Version"];

        public static string[] Languages => new string[]
        {
            "zh_CN",
            "en_US",
            "zh_TW",
            "ja_JP",
            "es_ES",
            "fr_FR",
            "de_DE",
            "ru_RU",
            "pt_BR",
            "pt_PT",
            "it_IT",
            "ko_KR",
            "tr_TR",
            "nl_NL",
            "th_TH",
            "sv_SE",
            "da_DK",
            "vi_VN",
            "nb_NO",
            "pl_PL",
            "fi_FI",
            "in_ID",
            "el_GR",
            "ro_RO",
            "cs_CZ",
            "uk_UA",
            "sk_SK",
            "hu_HU",
            "ar_EG",
            "iw_IL",
            "zh_HK"
        };

        public static string OutPath(string path, string watchName) => path.TrimStart('.')
            .Replace(@"\\", @"\")
            .Replace(@$"\ZHWatch\{watchName}\resources\", "")
            .Replace(@"\", "/");

        public static JsonSerializerSettings Settings => new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
        };

      

        public static void GenerateDesc(string? folderName)
        {
            var descriptionInfo = CommonHelper.OutPutDescriptJson(folderName);
            if (File.Exists(descriptionInfo))
            {
                var info = JsonConvert.DeserializeObject<WatchInfo>(File.ReadAllText(descriptionInfo), CommonHelper.Settings);
                var watch = new WatchInfoExtra();
                watch.zh.title = info.HnTheme.extra_zh_title;
                watch.zh.briefInfo = info.HnTheme.extra_zh_briefInfo;
                watch.en.title = info.HnTheme.extra_en_title;
                watch.en.briefInfo = info.HnTheme.extra_en_briefInfo;

                var js = JsonConvert.SerializeObject(watch);
                info.HnTheme.version = "qxzhzn.2.0.0";
                info.HnTheme.screen = "QXHD03";
                info.HnTheme.extra = js;
                var fs = CommonHelper.OutPutDescriptFile(folderName);
                XmlSerializer fsserializer = new XmlSerializer(typeof(HnTheme));
                var xmlSettings = new XmlWriterSettings
                {
                    Indent = true,
                    OmitXmlDeclaration = true // 去掉 XML 声明
                };

                using (StreamWriter writer = new StreamWriter(fs))
                using (XmlWriter xmlWriter = XmlWriter.Create(writer, xmlSettings))
                {
                    var namespaces = new XmlSerializerNamespaces();
                    namespaces.Add(string.Empty, string.Empty); // 添加一个空的命名空间
                    fsserializer.Serialize(xmlWriter, info.HnTheme, namespaces);
                }
            }
        }

        public static void GenerateXml(string? folderName)
        {
            var watchFach = JsonConvert.DeserializeObject<WatchFace>(File.ReadAllText(GetJsonPath(folderName)), Settings);
            var outXml = watchFach.GetWatchfaceOut();
            //outXml.WatchInfo = info;
            XmlSerializer serializer = new XmlSerializer(typeof(WatchfaceOut));
            var xmlPath = @$"{CommonHelper.CurrentPath(folderName)}\manifest.xml";
            using (StreamWriter writer = new StreamWriter(xmlPath))
            {
                serializer.Serialize(writer, outXml);
            }
        }

        public static void GenerateImage(string? folderName)
        {

            var watchFace = JsonConvert.DeserializeObject<WatchFace>(File.ReadAllText(GetJsonPath(folderName)), Settings);
            //Directory.Delete(CommonHelper.Preview(watchFace.FolderName), true);
            //Directory.CreateDirectory(CommonHelper.Preview(watchFace.FolderName));
            foreach (var style in watchFace.WatchStyles)
            {
                SetDefaultNum(style.DragBindBases);
                MyCanvas myCanvas = new MyCanvas();
                myCanvas.Width = watchFace.Width;
                myCanvas.Height = watchFace.Height;
                if (watchFace.IsAlbum ?? false)
                {
                    // style.DragBindBases.Insert(0,watchFace.AlbumBackground);
                    myCanvas.Background = watchFace.AlbumBackground!.Background;
                }
                else
                {
                    myCanvas.Background = Brushes.Black;
                }

                if (myCanvas.Width == myCanvas.Height)
                {
                    myCanvas.Clip = new EllipseGeometry
                    {
                        Center = new Point(myCanvas.Width / 2, myCanvas.Height / 2),
                        RadiusX = myCanvas.Width / 2,
                        RadiusY = myCanvas.Height / 2,
                    };
                }
                else
                {
                    myCanvas.Clip = new RectangleGeometry
                    {
                        Rect = new Rect(0, 0, myCanvas.Width, myCanvas.Height),
                        RadiusX = watchFace.Corner,
                        RadiusY = watchFace.Corner,
                    };
                }


                myCanvas.ItemsSource = style.DragBindBases;


                myCanvas.UpdateLayout();

                ExportToPng($@"{Preview(watchFace.FolderName)}\", style.StyleName, myCanvas, watchFace.ThumbnailWidth, watchFace.ThumbnailHeight);



            }
        }




        public static void SetDefaultNum(IEnumerable<DragBindBase> bindBases)
        {
           
            foreach (var bindBase in bindBases)
            {
                var setting = DataHelper.GetWatchSettingByItemName(bindBase.ItemName);

                if (bindBase is DragBindNums bindNums)
                {
                    bindNums.CurrentNum = (int)(setting?.Default ?? 0);
                }
                else if (bindBase is DragBindProgress bindProgress)
                {
                    bindProgress.TargetValue = (int)(setting?.TargetValue ?? 0);

                    bindProgress.CurrentNum = (int)(setting?.Default ?? 0);
                }
                else if (bindBase is DragBindDouble bindDouble)
                {
                    bindDouble.CurrentNum = setting?.Default ?? 0;
                }
                else if (bindBase is DragBindKeyValue bindKeyValue)
                {
                    bindKeyValue.CurrentNum = (int)(setting?.Default ?? 0);
                }
                else if (bindBase is DragBindNormalDateTime dragBindNormalDateTime)
                {
                    dragBindNormalDateTime.SetDateTime = MonitorItem.DefaultTime;

                }
                else if (bindBase is DragBindMonthDay dragMonthDay)
                {
                    dragMonthDay.SetDateTime = MonitorItem.DefaultTime;

                }
                else if (bindBase is DragBindWeek dragBindWeek)
                {
                    dragBindWeek.SetDateTime = MonitorItem.DefaultTime;

                }
                else if (bindBase is DragBindAMPM dragBindAMPM)
                {
                    dragBindAMPM.SetDateTime = MonitorItem.DefaultTime;

                }
                else if (bindBase is DragBindPoint dragPoint)
                {
                    
                    dragPoint.Value = WatchStyle.GetTimeAngle(dragPoint.ValueIndex, MonitorItem.DefaultTime);
                }
                else if (bindBase is DragBindSwitch dragSwitch)
                {
                    dragSwitch.IsOpen = dragSwitch.ItemName == "温度单位" ? true : false;
                }
                else if (bindBase is DragBindSingleDigit singleDigit)
                {
                    singleDigit.SetDateTime = MonitorItem.DefaultTime;
                }
                else if (bindBase is DragBindWidget widget)
                {
                    SetDefaultNum(widget.SubItems);
                }
                else if(bindBase is DragBindSlot slot) 
                {
                    SetDefaultNum(slot.SubItems);
                }
            }

        }

        public static void ExportToPng(string path, string pngName, Canvas surface, int thumbnailWidth = 0, int thumbnailHetght = 0)
        {

            Transform transform = surface.LayoutTransform;
            surface.LayoutTransform = null;


            RenderTargetBitmap renderBitmap = GetCanvasBitmap(surface, (int)surface.Width, (int)surface.Height);  // 效果图
            using (FileStream outStream = new FileStream(path + pngName + ".png", FileMode.OpenOrCreate))
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                encoder.Save(outStream);
            }

            if (thumbnailWidth == 0 || thumbnailHetght == 0)
                return;
            surface.LayoutTransform = new ScaleTransform
            {
                CenterX = 0.5,
                CenterY = 0.5,
                ScaleX = thumbnailWidth / surface.Width,
                ScaleY = thumbnailHetght / surface.Height,
            };
            renderBitmap = GetCanvasBitmap(surface, thumbnailWidth, thumbnailHetght);  //缩略图
            using (FileStream outStream = new FileStream(path + $"thumbnail_{pngName}" + ".png", FileMode.OpenOrCreate))
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                encoder.Save(outStream);
            }

            surface.LayoutTransform = transform;
        }


        static RenderTargetBitmap GetCanvasBitmap(Canvas canvas, int w, int h)
        {
            Size size = new Size(w, h);
            canvas.Measure(size);
            canvas.Arrange(new Rect(size));

            RenderTargetBitmap renderBitmap =
            new RenderTargetBitmap(
            (int)size.Width,
            (int)size.Height,
            96d,
            96d,
            PixelFormats.Pbgra32);
            renderBitmap.Render(canvas);
            return renderBitmap;
        }
    }
}