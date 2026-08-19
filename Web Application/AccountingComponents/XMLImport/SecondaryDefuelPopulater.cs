using System;

using FMBusinessObjects.DataObjects;

namespace XMLImport
{
	/// <summary>
	/// Summary description for SecondaryDefuelPopulater.
	/// </summary>
	public class SecondaryDefuelPopulater : TransactionPopulater
	{
		public SecondaryDefuelPopulater()
		{

		}

		protected override TransactionTypes TransactionTypeID
		{
			get
			{
				return TransactionTypes.T4_SecondaryDefuel;
			}
		}

		override protected void Populate()
		{
			SetShipTo();

			PopulatePaymentInfo();
			PopulateRouteInfo();
			PopulateRouteSchedule();

			PopulateWeightReadings();
		}

		override protected void PopulateLineItem()
		{

		}
	}
}
