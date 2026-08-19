// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IControllerLogs.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the BusinessInterface for IControllerLogs.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ServiceModel;

using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.BusinessInterfaces
{
	[ServiceContract]
	public interface IControllerLogs
	{
		/// <summary>
		/// This methods Adds a ControllerLog to the database.
		/// </summary>
		/// <param name="security">A SecurityClass instance.</param>
		/// <param name="controllerLog">A ControllerLogClass instance</param>
		/// <returns>A Guid</returns>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, ControllerLogClass controllerLog);

		/// <summary>
		/// This methods adds ControllerLog and ControllersLogToTransaction records to the database.
		/// </summary>
		/// <param name="security">A SecurityClass instance.</param>
		/// <param name="controllerLog">A ControllerLogClass instance</param>
		/// <param name="transactionGuid">A Transaction Id</param>
		/// <returns>A list containing the ControllerLog Guid and Transaction Guid</returns>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		List<Guid> AddControllerLogAndMapRecord(SecurityClass security, ControllerLogClass currentControllerLog, Guid transId);

		/// <summary>
		/// This method deletes a ControllerLog from the database.
		/// </summary>
		/// <param name="security">A SecurityClass instance</param>
		/// <param name="controllerLogGuid">A Guid</param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void DeleteControllerLog(SecurityClass security, Guid controllerLogGuid);

		/// <summary>
		/// This method enumerates the ControllerLogs by their identity key.
		/// </summary>
		/// <param name="security">A SecurityClass instance</param>
		/// <param name="controllerLogGuid">A Guid</param>
		/// <returns>A ControllerLogClass instance</returns>
		[OperationContract]
		ControllerLogClass EnumerateControllerLogByIdentityGuid(SecurityClass security, Guid controllerLogGuid);

		/// <summary>
		/// This method enumerates the ControllerLogs by a start date and stop date
		/// </summary>
		/// <param name="security">A SecurityClass instance</param>
		/// <param name="StartDate">The start date the controller log was entered</param>
		/// <param name="EndDate">The end date the controller log was entered</param>
		/// <param name="Deleted">A bool to indicate whether or not to retrieve logically deleted Controller Log records.</param>
		/// <returns>A List of ControllerLogClass instances</returns>
		[OperationContract]
		List<ControllerLogClass> EnumerateByStartStopDateAndDeleted(SecurityClass security, DateTimeOffset StartDate, DateTimeOffset EndDate, bool Deleted);

		/// <summary>
		/// This method enumerates the ControllerLogs by a start date and stop date
		/// </summary>s
		/// <param name="security">A SecurityClass instance</param>
		/// <param name="StartDate">The start date the controller log was entered</param>
		/// <param name="EndDate">The end date the controller log was entered</param>
		/// <param name="Deleted">A bool to indicate whether or not to retrieve logically deleted Controller Log records.</param>
		/// <param name="transactionGuid">A Transaction Guid</param>
		/// <returns>A List of ControllerLogClass instances</returns>
		[OperationContract]
		List<ControllerLogClass> EnumerateByStartStopTimeAndTransId(SecurityClass security, DateTimeOffset StartDate, DateTimeOffset EndDate, bool Deleted, Guid transactionGuid);

		/// <summary>
		/// This method enumerates the ControllerLogs by a start date and stop date and is
		/// called by old Dispatch and is here for backward compatiblity.
		/// </summary>
		/// <param name="security">A SecurityClass instance</param>
		/// <param name="StartDate">The start date the controller log was entered</param>
		/// <param name="EndDate">The end date the controller log was entered</param>
		/// <param name="Deleted">A bool to indicate whether or not to retrieve logically deleted Controller Log records.</param>
		/// <param name="transactionGuid">A Transaction Guid</param>
		/// <returns>A List of ControllerLogClass instances</returns>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		List<ControllerLogClass> EnumerateByStartStopTime(SecurityClass security, DateTime startDate, DateTime endDate, bool deleted);

		/// <summary>
		/// This method modifies an existing Controller Log instance in the database.
		/// </summary>
		/// <param name="security">A SecurityClass instance</param>
		/// <param name="controllerLog">A ControllerLogClass instance</param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, ControllerLogClass controllerLog);

		/// <summary>
		/// This is a special case purge method that instead of deleting the Controller Log instances
		/// it flips the logical delete flag inversely.
		/// </summary>
		/// <param name="security">A SecurityClass instance</param>
		/// <param name="controllerLogGuid">A Guid</param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid controllerLogGuid);

		/// <summary>
		/// This method performs the undelete functionality for a Controller Log instance
		/// </summary>
		/// <param name="security">A SecurityClass instance</param>
		/// <param name="controllerLogGuid">A Guid</param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void UnDeleteControllerLog(SecurityClass security, Guid controllerLogGuid);
		
	}
}
