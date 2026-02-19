using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchBasic;
using WatchBasic.Language;
using WatchBasic.Tool;

namespace WatchUI.CreateBin
{
    public class WatchTool
    {

        public static string GetWatchId(LanguageFactory factory,string watchlang, string desc)
        {
            var descs = desc.Split('_');
            var lang = factory.GetIdCode(watchlang);
            //00201101020010000401
            var str = $@"0{Shape(descs[0])}{DPI(descs[1])}{Type(descs[2])}{descs[3].Substring(0, 3)}{Author(descs[4])}{descs[5]}{descs[6].Substring(0, 2)}002{lang}";
            return str;
        }

        //public static string GetWatchDiyId(string desc,string wath)
        //{
        //    var descs = desc.Split('_');
           
        //    var str = $@"0{Shape(descs[0])}{DPI(descs[1])}{Type(descs[2])}{descs[3].Substring(0, 3)}{Author(descs[4])}{descs[5]}{descs[6].Substring(0,2)}00200";
        //    return str;
        //}


        static string Shape(string desc)
        {
            return desc == "方形" ? "0" : "1";
        }

        static string DPI(string desc)
        {
            var num1 = Convert.ToInt16(desc.Substring(0, 3));
            var num2 = Convert.ToInt16(new string(desc.ToArray().TakeLast(3).ToArray()));
            return (num1 * num2).ToString().Substring(0, 2);
        }
        static string Type(string desc)
        {
            return desc == "普通" ? "0" : "1"; //1为相册
        }
        static string Author(string desc)
        {
            if (desc == "M65A")
            {
                return "01";
            }
            return "00";
        }

        public static List<WatchDesc>? GetWatchDesc(string path)
        {
            List<WatchDesc> watchDescs = new List<WatchDesc>();
            var txtPath = $@"{path}/辅助文件/表盘信息.txt";
            if (File.Exists(txtPath))
            {
                var strs = File.ReadAllLines(txtPath).Where(i => i != "").Select((i, index) => new
                {
                    index = index,
                    str = i,
                });
                
                var heads = strs.Where(i => i.str.StartsWith("#====="));
                watchDescs?.Clear();
                foreach (var head in heads)
                {
                    if (head.str == "#=====表盘分类=====") { break; }
                    var infos = strs.Skip(head.index + 1).Take(4).ToList();
                    watchDescs?.Add(new WatchDesc
                    {
                        Language = head.str.Replace("#", "").Replace("=", ""),
                        Description = infos[3].str,
                        WatchName = infos[1].str,
                        WatchCategory = strs.LastOrDefault()?.str.Split(':').LastOrDefault(),

                    });

                }
            }
            return watchDescs;
        }

       

    }

   

}
