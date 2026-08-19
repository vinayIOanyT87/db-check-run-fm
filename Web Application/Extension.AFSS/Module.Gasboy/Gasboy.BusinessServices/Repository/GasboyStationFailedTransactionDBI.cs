// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GasboyStationTransactionDBI.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//  Implements methods for the Gasboy Station Failed transaction functionality that interact with the database 
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.GasboyRICE.BusinessServices.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    using FMBusinessObjects.DataObjects;

    using FMBusinessServices.DataAccessLayer;

    using FuelsManager.GasboyRICE.BusinessObjects.DataObjects;

    using Microsoft.SqlServer.Server;

    /// <summary>
    /// Implements methods for the Gasboy Station Failed transaction functionality that interact with the database 
    /// </summary>
    public class GasboyStationTransactionDBI
    {
        /// <summary>
        /// Provides database access
        /// </summary>
        private static readonly ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

        /// <summary>
        /// Get a failed transaction record identified by the provided guid from the database
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="externalStationFailedTransactionGuid">Identifies the failed transaction record to retrieve</param>
        /// <returns>The failed transaction record identified by the provided guid from the database, or null if it was not found</returns>
        public static GasboyStationTransaction Get(SecurityClass security, Guid externalStationFailedTransactionGuid)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_ExternalStationFailedTransactionGet";

                cmd.Parameters.Add("@ExternalStationFailedTransactionGuid", SqlDbType.UniqueIdentifier).Value = externalStationFailedTransactionGuid;

                DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

                if (set.Tables.Count == 0 || set.Tables[0].Rows.Count == 0)
                {
                    return null;
                }

                return LoadObjectFromDataRow(set.Tables[0].Rows[0]);
            }
        }

        /// <summary>
        /// Search the database for failed transaction records identified by the provided parameters
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="siteGuid">The site to retrieve failed transaction records for</param>
        /// <param name="externalStationGuid">The station to get failed transaction for, or Guid.Empty for all stations</param>
        /// <param name="beginDate">The beginning date to get failed transaction for</param>
        /// <param name="endDate">The end date to get failed transaction for</param>
        /// <param name="transactionID">If specified, failed transactions with this id or beginning with this id will be returned</param>
        /// <returns>External station failed transactions for the site matching the provided search parameters</returns>
        public static List<GasboyStationTransaction> GetList(
            SecurityClass security,
            Guid siteGuid,
            Guid externalStationGuid, 
            DateTimeOffset beginDate, 
            DateTimeOffset endDate, 
            string transactionID)
        {
            List<GasboyStationTransaction> failedTransactions = new List<GasboyStationTransaction>();

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_ExternalStationFailedTransactionEnumerate";

                cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = siteGuid;

                if (externalStationGuid != Guid.Empty)
                {
                    cmd.Parameters.Add("@ExternalStationGuid", SqlDbType.UniqueIdentifier).Value = externalStationGuid;
                }

                cmd.Parameters.Add("@BeginDate", SqlDbType.DateTimeOffset).Value = beginDate;
                cmd.Parameters.Add("@EndDate", SqlDbType.DateTimeOffset).Value = endDate;

                if (!string.IsNullOrEmpty(transactionID))
                {
                    cmd.Parameters.Add("@TransactionID", SqlDbType.NVarChar, 20).Value = transactionID;
                }

                DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

                if (set.Tables.Count == 0 || set.Tables[0].Rows.Count == 0)
                {
                    return failedTransactions;
                }

                foreach (DataRow row in set.Tables[0].Rows)
                {
                    GasboyStationTransaction failedTransaction = new GasboyStationTransaction
                    {
                        IdentityGuid = DataObject.getValue(row["ExternalStationFailedTransactionGuid"], Guid.Empty),
                        ExternalStationGuid = DataObject.getValue(row["ExternalStationGuid"], Guid.Empty),
                        ExternalStationID = DataObject.getValue(row["ExternalStationID"], string.Empty),
                        SiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty),
                        ID = DataObject.getValue(row["StationTransactionID"], string.Empty),
                        CreatedDate = DataObject.getValue(row["CreatedDate"], DateTimeOffset.Now)
                    };

                    failedTransactions.Add(failedTransaction);
                }
            }

            return failedTransactions;
        }

        /// <summary>
        /// Add failed transactions to the database
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="transactions">Failed transactions to save to the database</param>
        public static void Insert(SecurityClass security, List<GasboyStationTransaction> transactions)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_ExternalStationFailedTransactionInsert";

                SqlParameter tableValuedParameter = cmd.Parameters.Add("@ExternalStationFailedTransactions", SqlDbType.Structured);
                tableValuedParameter.Value = CreateSqlDataRecords(transactions);
                tableValuedParameter.TypeName = "dbo.ExternalStationFailedTransactionType";
                ConsolidatedDA.ExecuteQuery(security, cmd);
            }
        }

        /// <summary>
        /// Delete a failed transaction record identified by the provided guid from the database
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="externalStationFailedTransactionGuid">Identifies the failed transaction record to delete</param>
        public static void Purge(SecurityClass security, Guid externalStationFailedTransactionGuid)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_ExternalStationFailedTransactionDelete";

                cmd.Parameters.Add("@ExternalStationFailedTransactionGuid", SqlDbType.UniqueIdentifier).Value = externalStationFailedTransactionGuid;
                ConsolidatedDA.ExecuteQuery(security, cmd);
            }
        }

        /// <summary>
        /// Create the SqlDataRecords corresponding to the table valued parameter we pass to the insert stored procedure
        /// </summary>
        /// <param name="failedTransactions">The failed transactions to create SqlDataRecords for</param>
        /// <returns>SqlDataRecords corresponding to the table valued parameter we pass to the insert stored procedure</returns>
        private static IEnumerable<SqlDataRecord> CreateSqlDataRecords(IEnumerable<GasboyStationTransaction> failedTransactions)
        {
            SqlMetaData[] metaData = new SqlMetaData[6];

            int i = 0;
            metaData[i++] = new SqlMetaData("ExternalStationFailedTransactionGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("ExternalStationGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("SiteGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("StationTransactionID", SqlDbType.NVarChar, 20);
            metaData[i++] = new SqlMetaData("RawTransactionData", SqlDbType.NVarChar, -1);
            metaData[i] = new SqlMetaData("CreatedUpdatedBy", SqlDbType.NVarChar, 100);

            SqlDataRecord record = new SqlDataRecord(metaData);

            foreach (GasboyStationTransaction failedTransaction in failedTransactions)
            {
                int j = 0;

                record.SetGuid(j++, failedTransaction.IdentityGuid);
                record.SetGuid(j++, failedTransaction.ExternalStationGuid);
                record.SetGuid(j++, failedTransaction.SiteGuid);
                record.SetString(j++, failedTransaction.ID);
                record.SetString(j++, failedTransaction.RawTransactionData);
                record.SetString(j, failedTransaction.CreatedBy);

                yield return record;
            }
        }

        /// <summary>
        /// Load a gasboy failed transaction record from a dataRow read from the database
        /// </summary>
        /// <param name="row">The dataRow to read failed transaction information from</param>
        /// <returns>A populated GasboyStationTransaction object</returns>
        private static GasboyStationTransaction LoadObjectFromDataRow(DataRow row)
        {
            GasboyStationTransaction transaction = new GasboyStationTransaction();

            transaction.IdentityGuid = DataObject.getValue(row["ExternalStationFailedTransactionGuid"], Guid.Empty);
            transaction.SiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty);
            transaction.ID = DataObject.getValue(row["StationTransactionID"], string.Empty);
            transaction.ExternalStationGuid = DataObject.getValue(row["ExternalStationGuid"], Guid.Empty);
            transaction.ExternalStationID = DataObject.getValue(row["ExternalStationID"], string.Empty);
            transaction.RawTransactionData = DataObject.getValue(row["RawTransactionData"], string.Empty);
            transaction.CreatedDate = DataObject.getValue(row["CreatedDate"], DateTimeOffset.Now);
            transaction.CreatedBy = DataObject.getValue(row["CreatedBy"], BaseDataObject.ADMIN);
            transaction.UpdatedDate = DataObject.getValue(row["UpdatedDate"], DateTimeOffset.Now);
            transaction.UpdatedBy = DataObject.getValue(row["UpdatedBy"], BaseDataObject.ADMIN);

            return transaction;
        }
    }
}