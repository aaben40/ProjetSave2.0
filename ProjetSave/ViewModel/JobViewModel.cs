using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using ProjetSave.Service;
using System.Collections.ObjectModel;

namespace ProjetSave.ViewModel
{
    public class JobViewModel : INotifyPropertyChanged
    {
        
        private string name = "";
        public event PropertyChangedEventHandler? PropertyChanged;

        // Propriété pour le nom du job
        public string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        // Commandes pour Exécuter et Supprimer le job
        public ICommand ExecuteCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }

        // Référence à la collection parente pour pouvoir se supprimer
        private ObservableCollection<JobViewModel> parentCollection;

        public JobViewModel(ObservableCollection<JobViewModel> parent)
        {
            parentCollection = parent;
            ExecuteCommand = new RelayCommand(param => ExecuteJob());
            DeleteCommand = new RelayCommand(param => DeleteJob());
        }

        // Méthode pour exécuter le job
        private void ExecuteJob()
        {
            // Logique pour exécuter le job
            Console.WriteLine($"Executing job: {Name}");
        }

        // Méthode pour supprimer le job de la collection parente
        private void DeleteJob()
        {
            parentCollection.Remove(this);
            Console.WriteLine($"Deleted job: {Name}");
        }

        // Méthode pour notifier les changements de propriétés
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

       
    }
}
