using System;

namespace StandardXMLImportExport
{
	/// <summary>
	/// Summary description for ConsumerTransferPopulater.
	/// </summary>
	public class ConsumerTransferPopulater : TransactionPopulater
	{
		public ConsumerTransferPopulater()
		{
			
		}

		protected override string TransactionTypeID
		{
			get
			{
				return "ConsumerTransfer";
			}
		}

		protected override void Populate()
		{
			this.SetConjoinedTransID();
			SetShipTo();
		}

		protected override void PopulateLineItem()
		{

		}

		
	}
}
