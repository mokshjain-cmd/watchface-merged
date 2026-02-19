using ImTools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchBasic.UIBasic;
using WatchBasic.WatchBin.Model;
using WatchBasic.WatchBin;
using WatchDB;
using System.Windows;
using System.Drawing.Imaging;
using System.IO;
using WatchBasic.Tool;
using ControlzEx.Standard;
using Emgu.CV.Structure;
using static Emgu.CV.ML.KNearest;
using System.Runtime.InteropServices;
using Emgu.CV.Reg;
using WatchBasic;
using System.Runtime.Intrinsics.X86;

namespace WatchJieLi.Common
{
    public class DIYSerializeTools
    {
        public static DetailIndex GetIndex(IEnumerable<WatchGroup>? watchGroups, IEnumerable<LayerGroup> layerGroups, int langCount, BinImage binImage, bool isAlbum)
        {

            var detail = new DetailIndex()
            {
                KWHBackground = new DIYX1 { Key = 3 },
                KWHProgressbar = GetDIYX1(watchGroups, "0201", layerGroups, langCount, 1, 4, binImage, isAlbum),
                KWHText_CN = GetDIYX1(watchGroups, "0202", layerGroups, langCount, 2, 5, binImage, isAlbum),
                KWHText_EN = GetDIYX1(watchGroups, "0202", layerGroups, langCount, 1, 6, binImage, isAlbum),
                KWHText_CHT = GetDIYX1(watchGroups, "0202", layerGroups, langCount, 3, 7, binImage, isAlbum),

                KWHPicture = GetDIYX1(watchGroups, "0203", layerGroups, langCount, 1, 8, binImage, isAlbum),
                KWHNum = GetDIYX1(watchGroups, "0205", layerGroups, langCount, 1, 9, binImage, isAlbum),
                KWHPAH = GetDIYX1(watchGroups, "0204", layerGroups, langCount, 1, 10, binImage, isAlbum),
                BlueTooth = GetDIYX1(watchGroups, "0102", layerGroups, langCount, 1, 11, binImage, isAlbum),

                GenerateDateBackground = new DIYX1() { Key = 12 },
                GenerateDateMonthNum = GetDIYX1(watchGroups, "0301", layerGroups, langCount, 1, 13, binImage, isAlbum),
                GenerateDateSeparator = GetDIYX1(watchGroups, "0303", layerGroups, langCount, 1, 14, binImage, isAlbum),
                GenerateDateDayNum = GetDIYX1(watchGroups, "0304", layerGroups, langCount, 1, 15, binImage, isAlbum),

                GenerateDateWeek_CN = GetDIYX1(watchGroups, "0501", layerGroups, langCount, 2, 16, binImage, isAlbum),
                GenerateDateWeek_EN = GetDIYX1(watchGroups, "0501", layerGroups, langCount, 1, 17, binImage, isAlbum),
                GenerateDateWeek_CHT = GetDIYX1(watchGroups, "0501", layerGroups, langCount, 3, 18, binImage, isAlbum),


                GenerateTimeSpan_CN = GetDIYX1(watchGroups, "0502", layerGroups, langCount, 2, 19, binImage, isAlbum),
                GenerateTimeSpan_EN = GetDIYX1(watchGroups, "0502", layerGroups, langCount, 1, 20, binImage, isAlbum),
                GenerateTimeSpan_CHT = GetDIYX1(watchGroups, "0502", layerGroups, langCount, 3, 21, binImage, isAlbum),

                R3 = new DIYX1 { Key = 22 },

                StepBackground = new DIYX1 { Key = 23 },
                StepProgressbar = GetDIYX1(watchGroups, "0701", layerGroups, langCount, 1, 24, binImage, isAlbum),
                StepText_CN = GetDIYX1(watchGroups, "0702", layerGroups, langCount, 2, 25, binImage, isAlbum),
                StepText_EN = GetDIYX1(watchGroups, "0702", layerGroups, langCount, 1, 26, binImage, isAlbum),
                StepText_CHT = GetDIYX1(watchGroups, "0702", layerGroups, langCount, 3, 27, binImage, isAlbum),

                StepNum = GetDIYX1(watchGroups, "0705", layerGroups, langCount, 1, 28, binImage, isAlbum),
                StepUnit_CN = GetDIYX1(watchGroups, "0703", layerGroups, langCount, 2, 29, binImage, isAlbum),
                StepUnit_EN = GetDIYX1(watchGroups, "0703", layerGroups, langCount, 1, 30, binImage, isAlbum),
                StepUnit_CHT = GetDIYX1(watchGroups, "0703", layerGroups, langCount, 3, 31, binImage, isAlbum),
                StepAnimation = GetDIYX1(watchGroups, "0710", layerGroups, langCount, 1, 32, binImage, isAlbum),
                R5 = new DIYX1 { Key = 33 },

                CalorieBackground = new DIYX1 { Key = 34 },
                CalorieProgressbar = GetDIYX1(watchGroups, "0901", layerGroups, langCount, 1, 35, binImage, isAlbum),
                CalorieText_CN = GetDIYX1(watchGroups, "0902", layerGroups, langCount, 2, 36, binImage, isAlbum),
                CalorieText_EN = GetDIYX1(watchGroups, "0902", layerGroups, langCount, 1, 37, binImage, isAlbum),
                CalorieText_CHT = GetDIYX1(watchGroups, "0902", layerGroups, langCount, 3, 38, binImage, isAlbum),


                CalorieNum = GetDIYX1(watchGroups, "0905", layerGroups, langCount, 1, 39, binImage, isAlbum),
                CalorieUnit_CN = GetDIYX1(watchGroups, "0903", layerGroups, langCount, 2, 40, binImage, isAlbum),
                CalorieUnit_EN = GetDIYX1(watchGroups, "0903", layerGroups, langCount, 1, 41, binImage, isAlbum),
                CalorieUnit_CHT = GetDIYX1(watchGroups, "0903", layerGroups, langCount, 3, 42, binImage, isAlbum),
                CalorieAnimation = GetDIYX1(watchGroups, "0909", layerGroups, langCount, 1, 43, binImage, isAlbum),
                R7 = new DIYX1() { Key = 44 },

                HeartBackground = new DIYX1 { Key = 45 },
                HeartText_CN = GetDIYX1(watchGroups, "0801", layerGroups, langCount, 2, 46, binImage, isAlbum),
                HeartText_EN = GetDIYX1(watchGroups, "0801", layerGroups, langCount, 1, 47, binImage, isAlbum),
                HeartText_CHT = GetDIYX1(watchGroups, "0801", layerGroups, langCount, 3, 48, binImage, isAlbum),

                HeartNum = GetDIYX1(watchGroups, "0804", layerGroups, langCount, 1, 49, binImage, isAlbum),
                HeartNull = GetDIYX1(watchGroups, "0803", layerGroups, langCount, 1, 50, binImage, isAlbum),
                HeartUnit_CN = GetDIYX1(watchGroups, "0802", layerGroups, langCount, 2, 51, binImage, isAlbum),
                HeartUnit_EN = GetDIYX1(watchGroups, "0802", layerGroups, langCount, 1, 52, binImage, isAlbum),
                HeartUnit_CHT = GetDIYX1(watchGroups, "0802", layerGroups, langCount, 3, 53, binImage, isAlbum),
                HeartAnimation = GetDIYX1(watchGroups, "0807", layerGroups, langCount, 1, 54, binImage, isAlbum),
                R9 = new DIYX1 { Key = 55 },

                HourTens = GetDIYX1(watchGroups, "0601", layerGroups, langCount, 1, 56, binImage, isAlbum),
                HourUint = GetDIYX1(watchGroups, "0602", layerGroups, langCount, 1, 57, binImage, isAlbum),
                HourSplit = GetDIYX1(watchGroups, "0603", layerGroups, langCount, 1, 58, binImage, isAlbum),
                MinuteTens = GetDIYX1(watchGroups, "0604", layerGroups, langCount, 1, 59, binImage, isAlbum),
                MinuteUint = GetDIYX1(watchGroups, "0605", layerGroups, langCount, 1, 60, binImage, isAlbum),
                MinuteSplit = GetDIYX1(watchGroups, "0606", layerGroups, langCount, 1, 61, binImage, isAlbum),
                SecondTens = GetDIYX1(watchGroups, "0607", layerGroups, langCount, 1, 62, binImage, isAlbum),
                SecondUnit = GetDIYX1(watchGroups, "0608", layerGroups, langCount, 1, 63, binImage, isAlbum),
                R11 = new DIYX1 { Key = 64 },
                R12 = new DIYX1 { Key = 65 },
                HourPointer = GetPointDIYX1(watchGroups, "1301", layerGroups, 66, binImage),
                MinutePointer = GetPointDIYX1(watchGroups, "1302", layerGroups, 67, binImage),
                SecondPointer = GetPointDIYX1(watchGroups, "1303", layerGroups, 68, binImage),
                R13 = new DIYX1 { Key = 69 },
                R14 = new DIYX1 { Key = 70 },
            };
            return detail;
        }
        static DIYX1 GetDIYX1(IEnumerable<BaseGroup>? watchGroups, string groupCode, IEnumerable<LayerGroup>? layerGroups, int languageCount, int languageOrder, int key, BinImage binImage, bool isAlbum)
        {
            var dataFormat = new DIYX1();
            dataFormat.Key = key;
            if (watchGroups == null) return dataFormat;
            var groups = DIYX1BinTool.GetLayerGroups(layerGroups, groupCode);
            if (groups == null || groups.Count <= 0) return dataFormat;

            var group = groups.FirstOrDefault();
            if (group == null || group.LayerNum <= 0) return dataFormat;

            var location = watchGroups?.Where(i => i.GroupCode == groupCode).FirstOrDefault();
            if (location == null) return dataFormat;

            if (languageCount < languageOrder) return dataFormat;

            dataFormat = new DIYX1
            {
                X = location == null ? 0 : location.Left,
                Y = location == null ? 0 : location.Top,
                Height = group?.Height ?? 0,
                Width = group?.Width ?? 0,
                Key = key,
            };

            var parentCode = LayerTool.GetParentCode(groupCode);
            if (!string.IsNullOrWhiteSpace(parentCode) && binImage.ValuePairs.ContainsKey(parentCode))
            {
                var details = binImage.ValuePairs[parentCode];
                //dataFormat.Addrs = details.Addrs;
                //dataFormat.HeadSizes = details.HeadSizes;
                //dataFormat.Num = details.Num;
                dataFormat.ColorValue = details.ColorValue;
                //dataFormat.PicCompress = dataFormat.PicCompress;
                // return dataFormat;
            }

            if (location != null && !string.IsNullOrEmpty(location?.ColorDesc) && (group?.ShowLayer?.IsPng ?? false))
            {
                var color = ColorTranslator.FromHtml(location?.ColorDesc);
                dataFormat.ColorValue = (color.B << 16) | (color.G << 8) | color.R;
                binImage.ColorAddrs.Add((key - 3) * 112 + 107 + 30 + 100);
            }
            foreach (var g in groups)
            {
                if (LayerTool.GetLanguageCode().Contains(groupCode)) //需切换语言
                {
                    var count = g.LayerNum;
                    g.Layers = g.Layers?.Skip((languageOrder - 1) * count).Take(count).ToList();

                }
            }
            dataFormat.Num = group?.Layers == null ? 0 : group.Layers.Count;
            var imageType = DIYX1BinTool.GetImageType(groups.FirstOrDefault()?.Layers);
            var layers = groups.FirstOrDefault()?.Layers?.ToList();
            if (isAlbum)
            {
                if (imageType == ".PNG")
                {
                    dataFormat.PicCompress = PicCompress.png1;//约定好的相册图片类型为2
                    dataFormat!.Addrs.Add(binImage.SetImageAddr());
                    List<List<byte>> ints = new List<List<byte>>();
                    foreach (var layer in layers)
                    {
                        var bmp = new Bitmap(layer.ImgPath ?? "");
                        var imageBytes = GetPng565BytesALast(bmp);
                        ints.Add(imageBytes.ToList());
                    }
                    var max = ints.Select(i => i.Count()).Max();
                    while (max % 4 != 0)
                    {
                        max++;
                    }
                    dataFormat.HeadSizes.Add(max);
                    foreach (var bytes in ints)
                    {
                        while (bytes.Count() < max)
                        {
                            bytes.Add(0);
                        }
                        binImage.Bytes?.AddRange(bytes);
                        binImage.Addr += max;
                    }
                   
                }

            }
            else
            {
                // CRITICAL FIX: Properly handle both BMP and PNG images like the GUI does
                if (imageType == ".BMP")
                {
                    dataFormat.PicCompress = PicCompress.bmp1;
                    DIYX1BinTool.SetBmp(dataFormat, groupCode, layers, binImage);
                }
                else if (imageType == ".PNG")
                {
                    if (group?.Layers?.FirstOrDefault()?.NotDye ?? false) //不需要填色
                    {
                        dataFormat.PicCompress = PicCompress.png1;
                    }
                    else
                    {
                        dataFormat.PicCompress = PicCompress.png2;//只要透明度
                    }
                    DIYX1BinTool.SetPng(dataFormat, groupCode, layers, binImage, false);
                }
            }


            if (LayerTool.IsParentCode(groupCode))
            {
                binImage.ValuePairs.Add(groupCode, new BinImageDetail
                {
                    Addrs = dataFormat.Addrs,
                    HeadSizes = dataFormat.HeadSizes,
                    Num = dataFormat.Num,
                    ColorValue = dataFormat.ColorValue,
                    picCompress = dataFormat.PicCompress
                });
            }
            return dataFormat;
        }
        public static BackgroundIndex GetBackground(IEnumerable<WatchGroup>? watchGroups, IEnumerable<LayerGroup> layerGroups, int languageOrder, WatchSetting Setting, BinImage binImage)
        {
            var layers = layerGroups?.Where(i => i.GroupCode!.Contains("辅助文件")).FirstOrDefault()?.GetLayersByName($"缩略");
            var thumbnail = layers?.Skip(languageOrder).Take(1).FirstOrDefault();
            if (thumbnail == null)
            {
                MessageBox.Show("缩略图文件不存在或命名错误,导出终止");
                throw new Exception("缩略图文件不存在或命名错误");
            }
            thumbnail.Left = Setting.ThumbnailX;
            thumbnail.Top = Setting.ThumbnailY;
            var bgimage = layerGroups?.Where(i => i.GroupCode!.Contains("0101")).FirstOrDefault()?.ShowLayer;
            return new BackgroundIndex
            {
                Thumbnail = GetDIYX1ByBackground("缩略", thumbnail, 1, binImage),
                Background = GetDIYX1ByBackground("背景", bgimage, 2, binImage),
            };
        }



