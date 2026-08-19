namespace FuelsManagerService
{
	partial class ProjectInstaller
	{
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
			this.fuelsManagerServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
			this.fuelsManagerServiceInstaller = new System.ServiceProcess.ServiceInstaller();
			// 
			// fuelsManagerServiceProcessInstaller
			// 
			this.fuelsManagerServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
			this.fuelsManagerServiceProcessInstaller.Password = null;
			this.fuelsManagerServiceProcessInstaller.Username = null;
			// 
			// fuelsManagerServiceInstaller
			// 
			this.fuelsManagerServiceInstaller.ServiceName = "FuelsManager Service";
			// 
			// ProjectInstaller
			// 
			this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.fuelsManagerServiceProcessInstaller,
            this.fuelsManagerServiceInstaller});

		}

		#endregion

		private System.ServiceProcess.ServiceProcessInstaller fuelsManagerServiceProcessInstaller;
		private System.ServiceProcess.ServiceInstaller fuelsManagerServiceInstaller;
	}
}