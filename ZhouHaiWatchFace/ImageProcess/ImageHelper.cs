using Emgu.CV;
using Emgu.CV.CvEnum;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using WatchControlLibrary;
using WatchControlLibrary.Model;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace ImageProcess
{
    public class ImageHelper
    {
        public static Bitmap Resize(string src, int width, int height)
        {
            Mat srcImage = new Mat(src, ImreadModes.Unchanged);
            Mat destination = new Mat();
            CvInvoke.Resize(srcImage, destination, new Size(width, height), interpolation: Emgu.CV.CvEnum.Inter.Linear);
            srcImage.Dispose();
            var bitmap = destination.ToBitmap();
            destination.Dispose();
            return bitmap;
        }

        public static Tuple<int, int> GetCurrentValues(Size origin, Size destination, Point point)
        {
            var scaleX = (float)origin.Width / destination.Width;
            var scaleY = (float)origin.Height / destination.Height;
            return Tuple.Create((int)(point.X / scaleX), (int)(point.Y / scaleY));
        }

        public static string ResourcesPath(string savePath, string watchFolder) => $@"{savePath}\{watchFolder}\resources\";

        public static string OutputPath(string savePath, string watchFolder) => $@"{savePath}\{watchFolder}\output\";
        public static string ImageSrc(string savePath, string watchFolder, string src) => $"{ResourcesPath(savePath, watchFolder)}{src}";

        public static string JosnPath(string savePath, string watchFolder) => $"{ResourcesPath(savePath, watchFolder)}watchFace.json";

        public static string DesJsonPath(string savePath, string watchFolder) => $"{ResourcesPath(savePath, watchFolder)}description.json";

        public static string DesXmlPath(string savePath, string watchFolder) => $"{OutputPath(savePath, watchFolder)}description.xml";

        public static void ChangeImage(WatchFace watch, WatchImage image, string savePath, int width, int height)
        {
            var src = $@"{CommonHelper.CurrentPath(watch.FolderName)}\{image.Src}";
            var bitmap = new Bitmap(src);
            var values = GetCurrentValues(new Size(watch.Width, watch.Height), new Size(width, height), new Point(bitmap.Width, bitmap.Height));
            var newBitmap = Resize(src, values.Item1, values.Item2);
            var dest = ImageSrc(savePath, watch.FolderName, image.Src);
            var folder = Path.GetDirectoryName(dest);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            newBitmap.Save($"{dest}");
        }

        public static void SetStyleValue(DragBindBase drag, Size origin, Size destination)
        {
            var size = GetCurrentValues(origin, destination, new Point((int)drag.Width, (int)drag.Height));
            drag.Width = size.Item1;
            drag.Height = size.Item2;
            var location = GetCurrentValues(origin, destination, new Point((int)drag.Left, (int)drag.Top));
            drag.Left = location.Item1;
            drag.Top = location.Item2;
            if (drag is DragBindDouble bindDouble)
            {
                var values = GetCurrentValues(origin, destination, new Point((int)bindDouble.DecimalOffsetX, 0));
                bindDouble.DecimalOffsetX = values.Item1;
            }
            else if (drag is DragBindSlot bindSlot)
            {
                if (bindSlot?.Positions?.Any() ?? false)
                {
                    foreach (var position in bindSlot.Positions)
                    {
                        var values = GetCurrentValues(origin, destination, new Point((int)position.X, (int)position.Y));
                        position.X = values.Item1;
                        position.Y = values.Item2;
                    }
                }
            }
            else if(drag is DragBindPoint point) 
            {
                var values = GetCurrentValues(origin, destination, new Point((int)point.OriginPointX, (int)point.OriginPointY));
                point.OriginPointX = values.Item1;
                point.OriginPointY = values.Item2;
            }
            if (drag.SubItems?.Count != 0)
            {
                foreach (var sub in drag.SubItems)
                {
                    SetStyleValue(sub, origin, destination);
                }
            }
        }

        public static void ResizeWatch(WatchFace watch, string savePath, int width, int height, int thumbnailWidth, int thumbnailHeight, int corner)
        {
            var outXml = watch.GetWatchfaceOut();
            foreach (var image in outXml.Resources.Images)
            {
                ChangeImage(watch, image, savePath, width, height);
            }
            foreach (var image in outXml.Resources.ImageArrays.SelectMany(x => x.Images))
            {
                ChangeImage(watch, image, savePath, width, height);
            }

            foreach (var style in watch.WatchStyles)
            {
                if (style.DragBindBases?.Count != 0)
                {
                    foreach (var drag in style.DragBindBases)
                    {
                        SetStyleValue(drag, new Size(watch.Width, watch.Height), new Size(width, height));
                    }
                }
            }
            watch.Width = width;
            watch.Height = height;
            watch.Corner = corner;
            watch.ThumbnailWidth = thumbnailWidth;
            watch.ThumbnailHeight = thumbnailHeight;
            var watchStr = JsonConvert.SerializeObject(watch, CommonHelper.Settings);
            File.WriteAllText(JosnPath(savePath, watch.FolderName), watchStr);
            var descjson = CommonHelper.OutPutDescriptJson(watch.FolderName);
            if (File.Exists(descjson)) 
            {
                var info = JsonConvert.DeserializeObject<WatchInfo>(File.ReadAllText(descjson), CommonHelper.Settings);
                info.size = $"{watch.Width}x{watch.Height}";
                var jonstr= JsonConvert.SerializeObject(info, CommonHelper.Settings);
                File.WriteAllText(DesJsonPath(savePath, watch.FolderName), jonstr);
                
            }
            var outputdescxml = CommonHelper.OutPutDescriptFile(watch.FolderName);
            if (File.Exists(outputdescxml))
            {
                if (!Directory.Exists(OutputPath(savePath, watch.FolderName)))
                {
                    Directory.CreateDirectory(OutputPath(savePath, watch.FolderName));
                }
             
                File.Copy(outputdescxml, DesXmlPath(savePath, watch.FolderName), true);


            }



        }

 
    }
}
