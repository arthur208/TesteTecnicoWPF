using System.ComponentModel;
using System.Windows;
using TesteTecnicoWPF.ViewModels;

namespace TesteTecnicoWPF.Views
{
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                App.PessoaService.SalvarPessoas(vm.PessoaListVM.ObterTodasAsPessoas());
                App.ProdutoService.SalvarProdutos(vm.ProdutoListVM.ObterTodosOsProdutos());
            }
            base.OnClosing(e);
        }
    }
}