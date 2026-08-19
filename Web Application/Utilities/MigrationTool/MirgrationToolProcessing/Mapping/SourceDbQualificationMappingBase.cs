namespace MirgrationToolProcessing.Mapping
{
    using BusinessObjects.MigrationObjects;
    using MigrationToolDataAccessLayer;
    using System.Collections.Generic;

    public abstract class SourceDbQualificationMappingBase
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public SourceDbQualificationMappingBase()
        {
            this.Init();
        }
        #endregion

        #region Properties
        public List<QualificationMapsBaseDo> QualificationMapsBaseList { get; set; }
        public bool MessageFlag { get; set; }
        public string Message { get; set; }
        #endregion

        #region Public methods
        public abstract void GetSourceQualificationMaps(MigrationDatabaseDAClass migrationDa);
        #endregion

        #region Protected methods
        /// <summary>
        /// This method will initialize the object to its initial state.
        /// </summary>
        protected void Init()
        {
            this.QualificationMapsBaseList = new List<QualificationMapsBaseDo>();
            this.Message = string.Empty;
            this.MessageFlag = false;
        }
        #endregion
    }
}
