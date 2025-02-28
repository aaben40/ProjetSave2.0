using ProjetSave.Controller;
using ProjetSave.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProjetSave.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler? PropertyChanged;
        public ObservableCollection<JobViewModel> Jobs { get; }

        public BackupManager BackupManager { get; }


        // Utilisez cette méthode pour notifier la vue des changements de propriété
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private bool isJobRunning = false;
        private bool isJobPaused = false;

        public bool IsJobRunning
        {
            get => isJobRunning;
            set
            {
                if (isJobRunning != value)
                {
                    isJobRunning = value;
                    OnPropertyChanged(nameof(IsJobRunning));
                    OnPropertyChanged(nameof(CanStart));
                    OnPropertyChanged(nameof(CanTogglePause));
                    OnPropertyChanged(nameof(CanStop));
                    OnPropertyChanged(nameof(PauseButtonText));
                }
            }
        }

        public bool IsJobPaused
        {
            get => isJobPaused;
            set
            {
                if (isJobPaused != value)
                {
                    isJobPaused = value;
                    OnPropertyChanged(nameof(IsJobPaused));
                    OnPropertyChanged(nameof(PauseButtonText));
                }
            }
        }

        public bool CanStart => !IsJobRunning;
        public bool CanTogglePause => IsJobRunning;
        public bool CanStop => IsJobRunning;

        public string PauseButtonText => IsJobPaused ? "Reprendre" : "Pause";

        public ICommand StartCommand { get; }
        public ICommand TogglePauseCommand { get; }
        public ICommand StopCommand { get; }

        public MainViewModel()
        {
            // Assurez-vous que ces méthodes sont adaptées pour accepter un paramètre object, même si vous ne l'utilisez pas.
            Jobs = new ObservableCollection<JobViewModel>();
            BackupManager = new BackupManager(new Logger("logg.json"));
            StartCommand = new RelayCommand(_ => ExecuteJobs(), _ => CanStartt());
            TogglePauseCommand = new RelayCommand(_ => TogglePause(), _ => CanPause());
            StopCommand = new RelayCommand(_ => StopJobs(), _ => CanStopp());
        }

        private void ExecuteJobs()
        {
            IsJobRunning = true;
            // Votre logique pour démarrer les jobs
            BackupManager.ExecuteAllJobs(Jobs);
            OnPropertyChanged(nameof(CanStart));
            OnPropertyChanged(nameof(CanTogglePause));
            OnPropertyChanged(nameof(CanStop));

        }

        private bool CanStartt()
        {
            return !IsJobRunning;  // Déterminez si la commande peut être exécutée
        }

        private void TogglePause()
        {
            IsJobPaused = !IsJobPaused;
            // Votre logique pour mettre en pause ou reprendre
            // Votre logique pour mettre en pause ou reprendre
            if (IsJobPaused)
            {
                // Logique pour mettre en pause
                BackupManager.PauseAllJobs();
            }
            else
            {
                // Logique pour reprendre
                BackupManager.ResumeAllJobs();
            }
        }

        private bool CanPause()
        {
            return IsJobRunning;  // Déterminez si la commande pause peut être exécutée
        }

        private void StopJobs()
        {
            Console.WriteLine("Stopping all jobs");
            IsJobRunning = false;
            BackupManager.StopAllJobs(Jobs);
            OnPropertyChanged(nameof(CanStart));
            OnPropertyChanged(nameof(CanTogglePause));
            OnPropertyChanged(nameof(CanStop));
        }

        private bool CanStopp()
        {
            return IsJobRunning;  // Déterminez si la commande stop peut être exécutée
        }
    }

}
