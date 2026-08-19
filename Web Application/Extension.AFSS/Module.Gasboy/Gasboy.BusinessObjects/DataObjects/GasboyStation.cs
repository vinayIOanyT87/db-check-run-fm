// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GasboyStation.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Represents an External Fuel Service Station
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using System.Runtime.Serialization;

    using FMBusinessObjects.DataObjects;

    using FuelsManager.Afss.BusinessObjects.DataObjects;

    /// <summary>
    /// Describes the potential statuses of an External Station's connection
    /// </summary>
    public enum ExternalStationStatus
    {
        [Display(Name = "Inactive")]
        Inactive = 0,
        [Display(Name = "Good")]
        Good = 1,
        [Display(Name = "Bad")]
        Bad = 2,
		[Display(Name = "No Communication")]
		NoCommunication = 3,
	}

    /// <summary>
    /// Represents an External Fuel Service Station and information needed to connect to it.
    /// </summary>
    [DataContract]
    [Serializable]
    public class GasboyStation : BaseDataObject
    {
        /// <summary>
        /// The default value to display in password boxes if a password is present
        /// </summary>
        public const string PasswordDefaultValue = "********";

        /// <summary>
        /// Constructor for an Gasboy Station object
        /// </summary>
        public GasboyStation()
        {
            this.IdentityGuid = Guid.Empty;

            this.BillingID = string.Empty;
            this.SiteCode = null;
            this.UserName = string.Empty;
            this.Password = string.Empty;
            this.IpAddress = string.Empty;
            this.DownloadTransactionsAutomatically = false;
            this.IsSelected = false;
            this.Status = ExternalStationStatus.Inactive;
            this.LastConnectionAttempt = null;
            this.LastSuccessfulConnection = null;
            this.LastTransactionID = null;
            this.ProductMappings = new List<GasboyStationProductMapping>();
        }

        /// <summary>
        /// Get the entity type, which is used for entity assignments
        /// </summary>
        public override ENTITY_TYPE EntityType
        {
            get { return ENTITY_TYPE.EXTERNAL_STATION; }
        }

        /// <summary>
        /// Get the parent entity type
        /// </summary>
        public override ENTITY_TYPE ParentEntityType
        {
            get { return ENTITY_TYPE.NONE; }
        }

        /// <summary>
        /// The ID of the External Station
        /// </summary>
        [DataMember]
        [Required(ErrorMessage = "ID is required")]
        public override string ID
        {
            get
            {
                return base.ID;
            }

            set
            {
                base.ID = value;
            }
        }

        /// <summary>
        /// Uniquely identifies an external station
        /// </summary>
        [DataMember]
        [Range(0, 999999, ErrorMessage = "Site code must be between {1} and {2}")]
        public int? SiteCode { get; set; }

        /// <summary>
        /// The user name to use when connecting to the External Station
        /// </summary>
        [DataMember]
        public string UserName { get; set; }

        /// <summary>
        /// The Password to use when connecting to the External Station
        /// </summary>
        [DataMember]
        public string Password { get; set; }

        /// <summary>
        /// The IP Address to use when connecting to the External Station
        /// </summary>
        [DataMember]
        public string IpAddress { get; set; }

        /// <summary>
        /// BillingID  for the External Station - (i.e.: Department of Defense Activity Address Code)
        /// </summary>
        [DataMember]
        public string BillingID { get; set; }

        /// <summary>
        /// If true, we will attempt to download transactions from this External Station a periodic interval
        /// </summary>
        [DataMember]
        public bool DownloadTransactionsAutomatically { get; set; }

        /// <summary>
        /// The status of the connection to the External Station
        /// </summary>
        [DataMember]
        public ExternalStationStatus Status { get; set; }

        /// <summary>
        /// Used on the external station operation screen to determine if the station has been selected by the user
        /// </summary>
        [DataMember]
        public bool IsSelected { get; set; }

        /// <summary>
        /// The last time we successfully connected to the station
        /// </summary>
        [DataMember]
        public DateTimeOffset? LastSuccessfulConnection { get; set; }

		/// <summary>
		/// The number of devices we last pushed to the station
		/// </summary>
		[DataMember]
		public int? LastDeviceCount  { get; set; }

		/// <summary>
		/// The last time we attempted to connect to the station
		/// </summary>
		[DataMember]
        public DateTimeOffset? LastConnectionAttempt { get; set; }

        /// <summary>
        /// The last transaction ID we downloaded from the station.
        /// Note that the ID is the transaction ID generated by the station, not by FuelsManager
        /// </summary>
        [DataMember]
        public long? LastTransactionID { get; set; }

        /// <summary>
        /// Product Mappings between products defined at the External Station and products defined in FuelsManager
        /// </summary>
        [DataMember]
        public List<GasboyStationProductMapping> ProductMappings { get; set; }

        /// <summary>
        /// Return the values in the External Station to their original values
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            this.IdentityGuid = Guid.Empty;

            this.BillingID = string.Empty;
            this.SiteCode = null;
            this.UserName = string.Empty;
            this.Password = string.Empty;
            this.IpAddress = string.Empty;
            this.DownloadTransactionsAutomatically = false;
            this.IsSelected = false;
            this.Status = ExternalStationStatus.Inactive;
            this.LastConnectionAttempt = null;
            this.LastSuccessfulConnection = null;
            this.LastTransactionID = null;
            this.ProductMappings = new List<GasboyStationProductMapping>();
        }
    }
}
