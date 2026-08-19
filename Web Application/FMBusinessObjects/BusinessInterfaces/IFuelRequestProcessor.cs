// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFuelRequestProcessor.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// Receives transaction data from the Fuel Request Form and creates the transaction record
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System.ServiceModel;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;

	/// <summary>
	/// Receives transaction data from the Fuel Request Form and creates the transaction record
	/// </summary>
	[ServiceContract]
	public interface IFuelRequestProcessor
	{
		/// <summary>
		/// Using data entered on the form, create or update a transaction record.
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <param name="serviceRequest">Contains the transaction and other information we need to process the data</param>
		/// <returns>A result object which contains any warnings we need to display</returns>
		[OperationContract]
		[FaultContract(typeof(SaveTransactionsException))]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		FuelRequestResult Process(SecurityClass security, FuelRequestSR serviceRequest);
	}
}
