using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TesteTecnicoWPF.Commands;
using TesteTecnicoWPF.Models;
using TesteTecnicoWPF.Models.BrasilApi;
using TesteTecnicoWPF.Services;

namespace TesteTecnicoWPF.ViewModels
{
    public class PessoaFormViewModel : INotifyPropertyChanged
    {
        private readonly BrasilApiService _brasilApiService;
        private readonly IEnumerable<Pessoa> _listaPessoasExistentes;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public Pessoa PessoaEmEdicao { get; set; }
        public ObservableCollection<Estado> Estados { get; set; }
        public ObservableCollection<Municipio> Municipios { get; set; }

        private string _cpfErrorMessage;

        public string CpfErrorMessage
        {
            get => _cpfErrorMessage;
            set { _cpfErrorMessage = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasCpfError)); }
        }


        private Estado _estadoSelecionado;
        public Estado EstadoSelecionado
        {
            get => _estadoSelecionado;
            set
            {
                if (_estadoSelecionado == value) return;
                _estadoSelecionado = value;
                OnPropertyChanged();
                if (PessoaEmEdicao != null) PessoaEmEdicao.Estado = _estadoSelecionado?.Sigla;
                _ = CarregarMunicipios();
            }
        }
        public bool HasCpfError => !string.IsNullOrEmpty(CpfErrorMessage);

        public ICommand SalvarCommand { get; private set; }
        public ICommand CancelarCommand { get; private set; }
        public ICommand BuscarCepCommand { get; private set; }

        public PessoaFormViewModel(Pessoa pessoa, IEnumerable<Pessoa> pessoasExistentes)
        {
            _brasilApiService = new BrasilApiService();
            _listaPessoasExistentes = pessoasExistentes;
            PessoaEmEdicao = pessoa;
            // Assina o evento para saber quando uma propriedade da pessoa muda
            PessoaEmEdicao.PropertyChanged += PessoaEmEdicao_PropertyChanged;
            Estados = new ObservableCollection<Estado>();
            Municipios = new ObservableCollection<Municipio>();

            SalvarCommand = new RelayCommand(Salvar);
            CancelarCommand = new RelayCommand(Cancelar);
            BuscarCepCommand = new RelayCommand(async (p) => await BuscarCep());

            _ = CarregarDadosIniciais();
        }

        private void PessoaEmEdicao_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Gatilho para validações e buscas automáticas
            if (e.PropertyName == "CPF")
            {
                ValidarCpfEmTempoReal();
            }
            else if (e.PropertyName == "CEP")
            {
                _ = VerificarEBuscarCepAutomaticamente();
            }
        }
        private void Salvar(object parameter)
        {
            // A validação final antes de salvar
            if (HasCpfError || string.IsNullOrWhiteSpace(PessoaEmEdicao.Nome))
            {
                MessageBox.Show("Verifique os erros no formulário antes de salvar.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            FecharJanela(parameter as Window, true);
        }

        private void ValidarCpfEmTempoReal()
        {
            var cpf = PessoaEmEdicao.CPF;
            if (string.IsNullOrWhiteSpace(cpf) || cpf.Length != 11)
            {
                CpfErrorMessage = null; // Limpa o erro se o campo estiver incompleto
                return;
            }

            // 1. Validação de duplicidade
            if (_listaPessoasExistentes.Any(p => p.CPF == cpf && p.Id != PessoaEmEdicao.Id))
            {
                CpfErrorMessage = "Este CPF já está cadastrado.";
                return;
            }

            // 2. Validação matemática
            if (!IsCpfValido(cpf))
            {
                CpfErrorMessage = "O CPF informado é inválido.";
                return;
            }

            // Se tudo estiver OK, limpa a mensagem de erro
            CpfErrorMessage = null;
        }

        private bool _isBuscandoCep = false;
        private async Task VerificarEBuscarCepAutomaticamente()
        {
            if (PessoaEmEdicao.CEP?.Length == 8 && !_isBuscandoCep)
            {
                _isBuscandoCep = true;
                await BuscarCep();
                _isBuscandoCep = false;
            }
        }
        private static bool IsCpfValido(string cpf)
        {
            // O CPF já vem apenas com números.
            if (cpf.Length != 11) return false;

            // Verifica CPFs inválidos conhecidos (todos os dígitos iguais)
            if (new string(cpf[0], 11) == cpf) return false;

            var numeros = cpf.Select(c => c - '0').ToArray();

            // Validação do primeiro dígito verificador
            int soma = 0;
            for (int i = 0; i < 9; i++)
                soma += numeros[i] * (10 - i);

            int resto = soma % 11;
            int digitoVerificador1 = resto < 2 ? 0 : 11 - resto;

            if (numeros[9] != digitoVerificador1) return false;

            // Validação do segundo dígito verificador
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += numeros[i] * (11 - i);

            resto = soma % 11;
            int digitoVerificador2 = resto < 2 ? 0 : 11 - resto;

            if (numeros[10] != digitoVerificador2) return false;

            return true;
        }
        private void Cancelar(object parameter)
        {
            FecharJanela(parameter as Window, false);
        }

        private void FecharJanela(Window window, bool dialogResult)
        {
            if (window != null)
            {
                window.DialogResult = dialogResult;
                window.Close();
            }
        }

        private async Task CarregarDadosIniciais()
        {
            var estadosResult = await _brasilApiService.GetEstados();
            foreach (var estado in estadosResult) Estados.Add(estado);

            if (!string.IsNullOrEmpty(PessoaEmEdicao.Estado))
            {
                EstadoSelecionado = Estados.FirstOrDefault(e => e.Sigla == PessoaEmEdicao.Estado);
            }
        }

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
                MessageBox.Show($"O CEP '{PessoaEmEdicao.CEP}' não foi encontrado ou é inválido.", "CEP não encontrado", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}