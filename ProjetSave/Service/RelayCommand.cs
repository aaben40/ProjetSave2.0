using System;
using System.Windows.Input;

namespace ProjetSave.Service
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> execute;
        private readonly Func<object, bool> canExecute;

        // Constructeur
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException("execute");
            this.canExecute = canExecute;
        }

        // Déterminer si la commande peut être exécutée
        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        // Exécution de la commande
        public void Execute(object parameter)
        {
            execute(parameter);
        }

        // Événement déclenché lorsque les conditions d'exécution changent
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
