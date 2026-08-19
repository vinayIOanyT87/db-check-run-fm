using System;

using FM7Accounting;

namespace StandardXMLImportExport
{
	/// <summary>
	/// Summary description for PrimaryDefuelPopulater.
	/// </summary>
	public class PrimaryDefuelPopulater : TransactionPopulater
	{

		public PrimaryDefuelPopulater()
		{

		}

		protected override string TransactionTypeID
		{
			get
			{
				return "PrimaryDefuel";
			}
		}

		override protected void Populate()
		{

			transaction.LinkedDocumentNumber = GetStringValue("LinkedDocumentNumber", false);
		
			SetShipTo();

			PopulatePaymentInfo();
			PopulateRouteInfo();
			PopulateRouteSchedule();

			PopulateFuelingData();
			PopulateAviationGaugeReadings();
		}

		protected override void PopulateLineItem()
		{

		}

	}
}
