using System;

namespace StandardXMLImportExport
{
	/// <summary>
	/// Summary description for OwnerTransfer.
	/// </summary>
	public class OwnerTransferPopulater : TransactionPopulater
	{
		public OwnerTransferPopulater()
		{
			
		}

		protected override string TransactionTypeID
		{
			get
			{
				return "OwnerTransfer";
			}
		}

		protected override void Populate()
		{
			this.SetConjoinedTransID();
		}

		protected override void PopulateLineItem()
		{

		}

	}
}
