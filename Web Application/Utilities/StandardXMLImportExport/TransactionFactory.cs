using System;

using FM7Accounting;

namespace StandardXMLImportExport
{
	/// <summary>
	/// Summary description for TransactionFactory.
	/// </summary>
	public class TransactionFactory
	{
		public TransactionFactory()
		{
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

		public TransactionDO CreateTransaction(System.Xml.XmlDocument doc)
		{
			TransactionDO transaction = null;
			TransactionPopulater transactionPopulater = null;

			transaction = new TransactionDO();

			string transType = doc.DocumentElement.LocalName;
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
					throw new Exception("Cannot create Transaction Type \"" + transType + "\".");
					break;
			}

			transactionPopulater.PopulateTransaction(transaction, doc);
			
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
	}
}

