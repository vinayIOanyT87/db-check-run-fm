// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMSynchronizationService.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   The fm synchronization service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMSynchronizationService
{
    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
	using System;
	using System.Diagnostics;
	using System.ServiceModel;
	using System.ServiceProcess;
	using System.Threading;

	/// <summary>
	/// The fm synchronization service.
	/// </summary>
	public partial class FMSynchronizationService : ServiceBase
	{
		private ServiceHost synchronizationServices = null;

		public FMSynchronizationService()
		{
			this.AutoLog = false;
			this.CanShutdown = true;

			this.InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			this.Start();
		}

		protected override void OnStop()
		{
			this.Exit();
		}

		/// <summary>
		/// Start the service
		/// </summary>
		public void Start()
		{
			try
			{
                FMChannelHelper.MakeCall<IHardwareKey, ushort>(x => x.CheckActivatedLicenceVersion());

				// Start the Synchronization Processor
				SynchronizationProcessor.StartProcessThread();

				//Start the WCF service
				this.synchronizationServices = new ServiceHost(typeof(SynchronizationServices));
				this.synchronizationServices.Open();

				this.EventLog.WriteEntry("FMSynchronization Service Started", EventLogEntryType.Information);
			}
			catch (Exception ex)
			{
				this.EventLog.WriteEntry(ex.Message, EventLogEntryType.Error);
				Exit();
				Environment.Exit(1);
			}
		}

		/// <summary>
		/// Stop the service and its components
		/// </summary>
		public void Exit()
		{
			try
			{
				//Shut down the WCF service
				if (this.synchronizationServices != null)
					this.synchronizationServices.Close();
				SynchronizationProcessor.StopProcessThread();

				this.EventLog.WriteEntry("FMSynchronization Service Stopped", EventLogEntryType.Information);
			}
			catch (Exception ex)
			{
				this.EventLog.WriteEntry(ex.StackTrace, EventLogEntryType.Error);
				this.EventLog.WriteEntry(ex.Message, EventLogEntryType.Error);
			}
		}
	}
}
