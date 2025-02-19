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
<<<<<<< HEAD
using ProjetSave.Controller;
using System.ServiceModel.Channels;
using System.Windows;
=======
>>>>>>> my badversion stable traductionJob

namespace ProjetSave.ViewModel
{
    public class JobViewModel : INotifyPropertyChanged
    {
        private string name = "";
        private string sourceDirectory = "";
        private string targetDirectory = "";
        private BackupType backupType;
        private bool isEncrypted = false;
        public event PropertyChangedEventHandler? PropertyChanged;
        


        // Propriétés
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public string SourceDirectory
        {
            get => sourceDirectory;
            set => SetProperty(ref sourceDirectory, value);
        }

        public string TargetDirectory
        {
            get => targetDirectory;
            set => SetProperty(ref targetDirectory, value);
        }

        public BackupType BackupType
        {
            get => backupType;
            set => SetProperty(ref backupType, value);
<<<<<<< HEAD
        }

        public bool IsEncrypted
        {
            get => isEncrypted;
            set => SetProperty(ref isEncrypted, value);
        }

=======
        }

        public bool IsEncrypted
        {
            get => isEncrypted;
            set => SetProperty(ref isEncrypted, value);
        }

>>>>>>> my badversion stable traductionJob
        // Commandes pour exécuter et supprimer le job
        public ICommand ExecuteCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }

        // Référence à la collection parente pour pouvoir se supprimer
        private ObservableCollection<JobViewModel> parentCollection;
        private BackupManager backupManager;
        private BackupJob backupJob;

        public JobViewModel(BackupManager manager, BackupJob job, ObservableCollection<JobViewModel> parent)
        {
            this.backupManager = manager;
            this.backupJob = job;
            this.parentCollection = parent;
            ExecuteCommand = new RelayCommand(param => ExecuteJob());
            DeleteCommand = new RelayCommand(param => DeleteJob());

<<<<<<< HEAD
        }

       

=======
        private void ExecuteJob()
        {
            // Logique pour exécuter le job
            Console.WriteLine($"Executing job: {Name}, Source: {SourceDirectory}, Target: {TargetDirectory}, Type: {BackupType}, Encrypted: {IsEncrypted}");
        }

>>>>>>> my badversion stable traductionJob
        private void DeleteJob()
        {
            parentCollection.Remove(this);
            Console.WriteLine($"Deleted job: {Name}");
        }

        // Méthode générique pour notifier les changements de propriétés
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
<<<<<<< HEAD

        private void ExecuteJob()
        {
            backupManager.ExecuteJob(backupJob);

        }
=======
>>>>>>> my badversion stable traductionJob
    }


}
