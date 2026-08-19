using System;

using FMBusinessObjects.DataObjects;

namespace XMLImport
{
	/// <summary>
	/// Summary description for FillStandPopulater.
	/// </summary>
	public class FillStandPopulater : TransactionPopulater
	{
		public FillStandPopulater()
		{
		
		}

		#region Overrides
		protected override TransactionTypes TransactionTypeID
		{
			get
			{
				return TransactionTypes.T7_FillStand;
			}
		}

		protected override void Populate()
		{

		}

		protected override void PopulateLineItem()
		{
			PopulateSubLineItems();
		}
		#endregion Overrides
	}
}
