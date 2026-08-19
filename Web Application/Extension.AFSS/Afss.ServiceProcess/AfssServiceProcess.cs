namespace FuelsManager.Afss.ServiceProcess
{
    using System;
    using System.Diagnostics;
    using System.ServiceProcess;

    public partial class AfssServiceProcess : ServiceBase
    {
        public static string WindowsServiceName = "FuelsManager Automated Fuel Service Station Service";

        public static string WindowsServiceDescription =
            "Provides interfaces that can be used to communicate with automated fuel service stations in order to download transactions into FuelsManager.";

        private readonly ServiceManager serviceManager = new ServiceManager();

        public AfssServiceProcess()
        {
            ServiceName = AfssServiceProcess.WindowsServiceName;
            AutoLog = false;
            CanShutdown = true;
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            this.Start();
        }

        protected override void OnStop()
        {
            this.Exit();
            base.OnStop();
        }

        /// <summary>
        /// Start the service
        /// </summary>
        public void Start()
        {
            try
            {
                this.serviceManager.OpenAll();

                EventLog.WriteEntry("Automated Fuel Service Station Service Started", EventLogEntryType.Information);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(ex.Message, EventLogEntryType.Error);
            }
        }

        /// <summary>
        /// Stop the service and its components
        /// </summary>
        public void Exit()
        {
            try
            {
                this.serviceManager.CloseAll();

                EventLog.WriteEntry("Automated Fuel Service Station Service Stopped", EventLogEntryType.Information);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(ex.Message, EventLogEntryType.Error);
            }
        }
    }
}
