using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WatchBasic.Tool;
using Color = System.Windows.Media.Color;

namespace WatchBasic.ZHPicRSA
{
    public class DecodeMap
    {
        public static Bitmap GetBmp2(byte[] bytes, byte[] heads, int addr, int width, int height)
        {
            // var heads = bytes.Skip((int)addr).Take(height * 2);//头

            var headlist = heads.Select((x, i) => new { Index = i, Value = x }).GroupBy(x => x.Index / 2)
                       .Select(x => x.Select(v => v.Value).ToList()).ToList();
            var bitmap = new Bitmap(width, height);
            var init = 0;
            for (var i = 0; i < headlist.Count; i++)
            {
                var count = headlist[i].ToArray().GetInt16(true);
                var num = count - init;

                var imageDatas = bytes.Skip((int)addr + init * 3).Take(num * 3);
                init = count;
                var list = imageDatas.Select((x, i) => new { Index = i, Value = x }).GroupBy(x => x.Index / 3)
                      .Select(x => x.Select(v => v.Value).ToList()).ToList();
                if (list.Count == 0) continue;
                List<int> data = new List<int>();
                list.ForEach(j =>
                {

                    //j.Add(0);
                    //j.Reverse();
                    //if (j.Count != 4)
                    //{
                    //    j.Add(0);
                    //}
                    //var len = j[1];
                    //var color = j.Skip(2).Take(2).Reverse().GetInt16(true);
                    var len = j.Last();
                    var color=j.Take(2).GetInt16(false);
                    while (len > 0)
                    {
                        data.Add((int)color);
                        len--;
                    }
                });
                if (data.Count == width)
                    for (var j = 0; j < data.Count; j++)
                    {
                        bitmap.SetPixel(j, i, ImageUtils.GetColor(data[j]));
                    }
            }

            return bitmap;
        }

        public static Bitmap GetBmp24(byte[] bytes, byte[] heads, int addr, int width, int height)
        {
            // var heads = bytes.Skip((int)addr).Take(height * 2);//头

            var headlist = heads.Select((x, i) => new { Index = i, Value = x }).GroupBy(x => x.Index / 4)
                       .Select(x => x.Select(v => v.Value).ToList()).ToList();
            var bitmap = new Bitmap(width, height);
            var init = 0;
            for (var i = 0; i < headlist.Count; i++)
            {
                var count = (int)headlist[i].ToArray().GetInt32(true);
                var num = count - init;

                var imageDatas = bytes.Skip((int)addr + init * 4).Take(num * 4);
                init = count;
                var list = imageDatas.Select((x, i) => new { Index = i, Value = x }).GroupBy(x => x.Index / 4)
                      .Select(x => x.Select(v => v.Value).ToList()).ToList();
                if (list.Count == 0) continue;
                List<Color> data = new List<Color>();
                list.ForEach(j =>
                {

                    //j.Add(0);
                    //j.Reverse();
                    //if (j.Count != 4)
                    //{
                    //    j.Add(0);
                    //}
                    //var len = j[1];
                    //var color = j.Skip(2).Take(2).Reverse().GetInt16(true);
                    var len = j.Last();
                    var color = j.Take(3).ToList();
                    Color color1 = Color.FromRgb(color[0], color[1], color[2]);
                    while (len > 0)
                    {
                        data.Add(color1);
                        len--;
                    }
                });
                if (data.Count == width)
                    for (var j = 0; j < data.Count; j++)
                    {
                        bitmap.SetPixel(j, i,  System.Drawing.Color.FromArgb(data[j].R, data[j].G, data[j].B));
                    }
            }

            return bitmap;
        }

        public static Bitmap GetBmp(byte[] bytes, int addr, int count, int width, int height)
        {

            var imageDatas = bytes.Skip((int)addr).Take(count);
            var bitmap = new Bitmap(width, height);

            var list = imageDatas.Select((x, i) => new { Index = i, Value = x }).GroupBy(x => x.Index / 3)
                       .Select(x => x.Select(v => v.Value).ToList()).ToList();
            if (list.Count == 0) return bitmap;
            List<int> data = new List<int>();
            list.ForEach(i =>
            {
                i.Reverse();
                i.Add(0);

                var num = BitConverter.ToUInt32(i.ToArray());
                var len = num >> 16;
                var color = (num << 16) >> 16;
                while (len > 0)
                {
                    data.Add((int)color);
                    len--;
                }
            });//还原像素
            int row = 0, column = 0;
            for (var i = 0; i < data.Count; i++)
            {
                if (i != 0 && i % width == 0) //换行
                {
                    row += 1;
                    column = 0;
                }
                //rgb888 = (r << 16) | (g << 8) | b

                bitmap.SetPixel(column, row, ImageUtils.GetColor(data[i]));
                column++;
            }

            return bitmap;
        }

