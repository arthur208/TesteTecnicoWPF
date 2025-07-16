using System.Collections.ObjectModel;
using System.Windows;
using TesteTecnicoWPF.Models;
using TesteTecnicoWPF.Services;
using TesteTecnicoWPF.ViewModels;
using TesteTecnicoWPF.Views;
using System.Collections.Generic;

namespace TesteTecnicoWPF
{
    public partial class App : Application
    {
        private PessoaService _pessoaService;
        private PessoaListViewModel _pessoaListViewModel;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _pessoaService = new PessoaService();
            _pessoaListViewModel = new PessoaListViewModel();

            var pessoasSalvas = _pessoaService.CarregarPessoas();
            if (pessoasSalvas != null)
            {
                // Acesso à coleção interna do ViewModel para popular os dados
                var colecaoInterna = _pessoaListViewModel.PessoasView.SourceCollection as ObservableCollection<Pessoa>;
                if (colecaoInterna != null)
                {
                    foreach (var pessoa in pessoasSalvas)
                    {
                        colecaoInterna.Add(pessoa);
                    }
                }
            }

            var mainWindow = new PessoaListView(_pessoaListViewModel);
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Acesso à coleção interna para salvar os dados
            var colecaoParaSalvar = _pessoaListViewModel.PessoasView.SourceCollection as IEnumerable<Pessoa>;
            if (colecaoParaSalvar != null)
            {
                _pessoaService.SalvarPessoas(colecaoParaSalvar);
            }

            base.OnExit(e);
        }
    }
}