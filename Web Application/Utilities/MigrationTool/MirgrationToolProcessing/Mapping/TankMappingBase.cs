namespace MirgrationToolProcessing.Mapping
{
    using BusinessObjects.MigrationObjects;
    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using MigrationToolDataAccessLayer;
    using System;
    using System.Collections.Generic;

    public abstract class TankMappingBase
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public TankMappingBase()
        {
            this.Init();
        }
        #endregion

        #region Properties
        public bool MessageFlag { get; set; }
        public string Message { get; set; }
        public SecurityHandler SecurityHndlr { get; set; }
        public string SourceSiteId { get; set; }
        public string TargetSiteId { get; set; }

        public Dictionary<string, Guid> CompanyList { get; set; }
        public Dictionary<string, Guid> ProductList { get; set; }
        #endregion

        #region Public methods
        public abstract void PerformMapping(TankBaseDo tankBaseDo, MigrationDatabaseDAClass migrationDA, MigrationDatabaseDAClass migrationTargetDA);
        #endregion

        #region Protected methods
        /// <summary>
        /// This method will look for the company in the list first.  If not found,
        /// then it will call FM business servers to find the company. If found,
        /// it adds the company to the list for future reference.
        /// </summary>
        /// <param name="companyId">The company ID to search on.</param>
        /// <returns>Returns the company GUID or empty GUID if not found.</returns>
        protected Guid FindCompany(string companyId)
        {
            if (string.IsNullOrEmpty(companyId))
            {
                return Guid.Empty;
            }

            if (this.CompanyList.ContainsKey(companyId))
            {
                return this.CompanyList[companyId];
            }

            Guid companyGuid = Guid.Empty;

            try
            {
                companyGuid = FMChannelHelper.MakeCall<ICompanies, Guid>(x => x.GetIdentityGuid(this.SecurityHndlr.Security, companyId));
            }
            catch (Exception)
            {
                return companyGuid;
            }

            if (companyGuid == null || companyGuid == Guid.Empty)
            {
                return Guid.Empty;
            }

            this.CompanyList.Add(companyId, companyGuid);
            return companyGuid;
        }

        /// <summary>
        /// This method will look for the product in the list first.  If not found,
        /// then it will call FM business servers to find the product. If found,
        /// it adds the product to the list for future reference.
        /// </summary>
        /// <param name="productId">The product ID to search on.</param>
        /// <returns>Returns the product GUID or empty GUID if not found.</returns>
        protected Guid FindProduct(string productId)
        {
            if (string.IsNullOrEmpty(productId))
            {
                return Guid.Empty;
            }

            if (this.ProductList.ContainsKey(productId))
            {
                return this.ProductList[productId];
            }

            Guid productGuid = Guid.Empty;

            try
            {
                productGuid = FMChannelHelper.MakeCall<IProducts, Guid>(x => x.GetIdentityGuid(this.SecurityHndlr.Security, productId));
            }
            catch (Exception)
            {
                return productGuid;
            }

            if (productGuid == null || productGuid == Guid.Empty)
            {
                return Guid.Empty;
            }

            this.ProductList.Add(productId, productGuid);
            return productGuid;
        }

        /// <summary>
        /// This method sets the object to its initial state.
        /// </summary>
        protected void Init()
        {
            this.MessageFlag    = false;
            this.Message        = string.Empty;
            this.SecurityHndlr  = null;
            this.CompanyList    = new Dictionary<string, Guid>();
            this.ProductList    = new Dictionary<string, Guid>();
        }
        #endregion
    }
}
