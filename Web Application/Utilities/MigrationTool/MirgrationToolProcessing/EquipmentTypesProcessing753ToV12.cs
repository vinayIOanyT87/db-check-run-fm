namespace MirgrationToolProcessing
{
    using BusinessObjects.MigrationObjects;
    using MigrationToolBusinessObjects;
    using MigrationToolDataAccessLayer;
    using MirgrationToolProcessing.Mapping;

    public class EquipmentTypesProcessing753ToV12 : EquipmentTypesProcessingBase
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public EquipmentTypesProcessing753ToV12(DbConfigurationDO dbConfigurationDo)
            : base(dbConfigurationDo)
        {
            base.migrationDA = new MigrationDatabaseDAClass { ConnectionString = base.DbConfigDo.SourceConnectionString };
        }
        #endregion

        #region Public methods
        /// <summary>
        /// This method starts the migration of the personnel data which includes
        /// qualification data also.
        /// </summary>
        public override void MigrationProcess()
        {
            this.MigrateEquipmentTypes();
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method migrates the personnel information.
        /// </summary>
        private void MigrateEquipmentTypes()
        {
            var equipmentTypeDo = new EquipmentType753ToV12Do(base.SourceDbName, base.TargetDbName);
            var equipmentTypesMapping = new EquipmentTypesMapping753ToV12
            {
                SecurityHndlr = base.SecurityHndlr
                , SourceSiteId = base.SourceSiteId
                , TargetSiteId = base.TargetSiteId
            };

            equipmentTypesMapping.PerformMapping(equipmentTypeDo, base.migrationDA);

            if (equipmentTypesMapping.MessageFlag)
            {
                base.MessageFlag = true;
                base.Message = base.Message + equipmentTypesMapping.Message;
            }
        }
        #endregion
    }
}
