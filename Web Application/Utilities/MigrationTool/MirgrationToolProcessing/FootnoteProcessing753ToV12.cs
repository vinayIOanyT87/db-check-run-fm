namespace MirgrationToolProcessing
{
    using BusinessObjects.MigrationObjects;
    using MigrationToolBusinessObjects;
    using MigrationToolDataAccessLayer;
    using MirgrationToolProcessing.Mapping;

    public class FootnoteProcessing753ToV12 : FootnoteProcessingBase
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public FootnoteProcessing753ToV12(DbConfigurationDO dbConfigurationDo)
            : base(dbConfigurationDo)
        {
            base.migrationDA = new MigrationDatabaseDAClass { ConnectionString = base.DbConfigDo.SourceConnectionString };
        }
        #endregion

        #region Public methods
        /// <summary>
        /// This method starts the migration of the footnotes data.
        /// </summary>
        public override void MigrationProcess()
        {
            this.MigrateFootnotes();
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method migrates the footnotes information.
        /// </summary>
        private void MigrateFootnotes()
        {
            var footnoteDo = new Footnote753ToV12Do(base.SourceDbName, base.TargetDbName);
            var footnoteMapping = new FootnoteMapping753ToV12
            {
                SecurityHndlr = base.SecurityHndlr,
                SourceSiteId = base.SourceSiteId,
                TargetSiteId = base.TargetSiteId
            };

            footnoteMapping.PerformMapping(footnoteDo, base.migrationDA);

            if (footnoteMapping.MessageFlag)
            {
                base.MessageFlag = true;
                base.Message = base.Message + footnoteMapping.Message;
            }
        }
        #endregion
    }
}
