using Mapster.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchControlLibrary.Model
{
    public partial class DataItemTypeHelper  // Nums
    {

        public static Dictionary<string, List<DataItemType>> DataItemTypes2 = new Dictionary<string, List<DataItemType>>
        {
            {"Diastolic Pressure", new List<DataItemType>{ DataItemType.healthBloodDiastolicPressureMmhg,DataItemType.healthBloodDiastolicPressureKpa } },
            {"Systolic Pressure", new List<DataItemType>{ DataItemType.healthBloodSystolicPressureMmhg, DataItemType.healthBloodSystolicPressureKpa } },
        };

        internal static int GetTotalDigits(string itemName)
        {
            return itemName switch
            {
                "Steps" => 5,
                "Heart Rate" => 3,
                "Calories" => 5, // 4 -> 5
                "Battery" => 3,
                "Step Distance" => 5, // 3->5
                "Stand Count" => 2,
                "Exercise Duration" => 3,
                "Heart Rate Min" => 3,
                "Heart Rate Max" => 3,
                "Blood Oxygen" => 3,
                "Pressure Index" => 3,  // 2->3
                "Recovery Time" => 2,
                "7-Day Vitality" => 3,
                "Wind Speed" => 3,
                "Wind Direction Angle" => 3,
                "AQI" => 3,
                "Visibility" => 3,
                "UV Index" => 2,
                "Temperature" => 3,
                "Today Max Temperature" => 3,
                "Today Min Temperature" => 3,
                "Feels Like Temperature" => 3,
                "Humidity" => 3,
                "Running Power Index" => 2,
                "Rain Probability" => 3,
                "Sea Level Pressure" => 4,
                "Diastolic Pressure" => 3,
                "Systolic Pressure" => 3,
                "Sleep Duration" => 4,
                "Blood Sugar" => 4,
                "Year" => 4,
                "Month" or "Day" or "Hour" or "Minute" or "Second" => 2,
                _ => 0
            };
        }
        internal static List<Param>? GetParams(string itemName)
        {

            return itemName switch
            {
                var s when s.Contains("Month") => Enumerable.Range(1, 12).Select(x => new Param { Value = x, }).ToList(),
                var s when s.Contains("Day") => Enumerable.Range(1, 31).Select(x => new Param { Value = x, }).ToList(),
                //var s when s.Contains("Hour") => Enumerable.Range(0, 24).Select(x => new Param { Value = x, }).ToList(),
                //"Week" => Enumerable.Range(0, 7).Select(x => new Param { Value = x, }).ToList(),
                _ => null
            };
        }
    }
}
