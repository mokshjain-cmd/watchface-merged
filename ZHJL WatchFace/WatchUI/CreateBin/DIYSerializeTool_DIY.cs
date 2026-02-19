using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WatchBasic;
using WatchBasic.UIBasic;
using WatchBasic.WatchBin;
using WatchBasic.WatchBin.Model;
using WatchDB;
using WatchDB_DIY;
using WatchDBDIYJieLi;
using WatchUI.common;

namespace WatchUI.CreateBin
{
    public class DIYSerializeTool_DIY
    {
        public static IntPtr StructToIntPtr<T>(T info)
        {
            int size = Marshal.SizeOf(info);
            IntPtr intPtr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(info, intPtr, true);
            return intPtr;
        }

        static byte[] GetPng565Bytes(Bitmap bitmap)
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


        public static bool IsTransparency(Bitmap bitmap)
        {
            List<byte> bytes = new List<byte>();
            for (var i = 0; i < bitmap?.Height; i++)
            {
                for (var j = 0; j < bitmap?.Width; j++)
                {
                    var color = bitmap.GetPixel(j, i);
                    bytes.Add(color.A);
                }
            }
            return bytes.All(x => x == 0);
        }


        public static DIYX1 GetDIYX1(IEnumerable<BaseGroup>? watchGroups, string groupCode, LocationDetail detail, IEnumerable<LayerGroup>? layerGroups, int languageOrder, BinImage binImage)
        {
            var dataFormat = new DIYX1() { Key = 0 };
            binImage.CurrentAddr += 112;//X1类型长度
            if (watchGroups == null) return dataFormat;

            var groups = DIYX1BinTool.GetLayerGroups(layerGroups, groupCode);
            if (groups == null || groups.Count <= 0) return dataFormat;

            var group = groups.FirstOrDefault();
            if (group == null || group.LayerNum <= 0) return dataFormat;

            var location = watchGroups?.Where(i => i.GroupCode == groupCode).FirstOrDefault();
            if (location == null) return dataFormat;

            dataFormat = new DIYX1
            {
                X = location == null ? 0 : location.Left + (detail?.WatchLocation?.Left ?? 0),
                Y = location == null ? 0 : location.Top + (detail?.WatchLocation?.Top ?? 0),
                Height = group?.Height ?? 0,
                Width = group?.Width ?? 0,
                Key = 1,
            };
            if (location != null && !string.IsNullOrEmpty(location?.ColorDesc) && (group?.ShowLayer?.IsPng ?? false) && (!(group?.ShowLayer?.NotDye ?? false)))
            {
                var color = ColorTranslator.FromHtml(location?.ColorDesc);
                dataFormat.ColorValue = (color.B << 16) | (color.G << 8) | color.R;
                binImage.ColorAddrs.Add(binImage.CurrentAddr + 107 - 112);//107颜色所在位置
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
            if (CommonDefintion.IsJieLi) 
            {
                var firstLayer = new Bitmap(layers.FirstOrDefault().ImgPath);
                if (!IsTransparency(firstLayer)) 
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
                else 
                {
                    dataFormat.Key = 0;
                }
            }
            else 
            {
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
           

            return dataFormat;
        }

        public static IEnumerable<DIYX1> GetPointer(IEnumerable<LayerGroup>? layerGroups, BinImage binImage, WatchPointer pointer = null)
        {
            List<DIYX1> dIYX1s = new List<DIYX1>();
            dIYX1s.Add(GetDIYX1("3101", layerGroups, binImage,pointer));
            dIYX1s.Add(GetDIYX1("3102", layerGroups, binImage,pointer));
            dIYX1s.Add(GetDIYX1("3103", layerGroups, binImage, pointer));
            return dIYX1s;
        }

        /// <summary>
        /// 指针专用
        /// </summary>
        /// <param name="groupCode"></param>
        /// <param name="layerGroups"></param>
        /// <param name="binImage"></param>
        /// <returns></returns>
        static DIYX1 GetDIYX1(string groupCode, IEnumerable<LayerGroup>? layerGroups, BinImage binImage, WatchPointer pointer = null)
        {
            var dataFormat = new DIYX1() { Key = 0 };

            var groups = DIYX1BinTool.GetLayerGroups(layerGroups, groupCode);
            if (groups == null || groups.Count <= 0) return dataFormat;

            var group = groups.FirstOrDefault();
            if (group == null || group.LayerNum <= 0) return dataFormat;
            dataFormat = new DIYX1
            {
                Height = group?.Height ?? 0,
                Width = group?.Width ?? 0,
                Key = 1,
                Code = groupCode,
            };

            dataFormat.Num = 16;
            var imageType = DIYX1BinTool.GetImageType(groups.FirstOrDefault()?.Layers);
            var layers = groups.FirstOrDefault()?.Layers?.ToList();
           
            if (CommonDefintion.IsJieLi) 
            {
                if (imageType != ".PNG") throw new Exception("指针图片类型错误");
                if(pointer==null) throw new Exception("指针数据错误");
                var loaction = pointer.PointerGroups.FirstOrDefault(f => f.GroupCode == groupCode);
                if (loaction == null) throw new Exception("指针数据错误");
                dataFormat.X=loaction.Left; 
                dataFormat.Y=loaction.Top;
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

                //dataFormat.PicCompress = PicCompress.bmp2; //TODO 有歧义
                //SetBitmapWithByte(dataFormat, layers, binImage);
            } 
            else 
            {
                if (imageType == ".BMP")
                {
                    dataFormat.PicCompress = PicCompress.bmp1;
                    DIYX1BinTool.SetBmp(dataFormat, groupCode, layers, binImage);
                }
                else if (imageType == ".PNG")
                {
                    dataFormat.PicCompress = PicCompress.png1;
                    DIYX1BinTool.SetPng(dataFormat, groupCode, layers, binImage, true);
                }
            }
            
            return dataFormat;

        }

        static DIYX1 GetDIYX1(IEnumerable<BaseGroup>? watchGroups, string groupCode, IEnumerable<LayerGroup>? layerGroups, int languageCount, int languageOrder, BinImage binImage, bool IsDiy = false)
        {
            var dataFormat = new DIYX1() { Key = 0 };
            binImage.CurrentAddr += 112;//X1类型长度
            if (watchGroups == null) return dataFormat;

            var groups = DIYX1BinTool.GetLayerGroups(layerGroups, groupCode);
            if (groups == null || groups.Count <= 0) return dataFormat;

            var group = groups.FirstOrDefault();
            if (group == null || group.LayerNum <= 0) return dataFormat;

            var location = watchGroups?.Where(i => i.GroupCode == groupCode).FirstOrDefault();
            //if (location == null) return dataFormat;

            if (languageCount < languageOrder) return dataFormat;

            dataFormat = new DIYX1
            {
                X = location == null ? 0 : location.Left,
                Y = location == null ? 0 : location.Top,
                Height = group?.Height ?? 0,
                Width = group?.Width ?? 0,
                Key = 1,

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

            if (group?.ShowLayer?.IsPng ?? false)
            {
                var color = ColorTranslator.FromHtml(group?.ShowLayer?.ColorDesc);
                if (!string.IsNullOrEmpty(location?.ColorDesc))
                {
                    color= ColorTranslator.FromHtml(location.ColorDesc);
                }
                dataFormat.ColorValue = (color.B << 16) | (color.G << 8) | color.R;
                binImage.ColorAddrs.Add(binImage.CurrentAddr + 107 - 112);
                binImage.LocationLefts.Add(binImage.CurrentAddr + 1 - 112);//X
                binImage.LocationTops.Add(binImage.CurrentAddr + 3 - 112);//Y
                binImage.DisableAddrs.Add(binImage.CurrentAddr  - 112);
                binImage.GroupCodes.Add(groupCode);
                // ImageUtils.RGB888ToRGB565(color.R, color.G, color.B);
                //dataFormat.ColorValue = color.;
                // binImage.ColorAddrs.Add((key - 3) * 112 + 107 + 30 + 100);
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
            if (CommonDefintion.IsJieLi)
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
            else
            {
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
           

            return dataFormat;
        }

        public static DIYTime GetDIYTime(IEnumerable<BaseGroup>? timeGroups, IEnumerable<LayerGroup>? layerGroups, int languageCount, int languageOrder, BinImage binImage)
        {
            return new DIYTime
            {
                HourTens = GetDIYX1(timeGroups, "3001", layerGroups, languageCount, languageOrder, binImage, true),
                HourUnit = GetDIYX1(timeGroups, "3002", layerGroups, languageCount, languageOrder, binImage, true),
                HourSplit = GetDIYX1(timeGroups, "3003", layerGroups, languageCount, languageOrder, binImage, true),
                MinuteTens = GetDIYX1(timeGroups, "3004", layerGroups, languageCount, languageOrder, binImage, true),
                MinuteUnit = GetDIYX1(timeGroups, "3005", layerGroups, languageCount, languageOrder, binImage, true),
                MinuteSplit = GetDIYX1(timeGroups, "3006", layerGroups, languageCount, languageOrder, binImage, true),
                SecondTens = GetDIYX1(timeGroups, "3007", layerGroups, languageCount, languageOrder, binImage, true),
                SecondUnit = GetDIYX1(timeGroups, "3008", layerGroups, languageCount, languageOrder, binImage, true),

            };
        }

    }






}
