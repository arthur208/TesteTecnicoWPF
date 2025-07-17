using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TesteTecnicoWPF.Converters
{
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Se o valor for true, retorna Collapsed (escondido). Se for false, retorna Visible.
            return (value is bool b && b) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}