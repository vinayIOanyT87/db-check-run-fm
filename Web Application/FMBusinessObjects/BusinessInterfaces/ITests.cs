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
	public interface ITests
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		Guid Add ( SecurityClass security, TestClass test );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Modify ( SecurityClass security, TestClass test );

		[OperationContract]
		bool IsAssociatedWithTestResult ( SecurityClass security, Guid identityGuid );

		[OperationContract]
		TestClass Get ( SecurityClass security, Guid identityGuid );

		[OperationContract]
		Guid GetIdentityGuid ( SecurityClass security, string id );

		[OperationContract]
		bool ValidateTestResult ( SecurityClass security, TestClass test, string data );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Purge ( SecurityClass security, Guid testGuid );

		[OperationContract]
		TestCollectionClass Enumerate ( SecurityClass security, string filter, string order );
	}
}
