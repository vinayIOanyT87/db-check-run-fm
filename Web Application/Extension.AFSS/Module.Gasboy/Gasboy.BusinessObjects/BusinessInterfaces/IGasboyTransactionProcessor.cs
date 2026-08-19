// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGasboyTransactionProcessor.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Describes operations that can be performed by the External Station Service class
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.Afss.Module.Gasboy.BusinessObjects.BusinessInterfaces
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;

    using FMBusinessObjects.DataObjects;

    using FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects;

    /// <summary>
    /// Describes operations to support database operations for External Stations
    /// like adding, modifying, or deleting a record.
    /// </summary>
    [ServiceContract]
    public interface IGasboyTransactionProcessor
    {
        /// <summary>
        /// Attempt to process new transactions that have been received from a Gasboy Station
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="station">The station that originated the transactions</param>
        /// <param name="gasboyTransactions">The transactions to process</param>
        /// <returns>Any errors that may have been encountered when attempting to reprocess the transactions</returns>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        string ImportTransactions(
            SecurityClass security,
            GasboyStation station,
            List<GasboyStationTransaction> gasboyTransactions);

        /// <summary>
        /// Attempt to reprocess a transaction that has failed after being reviewed and possibly edited by a user
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="failedTransaction">The failed transaction to reprocess</param>
        /// <returns>Any errors that may have been encountered when attempting to reprocess the transaction</returns>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        string ProcessCorrectedTransaction(SecurityClass security, GasboyStationTransaction failedTransaction);

        /// <summary>
        /// Add transactions that have failed to be converted to a real transaction to the database
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="transactions">The transactions to add to the database</param>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void AddTransactions(SecurityClass security, List<GasboyStationTransaction> transactions);

        /// <summary>
        /// Updates one or more failed transactions with their new failed status
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="failedTransactions"></param>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void UpdateTransactionFailedStatuses(SecurityClass security, List<GasboyStationTransaction> failedTransactions);

        /// <summary>Retrieve a failed transaction from the database</summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="externalStationFailedTransactionGuid">Identifies the failed transaction to retrieve</param>
        /// <returns>The <see cref="GasboyStationTransaction"/> identified by the provided guid.</returns>
        [OperationContract]
        GasboyStationTransaction GetFailedTransaction(SecurityClass security, Guid externalStationFailedTransactionGuid);

        /// <summary>Get all failed transactions for a specific site from the database</summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="externalStationGuid">If not empty, identifies the external station to retrieve failed transactions for</param>
        /// <param name="beginDate">The beginning receive date of failed transactions to search for</param>
        /// <param name="endDate">The ending receive date of failed transactions to search for</param>
        /// <param name="transactionID">The external transaction ID to search for</param>
        /// <returns>All failed transactions for a specific site</returns>
        [OperationContract]
        List<GasboyStationTransaction> EnumerateFailedTransactions(SecurityClass security, Guid externalStationGuid, DateTimeOffset beginDate, DateTimeOffset endDate, string transactionID);
    }
}
