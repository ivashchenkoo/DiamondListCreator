using System;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace DiamondListCreator.Converters
{
    public class PathExistsToColorBrushConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Count() >= 2)
            {
                if (Directory.Exists(values[0].ToString()) || File.Exists(values[0].ToString()))
                {
                    if (System.Convert.ToBoolean(values[1]))
                    {
                        return new SolidColorBrush(Color.FromRgb(82, 154, 171));
                    }
                    else
                    {
                        return new SolidColorBrush(Color.FromRgb(0, 122, 152));
                    }
                }
                else
                {
                    if (System.Convert.ToBoolean(values[1]))
                    {
                        return new SolidColorBrush(Color.FromRgb(184, 17, 19));
                    }
                    else
                    {
                        return new SolidColorBrush(Colors.Red);
                    }
                }
            }
            else
            {
                return false;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
