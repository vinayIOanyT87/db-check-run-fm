// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEnterpriseSynchronization.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	The EnterpriseSynchronization interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;	
	using System.Collections.ObjectModel;
	using System.Data;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;

	using Microsoft.Synchronization.Data;

	/// <summary>
	/// The EnterpriseSynchronization interface.
	/// </summary>
	[ServiceContract]
	public interface IEnterpriseSynchronization
	{
		 /// <summary>
		 /// The create session.
		 /// </summary>
		 /// <param name="syncLoginRequest">
		 /// The sync login request.
		 /// </param>
		 /// <param name="security">
		 /// The security.
		 /// </param>
		 /// <param name="message">
		 /// The message.
		 /// </param>
		 /// <returns>
		 /// The <see cref="SYNCSERVICESTATE"/>.
		 /// </returns>
		 [OperationContract]
		 SYNCSERVICESTATE CreateSession(SecuritySyncLoginRequest syncLoginRequest, out SecurityClass security, out string message);

		/// <summary>
		/// The end session.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="syncSessionID">Synchronization Session ID associated with the FuelsManager Session that is being terminated.</param>
		/// <param name="finalSessionStatus">
		/// The final session status.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		[OperationContract]
		 string EndSession(SecurityClass security, Guid syncSessionID, SYNCSESSIONSTATUS finalSessionStatus);

		 /// <summary>
		 /// The get server id.
		 /// </summary>
		 /// <returns>
		 /// The <see cref="Guid"/>.
		 /// </returns>
		 [OperationContract]
		 Guid GetServerID();

		 /// <summary>
		 /// The get node name.
		 /// </summary>
		 /// <returns>
		 /// The name of the synchronization node.
		 /// </returns>
		 [OperationContract]
		 string GetNodeName();

		 /// <summary>
		 /// The get synchronization site list.
		 /// </summary>
		 /// <param name="remoteSiteList">
		 /// The remote site list.
		 /// </param>
		 /// <param name="syncContext">
		 /// The sync context.
		 /// </param>
		 /// <returns>
		 /// The <see cref="SiteSyncList"/>.
		 /// </returns>
		 [OperationContract]
		 SiteSyncList GetSynchronizationSiteList(SiteSyncList remoteSiteList, SyncContextFM syncContext);

		 /// <summary>
		 /// Gets the current enterprise synchronization anchor.
		 /// </summary>
		 /// <returns>
		 /// The current Enterprise Sync Anchor
		 /// </returns>
		 [OperationContract]
		 long GetEnterpriseSyncAnchor();

		 /// <summary>
		 /// The apply changes.
		 /// </summary>
		 /// <param name="groupMetadata">
		 /// The group metadata.
		 /// </param>
		 /// <param name="dataSetSurrogateBytes">
		 /// The data set.
		 /// </param>
		 /// <param name="syncSession">
		 /// The sync session.
		 /// </param>
		 /// <param name="syncContextFMBytes">
		 /// A serialized compressed byte array that contains an instance of the <see cref="SyncContextFM"/> object which provides FuelsManager synchronization attributes.
		 /// </param>
		 /// <returns>
		 /// The <see cref="SyncContext"/>.
		 /// </returns>
		 [OperationContract]
		 SyncContext ApplyChanges(SyncGroupMetadata groupMetadata, byte[] dataSetSurrogateBytes, SyncSession syncSession, byte[] syncContextFMBytes);

		 /// <summary>
		 /// The get changes.
		 /// </summary>
		 /// <param name="groupMetadata">
		 /// The group metadata.
		 /// </param>
		 /// <param name="syncSession">
		 /// The sync session.
		 /// </param>
		 /// <param name="syncContextFMBytes">
		 /// A serialized compressed byte array that contains an instance of the <see cref="SyncContextFM"/> object which provides FuelsManager synchronization attributes.
		 /// </param>
		 /// <param name="dataSetSurrogateBytes">
		 /// DataSet as a byte array
		 /// </param>
		 /// <returns>
		 /// The <see cref="SyncContext"/>.
		 /// </returns>
		 [OperationContract]
		 SyncContext GetChanges(SyncGroupMetadata groupMetadata, SyncSession syncSession, byte[] syncContextFMBytes, out byte[] dataSetSurrogateBytes);

		 /// <summary>
		 /// The get schema.
		 /// </summary>
		 /// <param name="tableNames">
		 /// The table names.
		 /// </param>
		 /// <param name="syncSession">
		 /// The sync session.
		 /// </param>
		 /// <param name="syncContextFMBytes">
		 /// A serialized compressed byte array that contains an instance of the <see cref="SyncContextFM"/> object which provides FuelsManager synchronization attributes.
		 /// </param>
		 /// <returns>
		 /// The <see cref="SyncSchema"/>.
		 /// </returns>
		 [OperationContract]
		 SyncSchema GetSchema(Collection<string> tableNames, SyncSession syncSession, byte[] syncContextFMBytes);

		 /// <summary>
		 /// The get server info.
		 /// </summary>
		 /// <param name="syncSession">
		 /// The sync session.
		 /// </param>
		 /// <param name="syncContextFMBytes">
		 /// A serialized compressed byte array that contains an instance of the <see cref="SyncContextFM"/> object which provides FuelsManager synchronization attributes.
		 /// </param>
		 /// <returns>
		 /// The <see cref="SyncServerInfo"/>.
		 /// </returns>
		 [OperationContract]
		 SyncServerInfo GetServerInfo(SyncSession syncSession, byte[] syncContextFMBytes);

		/// <summary>
		/// Reprocesses the conflicts.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="syncNodeGuid">Remote synchronization node to reprocess synchronization conflicts for.</param>
		/// <param name="syncSessionID">Synchronization Session ID to reprocess synchronization conflicts for.</param>
		[OperationContract]
		SyncConflictResolutionStatus ReprocessConflicts(SecurityClass security, Guid syncNodeGuid, Guid syncSessionID, SyncConflictResolutionStatus syncConflictResolutonStatus);

		/// <summary>
		/// Purges the logs.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="syncNodeGuid">The synchronize node unique identifier.</param>
		/// <param name="maximumDaysToRetainLogs">The number of days to retain logs.</param>
		[OperationContract]
		void PurgeLogs(SecurityClass security, Guid syncNodeGuid, int maximumDaysToRetainLogs);

		/// <summary>
		/// Synchronizes the archive.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="archiveDataElementList">The archive data element list.</param>
		[OperationContract]
		void SynchronizeValueArchive(SecurityClass security, List<ArchiveDataElement> archiveDataElementList);


		/// <summary>
		/// Synchronizes the archive.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="archiveDataElementList">The archive data element list.</param>
		[OperationContract]
		void SynchronizeAlarmAndEventArchive(SecurityClass security, List<AandEDataElement> archiveDataElementList);
	}
}
