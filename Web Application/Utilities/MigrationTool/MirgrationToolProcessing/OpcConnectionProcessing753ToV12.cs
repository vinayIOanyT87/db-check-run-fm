namespace MirgrationToolProcessing
{
    using BusinessObjects.MigrationObjects;
    using MigrationToolBusinessObjects;
    using MigrationToolDataAccessLayer;
    using MirgrationToolProcessing.Mapping;

    public class OpcConnectionProcessing753ToV12 : OpcConnectionProcessingBase
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public OpcConnectionProcessing753ToV12(DbConfigurationDO dbConfigurationDo)
            : base(dbConfigurationDo)
        {
            base.migrationDA = new MigrationDatabaseDAClass { ConnectionString = base.DbConfigDo.SourceConnectionString };
        }
        #endregion

        #region Public methods
        /// <summary>
        /// This method starts the migration of the OPC connection data.
        /// </summary>
        public override void MigrationProcess()
        {
            this.MigrateOpcConnections();
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method migrates the OPC connection information.
        /// </summary>
        private void MigrateOpcConnections()
        {
            var opcConnection = new OpcConnection753ToV12Do(base.SourceDbName, base.TargetDbName);
            var opcConnectionMapping = new OpcConnectionMapping753ToV12
            {
                SecurityHndlr = base.SecurityHndlr,
                SourceSiteId = base.SourceSiteId,
                TargetSiteId = base.TargetSiteId
            };

            opcConnectionMapping.PerformMapping(opcConnection, base.migrationDA);

            if (opcConnectionMapping.MessageFlag)
            {
                base.MessageFlag = true;
                base.Message = base.Message + opcConnectionMapping.Message;
            }
        }
        #endregion
    }
}
