// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GasboyDevice.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Represents an External Gasboy Service Station Device
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
	/// Represents an External Gasboy Service Station Device.
	/// </summary>
	[DataContract]
	[Serializable]
	public class GasboyDevice : BaseDataObject
	{
		private int authorizationPINSource = 1;

		/// <summary>
		/// Constructor for an External Gasboy Service Station Device object.
		/// </summary>
		public GasboyDevice()
		{
			this.IdentityGuid = Guid.Empty;

			this.SiteGuid = Guid.Empty;
			this.DepartmentIdentityGuid = Guid.Empty;
			this.DeviceID = 900000000;
			this.DeviceCode = null;
			this.DeviceName = string.Empty;
			this.CardNumber = string.Empty;
			this.GroupRuleName = GasboySpecialConstants.NoRestrictionGroupRuleName;
			this.DeviceType = GasboyDeviceType.Vehicle;
			this.RecordStatus = GasboyRecordStatus.Active;
			this.HardwareType = GasboyHardwareType.Tag;
			this.AuthorizationType = GasboyAuthorizationType.FuelCard;
			this.EmployeeType = GasboyEmployeeType.Attendant;
			this.VehiclePlate = string.Empty;
			this.DriverValidationType = GasboyTwoStageDriverValidationType.NotSelected;
			this.UsePINCode = false;
			this.PINCode = string.Empty;
			this.PromptForVehiclePlate = false;
			this.VehiclePlateCheckType = GasboyVehiclePlateCheckType.ValidVehicleNoForCurrentDevice;
			this.AlwaysPromptForAdditionalValidation = true;

			this.FleetID = null;
			this.FleetCode = null;
			this.DepartmentID = null;
			this.DepartmentCode = null;
		}

		#region Non-Persisted Navigational Data

		/// <summary>
		/// Get the entity type, which is used for entity assignments
		/// </summary>
		public override ENTITY_TYPE EntityType
		{
			get { return ENTITY_TYPE.EXTERNAL_STATION_DEVICE; }
		}

		/// <summary>
		/// Get the parent entity type
		/// </summary>
		public override ENTITY_TYPE ParentEntityType
		{
			get { return ENTITY_TYPE.NONE; }
		}

		/// <summary>
		/// The Fleet ID for the corresponding Gasboy Fleet record associated with the Department of the Device
		/// </summary>
		[DataMember]
		public int? FleetID{ get; set; }

		/// <summary>
		/// The Fleet Code for the corresponding Gasboy Fleet record associated with the Department of the Device
		/// </summary>
		[DataMember]
		public int? FleetCode { get; set; }

		/// <summary>
		/// The Department ID for the corresponding Gasboy Department record referenced by the DepartmentIdentityGuid
		/// </summary>
		[DataMember]
		public int? DepartmentID { get; set; }

		/// <summary>
		/// The Department Code for the corresponding Gasboy Department record referenced by the DepartmentIdentityGuid
		/// </summary>
		[DataMember]
		public int? DepartmentCode { get; set; }

		#endregion Non-Persisted Navigational Data

		#region Persisted Data Members

		/// <summary>
		/// The Department Guid for the Gasboy Department Entry
		/// </summary>
		[DataMember]
		[Required(ErrorMessage = "Department is required")]
		public Guid DepartmentIdentityGuid { get; set; }

		/// <summary>
		/// The Device Name of the Gasboy Device Entry
		/// </summary>
		[DataMember]
		[Required(ErrorMessage = "Device Name is required")]
		public string DeviceName
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
		/// Uniquely identifies a Device entry
		/// </summary>
		[DataMember]
		[Range(0, 999999999, ErrorMessage = "Device ID must be between {1} and {2}")]
		public long DeviceID { get; set; }

		/// <summary>
		/// Uniquely identifies a Device entry
		/// </summary>
		[DataMember]
		[Range(0, 999999999, ErrorMessage = "Device code must be between {1} and {2}")]
		public long? DeviceCode { get; set; }

		/// <summary>
		/// The card number associated with the Device
		/// </summary>
		[DataMember]
		public string CardNumber { get; set; }

		/// <summary>
		/// The type of Gasboy Device
		/// </summary>
		[DataMember]
		public GasboyDeviceType DeviceType { get; set; }

		/// <summary>
		/// Represents the Status of the Device entry
		/// </summary>
		[DataMember]
		public GasboyRecordStatus RecordStatus { get; set; }

		/// <summary>
		/// The hardware type associated with the Gasboy Device
		/// </summary>
		[DataMember]
		public GasboyHardwareType HardwareType { get; set; }

		/// <summary>
		/// The authorization type associated with the Gasboy Device
		/// </summary>
		[DataMember]
		public GasboyAuthorizationType AuthorizationType { get; set; }

		/// <summary>
		/// The employee type associated with the Gasboy Device
		/// </summary>
		[DataMember]
		public GasboyEmployeeType EmployeeType { get; set; }

		/// <summary>
		/// The driver validation type associated with the Gasboy Device
		/// </summary>
		[DataMember]
		public GasboyTwoStageDriverValidationType DriverValidationType { get; set; }

		/// <summary>
		/// The group rule name to used by Device entry
		/// </summary>
		[DataMember]
		public string GroupRuleName { get; set; }

		/// <summary>
		/// The vehicle plate no to associate to the Device entry
		/// </summary>
		[DataMember]
		public string VehiclePlate { get; set; }

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
		public bool AlwaysPromptForAdditionalValidation { get; set; }

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

		#endregion Persisted Data Members

		/// <summary>
		/// Return the values in the External Station to their original values
		/// </summary>
		public override void Reset()
		{
			base.Reset();

			this.IdentityGuid = Guid.NewGuid();

			this.SiteGuid = Guid.Empty;
			this.DepartmentIdentityGuid = Guid.Empty;
			this.DeviceID = 900000000;
			this.DeviceCode = null;
			this.DeviceName = string.Empty;
			this.CardNumber = string.Empty;
			this.GroupRuleName = GasboySpecialConstants.NoRestrictionGroupRuleName;
			this.DeviceType = GasboyDeviceType.Vehicle;
			this.RecordStatus = GasboyRecordStatus.Active;
			this.HardwareType = GasboyHardwareType.Tag;
			this.AuthorizationType = GasboyAuthorizationType.FuelCard;
			this.EmployeeType = GasboyEmployeeType.Attendant;
			this.VehiclePlate = string.Empty;
			this.DriverValidationType = GasboyTwoStageDriverValidationType.NotSelected;
			this.UsePINCode = false;
			this.PINCode = string.Empty;
			this.PromptForVehiclePlate = false;
			this.VehiclePlateCheckType = GasboyVehiclePlateCheckType.ValidVehicleNoForCurrentDevice;
			this.AlwaysPromptForAdditionalValidation = true;

			this.FleetID = null;
			this.FleetCode = null;
			this.DepartmentID = null;
			this.DepartmentCode = null;
		}
	}
}
