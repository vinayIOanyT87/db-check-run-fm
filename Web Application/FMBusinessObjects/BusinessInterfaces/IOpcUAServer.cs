using System;
using System.ServiceModel;

using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.BusinessInterfaces
{

	[ServiceContract]
	public interface IOpcUAServer
	{

		[OperationContract]
		OpcUAServerCollectionClass GetAll(SecurityClass security);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, OpcUAServerClass opcUaServer);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, OpcUAServerClass opcUaServer);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid opcUAServerGuid);

		[OperationContract]
		OpcUAServerClass Get(SecurityClass security, Guid opcUAServerGuid);

		[OperationContract]
		OpcUAServerClass GetByEndpoint(SecurityClass security, string endpoint);
	}
}
