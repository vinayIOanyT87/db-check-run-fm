// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISchemaChangeHistories.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ISchemaChangeHistories type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
    using System;
    using System.ServiceModel;

    using FMBusinessObjects.DataObjects;

    [ServiceContract]
    public interface ISchemaChangeHistories
    {
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        Guid Add(SecurityClass security, SchemaChangeHistoryDO schemaChangeHistory);

        [OperationContract]
        void Modify(SecurityClass security, SchemaChangeHistoryDO schemaChangeHistory);

        [OperationContract]
        void Purge(SecurityClass security, Guid identityGuid);

        [OperationContract]
        SchemaChangeHistoryDO GetByVersion(SecurityClass security, string versionNumber);

        [OperationContract]
        SchemaChangeHistoryDO Get(SecurityClass security, Guid identityGuid);

        [OperationContract]
        SchemaChangeHistoryCollection Enumerate(SecurityClass security);

        [OperationContract]
        SchemaChangeHistoryCollection EnumerateExt(SecurityClass security, int limit = 0);
    }
}
