using System;

using FMBusinessObjects.DataObjects;

namespace XMLImport
{
	/// <summary>
	/// Summary description for PrimaryRegradePopulater.
	/// </summary>
	public class PrimaryRegradePopulater : TransactionPopulater
	{
		public PrimaryRegradePopulater()
		{
		
		}

		protected override TransactionTypes TransactionTypeID
		{
			get
			{
				return TransactionTypes.T15_PrimaryRegrade;
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
