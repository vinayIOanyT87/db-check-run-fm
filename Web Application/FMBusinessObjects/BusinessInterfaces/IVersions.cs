// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IVersions.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the IVersions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
    using System;
    using System.ServiceModel;

    using FMBusinessObjects.DataObjects;

    [ServiceContract]
    public interface IVersions
    {
        [OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, VersionDO version);

        [OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid versionGuid);

        [OperationContract]
        VersionDO GetCurrent(SecurityClass security);

        [OperationContract]
        VersionDO Get(SecurityClass security, Guid versionGuid);

        [OperationContract]
        VersionCollection Enumerate(SecurityClass security);

        [OperationContract]
        VersionCollection EnumerateExt(SecurityClass security, int limit = 0);
    }
}
