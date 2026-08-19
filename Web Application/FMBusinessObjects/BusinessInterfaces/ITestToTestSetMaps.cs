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
	public interface ITestToTestSetMaps
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		Guid Add ( SecurityClass security, TestToTestSetMapClass testToTestSetMap );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Modify ( SecurityClass security, TestToTestSetMapClass testToTestSetMap );

		[OperationContract]
		TestToTestSetMapClass Get ( SecurityClass security, Guid identityGuid );

		[OperationContract]
		TestToTestSetMapClass GetByDefinition ( SecurityClass security, Guid testDefinitionGuid, Guid testSetDefinitionGuid );

		[OperationContract]
		Guid GetIdentityGuid(SecurityClass security, Guid testDefinitionGuid, Guid testSetDefinitionGuid);

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Purge(SecurityClass security, Guid testDefinitionToTestSetDefinitionGuid);

		[OperationContract]
		TestToTestSetMapCollectionClass Enumerate ( SecurityClass security );

		[OperationContract]
		TestToTestSetMapCollectionClass EnumerateByTestGuid ( SecurityClass security, Guid identityGuid );

		[OperationContract]
		TestToTestSetMapCollectionClass EnumerateByTestSetGuid(SecurityClass security, Guid identityGuid);
	}
}
