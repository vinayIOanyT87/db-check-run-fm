/// --------------------------------------------------------------------------------------------------------------------
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
	using System.ServiceModel;
	using System.Threading;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	internal static class UserAccountCleanup
    {

		/// <summary>
		/// This event is signaled when a new alarm or event log record is created
		/// </summary>
		private static readonly AutoResetEvent ArchiveUsersEvent = new AutoResetEvent(false);

		/// <summary>
		/// Stops processing
		/// </summary>
		private static readonly ManualResetEvent KillEvent = new ManualResetEvent(false);

		/// <summary>
		/// The thread responsible for processing
		/// </summary>
		private static Thread processThread = null;

		/// <summary>
		/// Starts execution of the ProcessThread.    Then it continually loops in processScan
		/// and looks for users to disable and users to archive.
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
		/// This is the ProcessThread worker method and is executed within the context of
		/// the ProcessThread.  First it disables inactive users.
		/// and then archives users that have been inactive more than the site configured period.
		/// </summary>
		/// <param name="security">
		/// Contains security information.
		/// </param>
		private static void ProcessScan(SecurityClass security, TimeSpan waitInterval)
		{

			WaitHandle[] events = { KillEvent };
			TimeSpan tenMinutes = TimeSpan.FromMinutes(10);

			while (0 != WaitHandle.WaitAny(events, waitInterval, true))
			{
				try
				{
					FMChannelHelper.MakeCall<IUsers>(
						users =>
						{
							((IClientChannel)users).OperationTimeout = tenMinutes;
							users.DisableUser(security);
						});
					
				}
				catch (Exception ex)
				{
					FuelsManagerServiceLogger.Instance.LogError(ex);
				}

				try
				{
					FMChannelHelper.MakeCall<IUsers>(
						users =>
						{
							((IClientChannel)users).OperationTimeout = tenMinutes;
							users.ArchiveUser(security);
						});
				}
				catch (Exception ex)
				{
					FuelsManagerServiceLogger.Instance.LogError(ex);
				}
			}

		}


    }
}
