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
        #endregion

        private Server _Server = null;

    }
}
