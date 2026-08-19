// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionImportProcessor.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the TransactionImportProcessorClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.ServiceModel;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.ServiceRequests;

    using DataAccessLayer;

    using Microsoft.SqlServer.Server;

    using IsolationLevel = System.Transactions.IsolationLevel;

    /// <summary>
	/// Represents information that can be retrieved for a particular Transaction
	/// when we attempt to match up legacy records with existing records based on the TransID
	/// </summary>
	public class TransactionGuidMap
	{
		/// <summary>
		/// The primary key of the transaction record.
		/// </summary>
		public Guid TransactionGuid;

		/// <summary>
		/// The primary key of the transaction note record. A transaction can only have one note.
		/// </summary>
		public Guid TransactionNoteGuid;

		/// <summary>
		/// The primary key of the transaction user data record. A transaction can only have one user data record.
		/// </summary>
		public Guid TransactionUserDataGuid;

		/// <summary>
		/// The primary key of the transaction signature record. A transaction can only have one signature record.
		/// </summary>
		public Guid TransactionSignatureGuid;
	}

	/// <summary>
	/// Represents information that can be retrieved for a particular line item
	/// when we attempt to match up legacy records with existing records based on the line item's sequence and owning transaction
	/// </summary>
    public class TransactionLineItemGuidMap
	{
		/// <summary>
		/// The primary key of the line item
		/// </summary>
		public Guid TransactionLineItemGuid;

		/// <summary>
		/// The primary key of the line item user data record. A line item can only have one user data record.
		/// </summary>
		public Guid TransactionLineItemUserDataGuid;
	}

	/// <summary>
	/// Allows us to associate a sequence ID with a sub line item. 
	/// We need to use the sequence to match legacy records up with existing records, 
	/// but there is no sequence ID on the sub line item.
	/// </summary>
    public class SubLineItemWithSequenceID
	{
		/// <summary>
		/// Used to identify a sub line item. This value should be unique for any particular sub line item belonging to a line item.
		/// </summary>
		public int SequenceID;

	    /// <summary>
	    /// The actual sub line item record
	    /// </summary>
	    public SubLineItemDO SubLineItem;
	}

	#region Transaction Import Processor Class
	/// <summary>
	/// The transaction import processor class.
	/// </summary>
	[ServiceBehavior(TransactionIsolationLevel = IsolationLevel.ReadCommitted)]
	public class TransactionImportProcessorClass : ITransactionImportProcessor
	{
		#region Public data members
		/// <summary>
		/// The section types.
		/// </summary>
		public enum SectionTypes
		{
			Header,
			LineItem,
			SubLineItem,
			TransportLineItem,
			None = 99
		};
		#endregion

		#region Private data members
		/// <summary>
		/// The parameters table.
		/// </summary>
		private DataTable parametersTable;

		/// <summary>
		/// The import keys list.
		/// </summary>
		private List<ImportKeysClass> importKeysList;

		#endregion

		#region Public Methods
		/// <summary>
		/// The process.
		/// </summary>
		/// <param name="transactionImportSr">
		/// The transaction import service request.
		/// </param>
		[OperationBehavior( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public void Process( TransactionImportSR transactionImportSr )
		{
			this.ImportTransaction(transactionImportSr);
		}

		/// <summary>
		/// The import transaction.
		/// </summary>
		/// <param name="transactionImportSr">
		/// The transaction import service request.
		/// </param>
		/// <exception cref="FaultException{TDetail}">
		/// Error saving transactions.
		/// </exception>
		public void ImportTransaction(TransactionImportSR transactionImportSr)
		{
			List<TransactionDO> transactionCollection;

			if (transactionImportSr.TransactionCollection == null || transactionImportSr.TransactionCollection.Count < 0)
			{
				transactionCollection = new List<TransactionDO> { transactionImportSr.TransactionDO };
			}
			else
			{
				if (transactionImportSr.TransactionCollection == null)
				{
					throw new Exception("No transactions!");
				}

				transactionCollection = transactionImportSr.TransactionCollection;
			}

			var sites = new SitesClass( );
			Guid siteGuid = sites.GetIdentityGuid(transactionImportSr.Security, transactionImportSr.Security.SiteID);

			if ( siteGuid == Guid.Empty )
			{
				throw new Exception("Cannot find Site: " + transactionImportSr.Security.SiteID);
			}

			// Ensure that all the transactions in the collection have the same Site ID.
			// Throw an exception if there are any site IDs that are different.
			bool haveSameSites = transactionCollection.TrueForAll(x => x.Site == transactionImportSr.Security.SiteID);

			if (haveSameSites == false)
			{
				throw new Exception("All Transactions must have the same site ID: " + transactionImportSr.Security.SiteID);
			}

			this.importKeysList = new List<ImportKeysClass>();
			var consolidatedDa = new ConsolidatedDAClass( );
			DataSet headerGuidsDataSet;
			DataSet lineItemGuidsDataSet;
			DataSet subLineItemGuidsDataSet;
			DataSet transportLineItemGuidsDataSet;

			// Build the parameters table that will be used to retrieve all the GUIDs
			// for each transaction in the collection.
			this.BuildParametersTable(transactionCollection, siteGuid);

			// Create a SQL command object to retrieve all the Header GUIDs related to the 
			// transaction collection.
			using (var sqlCommand = new SqlCommand())
			{
				sqlCommand.Parameters.Clear();
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.CommandText = "dbo.usp_GetRelatedGuids";

				SqlParameter sqlParamRecordsToUpdate = sqlCommand.Parameters.AddWithValue("@RelatedGuidParmTable", this.parametersTable);
				sqlParamRecordsToUpdate.SqlDbType = SqlDbType.Structured;
				sqlParamRecordsToUpdate.TypeName = "dbo.utt_RelatedGuidParameters";

				var parm = new SqlParameter("@Section", SqlDbType.Int) { Value = SectionTypes.Header };
				sqlCommand.Parameters.Add(parm);

				headerGuidsDataSet = consolidatedDa.GetDataSet(sqlCommand, transactionImportSr.Security);
			}

			// Create a SQL command object to retrieve all the Line Item GUIDs related to the 
			// transaction collection.
			using ( var sqlCommand = new SqlCommand( ) )
			{
				sqlCommand.Parameters.Clear();
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.CommandText = "dbo.usp_GetRelatedGuids";

				SqlParameter sqlParamRecordsToUpdate = sqlCommand.Parameters.AddWithValue("@RelatedGuidParmTable", this.parametersTable);
				sqlParamRecordsToUpdate.SqlDbType = SqlDbType.Structured;
				sqlParamRecordsToUpdate.TypeName = "dbo.utt_RelatedGuidParameters";

				var parm = new SqlParameter("@Section", SqlDbType.Int) { Value = SectionTypes.LineItem };
				sqlCommand.Parameters.Add(parm);

				lineItemGuidsDataSet = consolidatedDa.GetDataSet(sqlCommand, transactionImportSr.Security);
			}

			// Create a SQL command object to retrieve all the Sub-Line Item GUIDs related to the 
			// transaction collection.
			using ( var sqlCommand = new SqlCommand( ) )
			{
				sqlCommand.Parameters.Clear( );
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.CommandText = "dbo.usp_GetRelatedGuids";

				SqlParameter sqlParamRecordsToUpdate = sqlCommand.Parameters.AddWithValue("@RelatedGuidParmTable", this.parametersTable);
				sqlParamRecordsToUpdate.SqlDbType = SqlDbType.Structured;
				sqlParamRecordsToUpdate.TypeName = "dbo.utt_RelatedGuidParameters";

				var parm = new SqlParameter("@Section", SqlDbType.Int) { Value = SectionTypes.SubLineItem };
				sqlCommand.Parameters.Add(parm);

				subLineItemGuidsDataSet = consolidatedDa.GetDataSet(sqlCommand, transactionImportSr.Security);
			}

			// Create a SQL command object to retrieve all the transport Line Item GUIDs related to the 
			// transaction collection.
			using ( var sqlCommand = new SqlCommand( ) )
			{
				sqlCommand.Parameters.Clear( );
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.CommandText = "dbo.usp_GetRelatedGuids";

				SqlParameter sqlParamRecordsToUpdate = sqlCommand.Parameters.AddWithValue("@RelatedGuidParmTable", this.parametersTable);
				sqlParamRecordsToUpdate.SqlDbType = SqlDbType.Structured;
				sqlParamRecordsToUpdate.TypeName = "dbo.utt_RelatedGuidParameters";

				var parm = new SqlParameter("@Section", SqlDbType.Int) { Value = SectionTypes.TransportLineItem };
				sqlCommand.Parameters.Add(parm);

				transportLineItemGuidsDataSet = consolidatedDa.GetDataSet(sqlCommand, transactionImportSr.Security);
			}

			// Update the Transactions with retrieved GUIDs
			this.UpdateTransGuids(transactionCollection, headerGuidsDataSet, lineItemGuidsDataSet, subLineItemGuidsDataSet, transportLineItemGuidsDataSet);

		    transactionCollection = this.PopulateKeyTransactionGuids(transactionImportSr.Security, transactionCollection);

		    // save the transactions through Accounting Service
		    var saveTransactionsSr = new SaveTransactionsSR
		                             {
		                                 Security		= transactionImportSr.Security,
		                                 Transactions	= transactionCollection,
		                                 AccountingSite = transactionImportSr.AccountingSite,
		                                 ConvertUnits	= transactionImportSr.ConvertUnits,
                                         CreateMissingReversalPieces = transactionImportSr.CreateMissingReversalPieces,
                                         BypassValidation = transactionImportSr.BypassValidation
		                             };

		    var saveTxProcessor = new SaveTransactionsProcessor( );
		    saveTxProcessor.SaveTransactions(saveTransactionsSr);
		}

		/// <summary>
		/// Take a list of transactions and for each transaction in the list retrieve the primary keys associated with all child items of the transaction (e.g. line item, line item user data),
		/// if there is a matching existing record in our system.
		/// Also, handle Conjoined transaction information as it is vital for the import to function properly.
		/// Note that primary keys for weight reading records are not retrieved because we always insert weight readings. Existing records become historical.
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <param name="transactions">A list of transactions to get the primary keys for</param>
		/// <returns>A list of transactions updated with the primary keys</returns>
		public List<TransactionDO> PopulateKeyTransactionGuids(SecurityClass security, List<TransactionDO> transactions)
		{
			if (security == null)
			{
				throw new Exception("Security must be provided");
			}

			if (transactions == null || transactions.Count == 0)
			{
				throw new Exception("You must provide transactions to retrieve the primary keys for");
			}

			Dictionary<string, TransactionGuidMap> transIDMaps = new Dictionary<string, TransactionGuidMap>();

			using (SqlCommand cmd = new SqlCommand())
			{
				// Execute the stored procedure, passing in the list (table) of TransIDs
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "usp_GetTransactionGuids";

				SqlParameter tableValuedParameter = cmd.Parameters.Add("@TransIDs", SqlDbType.Structured);
                tableValuedParameter.Value = CreateGetTransactionGuidsSqlDataRecords(transactions.Select(transaction => transaction.TransID));
				tableValuedParameter.TypeName = "dbo.TransIDListType";

				ConsolidatedDAClass consolidatedDa = new ConsolidatedDAClass();
				DataSet results = consolidatedDa.GetDataSet(cmd, security);

				// Take the results and add them to a dictionary mapping TransIDs to primary key values (e.g. the TransactionGuid)
				if (results.Tables.Count > 0)
				{
					foreach (DataRow row in results.Tables[0].Rows)
					{
						string transID = DataObject.getString(row["TransID"]);

						TransactionGuidMap transactionGuidMap = new TransactionGuidMap
							                                        {
								                                        TransactionGuid = DataObject.getGuid(row["TransactionGuid"]),
								                                        TransactionNoteGuid = DataObject.getGuid(row["TransactionNoteGuid"]),
								                                        TransactionUserDataGuid = DataObject.getGuid(row["TransactionUserDataGuid"])
							                                        };

						if (!transIDMaps.ContainsKey(transID))
						{
							transIDMaps.Add(transID, transactionGuidMap);
						}
					}
				}
			}

			// Update the list of transactions with the corresponding primary key guids if a match was found
			foreach (KeyValuePair<string, TransactionGuidMap> transIDMap in transIDMaps)
			{
				TransactionDO matchingTransaction = transactions.Find(transaction => transaction.TransID == transIDMap.Key);

				if (matchingTransaction == null)
				{
					continue;
				}

				matchingTransaction.TransactionGuid = transIDMap.Value.TransactionGuid;
				matchingTransaction.TransactionNoteGuid = transIDMap.Value.TransactionNoteGuid;
				matchingTransaction.TransactionUserDataGuid = transIDMap.Value.TransactionUserDataGuid;
				matchingTransaction.TransactionSignatureGuid = transIDMap.Value.TransactionSignatureGuid;
			}

			List<KeyValuePair<Guid, LineItemDO>> lineItems = new List<KeyValuePair<Guid, LineItemDO>>();

			// We must resequence the line items ahead of time. This is because the save transactions processor will resequence them for us, 
			// so if you initially add a line item with Sequence = 1 it will actually be 0 when it is inserted. 
			// If the record is ever updated and imported again, the sequence numbers won't match up (1 in the application, 0 in the DB), and
 			// we'll end up creating a duplicate record.
			transactions.ForEach(
				transaction =>
					{
						for (int i = 0; i < transaction.LineItems.Count; i++)
						{
							transaction.LineItems[i].SequenceId = i;

							lineItems.Add(new KeyValuePair<Guid, LineItemDO>(transaction.TransactionGuid, transaction.LineItems[i]));
						}
					});

            this.GetLineItemPrimaryKeys(security, lineItems);

			List<KeyValuePair<Guid, SubLineItemWithSequenceID>> subLineItems = new List<KeyValuePair<Guid, SubLineItemWithSequenceID>>();

			// Add each sublineitem to the sublineitem collection. 
			// When we add a sub line item also add the owning LineItem's TransactionLineItemGuid and the index of the sublineitem in the lineitem's list of sub line items.
			// The overload of select you see here (Select((item, i)...) gives you the item's index represented as i.
			// We have to resequence the sub line items for the same reasons we have to resequence the line items.
			lineItems.ForEach(
				lineItem => subLineItems.AddRange(lineItem.Value.SubLineItems.Select((item, i) => new SubLineItemWithSequenceID
					                                                                               {
						                                                                               SequenceID = i,
																									   SubLineItem = item
					                                                                               })
				.Select(subLineItem => new KeyValuePair<Guid, SubLineItemWithSequenceID>(lineItem.Value.TransactionLineItemGuid, subLineItem))));

			this.GetSubLineItemPrimaryKeys(security, subLineItems);

			List<KeyValuePair<Guid, TransportLineItemDO>> transportLineItems = new List<KeyValuePair<Guid, TransportLineItemDO>>();

			transactions.ForEach(transaction =>
				{
					if (transaction.TransportInfoList != null)
					{
						transportLineItems.AddRange(transaction.TransportInfoList.Select(transportInfo => new KeyValuePair<Guid, TransportLineItemDO>(transaction.TransactionGuid, transportInfo)));
					}
				});

			this.GetTransportLineItemPrimaryKeys(security, transportLineItems);

            // Now that we have retrieved the primary key values we need to perform special processing for conjoined transaction types (Transfers and regrades).
            // The Save Transactions Processor handles these types of transactions differently.
            this.ProcessConjoinedTransactions(security, transactions);

			return transactions;
		}

        /// <summary>
        /// Create SqlDataRecords to get the primary key of the provided transactions
        /// </summary>
        /// <param name="transIDs">The records to get the primary keys of</param>
        /// <returns>SqlDataRecords to get the primary key of the provided transactions</returns>
        private static IEnumerable<SqlDataRecord> CreateGetTransactionGuidsSqlDataRecords(IEnumerable<string> transIDs)
        {
            SqlMetaData[] metaData = new SqlMetaData[1];

            metaData[0] = new SqlMetaData("TransID", SqlDbType.NVarChar, 64);

            SqlDataRecord record = new SqlDataRecord(metaData);

            foreach (string transID in transIDs)
            {
                record.SetString(0, transID);

                yield return record;
            }
        }

		/// <summary>
		/// Retrieve the primary key values associated with a line item like the TransactionLineItemGuid by matching up the TransactionGuid and SequenceID with existing records 
		/// in the database
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="lineItems">A list of TransactionGuids and corresponding line items to get primary key values for</param>
        private void GetLineItemPrimaryKeys(SecurityClass security, List<KeyValuePair<Guid, LineItemDO>> lineItems)
		{
			if (lineItems == null || lineItems.Count == 0)
			{
				return;
			}

			Dictionary<KeyValuePair<Guid, int>, TransactionLineItemGuidMap> lineItemMaps = new Dictionary<KeyValuePair<Guid, int>, TransactionLineItemGuidMap>();

			using (SqlCommand cmd = new SqlCommand())
			{
				// Execute the stored procedure, passing in the list (table) of TransactionGuids and line item sequences
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "usp_GetTransactionLineItemGuids";

				SqlParameter tableValuedParameter = cmd.Parameters.Add("@TransactionGuidAndLineItemSequences", SqlDbType.Structured);
                tableValuedParameter.Value = CreateGetTransactionLineItemGuidSqlDataRecords(lineItems);
				tableValuedParameter.TypeName = "dbo.TransactionGuidAndLineItemSequenceListType";

				ConsolidatedDAClass consolidatedDa = new ConsolidatedDAClass();
				DataSet results = consolidatedDa.GetDataSet(cmd, security);

				if (results.Tables.Count > 0)
				{
					foreach (DataRow row in results.Tables[0].Rows)
					{
						// Get the key values from the results
						Guid transactionGuid = DataObject.getGuid(row["TransactionGuid"]);
						int sequenceNumber = DataObject.getInt(row["SequenceID"]);

						// Get the corresponding primary key values from the results
						TransactionLineItemGuidMap transactionLineItemGuidMap = new TransactionLineItemGuidMap
						{
							TransactionLineItemGuid = DataObject.getGuid(row["TransactionLineItemGuid"]),
							TransactionLineItemUserDataGuid = DataObject.getGuid(row["TransactionLineItemUserDataGuid"]),
						};

						// Add the key values with the corresponding primary key values to a list.
						KeyValuePair<Guid, int> lineItemMapping = new KeyValuePair<Guid, int>(transactionGuid, sequenceNumber);

						if (!lineItemMaps.ContainsKey(lineItemMapping))
						{
							lineItemMaps.Add(lineItemMapping, transactionLineItemGuidMap);
						}
					}
				}
			}

			// Update the line items with the corresponding primary key values if a match was found
			foreach (KeyValuePair<KeyValuePair<Guid, int>, TransactionLineItemGuidMap> lineItemMap in lineItemMaps)
			{
				KeyValuePair<Guid, LineItemDO> matchingLineItem = lineItems.Find(lineItem => lineItem.Key == lineItemMap.Key.Key && lineItem.Value.SequenceId == lineItemMap.Key.Value);

				if (matchingLineItem.Key == Guid.Empty)
				{
					continue;
				}

				matchingLineItem.Value.TransactionLineItemGuid = lineItemMap.Value.TransactionLineItemGuid;
				matchingLineItem.Value.TransactionLineItemUserDataGuid = lineItemMap.Value.TransactionLineItemUserDataGuid;
			}
		}

        /// <summary>
        /// Create SqlDataRecords to get the primary key of the provided lineItems
        /// </summary>
        /// <param name="lineItems">The records to get the primary keys of</param>
        /// <returns>SqlDataRecords to get the primary key of the provided lineItems</returns>
        private static IEnumerable<SqlDataRecord> CreateGetTransactionLineItemGuidSqlDataRecords(IEnumerable<KeyValuePair<Guid, LineItemDO>> lineItems)
        {
            SqlMetaData[] metaData = new SqlMetaData[2];

            int i = 0;
            metaData[i++] = new SqlMetaData("TransactionGuid", SqlDbType.UniqueIdentifier);
            metaData[i] = new SqlMetaData("SequenceID", SqlDbType.SmallInt);

            SqlDataRecord record = new SqlDataRecord(metaData);

            foreach (KeyValuePair<Guid, LineItemDO> lineItem in lineItems)
            {
                int j = 0;
                record.SetGuid(j++, lineItem.Key);
                record.SetInt16(j, (short)lineItem.Value.SequenceId.GetValueOrDefault());

                yield return record;
            }
        }

		/// <summary>
		/// Retrieve the TransactionSubLineItemGuid for a sub line item by matching up the TransactionLineItemGuid and SequenceID with existing records 
		/// in the database
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="subLineItems">A list of TransactionLineItemGuids and corresponding sub line items to get primary key values for</param>
		private void GetSubLineItemPrimaryKeys(SecurityClass security, List<KeyValuePair<Guid, SubLineItemWithSequenceID>> subLineItems)
		{
			if (subLineItems == null || subLineItems.Count == 0)
			{
				return;
			}

			Dictionary<KeyValuePair<Guid, int>, Guid> subLineItemMaps = new Dictionary<KeyValuePair<Guid, int>, Guid>();

			using (SqlCommand cmd = new SqlCommand())
			{
				// Execute the stored procedure, passing in the list (table) of TransactionLineItemGuids and sub line item sequences
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "usp_GetTransactionSubLineItemGuids";

				SqlParameter tableValuedParameter = cmd.Parameters.Add("@TransactionGuidAndSubLineItemSequences", SqlDbType.Structured);
                tableValuedParameter.Value = CreateGetTransactionSubLineItemGuidsSqlDataRecords(subLineItems);
				tableValuedParameter.TypeName = "dbo.TransactionGuidAndSubLineItemSequenceListType";

				ConsolidatedDAClass consolidatedDa = new ConsolidatedDAClass();
				DataSet results = consolidatedDa.GetDataSet(cmd, security);

				if (results.Tables.Count > 0)
				{
					foreach (DataRow row in results.Tables[0].Rows)
					{
						// Get the key values from the results
						Guid transactionLineItemGuid = DataObject.getGuid(row["TransactionLineItemGuid"]);
						int sequenceNumber = DataObject.getInt(row["SequenceID"]);

						// Get the primary key value from the result
						Guid transactionSubLineItemGuid = DataObject.getGuid(row["TransactionSubLineItemGuid"]);

						// Add the key values with the corresponding TransactionSubLineItemGuid to a list.
						KeyValuePair<Guid, int> subLineItemMapping = new KeyValuePair<Guid, int>(transactionLineItemGuid, sequenceNumber);

						if (!subLineItemMaps.ContainsKey(subLineItemMapping))
						{
							subLineItemMaps.Add(subLineItemMapping, transactionSubLineItemGuid);
						}
					}
				}
			}

			// Update the sub line items with the corresponding TransactionSubLineItemGuid if a match was found
			foreach (KeyValuePair<KeyValuePair<Guid, int>, Guid> subLineItemMap in subLineItemMaps)
			{
				KeyValuePair<Guid, SubLineItemWithSequenceID> matchingSubLineItem = subLineItems.Find(subLineItem => subLineItem.Key == subLineItemMap.Key.Key && subLineItem.Value.SequenceID == subLineItemMap.Key.Value);

				if (matchingSubLineItem.Key == Guid.Empty)
				{
					continue;
				}

				matchingSubLineItem.Value.SubLineItem.TransactionSubLineItemGuid = subLineItemMap.Value;
			}
		}

        /// <summary>
        /// Create SqlDataRecords to get the primary key of the provided subLineItems
        /// </summary>
        /// <param name="subLineItems">The records to get the primary keys of</param>
        /// <returns>SqlDataRecords to get the primary key of the provided subLineItems</returns>
        private static IEnumerable<SqlDataRecord> CreateGetTransactionSubLineItemGuidsSqlDataRecords(IEnumerable<KeyValuePair<Guid, SubLineItemWithSequenceID>> subLineItems)
        {
            SqlMetaData[] metaData = new SqlMetaData[2];

            int i = 0;
            metaData[i++] = new SqlMetaData("TransactionLineItemGuid", SqlDbType.UniqueIdentifier);
            metaData[i] = new SqlMetaData("SequenceID", SqlDbType.SmallInt);

            SqlDataRecord record = new SqlDataRecord(metaData);

            foreach (KeyValuePair<Guid, SubLineItemWithSequenceID> subLineItem in subLineItems)
            {
                int j = 0;
                record.SetGuid(j++, subLineItem.Key);
                record.SetInt16(j, (short)subLineItem.Value.SequenceID);

                yield return record;
            }
        }

		/// <summary>
		/// Retrieve the primary keys of transport line items by matching up provided records with those in the database using a known TransactionGuid and TransportOrderNumber
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="transportLineItems">A list of TransactionGuids and corresponding transport line items to get primary key values for</param>
		private void GetTransportLineItemPrimaryKeys(SecurityClass security, List<KeyValuePair<Guid, TransportLineItemDO>> transportLineItems)
		{
			if (transportLineItems == null || transportLineItems.Count == 0)
			{
				return;
			}

			Dictionary<KeyValuePair<Guid, string>, Guid> transportLineItemMaps = new Dictionary<KeyValuePair<Guid, string>, Guid>();

			using (SqlCommand cmd = new SqlCommand())
			{
				// Execute the stored procedure, passing in the list (table) of TransactionLineItemGuids and transport order numbers
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "usp_GetTransactionTransportLineItemGuids";

				SqlParameter tableValuedParameter = cmd.Parameters.Add("@TransactionGuidAndTransportOrderNumbers", SqlDbType.Structured);
                tableValuedParameter.Value = CreateGetTransactionTransportLineItemGuidsSqlDataRecords(transportLineItems);
				tableValuedParameter.TypeName = "dbo.TransactionGuidAndTransportOrderNumberListType";

				ConsolidatedDAClass consolidatedDa = new ConsolidatedDAClass();
				DataSet results = consolidatedDa.GetDataSet(cmd, security);

				if (results.Tables.Count > 0)
				{
					foreach (DataRow row in results.Tables[0].Rows)
					{
						// Get the key values from the results
						Guid transactionGuid = DataObject.getGuid(row["TransactionGuid"]);
						string transportOrderNumber = DataObject.getString(row["TransportOrderNumber"]);

						// Get the primary key value from the result
						Guid transportLineItemGuid = DataObject.getGuid(row["TransactionTransportLineItemGuid"]);

						// Add the key values with the corresponding TransactionTransportLineItemGuid to a list.
						KeyValuePair<Guid, string> transportLineItemMapping = new KeyValuePair<Guid, string>(transactionGuid, transportOrderNumber);

						if (!transportLineItemMaps.ContainsKey(transportLineItemMapping))
						{
							transportLineItemMaps.Add(transportLineItemMapping, transportLineItemGuid);
						}
					}
				}
			}

			// Update the transport line items with the corresponding TransactionTransportLineItemGuid if a match was found
			foreach (KeyValuePair<KeyValuePair<Guid, string>, Guid> transportLineItemMap in transportLineItemMaps)
			{
				KeyValuePair<Guid, TransportLineItemDO> matchingTransportLineItem = transportLineItems.Find(transportLineItem => transportLineItem.Key == transportLineItemMap.Key.Key && transportLineItem.Value.TransportOrderNumber == transportLineItemMap.Key.Value);

				if (matchingTransportLineItem.Key == Guid.Empty)
				{
					continue;
				}

				matchingTransportLineItem.Value.TransactionTransportLineItemGuid = transportLineItemMap.Value;
			}
		}

        /// <summary>
        /// Create SqlDataRecords to get the primary key of the provided transportLineItems
        /// </summary>
        /// <param name="transportLineItems">The records to get the primary keys of</param>
        /// <returns>SqlDataRecords to get the primary key of the provided transportLineItems</returns>
        private static IEnumerable<SqlDataRecord> CreateGetTransactionTransportLineItemGuidsSqlDataRecords(IEnumerable<KeyValuePair<Guid, TransportLineItemDO>> transportLineItems)
        {
            SqlMetaData[] metaData = new SqlMetaData[2];

            int i = 0;
            metaData[i++] = new SqlMetaData("TransactionGuid", SqlDbType.UniqueIdentifier);
            metaData[i] = new SqlMetaData("TransportOrderNumber", SqlDbType.NVarChar, 50);

            SqlDataRecord record = new SqlDataRecord(metaData);

            foreach (KeyValuePair<Guid, TransportLineItemDO> transportLineItem in transportLineItems)
            {
                int j = 0;
                record.SetGuid(j++, transportLineItem.Key);
                record.SetString(j, transportLineItem.Value.TransportOrderNumber);

                yield return record;
            }
        }

        /// <summary>
        /// Perform special processing related to conjoined transactions (Transfers and Regrades).
        /// Because the Save Transactions Processor creates or modifies two transactions from one for conjoined transaction types,
        /// we must take some extra steps like setting the ConjoinedTransactionGuid.
        /// </summary>
        /// <param name="security">Contains Security Information.</param>
        /// <param name="transactions">A set of transactions to process. Only transactions that have a type of transfer or regrade will be processed.</param>
        private void ProcessConjoinedTransactions(SecurityClass security, IEnumerable<TransactionDO> transactions)
        {
            // Process every transaction that is a conjoined transaction type.
            List<TransactionDO> conjoinedTransactions = transactions.Where(
                    transaction =>
                    transaction.TransTypeID == TransactionTypes.T11_ConsumerTransfer
                    || transaction.TransTypeID == TransactionTypes.T13_OwnerTransfer
                    || transaction.TransTypeID == TransactionTypes.T15_PrimaryRegrade
                    || transaction.TransTypeID == TransactionTypes.T16_SecondaryRegrade
                    || transaction.TransTypeID == TransactionTypes.T23_StorageTransfer).ToList();

            List<TransactionDO> existingConjoinedTransactions = new List<TransactionDO>();
            List<TransactionDO> newTransactionsWithConjoinedGuids = new List<TransactionDO>();
    
            foreach (TransactionDO transaction in conjoinedTransactions)
            {
                // If the transaction we're processing already exists, we can assume that the conjoined transaction has also already been created.            
                if (transaction.TransactionGuid != Guid.Empty)
                {
                    // We must retrieve the existing record's ConjoinedTransactionGuid and other conjoined information 
                    // so that the Save Transactions Processor does not try to create a new record for us.
                    // Keep in mind that the external interface may not be providing any conjoined information whatsoever like the ConjoinedTransId
                    // so we can't rely on the values provided by the interface. 
                    existingConjoinedTransactions.Add(transaction);
                }
                else
                {
                    // The transaction is a new transaction. 
                    // We must consider that either the conjoined transaction has been specified by the 
                    // external interface and the ConjoinedTransactionGuid has been set
                    // or that if the ConjoinedTransactionGuid was not set that the conjoined transaction 
                    // will be automatically created by the Save Transactions Processor.

                    // If the subtype isn't provide by the interface we must specify one. 
                    // We will assume that this is a debit transaction.
                    if (string.IsNullOrEmpty(transaction.SubType))
                    {
                        transaction.SubType = TransactionDO.DEBIT;
                    }

                    // The conjoined guid will be set by the interface when it validates any conjoined trans id provided.
                    // This means that the external interface is telling us which transaction we are conjoined to.
                    // If this is the case, we still need to set the line item ConjoinedGuids and other conjoined values.
                    if (transaction.ConjoinedTransactionGuid != Guid.Empty)
                    {
                        newTransactionsWithConjoinedGuids.Add(transaction);
                    }
                }
              
                // Logic must be applied to ensure that quantities for debit transactions are negative 
                // and that quantities for credit transactions are positive.
                transaction.SetVolumeSigns(false);
            }

            this.GetConjoinedTransactionInformationUsingTransactionGuid(security, existingConjoinedTransactions);
            this.GetConjoinedTransactionInformationUsingConjoinedGuid(security, newTransactionsWithConjoinedGuids);

            // We only need to get line item information for records processed by either of the two methods above.
            existingConjoinedTransactions.AddRange(newTransactionsWithConjoinedGuids);
            this.GetConjoinedTransactionLineItemInformation(security, existingConjoinedTransactions);
	    }

	    /// <summary>
	    /// Use the transaction information provided to retrieve information necessary to save conjoined transactions.
	    /// We have to set all of the ConjoinedGuids or otherwise the conjoined transactions data won't get updated properly.
	    /// Retrieve the conjoined information using the TransactionGuid - we will determine which transaction we are conjoined to in the DB and
	    /// retrieve the corresponding conjoined transaction's guids.
	    /// </summary>
	    /// <param name="securityParam">Contains Security Information</param>
	    /// <param name="existingConjoinedTransactions">Transactions to get conjoined information for</param>
	    private void GetConjoinedTransactionInformationUsingTransactionGuid(SecurityClass securityParam, List<TransactionDO> existingConjoinedTransactions)
        {
            // Don't do anything if there are no records to process
            if (existingConjoinedTransactions.Count <= 0)
            {
                return;
            }

            var consolidatedDa = new ConsolidatedDAClass();

            using (var cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_TransactionHeaderGetConjoinedInformation";
                SqlParameter tableValuedParameter = cmd.Parameters.Add("@TransactionGuids", SqlDbType.Structured);
                tableValuedParameter.Value = CreateGetConjoinedInformationSqlDataRecords(existingConjoinedTransactions.Select(transaction => transaction.TransactionGuid));
                tableValuedParameter.TypeName = "dbo.GuidListType";

                DataSet results = consolidatedDa.GetDataSet(cmd, securityParam);

                if (results.Tables.Count <= 0 || results.Tables[0] == null || results.Tables[0].Rows.Count < 1)
                {
                    return;
                }

                foreach (DataRow row in results.Tables[0].Rows)
                {
                    Guid transactionGuid = DataObject.getGuid(row["TransactionGuid"]);

                    TransactionDO transaction = existingConjoinedTransactions.Find(matchingTransaction => matchingTransaction.TransactionGuid == transactionGuid);

                    // We will override any conjoined information provided by the external interface and instead use what's on record.
                    // This shouldn't be a problem unless someone tries to change the conjoined information or the subtype
                    // after the record has been intially created.          
                    if (transaction != null)
                    {
                        transaction.ConjoinedTransactionGuid = DataObject.getGuid(row["ConjoinTransactionGuid"]);
                        transaction.SubType = DataObject.getString(row["SubType"]);
                        transaction.ConjoinedTransID = DataObject.getString(row["ConjoinTransID"]);
                        transaction.ConjoinedUserDataGuid = DataObject.getGuid(row["ConjoinTransactionUserDataGuid"]);
                        transaction.ConjoinedNotesGuid = DataObject.getGuid(row["ConjoinTransactionNoteGuid"]);
                        transaction.ConjoinedSignatureGuid = DataObject.getGuid(row["ConjoinTransactionSignatureGuid"]);
                    }
                }
            }
        }

	    /// <summary>
	    /// Get the conjoined information for new transactions that have the ConjoinedTransactionGuid already set.
	    /// Even though we have the ConjoinedTransactionGuid we still need to retrieve the user data, notes, and signature guids.
	    /// </summary>
	    /// <param name="securityParam">Contains Security Information</param>
	    /// <param name="newTransactionsWithConjoinedGuids">Transactions to get the conjoined information for using the ConjoinedTransactionGuid</param>
	    private void GetConjoinedTransactionInformationUsingConjoinedGuid(SecurityClass securityParam, List<TransactionDO> newTransactionsWithConjoinedGuids)
        {
            // Don't do anything if there are no records to process
            if (newTransactionsWithConjoinedGuids.Count <= 0)
            {
                return;
            }

            var consolidatedDa = new ConsolidatedDAClass();

            using (var cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_TransactionHeaderGetNotesUserDataSignatureGuids";
                SqlParameter tableValuedParameter = cmd.Parameters.Add("@TransactionGuids", SqlDbType.Structured);
                tableValuedParameter.Value = CreateGetConjoinedInformationSqlDataRecords(newTransactionsWithConjoinedGuids.Select(transaction => transaction.ConjoinedTransactionGuid));
                tableValuedParameter.TypeName = "dbo.GuidListType";

                DataSet results = consolidatedDa.GetDataSet(cmd, securityParam);

                if (results.Tables.Count <= 0 || results.Tables[0] == null || results.Tables[0].Rows.Count < 1)
                {
                    return;
                }

                foreach (DataRow row in results.Tables[0].Rows)
                {           
                    Guid transactionGuid = DataObject.getGuid(row["TransactionGuid"]);
                    
                    // Find the transaction the results correspond to
                    TransactionDO transaction = newTransactionsWithConjoinedGuids.Find(matchingTransaction => matchingTransaction.ConjoinedTransactionGuid == transactionGuid);

                    // We will override any conjoined information provided by the external interface and instead use what's on record.
                    // This shouldn't be a problem unless someone tries to change the conjoined information or the subtype
                    // after the record has been intially created.          
                    if (transaction != null)
                    {
                        transaction.ConjoinedUserDataGuid = DataObject.getGuid(row["TransactionUserDataGuid"]);
                        transaction.ConjoinedNotesGuid = DataObject.getGuid(row["TransactionNoteGuid"]);
                        transaction.ConjoinedSignatureGuid = DataObject.getGuid(row["TransactionSignatureGuid"]);
                    }
                }
            }          
        }

	    /// <summary>
	    /// Use the transaction information provided to retrieve line item information necessary to save conjoined line items.
	    /// We have to set all of the ConjoinedGuids or otherwise the conjoined line item won't get updated properly
	    /// </summary>
	    /// <param name="securityParam">Contains Security Information</param>
	    /// <param name="transactions">Conjoined transactions to get conjoined line item information for</param>
	    private void GetConjoinedTransactionLineItemInformation(SecurityClass securityParam, List<TransactionDO> transactions)
        {
            // Don't do anything if there are no records to process
	        if (transactions.Count <= 0)
	        {
	            return;
	        }

            var consolidatedDa = new ConsolidatedDAClass();

            using (var cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_TransactionLineItemsGetConjoinedInformation";
                SqlParameter tableValuedParameter = cmd.Parameters.Add("@TransactionGuids", SqlDbType.Structured);
                tableValuedParameter.Value = CreateGetConjoinedInformationSqlDataRecords(transactions.Select(transaction => transaction.ConjoinedTransactionGuid));
                tableValuedParameter.TypeName = "dbo.GuidListType";
              
                DataSet results = consolidatedDa.GetDataSet(cmd, securityParam);

                if (results.Tables.Count <= 0 || results.Tables[0] == null || results.Tables[0].Rows.Count < 1)
                {
                    return;
                }

                foreach (DataRow row in results.Tables[0].Rows)
                {
                    Guid transactionGuid = DataObject.getGuid(row["TransactionGuid"]);

                    TransactionDO transaction = transactions.Find(matchingTransaction => matchingTransaction.ConjoinedTransactionGuid == transactionGuid);

                    if (transaction != null)
                    {
                        Guid transactionLineItemGuid = DataObject.getGuid(row["TransactionLineItemGuid"]);
                        int sequenceId = DataObject.getInt(row["SequenceID"]);
                        Guid transactionLineItemUserDataGuid = DataObject.getGuid(row["TransactionLineItemUserDataGuid"]);

                        // Retrieve each line item's ConjoinedGuid by matching the existing transaction line items on sequenceId.                 
                        LineItemDO conjoinedLineItem = transaction.LineItems.Find(matchingLineItem => matchingLineItem.SequenceId == sequenceId);

                        if (conjoinedLineItem != null)
                        {
                            conjoinedLineItem.ConjoinedTransactionLineItemGuid = transactionLineItemGuid;
                            conjoinedLineItem.ConjoinedTransactionLineItemUserDataGuid = transactionLineItemUserDataGuid;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Create SqlDataRecords to pass to the stored procedures which are used to process conjoined transactions
        /// </summary>
        /// <param name="transactionGuids">TransactionGuids to create SqlDataRecords for</param>
        /// <returns>SqlDataRecords to pass to the stored procedures which are used to process conjoined transactions</returns>
        private static IEnumerable<SqlDataRecord> CreateGetConjoinedInformationSqlDataRecords(IEnumerable<Guid> transactionGuids)
        {
            SqlMetaData[] metaData = new SqlMetaData[1];

            metaData[0] = new SqlMetaData("Guid", SqlDbType.UniqueIdentifier);

            SqlDataRecord record = new SqlDataRecord(metaData);

            foreach (Guid transactionGuid in transactionGuids)
            {
                record.SetGuid(0, transactionGuid);

                yield return record;
            }
        }

		/// <summary>
		/// Take a list of transactions and for each transaction in the list retrieve the primary keys associated with all records in the transaction, 
		/// and then save the transactions.
        /// Also, handle Conjoined transaction information as it is vital for the import to function properly.
		/// Note that primary keys for weight reading records are not retrieved because we always insert weight readings. Existing records become historical. 
		/// </summary>
		/// <param name="saveRequest">Information needed to save transactions, including the list of transactions themselves</param>
		/// <returns>The results from saving the transactions</returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public SaveTransactionsResultDO PopulateKeyTransactionGuidsAndSave(SaveTransactionsSR saveRequest)
		{
			List<TransactionDO> updatedTransactions = this.PopulateKeyTransactionGuids(saveRequest.Security, saveRequest.Transactions);

			saveRequest.Transactions = updatedTransactions;

			SaveTransactionsProcessor saveTransactionsProcessor = new SaveTransactionsProcessor();
			return saveTransactionsProcessor.SaveTransactions(saveRequest);
		}

		/// <summary>
		/// Retrieve only the corresponding TransactionGuid for the transIDs provided
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="transIds">A list of transIds to retrieve the corresponding guids for</param>
		/// <returns>A dictionary mapping the transIds provided to the TransactionGuids</returns>
		public Dictionary<string, Guid> GetTransactionGuidsForTransIDs(SecurityClass security, List<string> transIds)
		{
			if (security == null)
			{
				throw new Exception("Security must be provided");
			}

			if (transIds == null || transIds.Count == 0)
			{
				throw new Exception("You must provide trans ids to retrieve the transaction guids for");
			}

			Dictionary<string, Guid> transIdMapping = new Dictionary<string, Guid>();

			using (SqlCommand cmd = new SqlCommand())
			{
				// Execute the stored procedure, passing in the list (table) of TransIDs
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "usp_GetTransactionGuids";

				SqlParameter tableValuedParameter = cmd.Parameters.Add("@TransIDs", SqlDbType.Structured);
                tableValuedParameter.Value = CreateGetTransactionGuidsSqlDataRecords(transIds.Distinct());
				tableValuedParameter.TypeName = "dbo.TransIDListType";

				ConsolidatedDAClass consolidatedDa = new ConsolidatedDAClass();
				DataSet results = consolidatedDa.GetDataSet(cmd, security);

				// Take the results and add them to a dictionary mapping TransIDs to Transaction Guids
				if (results.Tables.Count > 0)
				{
					foreach (DataRow row in results.Tables[0].Rows)
					{
						string transID = DataObject.getString(row["TransID"]);

						Guid matchingGuid = DataObject.getGuid(row["TransactionGuid"]);

						if (!transIdMapping.ContainsKey(transID))
						{
							transIdMapping.Add(transID, matchingGuid);
						}
					}
				}
			}

			return transIdMapping;
		}

		#endregion

		#region Private methods

		/// <summary>
		/// This method will update the transaction header entities with their respective GUIDs. In addition,
		/// it will call the line item update method to update GUIDs.
		/// </summary>
		/// <param name="transList">
		/// The trans list.
		/// </param>
		/// <param name="headerDataSet">
		/// The header data set.
		/// </param>
		/// <param name="lineItemDataSet">
		/// The line item data set.
		/// </param>
		/// <param name="subLineItemDataSet">
		/// The sub line item data set.
		/// </param>
		/// <param name="transportLineItemDataSet">
		/// The transport line item data set.
		/// </param>
		private void UpdateTransGuids(
										List<TransactionDO> transList, 
										DataSet headerDataSet, 
										DataSet lineItemDataSet,
										DataSet subLineItemDataSet,
										DataSet transportLineItemDataSet)
		{
			List<ImportResultsClass> headerResults	= this.LoadResultsDataSet(headerDataSet);

			foreach (TransactionDO transactionDo in transList)
			{
				var importKey = this.importKeysList.Find(x => x.Identifier == transactionDo.TransID);
				
				if (importKey == null)
				{
					continue;
				}

				ImportResultsClass result = headerResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "TransID" && x.Section == SectionTypes.Header);
				transactionDo.TransactionGuid = result?.EntityGuid ?? Guid.Empty;

				result = headerResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "AliasName" && x.Section == SectionTypes.Header);
				transactionDo.TransactionAliasGuid = result?.EntityGuid ?? Guid.Empty;

				result = headerResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "ShipToID" && x.Section == SectionTypes.Header);
				transactionDo.ShipToCompanyGuid = result?.EntityGuid ?? Guid.Empty;

				result = headerResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "SupplierID" && x.Section == SectionTypes.Header);
				transactionDo.SupplierCompanyGuid = result?.EntityGuid ?? Guid.Empty;

				result = headerResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "ShipperID" && x.Section == SectionTypes.Header);
				transactionDo.ShipperCompanyGuid = result?.EntityGuid ?? Guid.Empty;

				result = headerResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "OwnerID" && x.Section == SectionTypes.Header);
				transactionDo.OwnerCompanyGuid = result?.EntityGuid ?? Guid.Empty;

				result = headerResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "ManagerID" && x.Section == SectionTypes.Header);
				transactionDo.ManagerCompanyGuid = result?.EntityGuid ?? Guid.Empty;

				result = headerResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "CarrierID" && x.Section == SectionTypes.Header);
				transactionDo.CarrierCompanyGuid = result?.EntityGuid ?? Guid.Empty;

				result = headerResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "BillToID" && x.Section == SectionTypes.Header);
				transactionDo.BillToCompanyGuid = result?.EntityGuid ?? Guid.Empty;

				if ( transactionDo.TransTypeID == TransactionTypes.T11_ConsumerTransfer )
				{
					result = headerResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "ToShipToID" && x.Section == SectionTypes.Header);
					((ConsumerTransferDO) transactionDo).ToShipToCompanyGuid = result?.EntityGuid ?? Guid.Empty;

					result = headerResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "ToBillToID" && x.Section == SectionTypes.Header);
					((ConsumerTransferDO) transactionDo).ToBillToCompanyGuid = result?.EntityGuid ?? Guid.Empty;
				}

				if ( transactionDo.TransTypeID == TransactionTypes.T13_OwnerTransfer )
				{
					result = headerResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "ToOwnerID" && x.Section == SectionTypes.Header);
					((OwnerTransferDO) transactionDo).ToOwnerCompanyGuid = result?.EntityGuid ?? Guid.Empty;

					result = headerResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "ToManagerID" && x.Section == SectionTypes.Header);
					((OwnerTransferDO) transactionDo).ToManagerCompanyGuid = result?.EntityGuid ?? Guid.Empty;
				}

				result = headerResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "DestinationRegistrationID1" && x.Section == SectionTypes.Header);
				transactionDo.DestinationEQ1.EquipmentGuid = result?.EntityGuid ?? Guid.Empty;

				result = headerResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "DestinationRegistrationID2" && x.Section == SectionTypes.Header);
				transactionDo.DestinationEQ2.EquipmentGuid = result?.EntityGuid ?? Guid.Empty;

				result = headerResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "DestinationRegistrationID3" && x.Section == SectionTypes.Header);
				transactionDo.DestinationEQ3.EquipmentGuid = result?.EntityGuid ?? Guid.Empty;

				result = headerResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "SourceRegistrationID1" && x.Section == SectionTypes.Header);
				transactionDo.SourceEQ1.EquipmentGuid = result?.EntityGuid ?? Guid.Empty;

				result = headerResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "SourceRegistrationID2" && x.Section == SectionTypes.Header);
				transactionDo.SourceEQ2.EquipmentGuid = result?.EntityGuid ?? Guid.Empty;

				result = headerResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "SourceRegistrationID3" && x.Section == SectionTypes.Header);
				transactionDo.SourceEQ3.EquipmentGuid = result?.EntityGuid ?? Guid.Empty;

				result = headerResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "OperatorID" && x.Section == SectionTypes.Header);
				transactionDo.OperatorPersonnelGuid = result?.EntityGuid ?? Guid.Empty;

				result = headerResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "FuelCardID" && x.Section == SectionTypes.Header);
				transactionDo.FuelCardGuid = result?.EntityGuid ?? Guid.Empty;

				result = headerResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "FinalStationID" && x.Section == SectionTypes.Header);
				transactionDo.RouteInfo.FinalStationIATAGuid = result?.EntityGuid ?? Guid.Empty;

				result = headerResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "PreviousStationID" && x.Section == SectionTypes.Header);
				transactionDo.RouteInfo.PreviousStationIATAGuid = result?.EntityGuid ?? Guid.Empty;

				result = headerResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "NextStationID" && x.Section == SectionTypes.Header);
				transactionDo.RouteInfo.NextStationIATAGuid = result?.EntityGuid ?? Guid.Empty;

				result = headerResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "OriginStationID" && x.Section == SectionTypes.Header);
				transactionDo.RouteInfo.OriginStationIATAGuid = result?.EntityGuid ?? Guid.Empty;

				// Update the line items for this transaction.
				this.UpdateLineItems(transactionDo, lineItemDataSet, subLineItemDataSet);

				// Update the transport line items for this transaction.
				this.UpdateTransportLineItems(transactionDo, transportLineItemDataSet);
			}
		}

		/// <summary>
		/// This method will update the line item entities with their respective GUIDs.
		/// </summary>
		/// <param name="transDo">
		/// The transaction data object.
		/// </param>
		/// <param name="lineItemDataSet">
		/// The line item data set.
		/// </param>
		/// <param name="subLineItemDataSet">
		/// The sub line item data set.
		/// </param>
		private void UpdateLineItems(TransactionDO transDo, DataSet lineItemDataSet, DataSet subLineItemDataSet)
		{
			List<ImportResultsClass> lineItemResults = this.LoadResultsDataSet(lineItemDataSet);
			int lineItemIndex = 1;

			foreach (LineItemDO lineItem in transDo.LineItems)
			{
				int identifierIndex = lineItemIndex++;

				if (lineItem.SequenceId != null)
				{
					identifierIndex = lineItem.SequenceId.Value;
				}

				string identifier = transDo.TransID + "|L" + identifierIndex;
				var importKey = this.importKeysList.Find(x => x.Identifier == identifier);

				if ( importKey == null )
				{
					continue;
				}

				ImportResultsClass result = lineItemResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "TransactionLineItemID" && x.Section == SectionTypes.LineItem);
				importKey.LineItemDo.TransactionLineItemGuid = result?.EntityGuid ?? Guid.Empty;

				result = lineItemResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "AdditiveProfileID" && x.Section == SectionTypes.LineItem);
				importKey.LineItemDo.AdditiveProfileGuid = result?.EntityGuid ?? Guid.Empty;

				result = lineItemResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "Product" && x.Section == SectionTypes.LineItem);
				importKey.LineItemDo.ProductGuid = result?.EntityGuid ?? Guid.Empty;

				if ( transDo.TransTypeID == TransactionTypes.T15_PrimaryRegrade || transDo.TransTypeID == TransactionTypes.T16_SecondaryRegrade )
				{
					result = lineItemResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "ToProduct" && x.Section == SectionTypes.LineItem);
					((RegradeLineItemDO) importKey.LineItemDo).ToProductGuid = result?.EntityGuid ?? Guid.Empty;
				}

				result = lineItemResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "DestinationRegistrationID1" && x.Section == SectionTypes.LineItem);
				importKey.LineItemDo.DestinationEQ.EquipmentGuid = result?.EntityGuid ?? Guid.Empty;

				result = lineItemResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "SourceRegistrationID1" && x.Section == SectionTypes.LineItem);
				importKey.LineItemDo.SourceEQ.EquipmentGuid = result?.EntityGuid ?? Guid.Empty;

				result = lineItemResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "DestinationCompartmentID" && x.Section == SectionTypes.LineItem);
				importKey.LineItemDo.DestinationCompartmentEquipmentGuid = result?.EntityGuid ?? Guid.Empty;

				result = lineItemResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "SourceCompartmentID" && x.Section == SectionTypes.LineItem);
				importKey.LineItemDo.SourceCompartmentEquipmentGuid = result?.EntityGuid ?? Guid.Empty;

				result = lineItemResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "OperatorID" && x.Section == SectionTypes.LineItem);
				importKey.LineItemDo.OperatorPersonnelGuid = result?.EntityGuid ?? Guid.Empty;

				result = lineItemResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "StorageLocationID" && x.Section == SectionTypes.LineItem);
				importKey.LineItemDo.StorageLocationTankGuid = result?.EntityGuid ?? Guid.Empty;

				if ( transDo.TransTypeID == TransactionTypes.T23_StorageTransfer )
				{
					result = lineItemResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "ToStorageLocationID" && x.Section == SectionTypes.LineItem);
					((StorageTransferLineItemDO) importKey.LineItemDo).ToStorageLocationTankGuid = result?.EntityGuid ?? Guid.Empty;
				}

				result = lineItemResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "LoadingLocationID" && x.Section == SectionTypes.LineItem);
				importKey.LineItemDo.LoadingLocationStationGuid = result?.EntityGuid ?? Guid.Empty;

				result = lineItemResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "MeterID" && x.Section == SectionTypes.LineItem);
				importKey.LineItemDo.MeterGuid = result?.EntityGuid ?? Guid.Empty;

				// Update the sub line item GUIDs
				this.UpdateSubLineItems(importKey.LineItemDo, subLineItemDataSet, identifier);
			}
		}

		/// <summary>
		/// This method will update the sub line item entities with their respective GUIDs.
		/// </summary>
		/// <param name="lineItem">
		/// The line item.
		/// </param>
		/// <param name="subLineItemDataSet">
		/// The sub line item data set.
		/// </param>
		/// <param name="lineItemIdentifier">
		/// The line Item Identifier.
		/// </param>
		private void UpdateSubLineItems(LineItemDO lineItem, DataSet subLineItemDataSet, string lineItemIdentifier)
		{
			List<ImportResultsClass> subLineItemResults = this.LoadResultsDataSet(subLineItemDataSet);

			int subLineItemIndex = 1;

			foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
			{
				int identifierIndex = subLineItemIndex++;
				
				if (subLineItem.SequenceId != null)
				{
					identifierIndex = subLineItem.SequenceId.Value;
				}

				string identifier = lineItemIdentifier + "|S" + identifierIndex;
				var importKey = this.importKeysList.Find(x => x.Identifier == identifier);

				if ( importKey == null )
				{
					continue;
				}

				ImportResultsClass result = subLineItemResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "TransactionLineItemID" && x.Section == SectionTypes.SubLineItem);
				importKey.SubLineItemDo.TransactionSubLineItemGuid = result?.EntityGuid ?? Guid.Empty;

				result = subLineItemResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "Product" && x.Section == SectionTypes.SubLineItem);
				importKey.SubLineItemDo.ProductGuid = result?.EntityGuid ?? Guid.Empty;

				result = subLineItemResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "StorageLocationID" && x.Section == SectionTypes.SubLineItem);
				importKey.SubLineItemDo.StorageLocationTankGuid = result?.EntityGuid ?? Guid.Empty;

				result = subLineItemResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "MeterID" && x.Section == SectionTypes.SubLineItem);
				importKey.SubLineItemDo.MeterGuid = result?.EntityGuid ?? Guid.Empty;
			}
		}

		/// <summary>
		/// The update transport line items.
		/// </summary>
		/// <param name="transDo">
		/// The transaction data object.
		/// </param>
		/// <param name="transportLineItemDataSet">
		/// The transport line item data set.
		/// </param>
		private void UpdateTransportLineItems(TransactionDO transDo, DataSet transportLineItemDataSet)
		{
			List<ImportResultsClass> transportLineItemResults = this.LoadResultsDataSet(transportLineItemDataSet);

			foreach ( TransportLineItemDO transportLineItem in transDo.TransportInfoList )
			{

				string identifier = transDo.TransID + "|T" + transportLineItem.TransportOrderNumber;
				var importKey = this.importKeysList.Find(x => x.Identifier == identifier);

				if ( importKey == null )
				{
					continue;
				}

				ImportResultsClass result = transportLineItemResults.Find(x => x.Identifier == importKey.Identifier && x.EntityType == "TransportLineItemID" && x.Section == SectionTypes.TransportLineItem);
				importKey.TransportLineItemDo.TransactionTransportLineItemGuid = result?.EntityGuid ?? Guid.Empty;
			}
		}

	    /// <summary>
	    /// The load results data set.
	    /// </summary>
	    /// <param name="dataSet">
	    /// The data set.
	    /// </param>
	    /// <returns>
	    /// The list of import results.
	    /// </returns>
	    private List<ImportResultsClass> LoadResultsDataSet(DataSet dataSet)
		{
			var resultsList = new List<ImportResultsClass>();

			if (dataSet != null && dataSet.Tables.Count > 0)
			{
				DataTable table = dataSet.Tables[0];

				if (table.Rows.Count > 0)
				{
					foreach (DataRow row in table.Rows)
					{
						var results = new ImportResultsClass
							              {
								              Section		= row.IsNull("Section") ? SectionTypes.None : (SectionTypes)row["Section"],
								              SiteGuid		= row.IsNull("SiteGuid") ? Guid.Empty : (Guid)row["SiteGuid"],
								              TransId		= row.IsNull("TransId") ? string.Empty : (string)row["TransId"],
								              EntityId		= row.IsNull("EntityId") ? string.Empty : (string)row["EntityId"],
								              EntityType	= row.IsNull("EntityType") ? string.Empty : (string)row["EntityType"],
								              EntityGuid	= row.IsNull("EntityGuid") ? Guid.Empty : (Guid)row["EntityGuid"],
											  Identifier	= row.IsNull("Identifier") ? string.Empty : (string)row["Identifier"]
							              };

						resultsList.Add(results);
					}
				}
			}

			return resultsList;
		}

		/// <summary>
		/// This method will build the header, line item, and sub-line item entries into the parameters table
		/// in order to be used to retrieve the appropriate GUIDs.
		/// </summary>
		/// <param name="transCollection">
		/// A collection of transaction data objects.
		/// </param>
		/// <param name="siteGuid">
		/// The site GUID.
		/// </param>
		private void BuildParametersTable(List<TransactionDO> transCollection, Guid siteGuid)
		{
			if (transCollection == null || transCollection.Count < 1)
			{
				return;
			}

			this.parametersTable = new DataTable();
			var column = new DataColumn("Section", typeof(int));
			this.parametersTable.Columns.Add(column);

			column = new DataColumn("SiteGuid", typeof(Guid));
			this.parametersTable.Columns.Add(column);

			column = new DataColumn("TransId", typeof(string));
			this.parametersTable.Columns.Add(column);

			column = new DataColumn("EntityId", typeof(string));
			this.parametersTable.Columns.Add(column);

			column = new DataColumn("EntityType", typeof(string));
			this.parametersTable.Columns.Add(column);

			column = new DataColumn("EntityGuid", typeof(Guid));
			this.parametersTable.Columns.Add(column);

			column = new DataColumn("Identifier", typeof(string));
			this.parametersTable.Columns.Add(column);

			foreach (TransactionDO transDo in transCollection)
			{
				transDo.SiteGuid = siteGuid;

				string identifier = transDo.TransID;
				var importKey = new ImportKeysClass { Identifier = identifier, TransactionDo = transDo };
				this.importKeysList.Add(importKey);

				DataRow row = this.parametersTable.NewRow();
				this.BuildRow(row, SectionTypes.Header, siteGuid, transDo.TransID, "TransID", transDo.TransID, identifier);
				this.parametersTable.Rows.Add(row);

				row = this.parametersTable.NewRow( );
				this.BuildRow(row, SectionTypes.Header, siteGuid, transDo.TransID, "AliasName", transDo.Alias, identifier);
				this.parametersTable.Rows.Add(row);

				row = this.parametersTable.NewRow( );
				this.BuildRow(row, SectionTypes.Header, siteGuid, transDo.TransID, "ShipToID", transDo.ShipToID, identifier);
				this.parametersTable.Rows.Add(row);

				row = this.parametersTable.NewRow( );
				this.BuildRow(row, SectionTypes.Header, siteGuid, transDo.TransID, "SupplierID", transDo.SupplierID, identifier);
				this.parametersTable.Rows.Add(row);

				row = this.parametersTable.NewRow( );
				this.BuildRow(row, SectionTypes.Header, siteGuid, transDo.TransID, "ShipperID", transDo.ShipperID, identifier);
				this.parametersTable.Rows.Add(row);

				row = this.parametersTable.NewRow( );
				this.BuildRow(row, SectionTypes.Header, siteGuid, transDo.TransID, "OwnerID", transDo.OwnerID, identifier);
				this.parametersTable.Rows.Add(row);

				row = this.parametersTable.NewRow( );
				this.BuildRow(row, SectionTypes.Header, siteGuid, transDo.TransID, "ManagerID", transDo.ManagerID, identifier);
				this.parametersTable.Rows.Add(row);

				row = this.parametersTable.NewRow( );
				this.BuildRow(row, SectionTypes.Header, siteGuid, transDo.TransID, "CarrierID", transDo.CarrierID, identifier);
				this.parametersTable.Rows.Add(row);

				row = this.parametersTable.NewRow( );
				this.BuildRow(row, SectionTypes.Header, siteGuid, transDo.TransID, "BillToID", transDo.BillToID, identifier);
				this.parametersTable.Rows.Add(row);

				row = this.parametersTable.NewRow( );
				this.BuildRow(row, SectionTypes.Header, siteGuid, transDo.TransID, "DestinationRegistrationID1", transDo.DestinationEQ1.RegistrationID, identifier);
				this.parametersTable.Rows.Add(row);

				row = this.parametersTable.NewRow( );
				this.BuildRow(row, SectionTypes.Header, siteGuid, transDo.TransID, "DestinationRegistrationID2", transDo.DestinationEQ2.RegistrationID, identifier);
				this.parametersTable.Rows.Add(row);

				row = this.parametersTable.NewRow( );
				this.BuildRow(row, SectionTypes.Header, siteGuid, transDo.TransID, "DestinationRegistrationID3", transDo.DestinationEQ3.RegistrationID, identifier);
				this.parametersTable.Rows.Add(row);

				row = this.parametersTable.NewRow( );
				this.BuildRow(row, SectionTypes.Header, siteGuid, transDo.TransID, "SourceRegistrationID1", transDo.SourceEQ1.RegistrationID, identifier);
				this.parametersTable.Rows.Add(row);

				row = this.parametersTable.NewRow( );
				this.BuildRow(row, SectionTypes.Header, siteGuid, transDo.TransID, "SourceRegistrationID2", transDo.SourceEQ2.RegistrationID, identifier);
				this.parametersTable.Rows.Add(row);

				row = this.parametersTable.NewRow( );
				this.BuildRow(row, SectionTypes.Header, siteGuid, transDo.TransID, "SourceRegistrationID3", transDo.SourceEQ3.RegistrationID, identifier);
				this.parametersTable.Rows.Add(row);

				row = this.parametersTable.NewRow( );
				this.BuildRow(row, SectionTypes.Header, siteGuid, transDo.TransID, "OperatorID", transDo.OperatorID, identifier);
				this.parametersTable.Rows.Add(row);

				row = this.parametersTable.NewRow( );
				this.BuildRow(row, SectionTypes.Header, siteGuid, transDo.TransID, "FuelCardID", transDo.FuelCardID, identifier);
				this.parametersTable.Rows.Add(row);

				row = this.parametersTable.NewRow( );
				this.BuildRow(row, SectionTypes.Header, siteGuid, transDo.TransID, "FinalStationID", transDo.RouteInfo.FinalStationIATAID, identifier);
				this.parametersTable.Rows.Add(row);

				row = this.parametersTable.NewRow( );
				this.BuildRow(row, SectionTypes.Header, siteGuid, transDo.TransID, "PreviousStationID", transDo.RouteInfo.PreviousStationIATAID, identifier);
				this.parametersTable.Rows.Add(row);

				row = this.parametersTable.NewRow( );
				this.BuildRow(row, SectionTypes.Header, siteGuid, transDo.TransID, "NextStationID", transDo.RouteInfo.NextStationIATAID, identifier);
				this.parametersTable.Rows.Add(row);

				row = this.parametersTable.NewRow( );
				this.BuildRow(row, SectionTypes.Header, siteGuid, transDo.TransID, "OriginStationID", transDo.RouteInfo.OriginStationIATAID, identifier);
				this.parametersTable.Rows.Add(row);
				
				if ( transDo.TransTypeID == TransactionTypes.T11_ConsumerTransfer )
				{
					row = this.parametersTable.NewRow( );
					this.BuildRow(row, SectionTypes.Header, siteGuid, transDo.TransID, "ToShipToID", ((ConsumerTransferDO) transDo).ToShipToID, identifier);
					this.parametersTable.Rows.Add(row);
				}

				if ( transDo.TransTypeID == TransactionTypes.T13_OwnerTransfer )
				{
					row = this.parametersTable.NewRow( );
					this.BuildRow(row, SectionTypes.Header, siteGuid, transDo.TransID, "ToOwnerID", transDo.ToOwnerID, identifier);
					this.parametersTable.Rows.Add(row);
				}

				if ( transDo.TransTypeID == TransactionTypes.T13_OwnerTransfer )
				{
					row = this.parametersTable.NewRow( );
					this.BuildRow(row, SectionTypes.Header, siteGuid, transDo.TransID, "ToManagerID", transDo.ToManagerID, identifier);
					this.parametersTable.Rows.Add(row);
				}

				if ( transDo.TransTypeID == TransactionTypes.T11_ConsumerTransfer )
				{
					row = this.parametersTable.NewRow( );
					this.BuildRow(row, SectionTypes.Header, siteGuid, transDo.TransID, "ToBillToID", ((ConsumerTransferDO) transDo).ToBillToID, identifier);
					this.parametersTable.Rows.Add(row);
				}

				// Build the line item table.
				this.BuildLineItemTable(transDo, siteGuid);

				// Build the transport line item table.
				this.BuildTransportLineItemTable(transDo, siteGuid);
			}
		}

		/// <summary>
		/// This method will build the line item table to be used to retrieve the 
		/// appropriate GUIDs.
		/// </summary>
		/// <param name="transDo">
		/// The transaction data object.
		/// </param>
		/// <param name="siteGuid">
		/// The site GUID.
		/// </param>
		private void BuildLineItemTable(TransactionDO transDo, Guid siteGuid)
		{
			if ( transDo.LineItems == null || transDo.LineItems.Count < 1 )
			{
				return;
			}

			int lineItemIndex = 1;

			foreach (LineItemDO lineItemDo in transDo.LineItems)
			{
				int identifierIndex = lineItemIndex++;

				// Use the Sequence ID of the line item if it is not null.
				if (lineItemDo.SequenceId != null)
				{
					identifierIndex = lineItemDo.SequenceId.Value;
				}

				string identifier = transDo.TransID + "|L" + identifierIndex;
				var importKey = new ImportKeysClass { Identifier = identifier, LineItemDo = lineItemDo };
				this.importKeysList.Add(importKey);

				DataRow row = this.parametersTable.NewRow( );
				this.BuildRow(row, SectionTypes.LineItem, siteGuid, transDo.TransID, "TransactionLineItemID", lineItemDo.TransactionLineItemGuid.ToString(), identifier);
				this.parametersTable.Rows.Add(row);

				row = this.parametersTable.NewRow( );
				this.BuildRow(row, SectionTypes.LineItem, siteGuid, transDo.TransID, "DestinationRegistrationID1", lineItemDo.DestinationEQ.CompanyEquipmentID, identifier);
				this.parametersTable.Rows.Add(row);

				row = this.parametersTable.NewRow( );
				this.BuildRow(row, SectionTypes.LineItem, siteGuid, transDo.TransID, "SourceRegistrationID1", lineItemDo.SourceEQ.CompanyEquipmentID, identifier);
				this.parametersTable.Rows.Add(row);

				row = this.parametersTable.NewRow( );
				this.BuildRow(row, SectionTypes.LineItem, siteGuid, transDo.TransID, "OperatorID", lineItemDo.OperatorID, identifier);
				this.parametersTable.Rows.Add(row);

				row = this.parametersTable.NewRow( );
				this.BuildRow(row, SectionTypes.LineItem, siteGuid, transDo.TransID, "AdditiveProfileID", lineItemDo.AdditiveProfileID, identifier);
				this.parametersTable.Rows.Add(row);

				row = this.parametersTable.NewRow( );
				this.BuildRow(row, SectionTypes.LineItem, siteGuid, transDo.TransID, "Product", lineItemDo.Product, identifier);
				this.parametersTable.Rows.Add(row);

				row = this.parametersTable.NewRow( );
				this.BuildRow(row, SectionTypes.LineItem, siteGuid, transDo.TransID, "DestinationCompartmentID", lineItemDo.DestinationCompartmentID, identifier);
				this.parametersTable.Rows.Add(row);

				row = this.parametersTable.NewRow( );
				this.BuildRow(row, SectionTypes.LineItem, siteGuid, transDo.TransID, "SourceCompartmentID", lineItemDo.SourceCompartmentID, identifier);
				this.parametersTable.Rows.Add(row);

				row = this.parametersTable.NewRow( );
				this.BuildRow(row, SectionTypes.LineItem, siteGuid, transDo.TransID, "StorageLocationID", lineItemDo.StorageLocationID, identifier);
				this.parametersTable.Rows.Add(row);

				row = this.parametersTable.NewRow( );
				this.BuildRow(row, SectionTypes.LineItem, siteGuid, transDo.TransID, "LoadingLocationID", lineItemDo.LoadingLocationID, identifier);
				this.parametersTable.Rows.Add(row);

				row = this.parametersTable.NewRow( );
				this.BuildRow(row, SectionTypes.LineItem, siteGuid, transDo.TransID, "MeterID", lineItemDo.MeterID, identifier);
				this.parametersTable.Rows.Add(row);

				if ( transDo.TransTypeID == TransactionTypes.T15_PrimaryRegrade || transDo.TransTypeID == TransactionTypes.T16_SecondaryRegrade )
				{
					row = this.parametersTable.NewRow( );
					this.BuildRow(row, SectionTypes.LineItem, siteGuid, transDo.TransID, "ToProduct", ((RegradeLineItemDO) lineItemDo).ToProduct, identifier);
					this.parametersTable.Rows.Add(row);
				}

				if ( transDo.TransTypeID == TransactionTypes.T23_StorageTransfer )
				{
					row = this.parametersTable.NewRow( );
					this.BuildRow(row, SectionTypes.LineItem, siteGuid, transDo.TransID, "ToStorageLocationID", ((StorageTransferLineItemDO) lineItemDo).ToStorageLocation, identifier);
					this.parametersTable.Rows.Add(row);
				}

				// Build the sub-line item table
				this.BuildSubLineItemTable(transDo, lineItemDo, identifier, siteGuid);
			}
		}

		/// <summary>
		/// This method will build the sub-line item table to be used to retrieve the 
		/// appropriate GUIDs.
		/// </summary>
		/// <param name="transDo">
		/// The transaction data object.
		/// </param>
		/// <param name="lineItemDo">
		/// The line item data object.
		/// </param>
		/// <param name="lineItemIdentifier">
		/// The line Item Identifier.
		/// </param>
		/// <param name="siteGuid">
		/// The site GUID.
		/// </param>
		private void BuildSubLineItemTable(TransactionDO transDo, LineItemDO lineItemDo, string lineItemIdentifier, Guid siteGuid)
		{
			if ( lineItemDo == null || lineItemDo.SubLineItems.Count < 1 )
			{
				return;
			}

			int subLineItemIndex = 1;

			foreach (SubLineItemDO subLineItemDo in lineItemDo.SubLineItems)
			{
				int identifierIndex = subLineItemIndex++;

				if (subLineItemDo.SequenceId != null)
				{
					identifierIndex = subLineItemDo.SequenceId.Value;
				}

				string identifier = lineItemIdentifier + "|S" + identifierIndex;
				var importKey = new ImportKeysClass { Identifier = identifier, SubLineItemDo = subLineItemDo };
				this.importKeysList.Add(importKey);

				DataRow row = this.parametersTable.NewRow( );
				this.BuildRow(row, SectionTypes.SubLineItem, siteGuid, transDo.TransID, "TransactionSubLineItemID", subLineItemDo.TransactionSubLineItemGuid.ToString( ), identifier);
				this.parametersTable.Rows.Add(row);

				row = this.parametersTable.NewRow( );
				this.BuildRow(row, SectionTypes.SubLineItem, siteGuid, transDo.TransID, "Product", subLineItemDo.Product, identifier);
				this.parametersTable.Rows.Add(row);

				row = this.parametersTable.NewRow( );
				this.BuildRow(row, SectionTypes.SubLineItem, siteGuid, transDo.TransID, "StorageLocationID", subLineItemDo.StorageLocationID, identifier);
				this.parametersTable.Rows.Add(row);

				row = this.parametersTable.NewRow( );
				this.BuildRow(row, SectionTypes.SubLineItem, siteGuid, transDo.TransID, "MeterID", subLineItemDo.MeterID, identifier);
				this.parametersTable.Rows.Add(row);
			}
		}

		/// <summary>
		/// The build transport line item table.
		/// </summary>
		/// <param name="transDo">
		/// The transaction data object.
		/// </param>
		/// <param name="siteGuid">
		/// The site GUID.
		/// </param>
		private void BuildTransportLineItemTable(TransactionDO transDo, Guid siteGuid)
		{
			if (transDo.TransportInfoList == null || transDo.TransportInfoList.Count < 1)
			{
				return;
			}

			foreach (TransportLineItemDO transportLineItem in transDo.TransportInfoList)
			{
				string identifier = transDo.TransID + "|T" + transportLineItem.TransportOrderNumber;
				var importKey = new ImportKeysClass { Identifier = identifier, TransportLineItemDo = transportLineItem };
				this.importKeysList.Add(importKey);

				DataRow row = this.parametersTable.NewRow( );
				this.BuildRow(row, SectionTypes.TransportLineItem, siteGuid, transDo.TransID, "TransportLineItemID", transportLineItem.TransactionTransportLineItemGuid.ToString(), identifier);
				this.parametersTable.Rows.Add(row);
			}
		}

		/// <summary>
		/// This method will populate one transaction header row with the entity
		/// information to retrieve the appropriate entity GUIDs.
		/// </summary>
		/// <param name="row">
		/// The row.
		/// </param>
		/// <param name="sectionType">
		/// The Section Type
		/// </param>
		/// <param name="siteGuid">
		/// The site GUID.
		/// </param>
		/// <param name="transId">
		/// The trans ID.
		/// </param>
		/// <param name="entityType">
		/// The entity type.
		/// </param>
		/// <param name="entityId">
		/// The entity ID.
		/// </param>
		/// <param name="identifier">
		/// The Identifier
		/// </param>
		private void BuildRow(DataRow row, SectionTypes sectionType, Guid siteGuid, string transId, string entityType, string entityId, string identifier)
		{
			row["Section"]		= sectionType;
			row["SiteGuid"]		= siteGuid;
			row["TransId"]		= transId;
			row["EntityType"]	= entityType;
			row["EntityId"]		= entityId;
			row["EntityGuid"]	= Guid.Empty;
			row["Identifier"]	= identifier;
		}
		#endregion
	}
	#endregion

	#region Import Results Class.
	/// <summary>
	/// The import results class.
	/// </summary>
	public class ImportResultsClass
	{
		#region Private data members
		/// <summary>
		/// The section.
		/// </summary>
		private TransactionImportProcessorClass.SectionTypes section;

		/// <summary>
		/// The site GUID.
		/// </summary>
		private Guid siteGuid;

		/// <summary>
		/// The transaction ID.
		/// </summary>
		private string transId;

		/// <summary>
		/// The entity ID.
		/// </summary>
		private string entityId;

		/// <summary>
		/// The entity type.
		/// </summary>
		private string entityType;

		/// <summary>
		/// The entity GUID.
		/// </summary>
		private Guid entityGuid;

		/// <summary>
		/// The identifier.
		/// </summary>
		private string identifier;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="ImportResultsClass"/> class.
		/// </summary>
		public ImportResultsClass()
		{
			this.Initialize();
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the section.
		/// </summary>
		public TransactionImportProcessorClass.SectionTypes Section
		{
			get { return this.section; }
			set { this.section = value; }
		}

		/// <summary>
		/// Gets or sets the site GUID.
		/// </summary>
		public Guid SiteGuid
		{
			get { return this.siteGuid; }
			set { this.siteGuid = value; }
		}

		/// <summary>
		/// Gets or sets the Transaction ID.
		/// </summary>
		public string TransId
		{
			get { return this.transId; }
			set { this.transId = value; }
		}

		/// <summary>
		/// Gets or sets the entity ID.
		/// </summary>
		public string EntityId
		{
			get { return this.entityId; }
			set { this.entityId = value; }
		}

		/// <summary>
		/// Gets or sets the entity type.
		/// </summary>
		public string EntityType
		{
			get { return this.entityType; }
			set { this.entityType = value; }
		}

		/// <summary>
		/// Gets or sets the entity GUID.
		/// </summary>
		public Guid EntityGuid
		{
			get { return this.entityGuid; }
			set { this.entityGuid = value; }
		}

		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		public string Identifier
		{
			get { return this.identifier; }
			set { this.identifier = value; }
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// The initialize.
		/// </summary>
		private void Initialize()
		{
			this.section	= TransactionImportProcessorClass.SectionTypes.None;
			this.siteGuid	= Guid.Empty;
			this.transId	= string.Empty;
			this.entityId	= string.Empty;
			this.entityType = string.Empty;
			this.entityGuid = Guid.Empty;
			this.identifier = string.Empty;
		}
		#endregion
	}
	#endregion

	#region Import Keys Class
	/// <summary>
	/// This class contains the identifier and the related transaction data object. It is used
	/// to matchup the GUID results to the right transaction, line item, and sub line item.
	/// </summary>
	public class ImportKeysClass
	{
		#region Private data members
		/// <summary>
		/// The identifier.
		/// </summary>
		private string identifier;

		/// <summary>
		/// The transaction data object.
		/// </summary>
		private TransactionDO transactionDo;

		/// <summary>
		/// The line item data object.
		/// </summary>
		private LineItemDO lineItemDo;

		/// <summary>
		/// The sub line item data object.
		/// </summary>
		private SubLineItemDO subLineItemDo;

		/// <summary>
		/// The transport line item data object.
		/// </summary>
		private TransportLineItemDO transportLineItemDo;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="ImportKeysClass"/> class.
		/// </summary>
		public ImportKeysClass()
		{
			this.Initialize();
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		public string Identifier
		{
			get { return this.identifier; }
			set { this.identifier = value; }
		}

		/// <summary>
		/// Gets or sets the transaction data object.
		/// </summary>
		public TransactionDO TransactionDo
		{
			get { return this.transactionDo; }
			set { this.transactionDo = value; }
		}

		/// <summary>
		/// Gets or sets the line item data object.
		/// </summary>
		public LineItemDO LineItemDo
		{
			get { return this.lineItemDo; }
			set { this.lineItemDo = value; }
		}

		/// <summary>
		/// Gets or sets the sub line item data object.
		/// </summary>
		public SubLineItemDO SubLineItemDo
		{
			get { return this.subLineItemDo; }
			set { this.subLineItemDo = value; }
		}

		/// <summary>
		/// Gets or sets the transport line item data object.
		/// </summary>
		public TransportLineItemDO TransportLineItemDo
		{
			get { return this.transportLineItemDo; }
			set { this.transportLineItemDo = value; }
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method initialize the object.
		/// </summary>
		private void Initialize()
		{
			this.identifier				= string.Empty;
			this.transactionDo			= null;
			this.lineItemDo				= null;
			this.subLineItemDo			= null;
			this.transportLineItemDo	= null;
		}
		#endregion
	}
	#endregion
}
