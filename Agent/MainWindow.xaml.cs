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
            DataContext = MainViewModel.Instance;

            MinimizeToTray.Enable(this);

            if (MainViewModel.Instance.Autostart)
            {
                this.WindowState = System.Windows.WindowState.Minimized;
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


    }
}
