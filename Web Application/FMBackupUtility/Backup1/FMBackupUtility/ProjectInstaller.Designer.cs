namespace FMBackupUtility
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
            this.serviceProcessInstallerBU = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstallerBU = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstallerBU
            // 
            this.serviceProcessInstallerBU.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstallerBU.Password = null;
            this.serviceProcessInstallerBU.Username = null;
            // 
            // serviceInstallerBU
            // 
            this.serviceInstallerBU.DisplayName = "FuelsManager Backup Utility";
            this.serviceInstallerBU.ServiceName = "FuelsManager Backup Utility";
            this.serviceInstallerBU.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstallerBU,
            this.serviceInstallerBU});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstallerBU;
        private System.ServiceProcess.ServiceInstaller serviceInstallerBU;
    }
}