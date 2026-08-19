// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatchPersonnelDO.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Data object for returing Personnel information for Dispatch client.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	using System.Collections.Generic;
	using System.Runtime.Serialization;

	/// <summary>
	/// Data object for communicating Personnel records to dispatch client.
	/// </summary>
	[DataContract]
	public class DispatchPersonnelDO
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DispatchPersonnelDO"/> class.
		/// </summary>
		public DispatchPersonnelDO()
		{
			this.Personnel = new List<DispatchPersonnelDisplayDO>();
			this.Refreshed = false;
		}

		/// <summary>
		/// Gets or sets Transactions.
		/// </summary>
		[DataMember]
		public List<DispatchPersonnelDisplayDO> Personnel { get; set; }

		/// <summary>
		/// Gets or sets TopVersion.
		/// </summary>
		[DataMember]
		public string TopVersion { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether Personnel have been refreshed.
		/// </summary>
		[DataMember]
		public bool Refreshed { get; set; }
	}
}