// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDispatchTransactionsProcessor.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the IDispatchTransactionsProcessor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;

	/// <summary>
	/// Service interface for enuemrating transactions for use in Dispatch
	/// </summary>
	[ServiceContract]
	public interface IDispatchTransactionsProcessor
	{
		#region Public Methods and Operators

		/// <summary>
		/// Processes the specified dispatch service request.
		/// </summary>
		/// <param name="sr">The service request object.</param>
		/// <returns>A DispatchTransactionDO object containing the requested transactions.</returns>
		[OperationContract]
		DispatchTransactionsDO Process(DispatchTransactionsSR sr);

		/// <summary>
		/// Gets the specified line items.
		/// </summary>
		/// <param name="sr">The service request object</param>
		/// <returns>A DispatchTransactionDO object containing the requested transactions.</returns>
		[OperationContract]
		DispatchTransactionsDO GetLineItems(DispatchTransactionsSR sr);

		#endregion
	}
}