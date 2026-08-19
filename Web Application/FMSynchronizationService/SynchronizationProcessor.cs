// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SynchronizationProcessor.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	The synchronization processor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMSynchronizationService
{
	using System;
	using System.Configuration;
	using System.Diagnostics;
	using System.Threading;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	/// <summary>
	/// The synchronization processor.
	/// </summary>
	public class SynchronizationProcessor
	{
		#region Private Attributes

		private const int TerminateServiceEventHandle = 0;
		private const int ManualSyncEventHandle = 1;

		/// <summary>
		/// The _ manual synchronization event.
		/// </summary>
		private static readonly AutoResetEvent _ManualSynchronizationEvent = new AutoResetEvent(false);

		/// <summary>
		/// The _ stop synchronization event.
		/// </summary>
		private static readonly AutoResetEvent _StopSynchronizationEvent = new AutoResetEvent(false);


		/// <summary>
		/// Indicates whether the <see cref="SynchronizationProcessor"/> is currently in the process of synchronizing data.
		/// This applies to periodic, manual, scheduled and resynchronization request types.
		/// </summary>
		private static bool _SynchronizationInProgress = false;

		/// <summary>
		/// Indicates if a synchronization stop request has been issued to the <see cref="SynchronizationProcessor"/>.
		/// This flag will be reset once the synchronization processor has stopped.
		/// </summary>
		private static bool _SynchronizationStopRequested = false;

		/// <summary>
		/// Setting this event will instruct the <see cref="SynchronizationProcessor"/> to notify the core synchronization engine that
		/// any active synchronization session should be terminated.
		/// </summary>
		private static ManualResetEvent _KillEvent = new ManualResetEvent(false);

		/// <summary>
		/// The main FuelsManager Synchronization Service thread that detects synchronization request events and handles periodic
		/// synchronization requests.
		/// </summary>
		private static Thread _ProcessThread = new Thread(ProcessScan);

		/// <summary>
		/// A handle to the Windows Event Log so that service errors can be logged.
		/// </summary>
		private static EventLog _EventLog = new EventLog("Application", ".", "FMSynchronizationService.SynchronizationProcessor");

		/// <summary>
		/// Contains the security context of the user that initiated the manual synchronization request.
		/// </summary>
		private static SecurityClass _ManuallyInitiatedSecurity = null;

		/// <summary>
		/// Contains the Site or Site Group that should be used when executing the manual synchronization request.
		/// </summary>
		private static SyncSelectedSiteDO _ManuallyInitiatedSite = null;

		/// <summary>
		/// Contains any certificate (if available) that existed during the initial manual synchronization request within FuelsManager.
		/// </summary>
		private static byte[] _ManuallyInitiatedPassThruCertificate = null;

		/// <summary>
		/// Identifies the type of synchronization request to perform during the manual synchronization process.
		/// <value>SYNCREQUESTTYPE.INIT</value>, <value>SYNCREQUESTTYPE.MANUAL</value> and <value>SYNCREQUESTTYPE.RESYNC</value> are 
		/// the most common request types for manual synchronization.
		/// </summary>
		private static SYNCREQUESTTYPE _ManuallyInitiatedSyncRequestType = SYNCREQUESTTYPE.INIT;

		/// <summary>
		/// Contains the security context of the user that initiated the stop synchronization request.
		/// </summary>
		private static SecurityClass _StopSynchronizationSecurity = null;

		/// <summary>
		/// Contains the <see cref="DateTimeOffset"/> of the last time this system synchronized with the enterprise.
		/// </summary>
		private static DateTimeOffset LastSyncEvent = DateTimeOffset.Now;


		/// <summary>
		/// The synchronize controller
		/// </summary>
		private static SyncController syncController = new SyncController();

		#endregion Private Attributes

		#region Synchronization Control Methods
		/// <summary>
		/// Starts execution of the ProcessThread.
		/// </summary>
		internal static void StartProcessThread()
		{
			SynchronizationProcessor._ProcessThread.Start();
		}

		/// <summary>
		/// Stops the ProcessThread.
		/// </summary>
		internal static void StopProcessThread()
		{
			SynchronizationProcessor.SystemStopSynchronization();
			SynchronizationProcessor._KillEvent.Set();
			if (SynchronizationProcessor._ProcessThread.ThreadState != System.Threading.ThreadState.Unstarted)
			{
				SynchronizationProcessor._ProcessThread.Join();
			}
		}

		/// <summary>
		/// Sets the state of the ManualSynchronizationEvent to signaled.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="selectedSite">
		/// The selected Site ID.
		/// </param>
		/// <param name="passThruCertificate">
		/// The pass through Certificate.
		/// </param>
		/// <param name="requestType">
		/// The request Type.
		/// </param>
		internal static void SetManualSynchronizationEvent(SecurityClass security, SyncSelectedSiteDO selectedSite, byte[] passThruCertificate, SYNCREQUESTTYPE requestType)
		{
			if (SynchronizationProcessor._SynchronizationInProgress)
			{
				return;
			}

			SynchronizationProcessor._ManuallyInitiatedSite = selectedSite;
			SynchronizationProcessor._ManuallyInitiatedSecurity = security;
			SynchronizationProcessor._ManuallyInitiatedPassThruCertificate = passThruCertificate;
			SynchronizationProcessor._ManuallyInitiatedSyncRequestType = requestType;

			SynchronizationProcessor._ManualSynchronizationEvent.Set();
			SynchronizationProcessor._StopSynchronizationEvent.Reset();
		}

		/// <summary>
		/// The initiate manual offline synchronization.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="selectedSite">
		/// The selected site id.
		/// </param>
		/// <param name="passThruCertificate">
		/// The pass through certificate.
		/// </param>
		/// <param name="requestType">
		/// The request type.
		/// </param>
		/// <param name="startRange">
		/// The start range.
		/// </param>
		/// <param name="endRange">
		/// The end range.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		internal static string InitiateManualOfflineSynchronization(
				SecurityClass security,
				SyncSelectedSiteDO selectedSite,
				byte[] passThruCertificate,
				SYNCREQUESTTYPE requestType,
				DateTimeOffset? startRange,
				DateTimeOffset? endRange)
		{
			if (SynchronizationProcessor._SynchronizationInProgress)
			{
				return string.Empty;
			}

			return SynchronizationProcessor.ProcessOfflineSynchronization(security, selectedSite, passThruCertificate, requestType, startRange, endRange);
		}

		/// <summary>
		/// Signals the synchronization engine to stop any active synchronization session.  Synchronization of groups that are already in progress
		/// will be allowed to complete in order to avoid data inconsistencies.  This is an asynchronous stop request.
		/// </summary>
		internal static void SystemStopSynchronization()
		{
			if (!_SynchronizationInProgress)
			{
				return;
			}

			_SynchronizationStopRequested = true;

			syncController.SystemStopSynchronization();
		}



		/// <summary>
		/// Signals the synchronization engine to stop any active synchronization session.  Synchronization of groups that are already in progress
		/// will be allowed to complete in order to avoid data inconsistencies.  This is an asynchronous stop request.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		internal static void StopSynchronization(SecurityClass security)
		{
			if (!_SynchronizationInProgress)
			{
				return;
			}

			_SynchronizationStopRequested = true;

			_StopSynchronizationSecurity = security;

			SynchronizationProcessor._StopSynchronizationEvent.Set();

			syncController.StopSynchronization(security);

			return;
		}

		/// <summary>
		/// The get service state.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <returns>
		/// The <see cref="SyncServiceStateDO"/>.
		/// </returns>
		internal static SyncServiceStateDO GetServiceState(SecurityClass security)
		{
			SyncServiceStateDO serviceState = null;

			if (SynchronizationProcessor._SynchronizationInProgress)
			{
				serviceState = new SyncServiceStateDO()
				{
					AsOfDate = DateTimeOffset.Now,
					CurrentSessionIsSynchronizing = true,
					SyncServiceState = SYNCSERVICESTATE.IN_PROGRESS
				};
			}
			else
			{
				serviceState = syncController.GetSynchronizationState(security);
			}

			return serviceState;
		}

		/// <summary>
		/// The resynchronization required.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		internal static bool ResynchronizationRequired(SecurityClass security)
		{
			return syncController.ResynchronizationRequired(security);
		}

		/// <summary>
		/// This method determines which type of synchronization request type to use for the manual synchronization request.  Normal day to day operations
		/// would utilize a Manual request type.  However; the overall application may require special synchronization requests to be performed before 
		/// normal day to day operations can continue.  These include <value>SYNCREQUESTTYPE.INIT</value> and <value>SYNCREQUESTTYPE.RESYNC</value>.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <returns>
		/// Returns the <see cref="SYNCREQUESTTYPE"/> that needs to be performed during the next synchronization request.
		/// </returns>
		/// <remarks>
		/// When a new client node is installed, an initial synchronization request must be performed.  Since the client node does not have configured users, 
		/// sites and permissions, the synchronization process will revert to the settings stored in the client synchronization configuration.
		/// After an application update, a resynchronization request may be required to retrieve data that was previously not supported by the client node.
		/// </remarks>
		internal static SYNCREQUESTTYPE GetSynchronizationRequestType(SecurityClass security)
		{
			return syncController.GetSynchronizationRequestType(security);
		}

		/// <summary>
		/// This method determines which SiteID to use for this synchronization request.
		/// Depending on the current request type, the Site ID that will be used for synchronization may be the Root Site specified in the client 
		/// synchronization settings instead of the currently selected Site from the FuelsManager user interface.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="requestType">
		/// The request type.
		/// </param>
		/// <returns>
		/// The <see cref="SyncSelectedSiteDO"/>.
		/// </returns>
		internal static SyncSelectedSiteDO GetSynchronizationSiteId(SecurityClass security, SYNCREQUESTTYPE requestType)
		{
			return syncController.GetSynchronizationSiteId(security, requestType);
		}

		#endregion Synchronization Control Methods

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

				// Interval at which this service will stop waiting for user initiated events and check to see if any periodic synchronization
				// event should be performed.  This can be extended to include synchronization schedules in the future.
				TimeSpan periodicTimeoutCheck;
				TimeSpan syncAutoRetryDelayTS;

				string periodicPollingInterval = ConfigurationManager.AppSettings["periodicPollingInterval"];
				string syncAutoRetryDelay = ConfigurationManager.AppSettings["syncAutoRetryDelay"];

				if (periodicPollingInterval != null && TypeHelper.IsNumeric(periodicPollingInterval))
				{
					periodicTimeoutCheck = TimeSpan.FromMinutes(Convert.ToInt32(periodicPollingInterval));
				}
				else
				{
					periodicTimeoutCheck = TimeSpan.FromMinutes(15);
				}

				if (syncAutoRetryDelay != null && TypeHelper.IsNumeric(syncAutoRetryDelay))
				{
					syncAutoRetryDelayTS = TimeSpan.FromMinutes(Convert.ToInt32(syncAutoRetryDelay));
				}
				else
				{
					syncAutoRetryDelayTS = TimeSpan.FromMinutes(5);
				}

				bool started = false;
				int retryCount = 0;
				ushort version = 0;

				while (!started)
				{
					try
					{
						FMChannelHelper.MakeCall<ISyncControllerProcessor>(x => x.CleanupAbandonedSyncController(security));

						started = true;
					}
					catch (Exception)
					{
						retryCount++;
						if (retryCount > 12)
						{
							throw new Exception("FMSynchronization Service Failed During Start.");
						}

						Thread.Sleep(10000);
					}
				}

                FMChannelHelper.MakeCall<IHardwareKey, ushort>(x => x.CheckActivatedLicenceVersion());


				WaitHandle[] waitHandles = { _KillEvent, _ManualSynchronizationEvent };

				int waitResult;

				// Revise Timeout to implement periodic or schedule synchronization
				// If we don't hear anything from any event, we need to fall through and check to see if any periodic synchronization requests should be performed.
				//
				// As long as we don't get a KillEvent then we'll keep waiting
				while (TerminateServiceEventHandle != (waitResult = WaitHandle.WaitAny(waitHandles, periodicTimeoutCheck, true)))
				{
					try
					{
						switch (waitResult)
						{
							// Manual Synchronization
							case ManualSyncEventHandle:

								EnterpriseSynchronizationEvents enterpriseSynchronizationEvents = new EnterpriseSynchronizationEvents();

								try
								{

									SynchronizationProcessor._SynchronizationInProgress = true;

									FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
										alarmAndEventChannel =>
										{
											alarmAndEventChannel.Add(
												SynchronizationProcessor._ManuallyInitiatedSecurity,
												enterpriseSynchronizationEvents.ManualSynchronizationInitiatedEvent(SynchronizationProcessor._ManuallyInitiatedSecurity.UserID));
										});

									// Run the synchronization process multiple times
									// if we detected synchronization didn't finish
									// OR 
									// An initial synchronization attempt finished but we still detect that we need to perform an initial sync.
									// 
									bool syncFinished = false;
									while (!syncFinished)
									{
										syncFinished = ProcessSynchronization(
											SynchronizationProcessor._ManuallyInitiatedSecurity,
											SynchronizationProcessor._ManuallyInitiatedSite,
											SynchronizationProcessor._ManuallyInitiatedPassThruCertificate,
											SynchronizationProcessor._ManuallyInitiatedSyncRequestType);

										// Only perform the following if a synchronization stop request hasn't been queued.
										if (!SynchronizationProcessor._SynchronizationStopRequested)
										{
											// If we were triggered because of an initial synchronization request and we still detect the system is still
											// in an "init" state; we need to automatically retry synchronization to finish up the initial synchronization process.
											if (SynchronizationProcessor._ManuallyInitiatedSyncRequestType == SYNCREQUESTTYPE.INIT
												&& SynchronizationProcessor.GetSynchronizationRequestType(security) == SYNCREQUESTTYPE.INIT)
											{
												FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
													alarmAndEventChannel =>
													{
														alarmAndEventChannel.Add(
															SynchronizationProcessor._ManuallyInitiatedSecurity,
															enterpriseSynchronizationEvents.InitialSynchronizationAutoResumeEvent(SynchronizationProcessor._ManuallyInitiatedSecurity.UserID));
													});

												WaitHandle[] waithandleArray = { _KillEvent, _StopSynchronizationEvent };

												if (WaitHandle.WaitAny(waithandleArray, syncAutoRetryDelayTS) != WaitHandle.WaitTimeout)
												{
													//killevent or stopevent has been set
													syncFinished = true;
												}
											}
										}
										else
										{
											syncFinished = true;
										}
									}
								}
								catch (Exception ex)
								{
									_EventLog.WriteEntry(ex.StackTrace, EventLogEntryType.Error);
									_EventLog.WriteEntry(string.Format("Manual Synchronization Exception: {0}", ex.Message), EventLogEntryType.Error);

									FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
										alarmAndEventChannel =>
										{
											alarmAndEventChannel.Add(
												_ManuallyInitiatedSecurity,
												enterpriseSynchronizationEvents.SynchronizationErrorEncounteredEvent(
													string.Format("Manual Synchronization Exception: {0}", ex.Message)));
										});
								}
								finally
								{
									LastSyncEvent = DateTimeOffset.Now;
									SynchronizationProcessor._SynchronizationInProgress = false;
									SynchronizationProcessor._SynchronizationStopRequested = false;
									SynchronizationProcessor._StopSynchronizationSecurity = null;

									FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
										alarmAndEventChannel =>
										{
											alarmAndEventChannel.Add(
												_ManuallyInitiatedSecurity,
												enterpriseSynchronizationEvents.ManualSynchronizationCompleteEvent(_ManuallyInitiatedSecurity.UserID));
										});
								}
								break;

							case WaitHandle.WaitTimeout:
								SecurityClass innerSecurityUser = GetServiceSecurityInstance();

								if (!SynchronizationProcessor._SynchronizationInProgress)
								{
									SyncServiceStateDO syncServiceStateDO = GetServiceState(innerSecurityUser);

									if (syncServiceStateDO.SyncServiceState == SYNCSERVICESTATE.IN_PROGRESS)
									{
										FMChannelHelper.MakeCall<ISyncControllerProcessor>(x => x.CleanupAbandonedSyncController(security));
										syncServiceStateDO = GetServiceState(innerSecurityUser);
									}

									if (syncServiceStateDO.SyncServiceState == SYNCSERVICESTATE.READY)
									{
										CheckForAutomatedSynchronizationEvents(innerSecurityUser);
									}
								}

								break;

							default:
								break;
						}
					}
					catch (Exception e)
					{
						_EventLog.WriteEntry(e.StackTrace, EventLogEntryType.Error);
						_EventLog.WriteEntry(e.Message, EventLogEntryType.Error);
					}
				}
			}
			catch (Exception e)
			{
				_EventLog.WriteEntry(e.StackTrace, EventLogEntryType.Error);
				_EventLog.WriteEntry(e.Message, EventLogEntryType.Error);
			}
		}
		#endregion Main Thread

		#region Synchronization Execution Methods

		#region Manual Synchronization Methods
		/// <summary>
		/// Initiates a call into the main SyncController which contains all of the synchronization sequencing logic
		/// for a manually initiated synchronization request.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="selectedSite">
		/// The selected Site Id.
		/// </param>
		/// <param name="passThruClientCertificate">
		/// The pass through Client Certificate.
		/// </param>
		/// <param name="requestType">
		/// The request Type.
		/// </param>
		private static bool ProcessSynchronization(SecurityClass security, SyncSelectedSiteDO selectedSite, byte[] passThruClientCertificate, SYNCREQUESTTYPE requestType)
		{
			try
			{
				return syncController.ExecuteDatabaseSynchronization(security, selectedSite, passThruClientCertificate, requestType);
			}
			catch (Exception e)
			{
				_EventLog.WriteEntry(e.StackTrace, EventLogEntryType.Error);
				_EventLog.WriteEntry(e.Message, EventLogEntryType.Error);
			}

			return false;
		}

		/// <summary>
		/// Initiates a call into the main SyncController which contains all of the synchronization sequencing logic
		/// for a manually initiated OFFLINE synchronization request.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="selectedSite">
		/// The selected Site Id.
		/// </param>
		/// <param name="passThruClientCertificate">
		/// The pass through Client Certificate.
		/// </param>
		/// <param name="requestType">
		/// The request Type.
		/// </param>
		/// <param name="startRange">
		/// The start Range.
		/// </param>
		/// <param name="endRange">
		/// The end Range.
		/// </param>
		/// <returns>
		/// Returns the full path and filename of the generated synchronization file, otherwise; an empty string if the file was not generated.
		/// </returns>
		private static string ProcessOfflineSynchronization(SecurityClass security, SyncSelectedSiteDO selectedSite, byte[] passThruClientCertificate, SYNCREQUESTTYPE requestType, DateTimeOffset? startRange, DateTimeOffset? endRange)
		{
			string outputFile = string.Empty;

			try
			{
				SynchronizationProcessor._SynchronizationInProgress = true;

				outputFile = syncController.ExecuteOfflineDatabaseSynchronization(security, selectedSite, passThruClientCertificate, requestType, startRange, endRange);
			}
			catch (Exception e)
			{
				_EventLog.WriteEntry(e.StackTrace, EventLogEntryType.Error);
				_EventLog.WriteEntry(e.Message, EventLogEntryType.Error);
			}
			finally
			{
				_SynchronizationInProgress = false;
			}

			return outputFile;
		}
		#endregion Manual Synchronization Methods

		#region Periodic Synchronization Methods
		/// <summary>
		/// Determines if any periodic synchronization requests need to be performed.  Since the SynchronizationProcessor 
		/// doesn't have knowledge regarding which site or site groups to check (hosted or reference), we will ask
		/// the SyncController to determine if something needs to be done and then kick it off if there is work to be done.
		/// </summary>
		/// <param name="security">
		/// The current security context of the caller.
		/// </param>
		private static void CheckForAutomatedSynchronizationEvents(SecurityClass security)
		{

			try
			{
				bool hasPendingAutomatedSyncEvents = false;

				hasPendingAutomatedSyncEvents = syncController.HasPendingAutomatedSynchronizationEvents(security, LastSyncEvent);

				if (hasPendingAutomatedSyncEvents)
				{
					hasPendingAutomatedSyncEvents = false;

					SyncSelectedSiteDO selectedSite = SynchronizationProcessor.GetSynchronizationSiteId(
						security, SYNCREQUESTTYPE.PERIODIC);

					security.SiteID = selectedSite.SiteID;
					security.SiteGuid = selectedSite.SiteGuid;

					var enterpriseSynchronizationEvents = new EnterpriseSynchronizationEvents();

					try
					{
						FMChannelHelper.MakeCall<IAlarmAndEventLogs>(alarmAndEventChannel =>
						{
							alarmAndEventChannel.Add(security, enterpriseSynchronizationEvents.PeriodicSynchronizationInitiatedEvent(security.UserID));
						});

						ProcessPeriodicSynchronization(security, selectedSite);
					}
					catch (Exception ex)
					{
						_EventLog.WriteEntry(ex.StackTrace, EventLogEntryType.Error);
						_EventLog.WriteEntry(string.Format("Periodic Synchronization Exception: {0}", ex.Message), EventLogEntryType.Error);

						FMChannelHelper.MakeCall<IAlarmAndEventLogs>(alarmAndEventChannel =>
						{
							alarmAndEventChannel.Add(security, enterpriseSynchronizationEvents.SynchronizationErrorEncounteredEvent(string.Format("Periodic Synchronization Exception: {0}", ex.Message)));
						});
					}
					finally
					{
						LastSyncEvent = DateTimeOffset.Now;

						FMChannelHelper.MakeCall<IAlarmAndEventLogs>(alarmAndEventChannel =>
						{
							alarmAndEventChannel.Add(security, enterpriseSynchronizationEvents.PeriodicSynchronizationCompleteEvent(security.UserID));
						});
					}
				}
			}
			catch (Exception e)
			{
				_EventLog.WriteEntry(e.StackTrace, EventLogEntryType.Error);
				_EventLog.WriteEntry(e.Message, EventLogEntryType.Error);
			}
		}

		/// <summary>
		/// Initiates a call into the main SyncController which contains all of the synchronization sequencing logic
		/// for a automated periodic synchronization request.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		private static void ProcessPeriodicSynchronization(SecurityClass security, SyncSelectedSiteDO selectedSite)
		{

			try
			{
				SynchronizationProcessor._SynchronizationInProgress = true;

				if (!syncController.ExecutePeriodicDatabaseSynchronization(security, selectedSite))
				{
					// Something wrong?  Is there any benefit to returning a boolean here?
					// session may need to be cleaned
					FMChannelHelper.MakeCall<ISyncControllerProcessor>(x => x.CleanupAbandonedSyncController(security));

				}
			}
			catch (Exception e)
			{
				// session may need to be cleaned
				FMChannelHelper.MakeCall<ISyncControllerProcessor>(x => x.CleanupAbandonedSyncController(security));
				_EventLog.WriteEntry(e.StackTrace, EventLogEntryType.Error);
				_EventLog.WriteEntry(e.Message, EventLogEntryType.Error);
			}
			finally
			{
				SynchronizationProcessor._SynchronizationInProgress = false;
				SynchronizationProcessor._SynchronizationStopRequested = false;
				SynchronizationProcessor._StopSynchronizationSecurity = null;
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
			SecurityClass syncSecurity = new SecurityClass();
			syncSecurity.LoginSiteGuid = Guids.SiteAdminGuid;
			syncSecurity.LoginSiteID = "SiteAdmin";
			syncSecurity.SiteGuid = Guids.SiteAdminGuid;
			syncSecurity.SiteID = "SiteAdmin";
			syncSecurity.UserGuid = Guids.UserAdminGuid;
			syncSecurity.UserID = "SyncService";
			syncSecurity.AddRight(RIGHT.BASE_EXPORT);
			syncSecurity.AddRight(RIGHT.INTERFACE_IMPORT);
			syncSecurity.AddRight(RIGHT.VIEW_SYNC_CONFIG_CLIENT_SETTINGS);
			syncSecurity.AddRight(RIGHT.PERFORM_SYNCHRONIZATION);

			return syncSecurity;
		}
		#endregion Periodic Synchronization Methods

		#endregion Synchronization Execution Methods
	}
}
