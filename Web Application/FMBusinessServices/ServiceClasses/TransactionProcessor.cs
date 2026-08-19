// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionProcessor.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the TransactionProcessorClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.IO;
	using System.Runtime.Serialization;
	using System.Text;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.ServiceRequests;

	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;

	using Microsoft.SqlServer.Server;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	using FMBusinessObjects.UtilityObjects;


	/// <summary>
	/// Retrieves a transaction record and its associated records such as line items or user data
	/// </summary>
	public class TransactionProcessorClass : ITransactionProcessor
    {
        /// <summary>
        /// The security.
        /// </summary>
        private SecurityClass security;

        /// <summary>
        /// The accounting site.
        /// </summary>
        private AccountingSite accountingSite;

        /// <summary>
        /// The process.
        /// </summary>
        /// <param name="sr">
        /// The service request.
        /// </param>
        /// <returns>
        /// The <see cref="TransactionDO"/>.
        /// </returns>
        /// <exception cref="AccountingServicesException">
        /// Failed to retrieve transaction.
        /// </exception>
        public TransactionDO Process(TransactionSR sr)
        {
            if ((!string.IsNullOrEmpty(sr.TransID) || sr.TransactionGuid != Guid.Empty) && sr.Transaction == null)
            {
                // Validate that the user has permission to retrieve the transaction.
                if (!sr.Security.HasRight(RIGHT.VIEW_TRANSACTION_DATA)
                    && !sr.Security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
                    && !sr.Security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
                    && !sr.Security.HasRight(RIGHT.MODIFY_DISPATCH) && !sr.Security.HasRight(RIGHT.VIEW_DISPATCH)
                    && !sr.Security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA) && !sr.Security.HasRight(RIGHT.BASE_EXPORT)
                    && !sr.Security.HasRight(RIGHT.BASE_EXPORT_MANUAL) && !sr.Security.HasRight(RIGHT.ENTERPRISE_EXPORT)
                    && !sr.Security.HasRight(RIGHT.MODIFY_INCOMING_TRUCK_DATA)
					&& !sr.Security.HasRight(RIGHT.IMPORT_TRANSACTION))
                {
                    string msg = "User " + sr.Security.UserID + " does not have permission to view transactions.";
                    throw new AccountingServicesException(msg);
                }

                this.security = sr.Security;

                TransactionDO trans = this.TransactionProcessorSelect(sr, sr.TransID);

                if (trans == null)
                {
                    return null;
                }

                if (sr.AllowCrossSiteTransactions == false)
                {
                    if (sr.AccountingSite == null)
                    {
                        var accountingSites = new AccountingSites();
                        this.accountingSite = accountingSites.LoadSiteInfo(sr.Security, sr.Security.SiteGuid);
                    }
                    else
                    {
                        this.accountingSite = sr.AccountingSite;
                    }

                    var permissions = new TransactionPermissions(this.accountingSite);

                    // Check that the transaction is for a site that the user is logged in to.
                    if (permissions.TransactionIsFromUserSites(trans) == false)
                    {
                        const string Msg = "Transaction not found.";
                        throw new AccountingServicesException(Msg);
                    }

                    // Check that the user is associated with a company that is a party to the transaction
                    // unless UserGuid is Empty which is a special case for Terminal Automation Services
                    // which has no associated user.
                    if (sr.Security.UserGuid != Guid.Empty && permissions.UserIsPartyToTransaction(trans) == false)
                    {
                        string msg = "User " + sr.Security.UserID + " is not a party to the transaction.";
                        throw new AccountingServicesException(msg);
                    }
                }

                // If requested, convert unit and time values from SI / UTC to the unit values defined for the product, transaction alias, or site
                if (sr.ConvertUnits)
                {
                    var converter = new TransactionUnitConverter(sr.Security, sr.Security.SiteGuid);
                    converter.ConvertFromSI(trans);
                }

                return trans;
            }

            return null;
        }

        /// <summary>
        /// Retrieve the previous (existing) version of the transaction from the database.
        /// This method is intended to be used to support processing performed by the save transactions processor
        /// which in some cases relies on "old" transaction data. 
        /// Instead of retrieving the entire transaction record, this method only retrieves the data the save transactions processor needs
        /// </summary>
        /// <param name="securityParam">Contains security information</param>
        /// <param name="transactionGuids">Identifies the transactions to retrieve</param>
        /// <returns>Data from the existing transaction records in the database. </returns>
        public Dictionary<Guid, TransactionPreviousVersionInformation> GetPreviousTransactionInformation(SecurityClass securityParam, List<Guid> transactionGuids)
        {
            // This method is only meant to be used by the save transactions processor and is not currently exposed via WCF.
            // However, we check the security rights anyway should it ever be used elsewhere
            if (!securityParam.HasRight(RIGHT.VIEW_TRANSACTION_DATA)
                && !securityParam.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
                && !securityParam.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
                && !securityParam.HasRight(RIGHT.MODIFY_DISPATCH) && !securityParam.HasRight(RIGHT.VIEW_DISPATCH)
                && !securityParam.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA) && !securityParam.HasRight(RIGHT.BASE_EXPORT)
                && !securityParam.HasRight(RIGHT.BASE_EXPORT_MANUAL) && !securityParam.HasRight(RIGHT.ENTERPRISE_EXPORT)
                && !securityParam.HasRight(RIGHT.MODIFY_INCOMING_TRUCK_DATA)
				&& !securityParam.HasRight(RIGHT.IMPORT_TRANSACTION))
            {
                string msg = "User " + securityParam.UserID + " does not have permission to view transactions.";
                throw new AccountingServicesException(msg);
            }

            var oldVersionsOfTransactions = new Dictionary<Guid, TransactionPreviousVersionInformation>();

            transactionGuids.RemoveAll(transactionGuid => transactionGuid == Guid.Empty);

            // Don't do any work if all the transactions have an empty guid (i.e. are new transactions)
            if (transactionGuids.Count == 0)
            {
                return oldVersionsOfTransactions;
            }

            var consolidatedDa = new ConsolidatedDAClass();

            using (var cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_TransactionGetPreviousVersionInformation";

                SqlParameter tableValuedParameter = cmd.Parameters.Add("@TransactionGuids", SqlDbType.Structured);
                tableValuedParameter.Value = CreateSqlDataRecordsForGetPreviousTransactionInformation(transactionGuids);
                tableValuedParameter.TypeName = "dbo.TransactionGuidListType";

                DataSet results = consolidatedDa.GetDataSet(cmd, this.security);

                if (results.Tables.Count <= 0 || results.Tables[0] == null || results.Tables[0].Rows.Count < 1)
                {
                    return oldVersionsOfTransactions;
                }

                foreach (DataRow row in results.Tables[0].Rows)
                {
                    Guid transactionGuid = DataObject.getGuid(row["TransactionGuid"]);
                     
                    var previousVersionOfTransaction = new TransactionPreviousVersionInformation
                    {
                        DeleteFlag				= DataObject.getValue(row["DeleteFlag"], false),
                        HasWeightReadings		= DataObject.getValue(row["HasWeightReadings"], false),
                        TransVersion			= DataObject.getLong(row["TransVersion"]),
                        Status					= DataObject.getValue(row["LookupTransactionStatusIndex"], TransactionStatus.Completed),
						InventoryDate			= DataObject.getValue(row["InventoryDate"], new DateTime()),
                        AssociatedTransactions	= new List<AssociatedTxDO>()
                    };

                    if (!oldVersionsOfTransactions.ContainsKey(transactionGuid))
                    {
                        oldVersionsOfTransactions.Add(transactionGuid, previousVersionOfTransaction);
                    }
                }

                this.GetPreviousTransactionLinks(transactionGuids, oldVersionsOfTransactions);

                return oldVersionsOfTransactions;
            }
        }

        /// <summary>
        /// Retrieve the existing associations (links) from the database.
        /// The save transactions processor needs the existing associations 
        /// to detemine which links to delete and which to create.
        /// </summary>
        /// <param name="transactionGuids">Identifies the transactions to get links for</param>
        /// <param name="previousTransactionVersionInformation">Details about the existing version of the transactions in the DB. 
        /// This will be updated with associated transaction information.
        /// </param>
        /// <returns>A list of links for the specified transaction. The list will be empty if none exist.</returns>
        private void GetPreviousTransactionLinks(List<Guid> transactionGuids, Dictionary<Guid, TransactionPreviousVersionInformation> previousTransactionVersionInformation)
        {
            // Don't do any work if all the transactions have an empty guid (i.e. are new transactions)
            if (transactionGuids.Count == 0)
            {
                return;
            }

            using (var cmd = new SqlCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_TransactionLinksGetList";

                SqlParameter tableValuedParameter = cmd.Parameters.Add("@TransactionGuids", SqlDbType.Structured);
                tableValuedParameter.Value = CreateSqlDataRecordsForGetPreviousTransactionInformation(transactionGuids);
                tableValuedParameter.TypeName = "dbo.TransactionGuidListType";

                var consolidatedDa = new ConsolidatedDAClass();
                DataSet ds = consolidatedDa.GetDataSet(cmd, this.security);

                if (ds.Tables.Count <= 0 || ds.Tables[0] == null || ds.Tables[0].Rows.Count < 1)
                {
                    return;
                }

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Guid transactionGuid = DataObject.getGuid(row["TransactionGuid"]);

                    TransactionPreviousVersionInformation oldVersionOfTransaction;

                    if (previousTransactionVersionInformation.TryGetValue(transactionGuid, out oldVersionOfTransaction) && oldVersionOfTransaction != null)
                    { 
                        var associatedTransaction = new AssociatedTxDO
                        {
                            // The assignment of the LineItemGuids appears backwards. However, this is the way the associated transaction information is retrieved -
                            // the TransactionLineItemGuid column returned is actually the LinkedTransactionLineItemGuid value from the DB.
                            // See the usp_TransactionLinksGet procedure.
                            TransactionLineItemGuid = DataObject.getValue(row["LinkedTransactionLineItemGuid"], Guid.Empty),
                            LinkedTransactionLineItemGuid = DataObject.getValue(row["TransactionLineItemGuid"], Guid.Empty),
                            Associated = 1,
                            TransID = DataObject.getValue(row["LinkedTransID"], string.Empty)
                        };

                        oldVersionOfTransaction.AssociatedTransactions.Add(associatedTransaction);
                    }
                }
            }
        }

        /// <summary>
        /// Create SqlDataRecords with TransactionGuids to call the stored procs used by GetPreviousTransactionInformation()
        /// </summary>
        /// <param name="transactionGuids">Identifies transactions we want to get the previous versions of</param>
        /// <returns>SqlDataRecords populated with TransactionGuids to call the stored procs used by GetPreviousTransactionInformation()</returns>
        private static IEnumerable<SqlDataRecord> CreateSqlDataRecordsForGetPreviousTransactionInformation(IEnumerable<Guid> transactionGuids)
        {
            var metaData = new SqlMetaData[1];

            metaData[0] = new SqlMetaData("TransactionGuid", SqlDbType.UniqueIdentifier);

            var record = new SqlDataRecord(metaData);

            foreach (Guid transactionGuid in transactionGuids)
            {
                record.SetGuid(0, transactionGuid);

                yield return record;
            }
        }

		/// <summary>
		/// The transaction processor select.
		/// </summary>
		/// <param name="sr">
		/// The service request.
		/// </param>
		/// <param name="transId">
		/// The transaction ID.
		/// </param>
		/// <returns>
		/// The <see cref="TransactionDO"/>.
		/// </returns>
		private TransactionDO TransactionProcessorSelect(TransactionSR sr, string transId)
		{
			TransactionDO originalTrans = this.SelectTransaction(sr.TransactionGuid, transId, isOriginalTransaction: true);

			// If the Transaction isn't found, don't continue processing
			if (originalTrans == null)
			{
				return null;
			}

			if (originalTrans.ConjoinedTransactionGuid != Guid.Empty)
			{
				TransactionDO conjoinedTrans = 
					this.SelectTransaction(originalTrans.ConjoinedTransactionGuid, originalTrans.ConjoinedTransID, isOriginalTransaction: false);

				originalTrans = this.SetTransferFields(originalTrans, conjoinedTrans);
			}

			this.GetSpecialInstructions(originalTrans);

			return originalTrans;
		}

		/// <summary>
		/// The select transaction.
		/// </summary>
		/// <param name="transGuid">
		/// The transaction GUID.
		/// </param>
		/// <param name="transId">
		/// The transaction ID.
		/// </param>
		/// <param name="isOriginalTransaction">
		/// The is Original Transaction.
		/// </param>
		/// <returns>
		/// The <see cref="TransactionDO"/>.
		/// </returns>
		private TransactionDO SelectTransaction(Guid transGuid, string transId, bool isOriginalTransaction)
		{
			var trans = new TransactionDO();

			if (transGuid != Guid.Empty)
			{
				trans.TransactionGuid = transGuid;
			}
			else
			{
				trans.TransID = transId;
			}

			trans = this.RetrieveHeader(trans, isOriginalTransaction);

			if (trans == null)
			{
				return null;
			}

			// Added to fix CSI #5932 and #5933. (02-Jun-2008 IGO)
			if (trans.TransTypeID == TransactionTypes.T5_PrimaryDisbursement || trans.TransTypeID == TransactionTypes.T25_Shipment)
			{
				this.RetrievePidxCollection(trans);
			}

			this.RetrieveLineItems(trans);
			this.RetrieveSubLineItems(trans);
			this.RetrieveWeightReadings(trans);
			this.RetrieveTransportLineItems(trans);
			this.RetrieveAssociatedTransactions(trans);
			this.RetrieveCloseoutDates(trans);
			this.RetrieveExternalInterfaceData(trans);

			if (trans.TransTypeID == TransactionTypes.T17_Order || trans.TransTypeID == TransactionTypes.T18_SupplyOrder)
			{
				this.GetOrderQuantities(trans);
			}

			this.SetQuantityAndValueReceived(trans);

			return trans;
		}

		/// <summary>
		/// The set transfer fields.
		/// </summary>
		/// <param name="originalTrans">
		/// The original trans.
		/// </param>
		/// <param name="conjoinedTrans">
		/// The conjoined trans.
		/// </param>
		/// <returns>
		/// The <see cref="TransactionDO"/>.
		/// </returns>
		private TransactionDO SetTransferFields(TransactionDO originalTrans, TransactionDO conjoinedTrans)
		{
			var consumerTransfer	= originalTrans as ConsumerTransferDO;
			var ownerTransfer		= originalTrans as OwnerTransferDO;
			var regrade				= originalTrans as RegradeDO;
			var storageTransfer		= originalTrans as StorageTransferDO;

			originalTrans.ConjoinedTransactionGuid	= conjoinedTrans.TransactionGuid;
		    originalTrans.ConjoinedNotesGuid		= conjoinedTrans.TransactionNoteGuid;
		    originalTrans.ConjoinedUserDataGuid		= conjoinedTrans.TransactionUserDataGuid;
		    originalTrans.ConjoinedSignatureGuid	= conjoinedTrans.TransactionSignatureGuid;

			if (consumerTransfer != null)
			{
				if (originalTrans.SubType.ToUpper().Equals(TransactionDO.DEBIT))
				{
					// The trans object contains the FROM and the conjoin transfer object contains
					// the TO. Copy the conjoin into the TO fields.
					consumerTransfer.ToBillToCode			= conjoinedTrans.BillToCode;
					consumerTransfer.ToBillToID				= conjoinedTrans.BillToID;
					consumerTransfer.ToBillToCompanyGuid	= conjoinedTrans.BillToCompanyGuid;
					consumerTransfer.ToShipToCode			= conjoinedTrans.ShipToCode;
					consumerTransfer.ToShipToID				= conjoinedTrans.ShipToID;
					consumerTransfer.ToShipToCompanyGuid	= conjoinedTrans.ShipToCompanyGuid;
				}
				else
				{
					// For a credit the trans object contains the TO data and the conjoin
					// contains the FROM data. This needs to be reversed so that the FROM
					// really contains the original FROM data and the TO contains the original
					// TO data.
					string toBillToCode = originalTrans.BillToCode;
					string toBillToId	= originalTrans.BillToID;
					Guid toBillToGuid	= originalTrans.BillToCompanyGuid;
					string toShipToCode = originalTrans.ShipToCode;
					string toShipToId	= originalTrans.ShipToID;
					Guid toShipToGuid	= originalTrans.ShipToCompanyGuid;

					originalTrans.BillToID			= conjoinedTrans.BillToID;
					originalTrans.BillToCode		= conjoinedTrans.BillToID;
					originalTrans.BillToCompanyGuid = conjoinedTrans.BillToCompanyGuid;
					originalTrans.ShipToID			= conjoinedTrans.ShipToID;
					originalTrans.ShipToCode		= conjoinedTrans.ShipToCode;
					originalTrans.ShipToCompanyGuid = conjoinedTrans.ShipToCompanyGuid;

					consumerTransfer.ToBillToCode			= toBillToCode;
					consumerTransfer.ToBillToID				= toBillToId;
					consumerTransfer.ToBillToCompanyGuid	= toBillToGuid;
					consumerTransfer.ToShipToCode			= toShipToCode;
					consumerTransfer.ToShipToID				= toShipToId;
					consumerTransfer.ToShipToCompanyGuid	= toShipToGuid;
				}
			}
			else if (ownerTransfer != null)
			{
				if (originalTrans.SubType.ToUpper().Equals(TransactionDO.DEBIT))
				{
					// The trans object contains the FROM and the conjoin transfer object contains
					// the TO. Copy the conjoin into the TO fields.
					ownerTransfer.ToCarrierCode			= conjoinedTrans.CarrierCode;
					ownerTransfer.ToCarrierID			= conjoinedTrans.CarrierID;
					ownerTransfer.ToCarrierCompanyGuid	= conjoinedTrans.CarrierCompanyGuid;
					ownerTransfer.ToManagerCode			= conjoinedTrans.ManagerCode;
					ownerTransfer.ToManagerID			= conjoinedTrans.ManagerID;
					ownerTransfer.ToManagerCompanyGuid	= conjoinedTrans.ManagerCompanyGuid;
					ownerTransfer.ToOwnerCode			= conjoinedTrans.OwnerCode;
					ownerTransfer.ToOwnerID				= conjoinedTrans.OwnerID;
					ownerTransfer.ToOwnerCompanyGuid	= conjoinedTrans.OwnerCompanyGuid;

					if (conjoinedTrans.LineItems != null)
					{
						for (int nextItem = 0; nextItem < conjoinedTrans.LineItems.Count; nextItem++)
						{
							var lineItem = conjoinedTrans.LineItems[nextItem] as LineItemDO;
							var storageLineItem = ownerTransfer.LineItems[nextItem] as StorageTransferLineItemDO;

							storageLineItem.ToStorageLocation = lineItem.StorageLocationID;
							storageLineItem.ToStorageLocationTankGuid = lineItem.StorageLocationTankGuid;
						}
					}
				}
				else
				{
					// For a credit the trans object contains the TO data and the conjoin
					// contains the FROM data. This needs to be reversed so that the FROM
					// really contains the original FROM data and the TO contains the original
					// TO data.
					string toManagerId		= originalTrans.ManagerID;
					string toManagerCode	= originalTrans.ManagerCode;
					Guid toManagerGuid		= originalTrans.ManagerCompanyGuid;
					string toOwnerId		= originalTrans.OwnerID;
					string toOwnerCode		= originalTrans.OwnerCode;
					Guid toOwnerGuid		= originalTrans.OwnerCompanyGuid;
					string toCarrierId		= originalTrans.CarrierID;
					string toCarrierCode	= originalTrans.CarrierCode;
					Guid toCarrierGuid		= originalTrans.CarrierCompanyGuid;

					originalTrans.ManagerID				= conjoinedTrans.ManagerID;
					originalTrans.ManagerCode			= conjoinedTrans.ManagerCode;
					originalTrans.ManagerCompanyGuid	= conjoinedTrans.ManagerCompanyGuid;
					originalTrans.OwnerID				= conjoinedTrans.OwnerID;
					originalTrans.OwnerCode				= conjoinedTrans.OwnerCode;
					originalTrans.OwnerCompanyGuid		= conjoinedTrans.OwnerCompanyGuid;
					originalTrans.CarrierID				= conjoinedTrans.CarrierID;
					originalTrans.CarrierCode			= conjoinedTrans.CarrierCode;
					originalTrans.CarrierCompanyGuid	= conjoinedTrans.CarrierCompanyGuid;

					ownerTransfer.ToManagerID			= toManagerId;
					ownerTransfer.ToManagerCode			= toManagerCode;
					ownerTransfer.ToManagerCompanyGuid	= toManagerGuid;
					ownerTransfer.ToOwnerID				= toOwnerId;
					ownerTransfer.ToOwnerCode			= toOwnerCode;
					ownerTransfer.ToOwnerCompanyGuid	= toOwnerGuid;
					ownerTransfer.ToCarrierID			= toCarrierId;
					ownerTransfer.ToCarrierCode			= toCarrierCode;
					ownerTransfer.ToCarrierCompanyGuid	= toCarrierGuid;

					if (originalTrans.LineItems != null)
					{
						for (int nextItem = 0; nextItem < originalTrans.LineItems.Count; nextItem++)
						{
							// Get the storage location information
							var lineItem = originalTrans.LineItems[nextItem] as LineItemDO;
							string toStorage = lineItem.StorageLocationID;
							Guid toStorageIndex = lineItem.StorageLocationTankGuid;

							// Populate the TO fields
							var conjoinedLineItem = conjoinedTrans.LineItems[nextItem] as LineItemDO;
							lineItem.StorageLocationID = conjoinedLineItem.StorageLocationID;
							lineItem.StorageLocationTankGuid = conjoinedLineItem.StorageLocationTankGuid;

							// Populate the StorageTransfer fields
							var storageTransferLineItem = ownerTransfer.LineItems[nextItem] as StorageTransferLineItemDO;
							storageTransferLineItem.ToStorageLocation = toStorage;
							storageTransferLineItem.ToStorageLocationTankGuid = toStorageIndex;
						}
					}
				}
			}
			else if (regrade != null)
			{
				if (originalTrans.SubType.ToUpper().Equals(TransactionDO.DEBIT))
				{
					// The trans object contains the FROM and the conjoin transfer object contains
					// the TO. Copy the conjoin into the TO fields.
					for (int next = 0; next < conjoinedTrans.LineItems.Count; next++)
					{
						LineItemDO lineItem = conjoinedTrans.LineItems[next];
						var regradeLineItem = regrade.LineItems[next] as RegradeLineItemDO;

						if (regradeLineItem != null)
						{
							regradeLineItem.ToProduct = lineItem.Product;
							regradeLineItem.ToProductCode = lineItem.ProductCode;
							regradeLineItem.ToProductType = lineItem.ProductType;
							regradeLineItem.ToProductGuid = lineItem.ProductGuid;

							regradeLineItem.ToStorageLocation = lineItem.StorageLocationID;
							regradeLineItem.ToStorageLocationTankGuid = lineItem.StorageLocationTankGuid;
						}
					}
				}
				else
				{
					// If the transaction is a credit, then ensure that the TO product and
					// FROM product remain consistent with that of the debit transaction.
					for (int idx = 0; idx < originalTrans.LineItems.Count; ++idx)
					{
						// Save off the transaction product info into a string.
						LineItemDO lineItem				= originalTrans.LineItems[idx];
						string toProduct				= lineItem.Product;
						string toProductCode			= lineItem.ProductCode;
						string toProductType			= lineItem.ProductType;
						Guid toProductGuid				= lineItem.ProductGuid;
						string toStorageLocation		= lineItem.StorageLocationID;
						Guid toStorageLocationTankGuid	= lineItem.StorageLocationTankGuid;

						// Take the conjoined which is the FROM in the transaction and
						// replace the product info.
						LineItemDO conjoinedLineItem		= conjoinedTrans.LineItems[idx];
						lineItem.Product					= conjoinedLineItem.Product;
						lineItem.ProductType				= conjoinedLineItem.ProductType;
						lineItem.ProductCode				= conjoinedLineItem.ProductCode;
						lineItem.ProductGuid				= conjoinedLineItem.ProductGuid;
						lineItem.StorageLocationID			= conjoinedLineItem.StorageLocationID;
						lineItem.StorageLocationTankGuid	= conjoinedLineItem.StorageLocationTankGuid;

						// Take the saved product TO info and place it in the regrade TO product
						// info.
						var regradeLineItem = regrade.LineItems[idx] as RegradeLineItemDO;

						if (regradeLineItem != null)
						{
							regradeLineItem.ToProduct					= toProduct;
							regradeLineItem.ToProductType				= toProductType;
							regradeLineItem.ToProductCode				= toProductCode;
							regradeLineItem.ToProductGuid				= toProductGuid;
							regradeLineItem.ToStorageLocation			= toStorageLocation;
							regradeLineItem.ToStorageLocationTankGuid	= toStorageLocationTankGuid;
						}
					}
				}
			}
			else if (storageTransfer != null)
			{
				if (originalTrans.SubType.ToUpper().Equals(TransactionDO.DEBIT))
				{
					// The trans object contains the FROM and the conjoined object contains
					// the TO.  Copy the conjoin to the TO fields
					for (int nextItem = 0; nextItem < conjoinedTrans.LineItems.Count; nextItem++)
					{
						LineItemDO lineItem = conjoinedTrans.LineItems[nextItem];
						var storageLineItem = storageTransfer.LineItems[nextItem] as StorageTransferLineItemDO;

						if (storageLineItem != null)
						{
							storageLineItem.ToStorageLocation = lineItem.StorageLocationID;
							storageLineItem.ToStorageLocationTankGuid = lineItem.StorageLocationTankGuid;
						}
					}
				}
				else
				{
					// The transaction is a credit.
					for (int nextItem = 0; nextItem < originalTrans.LineItems.Count; nextItem++)
					{
						// Get the storage location information
						LineItemDO lineItem				= originalTrans.LineItems[nextItem];
						string toStorage				= lineItem.StorageLocationID;
						Guid toStorageLocationTankGuid	= lineItem.StorageLocationTankGuid;

						// Populate the TO fields
						LineItemDO conjoinedLineItem = conjoinedTrans.LineItems[nextItem];
						lineItem.StorageLocationID = conjoinedLineItem.StorageLocationID;
						lineItem.StorageLocationTankGuid = conjoinedLineItem.StorageLocationTankGuid;

						// Populate the StorageTransfer fields
						var storageTransferLineItem = storageTransfer.LineItems[nextItem] as StorageTransferLineItemDO;

						if (storageTransferLineItem != null)
						{
							storageTransferLineItem.ToStorageLocation = toStorage;
							storageTransferLineItem.ToStorageLocationTankGuid = toStorageLocationTankGuid;
						}
					}
				}
			}

			// Set the original transaction line item conjoined GUID to the 
			// conjoined transaction line item GUID.
			foreach (LineItemDO originalLineItem in originalTrans.LineItems)
			{
				if (originalLineItem.SequenceId != null)
				{
					LineItemDO conjoinedLineItem = conjoinedTrans.LineItems.Find(x => x.SequenceId == originalLineItem.SequenceId);

					if (conjoinedLineItem != null)
					{
						originalLineItem.ConjoinedTransactionLineItemGuid = conjoinedLineItem.TransactionLineItemGuid;
					    originalLineItem.ConjoinedTransactionLineItemUserDataGuid = conjoinedLineItem.TransactionLineItemUserDataGuid;

						// Set the conjoined sub line item GUIDs.
						this.SetSubLineItems(originalLineItem, conjoinedLineItem);
					}
				}
				else
				{
					originalLineItem.ConjoinedTransactionLineItemGuid = Guid.Empty;
				    originalLineItem.ConjoinedTransactionLineItemUserDataGuid = Guid.Empty;

					foreach (SubLineItemDO subLineItem in originalLineItem.SubLineItems)
					{
						subLineItem.ConjoinedTransactionSubLineItemGuid = Guid.Empty;
					}
				}
			}

			return originalTrans;
		}

		/// <summary>
		/// This method will set the original sub line item conjoined GUID to the conjoined sub line item GUID.
		/// </summary>
		/// <param name="originalLineItem">Original Line Item object.</param>
		/// <param name="conjoinedLineItem">Conjoined Line Item object.</param>
		private void SetSubLineItems(LineItemDO originalLineItem, LineItemDO conjoinedLineItem)
		{
			foreach (SubLineItemDO originalSubLineItem in originalLineItem.SubLineItems)
			{
				if (originalSubLineItem.SequenceId != null)
				{
					SubLineItemDO conjoinedSubLineItem =
						conjoinedLineItem.SubLineItems.Find(x => x.SequenceId == originalSubLineItem.SequenceId);

					if (conjoinedSubLineItem != null)
					{
						originalSubLineItem.ConjoinedTransactionSubLineItemGuid = conjoinedSubLineItem.TransactionSubLineItemGuid;
					}
				}
				else
				{
					originalSubLineItem.ConjoinedTransactionSubLineItemGuid = Guid.Empty;
				}
			}
		}

		/// <summary>
		/// The retrieve header.
		/// </summary>
		/// <param name="trans">
		/// The transaction.
		/// </param>
		/// <param name="isOriginalTransaction">
		/// The is Original Transaction.
		/// </param>
		/// <returns>
		/// The <see cref="TransactionDO"/>.
		/// </returns>
		/// <exception cref="AccountingServicesException">
		/// Error retrieving transaction header.
		/// </exception>
		private TransactionDO RetrieveHeader(TransactionDO trans, bool isOriginalTransaction)
		{
			var consolidatedDa = new ConsolidatedDAClass();

			using (var cmd = new SqlCommand())
			{
			    cmd.CommandType = CommandType.StoredProcedure;
			    cmd.CommandText = "usp_TransactionHeaderNotesUserDataSignatureGet";

                if (trans.TransactionGuid != Guid.Empty)
				{
					cmd.Parameters.Add("@TransactionGuid", SqlDbType.UniqueIdentifier).Value = trans.TransactionGuid;
				}
				else
				{
					cmd.Parameters.Add("@TransID", SqlDbType.NVarChar, 64).Value = trans.TransID;
				}

				DataSet ds = consolidatedDa.GetDataSet(cmd, this.security);

				if (ds.Tables[0].Rows.Count < 1)
				{
					return null;
				}

				DataRow row = ds.Tables[0].Rows[0];

				var typeId = (TransactionTypes)Enum.Parse(typeof(TransactionTypes), row["LookupTransTypeIndex"].ToString());

				switch (typeId)
				{
					case TransactionTypes.T11_ConsumerTransfer:
						trans = new ConsumerTransferDO();
						break;
					case TransactionTypes.T13_OwnerTransfer:
						trans = new OwnerTransferDO();
						break;
					case TransactionTypes.T15_PrimaryRegrade:
					case TransactionTypes.T16_SecondaryRegrade:
						trans = new RegradeDO();
						break;
					case TransactionTypes.T23_StorageTransfer:
						trans = new StorageTransferDO();
						break;
				}

				trans.Alias = DataObject.getString(row["AliasName"]);
				trans.TransactionAliasGuid = DataObject.getGuid(row["TransactionAliasGuid"]);
				trans.BillToCode = DataObject.getString(row["BillToCode"]);
				trans.BillToID = DataObject.getString(row["BillToID"]);
				trans.BillToCompanyGuid = DataObject.getGuid(row["BillToCompanyGuid"]);
				trans.CarrierCode = DataObject.getString(row["CarrierCode"]);
				trans.CarrierID = DataObject.getString(row["CarrierID"]);
				trans.CarrierCompanyGuid = DataObject.getGuid(row["CarrierCompanyGuid"]);
				trans.ConjoinedTransID = DataObject.getString(row["ConjoinTransID"]);
				trans.CreatedBy = DataObject.getString(row["CreatedBy"]);
				trans.UpdatedBy = DataObject.getString(row["UpdatedBy"]);
				trans.DeleteFlag = DataObject.getBool(row["DeleteFlag"]);
				trans.DocumentNumber = DataObject.getString(row["DocumentNumber"]);
				trans.DriverIDNumber = DataObject.getString(row["DriverIdentificationNumber"]);
				trans.EstimatedFuelingDuration = DataObject.getOptionalInt(row["EstimatedFuelingDuration"]);
				trans.InventoryDate = DataObject.getValue<DateTime>(row["InventoryDate"], DateTime.Now);
				trans.LinkedDocumentNumber = DataObject.getString(row["LinkedDocumentNumber"]);
				trans.LoadID = DataObject.getString(row["LoadID"]);
				trans.ManagerCode = DataObject.getString(row["ManagerCode"]);
				trans.ManagerID = DataObject.getString(row["ManagerID"]);
				trans.ManagerCompanyGuid = DataObject.getGuid(row["ManagerCompanyGuid"]);
				trans.OperatorID = DataObject.getString(row["OperatorID"]);
				trans.OperatorPersonnelGuid = DataObject.getGuid(row["OperatorPersonnelGuid"]);
				trans.OperatorName = DataObject.getString(row["OperatorName"]);
				trans.OwnerCode = DataObject.getString(row["OwnerCode"]);
				trans.OwnerID = DataObject.getString(row["OwnerID"]);
				trans.OwnerCompanyGuid = DataObject.getGuid(row["OwnerCompanyGuid"]);
				trans.PONumber = DataObject.getString(row["PONumber"]);
				trans.RequestedDeliveryDate = DataObject.getOptionalDateTimeOffset(row["RequestedDeliveryDate"]);
				trans.ReversalType = row.IsNull("ReversalType") ? "" : DataObject.getString(row["ReversalType"]);
				trans.ReversedTransID = DataObject.getString(row["ReversedTransID"]);
				trans.SCACCode = DataObject.getString(row["SCACCode"]);
				trans.ShipmentNumber = DataObject.getString(row["ShipmentNumber"]);
				trans.ShipperCode = DataObject.getString(row["ShipperCode"]);
				trans.ShipperID = DataObject.getString(row["ShipperID"]);
				trans.ShipperCompanyGuid = DataObject.getGuid(row["ShipperCompanyGuid"]);
				trans.ShippingDocumentNumber = DataObject.getString(row["ShippingDocumentNumber"]);
				trans.ShipToCode = DataObject.getString(row["ShipToCode"]);
				trans.ShipToID = DataObject.getString(row["ShipToID"]);
				trans.ShipToCompanyGuid = DataObject.getGuid(row["ShipToCompanyGuid"]);
				trans.Site = DataObject.getString(row["Site"]);
				trans.SiteGuid = DataObject.getGuid(row["SiteGuid"]);
				trans.Status = DataObject.getValue(row["LookupTransactionStatusIndex"], TransactionStatus.Completed);
				trans.SubType = DataObject.getString(row["SubType"]);
				trans.SupplierCode = DataObject.getString(row["SupplierCode"]);
				trans.SupplierID = DataObject.getString(row["SupplierID"]);
				trans.SupplierCompanyGuid = DataObject.getGuid(row["SupplierCompanyGuid"]);
				trans.TicketMode = DataObject.getValue<TicketModes>(row["TicketMode"], TicketModes.Unknown);
				trans.TicketSource = DataObject.getString(row["TicketSource"]);
				trans.TimeEnd = DataObject.getOptionalDateTimeOffset(row["TimeEnd"]);
				trans.TimeIn = DataObject.getOptionalDateTimeOffset(row["TimeIn"]);
				trans.TimeOut = DataObject.getOptionalDateTimeOffset(row["TimeOut"]);
				trans.TransactionDateTime = DataObject.getOptionalDateTimeOffset(row["TransDateTime"]);
				trans.TransID = DataObject.getString(row["TransID"]);
				trans.TransactionGuid = DataObject.getGuid(row["TransactionGuid"]);
				trans.TransRefID = DataObject.getString(row["TransReferenceID"]);
				trans.TransTypeID = DataObject.getValue(row["LookupTransTypeIndex"], TransactionTypes.TransactionType_None);
				trans.TransVersion = DataObject.getLong(row["TransVersion"]);
				trans.FuelAdditiveFlag = DataObject.getBool(row["FuelAdditiveFlag"]);
				trans.IssuePoint = DataObject.getString(row["IssuePoint"]);
				trans.IssuePointNumber = DataObject.getString(row["IssuePointNumber"]);
				trans.RadioNumber = DataObject.getString(row["RadioNumber"]);
				trans.GateID = DataObject.getString(row["GateID"]);
				trans.GateGuid = DataObject.getGuid(row["GateGuid"]);

				trans.PaymentInfo.BillTo = DataObject.getString(row["BillToID"]);
				trans.PaymentInfo.CashAmount = DataObject.getOptionalDouble(row["CashAmount"]);
				trans.PaymentInfo.CashCurrencyType = null;
				trans.PaymentInfo.CreditCardAmount = DataObject.getOptionalDouble(row["CreditAmount"]);
				trans.PaymentInfo.CreditCardCurrencyType = null;
				trans.PaymentInfo.CreditCardExpiration = DataObject.getOptionalDateTimeOffset(row["CardExpiration"]);
				trans.PaymentInfo.CreditCardName = DataObject.getString(row["CardName"]);
				trans.PaymentInfo.CreditCardNumber = DataObject.getString(row["CardNumber"]);
				trans.PaymentInfo.CreditCardType = DataObject.getString(row["CardType"]);

				trans.RouteInfo.FinalStationIATAGuid = DataObject.getGuid(row["FinalStationIATAGuid"]);
				trans.RouteInfo.FinalStationIATAID = DataObject.getString(row["FinalStationIATAID"]);
				trans.RouteInfo.InternationalRouteIndicator = DataObject.getBool(row["InternationalRouteIndicator"]);
				trans.RouteInfo.NextStationIATAGuid = DataObject.getGuid(row["NextStationIATAGuid"]);
				trans.RouteInfo.NextStationIATAID = DataObject.getString(row["NextStationIATAID"]);
				trans.RouteInfo.OriginStationIATAGuid = DataObject.getGuid(row["OriginStationIATAGuid"]);
				trans.RouteInfo.OriginStationIATAID = DataObject.getString(row["OriginStationIATAID"]);
				trans.RouteInfo.PreviousRoutingID = DataObject.getString(row["PreviousRoutingID"]);
				trans.RouteInfo.PreviousStationIATAGuid = DataObject.getGuid(row["PreviousStationIATAGuid"]);
				trans.RouteInfo.PreviousStationIATAID = DataObject.getString(row["PreviousStationIATAID"]);
				trans.RouteInfo.RouteOriginationDate = DataObject.getOptionalDateTimeOffset(row["RouteOriginationDate"]);
				trans.RouteInfo.RoutingID = DataObject.getString(row["RoutingID"]);

				trans.RouteSchedule.ETA = DataObject.getOptionalDateTimeOffset(row["ETA"]);
				trans.RouteSchedule.ETD = DataObject.getOptionalDateTimeOffset(row["ETD"]);
				trans.RouteSchedule.FST = DataObject.getOptionalDateTimeOffset(row["FST"]);
				trans.RouteSchedule.SFT = DataObject.getOptionalDateTimeOffset(row["SFT"]);
				trans.RouteSchedule.STA = DataObject.getOptionalDateTimeOffset(row["STA"]);
				trans.RouteSchedule.STD = DataObject.getOptionalDateTimeOffset(row["STD"]);

				trans.DestinationEQ1.RegistrationID = DataObject.getString(row["DestinationRegistrationID1"]);
				trans.DestinationEQ1.SerialNumber = DataObject.getString(row["DestinationSerialNumber1"]);
				trans.DestinationEQ1.EquipmentType = DataObject.getString(row["DestinationEquipmentType1"]);
				trans.DestinationEQ1.EquipmentModel = DataObject.getString(row["DestinationEquipmentModel1"]);
				trans.DestinationEQ1.CompanyEquipmentID = DataObject.getString(row["DestinationCompanyEquipmentID1"]);
				trans.DestinationEQ1.EquipmentGuid = DataObject.getValue<Guid>(row["Destination1EquipmentGuid"], Guid.Empty);
				trans.DestinationEQ2.RegistrationID = DataObject.getString(row["DestinationRegistrationID2"]);
				trans.DestinationEQ2.SerialNumber = DataObject.getString(row["DestinationSerialNumber2"]);
				trans.DestinationEQ2.EquipmentType = DataObject.getString(row["DestinationEquipmentType2"]);
				trans.DestinationEQ2.EquipmentModel = DataObject.getString(row["DestinationEquipmentModel2"]);
				trans.DestinationEQ2.CompanyEquipmentID = DataObject.getString(row["DestinationCompanyEquipmentID2"]);
				trans.DestinationEQ2.EquipmentGuid = DataObject.getValue<Guid>(row["Destination2EquipmentGuid"], Guid.Empty);
				trans.DestinationEQ3.RegistrationID = DataObject.getString(row["DestinationRegistrationID3"]);
				trans.DestinationEQ3.SerialNumber = DataObject.getString(row["DestinationSerialNumber3"]);
				trans.DestinationEQ3.EquipmentType = DataObject.getString(row["DestinationEquipmentType3"]);
				trans.DestinationEQ3.EquipmentModel = DataObject.getString(row["DestinationEquipmentModel3"]);
				trans.DestinationEQ3.CompanyEquipmentID = DataObject.getString(row["DestinationCompanyEquipmentID3"]);
				trans.DestinationEQ3.EquipmentGuid = DataObject.getValue<Guid>(row["Destination3EquipmentGuid"], Guid.Empty);
				trans.SourceEQ1.RegistrationID = DataObject.getString(row["SourceRegistrationID1"]);
				trans.SourceEQ1.SerialNumber = DataObject.getString(row["SourceSerialNumber1"]);
				trans.SourceEQ1.EquipmentType = DataObject.getString(row["SourceEquipmentType1"]);
				trans.SourceEQ1.EquipmentModel = DataObject.getString(row["SourceEquipmentModel1"]);
				trans.SourceEQ1.CompanyEquipmentID = DataObject.getString(row["SourceCompanyEquipmentID1"]);
				trans.SourceEQ1.EquipmentGuid = DataObject.getValue<Guid>(row["Source1EquipmentGuid"], Guid.Empty);
				trans.SourceEQ2.RegistrationID = DataObject.getString(row["SourceRegistrationID2"]);
				trans.SourceEQ2.SerialNumber = DataObject.getString(row["SourceSerialNumber2"]);
				trans.SourceEQ2.EquipmentType = DataObject.getString(row["SourceEquipmentType2"]);
				trans.SourceEQ2.EquipmentModel = DataObject.getString(row["SourceEquipmentModel2"]);
				trans.SourceEQ2.CompanyEquipmentID = DataObject.getString(row["SourceCompanyEquipmentID2"]);
				trans.SourceEQ2.EquipmentGuid = DataObject.getValue<Guid>(row["Source2EquipmentGuid"], Guid.Empty);
				trans.SourceEQ3.RegistrationID = DataObject.getString(row["SourceRegistrationID3"]);
				trans.SourceEQ3.SerialNumber = DataObject.getString(row["SourceSerialNumber3"]);
				trans.SourceEQ3.EquipmentType = DataObject.getString(row["SourceEquipmentType3"]);
				trans.SourceEQ3.EquipmentModel = DataObject.getString(row["SourceEquipmentModel3"]);
				trans.SourceEQ3.CompanyEquipmentID = DataObject.getString(row["SourceCompanyEquipmentID3"]);
				trans.SourceEQ3.EquipmentGuid = DataObject.getValue<Guid>(row["Source3EquipmentGuid"], Guid.Empty);

				trans.EffectiveDate = DataObject.getOptionalDateTimeOffset(row["EffectiveDate"]);
				trans.ExpirationDate = DataObject.getOptionalDateTimeOffset(row["ExpirationDate"]);
				trans.ScheduledDate = DataObject.getOptionalDateTimeOffset(row["ScheduledDate"]);

				trans.AutoComplete = DataObject.getBool(row["AutoComplete"]);

				trans.Flag01 = DataObject.getBool(row["Flag01"]);
				trans.Flag02 = DataObject.getBool(row["Flag02"]);
				trans.Flag03 = DataObject.getBool(row["Flag03"]);
				trans.Flag04 = DataObject.getBool(row["Flag04"]);
				trans.Flag05 = DataObject.getBool(row["Flag05"]);
				trans.Flag06 = DataObject.getBool(row["Flag06"]);
				trans.ErrorFlag = DataObject.getBool(row["ErrorFlag"]);

				trans.Number01 = DataObject.getOptionalDouble(row["Number01"]);
				trans.Number02 = DataObject.getOptionalDouble(row["Number02"]);
				trans.Number03 = DataObject.getOptionalDouble(row["Number03"]);
				trans.Number04 = DataObject.getOptionalDouble(row["Number04"]);
				trans.Number05 = DataObject.getOptionalDouble(row["Number05"]);
				trans.Number06 = DataObject.getOptionalDouble(row["Number06"]);

				trans.ContactFirstName = DataObject.getString(row["ContactFirstName"]);
				trans.ContactSurname = DataObject.getString(row["ContactSurname"]);

				trans.Date01 = DataObject.getOptionalDateTimeOffset(row["Date01"]);
				trans.Date02 = DataObject.getOptionalDateTimeOffset(row["Date02"]);
				trans.Date03 = DataObject.getOptionalDateTimeOffset(row["Date03"]);
				trans.Date04 = DataObject.getOptionalDateTimeOffset(row["Date04"]);

				trans.CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
				trans.UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], DateTimeOffset.Now);

				trans.LegacyNumber = DataObject.getString(row["LegacyNumber"]);
				trans.Country = DataObject.getString(row["Country"]);
				trans.ContactInfo = DataObject.getString(row["ContactInfo"]);
				trans.AssociatedDocumentNumber = DataObject.getString(row["AssociatedDocNumber"]);
				trans.AssociatedCLIN = DataObject.getString(row["AssociatedCLIN"]);
				trans.AssociatedTransportOrderNumber = DataObject.getString(row["AssociatedTransportOrderNumber"]);

				trans.SubmittedToAccounting = DataObject.getOptionalBool(row["SubmittedToAccounting"]);
				trans.OriginApplication = row.IsNull("LookupOriginApplicationIndex")
					                          ? TransactionOrigin.None
					                          : (TransactionOrigin)DataObject.getInt(row["LookupOriginApplicationIndex"]);
				trans.FuelCardGuid = DataObject.getGuid(row["FuelCardGuid"]);
				trans.FuelCardID = DataObject.getString(row["FuelCardID"]);

				trans.RequestedDateTime = DataObject.getOptionalDateTimeOffset(row["RequestedDateTime"]);
				trans.DispatchedDateTime = DataObject.getOptionalDateTimeOffset(row["DispatchedDateTime"]);

			    trans.ShippingMethod = DataObject.getString(row["ShippingMethod"]);
			    trans.ReasonCodeGuid = DataObject.getGuid(row["ReasonCodeGuid"]);
                trans.RowVersion = (byte[])row["_RowVersion"];

				trans.LevelUnits = (EngineeringUnit)DataObject.getInt(row["LevelUnitIndex"]);
				trans.TemperatureUnits = (EngineeringUnit)DataObject.getInt(row["TemperatureUnitIndex"]);
				trans.DensityUnits = (EngineeringUnit)DataObject.getInt(row["DensityUnitIndex"]);
				trans.PressureUnits = (EngineeringUnit)DataObject.getInt(row["PressureUnitIndex"]);
				trans.FlowUnits = (EngineeringUnit)DataObject.getInt(row["FlowUnitIndex"]);
				trans.VolumeUnits = (EngineeringUnit)DataObject.getInt(row["VolumeUnitIndex"]);
				trans.AdditiveVolumeUnits = (EngineeringUnit)DataObject.getInt(row["AdditiveVolumeUnitIndex"]);
				trans.MassUnits = (EngineeringUnit)DataObject.getInt(row["MassUnitIndex"]);

				trans.LevelDecimalPlaces = (byte)DataObject.getInt(row["LevelDecimalPlaces"]);
				trans.TemperatureDecimalPlaces = (byte)DataObject.getInt(row["TemperatureDecimalPlaces"]);
				trans.DensityDecimalPlaces = (byte)DataObject.getInt(row["DensityDecimalPlaces"]);
				trans.PressureDecimalPlaces = (byte)DataObject.getInt(row["PressureDecimalPlaces"]);
				trans.FlowDecimalPlaces = (byte)DataObject.getInt(row["FlowDecimalPlaces"]);
				trans.VolumeDecimalPlaces = (byte)DataObject.getInt(row["VolumeDecimalPlaces"]);
				trans.AdditiveVolumeDecimalPlaces = (byte)DataObject.getInt(row["AdditiveVolumeDecimalPlaces"]);
				trans.MassDecimalPlaces = (byte)DataObject.getInt(row["MassDecimalPlaces"]);

				// Retrieve fields that come from the tblTransactionNotes table
				trans.TransactionNoteGuid = DataObject.getGuid(row["TransactionNoteGuid"]);
				trans.Notes = DataObject.getString(row["Notes"]);
				trans.AdditionalInformation = DataObject.getString(row["AdditionalInformation"]);

				// Retrieve fields that come from the tblTransactionSignature table
				trans.TransactionSignatureGuid = DataObject.getGuid(row["TransactionSignatureGuid"]);
				trans.Signature = DataObject.getOptionalVarBinary(row["Signature"]);

				// Retrieve transaction reference GUID
				trans.ReferencedTransactionGuid = DataObject.getGuid(row["ReferencedTransactionGuid"]);

				// Retrieve fields that come from the tblTransactionUserData table
				trans.TransactionUserDataGuid = DataObject.getGuid(row["TransactionUserDataGuid"]);

				for (int userDataFieldIndex = 1; userDataFieldIndex <= 24; ++userDataFieldIndex)
				{
					string userDataKey = "UserData" + userDataFieldIndex;
					string userDataValue = DataObject.getString(row[userDataKey]);

					if (userDataValue != null)
					{
						trans.UserData.Add(TransactionDO.UserDataKeyPrefix + userDataFieldIndex, userDataValue);
					}
				}
			}

			if (!string.IsNullOrEmpty(trans.ReversedTransID))
			{
				using (var cmd = new SqlCommand())
				{
					cmd.CommandText = "SELECT ConjoinTransID FROM tblTransactions WHERE TransID = @TransID";
					cmd.Parameters.Add("@TransID", SqlDbType.NVarChar, 64).Value = trans.ReversedTransID;

					DataSet ds = consolidatedDa.GetDataSet(cmd, this.security);
					if (ds.Tables[0].Rows.Count < 1)
					{
						throw new AccountingServicesException("TransactionID " + trans.ReversedTransID + " does not exist");
					}

					DataRow row = ds.Tables[0].Rows[0];
					trans.ConjoinReversedTransID = DataObject.getString(row["ConjoinTransID"]);
				}
			}
			else
			{
				trans.ConjoinReversedTransID = string.Empty;
			}

			// Only perform this if the transaction is the original.
			if (isOriginalTransaction)
			{
				if (string.IsNullOrEmpty(trans.ConjoinedTransID) == false)
				{
					using (var cmd = new SqlCommand())
					{
						cmd.CommandText = "SELECT TransactionGuid FROM tblTransactions WHERE TransID = @TransID";
						cmd.Parameters.Add("@TransID", SqlDbType.NVarChar, 64).Value = trans.ConjoinedTransID;

						DataSet ds = consolidatedDa.GetDataSet(cmd, this.security);
						if (ds.Tables[0].Rows.Count < 1)
						{
							throw new AccountingServicesException("TransactionID " + trans.ConjoinedTransID + " does not exist");
						}

						DataRow row = ds.Tables[0].Rows[0];
						trans.ConjoinedTransactionGuid = DataObject.getGuid(row["TransactionGuid"]);
					}
				}
				else
				{
					trans.ConjoinedTransactionGuid = Guid.Empty;
				}
			}

			return trans;
		}

		/// <summary>
		/// Populates each line item of the transaction with the ID's
		/// of associated transactions
		/// </summary>
		/// <param name="trans">
		/// The transaction whose line items will be populated
		/// </param>
		private void RetrieveAssociatedTransactions(TransactionDO trans)
		{
			foreach (LineItemDO lineItem in trans.LineItems)
			{
				using (var cmd = new SqlCommand())
				{
				    cmd.CommandType = CommandType.StoredProcedure;
				    cmd.CommandText = "usp_TransactionLinksGet";

					cmd.Parameters.Add("@TransID", SqlDbType.NVarChar, 64).Value = trans.TransID;
					cmd.Parameters.Add("@TransactionLineItemGuid", SqlDbType.UniqueIdentifier).Value = lineItem.TransactionLineItemGuid;

					var consolidatedDa = new ConsolidatedDAClass();
					DataSet ds = consolidatedDa.GetDataSet(cmd, this.security);

					foreach (DataRow dr in ds.Tables[0].Rows)
					{
						var associatedIds = new AssociatedTxDO
							                    {
								                    TransID = DataObject.getValue<string>(dr["TransID"], string.Empty),
								                    TransactionLineItemGuid = DataObject.getValue<Guid>(dr["TransactionLineItemGuid"], Guid.Empty),
								                    Associated = 1,
								                    BillToID = DataObject.getValue<string>(dr["BillToID"], string.Empty),
								                    DocumentNumber = DataObject.getValue<string>(dr["DocumentNumber"], string.Empty),
								                    InventoryDateTime = DataObject.getValue<DateTime>( dr["InventoryDate"], DateTime.Today)
							                    };

						if (this.accountingSite != null)
						{
							associatedIds.InventoryDate = this.accountingSite.FormatDate(associatedIds.InventoryDateTime);
						}

						associatedIds.Manager = DataObject.getValue<string>(dr["ManagerID"], string.Empty);
						associatedIds.Owner = DataObject.getValue<string>(dr["OwnerID"], string.Empty);
						associatedIds.PONumber = DataObject.getValue<string>(dr["PONumber"], string.Empty);
						associatedIds.ShipToID = DataObject.getValue<string>(dr["ShipToID"], string.Empty);
						associatedIds.SupplierID = DataObject.getValue<string>(dr["SupplierID"], string.Empty);
						associatedIds.TransactionDateTime = DataObject.getValue<DateTimeOffset>(dr["TransDateTime"], DateTimeOffset.Now);

						if (this.accountingSite != null)
						{
							associatedIds.TransactionDate = this.accountingSite.FormatDate(associatedIds.TransactionDateTime);
						}

						associatedIds.TransactionAlias = DataObject.getValue<string>(dr["AliasName"], string.Empty);
						associatedIds.Product = DataObject.getValue<string>(dr["Product"], string.Empty);
						associatedIds.GrossQuantity = DataObject.getValue<double>(dr["GrossQuantity"], 0.0);
						associatedIds.Excise = DataObject.getValue<double>(dr["Tax1"], 0.0);
						associatedIds.GST = DataObject.getValue<double>(dr["Tax2"], 0.0);
						associatedIds.Markup = DataObject.getValue<double>(dr["Tax3"], 0.0);
						associatedIds.DeliveryLocation = DataObject.getValue<string>(dr["DeliveryLocation"].ToString(), string.Empty);
						associatedIds.Site = DataObject.getValue<string>(dr["Site"].ToString(), string.Empty);

						lineItem.AssociatedTransactions.Add(associatedIds);
					}
				}
			}
		}

		/// <summary>
		/// The retrieve weight readings.
		/// </summary>
		/// <param name="trans">
		/// The trans.
		/// </param>
		private void RetrieveWeightReadings(TransactionDO trans)
		{
			using (var cmd = new SqlCommand())
			{
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_TransactionWeightReadingsGet";

				cmd.Parameters.Add("@TransactionGuid", SqlDbType.UniqueIdentifier).Value = trans.TransactionGuid;
				var consolidatedDa = new ConsolidatedDAClass();
				DataSet ds = consolidatedDa.GetDataSet(cmd, this.security);

				foreach (DataRow row in ds.Tables[0].Rows)
				{
					var weightReading = new WeightReadingDO
						                    {
							                    BeginQuantity = DataObject.getOptionalDouble(row["BeginQuantityValue"]),
							                    CompartmentName = DataObject.getString(row["CompartmentID"]),
							                    FinalQuantity = DataObject.getOptionalDouble(row["FinalQuantityValue"]),
							                    RequestedQuantity = DataObject.getOptionalDouble(row["RequestedQuantityValue"]),
							                    FuelsManagerVersionNumber = DataObject.getInt(row["FuelsManagerVersionNumber"]),
							                    SourceVersionNumber = DataObject.getOptionalInt(row["SourceVersionNumber"]),
							                    HistoricalFlag = DataObject.getBool(row["HistoricalFlag"])
						                    };

					trans.WeightReadings.Add(weightReading);
				}
			}
		}

		/// <summary>
		/// This method will retrieve the transport line items into a list.
		/// </summary>
		/// <param name="trans">
		/// Transaction data object.
		/// </param>
		private void RetrieveTransportLineItems(TransactionDO trans)
		{
			using (var cmd = new SqlCommand())
			{
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_TransactionTransportLineItemsGet";
				cmd.Parameters.Add("@TransactionGuid", SqlDbType.UniqueIdentifier).Value = trans.TransactionGuid;

				var consolidatedDa = new ConsolidatedDAClass();
				DataSet ds = consolidatedDa.GetDataSet(cmd, this.security);

				foreach (DataRow row in ds.Tables[0].Rows)
				{
					var transportLineItemDO = new TransportLineItemDO();
					transportLineItemDO.Load(row);
					trans.TransportInfoList.Add(transportLineItemDO);
				}
			}
		}

		/// <summary>
		/// The retrieve line items.
		/// </summary>
		/// <param name="trans">
		/// The transaction.
		/// </param>
		private void RetrieveLineItems(TransactionDO trans)
		{
			var consolidatedDa = new ConsolidatedDAClass();

			using (var cmd = new SqlCommand())
			{
			    cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_TransactionLineItemsAndUserDataGet";

				cmd.Parameters.Add("@TransactionGuid", SqlDbType.UniqueIdentifier).Value = trans.TransactionGuid;

				// When we retrieve the header, we retrieve the units and decimal places from either the sites or transaction alias table.
				// We may end up using the value the header retrieved or the one associated with the product of the line item.
				// We pass these values in so that we can make that determination without performing additional joins.
				cmd.Parameters.Add("@LevelUnitIndex", SqlDbType.Int).Value = (int)trans.LevelUnits;
				cmd.Parameters.Add("@TemperatureUnitIndex", SqlDbType.Int).Value = (int)trans.TemperatureUnits;
				cmd.Parameters.Add("@DensityUnitIndex", SqlDbType.Int).Value = (int)trans.DensityUnits;
				cmd.Parameters.Add("@PressureUnitIndex", SqlDbType.Int).Value = (int)trans.PressureUnits;
				cmd.Parameters.Add("@FlowUnitIndex", SqlDbType.Int).Value = (int)trans.FlowUnits;
				cmd.Parameters.Add("@VolumeUnitIndex", SqlDbType.Int).Value = (int)trans.VolumeUnits;
				cmd.Parameters.Add("@AdditiveVolumeUnitIndex", SqlDbType.Int).Value = (int)trans.AdditiveVolumeUnits;
				cmd.Parameters.Add("@MassUnitIndex", SqlDbType.Int).Value = (int)trans.MassUnits;

				cmd.Parameters.Add("@LevelDecimalPlaces", SqlDbType.TinyInt).Value = trans.LevelDecimalPlaces;
				cmd.Parameters.Add("@TemperatureDecimalPlaces", SqlDbType.TinyInt).Value = trans.TemperatureDecimalPlaces;
				cmd.Parameters.Add("@DensityDecimalPlaces", SqlDbType.TinyInt).Value = trans.DensityDecimalPlaces;
				cmd.Parameters.Add("@PressureDecimalPlaces", SqlDbType.TinyInt).Value = trans.PressureDecimalPlaces;
				cmd.Parameters.Add("@FlowDecimalPlaces", SqlDbType.TinyInt).Value = trans.FlowDecimalPlaces;
				cmd.Parameters.Add("@VolumeDecimalPlaces", SqlDbType.TinyInt).Value = trans.VolumeDecimalPlaces;
				cmd.Parameters.Add("@AdditiveVolumeDecimalPlaces", SqlDbType.TinyInt).Value = trans.AdditiveVolumeDecimalPlaces;
				cmd.Parameters.Add("@MassDecimalPlaces", SqlDbType.TinyInt).Value = trans.MassDecimalPlaces;
				cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = this.security.SiteGuid;

				DataSet dataSet = consolidatedDa.GetDataSet(cmd, this.security);

				foreach (DataRow row in dataSet.Tables[0].Rows)
				{
					LineItemDO lineItem;

					if (trans is RegradeDO)
					{
						lineItem = new RegradeLineItemDO();
					}
					else if (trans is StorageTransferDO || trans is OwnerTransferDO)
					{
						lineItem = new StorageTransferLineItemDO();
					}
					else
					{
						lineItem = new LineItemDO();
					}

					trans.LineItems.Add(lineItem);
					lineItem.Load(row, trans.TransTypeID);

					// Retrieve fields that come from the tblTransactionLineItemUserData table
					lineItem.TransactionLineItemUserDataGuid = DataObject.getGuid(row["TransactionLineItemUserDataGuid"]);

					for (int lineItemUserDataIndex = 1; lineItemUserDataIndex <= 24; lineItemUserDataIndex++)
					{
						string column = "UserData" + lineItemUserDataIndex;
						string columnValue = DataObject.getString(row[column]);

						if (columnValue != null)
						{
							// Add the item to the line item's user data collection
							lineItem.UserData.Add(BaseTransactionLineItemDO.UserDataLineItemKeyPrefix + lineItemUserDataIndex, columnValue);
						}
					}
				}
			}
		}

		/// <summary>
		/// The retrieve sub line items.
		/// </summary>
		/// <param name="trans">
		/// The transaction.
		/// </param>
		private void RetrieveSubLineItems(TransactionDO trans)
		{
			if (trans.TransTypeID == TransactionTypes.T15_PrimaryRegrade
			    || trans.TransTypeID == TransactionTypes.T16_SecondaryRegrade)
			{
				// Regrade sublineitem/blend handling happens inside the processors and is hidden 
				// from the upper layers; this is a consequence of the handling of the conjoined transactions
				// and difficulties expressing this visually
				return;
			}

			using (var cmd = new SqlCommand())
			{
			    cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_TransactionSubLineItemsGet";

				cmd.Parameters.Add("@TransactionGuid", SqlDbType.UniqueIdentifier).Value = trans.TransactionGuid;

				// When we retrieve the header, we retrieve the units and decimal places from either the sites or transaction alias table.
				// We may end up using the value the header retrieved or the one associated with the product of the sub line item.
				// We pass these values in so that we can make that determination without performing additional joins.
				cmd.Parameters.Add("@LevelUnitIndex", SqlDbType.Int).Value = (int)trans.LevelUnits;
				cmd.Parameters.Add("@TemperatureUnitIndex", SqlDbType.Int).Value = (int)trans.TemperatureUnits;
				cmd.Parameters.Add("@DensityUnitIndex", SqlDbType.Int).Value = (int)trans.DensityUnits;
				cmd.Parameters.Add("@PressureUnitIndex", SqlDbType.Int).Value = (int)trans.PressureUnits;
				cmd.Parameters.Add("@FlowUnitIndex", SqlDbType.Int).Value = (int)trans.FlowUnits;
				cmd.Parameters.Add("@VolumeUnitIndex", SqlDbType.Int).Value = (int)trans.VolumeUnits;
				cmd.Parameters.Add("@AdditiveVolumeUnitIndex", SqlDbType.Int).Value = (int)trans.AdditiveVolumeUnits;
				cmd.Parameters.Add("@MassUnitIndex", SqlDbType.Int).Value = (int)trans.MassUnits;

				cmd.Parameters.Add("@LevelDecimalPlaces", SqlDbType.TinyInt).Value = trans.LevelDecimalPlaces;
				cmd.Parameters.Add("@TemperatureDecimalPlaces", SqlDbType.TinyInt).Value = trans.TemperatureDecimalPlaces;
				cmd.Parameters.Add("@DensityDecimalPlaces", SqlDbType.TinyInt).Value = trans.DensityDecimalPlaces;
				cmd.Parameters.Add("@PressureDecimalPlaces", SqlDbType.TinyInt).Value = trans.PressureDecimalPlaces;
				cmd.Parameters.Add("@FlowDecimalPlaces", SqlDbType.TinyInt).Value = trans.FlowDecimalPlaces;
				cmd.Parameters.Add("@VolumeDecimalPlaces", SqlDbType.TinyInt).Value = trans.VolumeDecimalPlaces;
				cmd.Parameters.Add("@AdditiveVolumeDecimalPlaces", SqlDbType.TinyInt).Value = trans.AdditiveVolumeDecimalPlaces;
				cmd.Parameters.Add("@MassDecimalPlaces", SqlDbType.TinyInt).Value = trans.MassDecimalPlaces; 
				cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = this.security.SiteGuid;

				var consolidatedDa = new ConsolidatedDAClass();
				DataSet ds = consolidatedDa.GetDataSet(cmd, this.security);

				foreach (DataRow row in ds.Tables[0].Rows)
				{
					Guid currentTransactionLineItemGuid = DataObject.getGuid(row["TransactionLineItemGuid"]);

					LineItemDO lineItem = null;

					foreach (LineItemDO tempLineItem in trans.LineItems)
					{
						if (tempLineItem.TransactionLineItemGuid == currentTransactionLineItemGuid)
						{
							lineItem = tempLineItem;
							break;
						}
					}

					var sublineItem = new SubLineItemDO();

					sublineItem.TransactionSubLineItemGuid		= DataObject.getGuid(row["TransactionSubLineItemGuid"]);
					sublineItem.SequenceId						= DataObject.getInt(row["SequenceID"]);
					sublineItem.ArmNumber						= DataObject.getOptionalInt(row["ArmNumber"]);
					sublineItem.BatchNumber						= DataObject.getString(row["BatchNumber"]);
					sublineItem.BottomVolume					= DataObject.getOptionalDouble(row["BottomVolume"]);
					sublineItem.Customs							= DataObject.getString(row["Customs"]);
					sublineItem.DeleteFlag						= DataObject.getBool(row["DeleteFlag"]);
					sublineItem.Density							= DataObject.getOptionalDouble(row["Density"]);
					sublineItem.DifferentialPressure			= DataObject.getOptionalDouble(row["DifferentialPressure"]);
					sublineItem.DosageRate						= DataObject.getOptionalDouble(row["DosageRate"]);
					sublineItem.FreezePoint						= DataObject.getOptionalDouble(row["FreezePoint"]);
					sublineItem.LineFill						= DataObject.getOptionalDouble(row["LineFill"]);
					sublineItem.LineNumber						= DataObject.getOptionalInt(row["LineNumber"]);
					sublineItem.MeterReading.MeterFactor		= DataObject.getOptionalDouble(row["MeterFactor"]);
					sublineItem.MeterReading.MeterStart			= DataObject.getOptionalDouble(row["MeterStart"]);
					sublineItem.MeterReading.MeterStop			= DataObject.getOptionalDouble(row["MeterStop"]);
					sublineItem.MeterReading.StartDateTime		= DataObject.getOptionalDateTimeOffset(row["MeterStartDateTime"]);
					sublineItem.MeterReading.StopDateTime		= DataObject.getOptionalDateTimeOffset(row["MeterStopDateTime"]);
					sublineItem.NetCapacity						= DataObject.getOptionalDouble(row["NetCapacity"]);
					sublineItem.PresetAmount					= DataObject.getOptionalDouble(row["PresetAmount"]);
					sublineItem.Product							= DataObject.getString(row["Product"]);
					sublineItem.ProductCode						= DataObject.getString(row["ProductCode"]);
					sublineItem.ProductType						= DataObject.getString(row["ProductType"]);
					sublineItem.ProductGuid						= DataObject.getGuid(row["ProductGuid"]);
					sublineItem.Status							= (TransactionStatus)row["LookupTransactionStatusIndex"];
					sublineItem.TankStatus						= DataObject.getString(row["TankStatus"]);
					sublineItem.Temperature						= DataObject.getOptionalDouble(row["Temperature"]);
					sublineItem.VCF								= DataObject.getOptionalDouble(row["VCF"]);
					sublineItem.Pressure							= DataObject.getOptionalDouble(row["Pressure"]);
					sublineItem.Quantity.AffectsInventory		= true;
					sublineItem.Quantity.GrossInventoryChange	= DataObject.getDouble(row["GrossQuantity"]);
					sublineItem.Quantity.DeliveredGrossInventoryChange = DataObject.getDouble(row["DeliveredGrossQuantity"]);
					sublineItem.Quantity.NetInventoryChange		= DataObject.getDouble(row["NetQuantity"]);
					sublineItem.Quantity.DeliveredNetInventoryChange = DataObject.getDouble(row["DeliveredNetQuantity"]);
					sublineItem.StorageLocationID				= DataObject.getString(row["StorageLocationID"]);
					sublineItem.StorageLocationTankGuid			= DataObject.getGuid(row["StorageLocationTankGuid"]);
					sublineItem.MeterID							= DataObject.getString(row["MeterID"]);
					sublineItem.MeterGuid						= DataObject.getGuid(row["MeterGuid"]);
					sublineItem.COAID							= DataObject.getString(row["COAID"]);
					sublineItem.Quality							= (TransactionQuality)DataObject.getInt(row["LookupQualityIndex"]);
					sublineItem.Tax1							= DataObject.getOptionalDouble(row["Tax1"]);
					sublineItem.Tax2							= DataObject.getOptionalDouble(row["Tax2"]);
					sublineItem.Tax3							= DataObject.getOptionalDouble(row["Tax3"]);
					sublineItem.Tax4							= DataObject.getOptionalDouble(row["Tax4"]);
					sublineItem.Tax5							= DataObject.getOptionalDouble(row["Tax5"]);
					sublineItem.ImproperAdditization			= DataObject.getOptionalBool(row["ImproperAdditization"]);
					sublineItem.BrokenBlend						= DataObject.getOptionalBool(row["BrokenBlend"]);

					// vthompson 5-21-2008
					// Generic Flag fields
					sublineItem.Flag01 = DataObject.getBool(row["Flag01"]);
					sublineItem.Flag02 = DataObject.getBool(row["Flag02"]);
					sublineItem.Flag03 = DataObject.getBool(row["Flag03"]);
					sublineItem.Flag04 = DataObject.getBool(row["Flag04"]);
					sublineItem.Flag05 = DataObject.getBool(row["Flag05"]);
					sublineItem.Flag06 = DataObject.getBool(row["Flag06"]);

					// Generic number fields
					sublineItem.Number01 = DataObject.getOptionalDouble(row["Number01"]);
					sublineItem.Number02 = DataObject.getOptionalDouble(row["Number02"]);
					sublineItem.Number03 = DataObject.getOptionalDouble(row["Number03"]);
					sublineItem.Number04 = DataObject.getOptionalDouble(row["Number04"]);
					sublineItem.Number05 = DataObject.getOptionalDouble(row["Number05"]);
					sublineItem.Number06 = DataObject.getOptionalDouble(row["Number06"]);

					// Generic date fields
					sublineItem.Date01 = DataObject.getOptionalDateTimeOffset(row["Date01"]);
					sublineItem.Date02 = DataObject.getOptionalDateTimeOffset(row["Date02"]);
					sublineItem.Date03 = DataObject.getOptionalDateTimeOffset(row["Date03"]);
					sublineItem.Date04 = DataObject.getOptionalDateTimeOffset(row["Date04"]);

					sublineItem.Quantity.Mass = DataObject.getDouble(row["MassQuantity"]);
					sublineItem.Quantity.NetManualValueFlag = DataObject.getOptionalBool(row["NetManualValueFlag"]);
					sublineItem.Quantity.GrossManualValueFlag = DataObject.getOptionalBool(row["GrossManualValueFlag"]);
					sublineItem.Quantity.MassManualValueFlag = DataObject.getOptionalBool(row["MassManualValueFlag"]);
					sublineItem.Quantity.PackageManualValueFlag = DataObject.getOptionalBool(row["PackageManualValueFlag"]);
					sublineItem.Quantity.VcfManualValueFlag = DataObject.getOptionalBool(row["VcfManualValueFlag"]);
					sublineItem.Quantity.DeliveredGrossManualValueFlag = DataObject.getOptionalBool(row["DeliveredGrossManualValueFlag"]);
					sublineItem.Quantity.DeliveredNetManualValueFlag = DataObject.getOptionalBool(row["DeliveredNetManualValueFlag"]);

					sublineItem.LevelUnits = (EngineeringUnit)DataObject.getInt(row["LevelUnitIndex"]);
					sublineItem.TemperatureUnits = (EngineeringUnit)DataObject.getInt(row["TemperatureUnitIndex"]);
					sublineItem.DensityUnits = (EngineeringUnit)DataObject.getInt(row["DensityUnitIndex"]);
					sublineItem.PressureUnits = (EngineeringUnit)DataObject.getInt(row["PressureUnitIndex"]);
					sublineItem.FlowUnits = (EngineeringUnit)DataObject.getInt(row["FlowUnitIndex"]);
					sublineItem.VolumeUnits = (EngineeringUnit)DataObject.getInt(row["VolumeUnitIndex"]);
					sublineItem.MassUnits = (EngineeringUnit)DataObject.getInt(row["MassUnitIndex"]);
					sublineItem.LevelDecimalPlaces = (byte)DataObject.getInt(row["LevelDecimalPlaces"]);
					sublineItem.TemperatureDecimalPlaces = (byte)DataObject.getInt(row["TemperatureDecimalPlaces"]);
					sublineItem.DensityDecimalPlaces = (byte)DataObject.getInt(row["DensityDecimalPlaces"]);
					sublineItem.PressureDecimalPlaces = (byte)DataObject.getInt(row["PressureDecimalPlaces"]);
					sublineItem.FlowDecimalPlaces = (byte)DataObject.getInt(row["FlowDecimalPlaces"]);
					sublineItem.VolumeDecimalPlaces = (byte)DataObject.getInt(row["VolumeDecimalPlaces"]);
					sublineItem.MassDecimalPlaces = (byte)DataObject.getInt(row["MassDecimalPlaces"]);
					sublineItem.VolumePackageSize = DataObject.getDouble(row["VolumePackageSize"]);
					sublineItem.MassPackageSize = DataObject.getDouble(row["MassPackageSize"]);
					sublineItem.IsEthanol = DataObject.getBool(row["IsEthanol"]);

					if (!string.IsNullOrEmpty(row["VcfModuleSettings"] as string))
					{
						try
						{
							using (MemoryStream memoryStream = new MemoryStream(new UTF8Encoding().GetBytes(row["VcfModuleSettings"] as string)))
							{
								DataContractSerializer serializer = new DataContractSerializer(typeof(VcfModuleSettings));
								sublineItem.VcfModuleSettings = serializer.ReadObject(memoryStream) as VcfModuleSettings;
							}
						}
						catch
						{
							// Try catch can be removed after next release after FM12 SP3 as it will be fixed on first start after upgrade to SP3
							// All products will be resaved with new serializer on first start after upgrade.
							var serializer = CachingXmlSerializerFactory.Create(typeof(VcfModuleSettings));
							var stringReader = new StringReader(DataObject.getValue<string>(row["VcfModuleSettings"], null));
							sublineItem.VcfModuleSettings = (VcfModuleSettings)serializer.Deserialize(stringReader);
						}
					}


					if (lineItem != null)
					{
						lineItem.SubLineItems.Add(sublineItem);

						if (sublineItem.IsEthanol == true)
						{
							lineItem.IsEthanolBlend = true;
						}
					}
				}
			}
		}

		/// <summary>
		/// The retrieve PIDX collection.
		/// </summary>
		/// <param name="trans">
		/// The transaction.
		/// </param>
		private void RetrievePidxCollection(TransactionDO trans)
		{
			var transPidxsr = new TransactionPIDXSR
				                  {
					                  Security = this.security,
					                  PIDXRequestType = TransactionPIDXSR.PIDX_REQUEST_TYPES.GET_PIDX_TRANS,
					                  TransactionGuid = trans.TransactionGuid
				                  };

			var transactionPidxProcessorClass = new TransactionPIDXProcessorClass();
			TransactionPIDXCollectionDO transPidxCollection = transactionPidxProcessorClass.Process(transPidxsr);

			if (transPidxCollection != null)
			{
				foreach (TransactionPIDXDO transPidxDO in transPidxCollection.TransactionPIDXDOList)
				{
					if (null == trans.TransPIDXCollection)
					{
						trans.TransPIDXCollection = new List<TransactionPIDXDO>();
					}

					trans.TransPIDXCollection.Add(transPidxDO);
				}
			}
		}

		/// <summary>
		/// The retrieve closeout dates.
		/// </summary>
		/// <param name="trans">
		/// The transaction.
		/// </param>
		private void RetrieveCloseoutDates(TransactionDO trans)
		{
			// Determine a list of products
			var productList = new List<string>();

			foreach (LineItemDO lineItemDO in trans.LineItems)
			{
				foreach (SubLineItemDO subLineItemDO in lineItemDO.SubLineItems)
				{
					if (!productList.Contains(subLineItemDO.Product))
					{
						productList.Add(subLineItemDO.Product);
					}
				}

				if (!productList.Contains(lineItemDO.Product))
				{
					productList.Add(lineItemDO.Product);
				}
			}

			// Determine the closeout date for each product and update the line items
			var ledgerSr = new LedgerSR
				               {
					               Security = this.security,
					               Manager = trans.ManagerID,
					               Month = DateEfficacy.ConvertToMonthAndYear(trans.InventoryDate)
				               };

			var closeoutDO = new CloseoutDO();

			foreach (string product in productList)
			{
				ledgerSr.Product = product;

				using (var cmd = new SqlCommand())
				{
					closeoutDO.GetLatestCloseoutDateSelectSQL(cmd, ledgerSr, trans.Site);

					var dal = new ConsolidatedDAClass();
					DataSet dataSet = dal.GetDataSet(cmd, ledgerSr.Security);

					bool partialCloseout = false;

					DateTime latestCloseout = DateTimeOffset.MinValue.Date;

					if (dataSet != null)
					{
						closeoutDO.loadLatestCloseoutDate(dataSet);

						if (closeoutDO.CloseoutDate >= trans.InventoryDate)
						{
							if (closeoutDO.CloseoutDate > latestCloseout)
							{
								latestCloseout = closeoutDO.CloseoutDate;
							}

							foreach (LineItemDO lineItemDO in trans.LineItems)
							{
								foreach (SubLineItemDO subLineItemDO in lineItemDO.SubLineItems)
								{
									if (product != subLineItemDO.Product)
									{
										continue;
									}

									subLineItemDO.CloseoutDate = closeoutDO.CloseoutDate;
									partialCloseout = true;
								}

								if (product != lineItemDO.Product)
								{
									continue;
								}

								lineItemDO.CloseoutDate = closeoutDO.CloseoutDate;
								partialCloseout = true;
							}
						}
					}

					// determine if the transaction is closed out or partially closed out
					bool completeCloseout = partialCloseout;

					if (partialCloseout)
					{
						foreach (LineItemDO lineItemDO in trans.LineItems)
						{
							foreach (SubLineItemDO subLineItemDO in lineItemDO.SubLineItems)
							{
								if (subLineItemDO.CloseoutDate == null)
								{
									completeCloseout = false;
									break;
								}
							}

							if (!completeCloseout)
							{
								break;
							}

							if (lineItemDO.CloseoutDate == null)
							{
								completeCloseout = false;
								break;
							}
						}

						if (completeCloseout)
						{
							partialCloseout = false;
						}
					}

					trans.PartialCloseout = partialCloseout;

					if (completeCloseout)
					{
						if (latestCloseout != DateTimeOffset.MinValue)
						{
							trans.CloseoutDate = latestCloseout;
						}
					}
				}
			}
		}

		/// <summary>
		/// The find order quantity.
		/// </summary>
		/// <param name="orderQty">
		/// The order quantity.
		/// </param>
		/// <param name="line">
		/// The line.
		/// </param>
		/// <returns>
		/// The <see cref="OrderQuantities"/>.
		/// </returns>
		private OrderQuantities FindOrderQuantity(OrderQtyListDO orderQty, LineItemDO line)
		{
			// Find the matching quantity information if it exists
			foreach (OrderQuantities quantities in orderQty.Values)
			{
				if (quantities.TransactionLineItemGuid == line.TransactionLineItemGuid)
				{
					return quantities;
				}
			}

			// Did not find any associated transaction information for this line item
			return null;
		}

		/// <summary>
		/// The get order quantities.
		/// </summary>
		/// <param name="trans">
		/// The transaction.
		/// </param>
		private void GetOrderQuantities(TransactionDO trans)
		{
			var qtySr = new OrderQtyListSR { TransactionGuid = trans.TransactionGuid, Security = this.security };

			var proc = new OrderQtyListProcessorClass();
			OrderQtyListDO orderQty = proc.Process(qtySr);

			// Loop through all the transaction line items
			foreach (LineItemDO line in trans.LineItems)
			{
				// If we have quanity information, use it to calculate the remaining values
				OrderQuantities quantities = this.FindOrderQuantity(orderQty, line);

				if (quantities != null)
				{
					// always subtract quantites on certain associated types
					if (TransactionTypes.T18_SupplyOrder == trans.TransTypeID)
					{
						line.GrossQuantityReceived = quantities.AggregateGrossQuantity;
						line.NetQuantityReceived = quantities.AggregateNetQuantity;
						line.MassQuantityReceived = quantities.AggregateMassQuantity;

						line.GrossQuantityRemaining = line.Quantity.GrossInventoryChange - line.GrossQuantityReceived;
						line.NetQuantityRemaining = line.Quantity.NetInventoryChange - line.NetQuantityReceived;

						if (null != line.ProductPrice)
						{
							line.ValueRemaining = line.NetQuantityRemaining * line.ProductPrice.Value;
							line.TotalValue = line.Quantity.NetInventoryChange * line.ProductPrice.Value;
						}
					}
					else
					{
						line.GrossQuantityReceived = -quantities.AggregateGrossQuantity;
						line.NetQuantityReceived = -quantities.AggregateNetQuantity;
						line.MassQuantityReceived = -quantities.AggregateMassQuantity;

						line.GrossQuantityRemaining = line.Quantity.GrossInventoryChange - line.GrossQuantityReceived;
						line.NetQuantityRemaining = line.Quantity.NetInventoryChange - line.NetQuantityReceived;
						line.MassQuantityRemaining = line.Quantity.MassInventoryChange - line.MassQuantityReceived;
					}
				}
				else
				{
					// Otherwise, set the remaining values to the ordered values
					line.NetQuantityRemaining = line.Quantity.NetInventoryChange;
					line.GrossQuantityRemaining = line.Quantity.GrossInventoryChange;
					line.MassQuantityRemaining = line.Quantity.MassInventoryChange;

					// Set price based line items for supply orders
					if (TransactionTypes.T18_SupplyOrder == trans.TransTypeID)
					{
						if (null != line.ProductPrice)
						{
							line.ValueRemaining = line.NetQuantityRemaining * line.ProductPrice.Value;
							line.TotalValue = line.Quantity.Net * line.ProductPrice.Value;
						}
					}
				}
			}
		}

		/// <summary>
		/// Populates line items and sub line items with the special instruction text defined for the 
		/// ship to company's authorized products.
		/// </summary>
		/// <param name="transaction">The transaction record which contains line items and sub line items to 
		/// set the special instruction text for</param>
		private void GetSpecialInstructions(TransactionDO transaction)
		{
			// Get Special Instructions only if there is a shipto
			if (string.IsNullOrEmpty(transaction.ShipToID))
			{
				return;
			}

			Guid companyGuid;

			if (transaction.ShipToCompanyGuid != Guid.Empty)
			{
				companyGuid = transaction.ShipToCompanyGuid;
			}
			else
			{
				var companies = new CompaniesClass();
				companyGuid = companies.GetIdentityGuid(this.security, transaction.ShipToID);
			}

			// Load the ship to company's authorized product maps, which will include the special instruction text for the products
			var maps = new ProductMapsClass();
			ProductMapCollectionClass authorizedProductMapCollection =
				maps.EnumerateSpecialInstructionsByAssignedToCompany(this.security, companyGuid);

			// Set the special instruction text for each line item or sub line item depending on the product.
			if (authorizedProductMapCollection != null)
			{
				foreach (LineItemDO lineItem in transaction.LineItems)
				{
					ProductMapClass lineItemProductMap =
						authorizedProductMapCollection.Find((matchingMap) => matchingMap.AssignedGuid == lineItem.ProductGuid);

					if (lineItemProductMap != null)
					{
						lineItem.SpecialInstructionsNote = lineItemProductMap.SpecialInstructions;
						lineItem.SpecialInstructionsNoteGuid = lineItemProductMap.IdentityGuid;
						lineItem.SpecialInstructionsNoteProductMapType = lineItemProductMap.Type;
					}

					foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
					{
						ProductMapClass subLineItemProductMap =
							authorizedProductMapCollection.Find((matchingMap) => matchingMap.AssignedGuid == subLineItem.ProductGuid);

						if (subLineItemProductMap != null)
						{
							subLineItem.SpecialInstructionsNote = subLineItemProductMap.SpecialInstructions;
							subLineItem.SpecialInstructionsNoteGuid = subLineItemProductMap.IdentityGuid;
							subLineItem.SpecialInstructionsNoteProductMapType = subLineItemProductMap.Type;
						}
					}
				}
			}
		}

		/// <summary>
		/// Sets the passed line item's gross quantity received and gross value
		/// received properties.
		/// </summary>
		/// <param name="trans">
		/// The trans.
		/// </param>
		/// <remarks>
		/// Ultimately the Supply Order/Order logic should
		/// be replaced by this.
		/// </remarks>
		private void SetQuantityAndValueReceived(TransactionDO trans)
		{
			var drawdown = new TransactionHierarchyUtil(this.security);

			double receivedQty = 0;
			double receivedValue = 0;

			foreach (LineItemDO lineItem in trans.LineItems)
			{
				// See if the lineItem has any child line items associated.  If not, stop.
				// Get the hierarchy
				DataSet ds = drawdown.GetHierarchy(lineItem.TransactionLineItemGuid);

				if (ds.Tables["Children"] == null || ds.Tables["Children"].Rows.Count == 0)
				{
					continue;
				}

				DataTable children = ds.Tables["Children"];

				foreach (DataRow dr in children.Rows)
				{
					// If the child is a completed receipt and good quality
					// the gross quantity should be added to the parent
					// line item's receivedQty
					if (Convert.ToInt32(dr["LookupTransTypeIndex"]) == (int)TransactionTypes.T8_Receipt)
					{
						if (Convert.ToInt32(dr["LookupTransactionStatusIndex"]) == (int)TransactionStatus.Completed
						    && Convert.ToInt32(dr["LookupQualityIndex"]) == (int)TransactionQuality.Usable)
						{
							receivedQty += (double)dr["GrossQuantity"];

							// vthompson - Check to see if product price is null
							if (dr["ProductPrice"] != DBNull.Value)
							{
								receivedValue += (double)dr["ProductPrice"] * (double)dr["GrossQuantity"];
							}
						}
					}
					else
					{
						// Since the child line item is not a receipt check to see if
						// it is a completed line item.  If not, use the gross quantity
						if (Convert.ToInt32(dr["LookupTransactionStatusIndex"]) != (int)TransactionStatus.Completed
						    && Convert.ToInt16(dr["Tier"]) == 0)
						{
							receivedQty += (double)dr["GrossQuantity"];

							// vthompson - Check to see if product price is null
							if (dr["ProductPrice"] != DBNull.Value)
							{
								receivedValue += (double)dr["ProductPrice"] * (double)dr["GrossQuantity"];
							}
						}
					}
				}

				// Now set the line item's gross received, remaining and value remaining
				// vthompson - Product price may not be created
				if (lineItem.ProductPrice == null)
				{
					lineItem.ProductPrice = 0.0;
				}

				lineItem.GrossQuantityReceived = receivedQty;
				lineItem.GrossQuantityRemaining = lineItem.Quantity.Gross - receivedQty;
				lineItem.ValueRemaining = (lineItem.Quantity.Gross * lineItem.ProductPrice.Value) - receivedValue;
			}
		}

		/// <summary>
		/// This method will retrieve an external interface record from tblExportResultDetails.
		/// It will populate the interface data along with the error text.
		/// </summary>
		/// <param name="trans">
		/// Transaction.
		/// </param>
		protected void RetrieveExternalInterfaceData(TransactionDO trans)
		{
            var exportResultDetails = new ExportResultDetailsClass();
            ExportResultDetailClass exportResultDetail = exportResultDetails.GetByRecordIdAndTransVersion(this.security, trans.TransID, trans.TransVersion);

            if (exportResultDetail != null)
			{
				trans.InterfaceData01 = exportResultDetail.InterfaceData01;
				trans.InterfaceData02 = exportResultDetail.InterfaceData02;
				trans.InterfaceData03 = exportResultDetail.InterfaceData03;
				trans.InterfaceData04 = exportResultDetail.InterfaceData04;
				trans.InterfaceData05 = exportResultDetail.InterfaceData05;
				trans.InterfaceData06 = exportResultDetail.InterfaceData06;
				trans.InterfaceData07 = exportResultDetail.InterfaceData07;
				trans.InterfaceData08 = exportResultDetail.InterfaceData08;
				trans.TransErrorText = exportResultDetail.Error;
			}
		}
	}
}
