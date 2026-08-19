using System;

using FMBusinessObjects.DataObjects;

namespace XMLImport
{
	/// <summary>
	/// Summary description for ReceiptPopulater.
	/// </summary>
	public class ReceiptPopulater : TransactionPopulater
	{
		public ReceiptPopulater()
		{
	
		}

		protected override TransactionTypes TransactionTypeID
		{
			get
			{
				return TransactionTypes.T8_Receipt;
			}
		}

		protected override void Populate()
		{
			PopulatePaymentInfo();
			SetSupplier();
		}

		protected override void PopulateLineItem()
		{
			SetLineItemBatchNumber();
		}


	}
}
