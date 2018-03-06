using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GitBasic
{
    public class CollectionEmptyToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {           
            return ((ICollection)value).Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("ConvertBack not supported in CollectionEmptyToVisibilityConverter.");
        }
    }
}
