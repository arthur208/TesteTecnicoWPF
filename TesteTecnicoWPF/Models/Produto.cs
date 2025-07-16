using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TesteTecnicoWPF.Models
{
    public class Produto : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int _id;
        public int Id { get => _id; set { _id = value; OnPropertyChanged(); } }

        private string _nome;
        public string Nome { get => _nome; set { _nome = value; OnPropertyChanged(); } }

        private string _codigo;
        public string Codigo { get => _codigo; set { _codigo = value; OnPropertyChanged(); } }

        private decimal _valor;
        // Usamos 'decimal' para valores monetários para evitar problemas de arredondamento.
        public decimal Valor { get => _valor; set { _valor = value; OnPropertyChanged(); } }

        public Produto Clone()
        {
            return (Produto)this.MemberwiseClone();
        }
    }
}