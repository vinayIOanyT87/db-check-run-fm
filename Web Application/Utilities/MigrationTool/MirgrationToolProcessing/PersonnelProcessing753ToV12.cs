namespace MirgrationToolProcessing
{
    using BusinessObjects.MigrationObjects;
    using MigrationToolBusinessObjects;
    using MigrationToolDataAccessLayer;
    using MirgrationToolProcessing.Mapping;

    public class PersonnelProcessing753ToV12 : PersonnelProcessingBase
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public PersonnelProcessing753ToV12(DbConfigurationDO dbConfigurationDo) 
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
            this.MigratePersonnel();         
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method migrates the personnel information.
        /// </summary>
        private void MigratePersonnel()
        {
            var personnel = new Personnel753ToV12Do(base.SourceDbName, base.TargetDbName);
            var personnelMapping = new PersonnelMapping753ToV12
            {
                SecurityHndlr = base.SecurityHndlr
                , SourceSiteId = base.SourceSiteId
                , TargetSiteId = base.TargetSiteId
            };

            personnelMapping.PerformMapping(personnel, base.migrationDA);

            if (personnelMapping.MessageFlag)
            {
                base.MessageFlag = true;
                base.Message = base.Message + personnelMapping.Message;
            }
        }
        #endregion
    }
}
