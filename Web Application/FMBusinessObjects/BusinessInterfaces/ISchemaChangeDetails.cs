// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISchemaChangeDetails.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ISchemaChangeDetails type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
    using System;
    using System.ServiceModel;

    using FMBusinessObjects.DataObjects;

    [ServiceContract]
    public interface ISchemaChangeDetails
    {
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        Guid Add(SecurityClass security, SchemaChangeDetailDO schemaChangeDetail);

        [OperationContract]
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        void Modify(SecurityClass security, SchemaChangeDetailDO schemaChangeDetail);

        [OperationContract]
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        void Purge(SecurityClass security, Guid identityGuid);

        [OperationContract]
        SchemaChangeDetailDO Get(SecurityClass security, Guid identityGuid);

        [OperationContract]
        SchemaChangeDetailCollection Enumerate(SecurityClass security, Guid schemaChangeHistoryGuid);

        [OperationContract]
        SchemaChangeDetailCollection EnumerateExt(SecurityClass security, Guid schemaChangeHistoryGuid, int limit = 0);
    }
}
