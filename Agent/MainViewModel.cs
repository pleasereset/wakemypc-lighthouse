using System;
using System.ComponentModel;
using ree7.WakeMyPC.ProbeServer;
using System.ServiceProcess;
using ree7.WakeMyPC.WindowsService;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace Agent
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

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

        private string _Password = "";
        public string Password
        {
            get { return _Password; }
            set
            {
                if (value != _Password)
                {
                    _Password = value;
                    NotifyPropertyChanged(PasswordPropertyName);
                    CanSaveSettings = true;
                }
            }
        }

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
                        NotifyPropertyChanged(PortPropertyName);
                        CanSaveSettings = true;
                    }
                    else
                    {
                        throw new ArgumentException();
                    }
                }
            }
        }

        private bool _ServerRunning = false;
        public bool IsServerRunning
        {
            get { return _ServerRunning; }
            private set
            {
                _ServerRunning = value;
                NotifyPropertyChanged(IsServerRunningPropertyName);
            }
        }

        private bool _CanSaveSettings = false;
        public bool CanSaveSettings
        {
            get { return _CanSaveSettings; }
            private set
            {
                Console.WriteLine("CanSaveSettings=" + value);
                _CanSaveSettings = value;
                NotifyPropertyChanged(CanSaveSettingsPropertyName);
            }
        }

        private bool _IsBusy = false;
        public bool IsBusy
        {
            get { return _IsBusy; }
            private set
            {
                _IsBusy = value;
                NotifyPropertyChanged(IsBusyPropertyName);
            }
        }

        /// <summary>
        /// Please, tranform me into ICommand when you'll be less lasy
        /// </summary>
        public void StartServer()
        {
            IsBusy = true;
            if (_service != null) _service.Start();
            IsServerRunning = true; // TODO replace by probing real service status
            IsBusy = false;
        }

        /// <summary>
        /// Please, tranform me into ICommand when you'll be less lasy
        /// </summary>
        public void StopServer()
        {
            IsBusy = true;
            if (_service != null) _service.Stop();
            IsServerRunning = false; // TODO replace by probing real service status
            IsBusy = false;
        }

        /// <summary>
        /// Please, tranform me into ICommand when you'll be less lasy
        /// </summary>
        public void SaveSettings()
        {
            SaveSettingsToRegistry();
            CanSaveSettings = false;

            // Restart the server
            StopServer();
            StartServer();
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
    }
}
