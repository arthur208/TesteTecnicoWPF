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
using TesteTecnicoWPF.Services;
using TesteTecnicoWPF.Views;

namespace TesteTecnicoWPF.ViewModels
{
    public class PessoaListViewModel : INotifyPropertyChanged
    {

        private readonly PessoaService _pessoaService;
        private readonly ObservableCollection<Pessoa> _todasAsPessoas;
        public ICollectionView PessoasView { get; }

        // --- LÓGICA DE PEDIDOS ATUALIZADA ---
        private readonly ObservableCollection<Pedido> _todosOsPedidosDoCliente;
        public ICollectionView PedidosDoClienteView { get; }
        public List<string> StatusOptions { get; }

        public bool IsLookupMode { get; }

        private Pessoa _pessoaSelecionada;
        public Pessoa PessoaSelecionada
        {
            get => _pessoaSelecionada;
            set
            {
                if (_pessoaSelecionada == value) return;
                _pessoaSelecionada = value;
                OnPropertyChanged();
                CarregarPedidosDoCliente();
            }
        }

        // --- PROPRIEDADES PARA OS FILTROS DE PESSOAS ---
        private string _filtroNome;
        public string FiltroNome { get => _filtroNome; set { _filtroNome = value; OnPropertyChanged(); PessoasView.Refresh(); } }

        private string _filtroCPF;
        public string FiltroCPF { get => _filtroCPF; set { _filtroCPF = value; OnPropertyChanged(); PessoasView.Refresh(); } }

        // Adicione esta nova propriedade
        public ObservableCollection<string> StatusSelecionadosParaFiltro { get; }

        private ObservableCollection<string> _statusSelecionados = new ObservableCollection<string>();
        public ObservableCollection<string> StatusSelecionados
        {
            get => _statusSelecionados;
            set
            {
                if (_statusSelecionados != value)
                {
                    if (_statusSelecionados != null)
                        _statusSelecionados.CollectionChanged -= StatusSelecionados_CollectionChanged;
                    _statusSelecionados = value;
                    if (_statusSelecionados != null)
                        _statusSelecionados.CollectionChanged += StatusSelecionados_CollectionChanged;
                    OnPropertyChanged();
                    PedidosDoClienteView.Refresh();
                }
            }
        }

        public ICommand NovoCommand { get; }
        public ICommand EditarCommand { get; }
        public ICommand ExcluirCommand { get; }
        public ICommand IncluirPedidoCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public PessoaListViewModel(PessoaService pessoaService,bool isLookupMode = false)
        {
            _pessoaService = pessoaService;
            IsLookupMode = isLookupMode;
            var pessoasCarregadas = _pessoaService.CarregarPessoas();
            _todasAsPessoas = new ObservableCollection<Pessoa>(pessoasCarregadas);
            _todasAsPessoas = new ObservableCollection<Pessoa>();
            _todosOsPedidosDoCliente = new ObservableCollection<Pedido>();
            StatusOptions = new List<string> { "Pendente", "Pago", "Enviado", "Recebido" };

            PessoasView = CollectionViewSource.GetDefaultView(_todasAsPessoas);
            PessoasView.Filter = AplicaFiltroPessoas;

            StatusSelecionadosParaFiltro = new ObservableCollection<string>();
            // Cria a visualização filtrável para os pedidos
            PedidosDoClienteView = CollectionViewSource.GetDefaultView(_todosOsPedidosDoCliente);
            PedidosDoClienteView.Filter = AplicaFiltroPedidos;

            NovoCommand = new RelayCommand(AbrirFormularioNovaPessoa);
            EditarCommand = new RelayCommand(AbrirFormularioEditarPessoa, PodeEditarOuExcluir);
            ExcluirCommand = new RelayCommand(ExcluirPessoa, PodeEditarOuExcluir);
            IncluirPedidoCommand = new RelayCommand(AbrirFormularioNovoPedido, PodeEditarOuExcluir);
            StatusSelecionadosParaFiltro.CollectionChanged += (s, e) => PedidosDoClienteView.Refresh();
        }

