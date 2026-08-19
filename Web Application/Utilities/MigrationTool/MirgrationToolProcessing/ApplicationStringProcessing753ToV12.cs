namespace MirgrationToolProcessing
{
    using BusinessObjects.MigrationObjects;
    using MigrationToolBusinessObjects;
    using MigrationToolDataAccessLayer;
    using MirgrationToolProcessing.Mapping;

    public class ApplicationStringProcessing753ToV12 : ApplicationStringProcessingBase
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public ApplicationStringProcessing753ToV12(DbConfigurationDO dbConfigurationDo)
            : base(dbConfigurationDo)
        {
            base.migrationDA = new MigrationDatabaseDAClass { ConnectionString = base.DbConfigDo.SourceConnectionString };
        }
        #endregion

        #region Public methods
        /// <summary>
        /// This method starts the migration of the application string data.
        /// </summary>
        public override void MigrationProcess()
        {
            this.MigrateApplicationString();
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method migrates the application string information.
        /// </summary>
        private void MigrateApplicationString()
        {
            var applicationStringDo = new ApplicationString753ToV12Do(base.SourceDbName, base.TargetDbName);
            var applicationStringMapping = new ApplicationStringMapping753ToV12
            {
                SecurityHndlr = base.SecurityHndlr,
                SourceSiteId = base.SourceSiteId,
                TargetSiteId = base.TargetSiteId
            };

            applicationStringMapping.PerformMapping(applicationStringDo, base.migrationDA);

            if (applicationStringMapping.MessageFlag)
            {
                base.MessageFlag = true;
                base.Message = base.Message + applicationStringMapping.Message;
            }
        }
        #endregion
    }
}
