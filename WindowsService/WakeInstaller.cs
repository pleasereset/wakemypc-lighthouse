using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace ree7.WakeMyPC.WindowsService
{
    [RunInstaller(true)]
    public class WakeInstaller : Installer
    {
        private ServiceProcessInstaller processInstaller;
        private ServiceInstaller serviceInstaller;

        public WakeInstaller()
        {
            processInstaller = new ServiceProcessInstaller();
            serviceInstaller = new ServiceInstaller();

            processInstaller.Account = ServiceAccount.LocalSystem;
            serviceInstaller.StartType = ServiceStartMode.Automatic;
            serviceInstaller.ServiceName = WakeService.SvcName;
            serviceInstaller.Description = WakeService.SvcDescription;

            Installers.Add(serviceInstaller);
            Installers.Add(processInstaller);
        }
    }
}



