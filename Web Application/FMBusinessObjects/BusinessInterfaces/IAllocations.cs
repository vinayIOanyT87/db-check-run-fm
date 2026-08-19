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
	public interface IAllocations
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, AllocationClass allocation);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, AllocationClass allocation);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid identityGuid);

		[OperationContract]
		AllocationClass Get(SecurityClass security, Guid identityGuid, STATION_TYPE stationType, string transactionID);

		[OperationContract]
		AllocationClass GetBySiteGuid(SecurityClass security, Guid identityGuid, Guid siteGuid, STATION_TYPE stationType, string transactionID);

		[OperationContract]
		AllocationClass GetByInventoryDate(SecurityClass security, Guid identityGuid, Guid siteGuid, STATION_TYPE stationType, string transactionID, DateTimeOffset transactionDate);

		[OperationContract]
		Guid GetIdentityGuid(SecurityClass security, Guid companyMapGuid, DateTimeOffset effectiveDate, DateTimeOffset expirationDate, COMPANY_MAP_TYPE companyMapType);

		[OperationContract]
		AllocationCollectionClass Enumerate(SecurityClass security);

		[OperationContract]
		AllocationCollectionClass EnumerateByCompanyMapGuid(SecurityClass security, Guid companyMapGuid, COMPANY_MAP_TYPE companyMapType);

		[OperationContract]
		AllocationCollectionClass EnumerateByCompanyMapType(SecurityClass security, COMPANY_MAP_TYPE type);

		[OperationContract]
		AllocationCollectionClass EnumerateByAllocationGroupGuid(SecurityClass security, Guid allocationGroupGuid);

		[OperationContract]
		bool CanViewAllocation(SecurityClass security, CompanyMapClass companyMap, COMPANY_MAP_TYPE mapType, List<GroupClass> inUserGroupList);

		[OperationContract]
		bool CanModifyAllocation(SecurityClass security, CompanyMapClass companyMap, COMPANY_MAP_TYPE mapType, List<GroupClass> inUserGroupList);

		[OperationContract]
		List<GroupClass> GetUserGroups(SecurityClass security);

		[OperationContract]
		AllocationClass.UserAllocationStatus UserHasAllocationRightsAndCompanyMapCollection(SecurityClass security);
	}
}
