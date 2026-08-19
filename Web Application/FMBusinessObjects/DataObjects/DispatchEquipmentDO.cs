// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatchEquipmentDO.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Data object for returing equipment information for Dispatch client.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	using System.Collections.Generic;
	using System.Runtime.Serialization;

	/// <summary>
	/// Data object for communicating equipment records to dispatch client.
	/// </summary>
	[DataContract]
	public class DispatchEquipmentDO
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DispatchEquipmentDO"/> class.
		/// </summary>
		public DispatchEquipmentDO()
		{
			this.Equipment = new List<DispatchEquipmentDisplayDO>();
			this.Refreshed = false;
		}

		/// <summary>
		/// Gets or sets Transactions.
		/// </summary>
		[DataMember]
		public List<DispatchEquipmentDisplayDO> Equipment { get; set; }

		/// <summary>
		/// Gets or sets TopVersion.
		/// </summary>
		[DataMember]
		public string TopVersion { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether equipment have been refreshed.
		/// </summary>
		[DataMember]
		public bool Refreshed { get; set; }
	}
}