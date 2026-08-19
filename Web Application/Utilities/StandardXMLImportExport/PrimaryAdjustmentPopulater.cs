using System;

using FM7Accounting;

namespace StandardXMLImportExport
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

		override protected string TransactionTypeID
		{
			get { return "PrimaryAdjustment"; }
		}
		#endregion Overrides
	}
}
