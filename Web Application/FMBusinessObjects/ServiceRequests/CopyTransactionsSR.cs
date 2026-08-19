// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CopyTransactionsSR.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Service request description class for CopyTransactions processor
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessObjects.ServiceRequests
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.Serialization;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Service request description class for CopyTransactions processor
	/// </summary>
	[Serializable]
	[DataContract]
	public class CopyTransactionsSR : AccountingServiceRequest
	{
		#region Constructors and Destructors
		/// <summary>
		/// Initializes a new instance of the <see cref="CopyTransactionsSR"/> class.
		/// </summary>
		public CopyTransactionsSR()
		{
			this.TransactionIds = new List<string>();
			this.DocumentTypes = new List<DOCUMENT_TYPE>();
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the transaction ids generic list.
		/// </summary>
		/// <value>
		/// The transaction ids.
		/// </value>
		[DataMember]
		public List<string> TransactionIds { get; private set; }

		/// <summary>
		/// Gets the document number type.
		/// </summary>
		[DataMember]
		public List<DOCUMENT_TYPE> DocumentTypes { get; private set; }
		#endregion
	}
}