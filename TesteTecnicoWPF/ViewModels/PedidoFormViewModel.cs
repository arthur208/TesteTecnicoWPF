using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using TesteTecnicoWPF.Commands;
using TesteTecnicoWPF.Models;
using TesteTecnicoWPF.Views;

namespace TesteTecnicoWPF.ViewModels
{
    public class PedidoFormViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private readonly Action _onFinishInTabCallback;
        public bool IsDialogMode { get; }

        private Pedido _pedidoAtual;
        public Pedido PedidoAtual { get => _pedidoAtual; set { _pedidoAtual = value; OnPropertyChanged(); } }

        private Pessoa _clienteSelecionado;
        public Pessoa ClienteSelecionado
        {
            get => _clienteSelecionado;
            set { _clienteSelecionado = value; OnPropertyChanged(); if (PedidoAtual != null) { PedidoAtual.PessoaId = value?.Id ?? 0; } }
        }

        private Produto _produtoParaAdicionar;
        public Produto ProdutoParaAdicionar
        {
            get => _produtoParaAdicionar;
            set { _produtoParaAdicionar = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> FormasDePagamento { get; set; }

        private int _quantidadeParaAdicionar = 1;
        public int QuantidadeParaAdicionar
        {
            get => _quantidadeParaAdicionar;
            set { _quantidadeParaAdicionar = value > 0 ? value : 1; OnPropertyChanged(); }
        }

        public ICommand BuscarClienteCommand { get; private set; }
        public ICommand BuscarProdutoCommand { get; private set; }
        public ICommand ConfirmarAdicionarProdutoCommand { get; private set; }
        public ICommand RemoverProdutoCommand { get; private set; }
        public ICommand SalvarPedidoCommand { get; private set; }
        public ICommand CancelarCommand { get; private set; }

        public PedidoFormViewModel(Pedido pedido, Action onFinishInTabCallback = null)
        {
            _onFinishInTabCallback = onFinishInTabCallback;
            IsDialogMode = _onFinishInTabCallback == null;

            PedidoAtual = pedido;
            FormasDePagamento = new ObservableCollection<string> { "Dinheiro", "Cartão", "Boleto" };

            BuscarClienteCommand = new RelayCommand(BuscarCliente, p => !IsDialogMode);
            BuscarProdutoCommand = new RelayCommand(BuscarProduto);
            ConfirmarAdicionarProdutoCommand = new RelayCommand(ConfirmarAdicionarProduto, p => ProdutoParaAdicionar != null && QuantidadeParaAdicionar > 0);
            RemoverProdutoCommand = new RelayCommand(RemoverProduto);
            SalvarPedidoCommand = new RelayCommand(SalvarPedido);
            CancelarCommand = new RelayCommand(Cancelar);
        }

        private void SalvarPedido(object parameter)
        {
            if (PedidoAtual.PessoaId == 0)
            {
                MessageBox.Show("É necessário selecionar um cliente.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!PedidoAtual.Itens.Any())
            {
                MessageBox.Show("O pedido deve conter pelo menos um item.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(PedidoAtual.FormaPagamento))
            {
                MessageBox.Show("É necessário selecionar uma forma de pagamento.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (IsDialogMode)
            {
                FecharJanela(parameter as Window, true);
                return;
            }

            var todosOsPedidos = App.PedidoService.CarregarPedidos().ToList();
            var proximoId = todosOsPedidos.Any() ? todosOsPedidos.Max(p => p.Id) + 1 : 1;
            PedidoAtual.Id = proximoId;

            todosOsPedidos.Add(PedidoAtual);
            App.PedidoService.SalvarPedidos(todosOsPedidos);

            MessageBox.Show($"Pedido Nº {PedidoAtual.Id} incluído com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

            ReiniciarFormulario();
            _onFinishInTabCallback?.Invoke();
        }

        private void Cancelar(object parameter)
        {
            if (IsDialogMode)
            {
                FecharJanela(parameter as Window, false);
            }
            else
            {
                ReiniciarFormulario();
                _onFinishInTabCallback?.Invoke();
            }
        }

        private void FecharJanela(Window window, bool? dialogResult)
        {
            if (window != null)
            {
                try { window.DialogResult = dialogResult; }
                catch (InvalidOperationException) { /* Ignora se não for um diálogo */ }
                window.Close();
            }
        }

        private void ReiniciarFormulario()
        {
            PedidoAtual = new Pedido();
            OnPropertyChanged(nameof(PedidoAtual));
            ClienteSelecionado = null;
            ProdutoParaAdicionar = null;
            QuantidadeParaAdicionar = 1;
        }

        private void BuscarCliente(object obj)
        {
            var vm = new PessoaListViewModel(isLookupMode: true);
            vm.CarregarPessoas(App.PessoaService.CarregarPessoas());
            var view = new PessoaListView { DataContext = vm };

            var lookupWindow = new Window
            {
                Title = "Buscar Cliente",
                Content = view,
                Width = 1000,
                Height = 600,
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            if (lookupWindow.ShowDialog() == true && vm.PessoaSelecionada != null)
            {
                ClienteSelecionado = vm.PessoaSelecionada;
            }
        }

        private void BuscarProduto(object obj)
        {
            var vm = new ProdutoListViewModel(isLookupMode: true);
            vm.CarregarProdutos(App.ProdutoService.CarregarProdutos());
            var view = new ProdutoListView { DataContext = vm };

            var lookupWindow = new Window
            {
                Title = "Buscar Produto",
                Content = view,
                Width = 800,
                Height = 600,
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            if (lookupWindow.ShowDialog() == true && vm.ProdutoSelecionado != null)
            {
                ProdutoParaAdicionar = vm.ProdutoSelecionado;
            }
        }

        private void ConfirmarAdicionarProduto(object obj)
        {
            if (ProdutoParaAdicionar == null) return;

            var produtoEscolhido = ProdutoParaAdicionar;
            var itemExistente = PedidoAtual.Itens.FirstOrDefault(i => i.ProdutoId == produtoEscolhido.Id);

            if (itemExistente != null)
            {
                itemExistente.Quantidade += QuantidadeParaAdicionar;
            }
            else
            {
                PedidoAtual.Itens.Add(new ItemPedido
                {
                    ProdutoId = produtoEscolhido.Id,
                    CodigoProduto = produtoEscolhido.Codigo,
                    NomeProduto = produtoEscolhido.Nome,
                    ValorUnitario = produtoEscolhido.Valor,
                    Quantidade = QuantidadeParaAdicionar
                });
            }
            ProdutoParaAdicionar = null;
            QuantidadeParaAdicionar = 1;
        }

        private void RemoverProduto(object parameter)
        {
            if (parameter is ItemPedido itemParaRemover)
            {
                PedidoAtual.Itens.Remove(itemParaRemover);
            }
        }
    }
}