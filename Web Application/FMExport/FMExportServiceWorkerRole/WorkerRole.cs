// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkerRole.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//  Runs the FMExport service in Azure
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMExportServiceWorkerRole
{
	using System.Diagnostics;
	using System.Threading;
	using FMExportService;
	using Microsoft.WindowsAzure.ServiceRuntime;

	/// <summary>
	/// Runs the FMExport service in Azure
	/// </summary>
	public class WorkerRole : RoleEntryPoint
	{
		/// <summary>
		/// The FMExport service instance
		/// </summary>
		private FMExportService service = null;

		/// <summary>
		/// Run is the main method for a worker role. It should never return - if it does, the role gets recycled 
		/// </summary>
		public override void Run()
		{
			Trace.WriteLine("FMExportService entry point called", "Information");

			while (true)
			{
				// Wait 30 minutes between "alive" messages.
				Thread.Sleep(30 * 60 * 1000);
				Trace.WriteLine("Working", "Information");
			}
		}

		/// <summary>
		/// Starts the worker role instance and the FMExport service.
		/// </summary>
		/// <returns>True if initialization succeeds, false otherwise</returns>
		public override bool OnStart()
		{
			if (this.service == null)
			{
				this.service = new FMExportService();
			}

			this.service.ProxyStart();

			return base.OnStart();
		}

		/// <summary>
		/// Stops the worker role instance and the FMExport service.
		/// </summary>
		public override void OnStop()
		{
			if (this.service != null)
			{
				this.service.ProxyStop();
			}

			base.OnStop();
		}
	}
}
