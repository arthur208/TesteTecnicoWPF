using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace TesteTecnicoWPF.Converters
{
    public class CollectionContainsToBooleanConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 || !(values[0] is IList collection) || values[1] == null)
            {
                return false;
            }
            // Retorna 'true' (marcado) se a coleção contém o item
            return collection.Contains(values[1]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            // O primeiro valor do MultiBinding é a nossa coleção de filtros.
            // O segundo é o item (o status string) atrelado a este checkbox.
            // Esta lógica nunca será chamada porque o binding de volta não é necessário para esta implementação.
            // A manipulação da coleção será feita de outra forma.
            // O XAML será responsável por adicionar/remover itens da coleção.
            // Isso mantém o conversor simples e focado em sua tarefa principal.

            // Retornamos valores que indicam que nenhuma ação deve ser tomada pelo conversor
            return new object[] { Binding.DoNothing, Binding.DoNothing };
        }
    }
}