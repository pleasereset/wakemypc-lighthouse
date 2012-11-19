using System;
using System.Windows;

namespace Agent
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                DataContext = MainViewModel.Instance;
            }
            catch (Exception ex)
            {
                // Mayday, could not initalize the agent application, we're going down !
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown(-1);
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.Instance.StartServer();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.Instance.StopServer();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.Instance.SaveSettings();
        }
    }
}
