using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using ProjetSave.Service;
using System.Collections.ObjectModel;
using ProjetSave.Model;
using ProjetSave.Controller;

namespace ProjetSave.ViewModel
{
    public class JobViewModel : INotifyPropertyChanged
    {

        
        private string name = "";
        public event PropertyChangedEventHandler? PropertyChanged;
        public string sourceDirectory = "";
        public string targetDirectory = "";
        public BackupType backupType;
        public bool isEncrypted = false;
      



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
        private BackupManager backupManager;
        private BackupJob backupJob;


        //public JobViewModel(ObservableCollection<JobViewModel> parent);
        private int? priority;
        public int? Priority
        {
            get => priority;
            set
            {
                if (priority != value)
                {
                    backupJob.Priority = value;
                    priority = value;
                    Console.WriteLine($"Priority changed to {priority}");
                    OnPropertyChanged(nameof(Priority));
                }
            }
        }

        public JobViewModel(BackupManager manager, BackupJob job, ObservableCollection<JobViewModel> parent)

        {
            this.backupManager = manager;
            this.backupJob = job;
            this.parentCollection = parent;
            parentCollection = parent;
            ExecuteCommand = new RelayCommand(param => ExecuteJob());
            DeleteCommand = new RelayCommand(param => DeleteJob());
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

        // Méthode utilitaire pour setter les propriétés
        protected void SetProperty<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }


        private void ExecuteJob()
        {
            try
            {
                backupManager.ExecuteJob(backupJob);
                parentCollection.Remove(this);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                
            }
           

        }
        


    }
}
