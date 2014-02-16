using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Reflection;

namespace ree7.WakeMyPC.Agent
{
	public class UpdateAvailableEventArgs : EventArgs
	{
		public Exception Error { get; set; }

		public bool UpdateAvailable { get; set; }

		public string CurrentVersion { get; set; }
		public string NewVersion { get; set; }
		public Uri NewVersionUri { get; set; }
	}

	public class UpdateChecker
	{
		/// <summary>
		/// JSON file hosted as a public GIST on my personal account
		/// </summary>
		const string LHUpdateDataUrl = "https://gist.github.com/pleasereset/9005368/raw/";

		public event EventHandler<UpdateAvailableEventArgs> Completed;

		public void Start()
		{
			WebClient client = new WebClient();
			client.DownloadStringCompleted += client_DownloadStringCompleted;
			client.DownloadStringAsync(new Uri(LHUpdateDataUrl));
		}

		private void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
		{
			if (e.Error == null)
			{
				try
				{
					JObject json = JObject.Parse(e.Result);
					Version current = Assembly.GetExecutingAssembly().GetName().Version;
					Version latest = Version.Parse((string)json["win"]["version"]);
					Uri latestUrl = new Uri((string)json["win"]["url"], UriKind.Absolute);

					if(Completed != null)
					{
						Completed(this, new UpdateAvailableEventArgs()
							{
								UpdateAvailable = latest > current,
								CurrentVersion = current.ToString(),
								NewVersion = latest.ToString(),
								NewVersionUri = latestUrl
							});
					}
				}
				catch (Exception ex)
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
			if(Completed != null)
			{
				Completed(this, new UpdateAvailableEventArgs()
					{
						Error = e,
						UpdateAvailable = false
					});
			}
		}
	}
}
