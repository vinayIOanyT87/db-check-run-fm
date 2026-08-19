namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface IOperateScreenConfigurations
	{
		[OperationContract]
		OperateScreenConfiguration GetBySiteUserClientIpAddress(SecurityClass security, Guid siteGuid, Guid userGuid, string clientIpAddress);

		[OperationContract]
		void SetScreenMask(SecurityClass security, Guid siteGuid, Guid userGuid, string clientIpAddress, long screenMask);
	}
}
