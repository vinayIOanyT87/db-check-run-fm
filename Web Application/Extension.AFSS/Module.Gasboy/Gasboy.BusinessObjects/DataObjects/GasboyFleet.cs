// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GasboyFleet.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Represents an External Gasboy Service Station Fleet.
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
    using FuelsManager.Afss.Module.Gasboy.BusinessObjects.Constants;

    /// <summary>
    /// Represents an External Gasboy Service Station Fleet.
    /// </summary>
    [DataContract]
    [Serializable]
    public class GasboyFleet : BaseDataObject
    {
        private short authorizationPINSource = 1;

        /// <summary>
        /// Constructor for an External Gasboy Service Station Fleet object.
        /// </summary>
        public GasboyFleet()
        {
            this.IdentityGuid = Guid.Empty;

            this.SiteGuid = Guid.Empty;
	        this.FleetID = GasboySpecialConstants.DefaultFleetID;
            this.FleetCode = GasboySpecialConstants.DefaultFleetCode;
            this.FleetName = GasboySpecialConstants.DefaultFleetName;
            this.GroupRuleName = GasboySpecialConstants.NoRestrictionGroupRuleName;
            this.PriceListName = string.Empty;
            this.RecordStatus = GasboyRecordStatus.Active;
            this.UsePINCode = false;
            this.PINCode = string.Empty;
            this.PromptForVehiclePlate = false;
            this.VehiclePlateCheckType = GasboyVehiclePlateCheckType.ValidVehicleNoForCurrentDevice;
            this.AlwaysPromptForAdditionalValidation = true;
        }

        /// <summary>
        /// Get the entity type, which is used for entity assignments
        /// </summary>
        public override ENTITY_TYPE EntityType
        {
            get { return ENTITY_TYPE.NONE; }
        }

        /// <summary>
        /// Get the parent entity type
        /// </summary>
        public override ENTITY_TYPE ParentEntityType
        {
            get { return ENTITY_TYPE.NONE; }
        }

        /// <summary>
        /// The ID of the Gasboy Fleet Entry
        /// </summary>
        [DataMember]
        [Required(ErrorMessage = "Fleet Name is required")]
        public string FleetName
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
		/// Internal Islander ID that uniquely identifies a Fleet entry
		/// </summary>
		[DataMember]
		[Range(0, 999999999, ErrorMessage = "Fleet ID must be between {1} and {2}")]
		public long? FleetID { get; set; }

		/// <summary>
		/// Uniquely identifies a Fleet entry
		/// </summary>
		[DataMember]
        [Range(0, 999999999, ErrorMessage = "Fleet code must be between {1} and {2}")]
        public long? FleetCode { get; set; }

        /// <summary>
        /// The group rule name to used by Fleet entry
        /// </summary>
        [DataMember]
        public string GroupRuleName { get; set; }

        /// <summary>
        /// The Price List Name to associate to the Fleet entry
        /// </summary>
        [DataMember]
        public string PriceListName { get; set; }

        /// <summary>
        /// Represents the Status of the Fleet entry
        /// </summary>
        [DataMember]
        public GasboyRecordStatus RecordStatus { get; set; }

        /// <summary>
        /// Determines if all departments and devices within this fleet should use a PIN code
        /// </summary>
        [DataMember]
        public bool UsePINCode { get; set; }

        /// <summary>
        /// PIN Code
        /// </summary>
        [DataMember]
        public string PINCode { get; set; }

        /// <summary>
        /// Determines if all departments and devices within this fleet should prompt for a vehicle plate
        /// </summary>
        [DataMember]
        public bool PromptForVehiclePlate { get; set; }

        /// <summary>
        /// Determines whether or not the vehicle plate should be validated and if so, what to validate it against.
        /// </summary>
        [DataMember]
        public GasboyVehiclePlateCheckType VehiclePlateCheckType { get; set; }

        /// <summary>
        /// Determines whether or not the Gasboy unit will always display prompts related to capturing additional vehicle validation.
        /// </summary>
        [DataMember]
        public bool AlwaysPromptForAdditionalValidation{ get; set; }

        /// <summary>
        /// Identifies the PIN authorization source (default is 1 = database)
        /// </summary>
        [DataMember]
        public short AuthorizationPINSource
        {
            get
            {
                return this.authorizationPINSource;
            }
        }

        /// <summary>
        /// Return the values in the External Station to their original values
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            this.IdentityGuid = Guid.NewGuid();

            this.SiteGuid = Guid.Empty;
			this.FleetID = GasboySpecialConstants.DefaultFleetID;
			this.FleetCode = GasboySpecialConstants.DefaultFleetCode;
            this.FleetName = GasboySpecialConstants.DefaultFleetName;
            this.GroupRuleName = GasboySpecialConstants.NoRestrictionGroupRuleName;
            this.PriceListName = string.Empty;
            this.RecordStatus = GasboyRecordStatus.Active;
            this.UsePINCode = false;
            this.PINCode = string.Empty;
            this.PromptForVehiclePlate = false;
            this.VehiclePlateCheckType = GasboyVehiclePlateCheckType.ValidVehicleNoForCurrentDevice;
            this.AlwaysPromptForAdditionalValidation = true;
        }
    }
}
