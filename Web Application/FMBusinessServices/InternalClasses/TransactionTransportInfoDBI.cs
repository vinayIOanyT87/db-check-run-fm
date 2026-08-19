// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionTransportInfoDBI.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the TransactionTransportInfoDBI type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.InternalClasses
{
    using FMBusinessObjects.DataObjects;
    using FMBusinessServices.DataAccessLayer;
    using Microsoft.SqlServer.Server;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    /// <summary>
    /// Associates a transport line item with information from the transaction header. When saving a transport line item some of the
    /// header information is saved along with it.
    /// </summary>
    public class TransportLineItemWithTransactionInformation
    {
        /// <summary>
        /// The transport line item itself
        /// </summary>
        public TransportLineItemDO TransportLineItem;

        /// <summary>
        /// The transaction guid identifying the transaction header the transport line item is associated with
        /// </summary>
        public Guid TransactionGuid;

        /// <summary>
        /// The TransVersion associated with the current version of the entire transaction record
        /// </summary>
        public long TransVersion;
    }

    /// <summary>
    /// The transaction transport info DBI.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class TransactionTransportInfoDBI
    {
        /// <summary>
        /// The consolidated DA.
        /// </summary>
        internal ConsolidatedDAClass ConsolidatedDa = new ConsolidatedDAClass();

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionTransportInfoDBI"/> class.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        public TransactionTransportInfoDBI(string user)
        {
            this.User = user;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the user.
        /// The user who inserted or modified the transaction
        /// </summary>
        private string User { get; }

        #endregion

        public void Save(SecurityClass security, List<TransportLineItemWithTransactionInformation> transportLineItemsWithTransactionInformation)
        {
            if (transportLineItemsWithTransactionInformation.Count == 0)
            {
                return;
            }

            using (SqlCommand insertUpdateCommand = new SqlCommand())
            {
                insertUpdateCommand.CommandType = CommandType.StoredProcedure;
                insertUpdateCommand.CommandText = "usp_TransactionTransportLineItemsInsertUpdate";
                insertUpdateCommand.CommandTimeout = Math.Max(30, transportLineItemsWithTransactionInformation.Count);

                SqlParameter tableValuedParameter = insertUpdateCommand.Parameters.Add("@TransactionTransportLineItems", SqlDbType.Structured);
                tableValuedParameter.Value = CreateSqlDataRecords(transportLineItemsWithTransactionInformation, this.User);
                tableValuedParameter.TypeName = "dbo.TransactionTransportLineItemsType";

                this.ConsolidatedDa.ExecuteQuery(security, insertUpdateCommand);
            }
        }

        /// <summary>
        /// Create the SqlDataRecords corresponding to the table valued parameter we pass to the insert or update stored procedure
        /// </summary>
        /// <param name="transportLineItemsWithTransactionInformation">The transport line item records and transaction information to create SqlDataRecords for</param>
        /// <param name="user">The user saving the transport line item records</param>
        /// <returns>SqlDataRecords corresponding to the table valued parameter we pass to the insert or update stored procedure</returns>
        private static IEnumerable<SqlDataRecord> CreateSqlDataRecords(IEnumerable<TransportLineItemWithTransactionInformation> transportLineItemsWithTransactionInformation, string user)
        {
            SqlMetaData[] metaData = new SqlMetaData[13];
            int i = 0;

            metaData[i++] = new SqlMetaData("TransactionTransportLineItemGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("TransactionGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("TransportOrderNumber", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("TransVersion", SqlDbType.BigInt);
            metaData[i++] = new SqlMetaData("LocationName", SqlDbType.NVarChar, 30);
            metaData[i++] = new SqlMetaData("Address1", SqlDbType.NVarChar, 60);
            metaData[i++] = new SqlMetaData("Address2", SqlDbType.NVarChar, 60);
            metaData[i++] = new SqlMetaData("City", SqlDbType.NVarChar, 60);
            metaData[i++] = new SqlMetaData("State", SqlDbType.NVarChar, 20);
            metaData[i++] = new SqlMetaData("Zip", SqlDbType.NVarChar, 11);
            metaData[i++] = new SqlMetaData("POCName", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("POCPhone", SqlDbType.NVarChar, 20);
            metaData[i] = new SqlMetaData("CreatedUpdatedBy", SqlDbType.NVarChar, 100);

            SqlDataRecord record = new SqlDataRecord(metaData);

            foreach (TransportLineItemWithTransactionInformation transportLineItemWithTransactionInformation in transportLineItemsWithTransactionInformation)
            {
                int j = 0;
                record.SetNullableGuid(j++, transportLineItemWithTransactionInformation.TransportLineItem.TransactionTransportLineItemGuid);
                record.SetNullableGuid(j++, transportLineItemWithTransactionInformation.TransactionGuid);
                record.SetString(j++, transportLineItemWithTransactionInformation.TransportLineItem.TransportOrderNumber);
                record.SetInt64(j++, transportLineItemWithTransactionInformation.TransVersion);
                record.SetNullableString(j++, transportLineItemWithTransactionInformation.TransportLineItem.LocationName);
                record.SetNullableString(j++, transportLineItemWithTransactionInformation.TransportLineItem.Address1);
                record.SetNullableString(j++, transportLineItemWithTransactionInformation.TransportLineItem.Address2);
                record.SetNullableString(j++, transportLineItemWithTransactionInformation.TransportLineItem.City);
                record.SetNullableString(j++, transportLineItemWithTransactionInformation.TransportLineItem.State);
                record.SetNullableString(j++, transportLineItemWithTransactionInformation.TransportLineItem.Zip);
                record.SetNullableString(j++, transportLineItemWithTransactionInformation.TransportLineItem.POCName);
                record.SetNullableString(j++, transportLineItemWithTransactionInformation.TransportLineItem.POCPhone);
                record.SetString(j, user);

                yield return record;
            }
        }
    }
}