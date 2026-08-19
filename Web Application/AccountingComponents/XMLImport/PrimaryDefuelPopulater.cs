using System;

using FMBusinessObjects.DataObjects;

namespace XMLImport
{
	/// <summary>
	/// Summary description for PrimaryDefuelPopulater.
	/// </summary>
	public class PrimaryDefuelPopulater : TransactionPopulater
	{

		public PrimaryDefuelPopulater()
		{

		}

		protected override TransactionTypes TransactionTypeID
		{
			get
			{
				return TransactionTypes.T3_PrimaryDefuel;
			}
		}

		override protected void Populate()
		{

			transaction.LinkedDocumentNumber = GetStringValue("LinkedDocumentNumber", false, this.transactionNavigator);
		
			SetShipTo();

			PopulatePaymentInfo();
			PopulateRouteInfo();
			PopulateRouteSchedule();

//			PopulateFuelingData();
			PopulateWeightReadings();
		}

		protected override void PopulateLineItem()
		{

		}

	}
}
