using System.Collections;
using System.Windows;
using Microsoft.Xaml.Behaviors; // Certifique-se de ter o pacote NuGet Microsoft.Xaml.Behaviors.Wpf

namespace TesteTecnicoWPF.Behaviors // Mude o namespace se necessário
{
    public enum CollectionAction
    {
        Add,
        Remove
    }

    public class AddOrRemoveFromCollectionAction : TriggerAction<DependencyObject>
    {
        public static readonly DependencyProperty CollectionProperty =
            DependencyProperty.Register("Collection", typeof(IList), typeof(AddOrRemoveFromCollectionAction), new PropertyMetadata(null));

        public IList Collection
        {
            get { return (IList)GetValue(CollectionProperty); }
            set { SetValue(CollectionProperty, value); }
        }

        public static readonly DependencyProperty ItemProperty =
            DependencyProperty.Register("Item", typeof(object), typeof(AddOrRemoveFromCollectionAction), new PropertyMetadata(null));

        public object Item
        {
            get { return (object)GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); }
        }

        public CollectionAction Action { get; set; }

        protected override void Invoke(object parameter)
        {
            if (Collection == null || Item == null) return;

            if (Action == CollectionAction.Add)
            {
                if (!Collection.Contains(Item))
                {
                    Collection.Add(Item);
                }
            }
            else if (Action == CollectionAction.Remove)
            {
                if (Collection.Contains(Item))
                {
                    Collection.Remove(Item);
                }
            }
        }
    }
}