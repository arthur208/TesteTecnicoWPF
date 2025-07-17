using System.Windows;
using System.Windows.Input;

namespace TesteTecnicoWPF.Views
{
    /// <summary>
    /// Interaction logic for ProdutoFormView.xaml
    /// </summary>
    public partial class ProdutoFormView : Window
    {
        public ProdutoFormView()
        {
            InitializeComponent();
            // Foca no primeiro campo de texto quando a janela abre
            Loaded += (sender, e) => MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
        }
    }
}