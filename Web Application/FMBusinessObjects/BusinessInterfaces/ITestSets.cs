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
	public interface ITestSets
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		Guid Add ( SecurityClass security, TestSetClass testSet );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Modify ( SecurityClass security, TestSetClass testSet );

		[OperationContract]
		TestSetClass Get(SecurityClass security, Guid testSetGuid);

		[OperationContract]
		TestSetClass GetByIncludeTests(SecurityClass security, Guid testSetGuid, bool includeTests);

		[OperationContract]
		Guid GetIdentityGuid ( SecurityClass security, string id );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Purge(SecurityClass security, Guid testSetGuid);

		[OperationContract]
		TestSetCollectionClass Enumerate ( SecurityClass security, string filter, string order );
	}
}
