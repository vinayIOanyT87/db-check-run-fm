// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISyncSessions.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ISyncSessions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.ServiceModel;

    using FMBusinessObjects.DataObjects;

    [ServiceContract]
    public interface ISyncSessionLogs
    {
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        Guid Add(SecurityClass security, SyncSessionLogDO syncSession);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void Modify(SecurityClass security, SyncSessionLogDO syncSession);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void Purge(SecurityClass security, Guid syncSessionGuid);

        [OperationContract]
        SyncSessionLogDO Get(SecurityClass security, Guid syncSessionGuid);

        [OperationContract]
        System.Nullable<DateTimeOffset> GetLastSyncDateTime(SecurityClass security);

        [OperationContract]
        Dictionary<Guid, string> GetRemoteNodes(SecurityClass security);

		[OperationContract]
	    DataSet GetNodeHealthSummary(SecurityClass security, int nodeStatus);

	    [OperationContract]
	    DataSet GetNodeHealthSummaryWithOrder(SecurityClass security, string orderBy, int nodeStatus);

        [OperationContract]
        SyncSessionLogCollection Enumerate(SecurityClass security, Guid syncNodeGuid, DateTimeOffset? startDateTimeOffset, DateTimeOffset? endDateTimeOffset, bool? withConflicts);

        [OperationContract]
        SyncSessionLogCollection EnumerateActive(SecurityClass security, Guid syncNodeGuid);

        [OperationContract]
        SyncSessionLogCollection EnumerateExt(SecurityClass security, Guid syncNodeGuid, DateTimeOffset? startDateTimeOffset, DateTimeOffset? endDateTimeOffset, bool? onlyActiveFlag, bool? withConflicts);

        [OperationContract]
        void CloseActiveSessions(SecurityClass security);
    }
}
