using System;

using FMBusinessObjects.DataObjects;

namespace XMLImport
{
	/// <summary>
	/// Summary description for SecondaryAdjustmentPopulater.
	/// </summary>
	public class SecondaryAdjustmentPopulater : TransactionPopulater
	{
		public SecondaryAdjustmentPopulater()
		{

		}

		protected override TransactionTypes TransactionTypeID
		{
			get
			{
				return TransactionTypes.T2_SecondaryAdjustment;
			}
		}

		protected override void Populate()
		{
		}

		protected override void PopulateLineItem()
		{
			
		}
	}
}
