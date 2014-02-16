using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Input;

namespace ree7.WakeMyPC.Agent
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
			Dispatcher.BeginInvoke(new Action(() =>
			{
				LoadMac();

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
			}), System.Windows.Threading.DispatcherPriority.ApplicationIdle);
		}

		private void LoadMac()
		{
			MacList.Items.Clear();

			foreach(NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
			{
				if (IsWOLType(nic.NetworkInterfaceType))
				{
					NetworkInterfaceItem i = new NetworkInterfaceItem()
					{
						MAC = FormatMAC(nic.GetPhysicalAddress()),
						Name = nic.Name,
						Type = nic.NetworkInterfaceType.ToString()
					};
					MacList.Items.Add(i);
				}
			}
		}

		private bool IsWOLType(NetworkInterfaceType t)
		{
			return t == NetworkInterfaceType.Ethernet
				|| t == NetworkInterfaceType.Ethernet3Megabit
				|| t == NetworkInterfaceType.FastEthernetFx
				|| t == NetworkInterfaceType.FastEthernetT
				|| t == NetworkInterfaceType.GigabitEthernet;
		}

		private string FormatMAC(PhysicalAddress a)
		{
			byte[] mac = a.GetAddressBytes();
			return String.Join("-", (from b in mac select b.ToString("x2"))).ToUpper();
		}

		private void SettingsBoxes_KeyUp(object sender, KeyEventArgs e)
		{
			tbPassword.Text = tbPassword.Text.Replace('/', '_');
			MainViewModel.Instance.SettingsModified();
		}

		#region Menu
		private void TrayMenuExit_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown(0);
		}
		#endregion
	}

	public class NetworkInterfaceItem
	{
		public string MAC { get; set; }
		public string Name { get; set; }
		public string IP { get; set; }
		public string Type { get; set; }

		public object Icon
		{
			get
			{
				return null;
			}
		}
	}
}
