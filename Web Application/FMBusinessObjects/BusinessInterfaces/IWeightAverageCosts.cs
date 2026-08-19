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
	public interface IWeightedAverageCosts
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Add(SecurityClass security, WeightedAverageCostClass wac);

		[OperationContract]
		WeightedAverageCostClass GetByIdentityGuid(SecurityClass security, Guid weightedAverageCostGuid);

		[OperationContract]
		WeightedAverageCostClass GetLatest(SecurityClass security, Guid siteGuid, Guid productGuid);

		[OperationContract]
		WeightedAverageCostCollectionClass Enumerate(SecurityClass security);

		[OperationContract]
		WeightedAverageCostCollectionClass EnumerateBySiteProductDate(SecurityClass security,
																							Guid siteGuid,
																							Guid productGuid,
																							DateTimeOffset startDate,
																							DateTimeOffset endDate);
	}
}
