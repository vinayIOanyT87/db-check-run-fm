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
	public interface ICompanyRoleMaps
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Add ( SecurityClass security, CompanyRoleMapClass CompanyRoleMap );

		[OperationContract]
		CompanyRoleMapCollectionClass EnumerateByCompany(SecurityClass security, Guid companyGuid);

		[OperationContract]
		List<CompanyRoleMapClass> EnumerateByCriterion( SecurityClass security,
																							Guid inSiteGuid,
																							string inFindString,
																							Guid inCompanyGuid,
																							COMPANY_ROLE inRole,
																							bool includeMemberSites,
																							string sortKey );

		[OperationContract]
		List<CompanyRoleMapClass> EnumerateBySiteForRoleMapping(SecurityClass security, Guid targetSiteGuid);

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Purge ( SecurityClass security, CompanyRoleMapClass companyRoleMap );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void PurgeByRole(SecurityClass security, Guid companyGuid, COMPANY_ROLE Role);
	}
}
