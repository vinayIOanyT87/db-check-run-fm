namespace MirgrationToolProcessing
{
    using FMBusinessObjects.DataObjects;
    using MigrationToolBusinessObjects;
    using MigrationToolDataAccessLayer;

    public abstract class UserDataFieldProcessingBase
    {
        #region Data members
        protected MigrationDatabaseDAClass migrationDA;
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public UserDataFieldProcessingBase(DbConfigurationDO dbConfigurationDo)
        {
            this.Init();

            this.SourceDbName = dbConfigurationDo.SourceDbConnectionDbName;
            this.TargetDbName = dbConfigurationDo.TargetDbConnectionDbName;
            this.DbConfigDo = dbConfigurationDo;
        }
        #endregion

        #region Properties
        public string SourceDbName { get; private set; }
        public string TargetDbName { get; private set; }
        public DbConfigurationDO DbConfigDo { get; private set; }
        public SecurityHandler SecurityHndlr { get; private set; }
        public string SourceSiteId { get; set; }
        public string TargetSiteId { get; set; }
        public bool MessageFlag { get; set; }
        public string Message { get; set; }
        public ENTITY_TYPE UserDataEntityType { get; set; }
        #endregion

        #region public methods
        public abstract void MigrationProcess();
        #endregion

        #region Protected methods
        /// <summary>
        /// This method initializes the object to its initial state.
        /// </summary>
        protected void Init()
        {
            this.SourceDbName       = string.Empty;
            this.TargetDbName       = string.Empty;
            this.DbConfigDo         = null;
            this.SecurityHndlr      = new SecurityHandler();
            this.MessageFlag        = false;
            this.Message            = string.Empty;
            this.SourceSiteId       = string.Empty;
            this.TargetSiteId       = string.Empty;
            this.UserDataEntityType = ENTITY_TYPE.UNKNOWN;
        }
        #endregion
    }
}