        static DIYX1 GetDIYX1ByBackground(string layerName, Layer? layer, int key, BinImage binImage)
        {
            var dataFormat = new DIYX1();
            dataFormat.Key = key;

            dataFormat = new DIYX1
            {
                X = layer.Left,
                Y = layer.Top,
                Height = layer?.Height ?? 0,
                Width = layer?.Width ?? 0,
                Key = key,
            };
            dataFormat.Num = 1;
            var imageType = DIYX1BinTool.GetImageType(layer);
            List<Layer> layers = new List<Layer>();
            layers.Add(layer!);
            dataFormat.PicCompress = PicCompress.bmp2; //TODO 有歧义
            bool isThumbnail = key == 1;
            SetBitmapWithByte(dataFormat, layers, binImage, isThumbnail);

            //dataFormat.PicCompress = PicCompress.bmp1;
            //dataFormat!.Addrs.Add(binImage.SetImageAddr());
            //var imagePath = layers?.FirstOrDefault()?.ImgPath;
            //var bmp = new Bitmap(imagePath ?? "");
            //var imageBytes = GetPng565Bytes(bmp);
            //dataFormat.HeadSizes.Add(imageBytes.Length);
            //binImage.Bytes?.AddRange(imageBytes);
            //binImage.Addr += imageBytes.Length;
            return dataFormat;
        }
        /// <summary>
        /// 指针专用
        /// </summary>
        /// <param name="groupCode"></param>
        /// <param name="layerGroups"></param>
        /// <param name="key"></param>
        /// <param name="binImage"></param>
        /// <returns></returns>
        static DIYX1 GetPointDIYX1(IEnumerable<BaseGroup>? watchGroups, string groupCode, IEnumerable<LayerGroup>? layerGroups, int key, BinImage binImage)
        {
            var dataFormat = new DIYX1();
            dataFormat.Key = key;

            var groups = DIYX1BinTool.GetLayerGroups(layerGroups, groupCode);
            if (groups == null || groups.Count <= 0) return dataFormat;

            var group = groups.FirstOrDefault();
            if (group == null || group.LayerNum <= 0) return dataFormat;
            var location = watchGroups?.Where(i => i.GroupCode == groupCode).FirstOrDefault();
            if (location == null) return dataFormat;
            dataFormat = new DIYX1
            {
                X = location.Left,
                Y = location.Top,
                Height = group?.Height ?? 0,
                Width = group?.Width ?? 0,
                Key = key,
                Code = groupCode,
            };
            dataFormat.Num = 1;
            var imageType = DIYX1BinTool.GetImageType(groups.FirstOrDefault()?.Layers);
            var layers = groups.FirstOrDefault()?.Layers?.ToList();
            if (imageType == ".PNG")
            {
                dataFormat.PicCompress = PicCompress.bmp1;//这里指不压缩
                dataFormat!.Addrs.Add(binImage.SetImageAddr());
                var imagePath = layers?.FirstOrDefault()?.ImgPath;
                var bmp = new Bitmap(imagePath ?? "");
                ////计算指针坐标
                //var centerX=CommonDefintion.Setting.Width / 2;
                //var centerY=CommonDefintion.Setting.Height /2;
                //var y = centerY - bmp.Height;
                //var x=centerX - bmp.Width/2;
                //dataFormat.X = x;
                //dataFormat.Y = y;
                var imageBytes = GetPng565Bytes(bmp);
                dataFormat.HeadSizes.Add(imageBytes.Length);
                binImage.Bytes?.AddRange(imageBytes);
                binImage.Addr += imageBytes.Length;
            }
            else
            {
                throw new Exception("指针图片类型错误");
            }
            return dataFormat;

        }
       public static byte[] GetPng565Bytes(Bitmap bitmap)
        {
            List<byte> bytes = new List<byte>();
            for (var i = 0; i < bitmap?.Height; i++)
            {
                for (var j = 0; j < bitmap?.Width; j++)
                {
                    var color = bitmap.GetPixel(j, i);
                    var rgb565 = (color.R >> 3 << 11) | (color.G >> 2 << 5) | (color.B >> 3);
                    bytes.Add(color.A);
                    bytes.Add((byte)(rgb565 >> 8));
                    bytes.Add((byte)(rgb565 & 0xff));
                    
                }
            }
            return bytes.ToArray();
        }

