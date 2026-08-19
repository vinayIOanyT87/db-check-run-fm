// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IControllerLogToTransactionMap.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Business interface defining methods that save ControllerLogToTransactionMap instances.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.ServiceModel;

using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.BusinessInterfaces
{
	[ServiceContract]
	public interface IControllerLogToTransactionMap
	{
		///<summary>
		/// Returns a collection of ControllersLogToTransactionMapClass instances with the matching transactionGuid.
		///</summary>
		/// <param name="security">A SecurityClass instance.</param>
		/// <param name="transactionGuid">A Transaction Guid</param>
		/// <returns>A ControllersLogToTransactionCollectionClass instance.</returns>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		ControllersLogToTransactionCollectionClass EnumerateByTransactionGuid(SecurityClass security, Guid transactionGuid);

		///<summary>
		/// Returns a matching ControllersLogToTransactionMapClass instance.
		/// </summary>
		/// <param name="security">A SecurityClass instance.</param>
		/// <param name="transactionGuid">A Transaction Guid</param>
		/// <returns>A ControllersLogToTransactionMapClass instance.</returns>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		ControllersLogToTransactionMapClass GetByTransactionGuid(SecurityClass security, Guid transactionGuid);

		///<summary>
		/// Returns a matching ControllersLogToTransactionMapClass instance.
		/// </summary>
		/// <param name="security">A SecurityClass instance.</param>
		/// <param param name="controllersLogToTransactionGuid">The key field for this object.</param>
		/// <returns>A ControllersLogToTransactionMapClass instance.</returns>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		ControllersLogToTransactionMapClass GetById(SecurityClass security, Guid controllersLogToTransactionGuid);

		/// <summary>
		/// This method will add a ControllersLogToTransactionMapClass instance to map.tblControllersLogToTransaction
		/// </summary>
		/// <param name="security">A SecurityClass instance.</param>
		/// <param name="ControllersLogToTransactionMap">A ControllersLogToTransactionMapClass instance.</param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, ControllersLogToTransactionMapClass controllersLogToTransactionMap);

		/// <summary>
		/// This method will add or insert a ControllersLogToTransactionMapClass instance to map.tblControllersLogToTransaction
		/// </summary>
		/// <param name="security">A SecurityClass instance.</param>
		/// <param name="ControllersLogToTransactionMap">A ControllersLogToTransactionMapClass instance.</param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, ControllersLogToTransactionMapClass controllersLogToTransactionMap);

		/// <summary>
		/// This method will purge a row in the map.tblControllersLogToTransaction
		/// </summary>
		/// <param name="security">A SecurityClass instance. </param>
		/// <param name="ControllersLogToTransactionMap">A ControllersLogToTransactionMapClass instance.</param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, ControllersLogToTransactionMapClass controllersLogToTransactionMap);

		/// <summary>
		/// Returns a collection of  the ControllersLogToTransactionMapClass instances that meet 
		/// the criteria.
		/// </summary>
		/// <param name="security">A SecurityClass instance</param>
		/// <param name="bInTransaction">A bool representing if this method is wrapped in a transaction</param>
		/// <param name="StartDate">A Start Date</param>
		/// <param name="EndDate">The end date the ControllerLogClass record was stored in the database</param>
		/// <param name="Deleted">A bool indicating whether or not to retrieve logically deleted records</param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		ControllersLogToTransactionCollectionClass EnumerateByStartStopDates(SecurityClass security,  
			DateTimeOffset startDate, DateTimeOffset endDate, bool deleted, Guid transactionGuid);

	}
}
