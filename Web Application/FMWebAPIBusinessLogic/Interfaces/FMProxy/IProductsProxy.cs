using System;
using System.Data;
using FMBusinessObjects.DataObjects;

namespace FMWebAPIBusinessLogic.Interfaces.FMProxy
{
    public interface IProductsProxy
    {
        Guid Add(ProductClass product);
        ProductCollectionClass Enumerate(bool hideHiddenProducts = false);
        ProductCollectionClass EnumerateByFilter(string filter, bool hideHiddenProducts = false);
        ProductCollectionClass EnumerateByFilterAndLocalize(string filter, bool bLocalize, bool hideHiddenProducts = false);
        ProductCollectionClass EnumerateByManagerAndTanks(string managerID, bool hideHiddenProducts = false);
        ProductCollectionClass EnumerateBySite();
        ProductCollectionClass EnumerateByType(ProductType type, bool hideHiddenProducts = false);
        DataSet EnumerateByType1(ProductType Type);
        ProductCollectionClass EnumerateByTypeAndFilter(ProductType type, string filter, bool hideHiddenProducts = false);
        ProductCollectionClass EnumerateByTypeAndInhibitAccounting(ProductType type, bool inhibitAccounting);
        ProductCollectionClass EnumerateUndelegated();
        ProductClass Get(Guid productGuid, bool hideHiddenProducts = false);
        ProductClass GetBasicInfo(Guid productGuid, Guid siteGuid);
        ProductClass GetByCode(string code);
        ProductClass GetByID(string ID);
        ProductClass GetByInfoAuthorizedCompanies(Guid productGuid, bool getMinimalInfo, bool getAuthorizedCompanies, bool hideHiddenProducts = false);
        ProductClass GetByProductAuthorizedCompanies(Guid productGuid, bool getAuthorizedCompanies, bool hideHiddenProducts = false);
        Guid GetIdentityGuid(string id);
        Guid GetMasterRecordGuid(Guid productGuid);
        Guid GetMasterRecordGuidFromID(string id);
        void Import(ProductClass product);
        void Modify(ProductClass product);
        void Purge(Guid productGuid);
    }
}