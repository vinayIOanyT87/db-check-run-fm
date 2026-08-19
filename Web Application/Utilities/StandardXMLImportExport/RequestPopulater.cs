using System;

namespace StandardXMLImportExport
{
	/// <summary>
	/// Summary description for RequestPopulater.
	/// </summary>
	public class RequestPopulater : TransactionPopulater
	{
		public RequestPopulater()
		{
		
		}

		protected override string TransactionTypeID
		{
			get
			{
				return "Request";
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
