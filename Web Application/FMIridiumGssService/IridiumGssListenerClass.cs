namespace FMIridiumGssService
{
	using System.ServiceProcess;
	using System.Threading;

	public partial class IridiumGssListenerClass : ServiceBase
	{
		#region Private data members.
		private readonly IridiumGssListenerThread iridiumGssListenerThread;
		#endregion

		#region Main entry
		/// <summary>
		/// This is the main entry point for the service.
		/// </summary>
		/// <param name="args"></param>
		static void Main(string[] args)
		{
			Run(new IridiumGssListenerClass());
		}
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public IridiumGssListenerClass()
		{
			this.InitializeComponent();
			this.Init();
			this.iridiumGssListenerThread = new IridiumGssListenerThread();
		}
		#endregion

		#region Protected methods
		/// <summary>
		/// This method will handles the on start event.
		/// </summary>
		/// <param name="args">Starting arguements.</param>
		protected override void OnStart(string[] args)
		{
			base.OnStart(args);
			this.iridiumGssListenerThread.ListenerStartFlag = true;
			this.iridiumGssListenerThread.ListenerRestartCount = 0;
			this.iridiumGssListenerThread.Start();
		}

		/// <summary>
		/// This method will handles the on stop event.
		/// </summary>
		protected override void OnStop()
		{
			this.iridiumGssListenerThread.FMEventLog.WriteEntry("Iridium GSS Listener On Stop has been called.");

			this.iridiumGssListenerThread.ListenerStartFlag = false;
			this.iridiumGssListenerThread.Cleanup();
		}

		/// <summary>
		/// This method handles the on shutdown event.
		/// </summary>
		protected override void OnShutdown()
		{
			this.iridiumGssListenerThread.FMEventLog.WriteEntry("Iridium GSS Listener On Shutdown has been called.");

			this.iridiumGssListenerThread.ListenerStartFlag = false;
			this.iridiumGssListenerThread.Cleanup();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (this.components != null))
			{
				this.components.Dispose();
			}

			base.Dispose(disposing);
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.ServiceName = "FMIridiumGssService";
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
