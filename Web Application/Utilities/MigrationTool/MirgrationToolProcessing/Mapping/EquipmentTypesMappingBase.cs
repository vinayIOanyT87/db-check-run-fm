namespace MirgrationToolProcessing.Mapping
{
    using BusinessObjects.MigrationObjects;
    using FMBusinessObjects.DataObjects;
    using MigrationToolDataAccessLayer;
    using System;

    public abstract class EquipmentTypesMappingBase
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public EquipmentTypesMappingBase()
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
        #endregion

        #region Public methods
        public abstract void PerformMapping(EquipmentTypeBaseDo equipmentTypeDo, MigrationDatabaseDAClass migrationDA);
        #endregion

        #region Protected methods
        /// <summary>
        /// This method sets the object to its initial state.
        /// </summary>
        protected void Init()
        {
            this.MessageFlag = false;
            this.Message = string.Empty;
            this.SecurityHndlr = null;
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
