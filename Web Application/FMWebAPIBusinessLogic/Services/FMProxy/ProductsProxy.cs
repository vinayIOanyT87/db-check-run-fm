using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using FMCore.Interfaces;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic;
using FMWebAPIBusinessLogic.Interfaces.FMProxy;
using System;
using System.Data;
using System.Diagnostics;

namespace FMWebAPIBusinessLogic.Services.FMProxy
{
    public class ProductsProxy : IProductsProxy
    {
        ICurrentRequestContext _requestContext;
        IFMCustomLogger _logger;

        public ProductsProxy(ICurrentRequestContext requestContext,
            IFMCustomLogger logger)
        {
            this._requestContext = requestContext;
            this._logger = logger;
        }

        public Guid Add(ProductClass product)
        {
            throw new NotImplementedException();
        }

        public ProductCollectionClass Enumerate(bool hideHiddenProducts = false)
        {
            try
            {
                var timer = Stopwatch.StartNew();
                var currentSecurity = this._requestContext.GetCurrentSecurityContext();
                var result = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(
                    service => service.Enumerate(currentSecurity, hideHiddenProducts));
                timer.Stop();
                _logger.Debug($"Took {timer.ElapsedMilliseconds}ms");
                return result;
            }
            catch (Exception e)
            {
                _logger.Error(e, "something failed");
                throw;
            }
        }

        public ProductCollectionClass EnumerateByFilter(string filter, bool hideHiddenProducts = false)
        {
            throw new NotImplementedException();
        }

        public ProductCollectionClass EnumerateByFilterAndLocalize(string filter, bool bLocalize, bool hideHiddenProducts = false)
        {
            throw new NotImplementedException();
        }

        public ProductCollectionClass EnumerateByManagerAndTanks(string managerID, bool hideHiddenProducts = false)
        {
            throw new NotImplementedException();
        }

        public ProductCollectionClass EnumerateBySite()
        {
            throw new NotImplementedException();
        }

        public ProductCollectionClass EnumerateByType(ProductType type, bool hideHiddenProducts = false)
        {
            throw new NotImplementedException();
        }

        public DataSet EnumerateByType1(ProductType Type)
        {
            throw new NotImplementedException();
        }

        public ProductCollectionClass EnumerateByTypeAndFilter(ProductType type, string filter, bool hideHiddenProducts = false)
        {
            throw new NotImplementedException();
        }

        public ProductCollectionClass EnumerateByTypeAndInhibitAccounting(ProductType type, bool inhibitAccounting)
        {
            throw new NotImplementedException();
        }

        public ProductCollectionClass EnumerateUndelegated()
        {
            throw new NotImplementedException();
        }

        public ProductClass Get(Guid productGuid, bool hideHiddenProducts = false)
        {
            throw new NotImplementedException();
        }

        public ProductClass GetBasicInfo(Guid productGuid, Guid siteGuid)
        {
            throw new NotImplementedException();
        }

        public ProductClass GetByCode(string code)
        {
            throw new NotImplementedException();
        }

        public ProductClass GetByID(string ID)
        {
            throw new NotImplementedException();
        }

        public ProductClass GetByInfoAuthorizedCompanies(Guid productGuid, bool getMinimalInfo, bool getAuthorizedCompanies, bool hideHiddenProducts = false)
        {
            throw new NotImplementedException();
        }

        public ProductClass GetByProductAuthorizedCompanies(Guid productGuid, bool getAuthorizedCompanies, bool hideHiddenProducts = false)
        {
            throw new NotImplementedException();
        }

        public Guid GetIdentityGuid(string id)
        {
            throw new NotImplementedException();
        }

        public Guid GetMasterRecordGuid(Guid productGuid)
        {
            throw new NotImplementedException();
        }

        public Guid GetMasterRecordGuidFromID(string id)
        {
            throw new NotImplementedException();
        }

        public void Import(ProductClass product)
        {
            throw new NotImplementedException();
        }

        public void Modify(ProductClass product)
        {
            throw new NotImplementedException();
        }

        public void Purge(Guid productGuid)
        {
            throw new NotImplementedException();
        }
    }
}
