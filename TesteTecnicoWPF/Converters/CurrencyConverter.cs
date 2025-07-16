using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace TesteTecnicoWPF.Converters
{
    public class CurrencyConverter : IValueConverter
    {
        // Este método converte o dado do ViewModel (decimal) para a View (string formatada)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal valor)
            {
                // Formata o número com duas casas decimais usando a cultura pt-BR
                return valor.ToString("N2", new CultureInfo("pt-BR"));
            }
            return "0,00";
        }

        // Este método converte o que o usuário digita na View (string) de volta para o ViewModel (decimal)
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                // 1. Remove tudo que não for número
                string numbersOnly = Regex.Replace(str, @"[^\d]", "");

                if (string.IsNullOrEmpty(numbersOnly))
                {
                    return 0m;
                }

                // 2. Converte a string de números para decimal
                if (decimal.TryParse(numbersOnly, out decimal parsedValue))
                {
                    // 3. Divide por 100 para tratar os dois últimos dígitos como centavos
                    return parsedValue / 100.0m;
                }
            }
            return 0m;
        }
    }
}