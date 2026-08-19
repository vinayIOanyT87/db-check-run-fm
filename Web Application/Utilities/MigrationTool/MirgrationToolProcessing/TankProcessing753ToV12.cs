namespace MirgrationToolProcessing
{
    using BusinessObjects.MigrationObjects;
    using MigrationToolBusinessObjects;
    using MigrationToolDataAccessLayer;
    using MirgrationToolProcessing.Mapping;

    public class TankProcessing753ToV12 : TankProcessingBase
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public TankProcessing753ToV12(DbConfigurationDO dbConfigurationDo)
            : base(dbConfigurationDo)
        {
            base.migrationDA = new MigrationDatabaseDAClass { ConnectionString = base.DbConfigDo.SourceConnectionString };
            base.migrationTargetDA = new MigrationDatabaseDAClass { ConnectionString = base.DbConfigDo.TargetConnectionString };
        }
        #endregion

        #region Public methods
        /// <summary>
        /// This method starts the migration of the tank data which includes
        /// process variables data also.
        /// </summary>
        public override void MigrationProcess()
        {
            this.MigrateTanks();
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method migrates the Tanks.
        /// </summary>
        private void MigrateTanks()
        {
            var sourceTankDo = new Tank753ToV12Do(base.SourceDbName, base.TargetDbName);
            var tankMapping = new TankMapping753ToV12
            {
                SecurityHndlr = base.SecurityHndlr,
                SourceSiteId = base.SourceSiteId,
                TargetSiteId = base.TargetSiteId
            };

            tankMapping.PerformMapping(sourceTankDo, base.migrationDA, base.migrationTargetDA);

            if (tankMapping.MessageFlag)
            {
                base.MessageFlag = true;
                base.Message = base.Message + tankMapping.Message;
            }
        }
        #endregion
    }
}
