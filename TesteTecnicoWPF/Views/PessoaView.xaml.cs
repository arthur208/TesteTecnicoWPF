using System.Windows;
// Importe o namespace dos ViewModels
using TesteTecnicoWPF.ViewModels;

namespace TesteTecnicoWPF.Views
{
    public partial class PessoaView : Window
    {
        public PessoaView(PessoaViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel; // Usa o ViewModel que foi recebido
        }
    }
}