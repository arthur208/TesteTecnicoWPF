using System.Windows;
using TesteTecnicoWPF.ViewModels; // Adicione este using

namespace TesteTecnicoWPF.Views
{
    public partial class ProdutoListView : Window
    {
        public ProdutoListView(ProdutoListViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}