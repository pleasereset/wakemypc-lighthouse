using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace ree7.WakeMyPC.Agent
{
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
			UpdateChecker checker = new UpdateChecker();
			checker.Completed += (src, args) =>
			{
				if(args.Error == null)
				{
					this.DataContext = args;

					if(args.UpdateAvailable)
					{
						LoadingStateGrid.Visibility = Visibility.Collapsed;
						UpdateAvailableStateGrid.Visibility = Visibility.Visible;
					}
					else
					{
						LoadingStateGrid.Visibility = Visibility.Collapsed;
						NoUpdateAvailableStateGrid.Visibility = Visibility.Visible;
					}
				}
				else
				{
					OnException(args.Error);
				}
			};
			checker.Start();
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
