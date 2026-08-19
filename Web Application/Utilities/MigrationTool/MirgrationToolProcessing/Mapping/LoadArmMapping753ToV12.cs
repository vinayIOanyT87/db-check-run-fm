namespace MirgrationToolProcessing.Mapping
{
    using BusinessObjects.MigrationObjects;
    using FMBusinessObjects.DataObjects;
    using MigrationToolDataAccessLayer;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Runtime.InteropServices;
    using Varec.CommonComponents.EngineeringUnitsLibrary;

    public class LoadArmMapping753ToV12 : LoadArmMappingBase
    {
        #region Data Member
        private MigrationDatabaseDAClass migrationDA;
        private List<LoadArm753ToV12Do> sourceLoadArmDoList;
        private ProductMapMapping753ToV12 productMapMapping;
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public LoadArmMapping753ToV12()
        {
            base.Init();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// This method implements the Perform Mapping for load arms.
        /// </summary>
        /// <param name="loadArmBaseDo">The load arm data object to map.</param>
        /// <param name="migrationDA">The Migration data access object.</param>
        public override void RetrieveAllMapping(LoadArmBaseDo loadArmBaseDo, MigrationDatabaseDAClass migrationDA)
        {
            this.migrationDA = migrationDA;

            base.MessageFlag = false;
            base.Message = string.Empty;

            this.sourceLoadArmDoList = new List<LoadArm753ToV12Do>();
            LoadArm753ToV12Do sourceLoadArmDo = loadArmBaseDo as LoadArm753ToV12Do;
            DataSet sourceDataSet = null;

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
                sourceLoadArmDo.GetSiteIndexByIdSql(command, base.SourceSiteId);
                DataSet dataSet = this.migrationDA.GetDataSet(command);
                sourceSiteIndex = sourceLoadArmDo.GetSiteIndex(dataSet);
            }

            if (sourceSiteIndex == null)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Error: Source Site Index is not found.";
                return;
            }

            // Get source Load Arms
            using (var command = new SqlCommand())
            {
                sourceLoadArmDo.EnumerateSourceLoadArmsSql(command);
                sourceDataSet = this.migrationDA.GetDataSet(command);
            }

            if (sourceDataSet == null || sourceDataSet.Tables.Count == 0 || sourceDataSet.Tables[0].Rows.Count == 0)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Warning: No Load Arms found in the 7.5.3 " + sourceLoadArmDo.SourceDbName + " database.";
                return;
            }

            foreach (DataRow row in sourceDataSet.Tables[0].Rows)
            {
                var newLoadArmDo = new LoadArm753ToV12Do();
                newLoadArmDo.Load(row);
                this.sourceLoadArmDoList.Add(newLoadArmDo);
            }

            if (sourceLoadArmDoList.Count == 0)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Warning: No Load Arms found in the 7.5.3 " + sourceLoadArmDo.SourceDbName + " database.";
                return;
            }

            // Get product map for load arms
            var sourceProductMapDo = new ProductMap753ToV12Do(loadArmBaseDo.SourceDbName, loadArmBaseDo.TargetDbName);
            this.productMapMapping = new ProductMapMapping753ToV12
            {
                SourceSiteId = base.SourceSiteId,
                TargetSiteId = base.TargetSiteId,
                SecurityHndlr = base.SecurityHndlr
            };

            this.productMapMapping.RetrieveAllMapping(sourceProductMapDo, this.migrationDA);

            if(productMapMapping.MessageFlag)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine + productMapMapping.Message;
            }
        }

        /// <summary>
        /// This method will get a collection of load arms for a given source station index.
        /// It will return a target load arm collection.
        /// </summary>
        /// <param name="stationIndex">The station index to get the load arms.</param>
        /// <returns>Returns a target load arm collection.</returns>
        public override LoadArmCollectionClass GetLoadArmCollection(Stations753ToV12Do sourceStationDo, ProcessVariableMapping753ToV12 processVariableMap, bool productMapProcessVariable)
        {
            var targetLoadArmList = new LoadArmCollectionClass();

            if(this.sourceLoadArmDoList.Count > 0)
            {
                List<LoadArm753ToV12Do> foundLoadArmList = this.sourceLoadArmDoList.FindAll(x => x.BayAStationIndex == sourceStationDo.Index || x.BayBStationIndex == sourceStationDo.Index);

                if(foundLoadArmList == null || foundLoadArmList.Count == 0)
                {
                    base.MessageFlag = true;
                    base.Message = base.Message + Environment.NewLine + "Warning: No load arms found for station with ID of " + sourceStationDo.Id;
                    return targetLoadArmList;
                }

                int insertCount = 0;

                foreach (LoadArm753ToV12Do sourceLoadArmDo in foundLoadArmList)
                {
                    var targetLoadArmDo = new LoadArmClass
                    {
                        LoadRackText    = sourceLoadArmDo.LoadRackText,
                        Enabled         = sourceLoadArmDo.Enabled,
                        SwingArm        = sourceLoadArmDo.SwingArm,
                        PresetType      = this.GetMappedPresetType(sourceLoadArmDo.PresetType),
                        BayAStationID   = sourceLoadArmDo.BayAStationId,
                        BayBStationID   = sourceLoadArmDo.BayBStationId
                    };

                    if (sourceLoadArmDo.BayAArmNumber != null)
                    {
                        targetLoadArmDo.BayAArmNumber = sourceLoadArmDo.BayAArmNumber.Value;
                    }

                    if (sourceLoadArmDo.BayBArmNumber != null)
                    {
                        targetLoadArmDo.BayBArmNumber = sourceLoadArmDo.BayBArmNumber.Value;
                    }

                    targetLoadArmList.Add(targetLoadArmDo);

                    // Get the product maps along with the process variables associated to the product map.
                    targetLoadArmDo.OffloadExternalProductCollection = 
                                this.GetProductMapBasedOnType(PRODUCT_MAP_TYPE.OFFLOAD_EXTERNAL_METER_MAP, sourceLoadArmDo, processVariableMap, productMapProcessVariable);
                    targetLoadArmDo.ComponentCollection =
                                this.GetProductMapBasedOnType(PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP, sourceLoadArmDo, processVariableMap, productMapProcessVariable);
                    targetLoadArmDo.ExternalComponentCollection =
                                this.GetProductMapBasedOnType(PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP, sourceLoadArmDo, processVariableMap, productMapProcessVariable);
                    targetLoadArmDo.FlowControlledAdditiveCollection =
                                this.GetProductMapBasedOnType(PRODUCT_MAP_TYPE.PRESET_FLOW_CONTROLLED_ADDITIVE_MAP, sourceLoadArmDo, processVariableMap, productMapProcessVariable);
                    targetLoadArmDo.AdditiveInjectorCollection =
                                this.GetProductMapBasedOnType(PRODUCT_MAP_TYPE.PRESET_INJECTOR_MAP, sourceLoadArmDo, processVariableMap, productMapProcessVariable);
                    targetLoadArmDo.ProductRecipeCollection =
                                this.GetProductMapBasedOnType(PRODUCT_MAP_TYPE.PRESET_RECIPE_MAP, sourceLoadArmDo, processVariableMap, productMapProcessVariable);

                    // Get the associated load arm process variables.
                    ProcessVariableCollectionClass loadArmProcessVariableList1 =
                                processVariableMap.GetTargetProcessVariables(sourceLoadArmDo.Index, PROCESS_VARIABLE_TYPE.LOADARM_PV, new List<UNIT_TYPE> { UNIT_TYPE.LOADARM_UNIT });
                    
                    ProcessVariableCollectionClass loadArmProcessVariableList2 =
                                processVariableMap.GetTargetProcessVariables(sourceLoadArmDo.Index, PROCESS_VARIABLE_TYPE.ADDITIVE_METER_FLOW_TOTAL_PV, new List<UNIT_TYPE> { UNIT_TYPE.MAX_UNIT });

                    ProcessVariableCollectionClass loadArmCombinedProcessVariableList = this.CombineProcessVariableLists(loadArmProcessVariableList1, loadArmProcessVariableList2);

                    ProcessVariableCollectionClass loadArmInputPremissiveList =
                                processVariableMap.GetTargetProcessVariables(sourceLoadArmDo.Index, PROCESS_VARIABLE_TYPE.INPUT_PERMISSIVE_PV, new List<UNIT_TYPE> { UNIT_TYPE.LOADARM_INPUT_PERMISSIVE });

                    ProcessVariableCollectionClass loadArmOutputPremissiveList =
                                processVariableMap.GetTargetProcessVariables(sourceLoadArmDo.Index, PROCESS_VARIABLE_TYPE.OUTPUT_PERMISSIVE_PV, new List<UNIT_TYPE> { UNIT_TYPE.LOADARM_OUTPUT_PERMISSIVE });

                    ProcessVariableCollectionClass loadArmNoAdditiveInputPermList =
                                processVariableMap.GetTargetProcessVariables(sourceLoadArmDo.Index, PROCESS_VARIABLE_TYPE.INPUT_PERMISSIVE_PV, new List<UNIT_TYPE> { UNIT_TYPE.NOADDITIVE_INPUT_PERMISSIVE });

                    ProcessVariableCollectionClass loadArmNoAdditiveOutputPermList =
                                processVariableMap.GetTargetProcessVariables(sourceLoadArmDo.Index, PROCESS_VARIABLE_TYPE.OUTPUT_PERMISSIVE_PV, new List<UNIT_TYPE> { UNIT_TYPE.NOADDITIVE_OUTPUT_PERMISSIVE });

                    if(loadArmCombinedProcessVariableList != null && loadArmCombinedProcessVariableList.Count > 0)
                    {
                        targetLoadArmDo.ProcessVariableCollection = loadArmCombinedProcessVariableList;
                    }

                    if (loadArmInputPremissiveList != null && loadArmInputPremissiveList.Count > 0)
                    {
                        targetLoadArmDo.LoadArmPermissives.Inputs = loadArmInputPremissiveList;
                    }

                    if (loadArmOutputPremissiveList != null && loadArmOutputPremissiveList.Count > 0)
                    {
                        targetLoadArmDo.LoadArmPermissives.Outputs = loadArmOutputPremissiveList;
                    }

                    if (loadArmNoAdditiveInputPermList != null && loadArmNoAdditiveInputPermList.Count > 0)
                    {
                        targetLoadArmDo.NoAdditivePermissives.Inputs = loadArmNoAdditiveInputPermList;
                    }

                    if (loadArmNoAdditiveOutputPermList != null && loadArmNoAdditiveOutputPermList.Count > 0)
                    {
                        targetLoadArmDo.NoAdditivePermissives.Outputs = loadArmNoAdditiveOutputPermList;
                    }

                    if (this.productMapMapping.MessageFlag)
                    {
                        base.MessageFlag = true;
                        base.Message = base.Message + Environment.NewLine + this.productMapMapping.Message;
                    }

                    insertCount++;
                }

                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine + "Migrated " + insertCount + " Load Arms for station with ID of " + sourceStationDo.Id + ".";
            }

            if (targetLoadArmList.Count == 0)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine + "Migrated zero Load Arms for station with ID of " + sourceStationDo.Id + ".";
            }

            return targetLoadArmList;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method will retrieve the product maps along with the associated process variables base
        /// on the product maps associated to the load arm, product map type and unit types
        /// </summary>
        /// <param name="productMapType">The product map type to filter on.</param>
        /// <param name="sourceLoadArmDo">The load arm data object.</param>
        /// <param name="processVariableMap">The process variable map object.</param>
        /// <param name="productMapProcessVariableFlag">Flag to indicate whether to retrieve the process variables for a product map (true = retrieve)</param>
        /// <returns>Returns a product map collection or empty collection if not found.</returns>
        private ProductMapCollectionClass GetProductMapBasedOnType(PRODUCT_MAP_TYPE productMapType
                                , LoadArm753ToV12Do sourceLoadArmDo
                                , ProcessVariableMapping753ToV12 processVariableMap
                                , bool productMapProcessVariableFlag)
        {
            var unitTypeList = new List<UNIT_TYPE>();

            switch(productMapType)
            {
                case PRODUCT_MAP_TYPE.OFFLOAD_EXTERNAL_METER_MAP:
                    unitTypeList.Add(UNIT_TYPE.EXTERNAL_COMPONENT_INPUT_PERMISSIVE);
                    unitTypeList.Add(UNIT_TYPE.EXTERNAL_COMPONENT_OUTPUT_PERMISSIVE);
                    unitTypeList.Add(UNIT_TYPE.PRODUCT_MAP_OFFLOAD_EXTERNAL_METER);
                    unitTypeList.Add(UNIT_TYPE.PRODUCT_MAP_PRESET_EXTERNAL_COMPONENT);
                    break;
                case PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP:
                case PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP:
                    unitTypeList.Add(UNIT_TYPE.COMPONENT_INPUT_PERMISSIVE);
                    unitTypeList.Add(UNIT_TYPE.COMPONENT_OUTPUT_PERMISSIVE);
                    unitTypeList.Add(UNIT_TYPE.PRODUCT_MAP_PRESET_EXTERNAL_COMPONENT);
                    break;
                case PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP:
                    unitTypeList.Add(UNIT_TYPE.EXTERNAL_COMPONENT_INPUT_PERMISSIVE);
                    unitTypeList.Add(UNIT_TYPE.EXTERNAL_COMPONENT_OUTPUT_PERMISSIVE);
                    unitTypeList.Add(UNIT_TYPE.PRODUCT_MAP_PRESET_EXTERNAL_COMPONENT);
                    break;
                case PRODUCT_MAP_TYPE.PRESET_FLOW_CONTROLLED_ADDITIVE_MAP:
                    unitTypeList.Add(UNIT_TYPE.FLOW_CONTROLLED_ADDITIVE_INPUT_PERMISSIVE);
                    unitTypeList.Add(UNIT_TYPE.FLOW_CONTROLLED_ADDITIVE_OUTPUT_PERMISSIVE);
                    break;
                case PRODUCT_MAP_TYPE.PRESET_INJECTOR_MAP:
                    unitTypeList.Add(UNIT_TYPE.ADDITIVE_INPUT_PERMISSIVE);
                    unitTypeList.Add(UNIT_TYPE.ADDITIVE_OUTPUT_PERMISSIVE);
                    unitTypeList.Add(UNIT_TYPE.PRODUCT_MAP_PRESET_INJECTOR);
                    unitTypeList.Add(UNIT_TYPE.PRODUCT_MAP_PRESET_EXTERNAL_COMPONENT);
                    break;
                case PRODUCT_MAP_TYPE.PRESET_RECIPE_MAP:
                    unitTypeList.Add(UNIT_TYPE.RECIPE_INPUT_PERMISSIVE);
                    unitTypeList.Add(UNIT_TYPE.RECIPE_OUTPUT_PERMISSIVE);
                    break;
                default:
                    unitTypeList.Add(UNIT_TYPE.MAX_UNIT);
                    break;
            }

            if(unitTypeList.Count > 1 && unitTypeList[0] != UNIT_TYPE.MAX_UNIT)
            {
                this.productMapMapping.Message = string.Empty;
                this.productMapMapping.MessageFlag = false;
                ProductMapCollectionClass targetProductMapList = this.productMapMapping.GetProductMapCollection(sourceLoadArmDo
                                                                                                            , productMapType
                                                                                                            , processVariableMap
                                                                                                            , unitTypeList
                                                                                                            , productMapProcessVariableFlag);
                return targetProductMapList;
            }

            return new ProductMapCollectionClass();
        }

        /// <summary>
        /// This method is a helper method to combine process variable collections.
        /// </summary>
        /// <param name="list1">The first list to combine.</param>
        /// <param name="list2">The first list to combine.</param>
        /// <returns>Returns a combined list.</returns>
        private ProcessVariableCollectionClass CombineProcessVariableLists(ProcessVariableCollectionClass list1, ProcessVariableCollectionClass list2)
        {
            var combineList = new ProcessVariableCollectionClass();

            if ((list1 == null || list1.Count == 0)
                && (list2 == null || list2.Count == 0))
            {
                return combineList;
            }

            if(list1 != null && list1.Count > 0
                && list2 != null && list2.Count > 0)
            {
                foreach(ProcessVariableClass processVariable in list1)
                {
                    combineList.Add(processVariable);
                }

                foreach (ProcessVariableClass processVariable in list2)
                {
                    combineList.Add(processVariable);
                }

                return combineList;
            }

            if(list1 != null && list1.Count > 0)
            {
                return list1;
            }

            return list2;
        }

        /// <summary>
        /// This method will return the correct Preset Type mapping between the
        /// two versions of FuelsManager which are different.
        /// </summary>
        /// <param name="sourcePresetType">The source preset type</param>
        /// <returns>Return the correct Preset Type.</returns>
        private PRESET_TYPE GetMappedPresetType(int sourcePresetType)
        {
            // Zero through 14 are the same and so is 17.
            if(sourcePresetType <= 14 || sourcePresetType == 17)
            {
                return (PRESET_TYPE)sourcePresetType;
            }

            // 15 in v7.5.3 is VARECDET, but it is 16 in v12
            if(sourcePresetType == 15)
            {
                return PRESET_TYPE.VARECDET;
            }

            // 16 in v7.5.3 is CONTREC1010_RA, but it is 15 in v12
            if (sourcePresetType == 16)
            {
                return PRESET_TYPE.CONTREC1010_RA;
            }

            return PRESET_TYPE.MAX_PRESET_TYPE;
        }
        #endregion
    }
}
