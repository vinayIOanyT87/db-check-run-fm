// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncControllerProcessor.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Diagnostics.CodeAnalysis;
	using System.Linq;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;

	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;
	using FMBusinessServices.InternalClasses.SyncClasses;
	using FMBusinessServices.InternalClasses.SyncClasses.Client;
	using FMBusinessServices.InternalInterfaces;

	/// <summary>
	///	The sync controller processor.
	/// </summary>
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class SyncControllerProcessor : ISyncControllerProcessor
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
		///	The event log.
		/// </summary>
		private static readonly FMEventLog eventLog = new FMEventLog();

		private static readonly IPointTagArchiveDatabase PointTagArchiveDatabase = new PointTagArchiveDatabase();

		private static readonly IAandEArchiveDatabase AlarmAndEventArchiveDataBase = new AandEArchiveDatabase();


		/// <summary>
		///	Cached list of database version information
		/// </summary>
		private VersionCollection _DBVersionList = null;

		/// <summary>
		///	Cached list of Schema Change History information
		/// </summary>
		private SchemaChangeHistoryCollection _SchemaChangeHistoryList = null;

		#endregion Constants and Fields

		#region Constructors and Destructors

		/// <summary>
		///	Initializes a new instance of the <see cref="SyncControllerProcessor" /> class.
		/// </summary>
		public SyncControllerProcessor()
		{
		}

		#endregion Constructors and Destructors

		#region Public Properties

		/// <summary>
		///	Gets the current db version.
		/// </summary>
		public VersionDO CurrentDbVersion
		{
			get
			{
				if (null != this._DBVersionList && this._DBVersionList.Count > 0)
				{
					VersionDO current = this._DBVersionList.OrderByDescending(v => v.VersionIndex).FirstOrDefault();

					if (null != current)
					{
						return current;
					}
				}

				return ConsolidatedDAClass.GetVersionDetails();
			}
		}

		#endregion Public Properties

		#region Properties

		#endregion Properties

		#region Public Methods and Operators

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
							// If it happens to be SiteAdmin, we need to revert to the client configuration settings for synchronization
							// and use the root Site / Site Group Id.
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
					var sites = new SitesClass();
					SiteClass site = sites.GetByID(security, selectedSite.SiteID);

					selectedSite.SiteGuid = site.SiteGuid;
				}
			}
			catch (Exception eX)
			{
				// PLACEHOLDER.  NEED TO IDENTIFY SPECIFIC EXCEPTIONS THAT COULD COME BACK.
				eventLog.WriteEntry(
					string.Format("Unable to determine synchronization Site Id: {0}", eX.Message),
					FMEventLogEntryType.Error);
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
			return this.GetCurrentSynchronizationState(security);
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

				VersionDO currentVersion = this.CurrentDbVersion;

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
		///	Gets the selected synchronization profile to use for synchronization.
		/// </summary>
		/// <param name="security">
		///	The current security context of the caller.  <see cref="SecurityClass" />
		/// </param>
		/// <returns>
		///	The ID of the synchronization profile to use.
		/// </returns>
		public string GetSyncProfileToSynchronize(SecurityClass security)
		{
			var innerSecurity = new SecurityClass();
			innerSecurity.UserID = security.UserID; // required for AlarmAnEventLogs.Add to work with DESC key
			innerSecurity.AddRight(RIGHT.MODIFY_CONFIGURATION_SETTINGS);

			return SyncDBI.GetSelectedSynchronizationProfile(innerSecurity);
		}

		/// <summary>
		/// Synchronizes the scope.
		/// </summary>
		/// <param name="clientSyncConfig">The client synchronize configuration.</param>
		/// <param name="syncContext">The synchronize context.</param>
		/// <param name="syncSessionLog">The synchronize session log.</param>
		/// <param name="syncScope">The synchronize scope.</param>
		/// <returns>syncContext</returns>
		public (bool, SYNCSINGLEPASSPHASE) SynchronizeScope(SyncClientConfigurationDO clientSyncConfig, SyncContextFM syncContext, SyncSessionLogDO syncSessionLog, SyncScopeDO syncScope)
		{
			syncContext.MaxBatchSegmentRowCountEncountered = false;
			syncContext.MaxClientSyncAnchor = this.GetClientMaxSyncAnchor();
			syncContext.MaxEnterpriseSyncAnchor = this.GetEnterpriseMaxSyncAnchor(clientSyncConfig);
			var syncController = new SyncScopeControllerFM(syncContext);
			return syncController.SynchronizeScope(clientSyncConfig, syncSessionLog, syncScope, syncContext.CurrentSiteGuid, syncContext.CurrentSiteID);
		}

		/// <summary>
		///	Creates a new local FuelsManager session based on behalf of the passed in <see cref="SecurityClass" />.  The passed
		///	in security context must
		///	be associated with an existing FuelsManager session; otherwise an exception will be raised.
		/// </summary>
		/// <param name="security">
		///	The current security context of the caller.  <see cref="SecurityClass" />
		/// </param>
		/// <param name="enterpriseSynchronizationNodeId">
		///	The enterprise synchronization node id.
		/// </param>
		/// <param name="requestType">
		///	The type of synchronization request <see cref="SYNCREQUESTTYPE" />.
		/// </param>
		/// <exception cref="ArgumentException">
		///	Thrown if the incoming security context is not associated with an existing FuelsManager session.
		/// </exception>
		/// <returns>
		///	An instance of a <see cref="SecurityClass" /> that represents a local FuelsManager Session which has been
		///	associated with the current synchronization process.
		/// </returns>
		public SecurityClass CreateLocalSession(
					   SecurityClass security,
					   Guid enterpriseSynchronizationNodeId,
					   SYNCREQUESTTYPE requestType)
		{
			IUsers users = new UsersClass();

			// For manual synchronization, the incoming security context represents the user's current interactive login session.
			// For periodic synchronization, the incoming security context represents a restricted session with only enough rights to perform synchronization.
			UserClass user = users.Get(security, security.UserGuid);
			var localSyncSecurity = new SecurityClass();

			// Restrict this check to only manual synchronization requests which are originated by the FuelsManager UI.
			// For other types like Periodic, an existing session would not exist since the service does not perform a login.
			if (requestType == SYNCREQUESTTYPE.MANUAL && security.Token == Guid.Empty)
			{
				throw new ArgumentException(
					@"The passed in security context is not associated with an existing FuelsManager session.",
					"security");
			}

			// We're going to create a separate synchronization session based on the security context that was used to initiate synchronization.
			if (null != user && user.IdentityGuid != Guid.Empty)
			{
				// The innerSecurity object is used to create a new FuelsManager session record
				// that's associated with the synchronizing user.
				var innerSecurity = new SecurityClass();
				innerSecurity.UserID = DBAccess.ServiceLoginAccess;

				innerSecurity.SiteGuid = security.SiteGuid;
				innerSecurity.SiteID = security.SiteID;
				innerSecurity.LoginSiteGuid = security.LoginSiteGuid;
				innerSecurity.LoginSiteID = security.LoginSiteID;

				// innerSecurity.Password = user.Password;
				innerSecurity.ClientCertLogOn = security.ClientCertLogOn;
				innerSecurity.AddRight(RIGHT.VIEW_USERS);
				innerSecurity.AddRight(RIGHT.VIEW_USER_GROUPS);
				innerSecurity.AddRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS);

				localSyncSecurity.UserGuid = user.IdentityGuid;
				localSyncSecurity.UserID = user.ID;
				localSyncSecurity.Password = user.Password;

				localSyncSecurity.SiteID = security.SiteID;
				localSyncSecurity.SiteGuid = security.SiteGuid;
				localSyncSecurity.LoginSiteID = security.SiteID;
				localSyncSecurity.LoginSiteGuid = security.SiteGuid;

				var session = new SessionClass();
				session.LoginSiteGuid = localSyncSecurity.LoginSiteGuid;
				session.LoginSiteID = localSyncSecurity.LoginSiteID;
				session.SiteGuid = localSyncSecurity.SiteGuid;
				session.SiteID = localSyncSecurity.SiteID;
				session.UserGuid = localSyncSecurity.UserGuid;
				session.UserID = localSyncSecurity.UserID;
				session.Token = Guid.NewGuid();
				session.SynchronizationNodeGuid = enterpriseSynchronizationNodeId;

				var rightsClass = new RightsClass();

				// For initial and manual synchronization we should load the rights associated with the user
				if (requestType != SYNCREQUESTTYPE.PERIODIC && requestType != SYNCREQUESTTYPE.SCHEDULED
					&& requestType != SYNCREQUESTTYPE.RESYNC)
				{
					localSyncSecurity.RightCollection = rightsClass.EnumerateByUserBySite(
							innerSecurity,
							user.IdentityGuid,
							localSyncSecurity.SiteGuid);
				}
				else
				{
					// For periodic, scheduled and resync requests we don't have a specific user so we need to
					// setup the basic rights required to perform the necessary tasks.
					localSyncSecurity.AddRight(RIGHT.BASE_EXPORT);
					localSyncSecurity.AddRight(RIGHT.INTERFACE_IMPORT);
					localSyncSecurity.AddRight(RIGHT.VIEW_SYNC_CONFIG_CLIENT_SETTINGS);
					localSyncSecurity.AddRight(RIGHT.PERFORM_SYNCHRONIZATION);
					localSyncSecurity.AddRight(RIGHT.VIEW_SYNC_CONFLICT_STATUS);
				}

				localSyncSecurity.Token = session.Token;

				var sessions = new SessionsClass();
				sessions.Add(localSyncSecurity, session);
			}

			return localSyncSecurity;
		}

		/// <summary>
		///	The create enterprise session.
		/// </summary>
		/// <param name="security">
		///	The security.
		/// </param>
		/// <param name="loginRequest">
		///	The login request.
		/// </param>
		/// <param name="clientSyncConfig">
		///	The client sync config.
		/// </param>
		/// <returns>
		///	The <see cref="SecurityClass" />.
		/// </returns>
		/// <exception cref="FaultException">
		///	Throws a new <see cref="FaultException" /> if the call to CreateSession encounters an fault exception.
		/// </exception>
		/// <exception cref="Exception">
		///	Throws an exception if the call to CreateSession encounters an exception.
		/// </exception>
		public SecurityClass CreateEnterpriseSession(
				SecurityClass security,
				SecuritySyncLoginRequest loginRequest,
				SyncClientConfigurationDO clientSyncConfig)
		{
			SecurityClass enterpriseSyncSecurity = null;

			try
			{
				string sessionMessage = string.Empty;
				string syncServiceBindingType = ConfigurationManager.AppSettings[FMSyncChannelHelper.BindingTypeConfigKey];

				if (string.IsNullOrEmpty(syncServiceBindingType))
				{
					throw new Exception(ErrorConstants.SYNC_ERR_MSG_08001); // Binding error
				}

				SYNCSERVICESTATE serviceState =
					FMSyncChannelHelper.MakeCall<IEnterpriseSynchronization, SYNCSERVICESTATE>(
							clientSyncConfig,
							clientSyncConfig.EnterpriseURL,
							(x) => x.CreateSession(loginRequest, out enterpriseSyncSecurity, out sessionMessage));

				string errorMessage = string.Empty;

				switch (serviceState)
				{
					case SYNCSERVICESTATE.ENTERPRISE_FM_AUTHENTICATION_NOT_CONFIGURED:
						errorMessage = string.Format("Synchronization failed.  Unable to create Enterprise Session, invalid server configuration for FuelsManager Authentication: {0}", sessionMessage);
						break;

					case SYNCSERVICESTATE.FMAUTH_LOGIN_FAILURE:
						errorMessage = string.Format("Synchronization failed.  Unable to create Enterprise Session, Login Failed: {0}", sessionMessage);
						break;

					case SYNCSERVICESTATE.FMAUTH_ACCESS_DENIED:
						errorMessage = string.Format("Synchronization failed.  Enterprise FuelsManager Authentication Access Denied: {0}", sessionMessage);
						break;

					case SYNCSERVICESTATE.SERVICE_ACCESS_DENIED:
						errorMessage = string.Format("Synchronization failed.  Enterprise Server Access Denied to Web Service: {0}", sessionMessage);
						break;

					case SYNCSERVICESTATE.UNAVAILABLE:
					case SYNCSERVICESTATE.ENTERPRISE_NOT_ACCEPTING:
						errorMessage = string.Format("Synchronization unavailable.  Enterprise is not accepting synchronization requests at this time: {0}", sessionMessage);
						break;

					case SYNCSERVICESTATE.ENTERPRISE_NOT_ACCEPTING_SITE:
						errorMessage = string.Format("Synchronization unavailable.  Enterprise is not accepting synchronization requests for the site at this time: {0}", sessionMessage);
						break;

				}

				if (!string.IsNullOrEmpty(errorMessage))
				{
					eventLog.WriteEntry(errorMessage, FMEventLogEntryType.Error);
					SyncHelperFM.WriteErrorAlarmAndEvent(security, errorMessage);
				}
			}
			catch (FaultException)
			{
				throw;
			}
			catch (Exception)
			{
				throw;
			}

			return enterpriseSyncSecurity;
		}

		/// <summary>
		/// Gets the remote site synchronization list.
		/// </summary>
		/// <param name="clientSyncConfig">The client synchronize configuration.</param>
		/// <param name="localSyncList">The local synchronize list.</param>
		/// <param name="syncContext">The synchronize context.</param>
		/// <returns></returns>
		/// <exception cref="System.Exception"></exception>
		public SiteSyncList GetRemoteSiteSynchronizationList(SyncClientConfigurationDO clientSyncConfig, SiteSyncList localSyncList, SyncContextFM syncContext)
		{
			string syncServiceBindingType = ConfigurationManager.AppSettings[FMSyncChannelHelper.BindingTypeConfigKey];

			if (string.IsNullOrEmpty(syncServiceBindingType))
			{
				throw new Exception(ErrorConstants.SYNC_ERR_MSG_08001); // Binding error
			}

			return FMSyncChannelHelper.MakeCall<IEnterpriseSynchronization, SiteSyncList>(
				clientSyncConfig,
				clientSyncConfig.EnterpriseURL,
				(x) => x.GetSynchronizationSiteList(localSyncList, syncContext));

		}

		///  <summary>
		/// 	Removes the enterprise session associated with the current synchronization session.  This method will make a web
		/// 	service call to the
		/// 	enterprise synchronization service in order to obtain the remote node id.
		///  </summary>
		///  <param name="enterpriseSyncSecurity">
		/// 	The security context for the enterprise node that was associated with the synchronization process.
		///  </param>
		///  <param name="clientSyncConfig">
		/// 	The current client synchronization configuration that contains the URL for the enterprise synchronization node.
		///  </param>
		/// <param name="syncSessionID">Synchronization Session ID associated with the FuelsManager Session being terminated.</param>
		/// <param name="sessionStatus">
		/// 	The session status.
		///  </param>
		///  <exception cref="Exception">
		/// 	An exception will be thrown if we are unable to locate the following key 'syncEnterpriseBusinessBindingType' in the
		/// 	AppSettings.
		///  </exception>
		///  <remarks>
		/// 	The application settings (app.config / web.config) must contain a key named 'syncEnterpriseBusinessBindingType'
		/// 	that returns a valid WCF service binding type (basicHttpBinding, wsHttpBinding, etc)
		/// 	Optionally, an application setting key 'syncEnterpriseBusinessBindingConfiguration' should return a particular
		/// 	binding configuration to use (WsHttpsBinding (for https)).
		///  </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
				Justification = "Reviewed. Suppression is OK here.")]
		public void PurgeEnterpriseSession(
				SecurityClass enterpriseSyncSecurity,
				SyncClientConfigurationDO clientSyncConfig,
				Guid syncSessionID,
				SYNCSESSIONSTATUS sessionStatus)
		{
				string syncServiceBindingType = ConfigurationManager.AppSettings[FMSyncChannelHelper.BindingConfigurationConfigKey];

				if (syncServiceBindingType == null)
				{
					throw new Exception(ErrorConstants.SYNC_ERR_MSG_08001); // Binding error
				}


				FMSyncChannelHelper.MakeCall<IEnterpriseSynchronization, string>(
					clientSyncConfig,
					clientSyncConfig.EnterpriseURL,
					(x) => x.EndSession(enterpriseSyncSecurity, syncSessionID, sessionStatus));

				enterpriseSyncSecurity.Token = Guid.Empty;
		}

		///  <summary>
		/// 	Removes the local session associated with the current synchronization session.
		///  </summary>
		///  <param name="syncSecurity">
		/// 	The security context that was associated with the synchronization process.
		///  </param>
		/// <param name="syncSessionID">Synchronization Session ID associated with the FuelsManager Session being terminated.</param>
		/// <remarks>
		/// 	The session to purge SHOULD NOT be the FuelsManager session used to kick off synchronization since this will
		/// 	terminate the user's interactive session.
		///  </remarks>
		public void PurgeLocalSession(SecurityClass syncSecurity, Guid syncSessionID)
		{
			var sessions = new SessionsClass();
			sessions.Purge(syncSecurity, syncSessionID);

			syncSecurity.Token = Guid.Empty;
		}


		/// <summary>
		/// Reprocesses the enterprise conflicts.
		/// </summary>
		/// <param name="enterpriseSyncSecurity">The enterprise synchronize security.</param>
		/// <param name="clientSyncConfig">The client synchronize configuration.</param>
		/// <param name="syncContext">The synchronize context.</param>
		/// <exception cref="System.Exception"></exception>
		public SyncConflictResolutionStatus ReprocessEnterpriseConflicts(
				SecurityClass enterpriseSyncSecurity,
				SyncClientConfigurationDO clientSyncConfig,
				SyncContextFM syncContext,
				SyncConflictResolutionStatus syncConflictResolutionStatus)
		{
			string syncServiceBindingType = ConfigurationManager.AppSettings[FMSyncChannelHelper.BindingConfigurationConfigKey];

			if (string.IsNullOrEmpty(syncServiceBindingType))
			{
				throw new Exception(ErrorConstants.SYNC_ERR_MSG_08001); // Binding error
			}

			return FMSyncChannelHelper.MakeCall<IEnterpriseSynchronization, SyncConflictResolutionStatus>(
				clientSyncConfig,
				clientSyncConfig.EnterpriseURL,
				(x) =>
				{
					return x.ReprocessConflicts(enterpriseSyncSecurity, syncContext.ClientID, syncContext.SyncSessionID, syncConflictResolutionStatus);
				});
		}

		public void PurgeLogs(
				SecurityClass enterpriseSyncSecurity,
				SyncClientConfigurationDO clientSyncConfig,
				SyncContextFM syncContext,
				int maximumDaysToRetainLogs)
		{
			string syncServiceBindingType = ConfigurationManager.AppSettings[FMSyncChannelHelper.BindingTypeConfigKey];

			if (string.IsNullOrEmpty(syncServiceBindingType))
			{
				throw new Exception(ErrorConstants.SYNC_ERR_MSG_08001); // Binding error
			}

			FMSyncChannelHelper.MakeCall<IEnterpriseSynchronization>(
				clientSyncConfig,
				clientSyncConfig.EnterpriseURL,
				(x) =>
				{
					x.PurgeLogs(enterpriseSyncSecurity, syncContext.ClientID, maximumDaysToRetainLogs);
				});
		}


		/// <summary>
		/// Gets the client maximum synchronize anchor.
		/// </summary>
		/// <returns></returns>
		public long GetClientMaxSyncAnchor()
		{
			long syncAnchor = SyncDBI.GetMaxSyncAnchor();

			return syncAnchor;
		}

		/// <summary>
		/// Gets the enterprise maximum synchronize anchor.
		/// </summary>
		/// <param name="clientSyncConfig">The client synchronize configuration.</param>
		/// <returns></returns>
		/// <exception cref="System.Exception">syncEnterpriseBusinessBindingType not found in configuration</exception>
		public long GetEnterpriseMaxSyncAnchor(SyncClientConfigurationDO clientSyncConfig)
		{
			string syncServiceBindingType = ConfigurationManager.AppSettings[FMSyncChannelHelper.BindingConfigurationConfigKey];

			if (string.IsNullOrEmpty(syncServiceBindingType))
			{
				throw new Exception("syncEnterpriseBusinessBindingType not found in configuration");
			}


			long syncAnchor = FMSyncChannelHelper.MakeCall<IEnterpriseSynchronization, long>(
				clientSyncConfig,
				clientSyncConfig.EnterpriseURL,
				(x) => x.GetEnterpriseSyncAnchor());

			return syncAnchor;
		}

		/// <summary>
		///	Gets the node id for the enterprise server with which this client will synchronize with.  This method will make a
		///	web service call to the
		///	enterprise synchronization service in order to obtain the remote node id.
		/// </summary>
		/// <param name="security">
		///	The current security context of the caller.  <see cref="SecurityClass" />
		/// </param>
		/// <param name="clientSyncConfig">
		///	The current client synchronization configuration that contains the URL for the enterprise synchronization node.
		/// </param>
		/// <returns>
		///	The unique <see cref="Guid" /> node Id that represents the enterprise synchronization node with which this client
		///	node will synchronize with.
		/// </returns>
		/// <exception cref="Exception">
		///	An exception will be thrown if we are unable to locate the following key 'syncEnterpriseBusinessBindingType' in the
		///	AppSettings.
		/// </exception>
		/// <remarks>
		///	The application settings (app.config / web.config) must contain a key named 'syncEnterpriseBusinessBindingType'
		///	that returns a valid WCF service binding type (ie: basicHttpBinding, wsHttpBinding, etc)
		///	Optionally, an application setting key 'syncEnterpriseBusinessBindingConfiguration' should return a particular
		///	binding configuration to use (ie: WsHttpsBinding (for https)).
		/// </remarks>
		public Guid GetEnterpriseSynchronizationNodeId(
				SecurityClass security,
				SyncClientConfigurationDO clientSyncConfig)
		{
			string syncServiceBindingType = ConfigurationManager.AppSettings[FMSyncChannelHelper.BindingTypeConfigKey];

			if (string.IsNullOrEmpty(syncServiceBindingType))
			{
				throw new Exception(FMSyncChannelHelper.BindingTypeConfigKey + " not found in configuration");
			}

			Guid enterpriseNodeId = FMSyncChannelHelper.MakeCall<IEnterpriseSynchronization, Guid>(
				clientSyncConfig,
				clientSyncConfig.EnterpriseURL,
				(x) => x.GetServerID());

			return enterpriseNodeId;
		}

		/// <summary>
		///	Gets the node name for the enterprise server with which this client will synchronize with.  This method will make a
		///	web service call to the
		///	enterprise synchronization service in order to obtain the remote node's name.
		/// </summary>
		/// <param name="security">
		///	The current security context of the caller.  <see cref="SecurityClass" />
		/// </param>
		/// <param name="clientSyncConfig">
		///	The current client synchronization configuration that contains the URL for the enterprise synchronization node.
		/// </param>
		/// <returns>
		///	The sync node name that represents the enterprise synchronization node with which this client node will synchronize
		///	with.
		/// </returns>
		/// <exception cref="Exception">
		///	An exception will be thrown if we are unable to locate the following key 'syncEnterpriseBusinessBindingType' in the
		///	AppSettings.
		/// </exception>
		/// <remarks>
		///	The application settings (app.config / web.config) must contain a key named 'syncEnterpriseBusinessBindingType'
		///	that returns a valid WCF service binding type (ie: basicHttpBinding, wsHttpBinding, etc)
		///	Optionally, an application setting key 'syncEnterpriseBusinessBindingConfiguration' should return a particular
		///	binding configuration to use (ie: WsHttpsBinding (for https)).
		/// </remarks>
		public string GetEnterpriseSynchronizationNodeName(
				SecurityClass security,
				SyncClientConfigurationDO clientSyncConfig)
		{
			string syncServiceBindingType = ConfigurationManager.AppSettings[FMSyncChannelHelper.BindingConfigurationConfigKey];

			if (string.IsNullOrEmpty(syncServiceBindingType))
			{
				throw new Exception("syncEnterpriseBusinessBindingType not found in configuration");
			}


			string enterpriseNodeName =
				FMSyncChannelHelper.MakeCall<IEnterpriseSynchronization, string>(
					clientSyncConfig,
					clientSyncConfig.EnterpriseURL,
					(x) => x.GetNodeName());

			return enterpriseNodeName;
		}

		public void CleanupAbandonedSyncController(SecurityClass security)
		{
			ISyncSessionLogs syncSessionLogs = new SyncSessionLogs();
			syncSessionLogs.CloseActiveSessions(security);
		}

		/// <summary>
		/// Called after synchronization has completed for the first time on a new system.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="syncProfileId">The synchronize profile identifier.</param>
		/// <param name="clientSyncConfig">The client synchronize configuration.</param>
		/// <param name="requestType">Current synchronization request type.</param>
		public void ExecutePostSyncProcessing(SecurityClass security, string syncProfileId, SyncClientConfigurationDO clientSyncConfig, SYNCREQUESTTYPE requestType)
		{
			if (requestType == SYNCREQUESTTYPE.INIT)
			{
				SyncDBI.ResetUploadOnlySynchronizationScopes(security, syncProfileId);

				try
				{
					// Initiate rebuild of database indexes.
					SyncDBI.ReIndexAllTablesInSyncProfile(security);
				}
				catch (Exception ex)
				{
					if (ex.Message.IndexOf("timeout", 0, StringComparison.CurrentCultureIgnoreCase) >= 0)
					{
						string msg = string.Format("Timeout occurred while rebuilding indexes. Reindex will continue in background. Timeout details: {0}", ex.Message);

						eventLog.WriteEntry(msg, FMEventLogEntryType.Warning);
					}
					else
					{
						string msg = string.Format("Error occurred while rebuilding indexes: {0}", ex.Message);

						eventLog.WriteEntry(msg, FMEventLogEntryType.Error);
					}
				}
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
				var clientSyncConfigs = new SyncClientConfigurations();
				SyncClientConfigurationDO clientSyncConfig = clientSyncConfigs.Get(security);

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
		///	The get current synchronization state.
		/// </summary>
		/// <param name="security">
		///	The security.
		/// </param>
		/// <returns>
		///	The <see cref="SyncServiceStateDO" />.
		/// </returns>
		private SyncServiceStateDO GetCurrentSynchronizationState(SecurityClass security)
		{
			var serviceState = new SyncServiceStateDO()
										{
												CurrentSessionIsSynchronizing = false,
												AsOfDate = DateTimeOffset.Now,
												SyncServiceState = SYNCSERVICESTATE.READY
										};

			// First we should see if synchronization is enabled or not on the client.
			try
			{
				SyncClientConfigurationDO clientSyncConfig = GetClientSynchronizationSettings(security);

				if (null != clientSyncConfig)
				{
					if (clientSyncConfig.SuspendSynchronizationFlag)
					{
							serviceState.SyncServiceState = SYNCSERVICESTATE.DISABLED_LOCALLY;
					}

					// If we still have the default value of READY, look for any active sessions
					if (serviceState.SyncServiceState == SYNCSERVICESTATE.READY)
					{
						ISyncSessionLogs syncSessions = new SyncSessionLogs();

						SyncSessionLogCollection activeSessionsLog = syncSessions.EnumerateActive(
							security,
							clientSyncConfig.SyncNodeGuid);

						if (null != activeSessionsLog && activeSessionsLog.Count > 0)
						{
							serviceState.SyncServiceState = SYNCSERVICESTATE.IN_PROGRESS;

							foreach (SyncSessionLogDO syncSession in activeSessionsLog)
							{
									if (syncSession.IdentityGuid == security.Token)
									{
										serviceState.CurrentSessionIsSynchronizing = true;
										break;
									}
							}
						}
					}
				}
			}
			catch (Exception eX)
			{
				// PLACEHOLDER.  NEED TO IDENTIFY SPECIFIC EXCEPTIONS THAT COULD COME BACK.
				eventLog.WriteEntry(
					string.Format(
							"Synchronization exception encountered while checking synchronization status: {0}",
							eX.Message),
					FMEventLogEntryType.Error);
				SyncHelperFM.WriteErrorAlarmAndEvent(
					security,
					string.Format(
							"Synchronization encountered an exception while checking synchronization status: {0}",
							eX.Message));
			}

			return serviceState;
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
			ISyncSessionLogs syncSessions = new SyncSessionLogs();

			return syncSessions.GetLastSyncDateTime(security);
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
				var schemaHistoryService = new SchemaChangeHistories();
				this._SchemaChangeHistoryList = schemaHistoryService.Enumerate(security);
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
			var versionService = new Versions();
			this._DBVersionList = versionService.Enumerate(security);
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
					eventLog.WriteEntry(
							string.Format(
								"Periodic synchronization failed.  Insufficient User Rights for {0}",
								security.UserID),
							FMEventLogEntryType.Error);

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
							var sites = new SitesClass();
							SiteClass site = sites.GetByID(security, clientSyncConfig.RootSiteID);

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
				// PLACEHOLDER.  NEED TO IDENTIFY SPECIFIC EXCEPTIONS THAT COULD COME BACK.
				eventLog.WriteEntry(
					string.Format("Synchronization exception encountered: {0}", eX.Message),
					FMEventLogEntryType.Error);
				SyncHelperFM.WriteErrorAlarmAndEvent(
					security,
					string.Format("Synchronization encountered an exception: {0}", eX.Message));
			}

			return performPeriodicSync;
		}

		/// <summary>
		/// Synchronizes the archive.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="enterpriseSyncSecurity">The enterprise synchronize security.</param>
		/// <param name="clientSyncConfig">The client synchronize configuration.</param>
		/// <param name="startDateTimeOffset">The start data time offset</param>
		/// <returns>
		/// MaxBatchSegmentRowCountEncountered
		/// </returns>
		/// <exception cref="System.Exception"></exception>
		public bool SynchronizeArchiveValues(SecurityClass security,
				SecurityClass enterpriseSyncSecurity,
				SyncClientConfigurationDO clientSyncConfig,
				DateTimeOffset startDateTimeOffset,
				Guid siteGuid)
		{
			string syncServiceBindingType = ConfigurationManager.AppSettings["syncEnterpriseBusinessBindingType"];

			if (syncServiceBindingType == null)
			{
				throw new Exception(ErrorConstants.SYNC_ERR_MSG_08001); // Binding error
			}

			string syncServiceBindingConfiguration = ConfigurationManager.AppSettings["syncEnterpriseBusinessBindingConfiguration"];
			if (syncServiceBindingConfiguration == null)
			{
				throw new Exception(ErrorConstants.SYNC_ERR_MSG_08002); // Client synchronization settings missing
			}

			bool moreData;
			SynchronizationElement synchronizationElement;
			var archiveDataElementList = PointTagArchiveDatabase.GetArchiveData(security, startDateTimeOffset, siteGuid, out moreData, out synchronizationElement);

			if (archiveDataElementList != null
			&& archiveDataElementList.Count > 0)
			{
				FMSyncChannelHelper.MakeCall<IEnterpriseSynchronization>(
					clientSyncConfig,
					clientSyncConfig.EnterpriseURL,
					(x) =>
					{
						((IClientChannel)x).OperationTimeout = new TimeSpan(0, 10, 0);
						x.SynchronizeValueArchive(enterpriseSyncSecurity, archiveDataElementList);
					});
			}

			PointTagArchiveDatabase.SynchronizationComplete(security, synchronizationElement);
			return moreData;
		}

		/// <summary>
		/// Synchronizes the archive.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="enterpriseSyncSecurity">The enterprise synchronize security.</param>
		/// <param name="clientSyncConfig">The client synchronize configuration.</param>
		/// <param name="startDateTimeOffset">The start data time offset</param>
		/// <param name="siteGuid">The site to synchronize</param>
		/// <returns>
		/// MaxBatchSegmentRowCountEncountered
		/// </returns>
		/// <exception cref="System.Exception"></exception>
		public bool SynchronizeArchiveAlarmAndEvents(SecurityClass security,
				SecurityClass enterpriseSyncSecurity,
				SyncClientConfigurationDO clientSyncConfig,
				DateTimeOffset startDateTimeOffset,
				Guid siteGuid)
		{
			string syncServiceBindingType = ConfigurationManager.AppSettings["syncEnterpriseBusinessBindingType"];

			if (syncServiceBindingType == null)
			{
				throw new Exception(ErrorConstants.SYNC_ERR_MSG_08001); // Binding error
			}

			string syncServiceBindingConfiguration = ConfigurationManager.AppSettings["syncEnterpriseBusinessBindingConfiguration"];
			if (syncServiceBindingConfiguration == null)
			{
				throw new Exception(ErrorConstants.SYNC_ERR_MSG_08002); // Client synchronization settings missing
			}

			bool moreData;
			AlarmAndEventSynchronizationElement synchronizationElement;
			var archiveDataElementList = AlarmAndEventArchiveDataBase.GetArchiveData(security, startDateTimeOffset, siteGuid, out moreData, out synchronizationElement);

			if (archiveDataElementList != null
			&& archiveDataElementList.Count > 0)
			{
				FMSyncChannelHelper.MakeCall<IEnterpriseSynchronization>(
					clientSyncConfig,
					clientSyncConfig.EnterpriseURL,
					(x) =>
					{
						((IClientChannel)x).OperationTimeout = new TimeSpan(0, 10, 0);
						x.SynchronizeAlarmAndEventArchive(enterpriseSyncSecurity, archiveDataElementList);
					});
			}

			AlarmAndEventArchiveDataBase.SynchronizationComplete(security, synchronizationElement);

			return moreData;
		}




		#endregion Methods
	}
}