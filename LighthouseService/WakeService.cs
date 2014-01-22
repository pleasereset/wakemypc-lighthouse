using ree7.WakeMyPC.LighthouseCore;
using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace ree7.WakeMyPC.LighthouseService
{
	public partial class WakeService : ServiceBase
	{
		bool useLogs;
		IConfiguration currentConfiguration;
		Server currentServer;

		public const string SvcName = "Wake my PC Lighthouse";
		public const string SvcDescription = "Allows the phone application to know if the computer is running and to power it down.";
		
		public WakeService(bool useLogs)
		{
			InitializeComponent();
			CanStop = true;

			// Redirect the core logs to the service's EventLog
			ree7.WakeMyPC.LighthouseCore.Utils.Log.DefineMethod(LogOutlet);

			this.useLogs = useLogs;
#if DEBUG
			this.useLogs = true;
#endif
		}

		public static int Main(string[] args)
		{
			// Command line args :
			// -i	Install service
			// -u	Uninstall service
			// -d	Output debug logs in the Console & Service event log
			// -?	Display help

			bool useLogs = false;

			Console.WriteLine("Hey");

			try
			{
				foreach(string arg in args)
				{
					Console.WriteLine("arg: " + arg);

					switch(arg)
					{
						case "-?":
						case "--help":
							OnCLIHelp();
							return 0;
						case "-i" :
						case "--install":
							OnInstall(args);
							return 0;
						case "-u":
						case "--uninstall":
							OnUninstall(args);
							return 0;
						case "-d":
						case "--debug":
							useLogs = true;
							break;
						default: break;
					}
				}

				
				System.ServiceProcess.ServiceBase.Run(new WakeService(useLogs));

				return 0;
			}
			catch(Exception e)
			{
				Console.WriteLine(e.ToString());
				return -1;
			}
		}

		private static void OnCLIHelp()
		{
			Console.WriteLine("Wake my PC Lighthouse service");
			Console.WriteLine("=============================");
			Console.WriteLine("USAGE :");
			Console.WriteLine(" -?");
			Console.WriteLine(" --help		: Display this help");
			Console.WriteLine(" -d");
			Console.WriteLine(" --debug		: Activates debug logging in Windows EventLog");
			Console.WriteLine(" -i");
			Console.WriteLine(" --install	: Install the service");
			Console.WriteLine(" -u");
			Console.WriteLine(" --uninstall	: Uninstall the service");
		}

		private static void OnInstall(string[] args)
		{
			ProjectInstaller.Install(false, args);
		}

		private static void OnUninstall(string[] args)
		{
			ProjectInstaller.Install(true, args);
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

		private void LogOutlet(string message)
		{
			if (useLogs)
			{
				Console.WriteLine(message);
				if (this.EventLog != null) this.EventLog.WriteEntry(message);
			}
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
