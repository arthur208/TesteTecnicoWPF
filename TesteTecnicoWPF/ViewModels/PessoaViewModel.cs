using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows; // Adicionado para usar o MessageBox
using System.Windows.Data;
using System.Windows.Input;
using TesteTecnicoWPF.Commands;
using TesteTecnicoWPF.Models;
using TesteTecnicoWPF.Models.BrasilApi;
using TesteTecnicoWPF.Services;
using TesteTecnicoWPF.Views;

namespace TesteTecnicoWPF.ViewModels
{
    public class PessoaViewModel : INotifyPropertyChanged
    {
        private readonly BrasilApiService _brasilApiService;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ObservableCollection<Pessoa> _todasAsPessoas;
        public ICollectionView PessoasView { get; private set; }
        public ObservableCollection<Estado> Estados { get; set; }
        public ObservableCollection<Municipio> Municipios { get; set; }

        private Pessoa _pessoaEmEdicao;
        public Pessoa PessoaEmEdicao
        {
            get { return _pessoaEmEdicao; }
            set { _pessoaEmEdicao = value; OnPropertyChanged(); }
        }

        private Pessoa _pessoaSelecionada;
        public Pessoa PessoaSelecionada
        {
            get { return _pessoaSelecionada; }
            set
            {
                _pessoaSelecionada = value;
                OnPropertyChanged();

                if (_pessoaSelecionada != null)
                {
                    PessoaEmEdicao = new Pessoa
                    {
                        Id = _pessoaSelecionada.Id,
                        Nome = _pessoaSelecionada.Nome,
                        CPF = _pessoaSelecionada.CPF,
                        CEP = _pessoaSelecionada.CEP,
                        Logradouro = _pessoaSelecionada.Logradouro,
                        Numero = _pessoaSelecionada.Numero,
                        Bairro = _pessoaSelecionada.Bairro,
                        Complemento = _pessoaSelecionada.Complemento,
                        Cidade = _pessoaSelecionada.Cidade,
                        Estado = _pessoaSelecionada.Estado
                    };

                    if (!string.IsNullOrEmpty(_pessoaSelecionada.Estado))
                    {
                        EstadoSelecionado = Estados.FirstOrDefault(e => e.Sigla == _pessoaSelecionada.Estado);
                    }
                }
            }
        }

        private Estado _estadoSelecionado;
        public Estado EstadoSelecionado
        {
            get { return _estadoSelecionado; }
            set
            {
                if (_estadoSelecionado != value)
                {
                    _estadoSelecionado = value;
                    OnPropertyChanged();
                    if (PessoaEmEdicao != null)
                    {
                        PessoaEmEdicao.Estado = _estadoSelecionado?.Sigla;
                    }
                    _ = CarregarMunicipios();
                }
            }
        }

        private string _filtroNome;
        public string FiltroNome
        {
            get => _filtroNome;
            set { _filtroNome = value; OnPropertyChanged(); PessoasView.Refresh(); }
        }

        private string _filtroCPF;
        public string FiltroCPF
        {
            get => _filtroCPF;
            set { _filtroCPF = value; OnPropertyChanged(); PessoasView.Refresh(); }
        }

        public ICommand NovoCommand { get; private set; }
        public ICommand SalvarCommand { get; private set; }
        public ICommand ExcluirCommand { get; private set; }
        public ICommand BuscarCepCommand { get; private set; }
        public ICommand LostFocusCepCommand { get; private set; }

        public PessoaViewModel()
        { // Inicializa o ViewModel e carrega os dados iniciais
            _brasilApiService = new BrasilApiService();
            _todasAsPessoas = new ObservableCollection<Pessoa>();
            PessoasView = CollectionViewSource.GetDefaultView(_todasAsPessoas);
            PessoasView.Filter = AplicaFiltro;
            Estados = new ObservableCollection<Estado>();
            Municipios = new ObservableCollection<Municipio>();

            NovoCommand = new RelayCommand(NovaPessoa);
            SalvarCommand = new RelayCommand(SalvarPessoa);
            ExcluirCommand = new RelayCommand(ExcluirPessoa, CanExcluirPessoa);
            BuscarCepCommand = new RelayCommand(async (p) => await BuscarCep());

            _ = CarregarDadosIniciais();
            PessoaEmEdicao = new Pessoa();
        }

        private bool AplicaFiltro(object item)
        {   // Aplica o filtro de pesquisa na lista de pessoas
            if (item is Pessoa pessoa)
            {
                bool nomePassou = string.IsNullOrWhiteSpace(FiltroNome) ||
                                  pessoa.Nome.IndexOf(FiltroNome, StringComparison.OrdinalIgnoreCase) >= 0;

                bool cpfPassou = string.IsNullOrWhiteSpace(FiltroCPF) ||
                                 pessoa.CPF.Contains(FiltroCPF);

                return nomePassou && cpfPassou;
            }
            return false;
        }
        
        private void NovaPessoa(object obj)
        {   // Limpa os campos de edição e seleciona uma nova pessoa
            PessoaEmEdicao = new Pessoa();
            PessoaSelecionada = null;
            EstadoSelecionado = null;
            Municipios.Clear();
        }

