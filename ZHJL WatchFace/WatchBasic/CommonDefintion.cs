using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WatchBasic.UIBasic;
using WatchDB;

namespace WatchBasic
{
    public class CommonDefintion
    {
        public static WatchSetting? Setting { get; set; }
        public static bool IsColor888 { get; set; }

        public static int IsHeart { get; set; }
        public static string GetWatchId(WatchInfo watchInfo, string lan)
        {
            //1.1位 = 0(版本号，固定为0)
            //2.1位 = 0(形状，shape_items[]下标)
            //3.2位 = 20(分辨率，resolving_power_items[]下标)
            //4.1位 = 0(表盘类型, clock_type_items[]下标)
            //5.3位 = 203(系列，series_num_items[]下标)
            //6.2位 = 02(作者，author_items[]转author_str[])
            //7.3位 = 001(主编号)
            //8.2位 = 00(子编号)
            //9.2位 = 00(表盘风格，固定为00)
            //10.1位 = 0(数据格式，输出一个新的格式，固定为4吧)
            //11.2位 = 00(语言类型，英文 = 0，中文 = 1)
            var index = Authors.Select((i, index) => new { i = i, index = index }).Where(i => i.i.Key == watchInfo.Author).First();
            var lancode = lan == "英文" ? 0 : 1;
            return $"0{GetShape(watchInfo.Shape)}{GetDPI(watchInfo.Dpi).ToString("00")}" +
                $"{GetWatchType(watchInfo.Type)}{watchInfo.Series}{index.index.ToString("00")}{watchInfo.Code}{watchInfo.SubCode}004{lancode.ToString("00")}";

        }

        public static string GetParentWatchId(WatchInfo watchInfo, string lan)
        {
            //1.1位 = 0(版本号，固定为0)
            //2.1位 = 0(形状，shape_items[]下标)
            //3.2位 = 20(分辨率，resolving_power_items[]下标)
            //4.1位 = 0(表盘类型, clock_type_items[]下标)
            //5.3位 = 203(系列，series_num_items[]下标)
            //6.2位 = 02(作者，author_items[]转author_str[])
            //7.3位 = 001(主编号)
            //8.2位 = 00(子编号)
            //9.2位 = 00(表盘风格，固定为00)
            //10.1位 = 0(数据格式，输出一个新的格式，固定为4吧)
            //11.2位 = 00(语言类型，英文 = 0，中文 = 1)
            var index = Authors.Select((i, index) => new { i = i, index = index }).Where(i => i.i.Key == watchInfo.Author).First();
            var lancode = lan == "英文" ? 0 : 1;
            return $"0{GetShape(watchInfo.Shape)}{GetDPI(watchInfo.Dpi).ToString("00")}" +
                $"{GetWatchType(watchInfo.Type)}{watchInfo.Series}{index.index.ToString("00")}{watchInfo.Code}00004{lancode.ToString("00")}";

        }


        public static List<string> WriteWatchInfo(WatchInfo watchInfo, WatchDesc desc, int size)
        {
            var watchType = watchInfo.Type == "普通" ? "0" : "1";
            List<string> strings = new List<string>();
            var code = watchInfo.SubCode == "00" ? "" : GetParentWatchId(watchInfo, desc.Language);
            strings.Add($"# 作品相关==========");
            strings.Add($"clock_dial_type={watchType}");
            strings.Add($"clock_dial_data_format=1");
            strings.Add($"clock_dial_data_generation_mode=1");
            strings.Add($"works_number={GetWatchId(watchInfo, desc.Language)}");
            strings.Add($"author_name={Authors[watchInfo.Author]}");
            strings.Add($"works_name={desc.WatchName.Split(':')[1]}");
            strings.Add($"works_describe={desc.Description.Split(':')[1]}");
            strings.Add($"#设备相关==========");
            strings.Add($"device_width={CommonDefintion.Setting.Width}");
            strings.Add($"device_height={CommonDefintion.Setting.Height}");
            strings.Add($"device_shape={GetShape(watchInfo.Shape)}");
            strings.Add($"device_lan_number=2");
            strings.Add($"device_is_heart={IsHeart}");
            strings.Add("zhouhai_device_is_pointer=0");
            strings.Add($"bin_size={size}");
            strings.Add($"product_no=0");
            strings.Add($"product_version=1");
            strings.Add($"#表盘关联==========");
            strings.Add($"father_clockdial_id={code}");
            return strings;


        }


