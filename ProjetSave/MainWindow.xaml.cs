using ProjetSave.Controller;
using ProjetSave.ViewModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProjetSave
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<JobViewModel> Jobs { get; }

        public BackupManager BackupManager { get; }
        public MainWindow()
        {
            InitializeComponent();
            Jobs = new ObservableCollection<JobViewModel>();
            BackupManager = new BackupManager(new Service.Logger("logfile.json"));
            DataContext = this;

            // Appliquer la langue sauvegardée
            SetLanguage(Properties.Settings.Default.Language);
        }

        

        private void ConfigureBackup_Click(object sender, RoutedEventArgs e)
        {
            Configuration configWindow = new Configuration();
            configWindow.ShowDialog();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Changer la langue et enregistrer
        private void SetLanguage(string culture)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);

            Properties.Settings.Default.Language = culture;
            Properties.Settings.Default.Save();

            ReloadUI();
        }

        private void SetLanguage_English(object sender, RoutedEventArgs e)
        {
            SetLanguage("en");
        }

        private void SetLanguage_French(object sender, RoutedEventArgs e)
        {
            SetLanguage("fr");
        }

        // Mise à jour des textes
        private void ReloadUI()
        {
            foreach (var control in FindVisualChildren<MenuItem>(this))
            {
                if (control.Header.ToString() == "Settings") control.Header = Properties.Resources.Settings;
                if (control.Header.ToString() == "Configure Backup") control.Header = Properties.Resources.ConfigureBackup;
                if (control.Header.ToString() == "Language") control.Header = Properties.Resources.Language;
                if (control.Header.ToString() == "English") control.Header = Properties.Resources.English;
                if (control.Header.ToString() == "Français") control.Header = Properties.Resources.Français;
            }

            foreach (var control in FindVisualChildren<Button>(this))
            {
                if (control.Content.ToString() == "Add Job") control.Content = Properties.Resources.AddJob;
                if (control.Content.ToString() == "Start") control.Content = Properties.Resources.Start;
                if (control.Content.ToString() == "Pause") control.Content = Properties.Resources.Pause;
                if (control.Content.ToString() == "Stop") control.Content = Properties.Resources.Stop;
            }
        }

        // Récupérer tous les éléments d'UI
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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
