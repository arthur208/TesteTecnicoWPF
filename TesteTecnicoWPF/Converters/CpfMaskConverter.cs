using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace TesteTecnicoWPF.Converters
{
    public class CpfMaskConverter : IValueConverter
    {
        // Converte do dado bruto (11122233344) para a View (111.222.333-44)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var numbersOnly = value as string ?? "";
            if (numbersOnly.Length > 9)
            {
                return numbersOnly.Insert(9, "-").Insert(6, ".").Insert(3, ".");
            }
            if (numbersOnly.Length > 6)
            {
                return numbersOnly.Insert(6, ".").Insert(3, ".");
            }
            if (numbersOnly.Length > 3)
            {
                return numbersOnly.Insert(3, ".");
            }
            return numbersOnly;
        }

        // Converte da View (111.222.333-44) de volta para o dado bruto (11122233344)
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Regex.Replace(value as string ?? "", @"[^\d]", "");
        }
    }
}