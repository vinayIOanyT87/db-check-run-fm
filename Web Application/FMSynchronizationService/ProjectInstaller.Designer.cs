namespace FMSynchronizationService
{
	partial class ProjectInstaller
	{
		private System.ServiceProcess.ServiceProcessInstaller fmSynchronizationServiceProcessInstaller;
		private System.ServiceProcess.ServiceInstaller fmSynchronizationServiceInstaller;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();

			this.fmSynchronizationServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
			this.fmSynchronizationServiceInstaller = new System.ServiceProcess.ServiceInstaller();
			// 
			// LoadRackProcessInstaller
			// 
			this.fmSynchronizationServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
			this.fmSynchronizationServiceProcessInstaller.Password = null;
			this.fmSynchronizationServiceProcessInstaller.Username = null;
			// 
			// LoadRackServiceInstaller
			// 
			this.fmSynchronizationServiceInstaller.DisplayName = "FuelsManager Synchronization";
			this.fmSynchronizationServiceInstaller.ServiceName = "FuelsManager Synchronization Service";

			this.fmSynchronizationServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
			// 
			// ProjectInstaller
			// 
			this.Installers.AddRange(new System.Configuration.Install.Installer[] {
																						this.fmSynchronizationServiceProcessInstaller,
																						this.fmSynchronizationServiceInstaller});


		}

		#endregion
	}
}