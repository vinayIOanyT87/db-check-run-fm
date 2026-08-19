using System;

using FMBusinessObjects.DataObjects;

namespace XMLImport
{
	/// <summary>
	/// Summary description for SecondaryRegradePopulater.
	/// </summary>
	public class SecondaryRegradePopulater : TransactionPopulater
	{
		public SecondaryRegradePopulater()
		{
			
		}

		protected override TransactionTypes TransactionTypeID
		{
			get
			{
				return TransactionTypes.T16_SecondaryRegrade;
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
