// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatchTransactionDO.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the DispatchTransactionDO object.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	using System.Collections.Generic;
	using System.Runtime.Serialization;

	/// <summary>
	/// Data transport object for Dispatch requests
	/// </summary>
	[DataContract]
	public class DispatchTransactionDO
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DispatchTransactionDO"/> class.
		/// </summary>
		public DispatchTransactionDO()
		{
			this.Transactions = new List<DispatchTransaction>();
			this.Refreshed = false;
		}

		/// <summary>
		/// Gets or sets Transactions.
		/// </summary>
		[DataMember]
		public List<DispatchTransaction> Transactions { get; set; }

		/// <summary>
		/// Gets or sets TopVersion.
		/// </summary>
		[DataMember]
		public string TopVersion { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether transactions have been refreshed.
		/// </summary>
		[DataMember]
		public bool Refreshed { get; set; }
	}
}