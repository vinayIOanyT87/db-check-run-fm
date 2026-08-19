// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatchPersonnelDisplayDO.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Transport class for display information for dispatch Personnel.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	using System.Runtime.Serialization;

	/// <summary>
	/// Transport class for display information for dispatch Personnel.
	/// </summary>
	[DataContract]
	public class DispatchPersonnelDisplayDO
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DispatchPersonnelDisplayDO"/> class.
		/// </summary>
		/// <param name="person">The person to use for initialization.</param>
		public DispatchPersonnelDisplayDO( PersonClass person )
		{
			this.IdentityGuid = person.MasterRecordGuid.ToString();
			this.Last = person.LastName;
			this.First = person.FirstName;
			this.Status = person.StatusText;
			this.Equipment = person.AssignedEquipmentID;
			this.EquipmentGuid = person.AssignedEquipmentGuid.ToString();
			this.FullName = person.FullName;
			this.LockedOut = person.LockedOut;
		}

		/// <summary>
		/// Gets or sets the identity GUID.
		/// </summary>
		[DataMember]
		public string IdentityGuid { get; set; }

		/// <summary>
		/// Gets or sets the last name.
		/// </summary>
		[DataMember]
		public string Last { get; set; }

		/// <summary>
		/// Gets or sets the first name.
		/// </summary>
		[DataMember]
		public string First { get; set; }

		/// <summary>
		/// Gets or sets the status.
		/// </summary>
		[DataMember]
		public string Status { get; set; }

		/// <summary>
		/// Gets or sets the assigned equipment.
		/// </summary>
		[DataMember]
		public string Equipment { get; set; }

		/// <summary>
		/// Gets or sets the assiend equipment GUID.
		/// </summary>
		/// <value>
		/// The equipment GUID.
		/// </value>
		[DataMember]
		public string EquipmentGuid { get; set; }

		/// <summary>
		/// Gets or sets the full name.
		/// </summary>
		[DataMember]
		public string FullName { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether locked out.
		/// </summary>
		[DataMember]
		public bool LockedOut { get; set; }

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
