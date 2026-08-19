namespace MirgrationToolProcessing
{
    using BusinessObjects.MigrationObjects;
    using MigrationToolBusinessObjects;
    using MigrationToolDataAccessLayer;
    using MirgrationToolProcessing.Mapping;

    public class QualificationProcessing753ToV12 : QualificationProcessingBase
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public QualificationProcessing753ToV12(DbConfigurationDO dbConfigurationDo)
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
            this.MigrateQualifications();
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method migrates the qualification.
        /// </summary>
        private void MigrateQualifications()
        {
            var qualification = new Qualification753ToV12Do(base.SourceDbName, base.TargetDbName);
            var qualificationMapping = new QualificationMapping753ToV12
            {
                SecurityHndlr = base.SecurityHndlr
                , SourceSiteId = base.SourceSiteId
                , TargetSiteId = base.TargetSiteId
            };

            qualificationMapping.PerformMapping(qualification, base.migrationDA);

            if (qualificationMapping.MessageFlag)
            {
                base.MessageFlag = true;
                base.Message = base.Message + qualificationMapping.Message;
            }
        }
        #endregion
    }
}
