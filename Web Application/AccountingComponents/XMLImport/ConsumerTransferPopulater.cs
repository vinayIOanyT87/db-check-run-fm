using System;

using FMBusinessObjects.DataObjects;

namespace XMLImport
{
	/// <summary>
	/// Summary description for ConsumerTransferPopulater.
	/// </summary>
	public class ConsumerTransferPopulater : TransactionPopulater
	{
		public ConsumerTransferPopulater()
		{
			
		}

		protected override TransactionTypes TransactionTypeID
		{
			get
			{
				return TransactionTypes.T11_ConsumerTransfer;
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
