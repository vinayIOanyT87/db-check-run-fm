using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;
using System.Data;

namespace FMBusinessObjects.BusinessInterfaces
{
	[ServiceContract]
	public interface ICompanies
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, CompanyClass company);

		[OperationContract]
		CompanyCollectionClass Enumerate(SecurityClass security);

		[OperationContract]
		CompanyCollectionClass EnumerateExt(SecurityClass security, bool byGroupCompanies, bool bLocalize = true, bool getExtendedInfo = false);

		[OperationContract]
		CompanyCollectionClass EnumerateExtPrime(SecurityClass security, bool byGroupCompanies, bool bLocalize = true, bool getExtendedInfo = false);

		[OperationContract]
		CompanyCollectionClass EnumerateBySite(SecurityClass security);

		[OperationContract]
		DataSet EnumerateCompaniesAllSites(SecurityClass security);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		CompanyCollectionClass EnumerateByRole(SecurityClass security, COMPANY_ROLE role, bool byGroupCompanies, bool bLocalize, bool hideHiddenCompanies = false);

		[OperationContract]
		CompanyCollectionClass GetEntriesForFieldGeneratorByRole(SecurityClass security, COMPANY_ROLE role, Guid transContextCompanyGuid, Guid fuelCardGuid, bool hideHiddenCompanies = false);

		[OperationContract]
		CompanyClass Get(SecurityClass security, Guid CompanyGuid, bool getExtendedInfo = true, bool hideHiddenProducts = false);

		[OperationContract]
		CompanyCollectionClass GetLoadRackCompanyClasses(SecurityClass security, CompanyMapCollectionClass companyMapClassCollection,Guid shipToBillToMapGuid, DateTimeOffset siteTimeNow, bool getExtendedInfo = true, bool hideHiddenProducts = false);

		[OperationContract]
		CompanyClass GetCarrierForLoadRack(SecurityClass security, Guid CompanyGuid);

		[OperationContract]
		Guid GetIdentityGuid(SecurityClass security, string id);

		[OperationContract]
		Guid GetMasterRecordGuid(SecurityClass security, string id);

		[OperationContract]
		CompanyClass GetBasicInfo(SecurityClass security, Guid companyGuid, Guid siteGuid);

		[OperationContract]
		List<Guid> GetCompanyGuidList(SecurityClass security, bool byGroupCompanies, bool localize);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, DATA_TYPE type, CompanyClass company);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid companyGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Import(SecurityClass security, CompanyClass company);

		[OperationContract]
		CompanyCollectionClass EnumerateAuthorizedCustomerShipToForColumnValue(SecurityClass security,
			string column,
			string value,
			Guid carrierGuid);

		[OperationContract]
		CompanyCollectionClass EnumerateAuthorizedSupplierForColumnValue(SecurityClass security,
			string column,
			string value);

		[OperationContract]
		CompanyCollectionClass EnumerateByRoleAndFilter(SecurityClass security,
																	COMPANY_ROLE role,
																	string filter,
																	bool byGroupCompanies);

		[OperationContract]
		string[] EnumerateColumnForAuthorizedCustomerShipTo(SecurityClass security, Guid carrierGuid, string column);

		[OperationContract]
		DataSet EnumerateCompanySelectRole(SecurityClass security, COMPANY_ROLE role, bool hideHiddenCompanies = false);

		[OperationContract]
		DataSet EnumerateCompanySelectRoleByLoadTypes(SecurityClass security, COMPANY_ROLE role, bool loadTypes, bool hideHiddenCompanies = false);

		[OperationContract]
		DataSet EnumerateByRoleAndFilterCompanySelect(SecurityClass security, COMPANY_ROLE role, string filter, bool hideHiddenCompanies = false);

		[OperationContract]
		DataSet EnumerateByRoleAndFilterCompanySelectAndLoadType(SecurityClass security, COMPANY_ROLE role, string filter, bool loadTypes, bool hideHiddenCompanies = false);

		[OperationContract]
		DataSet EnumerateHierarchialCustomerFromRoleCompanySelect(SecurityClass security,
																			COMPANY_ROLE role,
																			string managerString,
																			string ownerString,
																			string shipperString,
																			string billToString,
																			string filter,
																									 bool hideHiddenCompanies = false);

		[OperationContract]
		DataSet EnumerateByRoleCompanyGrid(SecurityClass security, COMPANY_ROLE role, bool byGroupCompanies, bool hideHiddenCompanies = false);

		[OperationContract]
		DataSet EnumerateByRoleAndFilterCompanyGrid(SecurityClass security, COMPANY_ROLE role, string filter, bool byGroupCompanies, bool hideHiddenCompanies = false);

		[OperationContract]
		string[] EnumerateColumnForAuthorizedSupplierOffLoadID(SecurityClass security, string column);

		[OperationContract]
		CompanyCollectionClass EnumerateHierarchialCustomerFromRole(SecurityClass security,
																						COMPANY_ROLE role,
																						string managerString,
																						string ownerString,
																						string shipperString,
																						string billToString,
																						string filter);

		[OperationContract]
		CompanyCollectionClass EnumerateByRoleGetIDCodeTypesIdentityGuidOnly(SecurityClass security, COMPANY_ROLE[] roles, bool hideHiddenCompanies = false);

		[OperationContract]
		CompanyCollectionClass EnumerateUndelegated(SecurityClass security);
	}
}
