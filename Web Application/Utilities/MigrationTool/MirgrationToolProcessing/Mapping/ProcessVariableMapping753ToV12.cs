namespace MirgrationToolProcessing.Mapping
{
    using BusinessObjects.MigrationObjects;
    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using MigrationToolDataAccessLayer;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Runtime.InteropServices;
    using Varec.CommonComponents.EngineeringUnitsLibrary;

    public class ProcessVariableMapping753ToV12 : ProcessVariableMappingBase
    {
        #region Data Member
        private MigrationDatabaseDAClass migrationDA;
        private MigrationDatabaseDAClass targetMigrationDA;
        private List<ProcessVariables753ToV12Do> sourceProcessVariableDoList;
        private List<ApplicationString753ToV12Do> targetProcessVariableAppStrList;
        private Guid targetSiteGuid;
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public ProcessVariableMapping753ToV12()
        {
            base.Init();
            this.HasSourceProcessVariables = false;
        }

        public ProcessVariableMapping753ToV12(List<OpcConnection753ToV12Do> targetOpcConnectionDoList)
        {
            this.TargetOpcConnectionDoList = targetOpcConnectionDoList;
            base.Init();
            this.HasSourceProcessVariables = false;
        }
        #endregion

        #region Properties
        public List<OpcConnection753ToV12Do> TargetOpcConnectionDoList { get; set; }
        public bool HasSourceProcessVariables { get; set; }
        #endregion

        #region Public methods
        /// <summary>
        /// This method implements the Perform Mapping for load arms.
        /// </summary>
        /// <param name="loadArmBaseDo">The load arm data object to map.</param>
        /// <param name="migrationDA">The Migration data access object.</param>
        public override void RetrieveAllMapping(ProcessVariablesBaseDo processVariableBaseDo, MigrationDatabaseDAClass migrationDA, MigrationDatabaseDAClass targetMigrationDA)
        {
            this.migrationDA = migrationDA;
            this.targetMigrationDA = targetMigrationDA;

            base.MessageFlag = false;
            base.Message = string.Empty;

            this.sourceProcessVariableDoList = new List<ProcessVariables753ToV12Do>();
            ProcessVariables753ToV12Do sourceProcessVariableDo = processVariableBaseDo as ProcessVariables753ToV12Do;
            if (string.IsNullOrEmpty(base.SourceSiteId) || string.IsNullOrEmpty(base.TargetSiteId))

            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Error: Source Site ID or Target Site ID is null.";
                return;
            }

            int? sourceSiteIndex = null;

            using (var command = new SqlCommand())
            {
                sourceProcessVariableDo.GetSiteIndexByIdSql(command, base.SourceSiteId);
                DataSet dataSet = this.migrationDA.GetDataSet(command);
                sourceSiteIndex = sourceProcessVariableDo.GetSiteIndex(dataSet);
            }

            if (sourceSiteIndex == null)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Error: Source Site Index is not found.";
                return;
            }

            // Note: for stations related items the target site is the same as the source site.
            this.targetSiteGuid = Guid.Empty;

            try
            {
                var targetSite = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.GetByID(base.SecurityHndlr.Security, base.SourceSiteId, true));
                this.targetSiteGuid = targetSite.SiteGuid;
            }
            catch (Exception ex)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Error: Retrieving target site GUID for ID '" + base.TargetSiteId + "'. " + ex.Message;
                return;
            }

            if (this.targetSiteGuid == Guid.Empty)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Error: Target Site GUID is not found.";
                return;
            }

            // Get all the process variable information
            DataSet sourceDataSet = null;
            this.sourceProcessVariableDoList = new List<ProcessVariables753ToV12Do>();

            using (var command = new SqlCommand())
            {
                sourceProcessVariableDo.EnumerateProcessVariableSql(command);
                sourceDataSet = this.migrationDA.GetDataSet(command);
            }

            if (sourceDataSet == null || sourceDataSet.Tables.Count == 0 || sourceDataSet.Tables[0].Rows.Count == 0)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Warning: No Process Variable found in the 7.5.3 '" + sourceProcessVariableDo.SourceDbName + "' database.";
            }
            else
            {
                foreach (DataRow row in sourceDataSet.Tables[0].Rows)
                {
                    var newSourceProcessVariableDo = new ProcessVariables753ToV12Do();
                    newSourceProcessVariableDo.Load(row);
                    this.sourceProcessVariableDoList.Add(newSourceProcessVariableDo);
                }
            }

            if(this.sourceProcessVariableDoList.Count > 0)
            {
                this.HasSourceProcessVariables = true;

                try
                {
                    DataSet targetDataSet = null;

                    // Get target process variable application strings.
                    using (var command = new SqlCommand())
                    {
                        var appStrDo = new ApplicationString753ToV12Do(sourceProcessVariableDo.SourceDbName, sourceProcessVariableDo.TargetDbName);
                        appStrDo.EnumerateTargetApplicationStringProcessVariableSql(command, targetSiteGuid);

                        targetDataSet = this.targetMigrationDA.GetDataSet(command);
                    }

                    if (targetDataSet == null || targetDataSet.Tables.Count == 0 || targetDataSet.Tables[0].Rows.Count == 0)
                    {
                        base.MessageFlag = true;
                        base.Message = base.Message + Environment.NewLine
                                            + "Warning: No Process Variable Application Strings found in target '" + sourceProcessVariableDo.TargetDbName + "' database.";
                    }
                    else
                    {
                        this.targetProcessVariableAppStrList = new List<ApplicationString753ToV12Do>();

                        foreach (DataRow row in targetDataSet.Tables[0].Rows)
                        {
                            var newTargetApplicationStringDo = new ApplicationString753ToV12Do();
                            newTargetApplicationStringDo.Load(row);
                            this.targetProcessVariableAppStrList.Add(newTargetApplicationStringDo);
                        }
                    }
                }
                catch(Exception ex)
                {
                    this.SecurityHndlr.Security.SiteGuid = this.SecurityHndlr.SiteAdminGuid;
                    base.MessageFlag = true;
                    base.Message = base.Message + Environment.NewLine
                                        + "Error: Retrieving Application Strings from target database '" + sourceProcessVariableDo.TargetDbName + "'. " + ex.Message;
                }
            }
        }

        /// <summary>
        /// This method will get the associated process variable collection based
        /// on the unit type (load arm index), process variable type, and unit type.
        /// </summary>
        /// <param name="unitIndex">The unit index used to search on.</param>
        /// <param name="processVariableType">The process variable type.</param>
        /// <param name="unitType">The unit type.</param>
        /// <returns>Return a collection of process variables.</returns>
        public override ProcessVariableCollectionClass GetTargetProcessVariables(int unitIndex
                                                                                , PROCESS_VARIABLE_TYPE processVariableType
                                                                                , List<UNIT_TYPE> unitTypeList)
        {
            var targetProcessVariableList = new ProcessVariableCollectionClass();

            if (this.sourceProcessVariableDoList == null || sourceProcessVariableDoList.Count == 0)
            {
                return targetProcessVariableList;
            }

            var foundProcessVariableList = new List<ProcessVariables753ToV12Do>();

            if (processVariableType == PROCESS_VARIABLE_TYPE.MAX_PV && unitTypeList[0] == UNIT_TYPE.MAX_UNIT)
            {
                foundProcessVariableList = this.sourceProcessVariableDoList.FindAll
                                                (x => x.UnitIndex == unitIndex);
            }
            else if (processVariableType != PROCESS_VARIABLE_TYPE.MAX_PV && unitTypeList[0] != UNIT_TYPE.MAX_UNIT)
            {
                foreach (UNIT_TYPE unitType in unitTypeList)
                {
                    var tempProcessVariableList = this.sourceProcessVariableDoList.FindAll
                                                (x => x.UnitIndex == unitIndex
                                                 && x.ProcessVariableType == (int)processVariableType
                                                 && x.UnitType == (int)unitType);

                    if (tempProcessVariableList != null && tempProcessVariableList.Count > 0)
                    {
                        foreach (ProcessVariables753ToV12Do processVariable in tempProcessVariableList)
                        {
                            foundProcessVariableList.Add(processVariable);
                        }
                    }
                }
            }
            else if (processVariableType != PROCESS_VARIABLE_TYPE.MAX_PV)
            {
                foundProcessVariableList = this.sourceProcessVariableDoList.FindAll
                                                (x => x.UnitIndex == unitIndex
                                                 && x.ProcessVariableType == (int)processVariableType);
            }
            else
            {
                foreach (UNIT_TYPE unitType in unitTypeList)
                {
                    var tempProcessVariableList = this.sourceProcessVariableDoList.FindAll(x => x.UnitIndex == unitIndex
                                                     && x.UnitType == (int)unitType);
                    
                    if(tempProcessVariableList != null && tempProcessVariableList.Count > 0)
                    {
                        foreach(ProcessVariables753ToV12Do processVariable in tempProcessVariableList)
                        {
                            foundProcessVariableList.Add(processVariable);
                        }
                    }
                }
            }

            if (foundProcessVariableList == null || foundProcessVariableList.Count == 0)
            {
                return targetProcessVariableList;
            }

            foreach (ProcessVariables753ToV12Do sourceProcessVariable in foundProcessVariableList)
            {
                var targetProcessVariable = new ProcessVariableClass
                {
                    ProcessVariableType             = (PROCESS_VARIABLE_TYPE)sourceProcessVariable.ProcessVariableType,
                    InstanceNumber                  = sourceProcessVariable.InstanceNumber,
                    UnitGuid                        = Guid.Empty,
                    OPCConnectionGuid               = this.GetOpcConnectionGuid(sourceProcessVariable.OpcProgID),
                    OPCItemID                       = sourceProcessVariable.OpcItemId,
                    DataType                        = (VarEnum)sourceProcessVariable.DataType,
                    UnitType                        = (UNIT_TYPE)sourceProcessVariable.UnitType,
                    ServerUnits                     = (EngineeringUnit)sourceProcessVariable.ServerEngineeringUnitsIndex,
                    OPCQuality                      = sourceProcessVariable.Quality == null ? (short)0 : (short)sourceProcessVariable.Quality,
                    SIValue                         = sourceProcessVariable.SIValue,
                    DateTimeStamp                   = sourceProcessVariable.DateTimeStamp == null ? DateTime.Now : (DateTime)sourceProcessVariable.DateTimeStamp,
                    siMaximum                       = sourceProcessVariable.Maximum,
                    siMinimum                       = sourceProcessVariable.Minimum,
                    DataTypeEnabled                 = sourceProcessVariable.DataTypeEnabled,
                    Input                           = sourceProcessVariable.Input,
                    InputEnabled                    = sourceProcessVariable.InputEnabled,
                    MessageID                       = sourceProcessVariable.ApplicationStringID,
                    MessageApplicationStringGuid    = this.GetApplicationStringGuidById(sourceProcessVariable.ApplicationStringID),
                    ProgID                          = sourceProcessVariable.OpcProgID,
                    URL                             = sourceProcessVariable.OpcUrl
                };
                
                // When Input (a.k.a Internal) is true, the process variable type is additive meter flow total,
                // and the source unit type is product map preset external component, then we need to reset
                // the unit type for V12 to be product map preset injector.
                if(targetProcessVariable.Input 
                    && targetProcessVariable.ProcessVariableType == PROCESS_VARIABLE_TYPE.ADDITIVE_METER_FLOW_TOTAL_PV
                    && sourceProcessVariable.UnitType == (int)UNIT_TYPE.PRODUCT_MAP_PRESET_EXTERNAL_COMPONENT)
                {
                    targetProcessVariable.UnitType = UNIT_TYPE.PRODUCT_MAP_PRESET_INJECTOR;
                }

                // When Input (a.k.a Internal) is true, the process variable type is component meter flow total,
                // and the source unit type (17) is product map preset external component, then we need to reset
                // the unit type for V12 to be product map offload external meter (24).
                if (targetProcessVariable.Input
                    && sourceProcessVariable.ProcessVariableType == (int)PROCESS_VARIABLE_TYPE.COMPONENT_METER_FLOW_TOTAL_PV
                    && sourceProcessVariable.UnitType == (int)UNIT_TYPE.PRODUCT_MAP_PRESET_EXTERNAL_COMPONENT)
                {
                    targetProcessVariable.UnitType = UNIT_TYPE.PRODUCT_MAP_OFFLOAD_EXTERNAL_METER;
                }

                targetProcessVariableList.Add(targetProcessVariable);
            }

            return targetProcessVariableList;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This is a helper function to get the target OPC Connection Guid based on
        /// the Prog ID.
        /// </summary>
        /// <param name="sourceOpcProgId">The source Prog ID to search on.</param>
        /// <returns>Returns the target OPC connection Guid or empty Guid if not found.</returns>
        private Guid GetOpcConnectionGuid(string sourceOpcProgId)
        {
            if (string.IsNullOrEmpty(sourceOpcProgId))
            {
                return Guid.Empty;
            }

            if (this.TargetOpcConnectionDoList == null || this.TargetOpcConnectionDoList.Count == 0)
            {
                return Guid.Empty;
            }

            OpcConnection753ToV12Do targetOpcConnectionDo = this.TargetOpcConnectionDoList.Find(x => x.ProgId == sourceOpcProgId);

            if (targetOpcConnectionDo == null)
            {
                return Guid.Empty;
            }

            return targetOpcConnectionDo.OpcConnectionGuid;
        }

        /// <summary>
        /// This method will get the application string Guid based on the application string ID.
        /// </summary>
        /// <param name="applicationStringMessageId">The application string ID to search on.</param>
        /// <returns>Return the application string Guid that matches the ID. If not found, returns an empty Guid.</returns>
        private Guid GetApplicationStringGuidById(string applicationStringMessageId)
        {
            if(string.IsNullOrEmpty(applicationStringMessageId) 
                || this.targetProcessVariableAppStrList == null 
                || this.targetProcessVariableAppStrList.Count == 0)
            {
                return Guid.Empty;
            }

            ApplicationString753ToV12Do targetApplicationString = this.targetProcessVariableAppStrList.Find(x => x.ID == applicationStringMessageId);

            if(targetApplicationString == null)
            {
                return Guid.Empty;
            }

            return targetApplicationString.ApplicationStringGuid;
        }
        #endregion
    }
}
