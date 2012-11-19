using Microsoft.Win32;
using System;

namespace ree7.WakeMyPC.WindowsService
{
    public abstract class IConfiguration
    {
        public const int DefaultPort = 33666;
        public const string DefaultPassword = "changeme";

        public int Port { get; protected set; }
        public string Password { get; protected set; }
    }

    public class StaticConfiguration : IConfiguration
    {
        public StaticConfiguration(int port, string password)
        {
            Port = port;
            Password = password;
        }
    }

    /// <summary>
    /// Wake my PC Service configuration is stored in HKEY_LOCAL_MACHINE\SOFTWARE\ree7\Wake my PC Lighthouse
    /// </summary>
    public class RegistryConfiguration : IConfiguration
    {
        const string DirRoot = "SOFTWARE";
        const string DirVendor = "ree7";
        const string DirProduct = "Wake my PC Lighthouse";

        const string ValPort = "Port";
        const string ValPassword = "Password";

        public delegate void LoggingOutlet(string message);
        private LoggingOutlet logger;

        public RegistryConfiguration(LoggingOutlet logger = null)
        {
            this.logger = logger;

            Load();
        }

        private void Load()
        {
            try
            {
                using (RegistryKey KeyRoot = Registry.LocalMachine.CreateSubKey(DirRoot))
                {
                    using (RegistryKey KeyVendor = KeyRoot.CreateSubKey(DirVendor))
                    {
                        using (RegistryKey KeyProduct = KeyVendor.CreateSubKey(DirProduct))
                        {
                            object oPort = KeyProduct.GetValue(ValPort);
                            object oPassword = KeyProduct.GetValue(ValPassword);

                            if (oPort == null || oPassword == null)
                            {
                                // Falling back to default configuration and create records in the registry
                                oPort = DefaultPort;
                                oPassword = DefaultPassword;
                                KeyProduct.SetValue(ValPort, oPort, RegistryValueKind.DWord);
                                KeyProduct.SetValue(ValPassword, oPassword, RegistryValueKind.String);
                            }

                            Port = (int)oPort;
                            Password = (string)oPassword;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (logger != null) logger("Exception in RegistryConfiguration : " + ex.ToString());
                throw ex;
            }
        }

        public void Save(int port, string password)
        {
            using (RegistryKey KeyRoot = Registry.LocalMachine.CreateSubKey(DirRoot))
            {
                using (RegistryKey KeyVendor = KeyRoot.CreateSubKey(DirVendor))
                {
                    using (RegistryKey KeyProduct = KeyVendor.CreateSubKey(DirProduct))
                    {
                        KeyProduct.SetValue(ValPort, port, RegistryValueKind.DWord);
                        KeyProduct.SetValue(ValPassword, password, RegistryValueKind.String);
                    }
                }
            }


        }
    }
}
