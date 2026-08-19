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
	public interface IQueryDefaultFields
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Add(SecurityClass security, QueryDefaultFieldClass QueryDefaultField);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, QueryDefaultFieldClass QueryDefaultField);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid identityGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Update(SecurityClass security, QueryDefaultFieldCollectionClass fieldCollection);

		[OperationContract]
		QueryDefaultFieldClass Get(SecurityClass security, Guid identityGuid);

		[OperationContract]
		QueryDefaultFieldCollectionClass Enumerate(SecurityClass security);

		[OperationContract]
		QueryDefaultFieldCollectionClass EnumerateBySite(SecurityClass security);
	}
}
