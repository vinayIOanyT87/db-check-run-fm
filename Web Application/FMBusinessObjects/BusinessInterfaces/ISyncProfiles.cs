// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISyncProfiles.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ISyncProfiles type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
    using System;
    using System.ServiceModel;

    using FMBusinessObjects.DataObjects;

    [ServiceContract]
    public interface ISyncProfiles
    {
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        Guid Add(SecurityClass pSecurity, SyncProfileDO pSyncProfile);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void Modify(SecurityClass pSecurity, SyncProfileDO pSyncProfile);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void Purge(SecurityClass pSecurity, Guid pSyncProfileGuid);

        [OperationContract]
        SyncProfileDO Get(SecurityClass pSecurity, Guid pSyncProfileGuid);

        [OperationContract]
        SyncProfileDO GetById(SecurityClass pSecurity, string pID);

        [OperationContract]
        Guid GetIdentityGuid(SecurityClass pSecurity, string ID);

        [OperationContract]
        SyncProfileCollection Enumerate(SecurityClass pSecurity);

        [OperationContract]
        SyncProfileCollection EnumerateExt(SecurityClass pSecurity, int limit = 0);
    }
}
