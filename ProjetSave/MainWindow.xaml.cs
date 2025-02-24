using ProjetSave.Controller;
using ProjetSave.ViewModel;
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
        public BackupManager BackupManager { get; }
        public ObservableCollection<JobViewModel> Jobs { get; }
        public MainWindow()
        {
            InitializeComponent();
            Jobs = new ObservableCollection<JobViewModel>();
            BackupManager = new BackupManager(new Service.Logger("logfile.json"));
            DataContext = this;
           
            
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
    }
}