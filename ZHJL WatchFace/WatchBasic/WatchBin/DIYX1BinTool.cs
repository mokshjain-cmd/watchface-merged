using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchBasic.Tool;
using WatchBasic.UIBasic;
using WatchBasic.WatchBin.Model;
using WatchBasic.ZHPicRSA;
using WatchBasic.ZHPicRSA.Model;

namespace WatchBasic.WatchBin
{
    public class DIYX1BinTool
    {

        public static List<LayerGroup>? GetLayerGroups(IEnumerable<LayerGroup>? layerGroups, string groupCode)
        {
            return layerGroups?.Where(i => i.GroupCode == groupCode).Select(i => new LayerGroup
            {
                GroupCode = i.GroupCode,
                GroupName = i.GroupName,
                IsEnable = i.IsEnable,
                IsShow = i.IsShow,
                ShowIndex = i.ShowIndex,
                LanguageCount = i.LanguageCount,
                LanguagePage = i.LanguagePage,
                Layers = i.Layers?.Select(l => new Layer
                {
                    Height = l.Height,
                    Width = l.Width,
                    Id = l.Id,
                    NotBitmapSource = l.NotBitmapSource,
                    ImgPath = l.ImgPath,
                    LayerIndex = l.LayerIndex,
                    Left = l.Left,
                    Top = l.Top,
                    NotDye = l.NotDye,

                }).ToList()
            }).ToList();
        }

        public static string? GetImageType(IEnumerable<Layer>? layers)
        {
            var imgPath = layers?.FirstOrDefault()?.ImgPath ?? string.Empty;
            return Path.GetExtension(imgPath).ToUpper();
        }
        public static string? GetImageType(Layer? layer)
        {
            var imgPath = layer?.ImgPath;
            return Path.GetExtension(imgPath).ToUpper();
        }

        public static void SetBmp(DIYBase dataFormat, string groupCode, List<Layer>? layers, BinImage BinImage,bool isThumbnail=false)
        {
            if (layers == null || layers!.Count == 0)
            {
                dataFormat!.Addr = 0;
            }
            dataFormat.Num = layers.Count;
            foreach (var i in layers)
            {
                dataFormat.Addrs.Add(BinImage.SetImageAddr());
                Bitmap bitmap = new Bitmap(i?.ImgPath!);
                if (isThumbnail) 
                {
                    bitmap=DIYX1BinTool.GetThumnail(i?.ImgPath!,CommonDefintion.Setting.ProjectName);
                }
                BmpInfo bmpInfo = new BmpInfo();
                bmpInfo.BmpRows = BmpRSA.GetBmpRows(bitmap);
                bmpInfo.Finished();
                //是否用压缩
                var compressSize = bmpInfo.Heads.Count + bmpInfo.Images.Count;
                var nocompressSize = 0;
                if (CommonDefintion.IsColor888)
                {
                    nocompressSize = layers.FirstOrDefault()!.Width * layers.FirstOrDefault()!.Height * 3;
                }
                else
                {
                    nocompressSize = layers.FirstOrDefault()!.Width * layers.FirstOrDefault()!.Height * 2;
                }
                if (compressSize >= nocompressSize)
                {
                    dataFormat.HeadSizes.Add(nocompressSize);
                    var images = BmpRSA.GetImageToBytes(bitmap);
                    BinImage.Bytes.AddRange(images);
                    BinImage.Count = images.Count();
                }
                else //用压缩数据
                {
                    BinImage.Bytes.AddRange(bmpInfo.Heads.ToArray());
                    BinImage.Bytes.AddRange(bmpInfo.Images.ToArray());
                    BinImage.Count = compressSize;
                    dataFormat.HeadSizes.Add(compressSize);
                }
            }
        }
        public static void SetPng(DIYBase dataFormat, string groupCode, List<Layer>? layers, BinImage BinImage, bool isPoint,bool isThumbnail=false)
        {
            if (layers == null || layers.Count == 0)
            {
                dataFormat!.Addr = 0;
            }
            dataFormat!.Addrs.Add(BinImage.SetImageAddr());
            List<PointInfo> points = new List<PointInfo>();
            Queue<int> indexes = new Queue<int>();//记录Index
            var isArgb = dataFormat.PicCompress == PicCompress.png1;

            if (isPoint == true && layers?.Count == 1)
            {
                var imagePath = layers?.FirstOrDefault()?.ImgPath;
                var bmp = new Bitmap(imagePath ?? "");
                if (isThumbnail) 
                {
                    bmp = GetThumnail(imagePath, CommonDefintion.Setting.ProjectName);
                }
                var firstImage = PointInfo.GetPointInfo(bmp, true);
                points.Add(firstImage);
                var img = CVTool.ImgInit(imagePath);
                for (var i = 1; i <= 15; i++) //序列化图片
                {
                    var bitmap = ImageUtils.RotateImgCV(img, -6 * i);
                    var info2 = PointInfo.GetPointInfo(bitmap, true);
                    points.Add(info2);
                    bitmap.Dispose();
                }
            }
            else
            {
                if (layers != null)
                    foreach (var layer in layers)
                    {
                        var imagePath = layer.ImgPath;
                        var bmp = new Bitmap(imagePath ?? "");
                        var info = PointInfo.GetPointInfo(bmp, isArgb);
                        points.Add(info);
                        bmp.Dispose();
                    }
            }
            //List<int> countRecord = new List<int>();
            var maxSize = points.Select(i => i.Size).Max();
            dataFormat!.HeadSizes.Add(maxSize);
            foreach (var info2 in points)
            {
                indexes.Enqueue((BinImage.Addr ?? 0) - BinImage.OffsetAddr + 2);//2为高度字节
                var count = info2?.Heads?.Count + info2?.RowData?.Count;
                while (count < maxSize)
                {
                    info2?.RowData?.Add(0);
                    count++;
                }
                BinImage?.Bytes?.AddRange(info2?.Heads!);
                BinImage?.Bytes?.AddRange(info2?.RowData!);
                BinImage.Count += maxSize;
                BinImage.Addr += BinImage.Count;
                BinImage.Count = 0;
            }
            foreach (var p in points)
            {
                var _imageAddr = BinImage.SetImageAddr();
                var imageAddr = _imageAddr.GetBytes32();
                var addr = indexes.Dequeue();
                BinImage.Bytes![addr] = imageAddr[0];
                BinImage.Bytes[addr + 1] = imageAddr[1];
                BinImage.Bytes[addr + 2] = imageAddr[2];
                BinImage.Bytes[addr + 3] = imageAddr[3];
                BinImage.Bytes.AddRange(p.Image!);
                BinImage.Count = p.Image?.Count;
            }
        }

