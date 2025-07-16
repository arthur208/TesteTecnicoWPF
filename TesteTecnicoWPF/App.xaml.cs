using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using TesteTecnicoWPF.Models;
using TesteTecnicoWPF.Services;
using TesteTecnicoWPF.ViewModels;
using TesteTecnicoWPF.Views;

namespace TesteTecnicoWPF
{
    public partial class App : Application
    {
        private PessoaService _pessoaService;
        private PessoaViewModel _pessoaViewModel;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 1. Inicializa os serviços e o ViewModel
            _pessoaService = new PessoaService();
            _pessoaViewModel = new PessoaViewModel();

            // 2. Carrega os dados salvos para o ViewModel
            var pessoasSalvas = _pessoaService.CarregarPessoas();
            foreach (var pessoa in pessoasSalvas)
            {
                (_pessoaViewModel.PessoasView.SourceCollection as ObservableCollection<Pessoa>)?.Add(pessoa);
            }

            // 3. Cria a View principal e injeta o ViewModel nela
            var mainWindow = new PessoaView(_pessoaViewModel);
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Salva os dados do ViewModel no arquivo ao fechar a aplicação
            _pessoaService.SalvarPessoas(_pessoaViewModel.PessoasView.SourceCollection as IEnumerable<Pessoa>);

            base.OnExit(e);
        }
    }
}