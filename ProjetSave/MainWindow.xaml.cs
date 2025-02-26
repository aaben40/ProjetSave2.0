using ProjetSave.Controller;
using ProjetSave.ViewModel;
using ProjetSave.Service; // Assurez-vous d'ajouter l'espace de nom pour Logger
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Threading;
using ProjetSave.Model;

namespace ProjetSave
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public BackupManager BackupManager { get; }
        public ObservableCollection<JobViewModel> Jobs { get; private set; }
        private Logger logger;  // Ajout d'un membre Logger
        DispatcherTimer processCheckTimer;
        public MainWindow()
        {
            InitializeComponent();
            InitializeProcessCheckTimer();
            Jobs = new ObservableCollection<JobViewModel>();
            logger = new Logger("C:\\Users\\Utilisateur\\source\\repos\\ProjetSave2.0\\LOGS\\logfile.json");  // Configurez votre chemin de log ici
            BackupManager = new BackupManager(logger);
            DataContext = this;

            // Abonnement à l'événement de mise à jour du log
            logger.LogUpdated += Logger_LogUpdated;

            //test de log
            //logger.logAction(new LogEntry { BackupName = "Test", SourceFilePath = "C:\\Users\\Utilisateur\\source\\repos\\ProjetSave2.0\\CESI", TargetFilePath = "C:\\C:\\Users\\Utilisateur\\source\\repos\\ProjetSave2.0\\NON" });

            string testInputFile = @"C:\Users\Utilisateur\source\repos\ProjetSave2.0\CESI\test.txt";
            string testOutputFile = @"C:\Users\Utilisateur\source\repos\ProjetSave2.0\NON\test.aes";

            BackupJob job = new BackupJob("TestJob", @"C:\Users\Utilisateur\source\repos\ProjetSave2.0\CESI", @"C:\Users\Utilisateur\source\repos\ProjetSave2.0\NON", BackupType.Full);
            job.EncryptFile(testInputFile, testOutputFile);
        }

        private void Logger_LogUpdated(string logMessage)
        {
            // Mettre à jour la TextBox sur le thread UI
            Dispatcher.Invoke(() => {
                logTextBox.AppendText(logMessage + Environment.NewLine);  // Assurez-vous que logTextBox est le nom correct dans votre XAML
                logTextBox.ScrollToEnd();
            });
        }

        private void AddJob(JobViewModel job)
        {
            Jobs.Add(job);
        }

        private void ConfigureBackup_Click(object sender, RoutedEventArgs e)
        {
            Configuration configWindow = new Configuration();
            configWindow.ShowDialog(); // Utilisez ShowDialog pour une fenêtre modale si nécessaire
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void logTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Cette méthode peut rester vide si elle n'est pas utilisée
        }


        private void InitializeProcessCheckTimer()
        {
            processCheckTimer = new DispatcherTimer();
            processCheckTimer.Interval = TimeSpan.FromSeconds(5); // Vérifie toutes les 5 secondes
            processCheckTimer.Tick += ProcessCheckTimer_Tick;
            processCheckTimer.Start();
        }

        private void ProcessCheckTimer_Tick(object sender, EventArgs e)
        {
            if (IsProcessRunning("calc")) // "calc" est le nom du processus pour la calculatrice
            {
                processCheckTimer.Stop(); // Arrête le timer
                this.Close(); // Ferme l'application
            }
        }

        private bool IsProcessRunning(string processName)
        {
            foreach (Process proc in Process.GetProcesses())
            {
                if (proc.ProcessName.ToLower().Contains(processName.ToLower()))
                    return true;
            }
            return false;
        }

        private void EncryptCheckBox_Checked(object sender, RoutedEventArgs e)
        {
        }

    }
}
