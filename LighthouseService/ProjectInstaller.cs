using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;

namespace ree7.WakeMyPC.LighthouseService
{
	[RunInstaller(true)]
	public partial class ProjectInstaller : System.Configuration.Install.Installer
	{
		public ProjectInstaller()
		{
			InitializeComponent();
			serviceInstaller1.ServiceName = WakeService.SvcName;
			serviceInstaller1.Description = WakeService.SvcDescription;
		}

		public static bool Install(bool undo, string[] args)
		{
			try
			{
				Console.WriteLine(undo ? "Uninstalling..." : "Installing...");
				using (AssemblyInstaller inst = new AssemblyInstaller(typeof(WakeService).Assembly, args))
				{
					IDictionary state = new Hashtable();
					inst.UseNewContext = true;
					try
					{
						if (undo)
						{
							inst.Uninstall(state);
						}
						else
						{
							inst.Install(state);
							inst.Commit(state);
						}
					}
					catch
					{
						try
						{
							inst.Rollback(state);
						}
						catch { }
						throw;
					}
				}

				return true;
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex.Message);
			}

			return false;
		}

		private void serviceProcessInstaller1_AfterInstall(object sender, InstallEventArgs e)
		{

		}

		private void serviceInstaller1_AfterInstall(object sender, InstallEventArgs e)
		{

		}
	}
}
