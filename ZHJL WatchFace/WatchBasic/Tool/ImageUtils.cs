using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;

namespace WatchBasic.Tool
{
    public class ImageUtils
    {
        public static byte[]? BitmapToBtyes(string? path)
        {
            if (string.IsNullOrEmpty(path)) return null;
            Bitmap b = new Bitmap(path);
            ImageFormat format = null;
            if (Path.GetExtension(path).ToUpper() == "BMP")
            {
                format = ImageFormat.Bmp;
            }
            else
            {
                format = ImageFormat.Png;
            }
            MemoryStream ms = new MemoryStream();
            b.Save(ms, format);
            byte[] bytes = ms.GetBuffer();  
            ms.Close();
            ms.Dispose();
            b.Dispose();
            return bytes;
        }

        public static byte[]? BitmapToBtyes(Bitmap bitmap, ImageFormat format)
        {
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Png);
            byte[] bytes = ms.GetBuffer();  //byte[]   bytes=   ms.ToArray(); 这两句都可以，至于区别么，下面有解释
            ms.Close();
            return bytes;
        }

        public static Bitmap? BytesToBitmap(byte[]? Bytes)
        {
            if (Bytes == null) return null;
            MemoryStream? stream = null;
            try
            {
                stream = new MemoryStream(Bytes);
                return new Bitmap((Image)new Bitmap(stream));
            }
            catch (ArgumentNullException ex)
            {
                throw ex;
            }
            catch (ArgumentException ex)
            {
                throw ex;
            }
            finally
            {
                stream?.Close();
            }
        }


        public static Bitmap RotateImgCV(Image<Bgra, Byte> image, float angle)
        {

            return CVTool.ImgRotate(image, angle, 1);

        }

        public static Bitmap RotateImg3(Bitmap img, float angle)
        {
            //通过Png图片设置图片透明，修改旋转图片变黑问题。
            int width = img.Width;
            int height = img.Height;
            //角度
            Matrix mtrx = new Matrix();
            mtrx.RotateAt(angle, new PointF((width / 2), (height / 2)), MatrixOrder.Append);
            //得到旋转后的矩形
            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(new RectangleF(0f, 0f, width, height));
            RectangleF rct = path.GetBounds(mtrx);
            //生成目标位图
            Bitmap devImage = new Bitmap((int)(rct.Width), (int)(rct.Height));
            Graphics g = Graphics.FromImage(devImage);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //计算偏移量
            Point Offset = new Point((int)(rct.Width - width) / 2, (int)(rct.Height - height) / 2);
            //构造图像显示区域：让图像的中心与窗口的中心点一致
            Rectangle rect = new Rectangle(Offset.X, Offset.Y, (int)width, (int)height);
            Point center = new Point((int)(rect.X + rect.Width / 2), (int)(rect.Y + rect.Height / 2));
            g.TranslateTransform(center.X, center.Y);
            g.RotateTransform(angle);
            //恢复图像在水平和垂直方向的平移
            g.TranslateTransform(-center.X, -center.Y);
            g.DrawImage(img, rect);
            //重至绘图的所有变换
            g.ResetTransform();
            g.Save();
            g.Dispose();
            path.Dispose();
            return devImage;
        }

