using System.Windows;
using System.Windows.Input;

namespace TesteTecnicoWPF.Views
{
    public partial class PessoaFormView : Window
    {
        public PessoaFormView()
        {
            InitializeComponent();
            Loaded += (sender, e) => MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
        }
    }
}