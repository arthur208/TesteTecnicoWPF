using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;
using TesteTecnicoWPF.Commands;
using TesteTecnicoWPF.Models;
using TesteTecnicoWPF.Views;

namespace TesteTecnicoWPF.ViewModels
{
    public class PessoaListViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Pessoa> _todasAsPessoas;
        public ICollectionView PessoasView { get; private set; }

        private Pessoa _pessoaSelecionada;
        public Pessoa PessoaSelecionada
        {
            get => _pessoaSelecionada;
            set { _pessoaSelecionada = value; OnPropertyChanged(); }
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
        public ICommand EditarCommand { get; private set; }
        public ICommand ExcluirCommand { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public PessoaListViewModel()
        {
            _todasAsPessoas = new ObservableCollection<Pessoa>();
            PessoasView = CollectionViewSource.GetDefaultView(_todasAsPessoas);
            PessoasView.Filter = AplicaFiltro;

            NovoCommand = new RelayCommand(AbrirFormularioNovaPessoa);
            EditarCommand = new RelayCommand(AbrirFormularioEditarPessoa, PodeEditarOuExcluir);
            ExcluirCommand = new RelayCommand(ExcluirPessoa, PodeEditarOuExcluir);
        }

        private void AbrirFormularioNovaPessoa(object obj)
        {
            // Calcula o próximo ID baseado no maior ID existente na lista.
            var proximoId = _todasAsPessoas.Any() ? _todasAsPessoas.Max(p => p.Id) + 1 : 1;

            var novaPessoa = new Pessoa { Id = proximoId };

            // O resto do método continua igual, mas agora passaremos a lista para validar duplicidade.
            var formViewModel = new PessoaFormViewModel(novaPessoa, _todasAsPessoas);
            var formView = new PessoaFormView { DataContext = formViewModel };

            bool? resultado = formView.ShowDialog();

            if (resultado == true)
            {
                _todasAsPessoas.Add(novaPessoa);
            }
        }

        private void AbrirFormularioEditarPessoa(object obj)
        {
            var copiaPessoa = PessoaSelecionada.Clone();
            var formViewModel = new PessoaFormViewModel(copiaPessoa, _todasAsPessoas);
            var formView = new PessoaFormView { DataContext = formViewModel };

            bool? resultado = formView.ShowDialog();

            if (resultado == true)
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
                }
            }
        }

        private bool PodeEditarOuExcluir(object obj) => PessoaSelecionada != null;

        private void ExcluirPessoa(object obj)
        {
            if (PessoaSelecionada != null)
            {
                _todasAsPessoas.Remove(PessoaSelecionada);
            }
        }

        private bool AplicaFiltro(object item)
        {
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
    }
}