using System;
using System.ServiceModel;
using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.BusinessInterfaces
{
	[ServiceContract]
	public interface IExStarsSiteConfig
	{

		[OperationContract]
		ExStarsSiteConfigClass GetIrsSpecifiedIds(SecurityClass security, bool isTest, ref Guid managerCompanyGuid, ref Guid siteGuid);
	}
}
