using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WatchBasic.Tool
{
    public static class BitmapSourceConvert

    {

        /// <summary>

        /// Delete a GDI object

        /// </summary>

        /// <param name="o">The poniter to the GDI object to be deleted</param>

        /// <returns></returns>

        [DllImport("gdi32")]

        private static extern int DeleteObject(IntPtr o);

        /// <summary>

        /// Convert an Bitmap to a WPF BitmapSource. The result can be used in the Set Property of Image.Source

        /// </summary>

        /// <param name="image">Bitmap</param>

        /// <returns>The equivalent BitmapSource</returns>

        public static BitmapSource ToBitmapSource(System.Drawing.Bitmap? bitmap)
        {
            var hBitmap = bitmap.GetHbitmap();
            var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                System.Windows.Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            return bitmapSource;
        }

        public static BitmapImage ToBitmapImage(System.Drawing.Bitmap? bitmap) 
        {
            BitmapImage bitmapImage = new BitmapImage();
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);
                stream.Position = 0;
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }

        public static Bitmap BitmapImageToBitmap(BitmapSource bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

    }
}
