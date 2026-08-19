// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISyncClientConfigurations.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   The SyncClientConfigurations interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
    using System;
    using System.ServiceModel;

    using FMBusinessObjects.DataObjects;

    /// <summary>
    /// The SyncClientConfigurations interface.
    /// </summary>
    [ServiceContract]
	public interface ISyncClientConfigurations
	{
        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="syncClientConfiguration">
        /// The sync client configuration.
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        Guid Add(SecurityClass security, SyncClientConfigurationDO syncClientConfiguration);

        /// <summary>
        /// The modify.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="syncClientConfiguration">
        /// The sync client configuration.
        /// </param>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void Modify(SecurityClass security, SyncClientConfigurationDO syncClientConfiguration);

        /// <summary>
        /// The purge.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="syncClientConfigurationGuid">
        /// The sync client configuration guid.
        /// </param>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void Purge(SecurityClass security, Guid syncClientConfigurationGuid);

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <returns>
        /// The <see cref="SyncClientConfigurationDO"/>.
        /// </returns>
        [OperationContract]
        SyncClientConfigurationDO Get(SecurityClass security);
    }
}
