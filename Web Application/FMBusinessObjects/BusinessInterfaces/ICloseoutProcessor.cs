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
	public interface ICloseoutProcessor
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		CloseoutDO Process ( CloseoutSR sr );

		[OperationContract]
		bool IsClosedOut(
            SiteClass site, Guid managerGuid, Guid productGuid, string managerID, string productID,
			DateTime inventoryDate,
			ProductType type,
			CloseoutDO closeoutDO,
			SecurityClass security);

	}
}
