/// <summary>
/// File name:	TransactionFactory.cs
/// Purpose:	
///	Comments:	Copyright (C) Varec, Inc. Norcross, GA, USA, 
///				2007.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Varec.
///	Author(s):	
///	Version:	7.1.0  Current version
///	
///	Modification History:
///		Date:		By:				Reason:
///		----------	-------------	-------------------------------------------
///		25-Jan-07	I.Orndorff		1.0.0.1 - Modified "CreateTransaction()" to 
///											  use OwnerTransferDO (OwnerTransfer
///											  is derived from TransactionDO) instead 
///											  of TransactionDO to create imported 
///											  transactions. This fixes CSI #3782.
///		
/// </summary>
/// 

namespace XMLImport
{
	using System.Xml.XPath;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	/// <summary>
	/// Summary description for TransactionFactory.
	/// </summary>
	public class TransactionFactory
	{
		public TransactionFactory(XMLImportProcessor importProcessor)
		{
			this.importProcessor = importProcessor;

			primaryAdjustmentPopulater = new PrimaryAdjustmentPopulater();
			secondaryAdjustmentPopulater = new SecondaryAdjustmentPopulater();
			primaryDefuelPopulater = new PrimaryDefuelPopulater();
			secondaryDefuelPopulater = new SecondaryDefuelPopulater();
			primaryDisbursementPopulater = new PrimaryDisbursementPopulater();
			secondaryDisbursementPopulater = new SecondaryDisbursementPopulater();
			fillStandPopulater = new FillStandPopulater();
			receiptPopulater = new ReceiptPopulater();
			requestPopulater = new RequestPopulater();
			unloadPopulater = new UnloadPopulater();
			consumerTransferPopulater = new ConsumerTransferPopulater();
			type12Populater	= new Type12Populater();
			ownerTransferPopulater = new OwnerTransferPopulater();
			physicalInventoryPopulater = new PhysicalInventoryPopulater();
			primaryRegradePopulater = new PrimaryRegradePopulater();
			secondaryRegradePopulater = new SecondaryRegradePopulater();
		}

		public TransactionDO CreateTransaction(SecurityClass security, XPathDocument doc, out TransactionValidationResult transactionValidationResult)
		{
			OwnerTransferDO transaction = null;
			TransactionPopulater transactionPopulater = null;

			transaction = new OwnerTransferDO();
			XPathNavigator navigator = doc.CreateNavigator();
			navigator.MoveToRoot();
			navigator.MoveToFirstChild();

			string transType = navigator.LocalName;

			switch(transType)
			{
				case "PrimaryAdjustment":
					transactionPopulater = primaryAdjustmentPopulater;
					break;
				case "SecondaryAdjustment":
					transactionPopulater = secondaryAdjustmentPopulater;
					break;
				case "PrimaryDefuel":
					transactionPopulater = primaryDefuelPopulater;
					break;
				case "SecondaryDefuel":
					transactionPopulater = secondaryDefuelPopulater;
					break;
				case "PrimaryDisbursement":
					transactionPopulater = primaryDisbursementPopulater;
					break;
				case "SecondaryDisbursement":
					transactionPopulater = secondaryDisbursementPopulater;
					break;
				case "FillStand":
					transactionPopulater = fillStandPopulater;
					break;
				case "Receipt":
					transactionPopulater = receiptPopulater;
					break;
				case "Request":
					transactionPopulater = requestPopulater;
					break;
				case "Unload":
					transactionPopulater = unloadPopulater;
					break;
				case "ConsumerTransfer":
					transactionPopulater = consumerTransferPopulater;
					break;
				case "MeterMovement":
					transactionPopulater = type12Populater;
					break;
				case "OwnerTransfer":
					transactionPopulater = ownerTransferPopulater;
					break;
				case "PhysicalInventory":
					transactionPopulater = physicalInventoryPopulater;
					break;
				case "PrimaryRegrade":
					transactionPopulater = primaryRegradePopulater;
					break;
				case "SecondaryRegrade":
					transactionPopulater = secondaryRegradePopulater;
					break;
				case "PhysicalReceipt":
				default:
					throw new AccountingServicesException("Cannot create Transaction Type \"" + transType + "\".");
			}

			transactionPopulater.SetImportProcessor(this.importProcessor);
			transactionValidationResult = transactionPopulater.PopulateTransaction(security, transaction, navigator);
			
			return transaction;
		}

		protected TransactionPopulater primaryAdjustmentPopulater;
		protected TransactionPopulater secondaryAdjustmentPopulater;
		protected TransactionPopulater primaryDefuelPopulater;
		protected TransactionPopulater secondaryDefuelPopulater;
		protected TransactionPopulater primaryDisbursementPopulater;
		protected TransactionPopulater secondaryDisbursementPopulater;
		protected TransactionPopulater fillStandPopulater;
		protected TransactionPopulater receiptPopulater;
		protected TransactionPopulater requestPopulater;
		protected TransactionPopulater unloadPopulater;
		protected TransactionPopulater consumerTransferPopulater;
		protected TransactionPopulater type12Populater;
		protected TransactionPopulater ownerTransferPopulater;
		protected TransactionPopulater physicalInventoryPopulater;
		protected TransactionPopulater primaryRegradePopulater;
		protected TransactionPopulater secondaryRegradePopulater;

		protected XMLImportProcessor importProcessor;
	}
}

