namespace FMIridiumGssService
{
	using System;
	using System.Diagnostics;
	using System.Net.Sockets;
	using System.ServiceProcess;
	using System.Threading;

	public abstract class BaseThread
	{
		#region Data members
		protected Thread TheThread;
		protected int SleepTime;
		protected ManualResetEvent StopEvent;
		protected EventLog TheEventLog;
		protected TcpListener tcpListener;
		protected TcpClient tcpClient;
		protected NetworkStream stream;
		protected EventLog eventLog;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor
		/// </summary>
		protected BaseThread()
		{
			this.StopEvent		= new ManualResetEvent(false);
			this.TheThread		= new Thread(this.RunMethod);
			this.TheEventLog	= new EventLog { Log = "Application", Source = this.ThreadName };
			this.tcpListener	= null;
			this.tcpClient		= null;
			this.stream			= null;
			this.eventLog		= new EventLog("Application", ".", "FuelsManager");
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
				string msg = "Error starting the Iridium GSS Listener thread. " + e.Message;
				this.TheEventLog.WriteEntry(msg);
			}
		}

		public void Start()
		{
			this.TheThread.Start();
		}

		public void Cleanup()
		{
			if (this.tcpClient != null)
			{
				this.tcpClient.Close();
				this.tcpClient = null;
			}

			if (this.tcpListener != null)
			{
				if (this.stream != null)
				{
					this.stream.Flush();
					this.stream.Close();
					this.stream = null;
				}

				this.tcpListener.Stop();	
				this.tcpListener = null;
			}

			this.StopEvent.Set();
		}

		public void StopService()
		{
			ServiceController service = new ServiceController("FM Iridium Listener Service");
			service.Stop();
			service.WaitForStatus(ServiceControllerStatus.Stopped);
		}
		#endregion
	}
}
