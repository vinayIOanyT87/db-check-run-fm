namespace MirgrationToolProcessing.Mapping
{
    using BusinessObjects.MigrationObjects;
    using FMBusinessObjects.DataObjects;
    using MigrationToolDataAccessLayer;
    using System.Collections.Generic;

    public abstract class ProductMapMappingBase
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public ProductMapMappingBase()
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
        public abstract void RetrieveAllMapping(ProductMapBaseDo productMapBaseDo, MigrationDatabaseDAClass migrationDA);
        public abstract ProductMapCollectionClass GetProductMapCollection(LoadArm753ToV12Do sourceLoadArmDo
                                                                        , PRODUCT_MAP_TYPE productMapType
                                                                        , ProcessVariableMapping753ToV12 processVariableMap
                                                                        , List<UNIT_TYPE> unitTypeList
                                                                        , bool productMapProcessVariable);
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
