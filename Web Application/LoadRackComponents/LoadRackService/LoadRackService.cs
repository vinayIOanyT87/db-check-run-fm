/******************************************************************************

	FILE NAME:		LoadRackService.cs


	PURPOSE:			LoadRackServiceClass


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------
		11/14/2006	W.Gray		7.1.0.1 - Removed call to InitializeSecurity

*******************************************************************************/

namespace LoadRackService
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Channels;
    using System.Runtime.Remoting.Channels.Tcp;
    using System.Runtime.Remoting.Lifetime;
    using System.ServiceProcess;
    using System.Threading;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;

    using LoadRackLibrary;

    public class LoadRackServiceClass : ServiceBase
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private readonly Container components = null;
		private TcpServerChannel channel;
		private LoadRackManagerClass loadRackManager;
		private Thread loadRackThread;
		private ManualResetEvent killEvent;

		public LoadRackServiceClass()
		{
		    this.AutoLog = false;
		    this.CanShutdown = true;

			// This call is required by the Windows.Forms Component Designer.
		    this.InitializeComponent();

		}

		// The main entry point for the process
		static void Main()
		{
		    // More than one user Service may run within the same process. To add
			// another service to this process, change the following line to
			// create a second service object. For example,
			//
			//   ServicesToRun = new System.ServiceProcess.ServiceBase[] {new Service1(), new MySecondUserService()};
			//
		    var servicesToRun = new ServiceBase[] { new LoadRackServiceClass() };

		    Run(servicesToRun);

		}

        /// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// LoadRackServiceClass
			// 
			this.ServiceName = "LoadRackServiceClass";

		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
			    this.components?.Dispose();
			}
		    base.Dispose(disposing);
		}

		/// <summary>
		/// Set things in motion so your service can do its work.
		/// </summary>
		protected override void OnStart(string[] args)
		{
			try
			{
			    this.killEvent = new ManualResetEvent(false);
				ThreadStart loadRackStart = this.LoadRack;
			    this.loadRackThread = new Thread(loadRackStart);
			    this.loadRackThread.Start();
			}
			catch (Exception e)
			{
			    this.EventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
			}
		}

		/// <summary>
		/// Stop this service.
		/// </summary>
		protected override void OnStop()
		{
			if (this.killEvent != null)
			{
			    this.killEvent.Set();
			    this.killEvent = null;
			}

			if (this.loadRackThread != null)
			{
			    this.loadRackThread.Join();
			    this.loadRackThread = null;
			}
		}

		/// <summary>
		/// Shutdown this service.
		/// </summary>
		protected override void OnShutdown()
		{
			if (this.killEvent != null)
			{
			    this.killEvent.Set();
			    this.killEvent = null;
			}

			if (this.loadRackThread != null)
			{
			    this.loadRackThread.Join();
			    this.loadRackThread = null;
			}
		}


		protected void LoadRackStart()
		{
			try
			{
				//				OpcCom.Interop.InitializeSecurity();

				// since this application will abort if an attempt is made to write to the event log and it is full
				// we will change the event log settings to allow overwrite
				if (this.EventLog.OverflowAction != OverflowAction.OverwriteAsNeeded)
				{
				    this.EventLog.ModifyOverflowPolicy(OverflowAction.OverwriteAsNeeded, 1);
				}


                ushort version = FMChannelHelper.MakeCall<IHardwareKey, ushort>(x => x.CheckActivatedLicenceVersion());
				if ((version != 9999) && (version != 120))
				{
					throw new Exception("Wrong License Key Version " + (version / 10.0).ToString(CultureInfo.InvariantCulture));
				}

				//				Debugger.Launch();
				int portNum = 8087;

				try
				{
					string portNumString = FMChannelHelper.MakeCall<IConfigurationSettings, string>(
																	 x =>
																	 x.GetKeyValueByKey(null, ConfigurationSettingDOClass.Key_LoadRackPort)
																);

					if (!string.IsNullOrEmpty(portNumString))
					{
						int.TryParse(portNumString, out portNum);
					}
				}
				    // ReSharper disable once EmptyGeneralCatchClause
				catch
				{
				}

			    this.channel = new TcpServerChannel("tas", portNum);
				ChannelServices.RegisterChannel(this.channel, true);

			    this.loadRackManager = new LoadRackManagerClass(this.EventLog);
				if (this.loadRackManager == null) this.EventLog.WriteEntry("Error: CreateInstance LoadRackManager", EventLogEntryType.Information);
				else
				{
					RemotingServices.Marshal(this.loadRackManager, "LoadRackManager");
					ILease lease = (ILease)this.loadRackManager.GetLifetimeService();
					lease.Renew(new TimeSpan(10000, 0, 0, 0));
				}

			    this.EventLog.WriteEntry("Started", EventLogEntryType.Information);
			}
			catch (Exception e)
			{
			    this.EventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
				this.LoadRackStop();
				Environment.Exit(1);

			}
		}

		private void LoadRackStop()
		{
			try
			{
				if (this.loadRackManager != null)
				{
					RemotingServices.Disconnect(this.loadRackManager);
				    this.loadRackManager.Dispose();
				}

				ChannelServices.UnregisterChannel(this.channel);

			    this.EventLog.WriteEntry("Stopped");
			}
			catch (Exception e)
			{
			    this.EventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
			}
		}



		public void LoadRack()
		{
			try
			{
				/* todo - access the connection string
				ConsolidatedDAClass consolidatedDA=new ConsolidatedDAClass();
		  SqlDependency.Start(consolidatedDA.ConnectionString);
				*/

			    this.LoadRackStart();

				WaitHandle[] events = { this.killEvent };

			    while (0 != (WaitHandle.WaitAny(events, 1000, true)))
				{
				}

			    this.LoadRackStop();
				//SqlDependency.Stop(consolidatedDA.ConnectionString);
			}
			catch (Exception e)
			{
			    this.EventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
			}
		}
	}
}
