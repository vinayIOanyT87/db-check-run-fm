namespace MirgrationToolProcessing.Mapping
{
    using BusinessObjects.MigrationObjects;
    using MigrationToolDataAccessLayer;

    public abstract class QualificationMappingBase
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public QualificationMappingBase()
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
        #endregion

        #region Public methods
        public abstract void PerformMapping(QualificationBaseDo qualificationDo, MigrationDatabaseDAClass migrationDA);
        #endregion

        #region Protected methods
        /// <summary>
        /// This method sets the object to its initial state.
        /// </summary>
        protected void Init()
        {
            this.MessageFlag = false;
            this.Message = string.Empty;
            this.SecurityHndlr = null;
        }
        #endregion
    }
}
