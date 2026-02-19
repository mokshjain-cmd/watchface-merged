using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WatchBasic.Tool
{
    public static class MD5Tool
    {
        public static string MD5Stream(Stream stream)
        {
            MD5 md5 = MD5.Create();
            md5.ComputeHash(stream);
            byte[] b = md5.Hash;
            md5.Clear();
            StringBuilder sb = new StringBuilder(32);
            for (int i = 0; i < b.Length; i++)
            {
                sb.Append(b[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public static string MD5Stream(string filePath)
        {
            using (FileStream stream = File.Open(filePath, FileMode.Open))
            {
                return MD5Stream(stream);
            }
        }

        public static string md5(string str)
        {
            //将字符串编码为字节序列
            byte[] bt = Encoding.UTF8.GetBytes(str);
            //创建默认实现的实例
            var md5 = System.Security.Cryptography.MD5.Create();
            //计算指定字节数组的哈希值。
            var md5bt = md5.ComputeHash(bt);
            //将byte数组转换为字符串
            StringBuilder builder = new StringBuilder();
            foreach (var item in md5bt)
            {
                builder.Append(item.ToString("X2"));
            }
            string md5Str = builder.ToString();
            return builder.ToString();
        }

    }
}
