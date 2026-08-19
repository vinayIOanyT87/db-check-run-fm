using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Management;

namespace LoadRackService
{
	/// <summary>
	/// Summary description for ProjectInstaller.
	/// </summary>
	[RunInstaller(true)]
	public class ProjectInstaller : System.Configuration.Install.Installer
	{
		private System.ServiceProcess.ServiceProcessInstaller LoadRackProcessInstaller;
		private System.ServiceProcess.ServiceInstaller LoadRackServiceInstaller;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ProjectInstaller()
		{
			// This call is required by the Designer.
			InitializeComponent();
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}


		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

			this.LoadRackProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
			this.LoadRackServiceInstaller = new System.ServiceProcess.ServiceInstaller();
			// 
			// LoadRackProcessInstaller
			// 
			this.LoadRackProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
			this.LoadRackProcessInstaller.Password = null;
			this.LoadRackProcessInstaller.Username = null;
			// 
			// LoadRackServiceInstaller
			// 
			this.LoadRackServiceInstaller.DisplayName = "FuelsManager Terminal Automation";
			this.LoadRackServiceInstaller.ServiceName = "FuelsManager Terminal Automation";

			string nlastring = string.Empty;
            // Network Location Awareness service name changed from "nla" to "nlasvc" in Vista,
            // and has remained "nlasvc" since. Rather than just checking for specific versions,
            // changing the code to check anything equal to or newer than Vista to determine
            // the service name
			// Hardcoding the dependency to NlaSvc only, as 9.7 is not supported on anything prior to Windows Server 2012

			this.LoadRackServiceInstaller.ServicesDependedOn = new string[] {
																								 "RpcSs",
																								 "NlaSvc",
																								 "COMSysApp",
																								 "MSDTC"};

			this.LoadRackServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
			// 
			// ProjectInstaller
			// 
			this.Installers.AddRange(new System.Configuration.Install.Installer[] {
																											 this.LoadRackProcessInstaller,
																											 this.LoadRackServiceInstaller});

		}
		#endregion
	}
}
