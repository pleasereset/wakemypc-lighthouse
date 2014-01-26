using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;

namespace ree7.WakeMyPC.LighthouseService
{
	public class Application
	{
		private static string executablePath;

		/// <summary>Obtient le chemin d'accès au fichier exécutable ayant démarré l'application, y compris le nom de l'exécutable.</summary>
		/// <returns>Chemin d'accès et nom du fichier exécutable ayant démarré l'application.Ce chemin d'accès sera différent selon que l'application Windows Forms est déployée ou non à l'aide de ClickOnce.Les applications ClickOnce sont stockées dans un cache d'application par utilisateur, dans le répertoire C:\Documents and Settings\NomUtilisateur.Pour plus d'informations, consultez Accessing Local and Remote Data in ClickOnce Applications.</returns>
		/// <filterpriority>1</filterpriority>
		/// <PermissionSet>
		///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
		///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
		///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
		///   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
		/// </PermissionSet>
		public static string ExecutablePath
		{
			get
			{
				if (Application.executablePath == null)
				{
					Assembly entryAssembly = Assembly.GetEntryAssembly();
					if (entryAssembly == null)
					{
						StringBuilder stringBuilder = new StringBuilder(260);
						UnsafeNativeMethods.GetModuleFileName(NativeMethods.NullHandleRef, stringBuilder, stringBuilder.Capacity);
						Application.executablePath = IntSecurity.UnsafeGetFullPath(stringBuilder.ToString());
					}
					else
					{
						string codeBase = entryAssembly.CodeBase;
						Uri uri = new Uri(codeBase);
						if (uri.IsFile)
						{
							Application.executablePath = uri.LocalPath + Uri.UnescapeDataString(uri.Fragment);
						}
						else
						{
							Application.executablePath = uri.ToString();
						}
					}
				}
				Uri uri2 = new Uri(Application.executablePath);
				if (uri2.Scheme == "file")
				{
					new FileIOPermission(FileIOPermissionAccess.PathDiscovery, Application.executablePath).Demand();
				}
				return Application.executablePath;
			}
		}
	}
	
	public class UnsafeNativeMethods
	{
		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		public static extern int GetModuleFileName(HandleRef hModule, StringBuilder buffer, int length);
	}

	internal static class IntSecurity
	{
		internal static string UnsafeGetFullPath(string fileName)
		{
			string result = fileName;
			new FileIOPermission(PermissionState.None)
			{
				AllFiles = FileIOPermissionAccess.PathDiscovery
			}.Assert();
			try
			{
				result = Path.GetFullPath(fileName);
			}
			finally
			{
				CodeAccessPermission.RevertAssert();
			}
			return result;
		}
	}

	internal static class NativeMethods
	{
		public static HandleRef NullHandleRef;
		
		static NativeMethods()
		{
			NativeMethods.NullHandleRef = new HandleRef(null, IntPtr.Zero);
		}
	}
}
