namespace FMActiveDirectoryManageService
{
    using System.ComponentModel;
    using System.ServiceProcess;

    [RunInstaller(true)]
    public partial class FMActiveDirectoryManageServiceInstaller : System.Configuration.Install.Installer
    {
        public FMActiveDirectoryManageServiceInstaller()
        {
            this.InitializeComponent();

            ServiceProcessInstaller serviceProcessInstaller = new ServiceProcessInstaller();
            ServiceInstaller serviceInstaller = new ServiceInstaller();

            //# Service Account Information
            serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
            serviceProcessInstaller.Username = null;
            serviceProcessInstaller.Password = null;

            //# Service Information
            serviceInstaller.DisplayName = "FM Active Directory Manage Service";
            serviceInstaller.StartType = ServiceStartMode.Manual;

            //# This must be identical to the WindowsService.ServiceBase name
            //# set in the constructor of WindowsService.cs
            serviceInstaller.ServiceName = "FMActiveDirectoryManageService";

            this.Installers.Add(serviceProcessInstaller);
            this.Installers.Add(serviceInstaller);
        }
    }
}
