using System;
using System.ServiceModel;
using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.BusinessInterfaces
{
	[ServiceContract]
	public interface IWWIntegrationClass
	{
		[OperationContract]
		WWIntegrationDO GetForSite ( SecurityClass security, Guid siteGuid );// Get for a given siteGuid

		[OperationContract]
		WWIntegrationDO GetByIntegrationGuid(SecurityClass security, Guid integrationGuid);

		[OperationContract]
		WWIntegrationDOCollectionClass GetIntegrations( SecurityClass security );

		[OperationContract]
		WWIntegrationDO Get ( SecurityClass security );// Get for current site

		[OperationContract]
		WWIntegrationDOCollectionClass GetByStationIATACode ( SecurityClass security, string stationIATACode );

		[OperationContract]
		WWIntegrationDOCollectionClass GetByVendor(SecurityClass security, string vendorName);

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Add ( SecurityClass security, WWIntegrationDO integrationDO );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Save ( SecurityClass security, WWIntegrationDO integrationDO);

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Remove( SecurityClass security, Guid integrationGuid );
	}
}
