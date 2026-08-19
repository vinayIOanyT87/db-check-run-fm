using System;

using FM7Accounting;

namespace StandardXMLImportExport
{
	/// <summary>
	/// Summary description for PrimaryDisbursementPopulater.
	/// </summary>
	public class PrimaryDisbursementPopulater : TransactionPopulater
	{
		public PrimaryDisbursementPopulater()
		{

		}

		protected override string TransactionTypeID
		{
			get
			{
				return "PrimaryDisbursement";
			}
		}

		protected override void Populate()
		{
			SetShipTo();
			PopulateRouteInfo();
			PopulateRouteSchedule();
			PopulateAviationGaugeReadings();

		}

		protected override void PopulateLineItem()
		{
			PopulateSubLineItems();	
		}

	}
}
