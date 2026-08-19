using System;

namespace StandardXMLImportExport
{
	/// <summary>
	/// Summary description for SecondaryDisbursementPopulater.
	/// </summary>
	public class SecondaryDisbursementPopulater : TransactionPopulater
	{
		public SecondaryDisbursementPopulater()
		{
			
		}

		protected override string TransactionTypeID
		{
			get
			{
				return "SecondaryDisbursement";
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

		}
	}
}
