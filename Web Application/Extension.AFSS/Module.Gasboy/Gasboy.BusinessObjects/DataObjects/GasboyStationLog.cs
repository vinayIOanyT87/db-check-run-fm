// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GasboyStationLog.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Represents an error encountered when attempting to interact with a Gasboy Station. 
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;
    using FMBusinessObjects.DataObjects;

    /// <summary>
    /// The types of log entries for an Gasboy station
    /// </summary>
    public enum ExternalStationLogType
    {
        [Display(Name = "Connection Failure")]
        ConnectionFailure = 0,
        [Display(Name = "Validation Failure")]
        ValidationFailure = 1,
        [Display(Name = "Station Event")]
        StationEvent = 2
    }

    /// <summary>
    /// Represents an error encountered when attempting to interact with an External Station. 
    /// </summary>
    [DataContract]
    [Serializable]
    [KnownType(typeof(GasboyStationEvent))]
    public class GasboyStationLog : BaseDataObject
    {
        /// <summary>
        /// The ID of the external station the log entry pertains to
        /// </summary>
        [DataMember]
        public string ExternalStationID { get; set; }

        /// <summary>
        /// Identifies the external station the log entry pertains to
        /// </summary>
        [DataMember]
        public Guid ExternalStationGuid { get; set; }

        /// <summary>
        /// Identifies the external station the log entry pertains to
        /// </summary>
        [DataMember]
        public string LogText { get; set; }

        /// <summary>
        /// Identifies the type of log entry, for example connection failed or transaction validation failed
        /// </summary>
        [DataMember]
        public ExternalStationLogType LogType { get; set; }

        /// <summary>
        /// Identifies the date and time the log event occurred
        /// </summary>
        [DataMember]
        public DateTimeOffset LogDate { get; set; }

        /// <summary>
        /// Return the values in the External Station Log to their original values
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            this.ExternalStationGuid = Guid.Empty;
            this.ExternalStationID = string.Empty;
            this.LogType = ExternalStationLogType.ConnectionFailure;
            this.LogDate = DateTimeOffset.Now;
        }
    }
}
