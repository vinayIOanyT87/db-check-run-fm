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
	public interface ITestEquipmentResults
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, TestEquipmentResultClass TestEquipmentResult);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, TestEquipmentResultClass TestEquipmentResult);

		[OperationContract]
		TestEquipmentResultClass Get(SecurityClass security, Guid testEquipmentResultGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid testEquipmentResultGuid);

		[OperationContract]
		TestEquipmentResultCollectionClass Enumerate(SecurityClass security);

		[OperationContract]
		TestEquipmentResultCollectionClass EnumerateByTestSetEquipmentResultGuid(SecurityClass security, Guid identityGuid);
	}
}
