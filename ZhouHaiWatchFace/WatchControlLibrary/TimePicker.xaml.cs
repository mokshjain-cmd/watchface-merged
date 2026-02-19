using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WatchControlLibrary
{
    /// <summary>
    /// Interaction logic for TimePicker.xaml
    /// </summary>
    public partial class TimePicker : System.Windows.Controls.UserControl
    {
        public TimePicker()
        {
            InitializeComponent();
        }



        public DateTime DateTime
        {
            get { return (DateTime)GetValue(DateTimeProperty); }
            set { SetValue(DateTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DateTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DateTimeProperty =
            DependencyProperty.Register("DateTime", typeof(DateTime), typeof(TimePicker), new PropertyMetadata(DateTime.Now, GetTimeIndex));

        private static void GetTimeIndex(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var timePicker = (TimePicker)d;
            var value = (DateTime)e.NewValue;
            timePicker.DateTimeIndex = value.Hour switch
            {
                >= 0 and < 5 => 0,
                >= 5 and < 7 => 1,
                >= 7 and < 9 => 2,
                >= 9 and < 11 => 3,
                >= 11 and < 13 => 4,
                >= 13 and < 17 => 5,
                >= 17 and < 18 => 6,
                >= 18 and < 20 => 7,
                >= 20 => 8,
                _ => throw new Exception("Unknown time error")
            };
            foreach (var item in timePicker.TimeIndex.Children)
            {
                if (item is TextBlock textBlock)
                {
                    if (short.Parse(textBlock.Tag.ToString()!) == timePicker.DateTimeIndex)
                    {
                        textBlock.Background = System.Windows.Media.Brushes.Gray;
                    }
                    else
                    {
                        textBlock.Background = System.Windows.Media.Brushes.Transparent;
                    }
                }
            }
        }

        public short DateTimeIndex
        {
            get { return (short)GetValue(DateTimeIndexProperty); }
            set { SetValue(DateTimeIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DateTimeIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DateTimeIndexProperty =
            DependencyProperty.Register("DateTimeIndex", typeof(short), typeof(TimePicker), new PropertyMetadata(default(short))); 



    }

    public class StringDateToDoubleConverter : IValueConverter
    {
        private DateTime _date;
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            _date = (DateTime)value;
            return (string)parameter switch
            {
                "year" => _date.Year,
                "month" => _date.Month,
                "day" => _date.Day,
                "hour" => _date.Hour,
                "minute" => _date.Minute,
                "second" => _date.Second,
                _ => (_date.Hour * 60 + _date.Minute) * 60 + _date.Second
            };

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int curvalue = (int)(double)value;
            return (string)parameter switch
            {
                "year" => _date.AddYears(curvalue - _date.Year),
                "month" => _date.AddMonths(curvalue - _date.Month),
                "day" => _date.AddDays(curvalue - _date.Day),
                "hour" => _date.AddHours(curvalue - _date.Hour),
                "minute" => _date.AddMinutes(curvalue - _date.Minute),
                "second" => _date.AddSeconds(curvalue - _date.Second),
                _ => _date.Date.AddSeconds(curvalue)
            };
        }
    }
    public class StringDateConverter : IValueConverter
    {
        private static readonly ChineseLunisolarCalendar chineseDate = new();
        private DateTime _date;
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            _date = (DateTime)value;
            return (string)parameter switch
            {
                //"chineseYear" => chineseDate.GetYear(_date),
                //"chineseMonth" => chineseDate.GetMonth(_date),
                //"chineseDay" => chineseDate.GetDayOfMonth(_date),
                "year" => ((DateTime)value).Year,
                "month" => ((DateTime)value).Month.ToString("00"),
                "day" => ((DateTime)value).Day.ToString("00"),
                "hour" => ((DateTime)value).Hour.ToString("00"),
                "minute" => ((DateTime)value).Minute.ToString("00"),
                "second" => ((DateTime)value).Second.ToString("00"),
                _ => ((DateTime)value).ToString("yyyy/MM/dd HH:mm:ss"),
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (string)parameter switch
            {
                "year" => _date.AddYears(int.Parse((string)value) - _date.Year),
                "month" => _date.AddMonths(int.Parse((string)value) - _date.Month),
                "day" => _date.AddDays(int.Parse((string)value) - _date.Day),
                "hour" => _date.AddHours(int.Parse((string)value) - _date.Hour),
                "minute" => _date.AddMinutes(int.Parse((string)value) - _date.Minute),
                "second" => _date.AddSeconds(int.Parse((string)value) - _date.Second),
                _ => DateTime.TryParse((string)value, out DateTime result) ? result : _date
            };
        }
    }
}
