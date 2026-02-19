using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchControlLibrary.Model;

namespace WatchControlLibrary
{
    public class OutXmlHelper
    {

        public static IList<string> AllNames { get; set; } = new List<string>();

       

        public static string GetWatchElementName()
        {
            var name = GenerateRandomData(6);
            while (AllNames.Contains(name))
            {
                name = GenerateRandomData(6);
            }
            AllNames.Add(name);
            return name;
        }

        public static string GetWatchElementNameByPx(string px)
        {
            var last = (AllNames.Where(x => x.StartsWith(px))?.Select(x => System.Convert.ToInt32(x.Replace(px, ""))))?.OrderByDescending(x=>x).FirstOrDefault() ?? 0;
            var name = $"{px}{last + 1}";
            AllNames.Add(name);
            return name;
        }


        public static string GenerateRandomData(int length = 6)
        {
            // 初始化随机数生成器
            Random random = new Random();

            // 定义可能的字符
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            // 生成随机数据字符串
            string randomData = new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)])
                .ToArray());

            return randomData;
        }

    }


}
