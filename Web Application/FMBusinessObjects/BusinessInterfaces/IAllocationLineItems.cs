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
	public interface IAllocationLineItems
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, AllocationLineItemClass allocationLineItem);

		[OperationContract]
		double GetAmountLoaded(SecurityClass security,
														string allocationID,
														Guid itemGuid,
														ALLOCATION_TYPE allocationType,
														ALLOCATION_RESET_PERIOD resetPeriod,
														int resetMultiple,
														DateTimeOffset resetDate,
														DateTimeOffset lastAllocationResetDate,
														DateTimeOffset expirationDate,
														Guid siteGuid,
														STATION_TYPE stationType,
														string transactionID);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, AllocationLineItemClass allocationLineItem);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid AllocationLineItemGuid);
	}
}
