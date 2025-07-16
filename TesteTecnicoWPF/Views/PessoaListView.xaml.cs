using System.Windows;
using System.Windows.Input;
using TesteTecnicoWPF.ViewModels;

namespace TesteTecnicoWPF.Views
{
    public partial class PessoaListView : Window
    {
        public PessoaListView(PessoaListViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is PessoaListViewModel viewModel && viewModel.EditarCommand.CanExecute(null))
            {
                viewModel.EditarCommand.Execute(null);
            }
        }
    }
}