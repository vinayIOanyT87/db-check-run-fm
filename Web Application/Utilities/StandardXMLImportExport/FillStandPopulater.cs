using System;

using FM7Accounting;

namespace StandardXMLImportExport
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
		protected override string TransactionTypeID
		{
			get
			{
				return "FillStand";
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
