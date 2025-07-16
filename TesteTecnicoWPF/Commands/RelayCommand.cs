using System;
using System.Windows.Input;

namespace TesteTecnicoWPF.Commands
{
    /// <summary>
    /// Uma implementação genérica de ICommand para rotear comandos para os ViewModels.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        /// <summary>
        /// Evento que é disparado quando o resultado de CanExecute é alterado.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Construtor para comandos que sempre podem ser executados.
        /// </summary>
        /// <param name="execute">A lógica de execução.</param>
        public RelayCommand(Action<object> execute) : this(execute, null)
        {
        }

        /// <summary>
        /// Construtor principal.
        /// </summary>
        /// <param name="execute">A lógica de execução.</param>
        /// <param name="canExecute">A lógica que determina se o comando pode executar.</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// Verifica se o comando pode ser executado no estado atual.
        /// </summary>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        /// <summary>
        /// Executa a lógica do comando.
        /// </summary>
        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}