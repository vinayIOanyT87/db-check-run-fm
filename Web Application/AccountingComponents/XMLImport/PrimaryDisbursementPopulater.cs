using System;

using FMBusinessObjects.DataObjects;

namespace XMLImport
{
	/// <summary>
	/// Summary description for PrimaryDisbursementPopulater.
	/// </summary>
	public class PrimaryDisbursementPopulater : TransactionPopulater
	{
		public PrimaryDisbursementPopulater()
		{

		}

		protected override TransactionTypes TransactionTypeID
		{
			get
			{
				return TransactionTypes.T5_PrimaryDisbursement;
			}
		}

		protected override void Populate()
		{
			SetShipTo();
			PopulatePaymentInfo();
			PopulateRouteInfo();
			PopulateRouteSchedule();
			PopulateWeightReadings();

		}

		protected override void PopulateLineItem()
		{
			PopulateSubLineItems();	
		}

	}
}
