using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FuelsManagerService
{
	internal class CleanupPointCalculatorTables
	{
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
		/// Stops the ProcessThread.
		/// </summary>
		internal static void StopProcessThread()
		{
			KillEvent.Set();

			processThread?.Join();
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
			int intervalMinutesToKeep = FuelsManagerSettings.PointCalculatorRunTableCleanupIntervalMinutes; 

			while (WaitHandle.WaitTimeout == WaitHandle.WaitAny(events, waitInterval, true))
			{
				try
				{
					FMChannelHelper.MakeCall<IPointServiceManager>(
						x => x.CleanupPointCalculatorRunsFromDB(security, intervalMinutesToKeep));
				}
				catch (Exception ex)
				{
					FuelsManagerServiceLogger.Instance.LogError(ex);
				}
			}
		}
	}
}

