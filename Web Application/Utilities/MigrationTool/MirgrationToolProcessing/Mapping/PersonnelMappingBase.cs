namespace MirgrationToolProcessing.Mapping
{
    using BusinessObjects.DataObjects;
    using BusinessObjects.MigrationObjects;
    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using MigrationToolDataAccessLayer;
    using System;
    using System.Collections.Generic;

    public abstract class PersonnelMappingBase
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public PersonnelMappingBase()
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
        public List<EquipmentClass> EquipmentList { get; set; }
        public List<PersonnelSupervisorDo> SupervisorList { get; set; }
        public List<CompanyClass> CompanyList { get; set; }
        #endregion

        #region Public methods
        public abstract void PerformMapping(PersonnelBaseDo personnelDo, MigrationDatabaseDAClass migrationDA);
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
            this.EquipmentList  = new List<EquipmentClass>();
            this.SupervisorList = new List<PersonnelSupervisorDo>();
            this.CompanyList    = new List<CompanyClass>();
        }

        /// <summary>
        /// This method will get the equipment for a given ID and site. It
        /// will add the equipment if found to the equipment list.
        /// </summary>
        /// <param name="equipmentId">The equipment ID to retrieve.</param>
        /// <param name="siteGuid">The site Guid being processed.</param>
        protected void UpdateEquipmentList(string equipmentId, Guid siteGuid)
        {
            if (string.IsNullOrEmpty(equipmentId))
            {
                return;
            }

            EquipmentClass foundEquipment = this.EquipmentList.Find(x => x.ID.ToUpper() == equipmentId.ToUpper());

            // No need to add it to the list if it already exists.
            if (foundEquipment != null)
            {
                return;
            }

            var equipmentGuid = FMChannelHelper.MakeCall<IEquipments, Guid>(x => x.GetIdentityGuid(this.SecurityHndlr.Security, equipmentId));

            if (equipmentGuid == null || equipmentGuid == Guid.Empty)
            {
                return;
            }

            var equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(x => x.GetBasicInfo(this.SecurityHndlr.Security, equipmentGuid, siteGuid));

            if (equipment != null)
            {
                this.EquipmentList.Add(equipment);
            }
        }

        /// <summary>
        /// This method will get the target company for a given ID and site. It
        /// will add the company if found to the company list.
        /// </summary>
        /// <param name="equipmentId">The equipment ID to retrieve.</param>
        /// <param name="siteGuid">The site Guid being processed.</param>
        protected void UpdateCompanyList(string companyId, Guid siteGuid)
        {
            if (string.IsNullOrEmpty(companyId))
            {
                return;
            }

            CompanyClass foundCompany = this.CompanyList.Find(x => x.ID.ToUpper() == companyId.ToUpper());

            // No need to add it to the list if it already exists.
            if(foundCompany != null)
            {
                return;
            }

            var companyGuid = FMChannelHelper.MakeCall<ICompanies, Guid>(x => x.GetIdentityGuid(this.SecurityHndlr.Security, companyId));

            if (companyGuid == null || companyGuid == Guid.Empty)
            {
                return;
            }

            var company = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(x => x.GetBasicInfo(this.SecurityHndlr.Security, companyGuid, siteGuid));

            if (company != null)
            {
                this.CompanyList.Add(company);
            }
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
