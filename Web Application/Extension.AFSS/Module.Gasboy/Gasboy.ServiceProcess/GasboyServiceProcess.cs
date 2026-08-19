// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GasboyServiceProcess.cs" company="Varec, Inc.">
//  Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//  The processor class executes on an isolated thread and functions as a daemon that's responsible for coordinating
//  period activities along with external service requests 
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Afss.Module.Gasboy.ServiceProcess 
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Diagnostics;
	using System.Threading;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using FuelsManager.Afss.BusinessObjects.Constants;
	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.AlarmAndEvents;
	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.BusinessInterfaces;
	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.ChannelFactories;
	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects;
	using FuelsManager.Afss.Module.Gasboy.OrCU;

	/// <summary>
	/// The automated fuel service station service process for Gasboy Stations.
	/// This class provides periodic and scheduled processing by providing a service 
	/// thread that calls into the GasboyStationServices when it's time for automated 
	/// processing activities to take place.
	/// </summary>
	public class GasboyServiceProcess
	{
		#region Private Attributes

		/// <summary>
		/// The manual do work event.
		/// </summary>
		private static readonly ManualResetEvent ConfigurationChangeEvent = new ManualResetEvent(false);

		/// <summary>
		/// Contains the security context of the user that initiated the manual Gasboy request.
		/// </summary>
		private static SecurityClass Security = null;

		/// <summary>
		/// Contains the <see cref="DateTimeOffset"/> of the last time this system communicated with any Gasboy system.
		/// </summary>
		private static DateTimeOffset LastGasboyEventDateTime = DateTimeOffset.Now;

		/// <summary>
		/// Indicates whether the <see cref="GasboyServiceProcess"/> is currently in the process of communicating with the external Gasboy units.
		/// This applies to periodic, manual and scheduled request types.
		/// </summary>
		private static bool WorkInProgress = false;

		/// <summary>
		/// Setting this event will instruct the <see cref="GasboyServiceProcess"/> to notify the core Gasboy engine that
		/// any active Gasboy session should be terminated.
		/// </summary>
		private static readonly ManualResetEvent KillEvent = new ManualResetEvent(false);

		/// <summary>
		/// The main FuelsManager Gasboy Service thread that detects Gasboy request events and handles periodic
		/// download requests.
		/// </summary>
		private static Thread ProcessThread = null;

		/// <summary>
		/// A handle to the Windows Event Log so that service errors can be logged.
		/// </summary>
		private static readonly EventLog EventLog = new EventLog("Application", ".", "GasboyServiceProcess");

		/// <summary>
		/// The Gasboy communications controller
		/// </summary>
		private static readonly GasboyController GasboyController = new GasboyController();

		/// <summary>
		/// Contains the security context of the user that initiated the stop Gasboy request.
		/// </summary>
		private static SecurityClass StopGasboySecurity = null;

		/// <summary>
		/// Contains the security context before the station connection test started
		/// </summary>
		private static SecurityClass _GasboyAutomatedEventsSecurity = null;


		#endregion Private Attributes

		#region Gasboy Processor Control Methods
		/// <summary>
		/// Starts execution of the ProcessThread.
		/// </summary>
		public static void StartProcessThread()
		{
			if (null == GasboyServiceProcess.ProcessThread)
			{
				GasboyServiceProcess.ProcessThread = new Thread(ProcessScan);
				GasboyServiceProcess.ProcessThread.Start();
			}
		}

		/// <summary>
		/// Stops the ProcessThread.
		/// </summary>
		public static void StopProcessThread()
		{
			if (null != GasboyServiceProcess.ProcessThread)
			{
				GasboyServiceProcess.SystemStopGasboyCommunications();
				GasboyServiceProcess.KillEvent.Set();
				GasboyServiceProcess.ProcessThread.Join();

				GasboyServiceProcess.ProcessThread = null;

				GasboyServiceProcess.KillEvent.Reset();
			}
		}

		/// <summary>
		/// Notifies the configuration changes.
		/// </summary>
		public static void NotifyConfigurationChanges()
		{
			GasboyServiceProcess.ConfigurationChangeEvent.Set();

			return;
		}

		/// <summary>
		/// Signals the synchronization engine to stop any active synchronization session.  Synchronization of groups that are already in progress
		/// will be allowed to complete in order to avoid data inconsistencies.  This is an asynchronous stop request.
		/// </summary>
		public static void SystemStopGasboyCommunications()
		{
			if (!WorkInProgress)
			{
				return;
			}

			var security = GetServiceSecurityInstance();

			// GasboyController.StopDataTransfer(security);
		}

		/// <summary>
		/// Signals the Gasboy engine to stop any active Gasboy session.  Communications with Gasboy units that are already in progress
		/// will be allowed to complete in order to avoid data inconsistencies.  This is an asynchronous stop request.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		public static void StopGasboyCommunications(SecurityClass security)
		{
			if (!WorkInProgress)
			{
				return;
			}

			StopGasboySecurity = security;

			// GasboyController.StopDataTransfer(security);
			return;
		}

		/// <summary>
		/// The get service state.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <returns>
		/// The <see cref="GasboyServiceState"/>.
		/// </returns>
		public static GasboyServiceState GetServiceState(SecurityClass security)
		{
			GasboyServiceState processState = null;

			if (GasboyServiceProcess.WorkInProgress)
			{
				processState = new GasboyServiceState()
										{
											AsOfDate = DateTimeOffset.Now,
											WorkInProgress = true,
											ServiceState = ExternalStationServiceProcessState.InProgress
										};
			}
			else
			{
				processState = new GasboyServiceState()
				{
					AsOfDate = DateTimeOffset.Now,
					WorkInProgress = false,
					ServiceState = ExternalStationServiceProcessState.Ready
				};
			}

			return processState;
		}

		#endregion Gasboy Processor Control Methods

		#region Main Thread
		/// <summary>
		/// This is the ProcessThread worker method and is executed within the syncContext of
		/// the ProcessThread.
		/// </summary>
		private static void ProcessScan()
		{
			try
			{
				var security = new SecurityClass();
				//FMChannelHelper.MakeCall<ISyncControllerProcessor>(
				//    x => x.CleanupAbandonedSyncController(security));

				// Interval at which this service will stop waiting for user initiated events and check to see if any periodic Gasboy download
				// event should be performed.
				TimeSpan periodicTimeoutCheck;
				string periodicPollingInterval = ConfigurationManager.AppSettings["periodicPollingInterval"];

				if (periodicPollingInterval != null && TypeHelper.IsNumeric(periodicPollingInterval))
				{
					periodicTimeoutCheck = TimeSpan.FromMinutes(Convert.ToInt32(periodicPollingInterval));
				}
				else
				{
					periodicTimeoutCheck = TimeSpan.FromMinutes(15);
				}

				WaitHandle[] waitHandles = { KillEvent, ConfigurationChangeEvent };

				int waitResult;

				// Revise Timeout to implement periodic or schedule Gasboy downloads
				// If we don't hear anything from any event, we need to fall through and check to see if any periodic Gasboy requests should be performed.
				//
				// As long as we don't get a KillEvent then we'll keep waiting
				while (0 != (waitResult = WaitHandle.WaitAny(waitHandles, periodicTimeoutCheck, true)))
				{
					try
					{
						switch (waitResult)
						{
							// Manual Gasboy Event
							case 1:
								GasboyCommunicationEvents gasboyCommunicationEventsEvents =
									new GasboyCommunicationEvents();

								GasboyServiceProcess.WorkInProgress = true;

								try
								{
									FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
										alarmAndEventChannel =>
										alarmAndEventChannel.Add(
											Security,
											gasboyCommunicationEventsEvents.ManualGasboyDownloadInitiatedEvent(
												Security.UserID)));

									GasboyServiceProcess.LoadConfigurationSettings(
										GasboyServiceProcess.Security);
								}
								catch (Exception ex)
								{
									FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
										alarmAndEventChannel =>
										alarmAndEventChannel.Add(
											Security,
											gasboyCommunicationEventsEvents.GasboyDownloadErrorEncounteredEvent(
												string.Format(
													"Load Configuration Change Exception: {0}",
													ex.Message))));
								}
								finally
								{
									LastGasboyEventDateTime = DateTimeOffset.Now;
									GasboyServiceProcess.WorkInProgress = false;

									FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
										alarmAndEventChannel =>
										alarmAndEventChannel.Add(
											Security,
											gasboyCommunicationEventsEvents.ManualGasboyDownloadCompleteEvent(
												Security.UserID)));
								}
								break;

							case WaitHandle.WaitTimeout:
								SecurityClass innerSecurityUser = GetServiceSecurityInstance();

								if (!GasboyServiceProcess.WorkInProgress)
								{
									GasboyServiceState serviceStateDO = GetServiceState(innerSecurityUser);

									if (serviceStateDO.ServiceState == ExternalStationServiceProcessState.Ready)
									{
										CheckForAutomatedGasboyEvents(innerSecurityUser);
									}
								}

								break;

							default:
								break;
						}
					}
					catch (Exception e)
					{
						EventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
					}
				}
			}
			catch (Exception e)
			{
				EventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
			}
		}
		#endregion Main Thread

		#region Gasboy Communications Execution Methods

		#region Gasboy Configuration

		public static void LoadConfigurationSettings(
			SecurityClass security)
		{
			try
			{
				// Reload configuration settings
			}
			catch (Exception e)
			{
				EventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
			}
			finally
			{
			}

			return;
		}

		#endregion Gasboy Configuration

		#region Periodic Gasboy Communications Methods
		/// <summary>
		/// Determines if any periodic Gasboy work requests need to be performed.
		/// </summary>
		/// <param name="security">
		/// The current security context of the caller.
		/// </param>
		private static void CheckForAutomatedGasboyEvents(SecurityClass security)
		{
			try
			{
				//hasPendingAutomatedSyncEvents = false;

				bool hasPendingAutomatedSyncEvents = GasboyController.HasPendingPeriodicDownloadActions(security, LastGasboyEventDateTime);

				if (hasPendingAutomatedSyncEvents)
				{
					hasPendingAutomatedSyncEvents = false;

					List<GasboyStation> gasboyStations = GasboyChannelHelper.MakeCall<IGasboyStations, List<GasboyStation>>(gasboyChannel => gasboyChannel.Enumerate(security));

					var gasboyCommunicationEvents = new GasboyCommunicationEvents();

					try
					{
						FMChannelHelper.MakeCall<IAlarmAndEventLogs>(alarmAndEventChannel => alarmAndEventChannel.Add(security, gasboyCommunicationEvents.PeriodicGasboyDownloadInitiatedEvent(security.UserID)));

						ProcessPeriodicGasboyDownloads(security, gasboyStations);
					}
					catch (Exception ex)
					{
						FMChannelHelper.MakeCall<IAlarmAndEventLogs>(alarmAndEventChannel => alarmAndEventChannel.Add(security, gasboyCommunicationEvents.GasboyDownloadErrorEncounteredEvent(string.Format("Periodic Gasboy Download Exception: {0}", ex.Message))));
					}
					finally
					{
						LastGasboyEventDateTime = DateTimeOffset.Now;

						FMChannelHelper.MakeCall<IAlarmAndEventLogs>(alarmAndEventChannel => alarmAndEventChannel.Add(security, gasboyCommunicationEvents.PeriodicGasboyDownloadCompleteEvent(security.UserID)));
					}
				}
			}
			catch (Exception e)
			{
				EventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
			}
		}

		/// <summary>
		/// Initiates a call into the main SyncController which contains all of the synchronization sequencing logic
		/// for a automated periodic synchronization request.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		private static void ProcessPeriodicGasboyDownloads(SecurityClass security, IEnumerable<GasboyStation> externalStationList )
		{
			GasboyEvents gasboyEvents = new GasboyEvents();
			GasboyServiceProcess._GasboyAutomatedEventsSecurity = security;
			try
			{
				GasboyServiceProcess.WorkInProgress = true;

				foreach (GasboyStation station in externalStationList)
				{
					if (station.DownloadTransactionsAutomatically)
					{

						security.SiteGuid = station.SiteGuid;
							//transactions should import into the site the station is assigned to, not site admin (as the security object comes from GetServiceSecurityInstance in GasboyServiceProcess.cs)
						security.SiteID = FMChannelHelper.MakeCall<ISites, string>(
							site => site.GetIDNoRefresh(security, station.SiteGuid));
							//transactions should import into the site the station is assigned to, not site admin (as the security object comes from GetServiceSecurityInstance in GasboyServiceProcess.cs)

						FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
							alarmAndEventChannel =>
								{
									alarmAndEventChannel.Add(
										GasboyServiceProcess._GasboyAutomatedEventsSecurity,
										gasboyEvents.GasboyPeriodicTransactionDownloadInitiatedEvent(station.ID));
								});
						GasboyController.ExecutePeriodicDataTransfer(security, station);
					}

					// Make service call into IGasboyStationServices
					//if (!GasboyController.ExecutePeriodicDataTransfer(security, station))
					//{
					// Something wrong?  Is there any benefit to returning a boolean here?
					//}
				}
			}
			catch (Exception e)
			{
				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
									alarmAndEventChannel =>
									{
										alarmAndEventChannel.Add(
											GasboyServiceProcess._GasboyAutomatedEventsSecurity,
											gasboyEvents.GasboyPeriodicTransactionDownloadErrorEvent(security.UserID,e.ToString()));
									});
				throw new ArgumentNullException("security");
				EventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
			}
			finally
			{
				GasboyServiceProcess.WorkInProgress = false;
			}
		}

		/// <summary>
		/// The get service security instance.
		/// </summary>
		/// <returns>
		/// The <see cref="SecurityClass"/>.
		/// </returns>
		private static SecurityClass GetServiceSecurityInstance()
		{
			SecurityClass serviceProcessSecurity = new SecurityClass();
			serviceProcessSecurity.LoginSiteGuid = Guids.SiteAdminGuid;
			serviceProcessSecurity.LoginSiteID = "SiteAdmin";
			serviceProcessSecurity.SiteGuid = Guids.SiteAdminGuid;
			serviceProcessSecurity.SiteID = "SiteAdmin";
			serviceProcessSecurity.UserGuid = Guids.UserAdminGuid;
			serviceProcessSecurity.UserID = "GasboyService";
			serviceProcessSecurity.AddRight(RIGHT.BASE_EXPORT);
			serviceProcessSecurity.AddRight(RIGHT.INTERFACE_IMPORT);
			serviceProcessSecurity.AddRight(RIGHT.MODIFY_TRANSACTION_DATA);
			serviceProcessSecurity.AddRight(RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION);
			return serviceProcessSecurity;
		}
		#endregion Periodic Gasboy Communications Methods

		#endregion Gasboy Communications Execution Methods
	}
}
