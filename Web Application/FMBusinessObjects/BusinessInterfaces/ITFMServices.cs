using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;


namespace FMBusinessObjects.BusinessInterfaces
{
	[ServiceContract]
	public interface ITFMSServices
	{
		[OperationContract]
		bool IsDirectPurchaseNumberUnique ( SecurityClass security, string purchaseNumber );
	}
}
