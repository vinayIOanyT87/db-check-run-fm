using System;

using FM7Accounting;

namespace StandardXMLImportExport
{
	/// <summary>
	/// Summary description for SecondaryDefuelPopulater.
	/// </summary>
	public class SecondaryDefuelPopulater : TransactionPopulater
	{
		public SecondaryDefuelPopulater()
		{

		}

		protected override string TransactionTypeID
		{
			get
			{
				return "SecondaryDefuel";
			}
		}

		override protected void Populate()
		{
			SetShipTo();

			PopulatePaymentInfo();
			PopulateRouteInfo();
			PopulateRouteSchedule();

			PopulateFuelingData();
			PopulateAviationGaugeReadings();
		}

		override protected void PopulateLineItem()
		{

		}
	}
}
