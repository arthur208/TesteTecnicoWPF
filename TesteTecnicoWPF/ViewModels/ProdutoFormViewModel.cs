using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using TesteTecnicoWPF.Commands;
using TesteTecnicoWPF.Models;

namespace TesteTecnicoWPF.ViewModels
{
    public class ProdutoFormViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Produto ProdutoEmEdicao { get; set; }

        public ICommand SalvarCommand { get; private set; }
        public ICommand CancelarCommand { get; private set; }

        public ProdutoFormViewModel(Produto produto)
        {
            ProdutoEmEdicao = produto;
            SalvarCommand = new RelayCommand(Salvar);
            // O comando Cancelar não precisa de lógica, pois IsCancel=true na View já fecha o diálogo.
        }

        private void Salvar(object parameter)
        {
            // Validação dos campos obrigatórios
            if (string.IsNullOrWhiteSpace(ProdutoEmEdicao.Nome) ||
                string.IsNullOrWhiteSpace(ProdutoEmEdicao.Codigo) ||
                ProdutoEmEdicao.Valor <= 0)
            {
                MessageBox.Show("Todos os campos são obrigatórios e o valor deve ser maior que zero.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Se a validação passar, fecha a janela com resultado "sucesso" (true)
            FecharJanela(parameter as Window, true);
        }

        private void Cancelar(object parameter)
        {
            // O botão na View com IsCancel=true já cuida de fechar a janela.
            // Este comando é um fallback ou pode ser removido se o botão não tiver Command binding.
            FecharJanela(parameter as Window, false);
        }

        private void FecharJanela(Window window, bool? dialogResult)
        {
            if (window != null)
            {
                try
                {
                    window.DialogResult = dialogResult;
                }
                catch (InvalidOperationException) { /* Ignora o erro se não for um diálogo */ }
                window.Close();
            }
        }
    }
}