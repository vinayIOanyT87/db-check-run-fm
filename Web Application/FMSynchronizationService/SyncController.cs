// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncController.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMSynchronizationService
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Diagnostics;
	using System.Linq;
	using System.Security.Cryptography.X509Certificates;
	using System.ServiceModel;
	using System.Threading;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;


	/// <summary>
	///	The sync controller.
	/// </summary>
	public class SyncController
	{
		#region Enumerations

		private enum ActiveSyncControllerState
		{
			NotFound = 0,
			CurrentOwner = 1,
			AnotherOwner = 2
		}

		#endregion Enumerations

		#region Constants and Fields

		/// <summary>
		///	Created and owned by an active SyncController instance
		///	Used to determine if cleanup is required for open sync session log entries.
		/// </summary>
		public const string ActiveSyncControllerMutexName = "ActiveSyncControllerMutex";

		/// <summary>
		///	The event log.
		/// </summary>
		private static EventLog _EventLog = new EventLog("Application", ".", "FMSynchronizationService.SyncController");


		private static Object _ActiveSyncControllerLock = new Object();

		/// <summary>
		///	The current sync context.
		/// </summary>
		private SyncContextFM _CurrentSyncContext = null;

		/// <summary>
		///	Cached list of database version information
		/// </summary>
		private VersionCollection _DBVersionList = null;

		/// <summary>
		///	Cached list of Schema Change History information
		/// </summary>
		private SchemaChangeHistoryCollection _SchemaChangeHistoryList = null;

		private SyncControllerFM _SyncControllerFM = null;

		#endregion Constants and Fields

		#region Constructors and Destructors

		/// <summary>
		///	Initializes a new instance of the <see cref="SyncController" /> class.
		/// </summary>
		public SyncController()
		{
		}

		#endregion Constructors and Destructors

		#region Public Properties

		#endregion Public Properties

		#region Properties

		#endregion Properties

		#region Public Methods and Operators

		/// <summary>
		///	The execute manual database synchronization.
		/// </summary>
		/// <param name="security">
		///	The security.
		/// </param>
		/// <param name="selectedSite">
		///	The selected site id.
		/// </param>
		/// <param name="passThruClientCertificate">
		///	The pass through client certificate.
		/// </param>
		/// <param name="requestType">
		///	The request Type.
		/// </param>
		/// <returns>
		///	The <see cref="bool" />.
		/// </returns>
		public bool ExecuteDatabaseSynchronization(
				SecurityClass security,
				SyncSelectedSiteDO selectedSite,
				byte[] passThruClientCertificate,
				SYNCREQUESTTYPE requestType)
		{
			return this.Synchronize(security, selectedSite, passThruClientCertificate, requestType);
		}

		/// <summary>
		///	The execute offline database synchronization.
		/// </summary>
		/// <param name="security">
		///	The security.
		/// </param>
		/// <param name="selectedSite">
		///	The selected site id.
		/// </param>
		/// <param name="passThruClientCertificate">
		///	The pass through client certificate.
		/// </param>
		/// <param name="requestType">
		///	The request type.
		/// </param>
		/// <param name="startRange">
		///	The start range.
		/// </param>
		/// <param name="endRange">
		///	The end range.
		/// </param>
		/// <returns>
		///	The <see cref="string" />.
		/// </returns>
		public string ExecuteOfflineDatabaseSynchronization(
				SecurityClass security,
				SyncSelectedSiteDO selectedSite,
				byte[] passThruClientCertificate,
				SYNCREQUESTTYPE requestType,
				DateTimeOffset? startRange,
				DateTimeOffset? endRange)
		{
			return this.SynchronizeOffline(
				security,
				selectedSite,
				passThruClientCertificate,
				requestType,
				startRange,
				endRange);
		}

		/// <summary>
		///	The execute periodic database synchronization.
		/// </summary>
		/// <param name="security">
		///	The security.
		/// </param>
		/// <param name="selectedSite">
		///	Selected site to synchronize
		/// </param>
		/// <returns>
		///	The <see cref="bool" />.
		/// </returns>
		public bool ExecutePeriodicDatabaseSynchronization(SecurityClass security, SyncSelectedSiteDO selectedSite)
		{
			return this.Synchronize(security, selectedSite, null, SYNCREQUESTTYPE.PERIODIC);
		}

		/// <summary>
		///	The get synchronization request type.
		/// </summary>
		/// <param name="security">
		///	The security.
		/// </param>
		/// <returns>
		///	The <see cref="SYNCREQUESTTYPE" />.
		/// </returns>
		public SYNCREQUESTTYPE GetSynchronizationRequestType(SecurityClass security)
		{
			if (this.InitialSynchronizationRequired(security))
			{
				return SYNCREQUESTTYPE.INIT;
			}
			else if (this.ResynchronizationRequired(security))
			{
				return SYNCREQUESTTYPE.RESYNC;
			}
			else
			{
				return SYNCREQUESTTYPE.MANUAL;
			}
		}

		/// <summary>
		///	The get synchronization site id.
		/// </summary>
		/// <param name="security">
		///	The security.
		/// </param>
		/// <param name="requestType">
		///	The request type.
		/// </param>
		public SyncSelectedSiteDO GetSynchronizationSiteId(SecurityClass security, SYNCREQUESTTYPE requestType)
		{
			var selectedSite = new SyncSelectedSiteDO();

			try
			{
				SyncClientConfigurationDO clientSyncConfig = GetClientSynchronizationSettings(security);

				if (null != clientSyncConfig && !string.IsNullOrEmpty(clientSyncConfig.RootSiteID))
				{
					selectedSite.SiteID = clientSyncConfig.RootSiteID;
					clientSyncConfig = null;
				}
				else
				{
					// Root Site / SiteGroup ID not specified in client synchronization settings
					SyncHelperFM.WriteConfigurationAlarmAndEvent(security, ErrorConstants.SYNC_ERR_MSG_08003);
				}

				// Depending on the request type, we need to specify the correct SiteID that synchronization should be performed for.
				switch (requestType)
				{
					case SYNCREQUESTTYPE.MANUAL:
						// Synchronization will be based on the site that the user is currently synchronizing from.
						// We will only do this if synchronization was initiated for a Site / Site Group other than SiteAdmin.
						if (!security.SiteID.ToUpperInvariant().Equals("SITEADMIN"))
						{
							selectedSite.SiteID = security.SiteID;
							selectedSite.SiteGuid = security.SiteGuid;
						}

						break;
					default:
						break;
				}

				if (!string.IsNullOrEmpty(selectedSite.SiteID))
				{
					// If we don't have the SiteGuid, let's see if we can resolve it locally.
					var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.GetByID(security, selectedSite.SiteID, false));

					selectedSite.SiteGuid = site.SiteGuid;
				}
			}
			catch (Exception eX)
			{
				// PLACEHOLDER.  NEED TO IDENTIFY SPECIFIC EXCEPTIONS THAT COULD COME BACK.
				_EventLog.WriteEntry(
					string.Format("Unable to determine synchronization Site Id: {0}", eX.Message),
					EventLogEntryType.Error);
				SyncHelperFM.WriteErrorAlarmAndEvent(
					security,
					string.Format("Unable to determine synchronization Site Id: {0}", eX.Message));
			}

			return selectedSite;
		}

		/// <summary>
		///	The get synchronization state.
		/// </summary>
		/// <param name="security">
		///	The security.
		/// </param>
		/// <returns>
		///	The <see cref="SyncServiceStateDO" />.
		/// </returns>
		public SyncServiceStateDO GetSynchronizationState(SecurityClass security)
		{
			return
				FMChannelHelper.MakeCall<ISyncControllerProcessor, SyncServiceStateDO>(
					x => x.GetSynchronizationState(security));
		}

		/// <summary>
		///	The has pending automated synchronization events.
		/// </summary>
		/// <param name="security">
		///	The security.
		/// </param>
		/// <param name="callerLastSyncDateTime">
		///	The caller Last Sync Date Time.
		/// </param>
		/// <returns>
		///	The <see cref="bool" />.
		/// </returns>
		public bool HasPendingAutomatedSynchronizationEvents(
				SecurityClass security,
				DateTimeOffset callerLastSyncDateTime)
		{
			DateTimeOffset? lastSyncDateTime = this.GetLastSynchronizationDateTime(security);

			if ((!lastSyncDateTime.HasValue) || (lastSyncDateTime.Value < callerLastSyncDateTime))
			{
				lastSyncDateTime = callerLastSyncDateTime;
			}

			return this.NeedToPerformPeriodicSynchronization(security, lastSyncDateTime.Value);
		}

		/// <summary>
		///	The initial synchronization required.
		/// </summary>
		/// <param name="security">
		///	Instance of the current security context
		/// </param>
		/// <returns>
		///	True <see cref="bool" /> if we were unable to locate a version record that has completed a synchronization session,
		///	otherwise; False to indicate
		///	that this system has synchronized at least once in the past.
		/// </returns>
		public bool InitialSynchronizationRequired(SecurityClass security)
		{
			bool hasSynchronized = false;

			VersionCollection versions = this.GetVersionHistory(security);

			if (null != versions && versions.Count > 0)
			{
				IEnumerable<VersionDO> completedSyncVersions = from v in versions
															   where v.SyncCompletedFlag == true
															   select v;

				hasSynchronized = completedSyncVersions.Any();
			}

			return !hasSynchronized;
		}

		/// <summary>
		///	Checks to see if a resynchronization request is required by determining if there were any schema changes
		///	since the last recorded version that completed synchronization.
		/// </summary>
		/// <param name="security">
		///	The security.
		/// </param>
		/// <returns>
		///	The <see cref="bool" />.
		/// </returns>
		public bool ResynchronizationRequired(SecurityClass security)
		{
			bool possibleSchemaChange = false;

			// Are there any version that included schema changes.
			SchemaChangeHistoryCollection schemaChangeHistoryList = this.GetSchemaChangeHistory(security);

			if (null != schemaChangeHistoryList && schemaChangeHistoryList.Count > 0)
			{
				IEnumerable<SchemaChangeHistoryDO> completedSyncVersions = from v in schemaChangeHistoryList
																		   where v.HasSchemaChangeFlag == true
																		   select v;

				possibleSchemaChange = completedSyncVersions.Any();
			}

			// If we there may have been a schema change, see if any version synchronization would have already taken care of it.
			if (possibleSchemaChange)
			{
				VersionCollection versions = this.GetVersionHistory(security);

				VersionDO currentVersion = FMChannelHelper.MakeCall<IVersions, VersionDO>(x => x.GetCurrent(security));

				if (null != versions && versions.Count > 0 && null != currentVersion)
				{
					// Make sure we have a schemaChangeHistory record for this version.
					if (schemaChangeHistoryList.All(x => x.Version != currentVersion.Version))
					{
						throw new Exception("Version not found in SchemaChangeHistory table.");
					}
				}
			}

			// THIS NEEDS TO BE COMPLETED.  LOW PRIORITY RIGHT NOW, BUT LOGIC NEEDS TO BE ADDED WHICH WILL DETERMINE HOW FAR BACK WE NEED TO SYNCHRONIZE FROM
			// Section 11.5.2 and 11.5.3 in design document.

			return possibleSchemaChange;
		}

		/// <summary>
		/// The stop synchronization.
		/// </summary>
		public void SystemStopSynchronization()
		{
			if (null != this._SyncControllerFM && !this._SyncControllerFM.IsDisposed)
			{
				this._SyncControllerFM.SysStopFlag = true;
			}
		}


		/// <summary>
		///	The stop synchronization.
		/// </summary>
		/// <param name="security">
		///	The security.
		/// </param>
		public void StopSynchronization(SecurityClass security)
		{
			if (null != this._SyncControllerFM && !this._SyncControllerFM.IsDisposed)
			{
				this._SyncControllerFM.UserStopFlag = true;
			}
		}

		#endregion Public Methods and Operators

		#region Methods

		/// <summary>
		///	Gets the local client synchronization settings.
		/// </summary>
		/// <param name="security">
		///	The security.
		/// </param>
		/// <exception cref="Exception">
		///	An exception is thrown if the client configuration record could not be loaded.
		/// </exception>
		/// <returns>
		///	A copy of the current client synchronization settings <see cref="SyncClientConfigurationDO" />.
		/// </returns>
		private static SyncClientConfigurationDO GetClientSynchronizationSettings(SecurityClass security)
		{
			SyncClientConfigurationDO clientSyncConfig = FMChannelHelper.MakeCall<ISyncClientConfigurations, SyncClientConfigurationDO>(x => x.Get(security));

			if (null == clientSyncConfig || (null != clientSyncConfig && clientSyncConfig.IdentityGuid == Guid.Empty))
			{
				throw new Exception("Error retrieving client synchronization settings.  Check configuration.");
			}

			if (string.IsNullOrEmpty(clientSyncConfig.ServerAuthDomain)
				|| (!string.IsNullOrEmpty(clientSyncConfig.ServerAuthDomain)
					&& clientSyncConfig.ServerAuthDomain.Length == 0))
			{
				clientSyncConfig.ServerAuthDomain = ".";
			}

			return clientSyncConfig;
		}

		/// <summary>
		///	The get authentication certificate for the fuels manager user
		/// </summary>
		/// <param name="clientSyncConfig">
		///	The client sync config.
		/// </param>
		/// <returns>
		///	The <see cref="X509Certificate2" />.
		/// </returns>
		private static X509Certificate2 GetFuelsManagerAuthenticationCertificate(
				SyncClientConfigurationDO clientSyncConfig)
		{
			X509Certificate2 fuelsManagerAuthCertificate = null;

			if (!string.IsNullOrEmpty(clientSyncConfig.FMAuthClientCertificate))
			{
				var certStore = new X509Store(StoreLocation.LocalMachine);
				certStore.Open(OpenFlags.ReadOnly);

				X509Certificate2Collection certColl;
				certColl = certStore.Certificates.Find(X509FindType.FindByThumbprint, clientSyncConfig.FMAuthClientCertificate, true);

				if (certColl.Count == 0) //otherwise, it should be the subject name
				{
					certColl = certStore.Certificates.Find(X509FindType.FindBySubjectName, clientSyncConfig.FMAuthClientCertificate, true);
				}

				if (certColl.Count > 0)
				{
					fuelsManagerAuthCertificate = certColl[0];
				}

				certStore.Close();
			}

			return fuelsManagerAuthCertificate;
		}

		/// <summary>
		///	The get transport authentication certificate.
		/// </summary>
		/// <param name="clientSyncConfig">
		///	The client sync config.
		/// </param>
		/// <returns>
		///	The <see cref="X509Certificate2" />.
		/// </returns>
		private static X509Certificate2 GetTransportAuthenticationCertificate(SyncClientConfigurationDO clientSyncConfig)
		{
			X509Certificate2 serverAuthCertificate = null;


			if (!string.IsNullOrEmpty(clientSyncConfig.ServerAuthClientCertificate))
			{

				var certStore = new X509Store(StoreLocation.LocalMachine);
				certStore.Open(OpenFlags.ReadOnly);
				X509Certificate2Collection certColl;

				certColl = certStore.Certificates.Find(X509FindType.FindByThumbprint, clientSyncConfig.ServerAuthClientCertificate, true);

				if (certColl.Count == 0) //otherwise, it should be the subject name
				{
					certColl = certStore.Certificates.Find(X509FindType.FindBySubjectName, clientSyncConfig.ServerAuthClientCertificate, true);
				}

				if (certColl.Count > 0)
				{
					serverAuthCertificate = certColl[0];
				}

				certStore.Close();
			}

			return serverAuthCertificate;

		}

		/// <summary>
		///	Validates whether or not the supplied client synchronization settings can be used to perform synchronization.
		/// </summary>
		/// <param name="security">
		///	The current security context of the caller.  <see cref="SecurityClass" />
		/// </param>
		/// <param name="clientSyncConfig">
		///	An instance of the current client synchronization settings <see cref="SyncClientConfigurationDO" />.
		/// </param>
		/// <returns>
		///	Returns true if the client synchronization settings are valid, otherwise; false.
		/// </returns>
		private static bool ValidateClientSynchronizationSettings(
				SecurityClass security,
				SyncClientConfigurationDO clientSyncConfig)
		{
			bool isValid = true;

			if (string.IsNullOrEmpty(clientSyncConfig.RootSiteID))
			{
				// Root Site / SiteGroup ID not specified in client synchronization settings
				SyncHelperFM.WriteConfigurationAlarmAndEvent(security, ErrorConstants.SYNC_ERR_MSG_08003);

				isValid = false;
			}

			if (string.IsNullOrEmpty(clientSyncConfig.EnterpriseURL))
			{
				// Enterprise synchronization URL not specified in client synchronization settings
				SyncHelperFM.WriteConfigurationAlarmAndEvent(security, ErrorConstants.SYNC_ERR_MSG_08004);

				isValid = false;
			}

			return isValid;
		}

		/// <summary>
		///	The get last synchronization date time.
		/// </summary>
		/// <param name="security">
		///	The security.
		/// </param>
		/// <returns>
		///	The
		///	<see>
		///			<cref>DateTimeOffset?</cref>
		///	</see>
		///	.
		/// </returns>
		private DateTimeOffset? GetLastSynchronizationDateTime(SecurityClass security)
		{
			return FMChannelHelper.MakeCall<ISyncSessionLogs, DateTimeOffset?>(x => x.GetLastSyncDateTime(security));
		}

		/// <summary>
		///	The load schema version history and caches it.
		/// </summary>
		/// <param name="security">
		///	Instance of the security context executing this method
		/// </param>
		/// <returns>
		///	Reference to a collection of <see cref="SchemaChangeHistoryDO" /> records.
		/// </returns>
		/// <remarks>
		///	Once the SchemaChangeHistory is loaded, the cached reference is returned instead of reloading the history from the
		///	data store.
		/// </remarks>
		private SchemaChangeHistoryCollection GetSchemaChangeHistory(SecurityClass security)
		{
			if (null == this._SchemaChangeHistoryList)
			{
				this._SchemaChangeHistoryList = FMChannelHelper.MakeCall<ISchemaChangeHistories, SchemaChangeHistoryCollection>(x => x.Enumerate(security));
			}

			return this._SchemaChangeHistoryList;
		}

		/// <summary>
		///	The load database version history and caches it.
		/// </summary>
		/// <param name="security">
		///	Instance of the security context executing this method
		/// </param>
		/// <returns>
		///	Reference to a collection of <see cref="SchemaChangeHistoryDO" /> records.
		/// </returns>
		/// <remarks>
		///	Once the SchemaChangeHistory is loaded, the cached reference is returned instead of reloading the history from the
		///	data store.
		/// </remarks>
		private VersionCollection GetVersionHistory(SecurityClass security)
		{
			if (null == this._DBVersionList)
			{
				this._DBVersionList = FMChannelHelper.MakeCall<IVersions, VersionCollection>(x => x.Enumerate(security));
			}

			return this._DBVersionList;
		}
		/// <summary>
		///	Checks the local database for any site that has been configured to perform periodic synchronization.
		///	If so, we check to see if it's time for them to synchronize based on their last synchronization date.
		/// </summary>
		/// <param name="security">
		///	The current security context of the caller.  <see cref="SecurityClass" />
		/// </param>
		/// <param name="lastSyncDateTime">
		///	The last Sync Date Time.
		/// </param>
		/// <returns>
		///	Returns a value of true if a periodic synchronization request should be made, otherwise; false.
		/// </returns>
		/// <remarks>
		///	Other factors will determine whether or not periodic synchronization should be performed.  For example,
		///	synchronization is disabled locally on the client
		///	node or the hosted sites were not configured for periodic synchronization.
		/// </remarks>
		private bool NeedToPerformPeriodicSynchronization(SecurityClass security, DateTimeOffset lastSyncDateTime)
		{
			// 1. Load the RootSite.
			// 2. Get a list of the site synchronization list based on this root site
			// 3. See if any of them need to be synchronized.  Since there's no UI involved, we'll attempt to synchronize anything that's scheduled to be synchronized.
			//	Note: During synchronization, the server still has the option to deny synchronization based on the supplied credentials for each Site / Site Group.
			// 4. If we determine that a Site or Site Group was configured for periodic synchronization, we need to check the local configuration settings to see if
			//	synchronization was disabled.
			bool performPeriodicSync = false;

			try
			{
				if (!security.HasRight(RIGHT.PERFORM_SYNCHRONIZATION))
				{
					_EventLog.WriteEntry(
							string.Format(
								"Periodic synchronization failed.  Insufficient User Rights for {0}",
								security.UserID),
							EventLogEntryType.Error);

					SyncHelperFM.WriteErrorAlarmAndEvent(
							security,
							string.Format(
								"Periodic synchronization failed.  Insufficient User Rights for {0}",
								security.UserID));
				}
				else
				{
					SyncClientConfigurationDO clientSyncConfig = GetClientSynchronizationSettings(security);

					if (null != clientSyncConfig)
					{
						if (ValidateClientSynchronizationSettings(security, clientSyncConfig))
						{
							SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.GetByID(security, clientSyncConfig.RootSiteID, true));

							if (site.IdentityGuid != Guid.Empty)
							{
								if (site.EnablePeriodicSyncFlag && site.PeriodicSyncIntervalMinutes > 0
										&& (DateTime.Now >= lastSyncDateTime.AddMinutes(site.PeriodicSyncIntervalMinutes)))
								{
									performPeriodicSync = true;
								}
							}
						}
					}
				}
			}
			catch (Exception eX)
			{
				_EventLog.WriteEntry(
					string.Format("Synchronization exception encountered: {0}", eX.StackTrace),
					EventLogEntryType.Error);
				// PLACEHOLDER.  NEED TO IDENTIFY SPECIFIC EXCEPTIONS THAT COULD COME BACK.
				_EventLog.WriteEntry(
					string.Format("Synchronization exception encountered: {0}", eX.Message),
					EventLogEntryType.Error);
				SyncHelperFM.WriteErrorAlarmAndEvent(
					security,
					string.Format("Synchronization encountered an exception: {0}", eX.Message));
			}

			return performPeriodicSync;
		}



		/// <summary>
		///	Reprocesses the local conflicts.
		/// </summary>
		/// <param name="localSyncSecurity">The local synchronize security.</param>
		private void ReprocessLocalConflicts(SecurityClass localSyncSecurity)
		{
			var syncConflictResolutionStatus = new SyncConflictResolutionStatus();

			while (syncConflictResolutionStatus.Pass < 2
			&& !this._SyncControllerFM.SyncStopped)
			{
				syncConflictResolutionStatus = FMChannelHelper.MakeCall<IEnterpriseSynchronization, SyncConflictResolutionStatus>(
				x =>
				{
					return x.ReprocessConflicts(localSyncSecurity, this._CurrentSyncContext.ServerID, this._CurrentSyncContext.SyncSessionID, syncConflictResolutionStatus);
				});
			}
		}

		/// <summary>
		/// Reprocesses the enterprise conflicts.
		/// </summary>
		/// <param name="enterpriseSyncSecurity">The enterprise synchronize security.</param>
		/// <param name="clientSyncConfig">The client synchronize configuration.</param>
		private void ReprocessEnterpriseConflicts(SecurityClass enterpriseSyncSecurity, SyncClientConfigurationDO clientSyncConfig)
		{
			var syncConflictResolutionStatus = new SyncConflictResolutionStatus();

			while (syncConflictResolutionStatus.Pass < 2
			&& !this._SyncControllerFM.SyncStopped)
			{
				syncConflictResolutionStatus = FMChannelHelper.MakeCall<ISyncControllerProcessor, SyncConflictResolutionStatus>(
				x =>
				{
					return x.ReprocessEnterpriseConflicts(enterpriseSyncSecurity, clientSyncConfig, this._CurrentSyncContext, syncConflictResolutionStatus);
				});
			}
		}

		/// <summary>
		/// Purges the local logs.
		/// </summary>
		/// <param name="localSyncSecurity">The local synchronize security.</param>
		/// <param name="maximumDaysToRetainLogs">The maximum days to retain logs.</param>
		private void PurgeLocalLogs(SecurityClass localSyncSecurity, int maximumDaysToRetainLogs)
		{
			FMChannelHelper.MakeCall<IEnterpriseSynchronization>(
				x =>
				{
					x.PurgeLogs(localSyncSecurity, this._CurrentSyncContext.ServerID, maximumDaysToRetainLogs);
				});

		}

		/// <summary>
		/// Purges the enterprise logs.
		/// </summary>
		/// <param name="enterpriseSyncSecurity">The enterprise synchronize security.</param>
		/// <param name="clientSyncConfig">The client synchronize configuration.</param>
		/// <param name="maximumDaysToRetainLogs">The maximum days to retain logs.</param>
		private void PurgeEnterpriseLogs(SecurityClass enterpriseSyncSecurity, SyncClientConfigurationDO clientSyncConfig, int maximumDaysToRetainLogs)
		{
			FMChannelHelper.MakeCall<ISyncControllerProcessor>(
				x =>
				{
					x.PurgeLogs(enterpriseSyncSecurity, clientSyncConfig, this._CurrentSyncContext, maximumDaysToRetainLogs);
				});

		}

		private void CleanupAbandonedSyncController(SecurityClass security)
		{
			FMChannelHelper.MakeCall<ISyncSessionLogs>(x => x.CloseActiveSessions(security));
		}

		/// <summary>
		///	This method is responsible for allocating and initializing synchronization context and session information prior to
		///	handing
		///	control over to an instance of the core <see cref="SyncControllerFM" /> engine.
		/// </summary>
		/// <param name="security">
		/// </param>
		/// <param name="selectedSite">
		///	The selected Site Id.
		/// </param>
		/// <param name="passThruClientCertificate">
		///	A byte[] that contains the client certificate of the user who originally initiated the synchronization request.
		/// </param>
		/// <param name="requestType">
		/// </param>
		/// <returns>
		///	true if the entire synchronization process completed, otherwise; false which indicates the process was interrupted
		///	prior to completion.
		/// </returns>
		/// <remarks>
		///	During the pre-synchronization process, this method will allocate a new local and remote enterprise FuelsManager
		///	sessions.
		///	For manual synchronization events, the sessions are based on the user who triggered the synchronization event.  For
		///	periodic synchronization, the
		///	sessions will utilize the allocate FuelsManager synchronization service account.
		///	Authentication into the Enterprise FuelsManager instance is performed by initializing a
		///	<see cref="SecuritySyncLoginRequest" /> instance which
		///	contains the appropriate user credentials.  A <see cref="SyncContextFM" /> context object is also initialized with
		///	information for both sessions and
		///	given to the <see cref="SyncControllerFM" /> to be used during the entire synchronization process.
		/// </remarks>
		private bool Synchronize(
				SecurityClass security,
				SyncSelectedSiteDO selectedSite,
				byte[] passThruClientCertificate,
				SYNCREQUESTTYPE requestType)
		{
			bool syncFinished = false;

			SecurityClass localSyncSecurity = null;
			SecurityClass enterpriseSyncSecurity = null;
			SyncClientConfigurationDO clientSyncConfig = null;

			var rootSiteToSynchronize = new SyncSelectedSiteDO()
			{
				SiteID = selectedSite.SiteID,
				SiteGuid = selectedSite.SiteGuid
			};

			var sessionStatus = SYNCSESSIONSTATUS.NEW;

			Guid syncSessionID = Guid.NewGuid();

			try
			{
				// When processing a periodic, scheduled or resynchronization request, we typically synchronize
				// based on the rootSiteId specified in the client synchronization settings because we won't be
				// given a SiteId from the FuelsManager UI.  If we have a preselected SiteId then we'll use it, 
				// but if we don't, use the one specified in the client synchronization settings.
				// If we are executing a manual synchronization request AND it's the Administrator account; we should always
				// fallback to the client synchronization settings for everything (user / siteid).
				if ((requestType != SYNCREQUESTTYPE.MANUAL && string.IsNullOrEmpty(rootSiteToSynchronize.SiteID))
					|| requestType == SYNCREQUESTTYPE.MANUAL && security.UserGuid.ToString().Equals(Guids.UserAdminGuid.ToString())
					)
				{
					rootSiteToSynchronize = this.GetSynchronizationSiteId(security, requestType);
				}

				if (!string.IsNullOrEmpty(rootSiteToSynchronize.SiteID))
				{
					clientSyncConfig = GetClientSynchronizationSettings(security);

					if (null != clientSyncConfig)
					{
						Guid localSynchronizationNodeId = FMChannelHelper.MakeCall<IEnterpriseSynchronization, Guid>(x => x.GetServerID());
						Guid enterpriseSynchronizationNodeId = FMChannelHelper.MakeCall<ISyncControllerProcessor, Guid>(x => x.GetEnterpriseSynchronizationNodeId(
						   security,
						   clientSyncConfig));

						string localSynchronizationNodeName = FMChannelHelper.MakeCall<IEnterpriseSynchronization, string>(x => x.GetNodeName());
						string enterpriseSynchronizationNodeName = FMChannelHelper.MakeCall<ISyncControllerProcessor, string>(x => x.GetEnterpriseSynchronizationNodeName(security, clientSyncConfig));

						if (Guid.Empty != localSynchronizationNodeId
							&& Guid.Empty != enterpriseSynchronizationNodeId)
						{
							if (!localSynchronizationNodeId.Equals(enterpriseSynchronizationNodeId))
							{
								var syncLoginRequest = new SecuritySyncLoginRequest();

								syncLoginRequest.SyncSessionID = syncSessionID;
								syncLoginRequest.SyncRequestTypeIndex = requestType;
								syncLoginRequest.SyncTransferTypeIndex = SYNCTRANSFERTYPE.ONLINE;

								X509Certificate2 fuelsManagerClientAuthX509Certificate =
										GetFuelsManagerAuthenticationCertificate(clientSyncConfig);

								byte[] fuelsManagerClientAuthCertificate = null;

								if (null != fuelsManagerClientAuthX509Certificate)
								{
									fuelsManagerClientAuthCertificate =
										fuelsManagerClientAuthX509Certificate.Export(X509ContentType.SerializedCert);
								}

								syncLoginRequest.SourceNodeGuid = localSynchronizationNodeId;
								syncLoginRequest.SourceNodeMachineName = localSynchronizationNodeName;

								// Setup the FuelsManager authentication credentials required to synchronize the actual data.
								// This presents a problem if we don't have any passThruClientCertificate credentials because the system didn't capture it.
								// Technically we never captured the user's password which means we shouldn't be able to authenticate if we've stored the password
								// as a one-way hash.
								// If the Administrator account is being used, we should use the same certificate that was used during the initial sync rather than 
								// creating a remote session as an "administrator"
								syncLoginRequest.ClientCertificate = (requestType == SYNCREQUESTTYPE.MANUAL && !security.UserGuid.ToString().Equals(Guids.UserAdminGuid.ToString()))
																							? passThruClientCertificate
																							: fuelsManagerClientAuthCertificate;

								syncLoginRequest.UserID = string.IsNullOrEmpty(clientSyncConfig.FMAuthUserName)
																			? security.UserID
																			: clientSyncConfig.FMAuthUserName;

								syncLoginRequest.Password = string.IsNullOrEmpty(clientSyncConfig.FMAuthPassword)
																			? security.Password
																			: clientSyncConfig.FMAuthPassword;

								// If a client authentication certificate was specified for Transport Security, set this up now.
								syncLoginRequest.X509ClientCertificate =
										GetTransportAuthenticationCertificate(clientSyncConfig);

								syncLoginRequest.SiteID = rootSiteToSynchronize.SiteID;
								syncLoginRequest.TimeOut = 20;

								// Create a new local session because we're going to perform a logout at the end.  For manual synchronization we don't want to 
								// logout the user's current interactive session.
								localSyncSecurity = FMChannelHelper.MakeCall<ISyncControllerProcessor, SecurityClass>(x => x.CreateLocalSession(
									   security,
									   enterpriseSynchronizationNodeId,
									   requestType));

								// Create a new remote FuelsManager session on the enterprise.
								enterpriseSyncSecurity = FMChannelHelper.MakeCall<ISyncControllerProcessor, SecurityClass>(x => x.CreateEnterpriseSession(
									  security,
									  syncLoginRequest,
									  clientSyncConfig));

								if (null != enterpriseSyncSecurity)
								{
									this._CurrentSyncContext = SyncContextFM.CreateContext(
										syncSessionID,
										localSynchronizationNodeId,
										localSyncSecurity,
										enterpriseSynchronizationNodeId,
										enterpriseSyncSecurity,
										rootSiteToSynchronize.SiteID);

									this._CurrentSyncContext.TransferType = SYNCTRANSFERTYPE.ONLINE;
									this._CurrentSyncContext.RequestType = requestType;

									this._CurrentSyncContext.ClientName = localSynchronizationNodeName;
									this._CurrentSyncContext.ServerName = enterpriseSynchronizationNodeName;


									try
									{
										this._SyncControllerFM = new SyncControllerFM(this._CurrentSyncContext);
										string syncProfileId = SyncConstants.DEFAULT_PROFILE_COMPLETE;

										sessionStatus = this._SyncControllerFM.SynchronizeDatabases(syncProfileId, clientSyncConfig);

										if (sessionStatus == SYNCSESSIONSTATUS.COMPOK
											|| sessionStatus == SYNCSESSIONSTATUS.COMPCON)
										{
											// If we are performing an initial synchronization, we need to reset the Next Document / Invoice Numbers for the Site so
											// the local system continues where it previously left off.
											if (requestType == SYNCREQUESTTYPE.INIT)
											{
												FMChannelHelper.MakeCall<ISites>(
													x => x.InitializeNextDocumentInvoiceNumbers(security, selectedSite.SiteID));

												FMChannelHelper.MakeCall<ISyncControllerProcessor>(
													x => x.ExecutePostSyncProcessing(security, syncProfileId, clientSyncConfig, requestType));
											}

											// Make sure we flag this version as having been synchronized at least once.
											this.UpdateVersionSyncStatus(security, true, selectedSite.SiteID);

											this.ReprocessLocalConflicts(localSyncSecurity);
											this.ReprocessEnterpriseConflicts(
													enterpriseSyncSecurity,
													clientSyncConfig);
										}

										SiteClass site =
											FMChannelHelper.MakeCall<ISites, SiteClass>(
												x => x.Get(localSyncSecurity, localSyncSecurity.SiteGuid, false, false, false));

										this.PurgeLocalLogs(localSyncSecurity, site._MaximumDaysToRetainLogs);
										this.PurgeEnterpriseLogs(enterpriseSyncSecurity, clientSyncConfig, site._MaximumDaysToRetainLogs);

										syncFinished = true;
									}
									finally
									{
										if (this._SyncControllerFM != null)
										{
											this._SyncControllerFM.Dispose();
											this._SyncControllerFM = null;
										}
									}
								}
							}
							else
							{
								// Same system
								SyncHelperFM.WriteConfigurationAlarmAndEvent(
										security,
										ErrorConstants.SYNC_ERR_MSG_08006);
							}
						}
						else
						{
							// Missing local node ID (08007)
							// Missing enterprise node ID (08008)
							string nodeIdErrorMessage = Guid.Empty == localSynchronizationNodeId
																		? ErrorConstants.SYNC_ERR_MSG_08007
																		: ErrorConstants.SYNC_ERR_MSG_08008;

							SyncHelperFM.WriteConfigurationAlarmAndEvent(security, nodeIdErrorMessage);
						}
					}
				}
				else
				{
					SyncHelperFM.WriteConfigurationAlarmAndEvent(
							security,
							@"Unable to process synchronization request.  Site ID not specified");
				}
			}
			catch (EndpointNotFoundException eX)
			{
				string msg =
					string.Format(
							"Unable to connect to enterprise synchronization service.  Check config settings syncEnterpriseBusinessBindingType and syncEnterpriseBusinessBindingConfiguration.  These should match the enterprise server service endpoint settings.  Exception: {0}",
							eX.Message);

				_EventLog.WriteEntry(string.Format("{0}, {1}", msg, eX.StackTrace), EventLogEntryType.Error);
				SyncHelperFM.WriteErrorAlarmAndEvent(security, msg);
			}
			catch (Exception eX)
			{
				string msg = string.Format("Synchronization exception encountered: {0}", eX.Message);

				_EventLog.WriteEntry(string.Format("{0}, {1}", msg, eX.StackTrace), EventLogEntryType.Error);
				SyncHelperFM.WriteErrorAlarmAndEvent(security, msg);
			}
			finally
			{
				syncFinished = true;

				if (null != localSyncSecurity && localSyncSecurity.Token != Guid.Empty)
				{
					FMChannelHelper.MakeCall<ISyncControllerProcessor>(x => x.PurgeLocalSession(localSyncSecurity, localSyncSecurity.Token));
				}

				if (null != enterpriseSyncSecurity && enterpriseSyncSecurity.Token != Guid.Empty)
				{
					FMChannelHelper.MakeCall<ISyncControllerProcessor>(x => x.PurgeEnterpriseSession(enterpriseSyncSecurity, clientSyncConfig, syncSessionID, sessionStatus));
				}
			}

			return syncFinished;
		}

		/// <summary>
		///	This method is responsible for allocating and initializing synchronization context and session information prior to
		///	handing
		///	control over to an instance of the core <see cref="SyncControllerFM" /> engine.
		/// </summary>
		/// <param name="security">
		///	Calling security context.
		/// </param>
		/// <param name="selectedSite">
		///	The selected Site Id.
		/// </param>
		/// <param name="passThruClientCertificate">
		///	A byte[] that contains the client certificate of the user who originally initiated the synchronization request.
		/// </param>
		/// <param name="requestType">
		///	Indicates if this session was initiated manually or by a periodic request.
		/// </param>
		/// <param name="startRange">
		///	The start Range.
		/// </param>
		/// <param name="endRange">
		///	The end Range.
		/// </param>
		/// <returns>
		///	The name of the offline synchronization file that was generated, otherwise; an empty string if the file was not
		///	able to be created.
		/// </returns>
		/// <remarks>
		///	During offline synchronization, any pre-synchronization information related to the Enterprise such as the
		///	Enterprise Node identifier will not be known.  This value will be set once
		///	the offline file is processed on the Enterprise Server.
		/// </remarks>
		private string SynchronizeOffline(
				SecurityClass security,
				SyncSelectedSiteDO selectedSite,
				byte[] passThruClientCertificate,
				SYNCREQUESTTYPE requestType,
				DateTimeOffset? startRange,
				DateTimeOffset? endRange)
		{
			string generatedFilename = string.Empty;

			SyncClientConfigurationDO clientSyncConfig = null;

			SecurityClass localSyncSecurity = null;

			var sessionStatus = SYNCSESSIONSTATUS.NEW;

			Guid syncSessionID = Guid.NewGuid();

			try
			{
				clientSyncConfig = GetClientSynchronizationSettings(security);

				if (null != clientSyncConfig)
				{
					Guid localSynchronizationNodeId = FMChannelHelper.MakeCall<IEnterpriseSynchronization, Guid>(x => x.GetServerID());
					string localSynchronizationNodeName = FMChannelHelper.MakeCall<IEnterpriseSynchronization, string>(x => x.GetNodeName());

					if (Guid.Empty != localSynchronizationNodeId)
					{
						string syncProfileID = FMChannelHelper.MakeCall<ISyncControllerProcessor, string>(x => x.GetSyncProfileToSynchronize(security));

						var syncLoginRequest = new SecuritySyncLoginRequest();

						syncLoginRequest.SyncRequestTypeIndex = requestType;
						syncLoginRequest.SyncTransferTypeIndex = SYNCTRANSFERTYPE.OFFLINE;

						syncLoginRequest.SourceNodeGuid = localSynchronizationNodeId;
						syncLoginRequest.SourceNodeMachineName = localSynchronizationNodeName;
						syncLoginRequest.ClientCertificate = (requestType == SYNCREQUESTTYPE.MANUAL)
																			? (passThruClientCertificate ?? null)
																			: null;

						syncLoginRequest.X509ClientCertificate = GetTransportAuthenticationCertificate(clientSyncConfig);

						syncLoginRequest.UserID = string.IsNullOrEmpty(clientSyncConfig.FMAuthUserName)
																? security.UserID
																: clientSyncConfig.FMAuthUserName;

						syncLoginRequest.Password = string.IsNullOrEmpty(clientSyncConfig.FMAuthPassword)
																? security.Password
																: clientSyncConfig.FMAuthPassword;
						syncLoginRequest.SiteID = selectedSite.SiteID;
						syncLoginRequest.TimeOut = 20;

						localSyncSecurity = FMChannelHelper.MakeCall<ISyncControllerProcessor, SecurityClass>(x => x.CreateLocalSession(security, Guid.Empty, requestType));

						this._CurrentSyncContext = SyncContextFM.CreateContext(
							syncSessionID,
							localSynchronizationNodeId,
							localSyncSecurity,
							Guid.Empty,
							null,
							selectedSite.SiteID);

						this._CurrentSyncContext.TransferType = SYNCTRANSFERTYPE.OFFLINE;
						this._CurrentSyncContext.RequestType = requestType;

						try
						{
							this._SyncControllerFM = new SyncControllerFM(this._CurrentSyncContext);

							sessionStatus = this._SyncControllerFM.SynchronizeOfflineDatabases(
								syncProfileID,
								clientSyncConfig,
								ref generatedFilename);

							if (sessionStatus == SYNCSESSIONSTATUS.COMPOK || sessionStatus == SYNCSESSIONSTATUS.COMPCON)
							{
								// Make sure we flag this version as having been synchronized at least once.
								this.UpdateVersionSyncStatus(security, true, selectedSite.SiteID);

								this.ReprocessLocalConflicts(localSyncSecurity);
							}
						}
						finally
						{
							if (this._SyncControllerFM != null)
							{
								this._SyncControllerFM.Dispose();
								this._SyncControllerFM = null;
							}
						}
					}
					else
					{
						// Missing local node ID (08007)
						// Missing enterprise node ID (08008)
						string nodeIdErrorMessage = Guid.Empty == localSynchronizationNodeId
																? ErrorConstants.SYNC_ERR_MSG_08007
																: ErrorConstants.SYNC_ERR_MSG_08008;

						SyncHelperFM.WriteConfigurationAlarmAndEvent(security, nodeIdErrorMessage);
					}
				}
			}
			catch (Exception eX)
			{
				_EventLog.WriteEntry(
					string.Format("Synchronization exception encountered: {0}", eX.StackTrace),
					EventLogEntryType.Error);
				// PLACEHOLDER.  NEED TO IDENTIFY SPECIFIC EXCEPTIONS THAT COULD COME BACK.
				_EventLog.WriteEntry(
					string.Format("Synchronization exception encountered: {0}", eX.Message),
					EventLogEntryType.Error);
				SyncHelperFM.WriteErrorAlarmAndEvent(
					security,
					string.Format("Synchronization encountered an exception: {0}", eX.Message));
			}
			finally
			{
				if (null != localSyncSecurity && localSyncSecurity.Token != Guid.Empty)
				{
					FMChannelHelper.MakeCall<ISyncControllerProcessor>(x => x.PurgeLocalSession(localSyncSecurity, syncSessionID));
				}
			}

			return generatedFilename;
		}

		/// <summary>
		///	The update version sync status.
		/// </summary>
		/// <param name="security">
		///	The security.
		/// </param>
		/// <param name="isCompletedFlag">
		///	The is completed flag.
		/// </param>
		private void UpdateVersionSyncStatus(SecurityClass security, bool isCompletedFlag, string siteId)
		{
			VersionDO ver = FMChannelHelper.MakeCall<IVersions, VersionDO>(x => x.GetCurrent(security));

			// Only update this if it hasn't been updated already.
			if (!ver.SyncCompletedFlag && isCompletedFlag)
			{
				ver.SyncCompletedFlag = true;
				FMChannelHelper.MakeCall<IVersions>(x => x.Modify(security, ver));
			}
		}

		#endregion Methods
	}
}