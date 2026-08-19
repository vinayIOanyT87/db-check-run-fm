namespace FMActiveDirectoryManageService
{
    using System;
    using System.Diagnostics;
    using System.ServiceProcess;
    using System.Threading;

    public abstract class BaseThread
    {
        #region Data members
        protected Thread TheThread;
        protected int SleepTime;
        protected ManualResetEvent StopEvent;
        protected EventLog TheEventLog;
        private readonly EventLog eventLog;
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        protected BaseThread()
        {
            this.StopEvent = new ManualResetEvent(false);
            this.TheThread = new Thread(this.RunMethod);
            this.eventLog = new EventLog("Application", ".", "FMActiveDirectoryManageService");
        }
        #endregion

        #region Properties
        /// <summary>
        /// Get and set the thread name.
        /// </summary>
        protected string ThreadName { get; set; }
        public EventLog FMEventLog => this.eventLog;
        #endregion

        #region Abstract methods
        protected abstract void ThreadHandler();
        #endregion

        #region Public and Protected methods
        /// <summary>
        /// This method starts the thread process.
        /// </summary>
        protected void RunMethod()
        {
            try
            {
                this.ThreadHandler();
            }
            catch (Exception e)
            {
                string msg = AdManageThread.MessagePrefixKey + " Error starting the FM AD Manage thread. " + e.Message;
                this.FMEventLog.WriteEntry(msg);
            }
        }

        public void Start()
        {
            this.TheThread.Start();
        }

        /// <summary>
        /// This method will update the service status in the Services Window.
        /// </summary>
        public void StopService()
        {
            ServiceController service = new ServiceController("FM Active Directory Manage Service");
            service.Stop();
            service.WaitForStatus(ServiceControllerStatus.Stopped);
        }

        /// <summary>
        /// This method will check to see if there is another FM Active Directory Manage Service
        /// running on the machine.
        /// </summary>
        /// <returns>Return true if there is another service running. Otherwise, returns false.</returns>
        protected bool IsMoreThanOneProcessRunning()
        {
            int processCount = 0;

            foreach (var process in Process.GetProcesses())
            {
                if (process.ProcessName.Contains("FMActiveDirectoryManageService")) processCount++;
            }

            if (processCount > 1)
            {
                const string Message = AdManageThread.MessagePrefixKey + " There is an FM Active Directory Manage Service process already running.";
                this.FMEventLog.WriteEntry(Message, EventLogEntryType.Error);

                return true;
            }

            return false;
        }
        #endregion
    }
}
