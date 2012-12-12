using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

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

        private void SettingsBoxes_KeyUp(object sender, KeyEventArgs e)
        {
            MainViewModel.Instance.SettingsModified();
        }       
    }

    public class BusyStateBoolCursorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                bool b = (bool)value;
                return b ? Cursors.Wait : Cursors.Arrow;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