        public static void imageTest(Bitmap srcImage, int width, int height, int angle)
        {

            Rectangle pointRect = new Rectangle((srcImage.Width / 2) - width, (srcImage.Height / 2) - height, width, height);
            Bitmap pointBmp = srcImage.Clone(pointRect, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Bitmap b = RotateImg3(pointBmp, angle);
            Bitmap image1 = new Bitmap(srcImage.Width, srcImage.Height);
            Graphics graphics = Graphics.FromImage(image1);
            graphics.DrawImage(image1, 0, 0, image1.Width, image1.Height);

            if (b.Height >= height)
            {
                Rectangle rectangle = new Rectangle((b.Width / 2) - height, (b.Height / 2) - height, height * 2, height * 2);
                //Rectangle rectangle = new Rectangle(0, 0, 240, 240);
                Bitmap bmp = b.Clone(rectangle, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                graphics.DrawImage(bmp, 0, 20, bmp.Width, bmp.Height);
                image1.Save($"{angle}.png");

                Thread.Sleep(200);
            }


        }


        /// <summary>
        /// 第二种方法
        /// </summary>
        /// <param name="b"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public Bitmap RotateImg2(Bitmap b, decimal centerX, decimal centerY, float angle)
        {
            //Graphics graphics = this.CreateGraphics();
            return null;
        }



        /// <summary>
        /// 旋转图像
        /// </summary>
        /// <param name="b"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Bitmap RotateImage(Bitmap b, float angle)
        {
            //create a new empty bitmap to hold rotated image
            Bitmap returnBitmap = new Bitmap(b.Width, b.Height);
            //make a graphics object from the empty bitmap
            Graphics g = Graphics.FromImage(returnBitmap);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            //move rotation point to center of image
            g.TranslateTransform((float)b.Width / 2, (float)b.Height / 2);

            //rotate
            g.RotateTransform(angle);
            //move image back
            g.TranslateTransform(-(float)b.Width / 2, -(float)b.Height / 2);
            //draw passed in image onto graphics object
            g.DrawImage(b, new System.Drawing.Point(0, 0));


            return returnBitmap;
        }


        //public static byte[] GetBytesRGB565(Color color)
        //{
        //    var res = RGB888ToRGB565(color.R, color.G, color.B);
        //    return res.GetBytes16();

        //}

        public static int GetColor888(Color color) 
        {
            return  (color.B << 16) + (color.G << 8) + color.R;
           // return (color.B << 16) | (color.G << 8) | color.R;
        }

        /// <summary>
        /// 安卓端的解法正向(只参考)
        /// </summary>
        /// <param name="bmp_r"></param>
        /// <param name="bmp_g"></param>
        /// <param name="bmp_b"></param>
        /// <returns></returns>
        public static byte[] RGB24TORGB16Forward(byte bmp_r, byte bmp_g, byte bmp_b)
        {
            byte result_r = (byte)(bmp_r >> 3 & 0x1f);
            byte result_g = (byte)(bmp_g >> 2 & 0x3f);
            byte result_b = (byte)(bmp_b >> 3 & 0x1f);
            byte[] result = new byte[2];
            result[0] = (byte)((result_r << 3 & 0xf8) | (result_g >> 3 & 0x1f));
            result[1] = (byte)((result_g << 5 & 0xe0) | result_b);
            return result;
        }

        public static int RGB888ToRGB565(int r, int g, int b)
        {
            r = (int)(r / 8);
            g = (int)(g / 4);
            return (r << 11) + (g << 5) + (b / 8);
        }


        public static byte[] GetPngBytesOnlyA(List<Color> colors)
        {
            List<byte> bytes = new List<byte>();
            foreach (var c in colors)
            {
                bytes.Add(c.A);
            }
            return bytes.ToArray();
        }
        
        public static byte[] GetPngBytes(List<Color> colors, bool pointer)
        {
            List<byte> bytes = new List<byte>();
            foreach (var c in colors)
            {
                var R = c.R;
                var G = c.G;
                var B = c.B;
                var A = c.A;
                if (CommonDefintion.IsColor888) 
                {
                    var colorVal = GetColor888 (c);
                    var bits = colorVal.GetBytes(3).ToList();
                    bits.Add(c.A);
                    bytes.AddRange(bits);
                }
                else 
                {
                    var colorVal = RGB888ToRGB565(R, G, B);
                    var bits = colorVal.GetBytes16().ToList();
                    bits.Add(c.A);
                    bytes.AddRange(bits);
                }
               
            }
            return bytes.ToArray();
        }

        public static Color GetColor(int colorValue)
        {
            var colorR = (colorValue >> 8) & 0x0f8; //转换最高5位成8位红色的高5位
            var colorG = (colorValue >> 3) & 0x0fc; //转换中间6位成8位绿色的高6位
            var colorB = (colorValue << 3) & 0x0f8; //转换低5位成8位蓝色的高5位
            return Color.FromArgb(colorR, colorG, colorB);
        }


        public static string GetMainColor(Bitmap bitmap)
        {
            List<Color> colors = new List<Color>();
            for (var i = 0; i < bitmap.Height; i++)
            {
                for (var j = 0; j < bitmap.Width; j++)
                {
                    var color = bitmap.GetPixel(j, i);
                    if (color.A > 12)
                    {
                        colors.Add(color);
                    }
                }
            }
            var max = colors.Select(i => new
            {
                color = i,
                key = i.ToArgb()
            }).GroupBy(i => i.key).OrderByDescending(i => i.Count()).FirstOrDefault();
            if (max != null)
                return ColorTranslator.ToHtml(max.FirstOrDefault().color);
            return String.Empty;

        }

        public static Bitmap LayerColored(Bitmap bitmap, Color? color)
        {
            for (var i = 0; i < bitmap.Height; i++)
            {
                for (var j = 0; j < bitmap.Width; j++)
                {
                    var color1 = bitmap.GetPixel(j, i);
                    if (color1.A > 0)
                    {
                        if (color1.A == 255)
                        {
                            bitmap.SetPixel(j, i, (Color)color);
                        }
                        else
                        {
                            //var A = (int)color1.A;
                            //var R = (int)color?.R;
                            //var B = (int)color?.B;
                            //var G = (int)color?.G;
                            //R = (A * R + (255 - A) * 255) / 255;
                            //G = (A * G + (255 - A) * 255) / 255;
                            //B = (A * B + (255 - A) * 255) / 255;

                            // color =Color.FromArgb(R,G,B);

                            bitmap.SetPixel(j, i, Color.FromArgb(color1.A, (int)color?.R, (int)color?.G, (int)color?.B));
                        }
                    }
                }
            }

            return bitmap;
        }






    }
}
