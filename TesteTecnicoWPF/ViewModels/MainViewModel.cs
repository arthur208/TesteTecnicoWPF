using System.ComponentModel;
using System.Runtime.CompilerServices;
using TesteTecnicoWPF.Models;

namespace TesteTecnicoWPF.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public PessoaListViewModel PessoaListVM { get; set; }
        public ProdutoListViewModel ProdutoListVM { get; set; }
        public PedidoFormViewModel PedidoFormVM { get; set; }

        private int _selectedTabIndex;
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set
            {
                if (_selectedTabIndex == value) return;
                _selectedTabIndex = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            var pessoaService = App.PessoaService;
            var produtoService = App.ProdutoService;

            PessoaListVM = new PessoaListViewModel(pessoaService);
            PessoaListVM.CarregarPessoas(App.PessoaService.CarregarPessoas());

            ProdutoListVM = new ProdutoListViewModel();
            ProdutoListVM.CarregarProdutos(App.ProdutoService.CarregarProdutos());

            PedidoFormVM = new PedidoFormViewModel(new Pedido(), pessoaService, produtoService, () => { SelectedTabIndex = 0; });
        }
    }
}