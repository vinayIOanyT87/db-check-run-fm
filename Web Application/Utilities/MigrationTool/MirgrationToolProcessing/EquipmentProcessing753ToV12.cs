namespace MirgrationToolProcessing
{
    using BusinessObjects.MigrationObjects;
    using MigrationToolBusinessObjects;
    using MigrationToolDataAccessLayer;
    using MirgrationToolProcessing.Mapping;

    public class EquipmentProcessing753ToV12 : EquipmentProcessingBase
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public EquipmentProcessing753ToV12(DbConfigurationDO dbConfigurationDo)
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
            this.MigrateEquipment();
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method migrates the personnel information.
        /// </summary>
        private void MigrateEquipment()
        {
            var equipment = new Equipment753ToV12Do(base.SourceDbName, base.TargetDbName);
            var equipmentMapping = new EquipmentMapping753ToV12
            {
                SecurityHndlr = base.SecurityHndlr
                , SourceSiteId = base.SourceSiteId
                , TargetSiteId = base.TargetSiteId
            };

            equipmentMapping.PerformMapping(equipment, base.migrationDA);

            if (equipmentMapping.MessageFlag)
            {
                base.MessageFlag = true;
                base.Message = base.Message + equipmentMapping.Message;
            }
        }
        #endregion
    }
}
