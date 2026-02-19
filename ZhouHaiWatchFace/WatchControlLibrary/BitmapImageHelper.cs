using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WatchControlLibrary
{
    public  class BitmapImageHelper
    {
        /// <summary>
        /// 从 Uri 加载 BitmapImage 并确保资源不被锁定
        /// </summary>
        /// <param name="uri">图像的 Uri</param>
        /// <returns>加载后的 BitmapImage</returns>
        public static BitmapImage LoadFromUri(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            BitmapImage bitmapImage = new BitmapImage();

            // 使用 FileStream 加载图像
            using (var stream = new FileStream(uri.LocalPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad; // 确保图像完全加载到内存
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
                bitmapImage.Freeze(); // 冻结，防止多线程修改
            }
            return bitmapImage;
        }
    }
}
