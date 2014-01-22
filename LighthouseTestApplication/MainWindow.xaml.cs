using ree7.WakeMyPC.LighthouseCore;
using ree7.WakeMyPC.LighthouseService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LighthouseTestApplication
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		IConfiguration currentConfiguration;
		Server currentServer;

		public MainWindow()
		{
			InitializeComponent();

			// Redirect ProbeServer's logs to the service's EventLog
			ree7.WakeMyPC.LighthouseCore.Utils.Log.DefineMethod(LogOutlet);
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			currentConfiguration = new StaticConfiguration(33366, "changeme");
			StartServer();
		}

		private void LogOutlet(string message)
		{
			this.Dispatcher.BeginInvoke((ThreadStart)delegate()
			{
				Console.AppendText(message + Environment.NewLine);
				Console.ScrollToEnd();
			});
		}

		private void StartServer()
		{
			if(currentServer != null)
			{
				StopServer();
			}

			currentServer = new Server(currentConfiguration.Port, currentConfiguration.Password);
			currentServer.Start();
		}

		private void StopServer()
		{
			currentServer.Stop();
			currentServer = null;
		}
	}
}
