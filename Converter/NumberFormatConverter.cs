using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PnC_Insurance.Converter
{
    public class NumberFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter = null, CultureInfo culture = null)
        {
            if (value is int)
            {
                if ((int)value != 0)
                    if (parameter != null)
                        return ((int)value).ToString(parameter.ToString(), culture);
                    else
                        return ((int)value).ToString();
                else
                    return String.Empty;
            }
            else if (value is long)
            {
                if ((long)value != 0)
                    if (parameter != null)
                        return ((long)value).ToString(parameter.ToString(), culture);
                    else
                        return ((long)value).ToString();
                else
                    return String.Empty;
            }
            else
            {
                return String.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long ret;
            if (long.TryParse((string)value, NumberStyles.AllowThousands, culture, out ret))
            {
                return ret;
            }
            return 0;
        }
    }
}
