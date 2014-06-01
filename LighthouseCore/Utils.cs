using System;
using System.Diagnostics;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ree7.WakeMyPC.LighthouseCore.Utils
{
	public static class Account
	{
		/// <param name="passwordA">Candidate password</param>
		/// <param name="passwordB">Password stored in the settings</param>
		public static bool CheckPassword(string passwordA, string passwordB)
		{
			Log.d("Checking password : " + passwordA);
			if (passwordA[passwordA.Length - 1] == '\n') passwordA = passwordA.Substring(0, passwordA.Length - 1);
			return passwordB == passwordA;
		}
	}

	public static class System
	{
		public static void Shutdown()
		{
			/*ManagementBaseObject mboShutdown = null;
			ManagementClass mcWin32 = new ManagementClass("Win32_OperatingSystem");
			mcWin32.Get();

			// You can't shutdown without security privileges
			mcWin32.Scope.Options.EnablePrivileges = true;
			ManagementBaseObject mboShutdownParams =
					 mcWin32.GetMethodParameters("Win32Shutdown");

			// Flag 1 means we want to shut down the system. Use "2" to reboot.
			mboShutdownParams["Flags"] = "1";
			mboShutdownParams["Reserved"] = "0";
			foreach (ManagementObject manObj in mcWin32.GetInstances())
			{
				mboShutdown = manObj.InvokeMethod("Win32Shutdown",
											   mboShutdownParams, null);
			}*/

			Process.Start("shutdown", "/s /t 0");
		}

		public static void Reset()
		{
			Process.Start("shutdown", "/r /t 0");
		}

		public static void Standby()
		{
			try { SetSuspendState(false, true, true); }
			catch (Exception e)
			{
				Log.d("Error in System.Standby : " + e.ToString());
			}
		}

		public static void Hibernate()
		{
			try { SetSuspendState(true, true, true); }
			catch (Exception e)
			{
				Log.d("Error in System.Hibernate : " + e.ToString());
			}
		}

		public static void LogOff()
		{
			try { ExitWindowsEx(0, 0); }
			catch (Exception e)
			{
				Log.d("Error in System.LogOff : " + e.ToString());
			}
		}

		public static void LockSession()
		{
			try { LockWorkStation(); }
			catch (Exception e)
			{
				Log.d("Error in System.LockSession : " + e.ToString());
			}
		}

		#region Win32 DllImports

		[DllImport("user32")]
		private static extern bool ExitWindowsEx(uint uFlags, uint dwReason);

		[DllImport("user32")]
		private static extern void LockWorkStation();

		[DllImport("Powrprof.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern bool SetSuspendState(bool hiberate, bool forceCritical, bool disableWakeEvent);

		#endregion
	}
}
