// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SynchronizationServices.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the SynchronizationServices type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMSynchronizationService
{
    using System;
    using System.IO;
    using System.Web;

    using FMBusinessObjects.DataObjects;

    using FMSynchronizationCommon;

    /// <summary>
    /// The synchronization services.
    /// </summary>
    public class SynchronizationServices : ISynchronizationServices
    {
        /// <summary>
        /// The manually initiate.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="selectedSite">
        /// The selected Site Id or Site Group Id
        /// </param>
        /// <param name="passThruCertificate">
        /// The pass through certificate.
        /// </param>
        /// <param name="requestType">
        /// The request Type.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ManuallyInitiate(SecurityClass security, SyncSelectedSiteDO selectedSite, byte[] passThruCertificate, SYNCREQUESTTYPE requestType)
        {
            SynchronizationProcessor.SetManualSynchronizationEvent(security, selectedSite, passThruCertificate, requestType);

            return true;
        }

        /// <summary>
        /// The manually initiate offline.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="selectedSite">
        /// The selected site id.
        /// </param>
        /// <param name="passThruCertificate">
        /// The pass through certificate.
        /// </param>
        /// <param name="requestType">
        /// The request type.
        /// </param>
        /// <param name="startRange">
        /// The start range.
        /// </param>
        /// <param name="endRange">
        /// The end range.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ManuallyInitiateOffline(SecurityClass security, SyncSelectedSiteDO selectedSite, byte[] passThruCertificate, SYNCREQUESTTYPE requestType, DateTimeOffset? startRange, DateTimeOffset? endRange)
        {
            return SynchronizationProcessor.InitiateManualOfflineSynchronization(security, selectedSite, passThruCertificate, requestType, startRange, endRange);
        }

        /// <summary>
        /// The stop synchronization.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        public void StopSynchronization(SecurityClass security)
        {
            SynchronizationProcessor.StopSynchronization(security);
        }

        /// <summary>
        /// The get service state.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <returns>
        /// The <see cref="SyncServiceStateDO"/>.
        /// </returns>
        public SyncServiceStateDO GetServiceState(SecurityClass security)
        {
            return SynchronizationProcessor.GetServiceState(security);
        }
    }
}
