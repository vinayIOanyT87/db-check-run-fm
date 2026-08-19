namespace MirgrationToolProcessing.Mapping
{
    using BusinessObjects.MigrationObjects;
    using MigrationToolDataAccessLayer;
    using System.Collections.Generic;

    public abstract class SourceDbEquipTypesMappingBase
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public SourceDbEquipTypesMappingBase()
        {
            this.Init();
        }
        #endregion

        #region Properties
        public List<EquipmentTypeBaseDo> EquipmentTypesBaseList { get; set; }
        public bool MessageFlag { get; set; }
        public string Message { get; set; }
        #endregion

        #region Public methods
        public abstract void GetSourceEquipmentTypeMaps(MigrationDatabaseDAClass migrationDa, string sourceDbName);
        #endregion

        #region Protected methods
        /// <summary>
        /// This method will initialize the object to its initial state.
        /// </summary>
        protected void Init()
        {
            this.EquipmentTypesBaseList = new List<EquipmentTypeBaseDo>();
            this.Message = string.Empty;
            this.MessageFlag = false;
        }
        #endregion
    }
}
