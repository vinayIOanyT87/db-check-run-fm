namespace FMPointService
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
			this.FMPointServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
			this.FMPointServiceInstaller = new System.ServiceProcess.ServiceInstaller();
			// 
			// FMPointServiceProcessInstaller
			// 
			this.FMPointServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
			this.FMPointServiceProcessInstaller.Password = null;
			this.FMPointServiceProcessInstaller.Username = null;
			// 
			// FMPointServiceInstaller
			// 
			this.FMPointServiceInstaller.ServiceName = "FuelsManager Point Service";
			// 
			// ProjectInstaller
			// 
			this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.FMPointServiceProcessInstaller,
            this.FMPointServiceInstaller});

		}

		#endregion

		private System.ServiceProcess.ServiceProcessInstaller FMPointServiceProcessInstaller;
		private System.ServiceProcess.ServiceInstaller FMPointServiceInstaller;
	}
}