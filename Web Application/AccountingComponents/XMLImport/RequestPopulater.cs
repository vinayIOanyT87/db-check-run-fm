using System;

using FMBusinessObjects.DataObjects;

namespace XMLImport
{
	/// <summary>
	/// Summary description for RequestPopulater.
	/// </summary>
	public class RequestPopulater : TransactionPopulater
	{
		public RequestPopulater()
		{
		
		}

		protected override TransactionTypes TransactionTypeID
		{
			get
			{
				return TransactionTypes.T9_Request;
			}
		}

		protected override void Populate()
		{
			SetSupplier();
			SetRequestedDeliveryDate();
			PopulateRouteSchedule();
		}

		protected override void PopulateLineItem()
		{

		}


	}
}
