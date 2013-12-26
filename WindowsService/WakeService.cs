using ree7.WakeMyPC.ProbeServer;
using System.Diagnostics;
using System.ServiceProcess;

namespace ree7.WakeMyPC.WindowsService
{
	public partial class WakeService : ServiceBase
	{
		IConfiguration currentConfiguration;
		Server currentServer;

		public const string SvcName = "Wake my PC Lighthouse";
		public const string SvcDescription = "Allows remote applications to know if the computer is running and to power it down.";
		
		public WakeService()
		{
			InitializeComponent();

#if DEBUG
			// Redirect ProbeServer's logs to the service's EventLog
			ree7.WakeMyPC.ProbeServer.Utils.Log.DefineMethod(LogOutlet);
#endif
		}

		protected override void OnStart(string[] args)
		{
			currentConfiguration = new RegistryConfiguration(LogOutlet); //new StaticConfiguration(33666, "toto");
			StartServer();
		}

		protected override void OnStop()
		{
			StopServer();
		}

		public static void Main()
		{
			System.ServiceProcess.ServiceBase.Run(new WakeService());
		}

		private void LogOutlet(string message)
		{
#if DEBUG
			this.EventLog.WriteEntry(message);
#endif
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
