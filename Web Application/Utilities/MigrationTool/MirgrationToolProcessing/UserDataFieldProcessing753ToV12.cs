namespace MirgrationToolProcessing
{
    using BusinessObjects.MigrationObjects;
    using MigrationToolBusinessObjects;
    using MigrationToolDataAccessLayer;
    using MirgrationToolProcessing.Mapping;

    public class UserDataFieldProcessing753ToV12 : UserDataFieldProcessingBase
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public UserDataFieldProcessing753ToV12(DbConfigurationDO dbConfigurationDo) : base(dbConfigurationDo)
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
            this.MigrateUserDataFields();
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method migrates the qualification.
        /// </summary>
        private void MigrateUserDataFields()
        {
            var userDataFieldsDo = new UserDataFields753ToV12Do(base.SourceDbName, base.TargetDbName);
            var userDataFieldMapping = new UserDataFieldMapping753ToV12
            {
                SecurityHndlr = base.SecurityHndlr
                , SourceSiteId = base.SourceSiteId
                , TargetSiteId = base.TargetSiteId
                , UserDataEntityType = base.UserDataEntityType
            };

            userDataFieldMapping.PerformMapping(userDataFieldsDo, base.migrationDA);

            if (userDataFieldMapping.MessageFlag)
            {
                base.MessageFlag = true;
                base.Message = base.Message + userDataFieldMapping.Message;
            }
        }
        #endregion
    }
}
