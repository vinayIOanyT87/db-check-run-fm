namespace FMIridiumGssService
{
	using System.ComponentModel;
	using System.Configuration.Install;
	using System.ServiceProcess;

	[RunInstaller(true)]
	public partial class FMIridiumGssServiceInstaller : Installer
	{
		public FMIridiumGssServiceInstaller()
		{
			this.InitializeComponent();

			ServiceProcessInstaller serviceProcessInstaller = new ServiceProcessInstaller();
			ServiceInstaller serviceInstaller = new ServiceInstaller();

			//# Service Account Information
			serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
			serviceProcessInstaller.Username = null;
			serviceProcessInstaller.Password = null;

			//# Service Information
			serviceInstaller.DisplayName = "FM Iridium Listener Service";
			serviceInstaller.StartType = ServiceStartMode.Manual;

			//# This must be identical to the WindowsService.ServiceBase name
			//# set in the constructor of WindowsService.cs
			serviceInstaller.ServiceName = "FMIridiumGssService";

			this.Installers.Add(serviceProcessInstaller);
			this.Installers.Add(serviceInstaller);
		}
	}
}
