namespace FMUAAlarmServer
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
            this.FMUAAlarmServerServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.FMUAAlarmServerServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // FMUAAlarmServerServiceProcessInstaller
            // 
            this.FMUAAlarmServerServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalService;
            this.FMUAAlarmServerServiceProcessInstaller.Password = null;
            this.FMUAAlarmServerServiceProcessInstaller.Username = null;
            // 
            // FMUAAlarmServerServiceInstaller
            // 
            this.FMUAAlarmServerServiceInstaller.ServiceName = "FMUAAlarmServerService";
            this.FMUAAlarmServerServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.FMUAAlarmServerServiceProcessInstaller,
            this.FMUAAlarmServerServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller FMUAAlarmServerServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller FMUAAlarmServerServiceInstaller;
    }
}