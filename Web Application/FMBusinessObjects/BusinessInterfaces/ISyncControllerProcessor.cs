// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISyncControllerProcessor.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	Defines the ISyncControllerProcessor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Specialized;
	using System.ServiceModel;
	using System.Web;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;

	[ServiceContract]
	[ServiceKnownType(typeof(SecurityClass))]
	[ServiceKnownType(typeof(SiteClass))]
	[ServiceKnownType(typeof(SYNCREQUESTTYPE))]
	public interface ISyncControllerProcessor
	{
		#region Methods

		[OperationContract]
		bool InitialSynchronizationRequired(SecurityClass security);

		[OperationContract]
		bool ResynchronizationRequired(SecurityClass security);

		[OperationContract]
		SYNCREQUESTTYPE GetSynchronizationRequestType(SecurityClass security);

		[OperationContract]
		SyncSelectedSiteDO GetSynchronizationSiteId(SecurityClass security, SYNCREQUESTTYPE requestType);

		[OperationContract]
		bool HasPendingAutomatedSynchronizationEvents(SecurityClass security, DateTimeOffset callerLastSyncDateTime);

		[OperationContract]
		SyncServiceStateDO GetSynchronizationState(SecurityClass security);

		[OperationContract]
		SecurityClass CreateLocalSession(SecurityClass security, Guid enterpriseSynchronizationNodeId, SYNCREQUESTTYPE requestType);

		[OperationContract]
		SecurityClass CreateEnterpriseSession(SecurityClass security, SecuritySyncLoginRequest loginRequest, SyncClientConfigurationDO clientSyncConfig);

		[OperationContract]
		void PurgeLocalSession(SecurityClass syncSecurity, Guid syncSessionID);

		[OperationContract]
		void PurgeEnterpriseSession(SecurityClass enterpriseSyncSecurity, SyncClientConfigurationDO clientSyncConfig, Guid syncSessionID, SYNCSESSIONSTATUS sessionStatus);

		[OperationContract]
		string GetSyncProfileToSynchronize(SecurityClass security);

		[OperationContract]
		long GetClientMaxSyncAnchor();

		[OperationContract]
		long GetEnterpriseMaxSyncAnchor(SyncClientConfigurationDO clientSyncConfig);

		[OperationContract]
		Guid GetEnterpriseSynchronizationNodeId(SecurityClass security, SyncClientConfigurationDO clientSyncConfig);

		[OperationContract]
		string GetEnterpriseSynchronizationNodeName(SecurityClass security, SyncClientConfigurationDO clientSyncConfig);

		[OperationContract]
		SiteSyncList GetRemoteSiteSynchronizationList(SyncClientConfigurationDO clientSyncConfig, SiteSyncList localSyncList, SyncContextFM syncContext);

		[OperationContract]
		SyncConflictResolutionStatus ReprocessEnterpriseConflicts(SecurityClass enterpriseSyncSecurity, SyncClientConfigurationDO clientSyncConfig, SyncContextFM syncContext, SyncConflictResolutionStatus syncConflictResolutionStatus);

		[OperationContract]
		void PurgeLogs(SecurityClass enterpriseSyncSecurity, SyncClientConfigurationDO clientSyncConfig, SyncContextFM syncContext, int maximumDaysToRetainLogs);

		[OperationContract]
		(bool, SYNCSINGLEPASSPHASE) SynchronizeScope(SyncClientConfigurationDO clientSyncConfig, SyncContextFM syncContext,  SyncSessionLogDO syncSessionLog, SyncScopeDO syncScope);

		[OperationContract]
		void CleanupAbandonedSyncController(SecurityClass security);

		[OperationContract]
		void ExecutePostSyncProcessing(SecurityClass security, string syncProfileId, SyncClientConfigurationDO clientSyncConfig, SYNCREQUESTTYPE requestType);

		[OperationContract]
		bool SynchronizeArchiveValues(
			SecurityClass security,
			SecurityClass enterpriseSyncSecurity,
			SyncClientConfigurationDO clientSyncConfig,
			DateTimeOffset startDateTimeOffset,
			Guid siteGuid);

		[OperationContract]
		bool SynchronizeArchiveAlarmAndEvents(
			SecurityClass security,
			SecurityClass enterpriseSyncSecurity,
			SyncClientConfigurationDO clientSyncConfig,
			DateTimeOffset startDateTimeOffset,
			Guid siteGuid);


		#endregion Methods
	}
}