        public static Bitmap GetBmp(byte[] imageBytes, int width, int height)
        {
            var bitmap = new Bitmap(width, height);
            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    var colorBytes = imageBytes.Skip(i * (width * 2) + j * 2).Take(2);
                    var colorValue = colorBytes.GetInt16(true);

                    bitmap.SetPixel(j, i, ImageUtils.GetColor(colorValue));
                }
            }
            return bitmap;
        }

        public static Bitmap GetBmp24(byte[] imageBytes, int width, int height)
        {
            var bitmap = new Bitmap(width, height);
            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    var colorBytes = imageBytes.Skip(i * (width * 3) + j * 3).Take(3).ToList();
                    //var colorValue = colorBytes.GetInt16(true);

                    bitmap.SetPixel(j, i, System.Drawing.Color.FromArgb(colorBytes[0], colorBytes[1], colorBytes[2]));
                }
            }
            return bitmap;
        }


        public static List<Bitmap> GetBmps(byte[] dataBytes, int width, int height, int addr, int num, int size)
        {
            List<Bitmap> bitmap = new List<Bitmap>();
            for (int map = 0; map < num; map++)
            {
                var imageBytes = dataBytes.Skip((int)addr + (map * size)).Take(size);
                var imageAddr = BitConverter.ToUInt32(imageBytes.Take(4).ToArray());
                var count = BitConverter.ToInt32(imageBytes.Skip(4).Take(4).Reverse().ToArray());//取头信息
                var map1 = GetBmp(dataBytes, (int)imageAddr, count, width, height);
                map1.Save($"{map}.bmp");
                bitmap.Add(map1);
            }
            return bitmap;
        }


        public static List<Bitmap> GetBmps2(byte[] addrInfo, byte[] dataBytes, int width, int height, int num)
        {
            List<Bitmap> bitmap = new List<Bitmap>();

            for (int map = 0; map < num; map++)
            {
                // if (map == 6) break;
                var addr = addrInfo.Skip(map * 2 * 4).Take(4).GetInt32();
                var size = addrInfo.Skip((map * 2 + 1) * 4).Take(4).GetInt32();
                var size2 = width * height * 2;
                if (size < width * height * 2) //压缩
                {
                    var imageBytes = dataBytes.Skip((int)addr).Take((int)size);
                    var heads = imageBytes.Take(2 * height);
                    var map1 =GetBmp2(dataBytes, heads.ToArray(), (int)addr + heads.Count(), width, height);//16
                    //var map1 = GetBmp24(dataBytes, heads.ToArray(), (int)addr + heads.Count(), width, height);//24
                    bitmap.Add(map1);
                }
                else
                {
                    var imageBytes = dataBytes.Skip((int)addr).Take(size2).ToArray();
                    bitmap.Add(GetBmp24(imageBytes, width, height));
                }

            }
            return bitmap;
        }


        public static List<Bitmap> GetBmps24(byte[] addrInfo, byte[] dataBytes, int width, int height, int num)
        {
            List<Bitmap> bitmap = new List<Bitmap>();

            for (int map = 0; map < num; map++)
            {
                // if (map == 6) break;
                var addr = addrInfo.Skip(map * 2 * 4).Take(4).GetInt32();
                var size = addrInfo.Skip((map * 2 + 1) * 4).Take(4).GetInt32();
                var size2 = width * height * 3;
                if (size < width * height * 3) //压缩
                {
                    var imageBytes = dataBytes.Skip((int)addr).Take((int)size);
                    var heads = imageBytes.Take(4 * height);
                    var map1 = GetBmp24(dataBytes, heads.ToArray(), (int)addr + heads.Count(), width, height);
                    bitmap.Add(map1);
                }
                else
                {
                    var imageBytes = dataBytes.Skip((int)addr).Take(size2).ToArray();
                    bitmap.Add(GetBmp24(imageBytes, width, height));
                }

            }
            return bitmap;
        }


        public static List<Bitmap> Getjieli(int widith,int height, byte[] images)
        {
            List<Bitmap> bitmap = new List<Bitmap>();

            

            
            return bitmap;
        }


        //public static List<Bitmap> GetBmps(byte[] dataBytes, int width, int height, int addr, int num, int size)
        //{
        //    List<Bitmap> bitmap = new List<Bitmap>();
        //    for (int map = 0; map < num; map++)
        //    {
        //        var imageBytes = dataBytes.Skip((int)addr + (map * size)).Take(size);
        //        var imageAddr = BitConverter.ToUInt32(imageBytes.Take(4).ToArray());
        //        var count = BitConverter.ToInt32(imageBytes.Skip(4).Take(4).Reverse().ToArray());//取头信息
        //        var map1 = GetBmp(dataBytes, (int)imageAddr, count, width, height);
        //        map1.Save($"{map}.bmp");
        //        bitmap.Add(map1);
        //    }
        //    return bitmap;
        //}

        public static Bitmap GetPng(int width, int height, int x, int y, int addr, byte[] rowHeads, byte[] dataBytes,bool isColor24)
        {
            var bitmap = new Bitmap(width, height);
            var last = 0;
            for (var i = 0; i < height; i++)
            {
                var rowbytes = rowHeads.Skip(i * 4).Take(4);
                var xstart = rowbytes.Take(2).GetInt16();
                var rowWidth = rowbytes.Skip(2).Take(2).GetInt16();
                var currentwidth = rowWidth - last;
                for (var j = 0; j < currentwidth; j++)
                {
                    if (isColor24) 
                    {
                        var colorBytes = dataBytes.Skip((last + j) * 4 + addr).Take(4);
                        var color24Bytes = colorBytes.Take(3).ToList();
                        var colorA = (int)colorBytes.Skip(3).Take(1).FirstOrDefault();
                        bitmap.SetPixel(xstart - x + j, i, System.Drawing.Color.FromArgb(colorA,color24Bytes[0], color24Bytes[1], color24Bytes[2]));
                    }
                    else 
                    {
                        var colorBytes = dataBytes.Skip((last + j) * 3 + addr).Take(3);
                        var color565Bytes = colorBytes.Take(2);
                        var colorNum = color565Bytes.GetInt16();
                        var color = ImageUtils.GetColor(colorNum);
                        var colorA = (int)colorBytes.Skip(2).Take(1).FirstOrDefault();
                        bitmap.SetPixel(xstart - x + j, i, System.Drawing.Color.FromArgb(colorA, color.R, color.G, color.B));
                    }
                    
                }
                last = rowWidth;
            }
            return bitmap;
        }

        public static Bitmap GetPng2(int width, int height, int x, int y, int addr, byte[] rowHeads, byte[] dataBytes, System.Drawing.Color color)
        {
            var bitmap = new Bitmap(width, height);
            var last = 0;
            for (var i = 0; i < height; i++)
            {
                var rowbytes = rowHeads.Skip(i * 4).Take(4);
                var xstart = rowbytes.Take(2).GetInt16();
                var rowWidth = rowbytes.Skip(2).Take(2).GetInt16();
                var currentwidth = rowWidth - last;
                for (var j = 0; j < currentwidth; j++)
                {
                    var colorA = dataBytes.Skip((last + j) * 1 + addr).Take(1).First();
                    if (colorA > 0)
                        bitmap.SetPixel(xstart - x + j, i, System.Drawing.Color.FromArgb(colorA, color.R, color.G, color.B));
                }
                last = rowWidth;
            }

            return bitmap;
        }


        public static List<Bitmap> GetBasePngs(byte[] dataBytes, int addr, int num, int size, System.Drawing.Color? color = null)
        {
            List<Bitmap> bitmapList = new List<Bitmap>();
            for (int map = 0; map < num; map++)
            {
                //var bytes = dataBytes.Skip((int)addr + (map * size)).Take(size);
                //var len= bytes.Take(4).GetInt32();
                //var img = bytes.Skip(4).Take((int)len);
                //using (MemoryStream stream = new MemoryStream(img.ToArray()))
                //{
                //    var bit = new Bitmap(stream);
                //    bit.Save($"{map}.png");
                //   bitmapList.Add(bit);
                //}
                //var height = headInfos.Take(2).GetInt16();
                //var imgAddr = headInfos.Skip(2).Take(4).GetInt32();
                //var x = headInfos.Skip(6).Take(2).GetInt16();
                //var y = headInfos.Skip(8).Take(2).GetInt16();
                //var width = headInfos.Skip(10).Take(2).GetInt16();
                //var rowHeads = headInfos.Skip(12).Take(4 * height);
                //List<Byte> AAA = new List<byte>();
                //AAA.AddRange(headInfos);
                //AAA.AddRange(dataBytes.Skip((int)imgAddr).Take(height * width * 3));

                //if (color == null)
                //{
                //    var image = GetPng(width, height, x, y, (int)imgAddr, rowHeads.ToArray(), dataBytes, false);//true rgb24
                //    image.Save($"{map + 1}.png");
                //    bitmapList.Add(image);
                //}
                //else
                //{
                //    var image = GetPng2(width, height, x, y, (int)imgAddr, rowHeads.ToArray(), dataBytes, (System.Drawing.Color)color);
                //    bitmapList.Add(image);
                //}


            }
            return bitmapList;
        }

        public static List<Bitmap> GetPngs(byte[] dataBytes, int addr, int num, int size, System.Drawing.Color? color = null)
        {
            List<Bitmap> bitmapList = new List<Bitmap>();
            for (int map = 0; map < num; map++)
            {
                var headInfos = dataBytes.Skip((int)addr + (map * size)).Take(size);
                var height = headInfos.Take(2).GetInt16();
                var imgAddr = headInfos.Skip(2).Take(4).GetInt32();
                var x = headInfos.Skip(6).Take(2).GetInt16();
                var y = headInfos.Skip(8).Take(2).GetInt16();
                var width = headInfos.Skip(10).Take(2).GetInt16();
                var rowHeads = headInfos.Skip(12).Take(4 * height);
                List<Byte> AAA = new List<byte>();
                AAA.AddRange(headInfos);
                AAA.AddRange(dataBytes.Skip((int)imgAddr).Take(height * width * 3));

                if (color == null)
                {
                    var image = GetPng(width, height, x, y, (int)imgAddr, rowHeads.ToArray(), dataBytes,false);//true rgb24
                    image.Save($"{map + 1}.png");
                    bitmapList.Add(image);
                }
                else
                {
                    var image = GetPng2(width, height, x, y, (int)imgAddr, rowHeads.ToArray(), dataBytes, (System.Drawing.Color)color);
                    bitmapList.Add(image);
                }


            }
            return bitmapList;
        }
        public static ImageSource ToBitmapSource(Bitmap bitmap)
        {
            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Bmp);
            stream.Position = 0;
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = stream;
            bitmapImage.EndInit();
            return bitmapImage;
        }




        //public static Bitmap GetPng2(int width, int height, int x, int y, int addr, byte[] rowHeads, byte[] columnDescs, byte[] dataBytes)
        //{
        //    var headsList = rowHeads.Select((i, index) => new { index = index, val = i }).GroupBy(i => i.index / 2).Select(i => i.Select(i => i.val)).ToList();
        //    var rowIndex = 0;
        //    var yindex = 0;
        //    var colorIndex = 0;
        //    var lastrcount = 0;
        //    var lastCount = 0;
        //    var bitmap = new Bitmap(width, height);
        //    foreach (var i in headsList)
        //    {
        //        var rcount = BitConverter.ToUInt16(i.ToArray());
        //        var currentCount = rcount - lastrcount;

        //        for (var j = 0; j < currentCount; j++)
        //        {
        //            var rowbytes = columnDescs.Skip(rowIndex * 4).Take(4);
        //            var xstart = BitConverter.ToUInt16(rowbytes.Take(2).ToArray());
        //            var count = BitConverter.ToUInt16(rowbytes.Skip(2).Take(2).ToArray());
        //            var ccount = count - lastCount;
        //            for (var k = 0; k < ccount; k++) //
        //            {
        //                var colorBytes = dataBytes.Skip(colorIndex * 3 + addr).Take(3);
        //                var color565Bytes = colorBytes.Take(2);
        //                var colorNum = BitConverter.ToInt16(color565Bytes.ToArray());
        //                var color = ImageUtils.GetColor(colorNum);
        //                var colorA = (int)colorBytes.Skip(2).Take(1).FirstOrDefault();
        //                bitmap.SetPixel(xstart - x + k, yindex, System.Drawing.Color.FromArgb(colorA, color.R, color.G, color.B));
        //                colorIndex++;
        //            }
        //            lastCount = count;
        //            rowIndex++;

        //        }
        //        lastrcount = rcount;

        //        yindex++;
        //    }
        //    return bitmap;
        //}

        //H(2Byte)+Addr(4Byte)+X(2Byte)+Y(2Byte)+W(2Byte)+(2byte)*H+(4byte)*Sum(H[]);
        //public static List<Bitmap> GetPngs2(byte[] dataBytes, int addr, int num, int size)
        //{
        //    List<Bitmap> bitmapList = new List<Bitmap>();

        //    for (int map = 0; map < num; map++)
        //    {
        //        var headInfos = dataBytes.Skip((int)addr + (map * size)).Take(size);
        //        var height = headInfos.Take(2).GetInt16();
        //        var imageaddr = headInfos.Skip(2).Take(4).GetInt32();
        //        var x = headInfos.Skip(6).Take(2).GetInt16();
        //        var y = headInfos.Skip(8).Take(2).GetInt16();
        //        var width = headInfos.Skip(10).Take(2).GetInt16();
        //        var RHeads = headInfos.Skip(12).Take(2 * height).ToArray();
        //        var columnCount = RHeads.TakeLast(2).GetInt16();
        //        var columnDescs = headInfos.Skip(12 + 2 * height).Take(columnCount * 4).ToArray();
        //        var image = GetPng2(width, height, x, y, (int)imageaddr, RHeads, columnDescs, dataBytes);
        //        bitmapList.Add(image);
        //    }
        //    return bitmapList;



        //}
    }
}
