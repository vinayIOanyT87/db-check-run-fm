using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using FMCore.Interfaces;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic;
using FMWebAPIBusinessLogic.Interfaces.FMProxy;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace FMWebAPIBusinessLogic.Services.FMProxy
{
    public class CompanyProxy : ICompanyProxy
    {
        ICurrentRequestContext _requestContext;
        IFMCustomLogger _logger;
        public CompanyProxy(ICurrentRequestContext requestContext,
            IFMCustomLogger logger)
        {
            this._requestContext = requestContext;
            this._logger = logger;
        }

        public Guid Add(CompanyClass company)
        {
            throw new NotImplementedException();
        }

        public CompanyCollectionClass Enumerate()
        {
            throw new NotImplementedException();
        }

        public CompanyCollectionClass EnumerateAuthorizedCustomerShipToForColumnValue(string column, string value, Guid carrierGuid)
        {
            throw new NotImplementedException();
        }

        public CompanyCollectionClass EnumerateAuthorizedSupplierForColumnValue(string column, string value)
        {
            throw new NotImplementedException();
        }

        public CompanyCollectionClass EnumerateByRole(COMPANY_ROLE role, bool byGroupCompanies, bool bLocalize, bool hideHiddenCompanies = false)
        {
            try
            {
                var timer = Stopwatch.StartNew();
                var currentSecurity = this._requestContext.GetCurrentSecurityContext();
                var result = FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
                    service => service.EnumerateByRole(currentSecurity, role, byGroupCompanies, bLocalize, hideHiddenCompanies));
                timer.Stop();
                _logger.Debug($"Took {timer.ElapsedMilliseconds}ms");
                return result;
            }
            catch(Exception e)
            {
                _logger.Error(e, "something failed");
                throw;
            }
        }

        public CompanyCollectionClass EnumerateByRoleAndFilter(COMPANY_ROLE role, string filter, bool byGroupCompanies)
        {
            throw new NotImplementedException();
        }

        public DataSet EnumerateByRoleAndFilterCompanyGrid(COMPANY_ROLE role, string filter, bool byGroupCompanies, bool hideHiddenCompanies = false)
        {
            throw new NotImplementedException();
        }

        public DataSet EnumerateByRoleAndFilterCompanySelect(COMPANY_ROLE role, string filter, bool hideHiddenCompanies = false)
        {
            throw new NotImplementedException();
        }

        public DataSet EnumerateByRoleAndFilterCompanySelectAndLoadType(COMPANY_ROLE role, string filter, bool loadTypes, bool hideHiddenCompanies = false)
        {
            throw new NotImplementedException();
        }

        public DataSet EnumerateByRoleCompanyGrid(COMPANY_ROLE role, bool byGroupCompanies, bool hideHiddenCompanies = false)
        {
            throw new NotImplementedException();
        }

        public CompanyCollectionClass EnumerateByRoleGetIDCodeTypesIdentityGuidOnly(COMPANY_ROLE[] roles, bool hideHiddenCompanies = false)
        {
            throw new NotImplementedException();
        }

        public CompanyCollectionClass EnumerateBySite()
        {
            throw new NotImplementedException();
        }

        public string[] EnumerateColumnForAuthorizedCustomerShipTo(Guid carrierGuid, string column)
        {
            throw new NotImplementedException();
        }

        public string[] EnumerateColumnForAuthorizedSupplierOffLoadID(string column)
        {
            throw new NotImplementedException();
        }

        public DataSet EnumerateCompanySelectRole(COMPANY_ROLE role, bool hideHiddenCompanies = false)
        {
            throw new NotImplementedException();
        }

        public DataSet EnumerateCompanySelectRoleByLoadTypes(COMPANY_ROLE role, bool loadTypes, bool hideHiddenCompanies = false)
        {
            throw new NotImplementedException();
        }

        public CompanyCollectionClass EnumerateExt(bool byGroupCompanies, bool bLocalize = true, bool getExtendedInfo = false)
        {
            throw new NotImplementedException();
        }

        public CompanyCollectionClass EnumerateExtPrime(bool byGroupCompanies, bool bLocalize = true, bool getExtendedInfo = false)
        {
            throw new NotImplementedException();
        }

        public CompanyCollectionClass EnumerateHierarchialCustomerFromRole(COMPANY_ROLE role, string managerString, string ownerString, string shipperString, string billToString, string filter)
        {
            throw new NotImplementedException();
        }

        public DataSet EnumerateHierarchialCustomerFromRoleCompanySelect(COMPANY_ROLE role, string managerString, string ownerString, string shipperString, string billToString, string filter, bool hideHiddenCompanies = false)
        {
            throw new NotImplementedException();
        }

        public CompanyCollectionClass EnumerateUndelegated()
        {
            throw new NotImplementedException();
        }

        public CompanyClass Get(Guid CompanyGuid, bool getExtendedInfo = true, bool hideHiddenProducts = false)
        {
            throw new NotImplementedException();
        }

        public CompanyClass GetBasicInfo(Guid companyGuid, Guid siteGuid)
        {
            throw new NotImplementedException();
        }

        public List<Guid> GetCompanyGuidList(bool byGroupCompanies, bool localize)
        {
            throw new NotImplementedException();
        }

        public CompanyCollectionClass GetEntriesForFieldGeneratorByRole(COMPANY_ROLE role, Guid transContextCompanyGuid, Guid fuelCardGuid, bool hideHiddenCompanies = false)
        {
            throw new NotImplementedException();
        }

        public Guid GetIdentityGuid(string id)
        {
            throw new NotImplementedException();
        }

        public Guid GetMasterRecordGuid(string id)
        {
            throw new NotImplementedException();
        }

        public void Import(CompanyClass company)
        {
            throw new NotImplementedException();
        }

        public void Modify(DATA_TYPE type, CompanyClass company)
        {
            throw new NotImplementedException();
        }

        public void Purge(Guid companyGuid)
        {
            throw new NotImplementedException();
        }
    }
}
