using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TesteTecnicoWPF.Behaviors
{
    public static class CurrencyMaskBehavior
    {
        // Propriedade anexada que ativa o comportamento
        public static readonly DependencyProperty IsAttachedProperty =
            DependencyProperty.RegisterAttached("IsAttached", typeof(bool), typeof(CurrencyMaskBehavior), new PropertyMetadata(false, OnIsAttachedChanged));

        public static bool GetIsAttached(DependencyObject obj) => (bool)obj.GetValue(IsAttachedProperty);
        public static void SetIsAttached(DependencyObject obj, bool value) => obj.SetValue(IsAttachedProperty, value);

        // Método chamado quando a propriedade IsAttached é alterada
        private static void OnIsAttachedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is TextBox textBox)) return;

            if ((bool)e.NewValue)
            {
                // Se ativado, inscreve-se nos eventos do TextBox
                textBox.PreviewTextInput += TextBox_PreviewTextInput;
                textBox.TextChanged += TextBox_TextChanged;
                DataObject.AddPastingHandler(textBox, OnPaste);
            }
            else
            {
                // Se desativado, remove a inscrição
                textBox.PreviewTextInput -= TextBox_PreviewTextInput;
                textBox.TextChanged -= TextBox_TextChanged;
                DataObject.RemovePastingHandler(textBox, OnPaste);
            }
        }

        // Impede que caracteres não numéricos sejam digitados
        private static void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }

        // A lógica principal que formata o texto em tempo real
        private static void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            // Remove a inscrição para evitar um loop infinito enquanto mudamos o texto
            textBox.TextChanged -= TextBox_TextChanged;

            // Pega o texto, mantém apenas os números e remove zeros à esquerda
            string text = textBox.Text;
            string numbersOnly = new string(text.Where(char.IsDigit).ToArray());
            if (string.IsNullOrEmpty(numbersOnly)) numbersOnly = "0";

            // Converte para decimal e divide por 100
            decimal.TryParse(numbersOnly, out decimal parsedValue);
            decimal realValue = parsedValue / 100m;

            // Formata de volta para a string de moeda
            var culture = new CultureInfo("pt-BR");
            string formattedText = realValue.ToString("N2", culture);

            // Atualiza o texto e a posição do cursor
            textBox.Text = formattedText;
            textBox.CaretIndex = textBox.Text.Length;

            // Inscreve-se novamente no evento
            textBox.TextChanged += TextBox_TextChanged;
        }

        // Manipula o evento de colar para formatar o texto colado
        private static void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!text.All(char.IsDigit))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
    }
}