        private void SalvarPessoa(object obj)
        {
            // Validação de campos obrigatórios
            if (string.IsNullOrWhiteSpace(PessoaEmEdicao.Nome) || string.IsNullOrWhiteSpace(PessoaEmEdicao.CPF))
            {
                MessageBox.Show("Os campos 'Nome' e 'CPF' são obrigatórios.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // === NOVA VALIDAÇÃO MATEMÁTICA DO CPF ===
            if (!IsCpfValido(PessoaEmEdicao.CPF))
            {
                MessageBox.Show("O CPF informado é inválido. Verifique os dígitos.", "CPF Inválido", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            var pessoaExistente = _todasAsPessoas.FirstOrDefault(p => p.Id == PessoaEmEdicao.Id);

            if (pessoaExistente == null)
            {
                
                _todasAsPessoas.Add(PessoaEmEdicao);
            }
            else
            {
                
                pessoaExistente.Nome = PessoaEmEdicao.Nome;
                pessoaExistente.CPF = PessoaEmEdicao.CPF;
                pessoaExistente.CEP = PessoaEmEdicao.CEP;
                pessoaExistente.Logradouro = PessoaEmEdicao.Logradouro;
                pessoaExistente.Numero = PessoaEmEdicao.Numero;
                pessoaExistente.Bairro = PessoaEmEdicao.Bairro;
                pessoaExistente.Complemento = PessoaEmEdicao.Complemento;
                pessoaExistente.Cidade = PessoaEmEdicao.Cidade;
                pessoaExistente.Estado = PessoaEmEdicao.Estado;
            }

            NovaPessoa(null);
        }

        private void ExcluirPessoa(object obj)
        {
            if (PessoaSelecionada != null)
            {
                _todasAsPessoas.Remove(PessoaSelecionada);
                NovaPessoa(null);
            }
        }

        private bool CanExcluirPessoa(object obj)
        {
            return PessoaSelecionada != null;
        }

        private async Task CarregarDadosIniciais()
        {
            // Carrega os estados e municípios da BrasilAPI
            var estadosResult = await _brasilApiService.GetEstados();
            if (estadosResult.Any())
            {
                foreach (var estado in estadosResult) Estados.Add(estado);
            }
            else
            {
                MessageBox.Show("Não foi possível carregar a lista de estados. Verifique sua conexão com a internet.", "Erro de Rede", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Carrega os municípios do estado selecionado
        private async Task CarregarMunicipios()
        {
            Municipios.Clear();
            if (EstadoSelecionado != null)
            {
                var municipiosResult = await _brasilApiService.GetMunicipiosPorUF(EstadoSelecionado.Sigla);
                foreach (var municipio in municipiosResult) Municipios.Add(municipio);

                if (!string.IsNullOrEmpty(PessoaEmEdicao?.Cidade))
                {
                    var cidadeMatch = Municipios.FirstOrDefault(m => m.Nome.Equals(PessoaEmEdicao.Cidade, StringComparison.InvariantCultureIgnoreCase));
                    if (cidadeMatch != null) PessoaEmEdicao.Cidade = cidadeMatch.Nome;
                }
            }
        }
        // Método chamado quando o campo CEP perde o foco
        private async Task BuscarCep()
        {
            if (PessoaEmEdicao == null || string.IsNullOrWhiteSpace(PessoaEmEdicao.CEP)) return;

            var cepLimpo = Regex.Replace(PessoaEmEdicao.CEP, @"[^\d]", "");
            if (cepLimpo.Length != 8) return;

            var endereco = await _brasilApiService.GetEnderecoPorCep(cepLimpo);
            if (endereco != null)
            {
                PessoaEmEdicao.Logradouro = endereco.Street;
                PessoaEmEdicao.Bairro = endereco.Neighborhood;
                PessoaEmEdicao.Cidade = new CultureInfo("pt-BR", false).TextInfo.ToTitleCase(endereco.City.ToLower());
                EstadoSelecionado = Estados.FirstOrDefault(e => e.Sigla == endereco.State);
            }
            else
            {
                // === NOVA MENSAGEM DE ERRO ===
                MessageBox.Show($"O CEP '{PessoaEmEdicao.CEP}' não foi encontrado ou é inválido.", "CEP não encontrado", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private static bool IsCpfValido(string cpf)
        {
            // Limpa a máscara do CPF, deixando apenas números
            var numeros = new int[11];
            int j = 0;
            foreach (char c in cpf)
            {
                if (char.IsDigit(c))
                {
                    if (j < 11) numeros[j++] = c - '0';
                }
            }
            // Garante que temos 11 dígitos
            if (j != 11) return false;

            // --- Verifica CPFs inválidos conhecidos (todos os dígitos iguais) ---
            bool todosIguais = true;
            for (int i = 1; i < 11; i++)
            {
                if (numeros[i] != numeros[0])
                {
                    todosIguais = false;
                    break;
                }
            }
            if (todosIguais) return false;

            // --- Validação do primeiro dígito verificador ---
            int soma = 0;
            for (int i = 0; i < 9; i++)
                soma += numeros[i] * (10 - i);

            int resto = soma % 11;
            int digitoVerificador1 = resto < 2 ? 0 : 11 - resto;

            if (numeros[9] != digitoVerificador1) return false;

            // --- Validação do segundo dígito verificador ---
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += numeros[i] * (11 - i);

            resto = soma % 11;
            int digitoVerificador2 = resto < 2 ? 0 : 11 - resto;

            if (numeros[10] != digitoVerificador2) return false;

            // Se passou por todas as validações, o CPF é válido.
            return true;
        }
    }
}