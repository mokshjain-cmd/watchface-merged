using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using WatchControlLibrary;
using Path = System.IO.Path;

namespace WatchControlLibrary
{
    public class ImageCache
    {
        public static Dictionary<string, string> EzipImages = new();
        public static Dictionary<string, string> Images = new();
        public static void InitCache(string watchFolder)
        {
            EzipImages = Directory.GetFiles(CommonHelper.Ezip(watchFolder)).Where(x => Path.GetExtension(x) == ".png")
                  .ToDictionary(x => MD5Helper.GenerateFileMD5(x), x => x);
            Images = Directory.GetFiles(CommonHelper.CurrentPath(watchFolder)).Where(x => Path.GetExtension(x) == ".png")
                 .ToDictionary(x => MD5Helper.GenerateFileMD5(x), x => x);
        }


        public static string GetEzipImage(string watchFolder, string path)
        {
            Thread.Sleep(100);
            var md5 = MD5Helper.GenerateFileMD5(path);
            if (!EzipImages.ContainsKey(md5))
            {
                var fileName = DateTime.Now.ToString("yyMMddHHmmssfff");
                var savePath = $@"{CommonHelper.Ezip(watchFolder)}\{fileName}.png";
                File.Copy(path, $@"{CommonHelper.Ezip(watchFolder)}\{fileName}.png", true);
                EzipImages.Add(md5, savePath);
            }
            return GetRelativePath(AppDomain.CurrentDomain.BaseDirectory, EzipImages[md5]);
        }

        public static string GetImage(string watchFolder, string path)
        {
            Thread.Sleep(100);
            var md5 = MD5Helper.GenerateFileMD5(path);
            if (!Images.ContainsKey(md5))
            {
                var fileName = DateTime.Now.ToString("yyMMddHHmmssfff");
                var savePath = $@"{CommonHelper.CurrentPath(watchFolder)}\{fileName}.png";
                File.Copy(path, $@"{CommonHelper.CurrentPath(watchFolder)}\{fileName}.png", true);
                Images.Add(md5, savePath);
            }
            return GetRelativePath(AppDomain.CurrentDomain.BaseDirectory, Images[md5]);
        }

        private static string AppendDirectorySeparatorChar(string path)
        {
            if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                return path + Path.DirectorySeparatorChar;
            }
            return path;
        }
        public static string GetRelativePath(string basePath, string absolutePath)
        {
            // 获取完整的绝对路径
            Uri baseUri = new Uri(AppendDirectorySeparatorChar(basePath));
            Uri absoluteUri = new Uri(AppendDirectorySeparatorChar(absolutePath));

            // 如果基路径不是绝对路径，则抛出异常
            if (!baseUri.IsAbsoluteUri || !absoluteUri.IsAbsoluteUri)
            {
                throw new ArgumentException("The paths must be absolute.");
            }

            // 获取相对路径
            Uri relativeUri = baseUri.MakeRelativeUri(absoluteUri);
            string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            // 将 URI 格式的斜杠转换为 Windows 格式
            return "." + relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar).TrimEnd('\\');
        }



    }



}
