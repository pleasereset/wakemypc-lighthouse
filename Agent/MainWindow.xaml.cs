using System;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Data;
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

	public class IconExtractor
	{

		public static Icon Extract(string file, int number, bool largeIcon)
		{
			IntPtr large;
			IntPtr small;
			ExtractIconEx(file, number, out large, out small, 1);
			try
			{
				return Icon.FromHandle(largeIcon ? large : small);
			}
			catch
			{
				return null;
			}

		}
		[DllImport("Shell32.dll", EntryPoint = "ExtractIconExW", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		private static extern int ExtractIconEx(string sFile, int iIndex, out IntPtr piLargeVersion, out IntPtr piSmallVersion, int amountIcons);

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
