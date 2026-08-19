// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDispatchRequests.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the IDispatchRequests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Interface for Dispatch service requests. Primary interface for Dispatch.
	/// </summary>
	[ServiceContract]
	public interface IDispatchRequests
	{
		#region Public Methods and Operators

		/// <summary>
		/// Enumerates equipment entities for use in Dispatch.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="topVersion">The top Version</param>
		/// <returns>A dispatch equipment data object</returns>
		[OperationContract]
		DispatchEquipmentDO EnumerateEquipment(SecurityClass security, string topVersion);

		/// <summary>
		/// Enumerates personnel entities for use in Dispatch.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="topVersion">The top Version</param>
		/// <returns>A dispatch personnel data object</returns>
		[OperationContract]
		DispatchPersonnelDO EnumeratePersonnel(SecurityClass security, string topVersion);

		/// <summary>
		/// Enumerates standby personnel for use in Dispatch.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <returns>A list of dispatch personnel display data objects</returns>
		[OperationContract]
		List<DispatchPersonnelDisplayDO> EnumerateStandbyPersonnel(SecurityClass security);

		/// <summary>
		/// Enumerates the transactions.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="topVersion">The top version.</param>
		/// <param name="beginDate">The begin date.</param>
		/// <param name="endDate">The end date.</param>
		/// <param name="status">The status.</param>
		/// <param name="requestName">Name of the request.</param>
		/// <returns>A collection of transaction objects.</returns>
		[OperationContract]
		DispatchTransactionDO EnumerateTransactions(SecurityClass security, string topVersion, string beginDate, string endDate, string status, string requestName);

		/// <summary>
		/// Gets the dictionary translation.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="key">The key to translate.</param>
		/// <returns>A translated string value.</returns>
		[OperationContract]
		string GetDictionaryTranslation(SecurityClass security, string key);

		/// <summary>
		/// Sets status to Arrived for a set of transactions given an array of transaction Ids.
		/// Only transactions with statuses of Dispatched will be processed.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="transactionIds">The array of transaction Ids</param>
		/// <returns>The number of transactions statuses set to Arrived</returns>
		[OperationContract]
		int SetArrived(SecurityClass security, string[] transactionIds);

		/// <summary>
		/// Sets status to Started for a set of transactions given an array of transaction Ids.
		/// Only transactions with statuses of Arrived will be processed.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="transactionIds">The array of transaction Ids</param>
		/// <returns>The number of transactions statuses set to Started</returns>
		[OperationContract]
		int SetServiceStarted(SecurityClass security, string[] transactionIds);

		/// <summary>
		/// Sets status to Stopped for a set of transactions given an array of transaction Ids.
		/// Only transactions with statuses of Started will be processed.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="transactionIds">The array of transaction Ids</param>
		/// <returns>The number of transactions statuses set to Stopped</returns>
		[OperationContract]
		int SetServiceStopped(SecurityClass security, string[] transactionIds);

		/// <summary>
		/// This interface will retrieve the optional times configuration information
		/// that is saved by the web dispatch optional times page.
		/// </summary>
		/// <param name="security">he security object</param>
		/// <returns>Returns optional times configuration.</returns>
		[OperationContract]
		string RetrieveOptionalTimes(SecurityClass security);

		/// <summary>
		/// Verify that the specified lock out date is not after the current date, i.e. not a future date.
		/// Verify that the specified lock out date is not before the current lock out date.
		/// Verify that all transactions prior to the specified lock out date, with application orgin of 
		/// Dispatch, and submitted to accounting flag of false have a status of either Completed or Cancelled.
		/// Consider only those transactions with refuel and defuel alias names if no transaction alias exists.
		/// Otherwise consider only those transactions where transaction alias IncludeInDispatch flag is set
		/// and the transaction type is a refuel or defuel transaction type.
		/// Set SubmittedToAccounting flag to true for applicable transactions (ones that satisfy above conditions.)
		/// Set the OperationalLockDate in the Site table to the newly specified lock out date.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="lockOutDate">The lock out date</param>
		/// <returns>The result status of the operation</returns>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Dictionary<string, string> ReleaseToAccounting(SecurityClass security, DateTimeOffset lockOutDate);
		#endregion
	}
}
