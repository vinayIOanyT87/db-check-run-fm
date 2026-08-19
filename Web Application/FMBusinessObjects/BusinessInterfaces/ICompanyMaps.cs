using System;
using System.Collections.Generic;
using System.ServiceModel;
using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.BusinessInterfaces
{
	[ServiceContract]
	public interface ICompanyMaps
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		Guid Add ( SecurityClass security, CompanyMapClass CompanyMap );

		[OperationContract]
		CompanyMapCollectionClass EnumerateByType( SecurityClass security, COMPANY_MAP_TYPE type );

		[OperationContract]
		CompanyMapCollectionClass EnumerateByAssignedGuidAndType( SecurityClass security, Guid assignedGuid, COMPANY_MAP_TYPE Type );

		[OperationContract]
		CompanyMapCollectionClass EnumerateByAssignedToGuidAndType(SecurityClass security, Guid assignedToGuid, COMPANY_MAP_TYPE Type);

		[OperationContract]
		List<Guid> EnumerateGroupMapsWithAllCompaniesAssigned(SecurityClass security);

		[OperationContract]
		CompanyMapClass Get( SecurityClass security, Guid identityGuid, COMPANY_MAP_TYPE Type );

		[OperationContract]
		CompanyMapCollectionClass GetLoadRackCompanyMapClasses(SecurityClass security, Guid identityGuid);

		[OperationContract]
		Guid GetOffLoadIdentityGuidByMapID( SecurityClass security, string id );

		[OperationContract]
		Guid GetIdentityGuidByMapID(SecurityClass security, string ID);

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Modify ( SecurityClass security, CompanyMapClass CompanyMap );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Purge(SecurityClass security, Guid identityGuid, COMPANY_MAP_TYPE Type);

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void ModifyCollection(SecurityClass security, Guid identityGuid, string id, CompanyMapCollectionClass newCompanyMapCollection, CompanyMapCollectionClass existingCompanyMapCollection);

		[OperationContract]
		Guid GetIdentityGuidByGuidsAndType(SecurityClass security, Guid assignedToGuid, Guid assignedGuid, COMPANY_MAP_TYPE type);

		[OperationContract]
		CompanyMapClass GetLoadIdMapWithoutPersonnelCheck(SecurityClass security, Guid loadIDToCompanyShipToMapGuid);

		[OperationContract]
		CompanyMapClass GetMinimal(SecurityClass security, Guid identityGuid, COMPANY_MAP_TYPE Type);
	}
}
