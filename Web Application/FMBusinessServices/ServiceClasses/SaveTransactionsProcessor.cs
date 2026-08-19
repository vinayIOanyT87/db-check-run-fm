// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SaveTransactionsProcessor.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the SaveTransactionsProcessorClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Runtime.Serialization.Formatters.Binary;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.Interfaces;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;

	using IsolationLevel = System.Transactions.IsolationLevel;
	using TransactionStatus = FMBusinessObjects.DataObjects.TransactionStatus;

	/// <summary>
	/// The save transactions processor.
	/// </summary>
	[ServiceBehavior(TransactionIsolationLevel = IsolationLevel.ReadCommitted)]
	public class SaveTransactionsProcessor : ISaveTransactionsProcessor
	{
		#region Private attributes

		private TransactionUnitConverter unitsConverter;
		private TransactionHierarchyUtil hierarchyUtil;
		private SecurityClass security;
		private TransactionProcessorClass transactionProcessor;

		private long transVersion;

		private readonly ConsolidatedDAClass consolidatedDa = new ConsolidatedDAClass();

		/// <summary>
		/// Contains information needed by the SaveTransactionsProcessor about the existing version of transactions as they appear in the DB.
		/// The TransactionGuid is the key of the dictionary, the information is the value
		/// </summary>
		private Dictionary<Guid, TransactionPreviousVersionInformation> oldVersionsOfTransactions;

		private Guid fromOwnerCompanyGuid;
		private Guid fromManagerCompanyGuid;
		private Guid fromCarrierCompanyGuid;
		private Guid fromBillToCompanyGuid;
		private Guid fromShipToCompanyGuid;
		private Guid toManagerCompanyGuid;
		private Guid toOwnerCompanyGuid;
		private Guid toCarrierCompanyGuid;
		private Guid toBillToCompanyGuid;
		private Guid toShipToCompanyGuid;

		private string fromManagerId = string.Empty;
		private string fromManagerCode = string.Empty;
		private string fromOwnerId = string.Empty;
		private string fromOwnerCode = string.Empty;
		private string fromCarrierId = string.Empty;
		private string fromCarrierCode = string.Empty;
		private string fromBillToCode = string.Empty;
		private string fromBillToId = string.Empty;
		private string fromShipToCode = string.Empty;
		private string fromShipToId = string.Empty;
		private string toManagerId = string.Empty;
		private string toManagerCode = string.Empty;
		private string toOwnerId = string.Empty;
		private string toOwnerCode = string.Empty;
		private string toCarrierId = string.Empty;
		private string toCarrierCode = string.Empty;
		private string toBillToCode = string.Empty;
		private string toBillToId = string.Empty;
		private string toShipToCode = string.Empty;
		private string toShipToId = string.Empty;

		private List<string> toStorageId;
		private List<string> fromStorageId;
		private List<Guid> toStorageTankGuids;
		private List<Guid> fromStorageTankGuids;

		private List<string> fromProductType;
		private List<string> fromProduct;
		private List<string> fromProductCode;
		private List<Guid> fromProductGuid;
		private List<string> toProductType;
		private List<string> toProduct;
		private List<string> toProductCode;
		private List<Guid> toProductGuid;

		private string initialSubType = string.Empty;
		private string debitTransId = string.Empty;
		private string debitConjoinedId = string.Empty;
		private string reverseDebitTransId = string.Empty;
		private string creditTransId = string.Empty;
		private string creditConjoinedId = string.Empty;
		private string reverseCreditTransId = string.Empty;

		private Guid debitTransGuid = Guid.Empty;
		private Guid creditTransGuid = Guid.Empty;
		private Guid debitTransactionNotesGuid = Guid.Empty;
		private Guid creditTransactionNotesGuid = Guid.Empty;
		private Guid debitTransactionUserDataGuid = Guid.Empty;
		private Guid creditTransactionUserDataGuid = Guid.Empty;
		private Guid debitTransactionSignatureGuid = Guid.Empty;
		private Guid creditTransactionSignatureGuid = Guid.Empty;

		private List<Guid> debitLineItemGuids;
		private List<Guid> debitLineItemConjoinedGuids;
		private List<Guid> debitLineItemUserDataGuids;
		private List<Guid> creditLineItemGuids;
		private List<Guid> creditLineItemConjoinedGuids;
		private List<Guid> creditLineItemUserDataGuids;

		private List<Guid> debitSubLineItemGuids;
		private List<Guid> debitSubLineItemConjoinedGuids;
		private List<Guid> creditSubLineItemGuids;
		private List<Guid> creditSubLineItemConjoinedGuids;

		private List<Guid> debitTransportLineItemGuids;
		private List<Guid> creditTransportLineItemGuids;
		private List<Guid> debitTransportLineItemConjoinedGuids;
		private List<Guid> creditTransportLineItemConjoinedGuids;

		/// <summary>
		/// A list of alarm and event log records to save
		/// </summary>
		private readonly List<AlarmAndEventLogClass> alarmAndEventLogs = new List<AlarmAndEventLogClass>();

		#endregion

		/// <summary>
		/// The save transactions.
		/// </summary>
		/// <param name="sr">
		/// The service request.
		/// </param>
		/// <returns>
		/// The <see cref="SaveTransactionsResultDO"/>.
		/// </returns>
		/// <exception cref="FaultException{TDetail}">
		/// Save transaction exception.
		/// </exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public SaveTransactionsResultDO SaveTransactions(SaveTransactionsSR sr)
		{
			try
			{
				// Process save transaction Flags and Status only.
				if (sr.SubType == SaveTransactionsSR.SaveTransactionSubType.SaveTranactionFlagsAndStatus)
				{
					return this.SaveTransactionFlagsAndStatus(sr);
				}

				this.unitsConverter = null;

				if (sr.ConvertUnits)
				{
					this.unitsConverter = new TransactionUnitConverter(sr.Security, sr.CurrentSiteGuid);
				}

				this.alarmAndEventLogs.Clear();

				this.transactionProcessor = new TransactionProcessorClass();

				SaveTransactionsResultDO result = this.SaveTransaction(sr);

				if (result?.Results?.Find(transResult => transResult.IsValid == false) != null)
				{
					var saveException = new SaveTransactionsException(result.Results);
					throw new FaultException<SaveTransactionsException>(saveException, SaveTransactionsException.FaultExceptionReason);
				}

				this.SaveAlarmAndEventLogRecords(this.alarmAndEventLogs);

				return result;
			}
			catch (Exception e)
			{
				var eventLog = new FMEventLog();
				eventLog.WriteEntry("AccountingBLL - " + e.Message, FMEventLogEntryType.Error);
				throw;
			}
		}

		/// <summary>
		/// This method will save only the Flags and Status.
		/// </summary>
		/// <param name="sr">
		/// The service request.
		/// </param>
		/// <returns>
		/// The <see cref="SaveTransactionsResultDO"/>.
		/// </returns>
		private SaveTransactionsResultDO SaveTransactionFlagsAndStatus(SaveTransactionsSR sr)
		{
			var transResultDO = new SaveTransactionsResultDO();
			this.security = sr.Security;

			if ((sr.TransFlagsAndStatusCollection == null) || (sr.TransFlagsAndStatusCollection.Count <= 0))
			{
				var result = new TransactionValidationResult();
				result.ErrorList.Add("TransactionFlagsAndStatusCollection is null or empty.");
				transResultDO.Results.Add(result);
				return transResultDO;
			}

			var sqlCommand = new SqlCommand();

			string currentTransId = string.Empty;

			try
			{
				foreach (TransactionFlagsAndStatusDO txFlagsAndStatus in sr.TransFlagsAndStatusCollection)
				{
					currentTransId = txFlagsAndStatus.TransID;
					sqlCommand.Parameters.Clear();

					txFlagsAndStatus.UpdateSQLCommand(sqlCommand);

					this.consolidatedDa.ExecuteQuery(this.security, sqlCommand);
				}
			}
			catch (Exception ex)
			{
				var result = new TransactionValidationResult();
				result.ErrorList.Add("Error on Transaction ID: '" + currentTransId + "' - " + ex.Message);
				transResultDO.Results.Add(result);
			}
			finally
			{
				sqlCommand.Dispose();
			}

			return transResultDO;
		}

		/// <summary>
		/// The save transaction.
		/// </summary>
		/// <param name="sr">
		/// The service request.
		/// </param>
		/// <returns>
		/// The <see cref="SaveTransactionsResultDO"/>.
		/// </returns>
		private SaveTransactionsResultDO SaveTransaction(SaveTransactionsSR sr)
		{
			bool validationErrors = false;

			var result = new SaveTransactionsResultDO();

			if (sr?.Transactions == null || sr.Transactions.Count == 0)
			{
				var validationResult = new TransactionValidationResult();
				validationResult.ErrorList.Add("No transactions to save.");
				result.Results.Add(validationResult);
				return result;
			}

			this.security = sr.Security;

			TransactionValidatorClass validator = null;

			// Bypass validation if the user is not a real user or if requested
			if (sr.Security.UserGuid != Guid.Empty && sr.Security.UserID != DBAccess.ServiceLoginAccess && !sr.BypassValidation)
			{
				validator = new TransactionValidatorClass(sr.Security, sr.AccountingSite);
			}

			foreach (TransactionDO trans in sr.Transactions)
			{
				// Validate the transaction
				TransactionValidationResult validationResult = null;

				try
				{
					// If validation is not being bypassed, validate the result
					if (validator != null)
					{
						validationResult = validator.ValidateTransaction(trans);
					}
					else
					{
						validationResult = new TransactionValidationResult { TransID = trans.TransID, AliasName = trans.Alias };
					}

					if (!validationResult.IsValid || validationResult.HasWarnings)
					{
						if (!validationResult.IsValid)
						{
							validationErrors = true;
						}

						result.Results.Add(validationResult);
					}
				}
				catch (Exception e)
				{
					if (validationResult == null)
					{
						validationResult = new TransactionValidationResult { TransID = trans.TransID, AliasName = trans.Alias };
					}

					validationResult.ErrorList.Add(e.Message);
					result.Results.Add(validationResult);
				}
			}

			// If we got any errors during validation, abort saving
			if (validationErrors)
			{
				return result;
			}

			// Add products to ReserveLevelCalculator before loop since saveRegrade
			// alters productid.
			DateTimeOffset inventoryDate = DateTimeOffset.Now;

			var reserveLevelCalculator = new ReserveLevelCalculator();

			foreach (TransactionDO trans in sr.Transactions)
			{
				reserveLevelCalculator.AddProducts(this.security, trans);
				inventoryDate = trans.InventoryDate;
			}

			var hardwareKey = new HardwareKeyClass();

			this.hierarchyUtil = new TransactionHierarchyUtil(sr.Security);

			// Get the previous version of the transactions we're saving.
			// The old version of the transaction facilitates processing at various points in the SaveTransactionsProcessor
			this.oldVersionsOfTransactions = this.GetPreviousVersionOfTransactions(sr);

			var transactionsToSave = new List<TransactionDO>();
			var configSettings = new ConfigurationSettingsClass();
			string enterpriseAssemblies = configSettings.GetKeyValueByKey(this.security, ConfigurationSettingDOClass.Key_AccountingEnterpriseInterface);

			foreach (TransactionDO trans in sr.Transactions)
			{
				if (!string.IsNullOrEmpty(enterpriseAssemblies))
				{
					// Only send final update of transactions from LoadRack to Enterprise
					if (this.security.UserGuid == Guid.Empty && this.security.UserID == DBAccess.ServiceLoginAccess)
					{
						if (trans.Status == TransactionStatus.Completed || trans.Status == TransactionStatus.Cancelled)
						{
							this.SendEnterpriseTransaction(trans, enterpriseAssemblies);
						}
					}
					else if (trans.Status != TransactionStatus.Posted)
					{
						this.SendEnterpriseTransaction(trans, enterpriseAssemblies);
					}
				}

				// Increment the transVersion if the transaction is not a new transaction
				this.transVersion = 0;

				TransactionPreviousVersionInformation oldVersionOfTransaction;
				this.oldVersionsOfTransactions.TryGetValue(trans.TransactionGuid, out oldVersionOfTransaction);

				if (oldVersionOfTransaction != null)
				{
					// check if exising transaction has been posted before saving and inform user if so
					if (oldVersionOfTransaction.Status == TransactionStatus.Posted)
					{
						string error = string.Format("BOL {0} is now in a Posted state and cannot be edited. Any changes will be lost.", trans.DocumentNumber);
						throw new Exception(error);
					}

					this.transVersion = oldVersionOfTransaction.TransVersion;
					this.transVersion++;
				}

				// Set the transaction version per transaction only when necessary
				if (sr.ForceTransVersionUpdate)
				{
					trans.TransVersion = this.transVersion;
				}
				else if (trans.Status != TransactionStatus.Posted && trans.ReversalType != TransactionDO.Original &&
						  (oldVersionOfTransaction == null || trans.DeleteFlag == oldVersionOfTransaction.DeleteFlag) &&
							trans.OriginApplication != TransactionOrigin.EnterpriseUploadTransaction)
				{
					trans.TransVersion = this.transVersion;
				}

				if (sr.ConvertUnits)
				{
					this.unitsConverter.ConvertToSI(trans);
				}

				this.ReverseFinancialSigns(trans);

				if (hardwareKey.IsADFKey() && trans.ReversalType == TransactionDO.Reversal && oldVersionOfTransaction == null)
				{
					foreach (LineItemDO lineItem in trans.LineItems)
					{
						if (lineItem.AlternativeGrossVolume != null)
						{
							lineItem.AlternativeGrossVolume *= -1;
						}

						if (lineItem.AlternativeNetVolume != null)
						{
							lineItem.AlternativeNetVolume *= -1;
						}

						if (lineItem.ReceiptVariance != null)
						{
							lineItem.ReceiptVariance *= -1;
						}
					}
				}

				try
				{
					var relatedTransactions = new List<TransactionDO>();

					TransactionDO original = sr.CreateMissingReversalPieces ? this.OriginalTransaction(sr, trans) : null;

					if (original != null)
					{
						relatedTransactions.Add(original);
					}

					//TODO: We'll need logic here to apply the correct reversal date - current vs. original transaction 
					TransactionDO reversal = sr.CreateMissingReversalPieces ? this.ReversalTransaction(sr, trans, original) : null;

					if (reversal != null)
					{
						relatedTransactions.Add(reversal);
					}

					if (trans.TransTypeID == TransactionTypes.T8_Receipt || trans.TransTypeID == TransactionTypes.T25_Shipment)
					{
						// Find the shipment or receipt corresponding transaction via the transaction's ShipmentNumber.
						// The logic will update the shipment's tranasction's status to completed and make sure the 
						// TransRefIds point to each other
						TransactionDO correspondingTransDO = this.FindShipmentOrReceiptTransaction(trans);

						if (correspondingTransDO != null)
						{
							relatedTransactions.Add(correspondingTransDO);
						}

						TransactionDO supplyOrder = this.AutoCompleteSupplyOrder(sr, trans);

						if (supplyOrder != null)
						{
							relatedTransactions.Add(supplyOrder);
						}
					}
					else if (trans.TransTypeID != TransactionTypes.T13_OwnerTransfer
						 && trans.TransTypeID != TransactionTypes.T11_ConsumerTransfer
						 && trans.TransTypeID != TransactionTypes.T15_PrimaryRegrade
						 && trans.TransTypeID != TransactionTypes.T16_SecondaryRegrade
						 && trans.TransTypeID != TransactionTypes.T23_StorageTransfer)
					{
						// Perform AutoComplete processing, but only for certain transaction types.
						// TODO: The SaveTransactionProcessor's autocomplete logic appears to be flawed and in need of revision - some of the types which the AutoComplete methods are intended to work with 
						// (shipments and receipts) are specifically ignored by the if statement above. This was the way it was working before the SaveTransactionsProcessor was overhauled.

						TransactionDO order = this.AutoCompleteOrder(sr, trans);

						if (order != null)
						{
							relatedTransactions.Add(order);
						}

						TransactionDO supplyOrder = this.AutoCompleteSupplyOrder(sr, trans);

						if (supplyOrder != null)
						{
							relatedTransactions.Add(supplyOrder);
						}
					}

					// Document numbers have to be assigned late, otherwise sequence numbers may be assigned and then not used,
					// leaving gaps in the save sequences
					// Retrieve and increment the Next BOL or order Number as 
					if (string.IsNullOrEmpty(trans.DocumentNumber) && !trans.DeleteFlag)
					{
						var sites = new SitesClass();
						switch (trans.TransTypeID)
						{
							case TransactionTypes.T5_PrimaryDisbursement:
								{
									trans.DocumentNumber = sites.GetNextDocumentNumber(this.security, DOCUMENT_TYPE.MANUAL_BOL, this.security.SiteGuid);
									break;
								}

							case TransactionTypes.T17_Order:
							case TransactionTypes.T18_SupplyOrder:
								{
									trans.DocumentNumber = sites.GetNextDocumentNumber(this.security, DOCUMENT_TYPE.ORDER, this.security.SiteGuid);
									break;
								}
							default:
								{
									trans.DocumentNumber = sites.GetNextDocumentNumber(this.security, DOCUMENT_TYPE.TRANSACTION, this.security.SiteGuid);
									break;
								}
						}

						//propagate document numbers to all line items
						foreach (var lineItem in trans.LineItems)
						{
							lineItem.DocumentNumber = trans.DocumentNumber;
						}
					}

					// Certain types of transactions are conjoined. For conjoined transactions, we create or update two transactions 
					// using only one transaction object. These require special processing.
					switch (trans.TransTypeID)
					{
						case TransactionTypes.T13_OwnerTransfer:
							{
								transactionsToSave.AddRange(this.SaveOwnerTransfer(trans));

								foreach (TransactionDO extraTransaction in relatedTransactions)
								{
									transactionsToSave.AddRange(this.SaveOwnerTransfer(extraTransaction));
								}

								break;
							}

						case TransactionTypes.T11_ConsumerTransfer:
							{
								transactionsToSave.AddRange(this.SaveConsumerTransfer(trans));

								foreach (TransactionDO extraTransaction in relatedTransactions)
								{
									transactionsToSave.AddRange(this.SaveConsumerTransfer(extraTransaction));
								}

								break;
							}

						case TransactionTypes.T15_PrimaryRegrade:
						case TransactionTypes.T16_SecondaryRegrade:
							{
								transactionsToSave.AddRange(this.SaveRegrade(trans));

								foreach (TransactionDO extraTransaction in relatedTransactions)
								{
									transactionsToSave.AddRange(this.SaveRegrade(extraTransaction));
								}

								break;
							}

						case TransactionTypes.T23_StorageTransfer:
							{
								transactionsToSave.AddRange(this.SaveStorageTransfer(trans));

								foreach (TransactionDO extraTransaction in relatedTransactions)
								{
									transactionsToSave.AddRange(this.SaveStorageTransfer(extraTransaction));
								}

								break;
							}

						default:
							{

								transactionsToSave.Add(trans);

								foreach (TransactionDO extraTransaction in relatedTransactions)
								{
									transactionsToSave.Add(extraTransaction);
								}
								break;
							}
					}
				}
				catch (Exception e)
				{
					var validationResult = new TransactionValidationResult { TransID = trans.TransID, AliasName = trans.Alias };

					validationResult.ErrorList.Add(e.Message);
					result.Results.Add(validationResult);

					return result;
				}
			}

			this.SaveTransactions(transactionsToSave, sr);

			// We update the accompanying operator record. This update
			// here is for Dispatching transactions.
			if (sr.Operator != null)
			{
				using (SqlCommand operUpdateCommand = sr.Operator.UpdateSqlCommand(DATA_TYPE.DYNAMIC))
				{
					this.consolidatedDa.ExecuteQuery(this.security, operUpdateCommand);
				}
			}

			if (!string.IsNullOrEmpty(enterpriseAssemblies))
			{
				foreach (TransactionDO trans in sr.Transactions)
				{
					// Only send final update of transactions from LoadRack to Enterprise
					if (this.security.UserGuid == Guid.Empty && this.security.UserID == DBAccess.ServiceLoginAccess)
					{
						if (trans.Status == TransactionStatus.Completed || trans.Status == TransactionStatus.Cancelled)
						{
							this.SendEnterpriseTransaction(trans, enterpriseAssemblies);
						}
					}
					else if (trans.Status != TransactionStatus.Posted)
					{
						this.SendEnterpriseTransaction(trans, enterpriseAssemblies);
					}
				}
			}

			reserveLevelCalculator.CalculateVolume(this.security, inventoryDate, result);

			return result;
		}

		/// <summary>
		/// The save owner transfer.
		/// </summary>
		/// <param name="trans">
		/// The transaction.
		/// </param>
		private IEnumerable<TransactionDO> SaveOwnerTransfer(TransactionDO trans)
		{
			var conjoinedTransactions = new List<TransactionDO>();
			var ownerTransfer = trans as OwnerTransferDO;
			var storageTrans = trans as StorageTransferDO;

			this.fromCarrierCode = trans.CarrierCode;
			this.fromCarrierId = trans.CarrierID;
			this.fromCarrierCompanyGuid = trans.CarrierCompanyGuid;
			this.fromManagerCode = trans.ManagerCode;
			this.fromManagerId = trans.ManagerID;
			this.fromManagerCompanyGuid = trans.ManagerCompanyGuid;
			this.fromOwnerCode = trans.OwnerCode;
			this.fromOwnerId = trans.OwnerID;
			this.fromOwnerCompanyGuid = trans.OwnerCompanyGuid;

			if (ownerTransfer != null)
			{
				this.toCarrierCode = ownerTransfer.ToCarrierCode;
				this.toCarrierId = ownerTransfer.ToCarrierID;
				this.toCarrierCompanyGuid = ownerTransfer.ToCarrierCompanyGuid;
				this.toManagerCode = ownerTransfer.ToManagerCode;
				this.toManagerId = ownerTransfer.ToManagerID;
				this.toManagerCompanyGuid = ownerTransfer.ToManagerCompanyGuid;
				this.toOwnerCode = ownerTransfer.ToOwnerCode;
				this.toOwnerId = ownerTransfer.ToOwnerID;
				this.toOwnerCompanyGuid = ownerTransfer.ToOwnerCompanyGuid;
			}

			this.fromStorageId = new List<string>();
			this.fromStorageTankGuids = new List<Guid>();
			this.toStorageId = new List<string>();
			this.toStorageTankGuids = new List<Guid>();
			
			foreach (LineItemDO lineItem in trans.LineItems)
			{
				this.fromStorageId.Add(lineItem.StorageLocationID);
				this.fromStorageTankGuids.Add(lineItem.StorageLocationTankGuid);

				var transferLineItem = lineItem as StorageTransferLineItemDO;
				if (transferLineItem == null)
				{
					throw new Exception("Storage loction not found");
				}
				this.toStorageId.Add(transferLineItem.ToStorageLocation);
				this.toStorageTankGuids.Add(transferLineItem.ToStorageLocationTankGuid);
            }

			// The following statement must be invoked prior to the saves due to
			// debit/credit settings that will get changed.
			this.SetTransAndConjoinedTransIDs(trans);
			TransactionDO credit = CopyTransaction(trans);

			// set the destination storage location
			this.SaveDebitConjoinedTransaction(trans);
			conjoinedTransactions.Add(trans);

			this.SaveCreditConjoinedTransaction(credit);
			conjoinedTransactions.Add(credit);

			return conjoinedTransactions;
		}

		/// <summary>
		/// The save consumer transfer.
		/// </summary>
		/// <param name="trans">
		/// The trans.
		/// </param>
		private IEnumerable<TransactionDO> SaveConsumerTransfer(TransactionDO trans)
		{
			var conjoinedTransactions = new List<TransactionDO>();
			var consumerTransfer = trans as ConsumerTransferDO;

			this.fromBillToCode = trans.BillToCode;
			this.fromBillToId = trans.BillToID;
			this.fromBillToCompanyGuid = trans.BillToCompanyGuid;
			this.fromShipToCode = trans.ShipToCode;
			this.fromShipToId = trans.ShipToID;
			this.fromShipToCompanyGuid = trans.ShipToCompanyGuid;

			if (consumerTransfer != null)
			{
				this.toBillToCode = consumerTransfer.ToBillToCode;
				this.toBillToId = consumerTransfer.ToBillToID;
				this.toBillToCompanyGuid = consumerTransfer.ToBillToCompanyGuid;
				this.toShipToCode = consumerTransfer.ToShipToCode;
				this.toShipToId = consumerTransfer.ToShipToID;
				this.toShipToCompanyGuid = consumerTransfer.ToShipToCompanyGuid;
			}

			// The following statement must be invoked prior to the saves due to
			// debit/credit settings that will get changed.
			this.SetTransAndConjoinedTransIDs(trans);
			TransactionDO credit = CopyTransaction(trans);

			this.SaveDebitConjoinedTransaction(trans);
			conjoinedTransactions.Add(trans);

			this.SaveCreditConjoinedTransaction(credit);
			conjoinedTransactions.Add(credit);

			return conjoinedTransactions;
		}

		/// <summary>
		/// The save storage transfer.
		/// </summary>
		/// <param name="trans">
		/// The transaction.
		/// </param>
		private IEnumerable<TransactionDO> SaveStorageTransfer(TransactionDO trans)
		{
			var conjoinedTransactions = new List<TransactionDO>();

			var storageTrans = trans as StorageTransferDO;

			this.fromStorageId = new List<string>();
			this.fromStorageTankGuids = new List<Guid>();
			this.toStorageId = new List<string>();
			this.toStorageTankGuids = new List<Guid>();

			this.PersistStorageLocations(storageTrans);

			this.SetTransAndConjoinedTransIDs(trans);
			TransactionDO credit = CopyTransaction(trans);

			this.SaveDebitConjoinedTransaction(trans);
			conjoinedTransactions.Add(trans);

			this.SaveCreditConjoinedTransaction(credit);
			conjoinedTransactions.Add(credit);

			return conjoinedTransactions;
		}

		/// <summary>
		/// This method creates the To/From products and populates the values.
		/// In addition, the To/From storage location are created and populated.
		/// </summary>
		/// <param name="trans">The transaction to be saved.</param>
		private IEnumerable<TransactionDO> SaveRegrade(TransactionDO trans)
		{
			var conjoinedTransactions = new List<TransactionDO>();
			var regrade = trans as RegradeDO;

			this.fromProduct = new List<string>();
			this.fromProductCode = new List<string>();
			this.fromProductType = new List<string>();
			this.fromProductGuid = new List<Guid>();
			this.toProduct = new List<string>();
			this.toProductCode = new List<string>();
			this.toProductType = new List<string>();
			this.toProductGuid = new List<Guid>();

			this.fromStorageId = new List<string>();
			this.fromStorageTankGuids = new List<Guid>();
			this.toStorageId = new List<string>();
			this.toStorageTankGuids = new List<Guid>();

			this.PersistProducts(regrade);

			// The following statement must be invoked prior to the saves due to
			// debit/credit settings that will get changed.
			this.SetTransAndConjoinedTransIDs(regrade);
			TransactionDO credit = CopyTransaction(trans);

			this.SaveDebitConjoinedTransaction(regrade);
			conjoinedTransactions.Add(regrade);

			this.SaveCreditConjoinedTransaction(credit);
			conjoinedTransactions.Add(credit);

			return conjoinedTransactions;
		}

		/// <summary>
		/// This function returns the Reverse Transaction if the reversal type needs to be changed
		/// or the reversal needs to be added.  This will occur when an update is deleted
		/// or when an update is added
		/// </summary>
		/// <param name="sr">
		/// The service request.
		/// </param>
		/// <param name="trans">
		/// The transaction data object.
		/// </param>
		/// <param name="origTrans">
		/// Original transaction being reversed.  This is used to set the inventory date of the reversal if that is the date we use.
		/// </param>
		/// <returns>
		/// The reversed transaction data object <see cref="TransactionDO"/>.
		/// </returns>
		private TransactionDO ReversalTransaction(SaveTransactionsSR sr, TransactionDO trans, TransactionDO origTrans)
		{
			if (trans.ReversalType != TransactionDO.Update
				 || string.IsNullOrEmpty(trans.ReversedTransID)
				 || !sr.Security.EnableChangeTracking)
			{
				return null;
			}

			var getTransactionSr = new GetTransactionSR
			{
				Security = sr.Security,
				Request = GetTransactionRequest.SITE_TYPEID_REVERSEDTRANSID,
				ReversedTransID = trans.ReversedTransID,
				TransTypeID = trans.TransTypeID
			};

			string reversalTransId = null;

			// Security must use the site guid of the transaction instead of
			// the currently logged on site
			Guid currentSiteGuid = sr.Security.SiteGuid;
			string currentSiteId = sr.Security.SiteID;
			GetTransactionDO getTransactionDO;

			try
			{
				getTransactionSr.Security.SiteGuid = trans.SiteGuid;
				getTransactionSr.Security.SiteID = trans.Site;

				var getTxProcessor = new GetTransactionProcessorClass();
				getTransactionDO = getTxProcessor.Process(getTransactionSr);
			}
			finally
			{
				// Restore current site details
				this.security.SiteGuid = currentSiteGuid;
				this.security.SiteID = currentSiteId;
			}

			if (getTransactionDO?.TransactionDataSet != null && getTransactionDO.TransactionDataSet.Tables.Count == 1)
			{
				foreach (DataRow row in getTransactionDO.TransactionDataSet.Tables[0].Rows)
				{
					// Select the Reversal associated with the update
					if (row["ReversalType"] != null &&
						 (row["ReversalType"] as string == TransactionDO.Reversal || row["ReversalType"] as string == TransactionDO.ReversalWithUpdate))
					{
						reversalTransId = row["TransID"] as string;
						break;
					}
				}
			}

			// Setting reversalTransID to transReversedTransID in preparation to retrieve
			// the original transaction such that a reversal can be prepared
			if (string.IsNullOrEmpty(reversalTransId))
			{
				reversalTransId = trans.ReversedTransID;
			}

			var transactionSr = new TransactionSR
			{
				Security = sr.Security,
				TransID = reversalTransId,
				AccountingSite = sr.AccountingSite,
				ConvertUnits = false
			};

			TransactionDO reversal = this.transactionProcessor.Process(transactionSr);

			if (reversal.DeleteFlag)
			{
				return null;
			}

			// When Update is not deleted insure original ReversalType is Original
			if (trans.DeleteFlag == false)
			{
				if (reversal.ReversalType == TransactionDO.ReversalWithUpdate)
				{
					return null;
				}

				reversal.ReversalType = TransactionDO.ReversalWithUpdate;
			}
			else
			{
				if (reversal.ReversalType == TransactionDO.Reversal)
				{
					return null;
				}

				reversal.ReversalType = TransactionDO.Reversal;
			}

			// When there is no reversal prepare one
			if (reversal.TransID == trans.ReversedTransID)
			{
				reversal.TransVersion = this.transVersion;
				reversal.Status = TransactionStatus.Completed;
				reversal.ReversedTransID = reversal.TransID;
				reversal.ConjoinReversedTransID = reversal.ConjoinedTransID;
				reversal.TransID = FuelsManagerId.NewId();
				reversal.TransactionGuid = Guid.Empty;
				reversal.ConjoinedTransactionGuid = Guid.Empty;
				reversal.TransactionSignatureGuid = Guid.Empty;
				reversal.ConjoinedSignatureGuid = Guid.Empty;
				reversal.TransactionNoteGuid = Guid.Empty;
				reversal.ConjoinedNotesGuid = Guid.Empty;
				reversal.TransactionUserDataGuid = Guid.Empty;
				reversal.ConjoinedUserDataGuid = Guid.Empty;

				// Determine if the original transaction contains a conjoined transaction ID.
				// If so, then we need to create a new conjoined transaction ID.
				if (string.IsNullOrEmpty(reversal.ConjoinedTransID) == false)
				{
					reversal.ConjoinedTransID = FuelsManagerId.NewId();
				}

				reversal.TransactionDateTime = trans.TransactionDateTime;
				reversal.CloseoutDate = null;
				reversal.PartialCloseout = false;

				if (origTrans == null)
				{
					reversal.InventoryDate = trans.InventoryDate;
				}
				else
				{
					var genConfigSr = new GeneralConfigSR
					{
						Security = this.security,
						Request = GeneralConfigSR.GeneralConfigurationRequests.GET_CONFIGURATION
					};
					GeneralConfigProcessorClass gcProcessor = new GeneralConfigProcessorClass();

					GeneralConfigDO genConfigDO = gcProcessor.Get(genConfigSr);

					if (genConfigDO.ReverseTransactionDateMode == "Original")
					{
						// Make sure to keep the original inventory date
						reversal.InventoryDate = origTrans.InventoryDate;
					}
					else
					{
						reversal.InventoryDate = trans.InventoryDate;
					}
				}

				var hardwareKey = new HardwareKeyClass();

				// Only for Defense.
				if (hardwareKey.IsDescKey())
				{
					// the date03 field holds an ebs process date.  flag06 is a flag saying 
					if (reversal.Flag06)
					{
						reversal.Date03 = null;
					}

					reversal.SubmittedToAccounting = true;
					reversal.Flag05 = false;
					reversal.Flag06 = false;
				}

				// Only for NSPA
				if (hardwareKey.IsNspaEnterpriseKey() || hardwareKey.IsNspaProfessionalKey())
				{
					// Reset the Sent to SAP flag.
					reversal.Flag05 = false;

					// Reset the Process by SAP flag.
					reversal.Flag06 = false;

					// Reset error flag for SAP Interface.
					reversal.ErrorFlag = false;
				}

				// Reverse the quantities for both the line items and any
				// sub-line items.
				foreach (LineItemDO lineItem in reversal.LineItems)
				{
					lineItem.TransactionLineItemGuid = Guid.Empty;
					lineItem.ConjoinedTransactionLineItemGuid = Guid.Empty;
					lineItem.TransactionLineItemUserDataGuid = Guid.Empty;
					lineItem.ConjoinedTransactionLineItemUserDataGuid = Guid.Empty;

					lineItem.Quantity.GrossInventoryChange *= -1;
					lineItem.Quantity.NetInventoryChange *= -1;
					lineItem.Quantity.MassInventoryChange *= -1;
					lineItem.Quantity.PackageInventoryChange *= -1;

					lineItem.CloseoutDate = null;

					// Only for NSPA
					if (hardwareKey.IsNspaEnterpriseKey() || hardwareKey.IsNspaProfessionalKey())
					{
						// NSPA SAP Interface needs the line item to be set to completed
						// on the reverse.
						lineItem.Status = TransactionStatus.Completed;
					}

					TransactionPreviousVersionInformation oldVersionOfTransaction;
					this.oldVersionsOfTransactions.TryGetValue(trans.TransactionGuid, out oldVersionOfTransaction);

					if (hardwareKey.IsADFKey() && oldVersionOfTransaction == null)
					{
						if (lineItem.AlternativeGrossVolume != null)
						{
							lineItem.AlternativeGrossVolume *= -1;
						}

						if (lineItem.AlternativeNetVolume != null)
						{
							lineItem.AlternativeNetVolume *= -1;
						}

						if (lineItem.ReceiptVariance != null)
						{
							lineItem.ReceiptVariance *= -1;
						}
					}

					foreach (SubLineItemDO sublineItem in lineItem.SubLineItems)
					{
						sublineItem.TransactionSubLineItemGuid = Guid.Empty;
						sublineItem.ConjoinedTransactionSubLineItemGuid = Guid.Empty;
						sublineItem.Quantity.GrossInventoryChange *= -1;
						sublineItem.Quantity.NetInventoryChange *= -1;
						sublineItem.Quantity.MassInventoryChange *= -1;
						sublineItem.Quantity.PackageInventoryChange *= -1;
						sublineItem.CloseoutDate = null;
					}
				}

				foreach (TransportLineItemDO transportLineItem in reversal.TransportInfoList)
				{
					transportLineItem.TransactionTransportLineItemGuid = Guid.Empty;
					transportLineItem.ConjoinedTransactionTransportLineItemGuid = Guid.Empty;
				}

				this.ReverseFinancialSigns(reversal);

				if (reversal.TransPIDXCollection != null)
				{
					foreach (TransactionPIDXDO transactionPidxDo in reversal.TransPIDXCollection)
					{
						transactionPidxDo.TransactionGuid = Guid.Empty;
						transactionPidxDo.SentFlag = false;
						transactionPidxDo.AuthorizationNumber = string.Empty;
					}
				}
			}

			return reversal;
		}

		/// <summary>
		/// This function returns the Original Transaction, either and Original or an Update,
		/// if the reversal type needs to be revised.  This will occur when a reversal
		/// is deleted or when a reversal or an update is added
		/// </summary>
		/// <param name="sr">
		/// The service request.
		/// </param>
		/// <param name="trans">
		/// The transaction.
		/// </param>
		/// <returns>
		/// The <see cref="TransactionDO"/>.
		/// </returns>
		private TransactionDO OriginalTransaction(SaveTransactionsSR sr, TransactionDO trans)
		{
			// When trans is a reversal or an update it may be necessary to update the original
			if ((trans.ReversalType != TransactionDO.Reversal && trans.ReversalType != TransactionDO.Update)
				 || string.IsNullOrEmpty(trans.ReversedTransID)
				 || !sr.Security.EnableChangeTracking)
			{
				return null;
			}

			// Deletion of an update has no affect on the original
			if (trans.ReversalType == TransactionDO.Update && trans.DeleteFlag)
			{
				return null;
			}

			var transactionSr = new TransactionSR
			{
				Security = sr.Security,
				TransID = trans.ReversedTransID,
				AccountingSite = sr.AccountingSite,
				ConvertUnits = false
			};

			TransactionDO original = this.transactionProcessor.Process(transactionSr);

			if (original == null)
			{
				var hardwareKey = new HardwareKeyClass();
				if (hardwareKey.IsDescKey())
				{
					throw new Exception("Original transaction does not exist!");
				}
				return null;
			}

			if (original.DeleteFlag)
			{
				return null;
			}

			// When Reversal is not deleted ensure original ReversalType is Original
			if (!trans.DeleteFlag)
			{
				if (original.ReversalType == TransactionDO.Original || original.ReversalType == TransactionDO.UpdateOriginal)
				{
					return null;
				}

				if (original.ReversalType == TransactionDO.None)
				{
					original.ReversalType = TransactionDO.Original;
				}

				if (original.ReversalType == TransactionDO.Update)
				{
					original.ReversalType = TransactionDO.UpdateOriginal;
				}
			}
			else
			{
				// When Reversal is deleted ensure original ReversalType is None
				if (original.ReversalType == TransactionDO.None || original.ReversalType == TransactionDO.Update)
				{
					return null;
				}

				if (original.ReversalType == TransactionDO.Original)
				{
					original.ReversalType = TransactionDO.None;
				}

				if (original.ReversalType == TransactionDO.UpdateOriginal)
				{
					original.ReversalType = TransactionDO.Update;
				}
			}

			return original;
		}

		/// <summary>
		/// This method will retrieve the previous version of the (what's in the database) 
		/// transactions we are saving. 
		/// </summary>
		/// <param name="sr">
		/// The service request. Contains the transactions we're saving
		/// </param>
		/// <returns>
		/// A dictionary with the TransactionGuid as the key and the previous version information as the value.
		/// </returns>
		private Dictionary<Guid, TransactionPreviousVersionInformation> GetPreviousVersionOfTransactions(SaveTransactionsSR sr)
		{
			List<Guid> transactionGuids = sr.Transactions.Select(transaction => transaction.TransactionGuid).ToList();

			// Add the conjoined transactions to the list. Also ensure the GUIDS are distinct.
			transactionGuids.AddRange(sr.Transactions.Select(transaction => transaction.ConjoinedTransactionGuid).ToList());
			transactionGuids = transactionGuids.Distinct().ToList();

			Dictionary<Guid, TransactionPreviousVersionInformation> previousTransactionVersions =
				 this.transactionProcessor.GetPreviousTransactionInformation(sr.Security, transactionGuids);

			return previousTransactionVersions;
		}

		/// <summary>
		/// This function completes the order which is fulfilled
		/// by the disbursement transaction.
		/// </summary>
		/// <param name="sr">Service Request</param>
		/// <param name="trans">A disbursement transaction which fulfills an order</param>
		/// <returns>
		/// A Transaction data object containing the order which was auto completed
		/// null, if no order was auto completed.
		/// </returns>
		private TransactionDO AutoCompleteOrder(SaveTransactionsSR sr, TransactionDO trans)
		{
			if (!sr.UseAutoComplete)
			{
				return null;
			}

			// Do not need to account for "Posted" status here
			if ((trans.TransTypeID != TransactionTypes.T5_PrimaryDisbursement
				 && trans.TransTypeID != TransactionTypes.T6_SecondaryDisbursement
				 && trans.TransTypeID != TransactionTypes.T25_Shipment)
				 || string.IsNullOrEmpty(trans.TransRefID)
				 || trans.Status != TransactionStatus.Completed
				 || trans.ReversalType == TransactionDO.Reversal
				 || trans.ReversalType == TransactionDO.ReversalWithUpdate
				 || trans.ReversalType == TransactionDO.Update
				 || trans.ReversalType == TransactionDO.UpdateOriginal)
			{
				return null;
			}

			TransactionSR transactionSr = new TransactionSR
			{
				Security = sr.Security,
				TransID = trans.TransRefID,
				AccountingSite = sr.AccountingSite,
				ConvertUnits = false
			};

			TransactionDO order = this.transactionProcessor.Process(transactionSr);

			if (order == null || !order.AutoComplete)
			{
				return null;
			}

			bool changed = false;

			foreach (LineItemDO transLineItem in trans.LineItems)
			{
				// Do not need to test for TransactionStatus.Posted here
				if (transLineItem.Status == TransactionStatus.Completed)
				{
					foreach (LineItemDO orderLineItem in order.LineItems)
					{
						if (transLineItem.OrderReferenceTransactionLineItemGuid == orderLineItem.TransactionLineItemGuid)
						{
							if ((orderLineItem.Status != TransactionStatus.Completed) &&
									  (orderLineItem.Status != TransactionStatus.Posted))
							{
								orderLineItem.Status = TransactionStatus.Completed;
								changed = true;
							}

							break;
						}
					}
				}
			}

			if (!changed)
			{
				return null;
			}

			bool allItemsComplete = true;

			foreach (LineItemDO orderLineItem in order.LineItems)
			{
				if (orderLineItem.Status != TransactionStatus.Completed
					 && orderLineItem.Status != TransactionStatus.Cancelled
					 && orderLineItem.Status != TransactionStatus.Posted)
				{
					allItemsComplete = false;
					break;
				}
			}

			if (allItemsComplete && (order.Status != TransactionStatus.Posted))
			{
				order.Status = TransactionStatus.Completed;
			}

			return order;
		}

		/// <summary>
		/// This method will setup and save the debit portion of a transfer or re-grade.
		/// </summary>
		/// <param name="trans">Transaction data object.</param>
		private void SaveDebitConjoinedTransaction(TransactionDO trans)
		{
			// Set the correct trans ID and conjoined ID for this transaction.
			trans.TransactionGuid = this.debitTransGuid;
			trans.TransID = this.debitTransId;
			trans.ConjoinedTransID = this.debitConjoinedId;
			trans.SubType = TransactionDO.DEBIT;
			trans.TransactionNoteGuid = this.debitTransactionNotesGuid;
			trans.TransactionSignatureGuid = this.debitTransactionSignatureGuid;
			trans.TransactionUserDataGuid = this.debitTransactionUserDataGuid;
			trans.ConjoinedTransactionGuid = this.creditTransGuid;

			switch (trans.TransTypeID)
			{
				case TransactionTypes.T11_ConsumerTransfer:
					{
						trans.ShipToID = this.fromShipToId;
						trans.ShipToCode = this.fromShipToCode;
						trans.ShipToCompanyGuid = this.fromShipToCompanyGuid;
						trans.BillToCode = this.fromBillToCode;
						trans.BillToID = this.fromBillToId;
						trans.BillToCompanyGuid = this.fromBillToCompanyGuid;

						// Set the appropriate line item GUIDs.  In this case the debit line
						// item GUIDs.
						int nextGuid = 0;
						foreach (LineItemDO lineItem in trans.LineItems)
						{
							lineItem.TransactionLineItemGuid = this.debitLineItemGuids[nextGuid];
							lineItem.ConjoinedTransactionLineItemGuid = this.debitLineItemConjoinedGuids[nextGuid];
							lineItem.TransactionLineItemUserDataGuid = this.debitLineItemUserDataGuids[nextGuid];
							nextGuid++;

							// Set the appropriate sub line item GUIDs. In this case the debit 
							// sub line item GUIDs.
							int nextSubGuid = 0;

							foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
							{
								subLineItem.TransactionSubLineItemGuid = this.debitSubLineItemGuids[nextSubGuid];
								subLineItem.ConjoinedTransactionSubLineItemGuid = this.debitSubLineItemConjoinedGuids[nextSubGuid];
								nextSubGuid++;
							}
						}

						break;
					}

				case TransactionTypes.T13_OwnerTransfer:
					{
						trans.CarrierCode = this.fromCarrierCode;
						trans.CarrierID = this.fromCarrierId;
						trans.CarrierCompanyGuid = this.fromCarrierCompanyGuid;
						trans.ManagerCode = this.fromManagerCode;
						trans.ManagerID = this.fromManagerId;
						trans.ManagerCompanyGuid = this.fromManagerCompanyGuid;
						trans.OwnerCode = this.fromOwnerCode;
						trans.OwnerID = this.fromOwnerId;
						trans.OwnerCompanyGuid = this.fromOwnerCompanyGuid;

						// Set the appropriate line item GUIDs.  In this case the debit line
						// item GUIDs.
						int nextGuid = 0;
						foreach (LineItemDO lineItem in trans.LineItems)
						{
							lineItem.StorageLocationID = this.fromStorageId[nextGuid];
							lineItem.StorageLocationTankGuid = this.fromStorageTankGuids[nextGuid];

							lineItem.TransactionLineItemGuid = this.debitLineItemGuids[nextGuid];
							lineItem.ConjoinedTransactionLineItemGuid = this.debitLineItemConjoinedGuids[nextGuid];
							lineItem.TransactionLineItemUserDataGuid = this.debitLineItemUserDataGuids[nextGuid];
							nextGuid++;

							int nextSubGuid = 0;

							// Set the appropriate sub line item GUIDs. In this case the debit 
							// sub line item GUIDs.
							foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
							{
								subLineItem.TransactionSubLineItemGuid = this.debitSubLineItemGuids[nextSubGuid];
								subLineItem.ConjoinedTransactionSubLineItemGuid = this.debitSubLineItemConjoinedGuids[nextSubGuid];
								nextSubGuid++;
							}
						}

						break;
					}

				case TransactionTypes.T15_PrimaryRegrade:
				case TransactionTypes.T16_SecondaryRegrade:
					{
						for (int next = 0; next < trans.LineItems.Count; next++)
						{
							LineItemDO lineItem = trans.LineItems[next];
							lineItem.Product = this.fromProduct[next];
							lineItem.ProductCode = this.fromProductCode[next];
							lineItem.ProductType = this.fromProductType[next];
							lineItem.ProductGuid = this.fromProductGuid[next];
							lineItem.StorageLocationID = this.fromStorageId[next];
							lineItem.StorageLocationTankGuid = this.fromStorageTankGuids[next];

							// Set the appropriate line item GUIDs.  In this case the debit line
							// item GUIDs.
							lineItem.TransactionLineItemGuid = this.debitLineItemGuids[next];
							lineItem.ConjoinedTransactionLineItemGuid = this.debitLineItemConjoinedGuids[next];
							lineItem.TransactionLineItemUserDataGuid = this.debitLineItemUserDataGuids[next];

							// Note that Regrades should not have any sub-line items coming down from the upper layers - 
							// Nature of a regrade requires that the sublineitems for blends be created here in the correct proportion
							lineItem.SubLineItems.Clear();

							if (lineItem.ProductType == ProductClass.ProductTypeID(ProductType.BlendProduct))
							{
								var products = new ProductsClass();
								ProductClass blend = products.GetByProductAuthorizedCompanies(this.security, lineItem.ProductGuid, false);

								// Now add a sub-line item for each of the product's components
								foreach (ProductMapClass productMap in blend.ComponentCollection)
								{
									var subLineItem = new SubLineItemDO
									{
										ArmNumber = lineItem.ArmNumber,
										BatchNumber = lineItem.BatchNumber,
										Status = lineItem.Status,
										Product = productMap.AssignedID,
										ProductCode = productMap.AssignedCode,
										ProductType = ProductClass.ProductTypeID(productMap.AssignedProductType),
										ProductGuid = productMap.AssignedGuid,
										Quantity =
																						  {
																								GrossInventoryChange = lineItem.Quantity.GrossInventoryChange
																									 * productMap.BlendPercentage / 100.0,
																								NetInventoryChange = lineItem.Quantity.NetInventoryChange
																									 * productMap.BlendPercentage / 100.0
																						  }
									};

									lineItem.SubLineItems.Add(subLineItem);
								}
							}
						}

						break;
					}

				case TransactionTypes.T23_StorageTransfer:
					{
						int nextGuid = 0;
						foreach (LineItemDO lineItem in trans.LineItems)
						{
							lineItem.StorageLocationID = this.fromStorageId[nextGuid];
							lineItem.StorageLocationTankGuid = this.fromStorageTankGuids[nextGuid];
							lineItem.TransactionLineItemGuid = this.debitLineItemGuids[nextGuid];
							lineItem.ConjoinedTransactionLineItemGuid = this.debitLineItemConjoinedGuids[nextGuid];
							lineItem.TransactionLineItemUserDataGuid = this.debitLineItemUserDataGuids[nextGuid];

							nextGuid++;

							// Set the appropriate sub line item GUIDs. In this case the debit 
							// sub line item GUIDs.
							int nextSubGuid = 0;

							foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
							{
								subLineItem.TransactionSubLineItemGuid = this.debitSubLineItemGuids[nextSubGuid];
								subLineItem.ConjoinedTransactionSubLineItemGuid = this.debitSubLineItemConjoinedGuids[nextSubGuid];
								nextSubGuid++;
							}
						}

						break;
					}
			}

			int nextTransportGuid = 0;
			foreach (TransportLineItemDO transportLine in trans.TransportInfoList)
			{
				transportLine.TransactionTransportLineItemGuid = this.debitTransportLineItemGuids[nextTransportGuid];
				transportLine.ConjoinedTransactionTransportLineItemGuid = this.debitTransportLineItemConjoinedGuids[nextTransportGuid++];
				transportLine.TransactionGuid = this.debitTransGuid;
			}

			trans.ReversedTransID = this.reverseDebitTransId;

			this.ReverseQuantitySigns(trans);
		}

		/// <summary>
		/// This method will setup and save the credit portion of a transfer or re-grade.
		/// </summary>
		/// <param name="trans">Transaction data object.</param>
		private void SaveCreditConjoinedTransaction(TransactionDO trans)
		{
			// Set the correct trans ID and conjoined ID for this transaction.
			trans.TransactionGuid = this.creditTransGuid;
			trans.TransID = this.creditTransId;
			trans.ConjoinedTransID = this.creditConjoinedId;
			trans.SubType = TransactionDO.CREDIT;
			trans.TransactionNoteGuid = this.creditTransactionNotesGuid;
			trans.TransactionSignatureGuid = this.creditTransactionSignatureGuid;
			trans.TransactionUserDataGuid = this.creditTransactionUserDataGuid;
			trans.ConjoinedTransactionGuid = this.debitTransGuid;

			switch (trans.TransTypeID)
			{
				case TransactionTypes.T11_ConsumerTransfer:
					{
						trans.ShipToID = this.toShipToId;
						trans.ShipToCode = this.toShipToCode;
						trans.ShipToCompanyGuid = this.toShipToCompanyGuid;
						trans.BillToCode = this.toBillToCode;
						trans.BillToID = this.toBillToId;
						trans.BillToCompanyGuid = this.toBillToCompanyGuid;

						// Set the appropriate line item GUIDs.  In this case the credit line
						// item GUIDs.
						int nextGuid = 0;
						foreach (LineItemDO lineItem in trans.LineItems)
						{
							lineItem.TransactionLineItemGuid = this.creditLineItemGuids[nextGuid];
							lineItem.ConjoinedTransactionLineItemGuid = this.creditLineItemConjoinedGuids[nextGuid];
							lineItem.TransactionLineItemUserDataGuid = this.creditLineItemUserDataGuids[nextGuid];
							nextGuid++;

							// Set the appropriate sub line item GUIDs. In this case the credit 
							// sub line item GUIDs.
							int nextSubGuid = 0;

							foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
							{
								subLineItem.TransactionSubLineItemGuid = this.creditSubLineItemGuids[nextSubGuid];
								subLineItem.ConjoinedTransactionSubLineItemGuid = this.creditSubLineItemConjoinedGuids[nextSubGuid];
								nextSubGuid++;
							}
						}

						break;
					}

				case TransactionTypes.T13_OwnerTransfer:
					{
						trans.CarrierCode = this.toCarrierCode;
						trans.CarrierID = this.toCarrierId;
						trans.CarrierCompanyGuid = this.toCarrierCompanyGuid;
						trans.ManagerCode = this.toManagerCode;
						trans.ManagerID = this.toManagerId;
						trans.ManagerCompanyGuid = this.toManagerCompanyGuid;
						trans.OwnerCode = this.toOwnerCode;
						trans.OwnerID = this.toOwnerId;
						trans.OwnerCompanyGuid = this.toOwnerCompanyGuid;

						// Set the appropriate line item GUIDs.  In this case the credit line
						// item GUIDs.
						int nextGuid = 0;
						foreach (LineItemDO lineItem in trans.LineItems)
						{
							lineItem.StorageLocationID = this.toStorageId[nextGuid];
							lineItem.StorageLocationTankGuid = this.toStorageTankGuids[nextGuid];

							lineItem.TransactionLineItemGuid = this.creditLineItemGuids[nextGuid];
							lineItem.ConjoinedTransactionLineItemGuid = this.creditLineItemConjoinedGuids[nextGuid];
							lineItem.TransactionLineItemUserDataGuid = this.creditLineItemUserDataGuids[nextGuid];
							nextGuid++;

							// Set the appropriate sub line item GUIDs. In this case the credit 
							// sub line item GUIDs.
							int nextSubGuid = 0;

							foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
							{
								subLineItem.TransactionSubLineItemGuid = this.creditSubLineItemGuids[nextSubGuid];
								subLineItem.ConjoinedTransactionSubLineItemGuid = this.creditSubLineItemConjoinedGuids[nextSubGuid];
								nextSubGuid++;
							}
						}
						break;
					}

				case TransactionTypes.T15_PrimaryRegrade:
				case TransactionTypes.T16_SecondaryRegrade:
					{
						var products = new ProductsClass();

						for (int next = 0; next < trans.LineItems.Count; next++)
						{
							LineItemDO lineItem = trans.LineItems[next];
							lineItem.Product = this.toProduct[next];
							lineItem.ProductCode = this.toProductCode[next];
							lineItem.ProductType = this.toProductType[next];
							lineItem.ProductGuid = this.toProductGuid[next];
							lineItem.StorageLocationID = this.toStorageId[next];
							lineItem.StorageLocationTankGuid = this.toStorageTankGuids[next];

							// Set the appropriate line item GUIDs.  In this case the credit line
							// item GUIDs.
							lineItem.TransactionLineItemGuid = this.creditLineItemGuids[next];
							lineItem.ConjoinedTransactionLineItemGuid = this.creditLineItemConjoinedGuids[next];
							lineItem.TransactionLineItemUserDataGuid = this.creditLineItemUserDataGuids[next];

							// Note that Regrades should not have any sub-line items coming down from the upper layers - 
							// Nature of a regrade requires that the sublineitems for blends be created here in the correct proportion
							lineItem.SubLineItems.Clear();

							if (lineItem.ProductType == ProductClass.ProductTypeID(ProductType.BlendProduct))
							{
								ProductClass blend = products.GetByProductAuthorizedCompanies(this.security, lineItem.ProductGuid, false);

								// Now add a sub-line item for each of the product's components
								foreach (ProductMapClass productMap in blend.ComponentCollection)
								{
									var subLineItem = new SubLineItemDO
									{
										ArmNumber = lineItem.ArmNumber,
										BatchNumber = lineItem.BatchNumber,
										Status = lineItem.Status,
										Product = productMap.AssignedID,
										ProductCode = productMap.AssignedCode,
										ProductType = ProductClass.ProductTypeID(productMap.AssignedProductType),
										ProductGuid = productMap.AssignedGuid,
										Quantity =
																						{
																							 GrossInventoryChange = lineItem.Quantity.GrossInventoryChange
																								  * productMap.BlendPercentage / 100.0,
																							 NetInventoryChange = lineItem.Quantity.NetInventoryChange
																								  * productMap.BlendPercentage / 100.0
																						}
									};

									lineItem.SubLineItems.Add(subLineItem);
								}
							}
						}

						break;
					}

				case TransactionTypes.T23_StorageTransfer:
					{
						int nextGuid = 0;
						foreach (LineItemDO lineItem in trans.LineItems)
						{
							lineItem.StorageLocationID = this.toStorageId[nextGuid];
							lineItem.StorageLocationTankGuid = this.toStorageTankGuids[nextGuid];
							lineItem.TransactionLineItemGuid = this.creditLineItemGuids[nextGuid];
							lineItem.ConjoinedTransactionLineItemGuid = this.creditLineItemConjoinedGuids[nextGuid];
							lineItem.TransactionLineItemUserDataGuid = this.creditLineItemUserDataGuids[nextGuid];

							nextGuid++;

							// Set the appropriate sub line item GUIDs. In this case the credit 
							// sub line item GUIDs.
							int nextSubGuid = 0;

							foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
							{
								subLineItem.TransactionSubLineItemGuid = this.creditSubLineItemGuids[nextSubGuid];
								subLineItem.ConjoinedTransactionSubLineItemGuid = this.creditSubLineItemConjoinedGuids[nextSubGuid];
								nextSubGuid++;
							}
						}

						break;
					}
			}

			int nextTransportGuid = 0;
			foreach (TransportLineItemDO transportLine in trans.TransportInfoList)
			{
				transportLine.TransactionTransportLineItemGuid = this.creditTransportLineItemGuids[nextTransportGuid];
				transportLine.ConjoinedTransactionTransportLineItemGuid = this.creditTransportLineItemConjoinedGuids[nextTransportGuid++];
				transportLine.TransactionGuid = this.creditTransGuid;
			}

			trans.ReversedTransID = this.reverseCreditTransId;

			this.ReverseQuantitySigns(trans);
		}

		/// <summary>
		/// This method will reverse the quantity sign.
		/// </summary>
		/// <param name="trans">
		/// The transaction.
		/// </param>
		private void ReverseQuantitySigns(TransactionDO trans)
		{
			if (trans.SubType == this.initialSubType)
			{
				return;
			}

			// Loop through each line item and ensure that the 
			// quantity signs are correct.
			foreach (LineItemDO lineItem in trans.LineItems)
			{
				lineItem.Quantity.GrossInventoryChange *= -1;
				lineItem.Quantity.NetInventoryChange *= -1;
				lineItem.Quantity.MassInventoryChange *= -1;
				lineItem.Quantity.PackageInventoryChange *= -1;

				// Loop through each subline item and ensure that the 
				// quantity signs are correct.
				foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
				{
					subLineItem.Quantity.GrossInventoryChange *= -1;
					subLineItem.Quantity.NetInventoryChange *= -1;
					subLineItem.Quantity.MassInventoryChange *= -1;
					subLineItem.Quantity.PackageInventoryChange *= -1;
				}
			}
		}

		/// <summary>
		/// The reverse financial signs.
		/// </summary>
		/// <param name="trans">
		/// The transaction.
		/// </param>
		private void ReverseFinancialSigns(TransactionDO trans)
		{
			var hardwareKey = new HardwareKeyClass();

			TransactionPreviousVersionInformation oldVersionOfTransaction = null;
			this.oldVersionsOfTransactions.TryGetValue(trans.TransactionGuid, out oldVersionOfTransaction);

			if (hardwareKey.IsADFKey() && oldVersionOfTransaction == null && // only apply to new reversals
				 (trans.ReversalType == TransactionDO.Reversal || trans.ReversalType == TransactionDO.ReversalWithUpdate))
			{
				foreach (LineItemDO lineItem in trans.LineItems)
				{
					// In ADF, financials are also reversed
					if (lineItem.Tax1 != null)
					{
						lineItem.Tax1 *= -1;
					}

					if (lineItem.Tax2 != null)
					{
						lineItem.Tax2 *= -1;
					}

					if (lineItem.Tax3 != null)
					{
						lineItem.Tax3 *= -1;
					}

					if (trans.Alias.Contains("Sale"))
					{
						if (lineItem.Number02 != null)
						{
							lineItem.Number02 *= -1;
						}

						if (lineItem.Number03 != null)
						{
							lineItem.Number03 *= -1;
						}

						if (lineItem.Number04 != null)
						{
							lineItem.Number04 *= -1;
						}

						if (lineItem.Number05 != null)
						{
							lineItem.Number05 *= -1;
						}

						if (lineItem.Number06 != null)
						{
							lineItem.Number06 *= -1;
						}

						if (lineItem.UserData != null && lineItem.UserData.ContainsKey(BaseTransactionLineItemDO.USER_DATA_LINE_ITEM_KEY_14))
						{
							string lineItemUserData14 = lineItem.UserData[BaseTransactionLineItemDO.USER_DATA_LINE_ITEM_KEY_14];

							double onCost;
							if (double.TryParse(lineItemUserData14, out onCost))
							{
								onCost *= -1;

								lineItem.UserData[BaseTransactionLineItemDO.USER_DATA_LINE_ITEM_KEY_14] = onCost.ToString(CultureInfo.InvariantCulture);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// This method will save the product TO / FROM values to be used when
		/// a conjoined re-graded transaction.
		/// </summary>
		/// <param name="regradeTrans">
		/// The re-grade Transaction.
		/// </param>
		private void PersistProducts(RegradeDO regradeTrans)
		{
			foreach (LineItemDO lineItem in regradeTrans.LineItems)
			{
				this.fromProduct.Add(lineItem.Product);
				this.fromProductCode.Add(lineItem.ProductCode);
				this.fromProductType.Add(lineItem.ProductType);
				this.fromProductGuid.Add(lineItem.ProductGuid);

				this.fromStorageId.Add(lineItem.StorageLocationID);
				this.fromStorageTankGuids.Add(lineItem.StorageLocationTankGuid);
			}

			foreach (var lineItemDO in regradeTrans.LineItems)
			{
				var lineItem = (RegradeLineItemDO)lineItemDO;
				this.toProduct.Add(lineItem.ToProduct);
				this.toProductCode.Add(lineItem.ToProductCode);
				this.toProductType.Add(lineItem.ToProductType);
				this.toProductGuid.Add(lineItem.ToProductGuid);

				this.toStorageId.Add(lineItem.ToStorageLocation);
				this.toStorageTankGuids.Add(lineItem.ToStorageLocationTankGuid);
			}
		}

		/// <summary>
		/// This method will save the storage location TO/FROM values used
		/// with a conjoined storage transfer transaction
		/// </summary>
		/// <param name="storageTransferTrans">
		/// The storage Transfer Transaction.
		/// </param>
		private void PersistStorageLocations(StorageTransferDO storageTransferTrans)
		{
			foreach (LineItemDO lineItem in storageTransferTrans.LineItems)
			{
				this.fromStorageId.Add(lineItem.StorageLocationID);
				this.fromStorageTankGuids.Add(lineItem.StorageLocationTankGuid);

				var transferLineItem = lineItem as StorageTransferLineItemDO;

				if (transferLineItem != null)
				{
					this.toStorageId.Add(transferLineItem.ToStorageLocation);
					this.toStorageTankGuids.Add(transferLineItem.ToStorageLocationTankGuid);
				}
			}
		}

		/// <summary>
		/// This method will set the trans ID and the conjoined trans ID to a proper value.
		/// If the transaction is a debit, then the trans ID is set to the trans ID, but
		/// if it is a credit, then it is reversed.
		/// </summary>
		/// <param name="trans">Transaction data object</param>
		private void SetTransAndConjoinedTransIDs(TransactionDO trans)
		{
			this.initialSubType = trans.SubType;

			// If the conjoined transaction ID of the transaction is null or empty, then this must
			// be a new transfer type transaction. Set the current transaction ID to the debit and
			// create a new conjoined transaction ID and save it for later use.
			if (string.IsNullOrEmpty(trans.ConjoinedTransID))
			{
				this.debitTransId = trans.TransID;
				this.debitConjoinedId = FuelsManagerId.NewId();

				// Set the credit id to the debit conjoined id 
				this.creditTransId = this.debitConjoinedId;

				// Set the credit conjoined id to the debit id 
				this.creditConjoinedId = this.debitTransId;

				this.reverseDebitTransId = string.Empty;
				this.reverseCreditTransId = string.Empty;
				this.debitTransGuid = Guid.Empty;
				this.creditTransGuid = Guid.Empty;
				this.creditTransactionNotesGuid = Guid.Empty;
				this.debitTransactionNotesGuid = Guid.Empty;
				this.creditTransactionUserDataGuid = Guid.Empty;
				this.debitTransactionUserDataGuid = Guid.Empty;
				this.creditTransactionSignatureGuid = Guid.Empty;
				this.debitTransactionSignatureGuid = Guid.Empty;

				// Initialize the line item transaction GUIDs and the conjoined
				// line item GUIDs.
				this.debitLineItemConjoinedGuids = new List<Guid>();
				this.debitLineItemGuids = new List<Guid>();
				this.debitLineItemUserDataGuids = new List<Guid>();
				this.creditLineItemGuids = new List<Guid>();
				this.creditLineItemConjoinedGuids = new List<Guid>();
				this.creditLineItemUserDataGuids = new List<Guid>();

				// Initialize the sub line item transaction GUIDs and the conjoined
				// line item GUIDs.
				this.debitSubLineItemGuids = new List<Guid>();
				this.debitSubLineItemConjoinedGuids = new List<Guid>();
				this.creditSubLineItemGuids = new List<Guid>();
				this.creditSubLineItemConjoinedGuids = new List<Guid>();
				this.debitTransportLineItemGuids = new List<Guid>();
				this.creditTransportLineItemGuids = new List<Guid>();
				this.debitTransportLineItemConjoinedGuids = new List<Guid>();
				this.creditTransportLineItemConjoinedGuids = new List<Guid>();

				foreach (LineItemDO lineItem in trans.LineItems)
				{
					this.debitLineItemConjoinedGuids.Add(Guid.Empty);
					this.debitLineItemGuids.Add(Guid.Empty);
					this.debitLineItemUserDataGuids.Add(Guid.Empty);
					this.creditLineItemGuids.Add(Guid.Empty);
					this.creditLineItemConjoinedGuids.Add(Guid.Empty);
					this.creditLineItemUserDataGuids.Add(Guid.Empty);

					// TODO: Figure out what we're trying to do here

					// ReSharper disable once UnusedVariable
					foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
					{
						this.debitSubLineItemGuids.Add(Guid.Empty);
						this.debitSubLineItemConjoinedGuids.Add(Guid.Empty);
						this.creditSubLineItemGuids.Add(Guid.Empty);
						this.creditSubLineItemConjoinedGuids.Add(Guid.Empty);
					}
				}

				foreach (TransportLineItemDO transportLine in trans.TransportInfoList)
				{
					this.debitTransportLineItemGuids.Add(Guid.Empty);
					this.creditTransportLineItemGuids.Add(Guid.Empty);
					this.debitTransportLineItemConjoinedGuids.Add(Guid.Empty);
					this.creditTransportLineItemConjoinedGuids.Add(Guid.Empty);
				}
			}
			else
			{
				this.debitLineItemConjoinedGuids = new List<Guid>();
				this.debitLineItemGuids = new List<Guid>();
				this.debitLineItemUserDataGuids = new List<Guid>();
				this.creditLineItemGuids = new List<Guid>();
				this.creditLineItemConjoinedGuids = new List<Guid>();
				this.creditLineItemUserDataGuids = new List<Guid>();
				this.debitSubLineItemGuids = new List<Guid>();
				this.debitSubLineItemConjoinedGuids = new List<Guid>();
				this.creditSubLineItemGuids = new List<Guid>();
				this.creditSubLineItemConjoinedGuids = new List<Guid>();
				this.debitTransportLineItemGuids = new List<Guid>();
				this.creditTransportLineItemGuids = new List<Guid>();
				this.debitTransportLineItemConjoinedGuids = new List<Guid>();
				this.creditTransportLineItemConjoinedGuids = new List<Guid>();

				if (trans.SubType.ToUpper().Equals(TransactionDO.DEBIT))
				{
					this.debitTransGuid = trans.TransactionGuid;
					this.debitTransId = trans.TransID;
					this.debitConjoinedId = trans.ConjoinedTransID;
					this.debitTransactionNotesGuid = trans.TransactionNoteGuid;
					this.debitTransactionUserDataGuid = trans.TransactionUserDataGuid;
					this.debitTransactionSignatureGuid = trans.TransactionSignatureGuid;

					this.creditTransGuid = trans.ConjoinedTransactionGuid;
					this.creditTransId = trans.ConjoinedTransID;
					this.creditConjoinedId = trans.TransID;
					this.creditTransactionNotesGuid = trans.ConjoinedNotesGuid;
					this.creditTransactionUserDataGuid = trans.ConjoinedUserDataGuid;
					this.creditTransactionSignatureGuid = trans.ConjoinedSignatureGuid;

					this.reverseDebitTransId = trans.ReversedTransID;
					this.reverseCreditTransId = trans.ConjoinReversedTransID;

					foreach (LineItemDO lineItem in trans.LineItems)
					{
						this.debitLineItemGuids.Add(lineItem.TransactionLineItemGuid);
						this.debitLineItemConjoinedGuids.Add(lineItem.ConjoinedTransactionLineItemGuid);
						this.debitLineItemUserDataGuids.Add(lineItem.TransactionLineItemUserDataGuid);

						this.creditLineItemGuids.Add(lineItem.ConjoinedTransactionLineItemGuid);
						this.creditLineItemConjoinedGuids.Add(lineItem.TransactionLineItemGuid);
						this.creditLineItemUserDataGuids.Add(lineItem.ConjoinedTransactionLineItemUserDataGuid);

						foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
						{
							this.debitSubLineItemGuids.Add(subLineItem.TransactionSubLineItemGuid);
							this.debitSubLineItemConjoinedGuids.Add(subLineItem.ConjoinedTransactionSubLineItemGuid);
							this.creditSubLineItemGuids.Add(subLineItem.ConjoinedTransactionSubLineItemGuid);
							this.creditSubLineItemConjoinedGuids.Add(subLineItem.TransactionSubLineItemGuid);
						}
					}

					foreach (TransportLineItemDO transportLine in trans.TransportInfoList)
					{
						this.debitTransportLineItemGuids.Add(transportLine.TransactionTransportLineItemGuid);
						this.debitTransportLineItemConjoinedGuids.Add(transportLine.ConjoinedTransactionTransportLineItemGuid);
						this.creditTransportLineItemGuids.Add(transportLine.ConjoinedTransactionTransportLineItemGuid);
						this.creditTransportLineItemConjoinedGuids.Add(transportLine.TransactionTransportLineItemGuid);
					}
				}
				else
				{
					this.creditTransGuid = trans.TransactionGuid;
					this.creditTransId = trans.TransID;
					this.creditConjoinedId = trans.ConjoinedTransID;
					this.creditTransactionNotesGuid = trans.TransactionNoteGuid;
					this.creditTransactionUserDataGuid = trans.TransactionUserDataGuid;
					this.creditTransactionSignatureGuid = trans.TransactionSignatureGuid;

					this.debitTransGuid = trans.ConjoinedTransactionGuid;
					this.debitTransId = trans.ConjoinedTransID;
					this.debitConjoinedId = trans.TransID;
					this.debitTransactionNotesGuid = trans.ConjoinedNotesGuid;
					this.debitTransactionUserDataGuid = trans.ConjoinedUserDataGuid;
					this.debitTransactionSignatureGuid = trans.ConjoinedSignatureGuid;

					this.reverseCreditTransId = trans.ReversedTransID;
					this.reverseDebitTransId = trans.ConjoinReversedTransID;

					foreach (LineItemDO lineItem in trans.LineItems)
					{
						this.creditLineItemGuids.Add(lineItem.TransactionLineItemGuid);
						this.creditLineItemConjoinedGuids.Add(lineItem.ConjoinedTransactionLineItemGuid);
						this.creditLineItemUserDataGuids.Add(lineItem.TransactionLineItemUserDataGuid);

						this.debitLineItemGuids.Add(lineItem.ConjoinedTransactionLineItemGuid);
						this.debitLineItemConjoinedGuids.Add(lineItem.TransactionLineItemGuid);
						this.debitLineItemUserDataGuids.Add(lineItem.ConjoinedTransactionLineItemUserDataGuid);

						foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
						{
							this.creditSubLineItemGuids.Add(subLineItem.TransactionSubLineItemGuid);
							this.creditSubLineItemConjoinedGuids.Add(subLineItem.ConjoinedTransactionSubLineItemGuid);
							this.debitSubLineItemGuids.Add(subLineItem.ConjoinedTransactionSubLineItemGuid);
							this.debitSubLineItemConjoinedGuids.Add(subLineItem.TransactionSubLineItemGuid);
						}
					}

					foreach (TransportLineItemDO transportLine in trans.TransportInfoList)
					{
						this.debitTransportLineItemGuids.Add(transportLine.ConjoinedTransactionTransportLineItemGuid);
						this.debitTransportLineItemConjoinedGuids.Add(transportLine.TransactionTransportLineItemGuid);
						this.creditTransportLineItemGuids.Add(transportLine.TransactionTransportLineItemGuid);
						this.creditTransportLineItemConjoinedGuids.Add(transportLine.ConjoinedTransactionTransportLineItemGuid);
					}
				}
			}
		}

		/// <summary>
		/// This method will return a transaction corresponding to either a receipt or shipment 
		/// transaction type based on the shipment number.
		/// </summary>
		/// <param name="origTransDO">
		/// The original Transaction data object.
		/// </param>
		/// <returns>
		/// Returns a transaction data object.
		/// </returns>
		private TransactionDO FindShipmentOrReceiptTransaction(TransactionDO origTransDO)
		{
			TransactionDO correspondingTransDO = null;

			var services = new GetTransactionProcessorClass();

			if ((origTransDO.TransTypeID == TransactionTypes.T8_Receipt) &&
				 (string.IsNullOrEmpty(origTransDO.ShipmentNumber) == false))
			{
				var getTransSr = new GetTransactionSR
				{
					Security = this.security,
					TransTypeID = TransactionTypes.T25_Shipment,
					ShipmentNumber = origTransDO.ShipmentNumber,
					TransStatus = TransactionStatus.Completed,
					Request = GetTransactionRequest.SITE_TYPEID_STATUS_REF_NUM
				};

				// Get the Transaction ID that is based on the site, transaction status,
				// transaction type ID, shipment number, and reference ID.
				// Associated processor = GetTransactionProcessor.cs
				GetTransactionDO getTransDO = services.Process(getTransSr);

				// Get the Transaction that matches the transaction ID.
				if (getTransDO != null)
				{
					correspondingTransDO = this.GetCorrespondingShipmentOrReceiptTransaction(getTransDO);
				}

				// Set the reference ID on the original transaction (the one that is being imported) with
				// the transaction ID of the corresponding shipment transaction's ID and vice versa. In addition,
				// set the status to completed on the shipment transaction.
				if (correspondingTransDO != null)
				{
					correspondingTransDO.Status = TransactionStatus.Completed;   // Shipment transaction
					origTransDO.TransRefID = correspondingTransDO.TransID;  // Receipt transaction
					correspondingTransDO.TransRefID = origTransDO.TransID;           // Shipment transaction
				}
			}

			if ((origTransDO.TransTypeID == TransactionTypes.T25_Shipment) &&
				 (string.IsNullOrEmpty(origTransDO.ShipmentNumber) == false))
			{
				var getTransSr = new GetTransactionSR
				{
					Security = this.security,
					TransTypeID = TransactionTypes.T8_Receipt,
					ShipmentNumber = origTransDO.ShipmentNumber,
					TransStatus = TransactionStatus.Completed,
					Request = GetTransactionRequest.SITE_TYPEID_STATUS_REF_NUM
				};

				// Get the Transaction ID that is based on the site, transaction status,
				// transaction type ID, shipment number, and reference ID.
				// Associated processor = GetTransactionProcessor.cs
				GetTransactionDO getTransDO = services.Process(getTransSr);

				// Get the Transaction that matches the transaction ID.
				if (getTransDO != null)
				{
					correspondingTransDO = this.GetCorrespondingShipmentOrReceiptTransaction(getTransDO);
				}

				// Set the reference ID on the original transaction (the one that is being imported) with
				// the transaction ID of the corresponding shipment transaction's ID and vice versa. In addition,
				// set the status to completed on the shipment transaction.
				if (correspondingTransDO != null)
				{
					origTransDO.Status = TransactionStatus.Completed;   // Shipment transaction
					origTransDO.TransRefID = correspondingTransDO.TransID;  // Shipment transaction
					correspondingTransDO.TransRefID = origTransDO.TransID;           // Receipt transaction
				}
			}

			return correspondingTransDO;
		}

		/// <summary>
		/// This method will return the entire corresponding receipt or shipment transaction based on the
		/// transaction ID.
		/// </summary>
		/// <param name="getTransDO">Get Transaction data object.</param>
		/// <returns>A shipment or receipt transaction.</returns>
		private TransactionDO GetCorrespondingShipmentOrReceiptTransaction(GetTransactionDO getTransDO)
		{
			TransactionDO correspondingTransDO = null;

			DataSet dataSet = getTransDO.TransactionDataSet;

			if ((dataSet != null) && (dataSet.Tables.Count > 0))
			{
				DataTable table = dataSet.Tables[0];

				if (table?.Rows != null && (table.Rows.Count > 0))
				{
					DataRow row = table.Rows[0];

					if (row != null)
					{
						string transId = row.IsNull("TransID") ? null : row["TransID"].ToString();

						if (string.IsNullOrEmpty(transId) == false)
						{
							var accountingSites = new AccountingSites();
							AccountingSite accountingSite = accountingSites.LoadSiteInfo(this.security, this.security.SiteGuid);

							var transSr = new TransactionSR
							{
								TransID = transId,
								Security = this.security,
								AccountingSite = accountingSite,
								ConvertUnits = false,
								AllowCrossSiteTransactions = true
							};

							var transProcessor = new TransactionProcessorClass();
							correspondingTransDO = transProcessor.Process(transSr);
						}
					}
				}
			}

			return correspondingTransDO;
		}

		/// <summary>
		/// The auto complete supply order.
		/// </summary>
		/// <param name="sr">
		/// The service request.
		/// </param>
		/// <param name="trans">
		/// The transaction.
		/// </param>
		/// <returns>
		/// The <see cref="TransactionDO"/>.
		/// </returns>
		private TransactionDO AutoCompleteSupplyOrder(SaveTransactionsSR sr, TransactionDO trans)
		{
			if (!sr.UseAutoComplete)
			{
				return null;
			}

			// Do not need to account for "Posted" status here
			if ((trans.TransTypeID != TransactionTypes.T8_Receipt
					  && trans.TransTypeID != TransactionTypes.T18_SupplyOrder)
				 || string.IsNullOrEmpty(trans.TransRefID)
				 || trans.Status != TransactionStatus.InProgress
				 || trans.ReversalType == TransactionDO.Reversal
				 || trans.ReversalType == TransactionDO.ReversalWithUpdate
				 || trans.ReversalType == TransactionDO.Update
				 || trans.ReversalType == TransactionDO.UpdateOriginal)
			{
				return null;
			}

			var transactionSr = new TransactionSR
			{
				Security = sr.Security,
				TransID = trans.TransRefID,
				AccountingSite = sr.AccountingSite,
				ConvertUnits = false
			};

			TransactionDO supplyOrder = this.transactionProcessor.Process(transactionSr);

			if (supplyOrder == null || !supplyOrder.AutoComplete)
			{
				return null;
			}

			bool changed = false;

			foreach (LineItemDO transLineItem in trans.LineItems)
			{
				// Do not need to test for TransactionStatus.Posted here
				if (transLineItem.Status == TransactionStatus.Completed)
				{
					foreach (LineItemDO orderLineItem in supplyOrder.LineItems)
					{
						if (transLineItem.OrderReferenceTransactionLineItemGuid == orderLineItem.TransactionLineItemGuid)
						{
							if ((orderLineItem.Status != TransactionStatus.Completed) &&
									  (orderLineItem.Status != TransactionStatus.Posted))
							{
								orderLineItem.Status = TransactionStatus.Completed;
								changed = true;
							}

							break;
						}
					}
				}
			}

			if (!changed)
			{
				return null;
			}

			bool allItemsComplete = true;

			foreach (LineItemDO orderLineItem in supplyOrder.LineItems)
			{
				if (orderLineItem.Status != TransactionStatus.Completed
					 && orderLineItem.Status != TransactionStatus.Cancelled
					 && orderLineItem.Status != TransactionStatus.Posted)
				{
					allItemsComplete = false;
					break;
				}
			}

			if (allItemsComplete &&
				 (supplyOrder.Status != TransactionStatus.Posted))
			{
				supplyOrder.Status = TransactionStatus.Completed;
			}

			return supplyOrder;
		}

		/// <summary>
		/// The send enterprise transaction.
		/// </summary>
		/// <param name="transaction">
		/// The transaction.
		/// </param>
		/// <param name="enterpriseAssemblies">A semicolon delimited list of enterprise assemblies to load and use to send the transaction</param>
		private void SendEnterpriseTransaction(TransactionDO transaction, string enterpriseAssemblies)
		{
			try
			{
				if (!string.IsNullOrEmpty(enterpriseAssemblies))
				{
					char[] separator = { ';' };
					string[] enterpriseIfList = enterpriseAssemblies.Split(separator, StringSplitOptions.RemoveEmptyEntries);

					foreach (string assemblyName in enterpriseIfList)
					{
						try
						{
							Assembly dll = null;

							if (!AssemblyDictionary.ContainsKey(assemblyName.ToLower()))
							{
								try
								{
									dll = Assembly.LoadFrom(assemblyName.ToString());
								}
								catch
								{
									try
									{
										dll = Assembly.Load(assemblyName);
									}
									catch (Exception ex)
									{
										string message = "Assembly Load Error in Send Enterprise Transaction. " + ex.Message;
										FMEventLog eventLog = new FMEventLog();
										eventLog.WriteEntry(message, FMEventLogEntryType.Warning);
									}
								}

								if (dll != null)
									AssemblyDictionary.Add(assemblyName.ToLower(), dll);
							}
							else
							{
								dll = AssemblyDictionary.Get(assemblyName.ToLower());
							}

							try
							{
								Type[] types = dll.GetTypes();

								// sort these because some code expects it to run in alphabetical order
								Array.Sort(types, (type1, type2) => string.Compare(type1.Name, type2.Name, StringComparison.Ordinal));

								foreach (Type module in types)
								{
									Type iEnterprise = module.GetInterface("FMBusinessObjects.Interfaces.IEnterprise");

									if (iEnterprise != null)
									{
										object engine = Activator.CreateInstance(module);
										var enterprise = (IEnterprise)engine;

										enterprise.Send(this.security, transaction);
									}
								}
							}
							catch { }
						}
						catch (Exception e)
						{
							var eventLog = new FMEventLog();
							eventLog.WriteEntry("AccountingBLL - " + e.Message, FMEventLogEntryType.Error);
						}
					}
				}
			}
			catch (Exception e)
			{
				var eventLog = new FMEventLog();
				eventLog.WriteEntry("AccountingBLL - " + e.Message, FMEventLogEntryType.Error);
			}
		}

		/// <summary>
		/// This method will create an event for the FuelsManager event log indicating
		/// that a new transaction has been created or one that has been updated.
		/// </summary>
		/// <param name="transaction">The transaction record</param>
		private void CreateAlarmAndEventLogRecord(TransactionDO transaction, SaveTransactionsSR sr)
		{
			TransactionPreviousVersionInformation oldVersionOfTransaction;
			this.oldVersionsOfTransactions.TryGetValue(transaction.TransactionGuid, out oldVersionOfTransaction);

			bool hasStatusChanged = oldVersionOfTransaction != null && oldVersionOfTransaction.Status != transaction.Status;
			bool logUpdateEvent = false;
			bool isRealUser = this.security.UserGuid != Guid.Empty && this.security.UserID != DBAccess.ServiceLoginAccess;

			// Only write update events if the delete flag changed or a real user is making the update 
			// (and not the service login)
			// No need to log an update event if the status changes, because that's a separate event.
			if (oldVersionOfTransaction != null &&
				 (oldVersionOfTransaction.DeleteFlag != transaction.DeleteFlag) || sr.BOLFromLoadRackFlag == false)
			{
				logUpdateEvent = true;
			}

			var transAlarmEventDO = new TransactionAlarmEventDO
			{
				AliasName = transaction.Alias,
				TransTypeID = transaction.TransTypeID,
				InventoryDate = transaction.InventoryDate,
				DocumentNumber = transaction.DocumentNumber
			};

			// If the old transaction is null, this must be a new transaction, so log an event
			if (oldVersionOfTransaction == null)
			{
				switch (transAlarmEventDO.TransTypeID)
				{
					case TransactionTypes.T1_PrimaryAdjustment:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT1CreateEvent);
						break;
					case TransactionTypes.T2_SecondaryAdjustment:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT2CreateEvent);
						break;
					case TransactionTypes.T3_PrimaryDefuel:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT3CreateEvent);
						break;
					case TransactionTypes.T4_SecondaryDefuel:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT4CreateEvent);
						break;
					case TransactionTypes.T5_PrimaryDisbursement:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT5CreateEvent);
						break;
					case TransactionTypes.T6_SecondaryDisbursement:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT6CreateEvent);
						break;
					case TransactionTypes.T7_FillStand:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT7CreateEvent);
						break;
					case TransactionTypes.T8_Receipt:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT8CreateEvent);
						break;
					case TransactionTypes.T9_Request:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT9CreateEvent);
						break;
					case TransactionTypes.T10_Unload:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT10CreateEvent);
						break;
					case TransactionTypes.T11_ConsumerTransfer:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT11CreateEvent);
						break;
					case TransactionTypes.T12_InventoryNotAffected:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT12CreateEvent);
						break;
					case TransactionTypes.T13_OwnerTransfer:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT13CreateEvent);
						break;
					case TransactionTypes.T14_PhysicalInventory:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT14CreateEvent);
						break;
					case TransactionTypes.T15_PrimaryRegrade:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT15CreateEvent);
						break;
					case TransactionTypes.T16_SecondaryRegrade:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT16CreateEvent);
						break;
					case TransactionTypes.T17_Order:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT17CreateEvent);
						break;
					case TransactionTypes.T18_SupplyOrder:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT18CreateEvent);
						break;
					case TransactionTypes.T19_EndOfDay:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT19CreateEvent);
						break;
					case TransactionTypes.T20_EndOfMonth:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT20CreateEvent);
						break;
					case TransactionTypes.T21_AccountPayableInvoice:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT21CreateEvent);
						break;
					case TransactionTypes.T22_AccountReceivableInvoice:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT22CreateEvent);
						break;
					case TransactionTypes.T23_StorageTransfer:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT23CreateEvent);
						break;

					// The type 24 transaction, aggregate, does not log anything. 
					case TransactionTypes.T25_Shipment:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT25CreateEvent);
						break;
				}
			}
			else if (logUpdateEvent)
			{
				switch (transAlarmEventDO.TransTypeID)
				{
					case TransactionTypes.T1_PrimaryAdjustment:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT1UpdateEvent);
						break;
					case TransactionTypes.T2_SecondaryAdjustment:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT2UpdateEvent);
						break;
					case TransactionTypes.T3_PrimaryDefuel:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT3UpdateEvent);
						break;
					case TransactionTypes.T4_SecondaryDefuel:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT4UpdateEvent);
						break;
					case TransactionTypes.T5_PrimaryDisbursement:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT5UpdateEvent);
						break;
					case TransactionTypes.T6_SecondaryDisbursement:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT6UpdateEvent);
						break;
					case TransactionTypes.T7_FillStand:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT7UpdateEvent);
						break;
					case TransactionTypes.T8_Receipt:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT8UpdateEvent);
						break;
					case TransactionTypes.T9_Request:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT9UpdateEvent);
						break;
					case TransactionTypes.T10_Unload:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT10UpdateEvent);
						break;
					case TransactionTypes.T11_ConsumerTransfer:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT11UpdateEvent);
						break;
					case TransactionTypes.T12_InventoryNotAffected:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT12UpdateEvent);
						break;
					case TransactionTypes.T13_OwnerTransfer:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT13UpdateEvent);
						break;
					case TransactionTypes.T14_PhysicalInventory:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT14UpdateEvent);
						break;
					case TransactionTypes.T15_PrimaryRegrade:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT15UpdateEvent);
						break;
					case TransactionTypes.T16_SecondaryRegrade:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT16UpdateEvent);
						break;
					case TransactionTypes.T17_Order:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT17UpdateEvent);
						break;
					case TransactionTypes.T18_SupplyOrder:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT18UpdateEvent);
						break;
					case TransactionTypes.T19_EndOfDay:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT19UpdateEvent);
						break;
					case TransactionTypes.T20_EndOfMonth:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT20UpdateEvent);
						break;
					case TransactionTypes.T21_AccountPayableInvoice:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT21UpdateEvent);
						break;
					case TransactionTypes.T22_AccountReceivableInvoice:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT22UpdateEvent);
						break;
					case TransactionTypes.T23_StorageTransfer:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT23UpdateEvent);
						break;

					// The type 24 transaction, aggregate, does not log anything. 
					case TransactionTypes.T25_Shipment:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT25UpdateEvent);
						break;
				}
			}

			if (hasStatusChanged)
			{
				switch (transAlarmEventDO.TransTypeID)
				{
					case TransactionTypes.T1_PrimaryAdjustment:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT01StatusChangedEvent);
						break;
					case TransactionTypes.T2_SecondaryAdjustment:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT02StatusChangedEvent);
						break;
					case TransactionTypes.T3_PrimaryDefuel:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT03StatusChangedEvent);
						break;
					case TransactionTypes.T4_SecondaryDefuel:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT04StatusChangedEvent);
						break;
					case TransactionTypes.T5_PrimaryDisbursement:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT05StatusChangedEvent);
						break;
					case TransactionTypes.T6_SecondaryDisbursement:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT06StatusChangedEvent);
						break;
					case TransactionTypes.T7_FillStand:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT07StatusChangedEvent);
						break;
					case TransactionTypes.T8_Receipt:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT08StatusChangedEvent);
						break;
					case TransactionTypes.T9_Request:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT09StatusChangedEvent);
						break;
					case TransactionTypes.T10_Unload:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT10StatusChangedEvent);
						break;
					case TransactionTypes.T11_ConsumerTransfer:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT11StatusChangedEvent);
						break;
					case TransactionTypes.T12_InventoryNotAffected:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT12StatusChangedEvent);
						break;
					case TransactionTypes.T13_OwnerTransfer:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT13StatusChangedEvent);
						break;
					case TransactionTypes.T14_PhysicalInventory:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT14StatusChangedEvent);
						break;
					case TransactionTypes.T15_PrimaryRegrade:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT15StatusChangedEvent);
						break;
					case TransactionTypes.T16_SecondaryRegrade:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT16StatusChangedEvent);
						break;
					case TransactionTypes.T17_Order:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT17StatusChangedEvent);
						break;
					case TransactionTypes.T18_SupplyOrder:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT18StatusChangedEvent);
						break;
					case TransactionTypes.T19_EndOfDay:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT19StatusChangedEvent);
						break;
					case TransactionTypes.T20_EndOfMonth:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT20StatusChangedEvent);
						break;
					case TransactionTypes.T21_AccountPayableInvoice:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT21StatusChangedEvent);
						break;
					case TransactionTypes.T22_AccountReceivableInvoice:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT22StatusChangedEvent);
						break;
					case TransactionTypes.T23_StorageTransfer:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT23StatusChangedEvent);
						break;

					// The type 24 transaction, aggregate, does not log anything. 
					case TransactionTypes.T25_Shipment:
						this.alarmAndEventLogs.Add(transAlarmEventDO.TransactionT25StatusChangedEvent);
						break;
				}
			}
		}

		/// <summary>
		/// Saves the transmitted transactions.
		/// </summary>
		/// <param name="serviceRequestDataObject">The service request data object.</param>
		/// <param name="securityObject">The security.</param>
		/// <returns>A transmit transaction list result object.</returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public SaveTransmitTranListResultDO SaveTransmittedTransactions(TransmitTranListDO serviceRequestDataObject, SecurityClass securityObject)
		{
			if (securityObject == null)
			{
				throw new ArgumentNullException(nameof(securityObject));
			}

			this.security = securityObject;

			// Load a transaction object
			var trans = new TransactionDO();

			// Initialize a save transactions service request
			var sr = new SaveTransactionsSR
			{
				Security = this.security,
				SubType = SaveTransactionsSR.SaveTransactionSubType.SaveTransactions,
				IndividualDbTransaction = true,
				UseAutoComplete = true
			};

			sr.Transactions.Add(trans);

			// Save the transaction
			this.SaveTransactions(sr);

			// Return results
			throw new NotImplementedException();
		}

		/// <summary>
		/// Save alarm and event log records created during save transactions processing.
		/// </summary>
		/// <param name="alarmAndEventLogsToSave">Alarm and event log records to save to the DB</param>
		private void SaveAlarmAndEventLogRecords(List<AlarmAndEventLogClass> alarmAndEventLogsToSave)
		{
			var alarmAndEventLogsServiceClass = new AlarmAndEventLogsClass();
			alarmAndEventLogsServiceClass.AddList(this.security, alarmAndEventLogsToSave);
		}

		/// <summary>
		/// Save the records (header, line item, etc) in the transactions provided
		/// </summary>
		/// <param name="transactions"></param>
		/// <param name="forceNewPidx"></param>
		private void SaveTransactions(List<TransactionDO> transactions, SaveTransactionsSR sr)
		{
			var lineItemsWithTransactionInformation = new List<LineItemWithTransactionInformation>();
			var subLineItemsWithTransactionInformation = new List<SubLineItemWithTransactionInformation>();
			var weightReadingsWithTransactionGuids = new List<WeightReadingWithTransactionInformation>();
			var transactionGuidsThatPreviouslyHadWeightReadings = new List<Guid>();
			var existingTransactions = new List<TransactionDO>();
			var transactionPidxsWithTransactionInformation = new List<TransactionPIDXWithTransactionInformation>();
			var transportLineItemsWithTransactionInformation = new List<TransportLineItemWithTransactionInformation>();
			var transactionLinksWithTransactionAndLineItemInformation = new List<TransactionLinkWithTransactionAndLineItemInformation>();

			// Save the Transaction Header, Notes, Signature, and User Data information
			var headerDbi = new TransactionHeaderDBI(this.security.UserID);
			headerDbi.Save(this.security, transactions);

			foreach (TransactionDO transaction in transactions)
			{
				int lineItemSequenceId = 0;

				foreach (LineItemDO lineItem in transaction.LineItems)
				{
					// We always resequence the line item sequence numbers when saving the records
					// so that the first line item has a sequence = 0 and so on.
					lineItem.SequenceId = lineItemSequenceId++;

					// Any line item without a guid must be new, so we generate a new guid for it
					if (lineItem.TransactionLineItemGuid == Guid.Empty)
					{
						lineItem.TransactionLineItemGuid = Guid.NewGuid();
					}

					// Create a record associating the line item with the transaction header information needed to save 
					// the line item and add it to the list of line items to save
					var withTransactionInformation = new LineItemWithTransactionInformation
					{
						LineItem = lineItem,
						TransactionGuid = transaction.TransactionGuid,
						InventoryDate = transaction.InventoryDate,
						TransVersion = transaction.TransVersion,
						DeleteFlag = transaction.DeleteFlag
					};

					lineItemsWithTransactionInformation.Add(withTransactionInformation);

					// If the transaction is associated (linked) with other transactions, create records containing the association
					// as well as the transaction and line item information related to the association
					if (lineItem.AssociatedTransactions != null)
					{
						foreach (AssociatedTxDO transactionLink in lineItem.AssociatedTransactions)
						{
							var transactionLinkWithTransactionAndLineItemInformation = new TransactionLinkWithTransactionAndLineItemInformation
							{
								TransactionGuid = transaction.TransactionGuid,
								TransID = transaction.TransID,
								TransactionLineItemGuid = lineItem.TransactionLineItemGuid,
								TransactionLink = transactionLink
							};

							transactionLinksWithTransactionAndLineItemInformation.Add(transactionLinkWithTransactionAndLineItemInformation);
						}
					}

					if (lineItem.SubLineItems != null)
					{
						// We always resequence the sub line item sequence numbers when saving the records
						// so that the first sub line item has a sequence = 0 and so on.
						int subLineItemSequenceId = 0;

						foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
						{
							subLineItem.SequenceId = subLineItemSequenceId++;

							// Create a record associating the sub line item with the transaction header and line item information needed to save 
							// the sub line item and add it to the list of sub line items to save
							var subLineItemWithTransactionInformation = new SubLineItemWithTransactionInformation
							{
								SubLineItem = subLineItem,
								TransactionGuid = transaction.TransactionGuid,
								TransactionLineItemGuid = lineItem.TransactionLineItemGuid,
								InventoryDate = transaction.InventoryDate,
								TransVersion = transaction.TransVersion
							};

							subLineItemsWithTransactionInformation.Add(subLineItemWithTransactionInformation);
						}
					}
				}

				// Take any PIDX records associated with the transaction, create a record associating them with the transaction guid,
				// and add them to the list of PIDX records to save
				if (transaction.TransPIDXCollection != null)
				{
					foreach (TransactionPIDXDO pidx in transaction.TransPIDXCollection)
					{
						var transactionPIDXWithTransactionInformation = new TransactionPIDXWithTransactionInformation
						{
							TransactionPIDX = pidx,
							TransactionGuid = transaction.TransactionGuid
						};

						transactionPidxsWithTransactionInformation.Add(transactionPIDXWithTransactionInformation);
					}
				}

				// Take any transport line item records associated with the transaction, create a record associating them with transaction information,
				// and add them to the list of transport line item records to save
				if (transaction.TransportInfoList != null)
				{
					foreach (TransportLineItemDO transportLineItem in transaction.TransportInfoList)
					{

						if (transportLineItem.TransactionGuid == Guid.Empty)
						{
							transportLineItem.TransactionGuid = transaction.TransactionGuid;
						}
						if (transportLineItem.TransactionTransportLineItemGuid == Guid.Empty)
						{
							transportLineItem.TransactionTransportLineItemGuid = Guid.NewGuid();
						}


						var transportLineItemWithTransactionInformation = new TransportLineItemWithTransactionInformation
						{
							TransportLineItem = transportLineItem,
							TransVersion = transaction.TransVersion,
							TransactionGuid = transaction.TransactionGuid
						};

						transportLineItemsWithTransactionInformation.Add(transportLineItemWithTransactionInformation);
					}
				}

				// Get the previous (existing) version of this transaction. We use some of this data to facilitate processing.
				TransactionPreviousVersionInformation previousVersionInformation;
				this.oldVersionsOfTransactions.TryGetValue(transaction.TransactionGuid, out previousVersionInformation);

				// Any transaction that previously had weight readings needs to mark the existing weight readings as historical.
				// Create a list of transaction guids for transactions that previously had weight readings.
				if (previousVersionInformation != null && previousVersionInformation.HasWeightReadings)
				{
					transactionGuidsThatPreviouslyHadWeightReadings.Add(transaction.TransactionGuid);
				}

				// We need to perform the "Delete remaining" task for existing transactions which deletes old line items, sub line items, etc by the transVersion.
				// If the transaction is not new, add it to the list of transactions we'll perform the delete for
				if (previousVersionInformation != null)
				{
					existingTransactions.Add(transaction);
				}

				// Add all the weight readings that aren't historical to a list of weight readings to save along with the transaction information required to save them
				if (transaction.WeightReadings != null)
				{
					foreach (WeightReadingDO weightReading in transaction.WeightReadings.Where(reading => reading.HistoricalFlag == false).ToList())
					{
						var weightReadingWithTransactionInformation = new WeightReadingWithTransactionInformation
						{
							WeightReading = weightReading,
							TransactionGuid = transaction.TransactionGuid,
							TransVersion = transaction.TransVersion
						};

						weightReadingsWithTransactionGuids.Add(weightReadingWithTransactionInformation);
					}
				}

				// Create a record for the alarm and event log
				this.CreateAlarmAndEventLogRecord(transaction, sr);

				// TODO: Temporary commented out so that QA does not test change queue features.
				// Write to the tblChangesQueue table.
				// The transaction is new if the oldTransDO is null, otherwise, it's being modified.
				//if (this.oldTransDO == null)
				//{
				//	ChangeQueueRecordsClass.ProcessChangeTxQueueRecords(security, ChangeQueueEventType.Add, transaction.TransactionGuid, transaction.TransID, transaction.SiteGuid);
				//}
				//else
				//{
				//	ChangeQueueRecordsClass.ProcessChangeTxQueueRecords(security, ChangeQueueEventType.Modify, transaction.TransactionGuid, transaction.TransID, transaction.SiteGuid);
				//}
			}

			// Save the line items
			if (lineItemsWithTransactionInformation.Count > 0)
			{
				var lineItemDbi = new TransactionLineItemDBI(this.security.UserID);
				lineItemDbi.Save(this.security, lineItemsWithTransactionInformation);
			}

			// Insert or update the sub line items
			if (subLineItemsWithTransactionInformation.Count > 0)
			{
				var subLineItemDbi = new TransactionSubLineItemDBI(this.security.UserID);
				subLineItemDbi.Save(this.security, subLineItemsWithTransactionInformation);
			}

			// Save the Transaction PIDX Records
			var transPidxDbi = new TransactionPIDXDBI(this.security.UserID);
			transPidxDbi.Save(this.security, transactionPidxsWithTransactionInformation, sr.ForceNewPidxRecord);

			// Save the Transport information
			if (transportLineItemsWithTransactionInformation.Count > 0)
			{
				var transportInfoDbi = new TransactionTransportInfoDBI(this.security.UserID);
				transportInfoDbi.Save(this.security, transportLineItemsWithTransactionInformation);
			}

			var hardwareKey = new HardwareKeyClass();
			bool isAdfKey = hardwareKey.IsADFKey();

			var associatedTxDbi = new LineItemAssociatedTxDBI(this.security.UserID);

			if (!isAdfKey)
			{
				// Determine which (if any) of the provided line items have an associated parent transaction (matching tblTransactionLinks.LinkedTransactionLineItemGuid)
				// We only need to call the hierarchy util for these records
				Dictionary<Guid, bool> lineItemsWithParentAssociations = associatedTxDbi.GetLineItemsWithParentAssociations(this.security, lineItemsWithTransactionInformation);

				// Call the hierarchy util for each line item that has an associated parent 
				foreach (KeyValuePair<Guid, bool> lineItemWithParentAssociations in lineItemsWithParentAssociations)
				{
					// The hierarchy util will updated any parent line item's gross quantity
					// that may have been affected by saving this line item.
					this.hierarchyUtil.UpdateAggregatedParents(lineItemWithParentAssociations.Key, lineItemWithParentAssociations.Value);
				}
			}

			// Save Transaction Associations (aka Links)           
			associatedTxDbi.Save(transactionLinksWithTransactionAndLineItemInformation, this.oldVersionsOfTransactions, transactions, this.security);

			// Delete any child records associated with the transaction.
			// The deletion relies on the TransVersion to detect old records.
			// We only have to do this when the transaction is being updated (i.e. the old transaction is not null)
			if (existingTransactions.Count > 0)
			{
				var transactionHeaderDbi = new TransactionHeaderDBI(this.security.UserID);
				transactionHeaderDbi.DeleteRemaining(this.security, existingTransactions);
			}

			// All previous weight reading records become historical
			// We only have to do this if there were weight readings on the old transaction record.
			if (transactionGuidsThatPreviouslyHadWeightReadings.Count > 0)
			{
				var historicalWeightReadingDbi = new WeightReadingDBI(this.security.UserID);
				historicalWeightReadingDbi.MarkExistingRecordsAsHistorical(this.security, transactionGuidsThatPreviouslyHadWeightReadings);
			}

			// Save any new weight readings
			if (weightReadingsWithTransactionGuids.Count > 0)
			{
				var weightReadingDbi = new WeightReadingDBI(this.security.UserID);
				weightReadingDbi.Save(this.security, weightReadingsWithTransactionGuids);
			}
		}

		/// <summary>
		/// Make a copy of the transaction by serializing and then deserializing it.
		/// This is needed to support the conjoined transaction functionality,
		/// where we turn one transaction into two.
		/// </summary>
		/// <param name="transactionToCopy">The transaction to make a copy of</param>
		/// <returns>A copy of the provided transaction</returns>
		private static TransactionDO CopyTransaction(TransactionDO transactionToCopy)
		{
			if (transactionToCopy == null)
			{
				return null;
			}

			BinaryFormatter binaryFormatter = new BinaryFormatter();

			using (MemoryStream stream = new MemoryStream())
			{
				binaryFormatter.Serialize(stream, transactionToCopy);
				stream.Seek(0, SeekOrigin.Begin);

				TransactionDO deserializedTransaction = (TransactionDO)binaryFormatter.Deserialize(stream);

				return deserializedTransaction;
			}
		}
	}
}
