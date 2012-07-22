using System;
using System.Collections.Generic;
using System.Linq;
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
using ree7.WakeMyPC.ProbeServer;
using System.Windows.Forms;
using System.Drawing;

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
