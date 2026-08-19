// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISynchronizationServices.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ISynchronizationServices type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMSynchronizationCommon
{
    using System.Collections.Specialized;
    using System.ServiceModel;
    using System.Web;

    using FMBusinessObjects.DataObjects;

    /// <summary>
    /// The SynchronizationServices interface.
    /// </summary>
    [ServiceContract]
    [ServiceKnownType(typeof(SecurityClass))]
    [ServiceKnownType(typeof(SyncSelectedSiteDO))]
    [ServiceKnownType(typeof(SyncServiceStateDO))]
    [ServiceKnownType(typeof(SYNCREQUESTTYPE))]
    public interface ISynchronizationServices
    {
        /// <summary>
        /// The manually initiate.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="selectedSite">
        /// The selected Site Id.
        /// </param>
        /// <param name="passThruCertificate">
        /// The pass thru certificate.
        /// </param>
        /// <param name="requestType">
        /// The request Type.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [OperationContract]
        bool ManuallyInitiate(SecurityClass security, SyncSelectedSiteDO selectedSite, byte[] passThruCertificate, SYNCREQUESTTYPE requestType);

        /// <summary>
        /// The stop synchronization.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        [OperationContract]
        void StopSynchronization(SecurityClass security);

        /// <summary>
        /// The get service state.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <returns>
        /// The <see cref="SyncServiceStateDO"/>.
        /// </returns>
        [OperationContract]
        SyncServiceStateDO GetServiceState(SecurityClass security);
    }
}
