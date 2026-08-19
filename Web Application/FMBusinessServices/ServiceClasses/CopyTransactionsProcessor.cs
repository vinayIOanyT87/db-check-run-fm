// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CopyTransactionsProcessor.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   This service request processor copies (relogs) the specified
//   list of transactions (by TransID) for Dispatch.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

	/// <summary>
	/// This service request processor copies (relogs) the specified
	/// list of transactions (by TransID) for Dispatch.
	/// </summary>
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class CopyTransactionsProcessor : ICopyTransactionsProcessor
	{
		/// <summary>
		/// The service request
		/// </summary>
		private CopyTransactionsSR serviceRequest;

		/// <summary>
		/// The site class that the transactions is associated.
		/// </summary>
		private SiteClass site;

		/// <summary>
		/// Processes the specified copy transactions SR.
		/// </summary>
		/// <param name="copyTransactionsSR">The copy transactions SR.</param>
		/// <returns>A SaveTransactionsResultDO object describing the results of the save.</returns>
		[OperationBehavior( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public SaveTransactionsResultDO Process( CopyTransactionsSR copyTransactionsSR )
		{
			this.serviceRequest = copyTransactionsSR;

			// Is there anything to do?
			if ( this.serviceRequest.TransactionIds.Count == 0 )
			{
				return null;
			}

			var saveSR = new SaveTransactionsSR();

			var sites = new SitesClass();
			this.site = sites.Get(this.serviceRequest.Security, this.serviceRequest.Security.SiteGuid, false);
			
			// Get a count of transactions
			var count = this.serviceRequest.TransactionIds.Count;

			// Get a set of document numbers for our transactions
			var documentNumbers = sites.GetNextDocumentNumbers( this.serviceRequest.Security, this.serviceRequest.DocumentTypes, this.serviceRequest.Security.SiteGuid, count );

			// For each transaction ID we received, load the transaction and save a copy
			for ( int index = 0; index < count; ++index )
			{
				var transID = this.serviceRequest.TransactionIds[index];

				// Load the transaction
				var transaction = this.GetTransaction( transID );

				// Make the transaction a copy by resetting fields/etc.
				this.PrepareTransactionCopy( transaction, documentNumbers[index] );

				// Save the transaction to the SR in preparation for saving
				// all at one time at the end.
				saveSR.Transactions.Add( transaction );
			}

			// Save all the transactions 
			return this.SaveTransactions( saveSR );
		}

		/// <summary>
		/// Gets the transaction.
		/// </summary>
		/// <param name="transID">The trans ID.</param>
		/// <returns>The requested transaction object</returns>
		/// <exception cref="System.ArgumentNullException">transID</exception>
		/// <exception cref="System.ArgumentException">Transaction not found</exception>
		private TransactionDO GetTransaction( string transID )
		{
			if ( string.IsNullOrEmpty( transID ) )
			{
				throw new ArgumentNullException( "transID" );
			}

			var sr = new TransactionSR
				         {
					         Security = this.serviceRequest.Security, 
							 TransID = transID, 
							 ConvertUnits = false
				         };

			var processor = new TransactionProcessorClass();
			var transaction = processor.Process( sr );

			if ( transaction == null )
			{
				throw new ArgumentException( "Transaction not found." );
			}

			return transaction;
		}

		/// <summary>
		/// Prepares the transaction as a copy.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="documentNumber">The document number.</param>
		/// <exception cref="System.ApplicationException">Unhandled transaction type passed to dispatch.</exception>
		private void PrepareTransactionCopy( TransactionDO transaction, string documentNumber )
		{
			transaction.TransID					= FuelsManagerId.NewId();
			transaction.TransactionGuid			= Guid.Empty;
			transaction.TransactionUserDataGuid = Guid.Empty;
			transaction.TransactionNoteGuid		= Guid.Empty;
			
			transaction.DocumentNumber = documentNumber;

			transaction.Status = TransactionStatus.Requested;
			transaction.TimeIn = null;
			transaction.TimeOut = null;
			transaction.TimeEnd = null;
			transaction.RouteSchedule.FST = null;
			transaction.RequestedDateTime = null;
			transaction.SubmittedToAccounting = false;
			transaction.Flag05 = false;
			transaction.Flag06 = false;

			transaction.OperatorPersonnelGuid = Guid.Empty;
			transaction.OperatorID = string.Empty;
			transaction.OperatorName = string.Empty;

			foreach ( LineItemDO item in transaction.LineItems )
			{
				item.Quantity = new QuantityDO();
				item.Status = TransactionStatus.Requested;
				item.TransactionLineItemGuid = Guid.Empty;
				item.TransactionLineItemUserDataGuid = Guid.Empty;

				if (item.SubLineItems != null)
				{
					item.SubLineItems.ForEach(
						subLineItem =>
							{
								subLineItem.TransactionSubLineItemGuid = Guid.Empty;
							});
				}
			}

			transaction.TransactionDateTime = TimeConverter.Now(this.site);
			transaction.RequestedDateTime = transaction.TransactionDateTime;

			// Now do the "un-dispatching" preparations
			if ( transaction.OperatorPersonnelGuid != Guid.Empty )
			{
				var personnel = new PersonnelClass();
				PersonClass personClass = personnel.Get( this.serviceRequest.Security, transaction.OperatorPersonnelGuid );
				if ( personClass.IdentityGuid != Guid.Empty )
				{
					personClass.Status = PersonClass.STATUS.In;
				}
			}

			LineItemDO lineItem = transaction.LineItems[0];

			transaction.Status = TransactionStatus.Requested;
			lineItem.Status = TransactionStatus.Requested;

			transaction.OperatorPersonnelGuid = Guid.Empty;
			transaction.OperatorID = string.Empty;
			transaction.OperatorName = string.Empty;
			transaction.DispatchedDateTime = null;
			transaction.UserData[TransactionDO.USER_DATA_KEY_13] = string.Empty;
			transaction.UserData[TransactionDO.USER_DATA_KEY_14] = string.Empty;

			switch ( transaction.TransTypeID )
			{
				case TransactionTypes.T3_PrimaryDefuel:
				case TransactionTypes.T4_SecondaryDefuel:
				case TransactionTypes.T7_FillStand:
					transaction.DestinationEQ1 = new EquipmentDO();
					lineItem.DestinationEQ = new EquipmentDO();
					break;

				case TransactionTypes.T5_PrimaryDisbursement:
				case TransactionTypes.T6_SecondaryDisbursement:
				case TransactionTypes.T10_Unload:
				case TransactionTypes.T12_InventoryNotAffected:
					transaction.SourceEQ1 = new EquipmentDO();
					lineItem.SourceEQ = new EquipmentDO();
					break;

				default:
					throw new ApplicationException( "Unhandled transaction type passed to dispatch." );
			}
		}

		/// <summary>
		/// Saves the transactions.
		/// </summary>
		/// <param name="saveTransactionsSR">The save transactions SR.</param>
		/// <returns>A SaveTransactionsResultDO object describing the results of the save.</returns>
		private SaveTransactionsResultDO SaveTransactions( SaveTransactionsSR saveTransactionsSR )
		{
			// Finish initialization of the service request and call the 
			// Save Transactions processor.
			saveTransactionsSR.SubType = SaveTransactionsSR.SaveTransactionSubType.SaveTransactions;
			saveTransactionsSR.IndividualDbTransaction = false;
			saveTransactionsSR.Security = this.serviceRequest.Security;
			saveTransactionsSR.CurrentSiteGuid = this.serviceRequest.Security.SiteGuid;
			saveTransactionsSR.ConvertUnits = false;

			// Call the save processor and return the results.
			var processor = new SaveTransactionsProcessor();
			return processor.SaveTransactions( saveTransactionsSR );
		}
	}
}
