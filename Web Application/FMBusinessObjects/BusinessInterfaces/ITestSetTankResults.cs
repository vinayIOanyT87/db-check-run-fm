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
	public interface ITestSetTankResults
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, TestSetTankResultClass TestSetTankResult);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, TestSetTankResultClass TestSetTankResult);

		[OperationContract]
		TestSetTankResultClass Get(SecurityClass security, Guid testSetTankResultGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid testSetTankResultGuid);

		[OperationContract]
		TestSetTankResultCollectionClass EnumerateByDates(SecurityClass security, DateTimeOffset startDate, DateTimeOffset endDate);

		[OperationContract]
		TestSetTankResultCollectionClass Enumerate(SecurityClass security);

		[OperationContract]
		TestSetTankResultCollectionClass EnumerateByTankGuid(SecurityClass security, Guid identityGuid);

		[OperationContract]
		TestSetTankResultClass GetPreviousSampleNumber(SecurityClass security);

		[OperationContract]
		bool FindDuplicateSampleNumber(SecurityClass security, int SampleNumber, Guid testSetTankResultGuid);
	}
}
