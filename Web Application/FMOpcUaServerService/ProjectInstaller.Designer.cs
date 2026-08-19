namespace FMOpcUaServerService
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
			this.FMOpcUaServerServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
			this.FMOpcUaServerServiceInstaller = new System.ServiceProcess.ServiceInstaller();
			// 
			// FMOpcUaServerServiceProcessInstaller
			// 
			this.FMOpcUaServerServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
			this.FMOpcUaServerServiceProcessInstaller.Password = null;
			this.FMOpcUaServerServiceProcessInstaller.Username = null;
			// 
			// FMPOpcUaServerServiceInstaller
			// 
			this.FMOpcUaServerServiceInstaller.ServiceName = "FuelsManager Opc Ua Server Service";
			// 
			// ProjectInstaller
			// 
			this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.FMOpcUaServerServiceProcessInstaller,
            this.FMOpcUaServerServiceInstaller});

		}

		#endregion

		private System.ServiceProcess.ServiceProcessInstaller FMOpcUaServerServiceProcessInstaller;
		private System.ServiceProcess.ServiceInstaller FMOpcUaServerServiceInstaller;
	}
}