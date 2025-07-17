using System.Windows;
using TesteTecnicoWPF.Services;
using TesteTecnicoWPF.ViewModels;
using TesteTecnicoWPF.Views;

namespace TesteTecnicoWPF
{
    public partial class App : Application
    {
        public static PessoaService PessoaService { get; private set; }
        public static ProdutoService ProdutoService { get; private set; }
        public static PedidoService PedidoService { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            PessoaService = new PessoaService();
            ProdutoService = new ProdutoService();
            PedidoService = new PedidoService();

            var mainViewModel = new MainViewModel();
            var mainView = new MainView { DataContext = mainViewModel };
            mainView.Show();
        }
    }
}