namespace FuelsManager.Afss.ServiceProcess
{
    partial class ProjectInstaller
    {
        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller serviceInstaller;

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

            this.serviceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // LoadRackProcessInstaller
            // 
            this.serviceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstaller.Password = null;
            this.serviceProcessInstaller.Username = null;
            // 
            // LoadRackServiceInstaller
            // 
            this.serviceInstaller.DisplayName = "FuelsManager Automated Fuel Service Station";
            this.serviceInstaller.ServiceName = "FuelsManager AFSS Service";

            string nlastring = string.Empty;
            // Network Location Awareness service name changed from "nla" to "nlasvc" in Vista,
            // and has remained "nlasvc" since. Rather than just checking for specific versions,
            // changing the code to check anything equal to or newer than Vista to determine
            // the service name
            if (System.Environment.OSVersion.Version.Major >= 6)
                nlastring = "NlaSvc";
            else
                nlastring = "nla";

            this.serviceInstaller.ServicesDependedOn = new string[]
                                                           { "RpcSs", "RpcLocator", nlastring, "COMSysApp", "MSDTC" };
            
            this.serviceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(
                new System.Configuration.Install.Installer[] { this.serviceProcessInstaller, this.serviceInstaller });
        }

        #endregion
    }
}