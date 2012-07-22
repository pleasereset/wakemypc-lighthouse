using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using ree7.WakeMyPC.ProbeServer;
using System.Windows.Input;

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
            _Password = Settings.Default.Password;
            _Port = Settings.Default.Port;
        }

        ~MainViewModel()
        {
            Settings.Default.Password = _Password;
            Settings.Default.Port = _Port;
            Settings.Default.Save();
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
        const string IsServerRunningPropertyName = "IsServerRunning";

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
        #endregion

        private Server _Server = null;
        

        /// <summary>
        /// Please, tranform me into ICommand when you'll be less lasy
        /// </summary>
        public void StartServer()
        {
            if (!_ServerRunning)
            {
                _Server = new Server(int.Parse(Port), Password);
                _Server.Start();
                IsServerRunning = true;
            }
        }

        /// <summary>
        /// Please, tranform me into ICommand when you'll be less lasy
        /// </summary>
        public void StopServer()
        {
            if (_Server != null && _ServerRunning)
            {
                _Server.Stop();
                IsServerRunning = false;
            }
        }
    }
}
