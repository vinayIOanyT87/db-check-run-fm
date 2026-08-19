// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISyncSessionDetails.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ISyncSessionDetails type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;

    using FMBusinessObjects.DataObjects;

    [ServiceContract]
    public interface ISyncSessionScopeLogs
    {
        [OperationContract]
        Guid Add(SecurityClass security, SyncSessionScopeLogDO syncSessionScopeLog);

        [OperationContract]
        void Modify(SecurityClass security, SyncSessionScopeLogDO syncSessionScopeLog);

        [OperationContract]
        void Purge(SecurityClass security, Guid syncSessionScopeLogGuid);

        [OperationContract]
        SyncSessionScopeLogDO Get(SecurityClass security, Guid syncSessionScopeLogGuid);

        [OperationContract]
        SyncSessionScopeLogDO GetBySiteGuid(SecurityClass security, Guid syncSessionLogGuid, Guid? siteGuid);

        [OperationContract]
        SyncSessionScopeLogDO GetByCompositeKey(SecurityClass security, Guid syncSessionLogGuid, Guid? siteGuid, string scopeID);

        [OperationContract]
        SyncSessionScopeLogCollection Enumerate(SecurityClass security, Guid syncSessionLogGuid);

        [OperationContract]
        SyncSessionScopeLogCollection EnumerateExt(SecurityClass security, Guid syncSessionLogGuid, int limit);
    }
}
