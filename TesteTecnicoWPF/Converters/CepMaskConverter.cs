using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace TesteTecnicoWPF.Converters
{
    public class CepMaskConverter : IValueConverter
    {
        // Converte do dado bruto (89010025) para a View (89.010-025)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var numbersOnly = value as string ?? "";
            if (numbersOnly.Length > 5)
            {
                return numbersOnly.Insert(5, "-").Insert(2, ".");
            }
            if (numbersOnly.Length > 2)
            {
                return numbersOnly.Insert(2, ".");
            }
            return numbersOnly;
        }

        // Converte da View (89.010-025) de volta para o dado bruto (89010025)
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Regex.Replace(value as string ?? "", @"[^\d]", "");
        }
    }
}