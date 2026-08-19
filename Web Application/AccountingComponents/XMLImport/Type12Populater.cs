using System;

using FMBusinessObjects.DataObjects;

namespace XMLImport
{
	/// <summary>
	/// Summary description for Type12Populater.
	/// </summary>
	public class Type12Populater : TransactionPopulater
	{
		public Type12Populater()
		{
			
		}

		protected override TransactionTypes TransactionTypeID
		{
			get
			{
				return TransactionTypes.T12_InventoryNotAffected;
			}
		}

		protected override void Populate()
		{
			SetShipTo();
		}

		protected override void PopulateLineItem()
		{

		}
	}
}
