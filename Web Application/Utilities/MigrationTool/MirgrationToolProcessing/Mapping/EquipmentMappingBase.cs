namespace MirgrationToolProcessing.Mapping
{
    using BusinessObjects.MigrationObjects;
    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using MigrationToolDataAccessLayer;
    using System;
    using System.Collections.Generic;

    public abstract class EquipmentMappingBase
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public EquipmentMappingBase()
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
        public Dictionary<string, Guid> FuelCardList { get; set; }
        #endregion

        #region Public methods
        public abstract void PerformMapping(EquipmentBaseDo equipmentDo, MigrationDatabaseDAClass migrationDA);
        #endregion

        #region Protected methods
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
            this.FuelCardList   = new Dictionary<string, Guid>();
        }

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
            catch(Exception)
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
            catch(Exception)
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
        /// This method will look for the fuel card in the list first.  If not found,
        /// then it will call FM business servers to find the fuel card. If found,
        /// it adds the fuel card to the list for future reference.
        /// </summary>
        /// <param name="fuelCardId">The fueld card ID to search on.</param>
        /// <returns>Returns the fuel card GUID or empty GUID if not found.</returns>
        protected Guid FindFuelCard(string fuelCardId)
        {
            if (string.IsNullOrEmpty(fuelCardId))
            {
                return Guid.Empty;
            }

            if (this.FuelCardList.ContainsKey(fuelCardId))
            {
                return this.FuelCardList[fuelCardId];
            }

            Guid fuelCardGuid = Guid.Empty;

            try
            {
                fuelCardGuid = FMChannelHelper.MakeCall<IFuelCards, Guid>(x => x.GetIdentityGuid(this.SecurityHndlr.Security, fuelCardId));
            }
            catch(Exception)
            {
                return fuelCardGuid;
            }

            if (fuelCardGuid == null || fuelCardGuid == Guid.Empty)
            {
                return Guid.Empty;
            }

            this.FuelCardList.Add(fuelCardId, fuelCardGuid);
            return fuelCardGuid;
        }

        /// <summary>
        /// This method is a helper to perform entity assignment.
        /// </summary>
        /// <param name="entityAssignmentProcessor">The entity processor object.</param>
        /// <param name="entityGuid">The entity guid to perform the assignment.</param>
        /// <param name="sourceEntitySiteGuid">The source entity site guid.</param>
        /// <param name="targetEntitySiteGuid">The target entity site guid.</param>
        /// <param name="interfaceTypeGuid">The Interface type guid.</param>
        /// <param name="entityType">The entity type.</param>
        /// <param name="entityMessage">The message.</param>
        protected void PerformEntityAssignmentHelper(EntityService.EntityAssignmentProcessor entityAssignmentProcessor
                                            , Guid entityGuid
                                            , Guid sourceEntitySiteGuid
                                            , Guid targetEntitySiteGuid
                                            , Guid interfaceTypeGuid
                                            , ENTITY_TYPE entityType
                                            , string entityMessage)
        {
            entityAssignmentProcessor.MessageFlag = false;
            entityAssignmentProcessor.Message = string.Empty;

            entityAssignmentProcessor.PerformEntityAssignment(entityGuid
                                                            , sourceEntitySiteGuid
                                                            , targetEntitySiteGuid
                                                            , interfaceTypeGuid
                                                            , entityType);

            if (entityAssignmentProcessor.MessageFlag && string.IsNullOrEmpty(entityMessage) == false)
            {
                this.Message = this.Message + Environment.NewLine + entityAssignmentProcessor.Message + entityMessage;
            }
        }
        #endregion
    }
}
