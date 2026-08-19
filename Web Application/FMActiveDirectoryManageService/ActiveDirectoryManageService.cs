
namespace FMActiveDirectoryManageService
{
    using System.ServiceProcess;
    using System.Threading;

    public partial class ActiveDirectoryManageService : ServiceBase
    {
        #region Private data members.
        private readonly AdManageThread adManageThread;
        #endregion

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            var servicesToRun = new ServiceBase[] { new ActiveDirectoryManageService()  };
            Run(servicesToRun);
        }

        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ActiveDirectoryManageService()
        {
            this.InitializeComponent();
            this.Init();
            this.adManageThread = new AdManageThread();
        }
        #endregion

        protected override void OnStart(string[] args)
        {
            //Thread.Sleep(30000);
            base.OnStart(args);
            this.adManageThread.StopFlag = false;
            this.adManageThread.Start();
        }

        protected override void OnStop()
        {
            this.adManageThread.StopFlag = true;
            this.adManageThread.FMEventLog.WriteEntry(AdManageThread.MessagePrefixKey + " FM Active Directory Manage Service On Stop has been called.");
        }

        #region Private methods
        /// <summary>
        /// This method will initialize the object to its initial state.
        /// </summary>
        private void Init()
        {
            this.ServiceName = "FMActiveDirectoryManageService";
            this.EventLog.Log = "Application";

            // Flags to handle specific events
            this.CanHandlePowerEvent = true;
            this.CanHandleSessionChangeEvent = true;
            this.CanPauseAndContinue = true;
            this.CanShutdown = true;
            this.CanStop = true;
        }
        #endregion
    }
}
