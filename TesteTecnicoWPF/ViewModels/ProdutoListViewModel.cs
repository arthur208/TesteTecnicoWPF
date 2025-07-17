using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using TesteTecnicoWPF.Commands;
using TesteTecnicoWPF.Models;
using TesteTecnicoWPF.Views;

namespace TesteTecnicoWPF.ViewModels
{
    public class ProdutoListViewModel : INotifyPropertyChanged
    {
        private readonly ObservableCollection<Produto> _todosOsProdutos;
        public ICollectionView ProdutosView { get; }

        public bool IsLookupMode { get; }

        private Produto _produtoSelecionado;
        public Produto ProdutoSelecionado
        {
            get => _produtoSelecionado;
            set { _produtoSelecionado = value; OnPropertyChanged(); }
        }

        private string _filtroNome;
        public string FiltroNome
        {
            get => _filtroNome;
            set { _filtroNome = value; OnPropertyChanged(); ProdutosView.Refresh(); }
        }

        private string _filtroCodigo;
        public string FiltroCodigo
        {
            get => _filtroCodigo;
            set { _filtroCodigo = value; OnPropertyChanged(); ProdutosView.Refresh(); }
        }

        private decimal? _filtroValorInicial;
        public decimal? FiltroValorInicial
        {
            get => _filtroValorInicial;
            set { _filtroValorInicial = value; OnPropertyChanged(); ProdutosView.Refresh(); }
        }

        private decimal? _filtroValorFinal;
        public decimal? FiltroValorFinal
        {
            get => _filtroValorFinal;
            set { _filtroValorFinal = value; OnPropertyChanged(); ProdutosView.Refresh(); }
        }

        public ICommand NovoCommand { get; }
        public ICommand EditarCommand { get; }
        public ICommand ExcluirCommand { get; }
        public ICommand SelecionarCommand { get; } // Comando que estava faltando

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public ProdutoListViewModel(bool isLookupMode = false)
        {
            IsLookupMode = isLookupMode;
            _todosOsProdutos = new ObservableCollection<Produto>();
            ProdutosView = CollectionViewSource.GetDefaultView(_todosOsProdutos);
            ProdutosView.Filter = AplicaFiltro;

            NovoCommand = new RelayCommand(AbrirFormularioNovoProduto);
            EditarCommand = new RelayCommand(AbrirFormularioEditarProduto, PodeEditarOuExcluir);
            ExcluirCommand = new RelayCommand(ExcluirProduto, PodeEditarOuExcluir);
            SelecionarCommand = new RelayCommand(Selecionar, PodeEditarOuExcluir); // Inicialização que estava faltando
        }

        public void CarregarProdutos(IEnumerable<Produto> produtos)
        {
            _todosOsProdutos.Clear();
            if (produtos != null)
            {
                foreach (var produto in produtos)
                {
                    _todosOsProdutos.Add(produto);
                }
            }
        }

        public IEnumerable<Produto> ObterTodosOsProdutos() => _todosOsProdutos;

        private void AbrirFormularioNovoProduto(object obj)
        {
            var proximoId = _todosOsProdutos.Any() ? _todosOsProdutos.Max(p => p.Id) + 1 : 1;
            var novoProduto = new Produto { Id = proximoId };
            var formViewModel = new ProdutoFormViewModel(novoProduto);
            var formView = new ProdutoFormView { DataContext = formViewModel };

            if (formView.ShowDialog() == true)
            {
                _todosOsProdutos.Add(novoProduto);
            }
        }

        private void AbrirFormularioEditarProduto(object obj)
        {
            var copiaProduto = ProdutoSelecionado.Clone();
            var formViewModel = new ProdutoFormViewModel(copiaProduto);
            var formView = new ProdutoFormView { DataContext = formViewModel };

            if (formView.ShowDialog() == true)
            {
                var original = _todosOsProdutos.FirstOrDefault(p => p.Id == copiaProduto.Id);
                if (original != null)
                {
                    original.Nome = copiaProduto.Nome;
                    original.Codigo = copiaProduto.Codigo;
                    original.Valor = copiaProduto.Valor;
                }
            }
        }

        private void ExcluirProduto(object obj)
        {
            if (MessageBox.Show($"Tem certeza que deseja excluir '{ProdutoSelecionado.Nome}'?", "Confirmar Exclusão", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _todosOsProdutos.Remove(ProdutoSelecionado);
            }
        }

        // Método que estava faltando
        private void Selecionar(object parameter)
        {
            if (parameter is Window window)
            {
                window.DialogResult = true;
                window.Close();
            }
        }

        private bool PodeEditarOuExcluir(object obj)
        {
            return ProdutoSelecionado != null;
        }

        private bool AplicaFiltro(object item)
        {
            if (item is Produto produto)
            {
                bool nomePassou = string.IsNullOrWhiteSpace(FiltroNome) || produto.Nome.IndexOf(FiltroNome, StringComparison.OrdinalIgnoreCase) >= 0;
                bool codigoPassou = string.IsNullOrWhiteSpace(FiltroCodigo) || produto.Codigo.IndexOf(FiltroCodigo, StringComparison.OrdinalIgnoreCase) >= 0;
                bool valorInicialPassou = !FiltroValorInicial.HasValue || produto.Valor >= FiltroValorInicial.Value;
                bool valorFinalPassou = !FiltroValorFinal.HasValue || produto.Valor <= FiltroValorFinal.Value;
                return nomePassou && codigoPassou && valorInicialPassou && valorFinalPassou;
            }
            return false;
        }
    }
}