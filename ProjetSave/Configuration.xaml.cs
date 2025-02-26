using ProjetSave.Model;
using ProjetSave.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProjetSave
{
    /// <summary>
    /// Logique d'interaction pour Window1.xaml
    /// </summary>
    public partial class Configuration : Window
    {

       

        private ViewModel.MainViewModel mainViewModel;

        public Configuration(ViewModel.MainViewModel viewModel)
        {
            InitializeComponent();
            mainViewModel = viewModel;
            
           

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow == null)
            {
                MessageBox.Show("Main window not available"); return;
            }
            if (backupTypeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a backup type"); return;
            }

            // Créer un nouveau JobViewModel avec les informations saisies
            BackupJob newBackupJob = new BackupJob(
                JobNameTextbox.Text,
                sourceTextBox.Text,
                targetTextBox.Text,
                (BackupType)Enum.Parse(typeof(BackupType), ((ComboBoxItem)backupTypeComboBox.SelectedItem).Content.ToString())
            )
            {
                IsEncrypted = encryptCheckBox.IsChecked ?? false
            };


            // Créer un nouveau JobViewModel avec les informations saisies
            JobViewModel newJob = new JobViewModel(mainWindow.BackupManager, newBackupJob, mainWindow.Jobs)

           

            // Créer un nouveau JobViewModel avec les informations saisies
            

            {
                Name = JobNameTextbox.Text,
                sourceDirectory = sourceTextBox.Text,
                targetDirectory = targetTextBox.Text,

                backupType = (BackupType)Enum.Parse(typeof(BackupType), ((ComboBoxItem)backupTypeComboBox.SelectedItem).Content.ToString()),
                isEncrypted = encryptCheckBox.IsChecked ?? false
                
            };

            // Ajouter le nouveau job à la collection dans MainViewModel
            //mainWindow.Jobs.Add(newJob);
            mainViewModel.Jobs.Add(newJob);

            // Fermer la fenêtre de configuration
            this.Close();

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
