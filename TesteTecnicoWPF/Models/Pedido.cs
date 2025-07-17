using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using TesteTecnicoWPF.Models;

namespace TesteTecnicoWPF.Models
{
    public class Pedido : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int Id { get; set; }
        public int PessoaId { get; set; }
        public ObservableCollection<ItemPedido> Itens { get; set; }
        public decimal ValorTotal => Itens?.Sum(item => item.Subtotal) ?? 0;
        public DateTime DataVenda { get; set; }
        public string FormaPagamento { get; set; }
        
        private string _status;
        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }

        public Pedido()
        {
            Itens = new ObservableCollection<ItemPedido>();
            Itens.CollectionChanged += Itens_CollectionChanged;
            DataVenda = DateTime.Now;
            Status = "Pendente";
        }

        private void Itens_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(ValorTotal));

            if (e.NewItems != null)
            {
                foreach (INotifyPropertyChanged item in e.NewItems)
                {
                    item.PropertyChanged += Item_PropertyChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged item in e.OldItems)
                {
                    item.PropertyChanged -= Item_PropertyChanged;
                }
            }
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ItemPedido.Subtotal))
            {
                OnPropertyChanged(nameof(ValorTotal));
            }
        }
    }
}