        public void CarregarPessoas(IEnumerable<Pessoa> pessoas)
        {
            _todasAsPessoas.Clear();
            if (pessoas != null) foreach (var pessoa in pessoas) _todasAsPessoas.Add(pessoa);
        }

        public IEnumerable<Pessoa> ObterTodasAsPessoas() => _todasAsPessoas;

        private void AbrirFormularioNovaPessoa(object obj)
        {
            var proximoId = _todasAsPessoas.Any() ? _todasAsPessoas.Max(p => p.Id) + 1 : 1;
            var novaPessoa = new Pessoa { Id = proximoId };
            var formViewModel = new PessoaFormViewModel(novaPessoa, _todasAsPessoas);

            // 1. Crie a sua janela de formulário
            var formView = new PessoaFormView();
            // 2. Defina o DataContext e o Owner dela
            formView.DataContext = formViewModel;
            formView.Owner = Application.Current.MainWindow;

            // 3. Chame ShowDialog() nela DIRETAMENTE
            if (formView.ShowDialog() == true)
            {
                _todasAsPessoas.Add(novaPessoa);

                _pessoaService.SalvarPessoas(_todasAsPessoas);
            }
        }

        private void AbrirFormularioEditarPessoa(object obj)
        {
            if (PessoaSelecionada == null) return;

            var copiaPessoa = PessoaSelecionada.Clone();
            var formViewModel = new PessoaFormViewModel(copiaPessoa, _todasAsPessoas);

            // 1. Crie a sua janela de formulário
            var formView = new PessoaFormView();
            // 2. Defina o DataContext e o Owner dela
            formView.DataContext = formViewModel;
            formView.Owner = Application.Current.MainWindow;

            // 3. Chame ShowDialog() nela DIRETAMENTE
            if (formView.ShowDialog() == true)
            {
                var original = _todasAsPessoas.FirstOrDefault(p => p.Id == copiaPessoa.Id);
                if (original != null)
                {
                    original.Nome = copiaPessoa.Nome;
                    original.CPF = copiaPessoa.CPF;
                    original.CEP = copiaPessoa.CEP;
                    original.Logradouro = copiaPessoa.Logradouro;
                    original.Numero = copiaPessoa.Numero;
                    original.Bairro = copiaPessoa.Bairro;
                    original.Complemento = copiaPessoa.Complemento;
                    original.Cidade = copiaPessoa.Cidade;
                    original.Estado = copiaPessoa.Estado;

                    _pessoaService.SalvarPessoas(_todasAsPessoas);
                }
            }
        }

