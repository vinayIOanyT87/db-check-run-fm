// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISyncConflictRecords.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ISyncRecordConflicts type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
    using System;
    using System.ServiceModel;

    using FMBusinessObjects.DataObjects;

    [ServiceContract]
    public interface ISyncRecordConflicts
    {
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        Guid Add(SecurityClass security, SyncSessionScopeLogDO syncSessionDetail, SyncRecordConflictDO syncRecordConflict);

        [OperationContract]
        void Modify(SecurityClass security, SyncRecordConflictDO syncRecordConflict);

        [OperationContract]
        void Purge(SecurityClass security, Guid syncRecordConflictGuid);

        [OperationContract]
        SyncRecordConflictDO Get(SecurityClass security, Guid syncRecordConflictGuid);

        [OperationContract]
        SyncRecordConflictDO GetByTableAndEntityKey(SecurityClass security, string tableName, string entityKey, bool onlyUnresolved);

        [OperationContract]
		SyncRecordConflictCollection Enumerate(SecurityClass security);

        [OperationContract]
        SyncRecordConflictCollection EnumerateUnresolved(SecurityClass security, Guid syncNodeGuid, Int64? maxRecords, Int64 startRowVersion);

        [OperationContract]
        SyncRecordConflictCollection EnumerateByStatus(
            SecurityClass security,
            SYNCCONFLICTRESOLUTIONSTATUS conflictResolutionStatus,
            Guid? syncSessionLogGuid);

		[OperationContract]
		SyncRecordConflictCollection EnumerateBySyncSessionLog(SecurityClass security, Guid syncSessionLogGuid, Int64? maxRecords, Int64 startRowVersion);
		
		[OperationContract]
        SyncRecordConflictCollection EnumerateBySyncSessionScopeLog(SecurityClass security, Guid syncSessionScopeLogGuid);

        [OperationContract]
		SyncRecordConflictCollection EnumerateExt(SecurityClass security);

        [OperationContract]
		SyncRecordConflictCollection EnumerateUnresolvedExt(SecurityClass security, Guid syncNodeGuid, Int64? maxRecords, Int64 startRowVersion);

        [OperationContract]
        SyncRecordConflictCollection EnumerateByStatusExt(
            SecurityClass security,
            SYNCCONFLICTRESOLUTIONSTATUS conflictResolutionStatus,
            Guid? syncSessionLogGuid);

		[OperationContract]
		SyncRecordConflictCollection EnumerateBySyncSessionLogExt(SecurityClass security, Guid syncSessionLogGuid, Int64? maxRecords, Int64 startRowVersion);
		
		[OperationContract]
        SyncRecordConflictCollection EnumerateBySyncSessionScopeLogExt(SecurityClass security, Guid syncSessionScopeLogGuid);

        [OperationContract]
        SyncRecordConflictCountDO GetUnresolvedConflictsCount(SecurityClass security, Guid? syncNodeGuid);
    };
}
