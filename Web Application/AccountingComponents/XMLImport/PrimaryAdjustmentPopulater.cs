using System;

using FMBusinessObjects.DataObjects;

namespace XMLImport
{
	/// <summary>
	/// Summary description for PrimaryAdjustmentPopulater.
	/// </summary>
	public class PrimaryAdjustmentPopulater : TransactionPopulater
	{
		public PrimaryAdjustmentPopulater()
		{

		}

		#region Overrides
		protected override void  Populate()
		{
			
		}

		protected override void PopulateLineItem()
		{

		}

		override protected TransactionTypes TransactionTypeID
		{
			get { return TransactionTypes.T1_PrimaryAdjustment; }
		}
		#endregion Overrides
	}
}
