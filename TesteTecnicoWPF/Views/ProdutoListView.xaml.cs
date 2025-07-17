using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TesteTecnicoWPF.ViewModels;

namespace TesteTecnicoWPF.Views
{
    public partial class ProdutoListView : UserControl
    {
        public ProdutoListView()
        {
            InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is ProdutoListViewModel viewModel)
            {
                if (viewModel.IsLookupMode)
                {
                    if (viewModel.ProdutoSelecionado != null)
                    {
                        Window.GetWindow(this).DialogResult = true;
                    }
                }
                else if (viewModel.EditarCommand.CanExecute(null))
                {
                    viewModel.EditarCommand.Execute(null);
                }
            }
        }

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && DataContext is ProdutoListViewModel viewModel)
            {
                if (viewModel.IsLookupMode && viewModel.ProdutoSelecionado != null)
                {
                    Window.GetWindow(this).DialogResult = true;
                    e.Handled = true;
                }
            }
        }
    }
}