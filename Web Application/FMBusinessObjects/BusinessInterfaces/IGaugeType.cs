using FMBusinessObjects.DataObjects;
using System;
using System.ServiceModel;

namespace FMBusinessObjects.BusinessInterfaces
{
    [ServiceContract]
	public interface IGaugeTypes
	{
		[OperationContract]
		GaugeTypeClass Get(SecurityClass security, Guid guid);


		[OperationContract]
		GaugeTypeClass GetByID(SecurityClass security, string id);

		[OperationContract]
		Guid GetIdentityGuid ( SecurityClass security, string id );

		[OperationContract]
		GaugeTypeCollectionClass Enumerate ( SecurityClass security );
	}
}
