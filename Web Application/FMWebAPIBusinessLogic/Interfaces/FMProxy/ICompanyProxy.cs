using System;
using System.Collections.Generic;
using System.Data;
using FMBusinessObjects.DataObjects;

namespace FMWebAPIBusinessLogic.Interfaces.FMProxy
{
    public interface ICompanyProxy
    {
        Guid Add(CompanyClass company);
        CompanyCollectionClass Enumerate();
        CompanyCollectionClass EnumerateAuthorizedCustomerShipToForColumnValue(string column, string value, Guid carrierGuid);
        CompanyCollectionClass EnumerateAuthorizedSupplierForColumnValue(string column, string value);
        CompanyCollectionClass EnumerateByRole(COMPANY_ROLE role, bool byGroupCompanies, bool bLocalize, bool hideHiddenCompanies = false);
        CompanyCollectionClass EnumerateByRoleAndFilter(COMPANY_ROLE role, string filter, bool byGroupCompanies);
        DataSet EnumerateByRoleAndFilterCompanyGrid(COMPANY_ROLE role, string filter, bool byGroupCompanies, bool hideHiddenCompanies = false);
        DataSet EnumerateByRoleAndFilterCompanySelect(COMPANY_ROLE role, string filter, bool hideHiddenCompanies = false);
        DataSet EnumerateByRoleAndFilterCompanySelectAndLoadType(COMPANY_ROLE role, string filter, bool loadTypes, bool hideHiddenCompanies = false);
        DataSet EnumerateByRoleCompanyGrid(COMPANY_ROLE role, bool byGroupCompanies, bool hideHiddenCompanies = false);
        CompanyCollectionClass EnumerateByRoleGetIDCodeTypesIdentityGuidOnly(COMPANY_ROLE[] roles, bool hideHiddenCompanies = false);
        CompanyCollectionClass EnumerateBySite();
        string[] EnumerateColumnForAuthorizedCustomerShipTo(Guid carrierGuid, string column);
        string[] EnumerateColumnForAuthorizedSupplierOffLoadID(string column);
        DataSet EnumerateCompanySelectRole(COMPANY_ROLE role, bool hideHiddenCompanies = false);
        DataSet EnumerateCompanySelectRoleByLoadTypes(COMPANY_ROLE role, bool loadTypes, bool hideHiddenCompanies = false);
        CompanyCollectionClass EnumerateExt(bool byGroupCompanies, bool bLocalize = true, bool getExtendedInfo = false);
        CompanyCollectionClass EnumerateExtPrime(bool byGroupCompanies, bool bLocalize = true, bool getExtendedInfo = false);
        CompanyCollectionClass EnumerateHierarchialCustomerFromRole(COMPANY_ROLE role, string managerString, string ownerString, string shipperString, string billToString, string filter);
        DataSet EnumerateHierarchialCustomerFromRoleCompanySelect(COMPANY_ROLE role, string managerString, string ownerString, string shipperString, string billToString, string filter, bool hideHiddenCompanies = false);
        CompanyCollectionClass EnumerateUndelegated();
        CompanyClass Get(Guid CompanyGuid, bool getExtendedInfo = true, bool hideHiddenProducts = false);
        CompanyClass GetBasicInfo(Guid companyGuid, Guid siteGuid);
        List<Guid> GetCompanyGuidList(bool byGroupCompanies, bool localize);
        CompanyCollectionClass GetEntriesForFieldGeneratorByRole(COMPANY_ROLE role, Guid transContextCompanyGuid, Guid fuelCardGuid, bool hideHiddenCompanies = false);
        Guid GetIdentityGuid(string id);
        Guid GetMasterRecordGuid(string id);
        void Import(CompanyClass company);
        void Modify(DATA_TYPE type, CompanyClass company);
        void Purge(Guid companyGuid);
    }
}