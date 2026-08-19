// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncOfflinePayload.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   The sync offline payload.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Data;
    using System.Runtime.Serialization;

    using Microsoft.Synchronization.Data;

    /// <summary>
    /// The sync offline payload.
    /// </summary>
    [Serializable]
    [DataContract]
    [KnownType(typeof(SyncContextFM))]
    [KnownType(typeof(SyncGroupMetadata))]
    [KnownType(typeof(DataSet))]
    [KnownType(typeof(SyncSession))]
    [KnownType(typeof(SyncContext))]
    [KnownType(typeof(DBNull))]
    public class SyncOfflinePayload
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the context fuels manager.
        /// </summary>
        [DataMember]
// ReSharper disable InconsistentNaming
        public SyncContextFM ContextFM { get; set; }
// ReSharper restore InconsistentNaming

        /// <summary>
        /// Gets or sets the group metadata.
        /// </summary>
        [DataMember]
        public SyncGroupMetadata GroupMetadata { get; set; }

        /// <summary>
        /// Gets or sets the record sets.
        /// </summary>
        [DataMember]
        public DataSet RecordSets { get; set; }

        /// <summary>
        /// Gets or sets the session.
        /// </summary>
        [DataMember]
        public SyncSession Session { get; set; }

        /// <summary>
        /// Gets or sets the synchronization framework context.  This is used during the return trip.
        /// </summary>
        [DataMember]
        public SyncContext Context { get; set; }
        
        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// The get payload name.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an exception if the FuelsManager synchronization context is not provided.
        /// </exception>
        public string GetPayloadName()
        {
            if (null == this.ContextFM)
            {
                throw new Exception("Cannot synchronize offline without a synchronization Context");
            }

            string step;

            switch (this.ContextFM.CurrentControllerStep)
            {
                case SYNCCONTROLLERSTEP.PROCESS_INSERT_UPDATE:
                case SYNCCONTROLLERSTEP.PROCESS_INSERT_UPDATE_CONFLICT:
                    step = "_INSUPD";
                    break;
                case SYNCCONTROLLERSTEP.PROCESS_DELETE:
                case SYNCCONTROLLERSTEP.PROCESS_DELETE_CONFLICT:
                    step = "_DEL";
                    break;
                default:
                    step = "_ALL";
                    break;
            }

            string payloadName = string.Format(
                "{0}_{1}{2}{3}",
                this.ContextFM.CurrentSyncProfileID,
                this.ContextFM.CurrentSyncScopeID,
                !string.IsNullOrEmpty(this.ContextFM.CurrentSiteID) ? "_" + this.ContextFM.CurrentSiteID : string.Empty,
                step);

            return payloadName;
        }
        #endregion Public Methods
    }
}
