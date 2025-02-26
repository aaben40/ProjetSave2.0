
using ProjetSave.Controller;
using ProjetSave.ViewModel;

﻿
using ProjetSave.Service;

using System.Collections.Generic;

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

namespace ProjetSave
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public ObservableCollection<JobViewModel> Jobs { get; }
        //public ICommand StartCommand { get; }
        public BackupManager BackupManager { get; }
        public MainViewModel ViewModel { get; } = new MainViewModel();  

        public MainWindow()
        {
            InitializeComponent();
            Jobs = new ObservableCollection<JobViewModel>();

             

            BackupManager = new BackupManager(new Service.Logger("logfile.json"));
            DataContext = ViewModel;
            TextBox? logTextBox = this.FindName("logTextBox") as TextBox;
            if (logTextBox != null)
            {
                Console.SetOut(new TextBoxOutputter(logTextBox, Dispatcher));
            }
            // Appliquer la langue sauvegardée
            //SetLanguage(Properties.Settings.Default.Language);
            //StartCommand = new RelayCommand(param => BackupManager.ExecuteAllJobs(Jobs));

        }

       

        private void ConfigureBackup_Click(object sender, RoutedEventArgs e)
        {
            Configuration configWindow = new Configuration(ViewModel);
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
            Configuration configWindow = new Configuration(ViewModel);
            configWindow.ShowDialog();
        }

       

        private void logTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}