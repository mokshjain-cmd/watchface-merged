using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using WatchControlLibrary.Model;

namespace WatchControlLibrary.Convert
{
  
    public class EqualityConverter : IValueConverter
    {
        public static readonly EqualityConverter Instance = new EqualityConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.Equals(parameter) ?? false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.Equals(true) == true ? parameter : System.Windows.Data.Binding.DoNothing;
        }
    }

   

    public class AbsoutelySingleConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not "" && value is string source)
            {
                var path= CommonHelper.AbsolutePath(source);

                try
                {
                    BitmapImage bitmapImage = new BitmapImage();

                    using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad; // Fully load image to memory
                        bitmapImage.StreamSource = stream;
                        bitmapImage.EndInit();
                        bitmapImage.Freeze(); // Freeze to ensure image resource is not modified
                    }

                    return bitmapImage;
                }
                catch
                {
                    return null; // Handle load failure here
                }
            }
            //return string.Empty;
            return CommonHelper.AddBtnBackgroundPath;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ItemNameConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((string)value) switch
            {
                string name when  name.Contains("Steps") => "Steps",
                //string name when  name.Contains("Heart Rate") => "Heart Rate",
                string name when  name.Contains("Calories") => "Calories",
                //string name when  name.Contains("Battery") => "Battery",
                //string name when name.Contains("Blood Oxygen") => "Blood Oxygen",
                //string name when name.Contains("Pressure") => "Pressure",
                string name when name.Contains("Exercise") => "Exercise",
                _ => "Other"
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

  
}
