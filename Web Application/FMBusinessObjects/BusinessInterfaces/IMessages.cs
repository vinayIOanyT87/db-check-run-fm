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
	public interface IMessages
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		Guid Add(SecurityClass security, MessageClass Message);

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Modify ( SecurityClass security, MessageClass Message );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		MessageClass Get ( SecurityClass security, Guid identityGuid );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		Guid GetIdentityGuid(SecurityClass security, string ID, Guid companyGuid, Guid personnelGuid);

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Purge(SecurityClass security, Guid messageGuid);

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		MessageCollectionClass Enumerate ( SecurityClass security );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		MessageCollectionClass EnumerateByCompany(SecurityClass security, Guid companyGuid);

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		MessageCollectionClass EnumerateByGuids(SecurityClass security, Guid companyGuid, Guid personnelGuid);

	}

	
}
