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
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public Produto ProdutoEmEdicao { get; set; }

        public ICommand SalvarCommand { get; private set; }
        public ICommand CancelarCommand { get; private set; }

        public ProdutoFormViewModel(Produto produto)
        {
            ProdutoEmEdicao = produto;
            SalvarCommand = new RelayCommand(Salvar);
            CancelarCommand = new RelayCommand(Cancelar);
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

            FecharJanela(parameter as Window, true);
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
    }
}