        public static byte[] GetPng565BytesALast(Bitmap bitmap)
        {
            List<byte> bytes = new List<byte>();
            for (var i = 0; i < bitmap?.Height; i++)
            {
                for (var j = 0; j < bitmap?.Width; j++)
                {
                    var color = bitmap.GetPixel(j, i);
                    var rgb565 = (color.R >> 3 << 11) | (color.G >> 2 << 5) | (color.B >> 3);
                    bytes.Add((byte)(rgb565 >> 8));
                    bytes.Add((byte)(rgb565 & 0xff));
                    bytes.Add(color.A);
                }
            }
            return bytes.ToArray();
        }

        public static IntPtr StructToIntPtr<T>(T info)
        {
            int size = Marshal.SizeOf(info);
            IntPtr intPtr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(info, intPtr, true);
            return intPtr;
        }
        public static byte[] GetImageCompess(Bitmap bitmap)
        {

            var imageBytes = GetPng565Bytes(bitmap);
            IntPtr imageDataPtr = Marshal.AllocHGlobal(imageBytes.Length);
            Marshal.Copy(imageBytes, 0, imageDataPtr, imageBytes.Length);
            image_info info = new image_info()
            {
                width = bitmap.Width,
                height = bitmap.Height,
                format = 1,
                buf = imageDataPtr,
                len = imageBytes.Length,
            };
            var infoIntPtr = StructToIntPtr<image_info>(info);

            // var resultPtr = new ImageHelper($"{AppDomain.CurrentDomain.BaseDirectory}img64\\image.dll").image_compress(infoIntPtr);
            var resultPtr = ImageHelper.image_compress2(infoIntPtr);

            image_buf result = Marshal.PtrToStructure<image_buf>(resultPtr);

            byte[] compressedData = new byte[result.len];
            Marshal.Copy(result.buf, compressedData, 0, result.len);
            // 释放内存
            Marshal.FreeHGlobal(imageDataPtr);
            Marshal.FreeHGlobal(result.buf);


            return compressedData;
        }
        public static void SetBitmapWithByte(DIYBase dataFormat, List<Layer>? layers, BinImage BinImage, bool isThumbnail = false)
        {
            if (layers == null || layers.Count == 0)
            {
                dataFormat!.Addr = 0;
            }
            dataFormat!.Addrs.Add(BinImage.SetImageAddr());
            List<List<byte>> pngBytes = new List<List<byte>>();

            if (layers != null)
            {
                foreach (var layer in layers)
                {
                    var imagePath = layer.ImgPath;
                    var bmp = new Bitmap(imagePath ?? "");
                    if (isThumbnail)
                    {
                        bmp = DIYX1BinTool.GetThumnail(imagePath, CommonDefintion.Setting.ProjectName);
                    }
                    pngBytes.Add(GetImageCompess(bmp).ToList());
                    bmp.Dispose();
                }
            }
            var maxSize = pngBytes.Select(i => i.Count).Max();
            while (maxSize % 4 != 0)
            {
                maxSize++;
            }
            dataFormat!.HeadSizes.Add(maxSize);
            foreach (var png in pngBytes)
            {
                var len = png.Count();
                while (png.Count < maxSize)
                {
                    png.Add(0);
                }
                //png.InsertRange(0, len.GetBytes32());
                BinImage.Bytes?.AddRange(png);
                BinImage.Addr += maxSize;
            }
        }
    }

}
