using System;

using FMBusinessObjects.DataObjects;

namespace XMLImport
{
	/// <summary>
	/// Summary description for SecondaryDisbursementPopulater.
	/// </summary>
	public class SecondaryDisbursementPopulater : TransactionPopulater
	{
		public SecondaryDisbursementPopulater()
		{
			
		}

		protected override TransactionTypes TransactionTypeID
		{
			get
			{
				return TransactionTypes.T6_SecondaryDisbursement;
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

		}
	}
}
