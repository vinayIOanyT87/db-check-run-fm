namespace MirgrationToolProcessing.Mapping
{
    using BusinessObjects.MigrationObjects;
    using FMBusinessObjects.DataObjects;
    using MigrationToolDataAccessLayer;

    public abstract class UserDataFieldMappingBase
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public UserDataFieldMappingBase()
        {
            this.Init();
        }
        #endregion

        #region Properties
        public bool MessageFlag { get; set; }
        public string Message { get; set; }
        public SecurityHandler SecurityHndlr { get; set; }
        public string SourceSiteId { get; set; }
        public string TargetSiteId { get; set; }
        public ENTITY_TYPE UserDataEntityType { get; set; }
        #endregion

        #region Public methods
        public abstract void PerformMapping(UserDataFieldsBaseDo userDataFieldsBaseDo, MigrationDatabaseDAClass migrationDA);
        #endregion

        #region Protected methods
        /// <summary>
        /// This method sets the object to its initial state.
        /// </summary>
        protected void Init()
        {
            this.MessageFlag        = false;
            this.Message            = string.Empty;
            this.SecurityHndlr      = null;
            this.UserDataEntityType = ENTITY_TYPE.UNKNOWN;
        }
        #endregion
    }
}