        private void AbrirFormularioNovoPedido(object obj)
        {
            var novoPedido = new Pedido { PessoaId = PessoaSelecionada.Id };
            var formViewModel = new PedidoFormViewModel(novoPedido, App.PessoaService, App.ProdutoService, null);
            formViewModel.ClienteSelecionado = this.PessoaSelecionada;
            var formView = new PedidoFormView { DataContext = formViewModel };
            var dialogWindow = new Window
            {
                Title = $"Incluir Pedido para {PessoaSelecionada.Nome}",
                Content = formView,
                Width = 850,
                Height = 650,
                ResizeMode = ResizeMode.CanResize,
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            if (dialogWindow.ShowDialog() == true)
            {
                var todosOsPedidos = App.PedidoService.CarregarPedidos().ToList();
                var proximoId = todosOsPedidos.Any() ? todosOsPedidos.Max(p => p.Id) + 1 : 1;
                novoPedido.Id = proximoId;

                todosOsPedidos.Add(novoPedido);
                App.PedidoService.SalvarPedidos(todosOsPedidos);
                MessageBox.Show("Pedido incluído com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                CarregarPedidosDoCliente();
            }
        }

        // Em PessoaListViewModel.cs

        private void CarregarPedidosDoCliente()
        {
            // ----> PASSO 1.A: Cancelar inscrição dos eventos antigos
            foreach (var pedido in _todosOsPedidosDoCliente)
            {
                pedido.PropertyChanged -= Pedido_PropertyChanged;
            }
            // <---- FIM DO PASSO 1.A

            _todosOsPedidosDoCliente.Clear();

            if (PessoaSelecionada != null)
            {
                var todosOsPedidos = App.PedidoService.CarregarPedidos();
                var pedidosDoCliente = todosOsPedidos.Where(p => p.PessoaId == PessoaSelecionada.Id);

                foreach (var pedido in pedidosDoCliente.OrderByDescending(p => p.DataVenda))
                {
                    // ----> PASSO 1.B: Inscrever-se no evento de cada novo pedido
                    pedido.PropertyChanged += Pedido_PropertyChanged;
                    // <---- FIM DO PASSO 1.B
                    _todosOsPedidosDoCliente.Add(pedido);
                }
            }
        }
        // Em PessoaListViewModel.cs, adicione este novo método

        private void Pedido_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Verificamos se a propriedade que mudou foi "Status"
            if (e.PropertyName == nameof(Pedido.Status))
            {
                // Se foi, chamamos nosso método para salvar as alterações
                SalvarAlteracoesPedidos();
            }
        }
        // Em PessoaListViewModel.cs, adicione este outro novo método

        private void SalvarAlteracoesPedidos()
        {
            // 1. Carrega TODOS os pedidos existentes do arquivo JSON
            var todosOsPedidosDoSistema = App.PedidoService.CarregarPedidos().ToList();

            // 2. Itera sobre os pedidos que estão na tela (que podem ter sido modificados)
            foreach (var pedidoModificado in _todosOsPedidosDoCliente)
            {
                // 3. Encontra o pedido correspondente na lista completa do sistema
                var pedidoOriginal = todosOsPedidosDoSistema.FirstOrDefault(p => p.Id == pedidoModificado.Id);
                if (pedidoOriginal != null)
                {
                    // 4. Atualiza o status do pedido na lista completa
                    pedidoOriginal.Status = pedidoModificado.Status;
                }
            }

            // 5. Salva a lista COMPLETA E ATUALIZADA de volta no arquivo JSON
            App.PedidoService.SalvarPedidos(todosOsPedidosDoSistema);
        }

        private void ExcluirPessoa(object obj)
        {
            if (MessageBox.Show($"Tem certeza que deseja excluir '{PessoaSelecionada.Nome}'?", "Confirmar Exclusão", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _todasAsPessoas.Remove(PessoaSelecionada);

                _pessoaService.SalvarPessoas(_todasAsPessoas);

            }
        }

        private bool PodeEditarOuExcluir(object obj) => PessoaSelecionada != null;

        private bool AplicaFiltroPessoas(object item)
        {
            if (item is Pessoa pessoa)
            {
                bool nomePassou = string.IsNullOrWhiteSpace(FiltroNome) || pessoa.Nome.IndexOf(FiltroNome, StringComparison.OrdinalIgnoreCase) >= 0;
                bool cpfPassou = string.IsNullOrWhiteSpace(FiltroCPF) || pessoa.CPF.Contains(FiltroCPF);
                return nomePassou && cpfPassou;
            }
            return false;
        }

        // NOVO MÉTODO PARA FILTRAR OS PEDIDOS
        // Substitua o método antigo por este
        private bool AplicaFiltroPedidos(object item)
        {
            var pedido = item as Pedido;
            if (pedido == null) return false;

            // Se nenhuma opção de filtro estiver selecionada, mostre todos os pedidos.
            if (StatusSelecionadosParaFiltro.Count == 0)
            {
                return true;
            }

            // Caso contrário, mostre o pedido apenas se seu status ESTIVER CONTIDO
            // na lista de status selecionados para o filtro.
            return StatusSelecionadosParaFiltro.Contains(pedido.Status);
        }

        private void StatusSelecionados_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PedidosDoClienteView.Refresh();
        }
    }
}