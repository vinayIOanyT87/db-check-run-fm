// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProducts.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Interface definition for products service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.ServiceModel;

    using FMBusinessObjects.DataObjects;

    /// <summary>
    /// Interface definition for products service class.
    /// </summary>
    [ServiceContract]
    public interface IProducts
    {
        /// <summary>
        /// Adds the specified product recrod.
        /// </summary>
        /// <param name="security">The securityParam.</param>
        /// <param name="product">The product.</param>
        /// <returns>The identity guid of the newly added product record.</returns>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        Guid Add(SecurityClass security, ProductClass product);

        /// <summary>
        /// Gets the specified product.
        /// </summary>
        /// <param name="security">The securityParam.</param>
        /// <param name="productGuid">The product GUID.</param>
        /// <param name="hideHiddenProducts">If true, only products that are not marked as hidden will be returned.</param>
        /// <returns>The specified product object.</returns>
        [OperationContract]
        ProductClass Get(SecurityClass security, Guid productGuid, bool hideHiddenProducts = false, bool LoadProcessVariables = true);

        [OperationContract]
        ProductClass GetMinimalProductData(SecurityClass security, Guid productGuid);

        [OperationContract]
        ProductClass GetByProductAuthorizedCompanies(SecurityClass security, Guid productGuid, bool getAuthorizedCompanies, bool hideHiddenProducts = false, bool LoadProcessVariables = true);

        [OperationContract]
        ProductClass GetByInfoAuthorizedCompanies(SecurityClass security, Guid productGuid, bool getMinimalInfo, bool getAuthorizedCompanies, bool hideHiddenProducts = false, bool LoadProcessVariables = true);

        [OperationContract]
        ProductClass GetByID(SecurityClass security, string ID);

        /// <summary>
        /// The get by code.
        /// </summary>
        /// <param name="security">
        /// The securityParam.
        /// </param>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="ProductClass"/>.
        /// </returns>
        [OperationContract]
        ProductClass GetByCode(SecurityClass security, string code);

        [OperationContract]
        ProductClass GetBasicInfo(SecurityClass security, Guid productGuid, Guid siteGuid);

        [OperationContract]
        Guid GetIdentityGuid(SecurityClass security, string id);

        [OperationContract]
        Guid GetMasterRecordGuidFromID(SecurityClass security, string id);

        [OperationContract]
        Guid GetMasterRecordGuid(SecurityClass security, Guid productGuid);

        [OperationContract]
        DataSet EnumerateByType1(SecurityClass security, ProductType Type);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void Modify(SecurityClass security, ProductClass product);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void Purge(SecurityClass security, Guid productGuid);

        [OperationContract]
        ProductCollectionClass EnumerateByType(SecurityClass security, ProductType type, bool hideHiddenProducts = false, int? limit = null);

        [OperationContract]
        ProductCollectionClass EnumerateByTypeAndFilter(SecurityClass security, ProductType type, string filter, bool hideHiddenProducts = false, int? limit = null);

        [OperationContract]
        ProductCollectionClass EnumerateByManagerAndTanks(SecurityClass security, string managerID, bool hideHiddenProducts = false);

        [OperationContract]
        ProductCollectionClass EnumerateByFilter(SecurityClass security, string filter, bool hideHiddenProducts = false);

        [OperationContract]
        ProductCollectionClass Enumerate(SecurityClass security, bool hideHiddenProducts = false, SiteClass site = null);

        [OperationContract]
        ProductCollectionClass EnumerateByFilterAndLocalize(SecurityClass security, string filter, bool bLocalize, bool hideHiddenProducts = false, SiteClass site = null);

        [OperationContract]
        ProductCollectionClass EnumerateByTypeAndInhibitAccounting(SecurityClass security, ProductType type, bool inhibitAccounting);

        [OperationContract]
        ProductCollectionClass EnumerateBySite(SecurityClass security);

        [OperationContract]
        ProductCollectionClass EnumerateUndelegated(SecurityClass security);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void Import(SecurityClass security, ProductClass product);

        [OperationContract]
        List<string> EnumerateIdBySite(SecurityClass security);

        [OperationContract]
        DataSet EnumerateProductsAtAllSites(SecurityClass security);

    }
}
