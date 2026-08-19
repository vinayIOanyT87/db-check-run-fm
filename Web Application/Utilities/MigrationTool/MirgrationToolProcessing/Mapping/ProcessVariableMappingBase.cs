namespace MirgrationToolProcessing.Mapping
{
    using BusinessObjects.MigrationObjects;
    using FMBusinessObjects.DataObjects;
    using MigrationToolDataAccessLayer;
    using System.Collections.Generic;

    public abstract class ProcessVariableMappingBase
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public ProcessVariableMappingBase()
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
        public abstract void RetrieveAllMapping(ProcessVariablesBaseDo processVariableBaseDo, MigrationDatabaseDAClass migrationDA, MigrationDatabaseDAClass targetMigrationDA);
        public abstract ProcessVariableCollectionClass GetTargetProcessVariables(int unitIndex
                                                                            , PROCESS_VARIABLE_TYPE processVariableType
                                                                            , List<UNIT_TYPE> unitTypeList);
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
