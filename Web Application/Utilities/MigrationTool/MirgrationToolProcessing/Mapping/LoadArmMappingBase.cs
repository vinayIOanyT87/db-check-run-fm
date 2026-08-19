namespace MirgrationToolProcessing.Mapping
{
    using BusinessObjects.MigrationObjects;
    using FMBusinessObjects.DataObjects;
    using MigrationToolDataAccessLayer;
    using System.Collections.Generic;

    public abstract class LoadArmMappingBase
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public LoadArmMappingBase()
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
        public abstract void RetrieveAllMapping(LoadArmBaseDo loadArmBaseDo, MigrationDatabaseDAClass migrationDA);
        public abstract LoadArmCollectionClass GetLoadArmCollection(Stations753ToV12Do sourceStationDo, ProcessVariableMapping753ToV12 processVariableMap, bool productMapProcessVariable);
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
