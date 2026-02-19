using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WatchControlLibrary
{
    public class MD5Helper
    {
       public static string GenerateFileMD5(string filePath)
        {
            using (var md5 = MD5.Create())// 如果路径不是以目录分隔符结尾，则添加目录分隔符

            {
                using (var stream = File.OpenRead(filePath))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }


        public static string GetMD5Str(byte[] bytes) 
        {

            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(bytes);

                // 将字节数组转换为十六进制字符串
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2")); // 转换为两位的十六进制
                }
                return sb.ToString();
            }
           
        }


        public static string GenerateFileMD5str(string str) 
        {
            // 将输入字符串转换为字节数组
            byte[] inputBytes = Encoding.UTF8.GetBytes(str);
            // 计算哈希值
            return GetMD5Str(inputBytes);
           
        }
    }
}
