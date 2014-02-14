using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace ree7.WakeMyPC.Agent
{
	public class UpdateWindowData
	{
		public string CurrentVersion { get; set; }
		public string NewVersion { get; set; }
		public Uri NewVersionUri { get; set; }
	}

	public partial class UpdateWindow : Window
	{
		/// <summary>
		/// JSON file hosted as a public GIST on my personal account
		/// </summary>
		const string LHUpdateDataUrl = "https://gist.github.com/pleasereset/9005368/raw/";

		public UpdateWindow()
		{
			InitializeComponent();
			Loaded += (p1, p2) => { CheckForUpdates(); };
		}

		private void CheckForUpdates()
		{
			WebClient client = new WebClient();
			client.DownloadStringCompleted += client_DownloadStringCompleted;
			client.DownloadStringAsync(new Uri(LHUpdateDataUrl));
		}

		private void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
		{
			if(e.Error == null)
			{
				try
				{
					JObject json = JObject.Parse(e.Result);
					string latestVString = (string)json["win"]["version"];
					string latestVUrl = (string)json["win"]["url"];

					Version current = Assembly.GetExecutingAssembly().GetName().Version;
					Version latest = Version.Parse(latestVString);

					if(latest > current)
					{
						// An update is available
						this.DataContext = new UpdateWindowData()
						{
							CurrentVersion = current.ToString(),
							NewVersion = latest.ToString(),
							NewVersionUri = new Uri(latestVUrl, UriKind.Absolute)
						};

						LoadingStateGrid.Visibility = Visibility.Collapsed;
						UpdateAvailableStateGrid.Visibility = Visibility.Visible;
					}
					else
					{
						// The software is up-to date
						this.DataContext = new UpdateWindowData()
						{
							CurrentVersion = current.ToString(),
						};

						LoadingStateGrid.Visibility = Visibility.Collapsed;
						NoUpdateAvailableStateGrid.Visibility = Visibility.Visible;
					}
				}
				catch(Exception ex)
				{
					OnException(ex);
				}
			}
			else
			{
				OnException(e.Error);
			}
		}

		private void OnException(Exception e)
		{
			string msg = "An error occurred while checking for updates." + Environment.NewLine + "Details " + e.ToString();
			MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			this.Close();
		}

		private void OnNewVersionHyperlinkClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			// The textblock should contain the URL to open
			TextBlock src = (TextBlock)sender;
			Process.Start(src.Text);
		}

		private void OnCloseClick(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
