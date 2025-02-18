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

        public ObservableCollection<JobViewModel> Jobs { get; }
        public MainWindow()
        {
            InitializeComponent();
            Jobs = new ObservableCollection<JobViewModel>();
            DataContext = this;
            AddTestJob();   
        }

        private void AddTestJob()
        {
            Jobs.Add(new JobViewModel(Jobs) { Name = "Job 1" });
            Jobs.Add(new JobViewModel(Jobs) { Name = "Job 2" });
            Jobs.Add(new JobViewModel(Jobs) { Name = "Job 3" });
            Jobs.Add(new JobViewModel(Jobs) { Name = "Job 4" });
            Jobs.Add(new JobViewModel(Jobs) { Name = "Job 5" });
            Jobs.Add(new JobViewModel(Jobs) { Name = "Job 6" });
            Jobs.Add(new JobViewModel(Jobs) { Name = "Job 7" });
            Jobs.Add(new JobViewModel(Jobs) { Name = "Job 8" });
            Jobs.Add(new JobViewModel(Jobs) { Name = "Job 9" });
            Jobs.Add(new JobViewModel(Jobs) { Name = "Job 10" });

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