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
	public interface ITestTankResults
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		Guid Add ( SecurityClass security, TestTankResultClass TestTankResult );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Modify ( SecurityClass security, TestTankResultClass TestTankResult );

		[OperationContract]
		TestTankResultClass Get(SecurityClass security, Guid testTankResultGuid);

		[OperationContract]
		TestTankResultCollectionClass Enumerate ( SecurityClass security );

		[OperationContract]
		TestTankResultCollectionClass EnumerateByTestSetTankResultGuid ( SecurityClass security, Guid identityGuid );
	}
}
