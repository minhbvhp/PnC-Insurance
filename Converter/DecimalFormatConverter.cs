using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PnC_Insurance.Converter
{
    public class DecimalFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal)
            {
                if ((decimal)value != 0)
                    if (parameter != null)
                        return ((decimal)value).ToString(parameter.ToString(), culture);
                    else
                        return ((decimal)value).ToString();
                else
                    return 0.ToString(parameter.ToString(), culture);
            }

            return String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal ret;

            if (decimal.TryParse((string)value, out ret))
                return ret;

            return 0;
        }
    }
}
