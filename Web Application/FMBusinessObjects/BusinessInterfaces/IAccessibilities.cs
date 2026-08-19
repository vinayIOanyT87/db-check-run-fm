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
	public interface IAccessibilities
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, AccessibilityClass Accessibility);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, AccessibilityClass Accessibility);

		//[OperationContract]
		//[TransactionFlow(TransactionFlowOption.Allowed)]
		//void Purge(SecurityClass security, Guid identityGuid);

		[OperationContract]
		AccessibilityClass Get(SecurityClass security, Guid userGuid, string settingKey);

		//[OperationContract]
		//AllocationClass GetBySiteGuid(SecurityClass security, Guid identityGuid, Guid siteGuid, STATION_TYPE stationType, string transactionID);

		//[OperationContract]
		//AllocationClass GetByInventoryDate(SecurityClass security, Guid identityGuid, Guid siteGuid, STATION_TYPE stationType, string transactionID, DateTimeOffset transactionDate);

		//[OperationContract]
		//Guid GetIdentityGuid(SecurityClass security, Guid companyMapGuid, DateTimeOffset effectiveDate, DateTimeOffset expirationDate, COMPANY_MAP_TYPE companyMapType);

		[OperationContract]
		AccessibilityCollectionClass Enumerate(SecurityClass security, Guid userGuid);

		//[OperationContract]
		//AllocationCollectionClass EnumerateByCompanyMapGuid(SecurityClass security, Guid companyMapGuid, COMPANY_MAP_TYPE companyMapType);

		//[OperationContract]
		//AllocationCollectionClass EnumerateByCompanyMapType(SecurityClass security, COMPANY_MAP_TYPE type);

		//[OperationContract]
		//AllocationCollectionClass EnumerateByAllocationGroupGuid(SecurityClass security, Guid allocationGroupGuid);
	}
}
