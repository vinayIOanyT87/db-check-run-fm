// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatchEquipmentDisplayDO.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Transport class for display information for dispatch equipment.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	using System.Runtime.Serialization;

	/// <summary>
	/// Transport class for display information for dispatch equipment.
	/// </summary>
	[DataContract]
	public class DispatchEquipmentDisplayDO
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DispatchEquipmentDisplayDO"/> class.
		/// </summary>
		/// <param name="equipment">The equipment to use for initialization.</param>
		public DispatchEquipmentDisplayDO (EquipmentClass equipment)
		{
			this.IdentityGuid = equipment.IdentityGuid.ToString();
			this.RegID = equipment.Xref;
			this.Vehicle = equipment.ID;
			this.Type = equipment.EqTypeName;
			this.Grade = equipment.ProductID;
			this.Volume = equipment.Volume;
			this.FuelAdditiveFlag = equipment.FuelAdditiveFlag;
			this.InService = equipment.InServiceFlag;
			this.FuelingState = equipment.FuelingState;
			this.IssPt = equipment.IssPt;
			this.IssPtNum = equipment.IssPtNum;
			this.TypeEnum = equipment.Type.ToString();
			this.LockedOut = equipment.LockedOut;
			this.Color = string.Empty;
		}

		/// <summary>
		/// Gets or sets the identity GUID.
		/// </summary>
		[DataMember]
		public string IdentityGuid { get; set; }

		/// <summary>
		/// Gets or sets the reg ID.
		/// </summary>
		[DataMember]
		public string RegID { get; set; }

		/// <summary>
		/// Gets or sets the vehicle.
		/// </summary>
		[DataMember]
		public string Vehicle { get; set; }

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		[DataMember]
		public string Type { get; set; }

		/// <summary>
		/// Gets or sets the grade.
		/// </summary>
		[DataMember]
		public string Grade { get; set; }

		/// <summary>
		/// Gets or sets the volume.
		/// </summary>
		[DataMember]
		public string Volume { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether fuel additive flag.
		/// </summary>
		[DataMember]
		public bool FuelAdditiveFlag { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether in service.
		/// </summary>
		[DataMember]
		public bool InService { get; set; }

		/// <summary>
		/// Gets or sets the fueling state.
		/// </summary>
		[DataMember]
		public string FuelingState { get; set; }

		/// <summary>
		/// Gets or sets the iss pt.
		/// </summary>
		[DataMember]
		public string IssPt { get; set; }

		/// <summary>
		/// Gets or sets the iss pt num.
		/// </summary>
		[DataMember]
		public string IssPtNum { get; set; }

		/// <summary>
		/// Gets or sets the type enum.
		/// </summary>
		[DataMember]
		public string TypeEnum { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether locked out.
		/// </summary>
		[DataMember]
		public bool LockedOut { get; set; }

		/// <summary>
		/// Gets or sets the color.
		/// </summary>
		[DataMember]
		public string Color { get; set; }

		/// <summary>
		/// Gets or sets the selection back color.
		/// </summary>
		[DataMember]
		public string SelectionBackColor { get; set; }

		/// <summary>
		/// Gets or sets the fore color.
		/// </summary>
		[DataMember]
		public string ForeColor { get; set; }

		/// <summary>
		/// Gets or sets the selection fore color.
		/// </summary>
		[DataMember]
		public string SelectionForeColor { get; set; }
	}
}
