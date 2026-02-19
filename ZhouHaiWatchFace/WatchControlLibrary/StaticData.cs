using MiniExcelLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchControlLibrary
{
    public static class StaticData
    {

        public static IEnumerable<Weather> Weathers => MiniExcel.Query<Weather>($@"{AppDomain.CurrentDomain.BaseDirectory}\Data\Weather.xlsx").OrderBy(x=>x.Id);


        public static readonly Dictionary<int, string> HeartZonesData = new()
        {
            {0, "Quiet" },
            {1, "Warm-up" },
            {2, "Fat Burn" },
            {3, "Aerobic" },
            {4, "Anaerobic" },
            {5, "Maximum" },
        };

        public static readonly Dictionary<int, string> PressLevelData = new()
        {
            {0, "Relaxed" },
            {1, "Normal" },
            {2, "Medium" },
            {3, "High" },
        };

      
        public static Dictionary<int, string> WeatherData => Weathers.GroupBy(x => x.GroupId).ToDictionary(x => x.Key, x => x.FirstOrDefault()?.Content_CN);







        public static readonly Dictionary<int, string> WindDirectionData = new()
        {
            {0 ,"No Persistent Wind"},
            {1 ,"Variable Wind"},
            {2 ,"North Wind"},
            {3 ,"Northeast Wind"},
            {4 ,"East Wind"},
            {5 ,"Southeast Wind"},
            {6 ,"South Wind"},
            {7 ,"Southwest Wind"},
            {8 ,"West Wind"},
            {9 ,"Northwest Wind"},
        };

        public static readonly Dictionary<int, string> WindLevelData = new()
        {
            {0 , "Light Breeze"},
            {1 , "Level 1"},
            {2 , "Level 2"},
            {3 , "Level 3"},
            {4 , "Level 4"},
            {5 , "Level 5"},
            {6 , "Level 6"},
            {7 , "Level 7"},
            {8 , "Level 8"},
            {9 , "Level 9"},
            {10 ,"Level 10"},
            {11 ,"Level 11"},
            {12 ,"Level 12 and above"},
        };

        public static readonly Dictionary<int, string> AQILevelData = new()
        {
            {0 , "Excellent"},
            {1 , "Good"},
            {2 , "Light Pollution"},
            {3 , "Moderate Pollution"},
            {4 , "Heavy Pollution"},
            {5 , "Severe Pollution"},
        };

        public static readonly Dictionary<int, string> DressLevelData = new()
        {
            {0 , "Suitable for T-shirt"},
            {1 , "Suitable for Shirt"},
            {2 , "Suitable for Light Jacket"},
            {3 , "Suitable for Jacket"},
            {4 , "Suitable for Windbreaker"},
            {5 , "Suitable for Cotton Coat"},
            {6 , "Suitable for Winter Coat"},
            {7 , "Suitable for Down Jacket"},
        };

        public static readonly Dictionary<int, string> TimeSpan = new()
        {
            {0 , "Late Night"},
            {1 , "Dawn"},
            {2 , "Early Morning"},
            {3 , "Morning"},
            {4 , "Noon"},
            {5 , "Afternoon"},
            {6 , "Evening"},
            {7 , "Dusk"},
            {8 , "Night"},
        };

        public static IDictionary<int, string> ToolKeyValuesFactory(string createName)
        {

            return createName switch
            {
                "Wind Direction" => WindDirectionData,
                "Heart Rate Zone" => HeartZonesData,
                "Weather" =>  WeatherData,
                "Wind Level" => WindLevelData,
                "Air Quality Index" => AQILevelData,
                "Dress Index" => DressLevelData,
                "Time Period" => TimeSpan,
                "Pressure Level" => PressLevelData,
                _ => throw new Exception("Unsupported type")
            }; ;
        }

    }

    public class Weather
    {
        public int Id { get; set; }

        public string? Content { get; set; }

        public string? Content2 { get; set; }

        public string? Content_CN { get; set; }

        public int GroupId { get; set; }
    }
}
