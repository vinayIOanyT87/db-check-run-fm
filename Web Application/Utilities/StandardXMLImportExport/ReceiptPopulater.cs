using System;

namespace StandardXMLImportExport
{
	/// <summary>
	/// Summary description for ReceiptPopulater.
	/// </summary>
	public class ReceiptPopulater : TransactionPopulater
	{
		public ReceiptPopulater()
		{
	
		}

		protected override string TransactionTypeID
		{
			get
			{
				return "Receipt";
			}
		}

		protected override void Populate()
		{
			SetSupplier();
		}

		protected override void PopulateLineItem()
		{
			SetLineItemBatchNumber();
		}


	}
}