        static int GetWatchType(string type)
        {
            return type == "普通" ? 0 : 1;
        }

        static int GetShape(string Shape)
        {
            if (Shape_Items.ContainsKey(Shape))
            {
                return Shape_Items[Shape];
            }
            throw new Exception("未知形状");
        }
        static int GetDPI(string DPI)
        {
            if (Resolving_power_items.ContainsKey(DPI))
            {
                return Resolving_power_items[DPI];
            }
            throw new Exception("未知分辨率");
        }
        public static Dictionary<string, int> Shape_Items => new Dictionary<string, int>
        {
            {"方形",0 },
            {"球拍",1 },
            {"圆形",2 },
            {"圆角矩形1",3 },

        };
        public static Dictionary<string, int> Resolving_power_items => new Dictionary<string, int>
        {
           {"128x220",0},
           {"135x240",1},
           {"240x240",2},
           {"320x320",3},
           {"360x360",4},
           {"320x385",5},
           {"454x454",6},
           {"240x198",7},
           {"240x204",8},
           {"240x210",9},
           {"360x400",10},
           {"368x448",11},
           {"240x280",12},
           {"356x400",13},
           {"466x466",14},
           {"172x320",15},
           {"400x454",16},
           {"320x380",17},
           {"410x502",18},
           {"228x460",19},
           {"390x450",20},
           {"240x296",20},


        };

        public static String[] series_num_items = {
                    "101",//0
                    "201",//1
                    "202",//2
                    "203",//3
                    "204",//4
                    "205",//5
                    "206",//6
                    "301",//7
                    "302",//8
                    "303",//9
                    "304",//10
                    "305",//11
                    "306",//12
                    "307",//13
                    "308",//14
                    "309",//15
                    "401",//16
                    "501",//16
                    "601"//17
         };

        public static bool IsJieLi { get; set; } = false;

//        public static String[] author_str = 
//            {"Jwei", "WuY", "LiJ", "LianYS", "HHY", " SYF", "JiangZC", " LHF", "GSS"
//        , "PHC"
//        , "N1"
//        , "ZHXL"
//        , "CGL"
//        , "YJJ"
//        , "ZW"
//        , "HZM"
//        , "PH"
//        , "WCC"
//        , "Designer01"
//        , "Designer02"
//        , "Designer03"
//        , "Designer04"
//        , "Designer05"
//        , "Designer06"
//        , "Designer07"
//        , "Designer08"
//        , "Designer09"
//        , "Designer10"
//};
//        public static String[] author_items =
//            {"JW", "伍洋", "李剑", "梁韵诗", "黄辉洋", "宋芋飞", "江梓城", "李慧芳", "郭莎莎"
//        , "彭浩冲"
//        , "N1"
//        , "朱兴林"
//        , "陈关磊"
//        , "余俊杰"
//        , "赵唯"
//        , "黄梓铭"
//        , "潘恒"
//        , "Arien"
//        , "Brian"
//        , "Chris"
//        , "Danie"
//        , "Emerson"
//        , "Harper"
//        , "Jean"
//        , "Victoria"
//        , "Xavier"
//        , "Sarah"
//};
        public static Dictionary<string, string> Authors => new Dictionary<string, string>
        {
            {"JW", "Jwei"},
            {"伍洋", "WuY"},
            {"李剑", "LiJ"},
            {"梁韵诗", "LianYS"},
            {"黄辉洋", "HHY"},
            {"宋芋飞", "SYF"},
            {"江梓城", "JiangZC"},
            {"李慧芳", "LHF"},
            {"郭莎莎", "GSS"},
            {"彭浩冲", "PHC"},
            {"N1", "N1"},
            {"朱兴林", "ZHXL"},
            {"陈关磊", "CGL"},
            {"余俊杰", "YJJ"},
            {"赵唯", "ZW"},
            {"黄梓铭", "HZM"},
            {"潘恒", "PH"},
            { "王晨晨","WCC"},
            {"Arien","Designer01"},
            {"Brian","Designer02"},
            {"Chris","Designer03"},
            { "Daniel", "Designer04" },
            { "Emerson", "Designer05" },
            { "Harper", "Designer06" },
            { "Jean", "Designer07" },
            { "Victoria", "Designer08" },
            { "Xavier", "Designer09" },
            { "Sarah", "Designer10" },
        };





    }
}
