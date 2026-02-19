using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchBasic.Tool;
using WatchBasic.ZHPicRSA.Model;

namespace WatchBasic.ZHPicRSA
{
    public class BmpRSA
    {
        //public static byte[] GetImageToBytesCompress(string path, Bitmap map = null)
        //{
        //    List<byte> rgbBytes = new List<byte>();
        //    List<byte> compressBytes = new List<byte>();

        //    var bitmap = new Bitmap(path); ;
        //    if (map != null)
        //    {
        //        bitmap = new Bitmap(map);
        //    }
            
        //    BmpCompress helper = new BmpCompress();
        //    for (var i = 0; i < bitmap.Height; i++)
        //    {
        //        for (var j = 0; j < bitmap.Width; j++)
        //        {
        //            var color = bitmap.GetPixel(j, i);
        //            var colorVal = ImageUtils.GetBytesRGB565(color);
        //            var val = ImageUtils.RGB888ToRGB565(color.R, color.G, color.B);
        //            rgbBytes.AddRange(colorVal);

        //            if (j == 0 && i == 0)
        //            {
        //                helper.Color = val;
        //                helper.Count++;
        //                continue;
        //            }
        //            if (helper.Color != val || helper.Count == 255)
        //            {
                      
        //                compressBytes.AddRange(helper.CompressBytes);

        //                helper = new BmpCompress
        //                {
        //                    Count = 1,
        //                    Color = val,
        //                };
        //            }
        //            else
        //            {
        //                helper.Count++;
        //            }

        //        }
        //    }

        //    compressBytes.AddRange(helper.CompressBytes);
        //    return rgbBytes.Count > compressBytes.Count ? compressBytes.ToArray() : rgbBytes.ToArray();
        //}

        public static List<BmpRow> GetBmpRows(Bitmap bitmap)
        {

            List<BmpRow> bmpRows = new List<BmpRow>();
            for (var i = 0; i < bitmap.Height; i++)
            {
                BmpRow row = new BmpRow();
                BmpCompress helper = new BmpCompress();
                for (var j = 0; j < bitmap.Width; j++)
                {
                    var color = bitmap.GetPixel(j, i);
                    var val = 0;
                    if (CommonDefintion.IsColor888) 
                    {
                        val= ImageUtils.GetColor888(color);
                       
                    }
                    else 
                    {
                        val = ImageUtils.RGB888ToRGB565(color.R, color.G, color.B);
                    }
                    //var bytes =ImageUtils.RGB24TORGB16Forward(color.R, color.G, color.B);
                  
                    //val = BitConverter.ToUInt16(bytes);
                    if (j == 0)
                    {
                        helper.Color = val;
                        helper.Count = 1;
                        row.BmpCompresses!.Add(helper);
                        continue;
                    }
                    if (helper.Color != val || helper.Count == 255)
                    {
                        helper = new BmpCompress
                        {
                            Count = 1,
                            Color = val,
                        };
                        row.BmpCompresses!.Add(helper);
                    }
                    else
                    {
                        helper.Count++;
                    }
                }
                bmpRows.Add(row);
            }
            return bmpRows;
        }

        public static byte[] GetImageToBytes(Bitmap bitmap)
        {
            List<byte> rgbBytes = new List<byte>();
            for (var i = 0; i < bitmap.Height; i++)
            {
                for (var j = 0; j < bitmap.Width; j++)
                {
                    var color = bitmap.GetPixel(j, i);
                    if (!CommonDefintion.IsColor888) 
                    {
                        var colorVal = ImageUtils.RGB888ToRGB565(color.R, color.G, color.B);
                        var colorBytes = colorVal.GetBytes16(true);
                        rgbBytes.AddRange(colorBytes);
                    }
                    else 
                    {
                        var colorVal = ImageUtils.GetColor888(color);
                        var colorBytes = colorVal.GetBytes(3,true);
                        rgbBytes.AddRange(colorBytes);
                    }
                   
                }
            }
            return rgbBytes.ToArray();
        }
    }
}
