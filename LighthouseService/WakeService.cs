using ree7.WakeMyPC.LighthouseCore;
using System;
using System.Diagnostics;
using System.Security.Principal;
using System.ServiceProcess;

namespace ree7.WakeMyPC.LighthouseService
{
	public partial class WakeService : ServiceBase
	{
		public const string SvcName = "Wake my PC Lighthouse";
		public const string SvcDescription = "Allows the phone application to know if the computer is running and to power it down.";

		const string cliDebugShort = "-d";
		const string cliDebugLong = "--debug";
		const string cliHelpShort = "-?";
		const string cliHelpLong = "--help";
		const string cliInstallShort = "-i";
		const string cliInstallLong = "--install";
		const string cliUninstallShort = "-u";
		const string cliUninstallLong = "--uninstall";

		bool useLogs;
		IConfiguration currentConfiguration;
		Server currentServer;
			
		
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

			try
			{
				foreach(string arg in args)
				{
					//Console.WriteLine("arg: " + arg);

					switch(arg.ToLowerInvariant())
					{
						case cliHelpLong:
						case cliHelpShort:
							OnCLIHelp();
							return 0;
						case cliInstallLong :
						case cliInstallShort:
							OnInstall(args);
							return 0;
						case cliUninstallLong:
						case cliUninstallShort:
							OnUninstall(args);
							return 0;
						case cliDebugLong:
						case cliDebugShort:
							useLogs = true;
							break;
						default: break;
					}
				}

				Console.WriteLine("Starting the service...");
				System.ServiceProcess.ServiceBase.Run(new WakeService(useLogs));

				return 0;
			}
			catch(Exception e)
			{
				Console.WriteLine(e.ToString());
				return -1;
			}
		}

		#region Command-line interface

		private static void OnCLIHelp()
		{
			Console.WriteLine("Wake my PC Lighthouse service");
			Console.WriteLine("=============================");
			Console.WriteLine("USAGE :");
			Console.WriteLine(" -?");
			Console.WriteLine(" --help	    : Display this help");
			Console.WriteLine(" -d");
			Console.WriteLine(" --debug	    : Activates debug logging in Windows EventLog");
			Console.WriteLine(" -i");
			Console.WriteLine(" --install   : Install the service");
			Console.WriteLine(" -u");
			Console.WriteLine(" --uninstall : Uninstall the service");
		}

		private static bool IsUserAdministrator()
		{
			bool isAdmin;
			try
			{
				WindowsIdentity user = WindowsIdentity.GetCurrent();
				WindowsPrincipal principal = new WindowsPrincipal(user);
				isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
			}
			catch (UnauthorizedAccessException)
			{
				isAdmin = false;
			}
			catch (Exception)
			{
				isAdmin = false;
			}

			return isAdmin;
		}

		private static void RestartAsAdministrator(params string[] args)
		{
			var proc = new Process();
			if (System.Environment.OSVersion.Version.Major >= 6)  // Windows Vista or higher
			{
				proc.StartInfo.Verb = "runas";
			}
			proc.StartInfo.FileName = Application.ExecutablePath;
			proc.StartInfo.Arguments = String.Join(" ", args);
			proc.StartInfo.UseShellExecute = true;

			proc.Start();
			return;
		}

		private static ServiceController LocateService()
		{
			ServiceController[] allServices = ServiceController.GetServices();
			foreach (ServiceController svc in allServices)
			{
				if (svc.ServiceName == WakeService.SvcName)
					return svc;
			}

			return null;
		}

		private static void OnInstall(string[] args)
		{
			if (IsUserAdministrator())
			{
				if (ProjectInstaller.Install(false, args))
				{
					Console.WriteLine("Service installed.");
					Console.WriteLine("Starting the service...");
					LocateService().Start();
					Console.WriteLine("Service state : " + LocateService().Status.ToString());
				}
			}
			else
				RestartAsAdministrator("-i");
		}

		private static void OnUninstall(string[] args)
		{
			if (IsUserAdministrator())
			{
				if(ProjectInstaller.Install(true, args))
					Console.WriteLine("Service uninstalled.");
			}
			else
				RestartAsAdministrator("-u");
		}

		#endregion

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