        public static Bitmap PictureOverlay(string basePath, string overPath)
        {
            using (Image baseImage = Image.FromFile(basePath))
            {
                // 加载要叠加的图像
                using (Image overImage = Image.FromFile(overPath))
                {
                    // 创建一个新的位图，大小与底图相同
                    Bitmap resultImage = new Bitmap(baseImage.Width, baseImage.Height);

                    using (Graphics graphics = Graphics.FromImage(resultImage))
                    {

                        // 将底图绘制到结果图像上
                        graphics.DrawImage(baseImage, 0, 0, baseImage.Width, baseImage.Height);

                        // 将叠加图像绘制到结果图像上
                        graphics.DrawImage(overImage, 0, 0, overImage.Width, overImage.Height);
                    }
                    return resultImage;
                }
            }
        }

        public static Bitmap PictureOverlay(Bitmap baseImage, string overPath) 
        {
            using (Image overImage = Image.FromFile(overPath))
            {
                // 创建一个新的位图，大小与底图相同
                Bitmap resultImage = new Bitmap(baseImage.Width, baseImage.Height);

                using (Graphics graphics = Graphics.FromImage(resultImage))
                {

                    // 将底图绘制到结果图像上
                    graphics.DrawImage(baseImage, 0, 0, baseImage.Width, baseImage.Height);

                    // 将叠加图像绘制到结果图像上
                    graphics.DrawImage(overImage, 0, 0, overImage.Width, overImage.Height);
                }
                
                return resultImage;
            }
        }

        public static Bitmap PictureClearBorder(string basePath, string overPath)
        {
            var locations = GetLocations(new Bitmap(overPath));
            var resultImage=PictureOverlay(basePath, overPath);
            SetLocation(resultImage, locations.ToList());
            return resultImage;
        }


        public static IList<Location> GetLocations(Bitmap map)
        {
            List<Location> locations = new List<Location>();
            bool islastRow = false;
            for (var i = 0; i < map.Height; i++)
            {
                for (var j = 0; j < map.Width; j++)
                {
                    var color = map.GetPixel(j, i);
                    if (color.A > 0)
                    {
                        locations.Add(new Location { x = j, y = i });
                    }
                    else
                    {
                        if (j == 0)
                            islastRow = true;

                        break;
                    }
                }
                if (islastRow) break;
            }
            return locations;
        }

        public static void SetLocation(Bitmap map, List<Location> locations, Color? color = null)
        {
            if (color == null)
            {
                color = Color.Transparent;
            }

            locations.ForEach((l) =>
            {
                map.SetPixel(l.x, l.y, (Color)color);

            });

            locations.ForEach((l) =>
            {
                map.SetPixel(l.x, l.y + (map.Height - (l.y) * 2 - 1), (Color)color);

            });

            locations.ForEach((l) =>
            {
                map.SetPixel(l.x + (map.Width - (l.x) * 2 - 1), l.y, (Color)color);

            });

            locations.ForEach((l) =>
            {
                map.SetPixel(l.x + (map.Width - (l.x) * 2 - 1), l.y + (map.Height - (l.y) * 2 - 1), (Color)color);

            });
        }

        public static Bitmap PictureClearBorder(Bitmap baseImage, string overPath)
        {
            var locations = GetLocations(new Bitmap(overPath));
            var resultImage = PictureOverlay(baseImage, overPath);
            SetLocation(resultImage, locations.ToList());
            return resultImage;
        }

        public static Bitmap GetThumnail(string thummailPath,string projectName) 
        {
            var overlayPath = Directory.GetFiles($"{AppDomain.CurrentDomain.BaseDirectory}\\Border").Where(i => i.Contains(projectName.Replace("*","X"))).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(overlayPath)) 
            {
                return new Bitmap(thummailPath);
            }
            return PictureOverlay(thummailPath, overlayPath);
        }

        public static Bitmap PictureRemoveCorner(Bitmap resultImage)
        {
            var locations = GetLocations(resultImage);
            SetLocation(resultImage, locations.ToList());
            //for (int y = 0; y < resultImage.Height; y++)
            //{
            //    for (int x = 0; x < resultImage.Width; x++)
            //    {
            //        Color pixelColor = resultImage.GetPixel(x, y);

            //        // 检查颜色是否为黑色
            //        if (pixelColor.R == 0 && pixelColor.G == 0 && pixelColor.B == 0)
            //        {
            //            // 设置透明色
            //            resultImage.SetPixel(x, y, Color.Transparent);
            //        }
            //    }
            //}

            return resultImage;
        }
    }

    public class Location
    {
        public int x;
        public int y;
    }
}
