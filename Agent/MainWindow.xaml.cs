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

namespace Agent
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Server server;

        public MainWindow()
        {
            InitializeComponent();

            server = new Server(39002, "test");
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            server.Start();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            server.Stop();
        }


    }
}
