using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Control = System.Windows.Controls.Control;
using Image = System.Windows.Controls.Image;

namespace WatchControlLibrary
{
   

    public class DragDateTimeHelper
    {
        public static string iconStr => "&";
        public static string monthStr => "@";
        public static string dayStr => "#";
        static string GetLeadingZeroStr(bool leadingZero, int num)
        {
            return leadingZero ? $"{num.ToString("00")}" : num.ToString();
        }
        public static string GetDateTime(string dataTimeFormat, bool leadingZero, bool hasIcon, DateTime dateTime)
        {

            return dataTimeFormat switch
            {
                "Year" => $"{dateTime.ToString("yyyy")}{(hasIcon ? iconStr : "")}",
                "Month" => $"{GetLeadingZeroStr(leadingZero, dateTime.Month)}{(hasIcon ? iconStr : "")}",
                "Day" => $"{GetLeadingZeroStr(leadingZero, dateTime.Day)}{(hasIcon ? iconStr : "")}",
                "Hour" => $"{GetLeadingZeroStr(leadingZero, dateTime.Hour)}{(hasIcon ? iconStr : "")}",
                "Minute" => $"{GetLeadingZeroStr(leadingZero, dateTime.Minute)}{(hasIcon ? iconStr : "")}",
                "Second" => $"{GetLeadingZeroStr(leadingZero, dateTime.Second)}{(hasIcon ? iconStr : "")}",
                "Time" => $"{GetLeadingZeroStr(leadingZero, dateTime.Hour)}{(hasIcon ? iconStr : "")}{dateTime.ToString("mm")}",
                _ => throw new NotImplementedException()
            };
        }

        public static string GetMothDay(bool leadingZero, MonthDayModeEnum showMonthType,bool hasIcon,bool hasMothIcon,bool hasDayIcon, DateTime dateTime)
        {
            if (showMonthType == MonthDayModeEnum.unit)
            {
                return $"{GetLeadingZeroStr(leadingZero, dateTime.Month)}{(hasMothIcon ? monthStr : "")}{GetLeadingZeroStr(leadingZero, dateTime.Day)}{(hasMothIcon ? dayStr : "")}";
            }
            else if (showMonthType == MonthDayModeEnum.split)
            {
                return $"{GetLeadingZeroStr(leadingZero, dateTime.Month)}{(hasIcon ? iconStr : "")}{GetLeadingZeroStr(leadingZero, dateTime.Day)}";
            }
            return $"{GetLeadingZeroStr(leadingZero, dateTime.Month)}{GetLeadingZeroStr(leadingZero, dateTime.Day)}";
        }

        

    }



}
