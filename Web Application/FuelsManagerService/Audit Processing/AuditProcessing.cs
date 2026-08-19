// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuditProcessing.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//    Provides the ability to process audit tables in a separate thread.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManagerService
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;
	using System.Threading;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	class AuditProcessing
	{
		#region Constants and Fields

		/// <summary>
		/// Stops processing
		/// </summary>
		private static readonly ManualResetEvent KillEvent = new ManualResetEvent(false);

		/// <summary>
		/// The thread responsible for processing
		/// </summary>
		private static Thread processThread = null;

		#endregion

		#region Methods

		/// <summary>
		/// Starts execution of the ProcessThread.
		/// </summary>
		/// <param name="security">
		/// Contains Security Information.
		/// </param>
		internal static void StartProcessThread(SecurityClass security, TimeSpan waitInterval)
		{
			processThread = new Thread(() => ProcessScan(security, waitInterval));
			processThread.Start();
		}

		/// <summary>
		///     Stops the ProcessThread.
		/// </summary>
		internal static void StopProcessThread()
		{
			KillEvent.Set();

			if (processThread != null)
			{
				processThread.Join();
			}
		}

		/// <summary>
		/// Processes the scan.
		/// </summary>
		/// <param name="security">The security.</param>
		private static void ProcessScan(SecurityClass security, TimeSpan waitInterval)
		{
			WaitHandle[] events = { KillEvent };
			DateTime start = DateTime.Now;
			TimeSpan tenMinutes = TimeSpan.FromMinutes(10);

			while (0 != WaitHandle.WaitAny(events, waitInterval, true))
			{
				try
				{
					FMChannelHelper.MakeCall<IAuditLogs>(auditLogs =>
					{
						auditLogs.ProcessPendingAudits(security);
					});

					if (start.Date != DateTime.Now.Date)
					{
						ProcessPurgeAudits(security);
						start = DateTime.Now;
					}
				}
				catch (Exception ex)
				{
					FuelsManagerServiceLogger.Instance.LogError(ex);
				}
			}
		}

		private static void ProcessPurgeAudits(SecurityClass security)
		{
			Dictionary<Guid, int?>  siteRetentionList = FMChannelHelper.MakeCall<IAuditLogs, Dictionary<Guid, int?>>(  x => { return x.GetAllSiteRetentionForShadowTable(security); });

			// these are looped here outside of transaction so that the purging can be successful per site instead of timing for all sites and rollingback without actually deleting
			foreach (Guid siteGuid in siteRetentionList.Keys)
			{
				if (siteRetentionList[siteGuid].HasValue)
				{
					FMChannelHelper.MakeCall<IAuditLogs>(
						auditLogs =>
						{
							auditLogs.ProcessAuditPurgeOld(security, siteGuid, siteRetentionList[siteGuid].Value);
						});
				}

				FMChannelHelper.MakeCall<IAuditLogs>(
					auditLogs =>
					{
						auditLogs.PurgeShadowSiteTable(security, siteGuid);
					});
			}
		}
		
		#endregion
	}
}
