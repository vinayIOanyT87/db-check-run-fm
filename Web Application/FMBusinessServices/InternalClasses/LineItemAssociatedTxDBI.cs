// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LineItemAssociatedTxDBI.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// Writes records to associate transactions
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
    /// Associates a TransactionLink with other information from the transaction record structure needed 
    /// when saving associated transactions
    /// </summary>
    public class TransactionLinkWithTransactionAndLineItemInformation
    {
        /// <summary>
        /// Identifies the transaction which owns the link
        /// </summary>
        public Guid TransactionGuid;

        /// <summary>
        /// The trans ID of the transaction which owns the link
        /// </summary>
        public string TransID;

        /// <summary>
        /// Identifies the line item which owns the link
        /// </summary>
        public Guid TransactionLineItemGuid;

        /// <summary>
        /// The transaction link object
        /// </summary>
        public AssociatedTxDO TransactionLink;
    }

    /// <summary>
    /// Writes records to associate transactions
    /// </summary>
    public class LineItemAssociatedTxDBI
    {
        /// <summary>
        /// Allows access to the database
        /// </summary>
        internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

        /// <summary>
        /// The user who inserted or modified the transaction
        /// </summary>
        private string User { get; set; }

        public LineItemAssociatedTxDBI(string user)
        {
            this.User = user;
        }

        /// <summary>
        /// Saves new associated transactions and deletes removed associated transactions
        /// </summary>
        /// <param name="transactionLinksWithTransactionAndLineItemInformation">
        /// The line item transactions will be associated with
        /// </param>
        /// <param name="oldTransactions">
        /// Used to determine which links were deleted
        /// </param>
        /// <param name="newTransactions">Used to determine which transactions were deleted and to provide supporting information for deletes</param>
        /// <param name="security">
        /// Contains security credentials
        /// </param>
        public void Save(List<TransactionLinkWithTransactionAndLineItemInformation> transactionLinksWithTransactionAndLineItemInformation, Dictionary<Guid, TransactionPreviousVersionInformation> oldTransactions, List<TransactionDO> newTransactions, SecurityClass security)
        {
            var toAdd = new List<TransactionLinkWithTransactionAndLineItemInformation>();
            var toRemove = new List<TransactionLinkWithTransactionAndLineItemInformation>();

            // Go through all of the associated transactions contained on the previous version of transactions
            // If the associated transaction isn't found in the current version of the transaction, then it should be deleted.
            foreach (KeyValuePair<Guid, TransactionPreviousVersionInformation> oldTransaction in oldTransactions)
            {
                if (oldTransaction.Value != null && oldTransaction.Value.AssociatedTransactions != null)
                {
                    foreach (AssociatedTxDO oldAssociatedTransaction in oldTransaction.Value.AssociatedTransactions)
                    {
                        // Search the new line item for a matching associated transaction.                     
                        if (transactionLinksWithTransactionAndLineItemInformation.FindIndex(
                                newAssociatedTransaction =>
                                newAssociatedTransaction.TransactionLink.Associated == 1
                                && newAssociatedTransaction.TransactionLink.TransactionLineItemGuid == oldAssociatedTransaction.TransactionLineItemGuid
                                && newAssociatedTransaction.TransactionLineItemGuid == oldAssociatedTransaction.LinkedTransactionLineItemGuid) < 0)
                        {
                            TransactionDO originalTransaction = newTransactions.Find(newTransaction => newTransaction.TransactionGuid == oldTransaction.Key);

                            if (originalTransaction != null && !originalTransaction.DeleteFlag)
                            {
                                toRemove.Add(new TransactionLinkWithTransactionAndLineItemInformation
                                        {
                                            TransactionGuid = oldTransaction.Key,
                                            TransID = originalTransaction.TransID,
                                            TransactionLineItemGuid = oldAssociatedTransaction.LinkedTransactionLineItemGuid,
                                            TransactionLink = oldAssociatedTransaction
                                        });
                            }
                        }
                    }
                }
            }

            // Delete any transaction links belonging to deleted transactions
            foreach (TransactionDO transaction in newTransactions)
            {
                if (transaction.DeleteFlag)
                {
                    toRemove.Add(new TransactionLinkWithTransactionAndLineItemInformation
                    {
                        TransactionGuid = Guid.Empty,
                        TransID = string.Empty,
                        TransactionLineItemGuid = Guid.Empty,
                        TransactionLink = new AssociatedTxDO { TransID = transaction.TransID }
                    });
                }
            }

            foreach (TransactionLinkWithTransactionAndLineItemInformation transactionLinkWithTransactionAndLineItemInformation in transactionLinksWithTransactionAndLineItemInformation)
            {
                if (transactionLinkWithTransactionAndLineItemInformation.TransactionLink.Associated == 1)
                {
                    TransactionPreviousVersionInformation oldVersionOfTransaction = null;
                    oldTransactions.TryGetValue(transactionLinkWithTransactionAndLineItemInformation.TransactionGuid, out oldVersionOfTransaction);

                    if (oldVersionOfTransaction != null && oldVersionOfTransaction.AssociatedTransactions != null)
                    {
                        // Search the old line item for a matching associated transaction.                    
                        if (oldVersionOfTransaction.AssociatedTransactions.Find(
                                oldAssociatedTransaction =>
                                transactionLinkWithTransactionAndLineItemInformation.TransactionLink.TransactionLineItemGuid == oldAssociatedTransaction.TransactionLineItemGuid
                                && transactionLinkWithTransactionAndLineItemInformation.TransactionLineItemGuid == oldAssociatedTransaction.LinkedTransactionLineItemGuid) == null)
                        {
                            toAdd.Add(transactionLinkWithTransactionAndLineItemInformation);
                        }
                    }
                    else // If the old line item had no associated transactions, then all of the ones belonging to this line item are new
                    {
                        toAdd.Add(transactionLinkWithTransactionAndLineItemInformation);
                    }
                }
            }

            // Insert newly associated transactions
            if (toAdd.Count > 0)
            {
                this.Insert(toAdd, security);
            }

            // Delete removed associations
            if (toRemove.Count > 0)
            {             
                this.Delete(security, toRemove);
            }                  
        }

        /// <summary>
        /// Check the TransactionLink table for records indicating that the line item is linked to via LinkedTransactionLineItemGuid.
        /// This is used as a pre-check to screen out line items that aren't worth processing before running the hierarchy util's UpdateAggregatedParents method.
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="lineItems">The line items to check for parent associations</param>
        /// <returns>LineItem guids and the delete flag for records which have parent associations</returns>
        public Dictionary<Guid, bool> GetLineItemsWithParentAssociations(SecurityClass security, List<LineItemWithTransactionInformation> lineItems)
        {
            var lineItemsWithParentAssociations = new Dictionary<Guid, bool>();

            if (lineItems.Count == 0)
            {
                return lineItemsWithParentAssociations;
            }

            using (var checkParentAsssociationsCommand = new SqlCommand())
            {
                checkParentAsssociationsCommand.CommandType = CommandType.StoredProcedure;
                checkParentAsssociationsCommand.CommandText = "usp_TransactionLinksGetLineItemsWithParentAssociations";

                SqlParameter tableValuedParameter = checkParentAsssociationsCommand.Parameters.Add("@LineItemGuids", SqlDbType.Structured);
                tableValuedParameter.Value = CreateSqlDataRecordsForParentAssociationCheck(lineItems);
                tableValuedParameter.TypeName = "dbo.GuidListType";

                DataSet dataSet = this.ConsolidatedDA.GetDataSet(checkParentAsssociationsCommand, security);

                if (dataSet == null || dataSet.Tables.Count <= 0 || dataSet.Tables[0].Rows == null)
                {
                    return lineItemsWithParentAssociations;
                }

                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    Guid lineItemGuid = DataObject.getGuid(row["Guid"]);
                    LineItemWithTransactionInformation lineItemWithTransactionInformation = lineItems.Find(matchingLineItem => matchingLineItem.LineItem.TransactionLineItemGuid == lineItemGuid);

                    if (lineItemWithTransactionInformation.LineItem.TransactionLineItemGuid != Guid.Empty && !lineItemsWithParentAssociations.ContainsKey(lineItemWithTransactionInformation.LineItem.TransactionLineItemGuid))
                    {
                        lineItemsWithParentAssociations.Add(lineItemWithTransactionInformation.LineItem.TransactionLineItemGuid, lineItemWithTransactionInformation.DeleteFlag);
                    }
                }
            }

            return lineItemsWithParentAssociations;
        }

        /// <summary>
        /// Stores newly added associations to the database
        /// </summary>
        /// <param name="toAdd">
        /// A list of transaction links to add to the database
        /// </param>
        /// <param name="security">
        /// Contains security credentials
        /// </param>
        private void Insert(IEnumerable<TransactionLinkWithTransactionAndLineItemInformation> toAdd, SecurityClass security)
        {
            using (var insertCommand = new SqlCommand())
            {
                insertCommand.CommandType = CommandType.StoredProcedure;
                insertCommand.CommandText = "usp_TransactionLinksInsertList";

                SqlParameter tableValuedParameter = insertCommand.Parameters.Add("@TransactionLinks", SqlDbType.Structured);
                tableValuedParameter.Value = CreateSqlDataRecordsForInsert(toAdd, this.User, security.SiteGuid);
                tableValuedParameter.TypeName = "dbo.TransactionLinksType";

                this.ConsolidatedDA.ExecuteQuery(security, insertCommand);
            }
        }

        /// <summary>
        /// Removes a list of associations from the database
        /// </summary>
        /// <param name="security">
        /// Contains security credentials
        /// </param>
        /// <param name="toRemove">
        /// The associations to remove
        /// </param>
        private void Delete(SecurityClass security, IEnumerable<TransactionLinkWithTransactionAndLineItemInformation> toRemove)
        {
            using (var deleteCommand = new SqlCommand())
            {
                deleteCommand.CommandType = CommandType.StoredProcedure;
                deleteCommand.CommandText = "usp_TransactionLinksDeleteList";

                SqlParameter tableValuedParameter = deleteCommand.Parameters.Add("@TransactionLinks", SqlDbType.Structured);
                tableValuedParameter.Value = CreateSqlDataRecordsForDelete(toRemove);
                tableValuedParameter.TypeName = "dbo.TransactionLinksDeleteType";

                this.ConsolidatedDA.ExecuteQuery(security, deleteCommand);
            }
        }

        /// <summary>
        /// Create the SqlDataRecords corresponding to the table valued parameter we pass to the insert stored procedure
        /// </summary>
        /// <param name="transactionLinksWithTransactionAndLineItem">The transaction link records and transaction information to create SqlDataRecords for</param>
        /// <param name="user">The user saving the transaction link records</param>
        /// <param name="siteGuid">The site the records belong to</param>
        /// <returns>SqlDataRecords corresponding to the table valued parameter we pass to the insert stored procedure</returns>
        private static IEnumerable<SqlDataRecord> CreateSqlDataRecordsForInsert(IEnumerable<TransactionLinkWithTransactionAndLineItemInformation> transactionLinksWithTransactionAndLineItem, string user, Guid siteGuid)
        {
            var metaData = new SqlMetaData[7];
            int i = 0;

            metaData[i++] = new SqlMetaData("SiteGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("OriginalTransID", SqlDbType.NVarChar, 64);
            metaData[i++] = new SqlMetaData("LinkedTransID", SqlDbType.NVarChar, 64);
            metaData[i++] = new SqlMetaData("Level", SqlDbType.Int);
            metaData[i++] = new SqlMetaData("TransactionLineItemGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("LinkedTransactionLineItemGuid", SqlDbType.UniqueIdentifier);
            metaData[i] = new SqlMetaData("CreatedUpdatedBy", SqlDbType.NVarChar, 100);

            var record = new SqlDataRecord(metaData);

            foreach (TransactionLinkWithTransactionAndLineItemInformation transactionLinkWithTransactionAndLineItemInformation in transactionLinksWithTransactionAndLineItem)
            {
                int j = 0;

                record.SetGuid(j++, siteGuid);
                record.SetString(j++, transactionLinkWithTransactionAndLineItemInformation.TransID);
                record.SetString(j++, transactionLinkWithTransactionAndLineItemInformation.TransactionLink.TransID);
                record.SetInt32(j++, 0); // 0 = LineItem, 1 = Header. We never seem to use 1 (Header)
                record.SetGuid(j++, transactionLinkWithTransactionAndLineItemInformation.TransactionLineItemGuid);
                record.SetGuid(j++, transactionLinkWithTransactionAndLineItemInformation.TransactionLink.TransactionLineItemGuid);
                record.SetString(j, user);

                yield return record;
            }
        }

        /// <summary>
        /// Create the SqlDataRecords corresponding to the table valued parameter we pass to the delete stored procedure
        /// </summary>
        /// <param name="transactionLinksWithTransactionAndLineItem">The transaction link records and transaction information to create SqlDataRecords for</param>
        /// <returns>SqlDataRecords corresponding to the table valued parameter we pass to the delete stored procedure</returns>
        private static IEnumerable<SqlDataRecord> CreateSqlDataRecordsForDelete(IEnumerable<TransactionLinkWithTransactionAndLineItemInformation> transactionLinksWithTransactionAndLineItem)
        {
            var metaData = new SqlMetaData[4];
            int i = 0;

            metaData[i++] = new SqlMetaData("OriginalTransID", SqlDbType.NVarChar, 64);
            metaData[i++] = new SqlMetaData("LinkedTransID", SqlDbType.NVarChar, 64);
            metaData[i++] = new SqlMetaData("TransactionLineItemGuid", SqlDbType.UniqueIdentifier);
            metaData[i] = new SqlMetaData("LinkedTransactionLineItemGuid", SqlDbType.UniqueIdentifier);

            var record = new SqlDataRecord(metaData);

            foreach (TransactionLinkWithTransactionAndLineItemInformation transactionLinkWithTransactionAndLineItemInformation in transactionLinksWithTransactionAndLineItem)
            {
                int j = 0;

                record.SetNullableString(j++, transactionLinkWithTransactionAndLineItemInformation.TransID);
                record.SetString(j++, transactionLinkWithTransactionAndLineItemInformation.TransactionLink.TransID);
                record.SetNullableGuid(j++, transactionLinkWithTransactionAndLineItemInformation.TransactionLineItemGuid);
                record.SetNullableGuid(j, transactionLinkWithTransactionAndLineItemInformation.TransactionLink.TransactionLineItemGuid);

                yield return record;
            }
        }

        /// <summary>
        /// Create the SqlDataRecords corresponding to the table valued parameter we pass to the delete stored procedure
        /// </summary>
        /// <param name="lineItems">The transaction link records and transaction information to create SqlDataRecords for</param>
        /// <returns>SqlDataRecords corresponding to the table valued parameter we pass to the insert stored procedure</returns>
        private static IEnumerable<SqlDataRecord> CreateSqlDataRecordsForParentAssociationCheck(IEnumerable<LineItemWithTransactionInformation> lineItems)
        {
            var metaData = new SqlMetaData[1];

            metaData[0] = new SqlMetaData("Guid", SqlDbType.UniqueIdentifier);

            var record = new SqlDataRecord(metaData);

            foreach (LineItemWithTransactionInformation lineItem in lineItems)
            {
                record.SetGuid(0, lineItem.LineItem.TransactionLineItemGuid);

                yield return record;
            }
        }
    }
}