
﻿using ProjetSave.Model;
using ProjetSave.ViewModel;

﻿using Microsoft.Win32;

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
using System.Windows.Forms;
using System.IO;
using ProjetSave.Model;
using ProjetSave.ViewModel;

namespace ProjetSave
{
    /// <summary>
    /// Logique d'interaction pour Window1.xaml
    /// </summary>
    public partial class Configuration : Window
    {



            
           


        private BackupJob _backupJob;

        public Configuration(BackupJob job)
        {
            InitializeComponent();
            _backupJob = job;
        }

        private ViewModel.BackupJobViewModel ViewModel;

        public Configuration()
        {
            InitializeComponent();
            ViewModel = new ViewModel.BackupJobViewModel();

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

                IsEncrypted = EncryptCheckBox.IsChecked ?? false

            };


            // Créer un nouveau JobViewModel avec les informations saisies

            JobViewModel newJob = new JobViewModel(mainWindow.BackupManager, newBackupJob, mainWindow.Jobs)
            {
                Name = JobNameTextbox.Text,

                sourceDirectory = sourceTextBox.Text,
                targetDirectory = targetTextBox.Text,

                backupType = (BackupType)Enum.Parse(typeof(BackupType), ((ComboBoxItem)backupTypeComboBox.SelectedItem).Content.ToString()),
                isEncrypted = EncryptCheckBox.IsChecked ?? false
                
            };

            // Ajouter le nouveau job à la collection dans MainViewModel
            mainWindow.Jobs.Add(newJob);
            //mainViewModel.Jobs.Add(newJob);

            // Fermer la fenêtre de configuration
            this.Close();


          
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {

        }

        // ...

        private void BrowseSource_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All files (*.*)|*.*"; // Modifier si nécessaire pour filtrer certains types de fichiers
            if (openFileDialog.ShowDialog() == true)
            {
                sourceTextBox.Text = openFileDialog.FileName;
            }
        }

        private void BrowseTarget_Click(object sender, RoutedEventArgs e)
        {
            //using (var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog())
            //{
            //    if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //    {
            //        targetTextBox.Text = folderBrowserDialog.SelectedPath;
            //    }
            //}
        }

        // Exemple de code, supposant que vous avez une instance de BackupJob nommée 'currentJob'


        private void EncryptCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        private void EncryptCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
