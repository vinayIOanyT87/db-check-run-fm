// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISaveTransactionsProcessor.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System.ServiceModel;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.ServiceRequests;

	[ServiceContract]
	public interface ISaveTransactionsProcessor
	{
		#region Public Methods and Operators

		[OperationContract]
		[FaultContract(typeof(SaveTransactionsException))]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		SaveTransactionsResultDO SaveTransactions(SaveTransactionsSR sr);

		/// <summary>
		/// Saves the transmitted transactions.
		/// </summary>
		/// <param name="serviceRequestDataObject">The service request data object.</param>
		/// <param name="securityObject">The security.</param>
		/// <returns>A transmit transaction list result object.</returns>
		[OperationContract]
		[FaultContract(typeof(SaveTransactionsException))]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		SaveTransmitTranListResultDO SaveTransmittedTransactions(TransmitTranListDO serviceRequestDataObject, SecurityClass securityObject);

		#endregion
	}
}
