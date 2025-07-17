using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TesteTecnicoWPF.Models
{
    public class ItemPedido : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Armazena o ID do produto para podermos encontrá-lo depois.
        public int ProdutoId { get; set; }

        // Propriedades do produto que queremos mostrar na grade de itens do pedido.
        // Não precisam notificar mudanças, pois são apenas para exibição.
        public string CodigoProduto { get; set; }
        public string NomeProduto { get; set; }
        public decimal ValorUnitario { get; set; }

        private int _quantidade;
        public int Quantidade
        {
            get => _quantidade;
            set
            {
                if (_quantidade == value) return;
                _quantidade = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Subtotal)); // Avisa que o Subtotal também mudou.
            }
        }

        // Propriedade calculada para o subtotal do item.
        public decimal Subtotal => Quantidade * ValorUnitario;
    }
}