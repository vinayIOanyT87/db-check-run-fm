// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITransactionProcessor.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ITransactionProcessor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;

	/// <summary>
	/// Interface for service class used to get transaction data.
	/// </summary>
	[ServiceContract]
	public interface ITransactionProcessor
	{
		#region Public Methods and Operators

		/// <summary>
		/// Processes the specified sr to retrieve a transaction.
		/// </summary>
		/// <param name="sr">The sr.</param>
		/// <returns>A transaction data object.</returns>
		[OperationContract]
		TransactionDO Process(TransactionSR sr);

		#endregion
	}
}