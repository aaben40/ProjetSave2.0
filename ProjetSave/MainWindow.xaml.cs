

using ProjetSave.Controller;
using ProjetSave.ViewModel;

﻿
using ProjetSave.Service;

using System.Collections.Generic;


﻿using ProjetSave.Controller;
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
using System.Globalization;

namespace ProjetSave
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


      
        //public ICommand StartCommand { get; }
        
        public MainViewModel ViewModel { get; } = new MainViewModel();  


        public BackupManager BackupManager { get; }
        public ObservableCollection<JobViewModel> Jobs { get; private set; }
        private Logger logger;  // Ajout d'un membre Logger
        DispatcherTimer processCheckTimer;

        public MainWindow()
        {
            InitializeComponent();
            InitializeProcessCheckTimer();
            Jobs = new ObservableCollection<JobViewModel>();


             

            
            DataContext = ViewModel;
            TextBox? logTextBox = this.FindName("logTextBox") as TextBox;
            if (logTextBox != null)
            {
                Console.SetOut(new TextBoxOutputter(logTextBox, Dispatcher));
            }
            // Appliquer la langue sauvegardée
            //SetLanguage(Properties.Settings.Default.Language);
            //StartCommand = new RelayCommand(param => BackupManager.ExecuteAllJobs(Jobs));

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

            //Configuration configWindow = new Configuration(ViewModel);
            //configWindow.ShowDialog(); // Utilisez ShowDialog pour une fenêtre modale si nécessaire

            Configuration configWindow = new Configuration();
            configWindow.ShowDialog(); // Utilisez ShowDialog pour une fenêtre modale si nécessaire

            //Configuration configWindow = new Configuration();
            //configWindow.ShowDialog();

        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Configuration configWindow = new Configuration();
            configWindow.ShowDialog();
        }

       

        private void logTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {

        }

        /*private void logTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Cette méthode peut rester vide si elle n'est pas utilisée
        }*/


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

        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject

        {

            if (depObj != null)

            {

                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)

                {

                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);

                    if (child is T obj)

                    {

                        yield return obj;

                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))

                    {

                        yield return childOfChild;

                    }

                }

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
        public void ReloadUI()
        {
            foreach (var control in FindVisualChildren<MenuItem>(this))
            {
                if (control.Header is string headerText)
                {
                    if (headerText == "Settings") control.Header = Properties.Resources.Settings;
                    if (headerText == "Configure Backup") control.Header = Properties.Resources.ConfigureBackup;
                    if (headerText == "Language") control.Header = Properties.Resources.Language;
                    if (headerText == "English") control.Header = Properties.Resources.English;
                    if (headerText == "Français") control.Header = Properties.Resources.Français;
                }
            }

            foreach (var control in FindVisualChildren<Button>(this))
            {
                if (control.Content is string contentText)
                {
                    if (contentText == "Add Job") control.Content = Properties.Resources.AddJob;
                    if (contentText == "Start") control.Content = Properties.Resources.Start;
                    if (contentText == "Pause") control.Content = Properties.Resources.Pause;
                }
            }
        }
        public void SetLanguage(string culture)

        {

            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);

            // Sauvegarde de la langue sélectionnée

            Properties.Settings.Default.Langue = culture;

            Properties.Settings.Default.Save();

            // Mise à jour de l'UI

            ReloadUI();

        }

        public void SetLanguage_English(object sender, RoutedEventArgs e)

        {

            SetLanguage("en");

        }

        public void SetLanguage_French(object sender, RoutedEventArgs e)

        {

            SetLanguage("fr");

        }


    }
}
