// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GasboyDepartment.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Represents an External Gasboy Service Station Department
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

	[Serializable]
	[CollectionDataContract]
	public class GasboyDepartmentCollection : List<GasboyDepartment>
	{
	}

	/// <summary>
	/// Represents an External Gasboy Service Station Department.
	/// </summary>
	[DataContract]
    [Serializable]
    public class GasboyDepartment : BaseDataObject
    {
        private int authorizationPINSource = 1;

        /// <summary>
        /// Constructor for an External Gasboy Service Station Department object.
        /// </summary>
        public GasboyDepartment()
        {
            this.IdentityGuid = Guid.Empty;

            this.SiteGuid = Guid.Empty;
            this.FleetIdentityGuid = Guid.Empty;
	        this.DepartmentID = GasboySpecialConstants.DefaultDepartmentID;
			this.DepartmentCode = GasboySpecialConstants.DefaultDepartmentCode;
            this.DepartmentName = GasboySpecialConstants.DefaultDepartmentName;
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
        /// The Department Name of the Gasboy Department Entry
        /// </summary>
        [DataMember]
        [Required(ErrorMessage = "Department Name is required")]
        public string DepartmentName 
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
        /// The Fleet Guid for the Gasboy Fleet Entry
        /// </summary>
        [DataMember]
        [Required(ErrorMessage = "Fleet is required")]
        public Guid FleetIdentityGuid { get; set; }

		/// <summary>
		/// Internal Islander ID that uniquely identifies a Department entry
		/// </summary>
		[DataMember]
		[Range(0, 999999999, ErrorMessage = "Department ID must be between {1} and {2}")]
		public long? DepartmentID { get; set; }

		/// <summary>
		/// Uniquely identifies a Department entry
		/// </summary>
		[DataMember]
        [Range(0, 999999, ErrorMessage = "Department code must be between {1} and {2}")]
        public long? DepartmentCode { get; set; }

        /// <summary>
        /// The group rule name to used by Department entry
        /// </summary>
        [DataMember]
        public string GroupRuleName { get; set; }

        /// <summary>
        /// The Price List Name to associate to the Department entry
        /// </summary>
        [DataMember]
        public string PriceListName { get; set; }

        /// <summary>
        /// Represents the Status of the Department entry
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
        public int AuthorizationPINSource
        {
            get
            {
                return this.authorizationPINSource;
            }
			set
			{
				this.authorizationPINSource = value;
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
            this.FleetIdentityGuid = Guid.Empty;
			this.DepartmentID = GasboySpecialConstants.DefaultDepartmentID;
			this.DepartmentCode = GasboySpecialConstants.DefaultDepartmentCode;
            this.DepartmentName = GasboySpecialConstants.DefaultDepartmentName;
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
