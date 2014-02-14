using System;
using System.ComponentModel;
using ree7.WakeMyPC.LighthouseCore;
using System.ServiceProcess;
using ree7.WakeMyPC.LighthouseService;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight;

namespace ree7.WakeMyPC.Agent
{
	public class MainViewModel : ObservableObject
	{
		#region Singleton

		private MainViewModel()
		{
			Initialize();
		}

		private static MainViewModel _Instance = null;
		public static MainViewModel Instance
		{
			get
			{
				if (_Instance == null) _Instance = new MainViewModel();
				return _Instance;
			}
		}

		#endregion

		#region Bindings
		const string PasswordPropertyName = "Password";
		const string PortPropertyName = "Port";
		const string IsBusyPropertyName = "IsBusy";
		const string IsServerRunningPropertyName = "IsServerRunning";
		const string CanSaveSettingsPropertyName = "CanSaveSettings";

		#region public string Password
		private string _Password = "";
		public string Password
		{
			get { return _Password; }
			set
			{
				if (value != _Password)
				{
					_Password = value;
					RaisePropertyChanged(PasswordPropertyName);
					CanSaveSettings = true;
				}
			}
		}
		#endregion
		#region public string Port
		private string _Port = "";
		public string Port
		{
			get { return _Port; }
			set
			{
				if (value != _Port)
				{
					// Check input validity
					int i = -1;
					if (int.TryParse(value, out i) && i > 0 && i < 65536)
					{
						_Port = value;
						RaisePropertyChanged(PortPropertyName);
						CanSaveSettings = true;
					}
					else
					{
						throw new ArgumentException();
					}
				}
			}
		}
		#endregion
		#region public bool IsServerRunning
		private bool _ServerRunning = false;
		public bool IsServerRunning
		{
			get { return _ServerRunning; }
			private set
			{
				_ServerRunning = value;
				RaisePropertyChanged(IsServerRunningPropertyName);
			}
		}
		#endregion
		#region public bool CanSaveSettings
		private bool _CanSaveSettings = false;
		public bool CanSaveSettings
		{
			get { return _CanSaveSettings; }
			private set
			{
				_CanSaveSettings = value;
				RaisePropertyChanged(CanSaveSettingsPropertyName);
			}
		}
		#endregion
		#region public bool IsBusy
		private bool _IsBusy = false;
		public bool IsBusy
		{
			get { return _IsBusy; }
			private set
			{
				_IsBusy = value;
				RaisePropertyChanged(IsBusyPropertyName);
			}
		}
		#endregion

		#region public RelayCommand StartServer
		public RelayCommand StartServer
		{
			get
			{
				return new RelayCommand(OnStartServer);
			}
		}
		#endregion
		#region public RelayCommand StopServer
		public RelayCommand StopServer
		{
			get
			{
				return new RelayCommand(OnStopServer);
			}
		}
		#endregion
		#region public RelayCommand SaveSettings
		public RelayCommand SaveSettings
		{
			get
			{
				return new RelayCommand(OnSaveSettings);
			}
		}
		#endregion
		#region public RelayCommand CheckForUpdates
		private RelayCommand _CheckForUpdates;
		public RelayCommand CheckForUpdates
		{
			get
			{
				return new RelayCommand(OnCheckForUpdates);
			}
		}
		#endregion
		
		public void SettingsModified()
		{
			CanSaveSettings = true;
		}

		#endregion

		private ServiceController _service;
		private RegistryConfiguration _registrySettings;

		private void Initialize()
		{
			_service = LocateService();
			IsServerRunning = _service.Status == ServiceControllerStatus.Running;

			LoadSettingsFromRegistry();
			CanSaveSettings = false;
		}

		private ServiceController LocateService()
		{
			ServiceController[] allServices = ServiceController.GetServices();
			foreach (ServiceController svc in allServices)
			{
				if (svc.ServiceName == WakeService.SvcName)
					return svc;
			}

			throw new WarningException("Wake my PC Lighthouse service is not installed.");
		}

		
		private void LoadSettingsFromRegistry()
		{
			_registrySettings = new RegistryConfiguration();
			Port = _registrySettings.Port.ToString();
			Password = _registrySettings.Password;
		}

		private void SaveSettingsToRegistry()
		{
			int port = Int32.Parse(Port);
			string password = Password;
			_registrySettings.Save(port, password);
		}

		private void OnStartServer()
		{
			IsBusy = true;
			if (_service != null)
			{
				try
				{
					_service.Start();
					_service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
					IsServerRunning = _service.Status == ServiceControllerStatus.Running;
					Console.WriteLine("After StartServer : " + _service.Status);
				}
				catch (Exception ex)
				{
					Console.WriteLine("Exception in StartServer : " + ex.ToString());
				}
			}
			IsBusy = false;
		}

		private void OnStopServer()
		{
			IsBusy = true;
			if (_service != null)
			{
				try
				{
					_service.Stop();
					_service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
					IsServerRunning = _service.Status != ServiceControllerStatus.Stopped;
					Console.WriteLine("After StopServer : " + _service.Status);
				}
				catch (Exception ex)
				{
					Console.WriteLine("Exception in StopServer : " + ex.ToString());
				}
			}
			IsBusy = false;
		}

		private void OnSaveSettings()
		{
			if (IsServerRunning == true) OnStopServer();

			SaveSettingsToRegistry();
			CanSaveSettings = false;

			if (IsServerRunning == false) OnStartServer();
		}

		private void OnCheckForUpdates()
		{
			var updateWindow = new UpdateWindow();
			updateWindow.ShowDialog();
		}
	}
}
