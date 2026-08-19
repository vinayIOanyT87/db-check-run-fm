namespace MirgrationToolProcessing.Mapping
{
    using BusinessObjects.MigrationObjects;
    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using MigrationToolBusinessObjects.Handlers;
    using MigrationToolDataAccessLayer;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Runtime.InteropServices;
    using Varec.CommonComponents.EngineeringUnitsLibrary;

    public class StationMapping753ToV12 : StationMappingBase
    {
        #region Data Member
        private MigrationDatabaseDAClass migrationDA;
        private MigrationDatabaseDAClass migrationTargetDA;
        private List<OpcConnection753ToV12Do> targetOpcConnectionDoList;
        private List<QualificationMapsBaseDo> qualificationMapsList;
        private QualificationCollectionClass targetQualificationCollection;
        private Guid targetSiteGuid;
        private ProcessVariableMapping753ToV12 processVariableMap;
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public StationMapping753ToV12()
        {
            base.Init();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// This method implements the Perform Mapping for tanks.
        /// </summary>
        /// <param name="stationBaseDo">The station data object to map.</param>
        /// <param name="migrationDA">The Migration data access object.</param>
        public override void PerformMapping(StationsBaseDo stationBaseDo, MigrationDatabaseDAClass migrationDA, MigrationDatabaseDAClass migrationTargetDA)
        {
            this.migrationDA = migrationDA;
            this.migrationTargetDA = migrationTargetDA;

            base.MessageFlag = false;
            base.Message = string.Empty;

            var sourceStationDoList = new List<Stations753ToV12Do>();
            Stations753ToV12Do sourceStationDo = stationBaseDo as Stations753ToV12Do;
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
                sourceStationDo.GetSiteIndexByIdSql(command, base.SourceSiteId);
                DataSet dataSet = this.migrationDA.GetDataSet(command);
                sourceSiteIndex = sourceStationDo.GetSiteIndex(dataSet);
            }

            if (sourceSiteIndex == null)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Error: Source Site Index is not found.";
                return;
            }

            // Get target site
            this.targetSiteGuid = Guid.Empty;

            try
            {
                this.targetSiteGuid = FMChannelHelper.MakeCall<ISites, Guid>(x => x.GetIdentityGuid(base.SecurityHndlr.Security, base.TargetSiteId));
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

            // Get source stations
            using (var command = new SqlCommand())
            {
                sourceStationDo.EnumerateStationsSql(command, sourceSiteIndex.Value);
                sourceDataSet = this.migrationDA.GetDataSet(command);
            }

            if (sourceDataSet == null || sourceDataSet.Tables.Count == 0 || sourceDataSet.Tables[0].Rows.Count == 0)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Warning: No Stations found in the 7.5.3 '" + sourceStationDo.SourceDbName + "' database.";
                return;
            }

            foreach (DataRow row in sourceDataSet.Tables[0].Rows)
            {
                var newSourceStationDo = new Stations753ToV12Do();
                newSourceStationDo.Load(row);
                sourceStationDoList.Add(newSourceStationDo);
            }

            if (sourceStationDoList.Count == 0)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Warning: No Stations found in the 7.5.3 '" + sourceStationDo.SourceDbName + "' database.";
                return;
            }

            // Get load arms
            var sourceLoadArmDo = new LoadArm753ToV12Do(sourceStationDo.SourceDbName, sourceStationDo.TargetDbName);
            var loadArmMapping = new LoadArmMapping753ToV12
            {
                SourceSiteId = base.SourceSiteId,
                TargetSiteId = base.TargetSiteId,
                SecurityHndlr = base.SecurityHndlr
            };
            loadArmMapping.RetrieveAllMapping(sourceLoadArmDo, this.migrationDA);
           
            // Get the OPC connection information from the target database.
            DataSet targetDataSet = null;
            this.targetOpcConnectionDoList = new List<OpcConnection753ToV12Do>();

            using (var command = new SqlCommand())
            {
                var targetOpcConnectionDo = new OpcConnection753ToV12Do(sourceStationDo.SourceDbName, sourceStationDo.TargetDbName);
                targetOpcConnectionDo.EnumerateTargetOpcConnectionSql(command);
                targetDataSet = this.migrationTargetDA.GetDataSet(command);
            }

            if (targetDataSet == null || targetDataSet.Tables.Count == 0 || targetDataSet.Tables[0].Rows.Count == 0)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Warning: No OPC Connection found in the V12 '" + sourceStationDo.TargetDbName + "' database.";
            }
            else
            {
                foreach (DataRow row in targetDataSet.Tables[0].Rows)
                {
                    var targetOpcConnectionDo = new OpcConnection753ToV12Do();
                    targetOpcConnectionDo.Load(row);
                    this.targetOpcConnectionDoList.Add(targetOpcConnectionDo);
                }
            }

            // Get all the process variable information
            var sourceProcessVariableDo = new ProcessVariables753ToV12Do(sourceStationDo.SourceDbName, sourceStationDo.TargetDbName);
            this.processVariableMap = new ProcessVariableMapping753ToV12()
            {
                SourceSiteId = base.SourceSiteId,
                TargetSiteId = base.TargetSiteId,
                SecurityHndlr = base.SecurityHndlr,
                TargetOpcConnectionDoList = this.targetOpcConnectionDoList
            };
            this.processVariableMap.RetrieveAllMapping(sourceProcessVariableDo, this.migrationDA, this.migrationTargetDA);

            // Get the list of source DB qualification maps.
            var sourceDbQualificationMapping = new SourceDbQualificationMapping753ToV12();
            sourceDbQualificationMapping.GetSourceQualificationMaps(this.migrationDA);
            this.qualificationMapsList = sourceDbQualificationMapping.QualificationMapsBaseList;

            try
            {
                // Get the target qualification list.
                this.GetTargetQualifications();
            }
            catch (Exception ex)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Error: Could not get the target qualifications for site: " + base.TargetSiteId + ". " + ex.Message;
                return;
            }

            this.MapStations(sourceStationDoList, loadArmMapping);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method maps the source tanks to the target.
        /// </summary>
        /// <param name="sourceTankDoList">The list of source tanks.</param>
        private void MapStations(List<Stations753ToV12Do> sourceStationDoList, LoadArmMapping753ToV12 loadArmMapping)
        {
            // For stations, the target site is going to be the same as the source site.
            Guid targetSiteGuid = Guid.Empty;

            try
            {
                // For stations, the target site is going to be the same as the source site.
                targetSiteGuid = FMChannelHelper.MakeCall<ISites, Guid>(x => x.GetIdentityGuid(base.SecurityHndlr.Security, base.SourceSiteId));
            }
            catch (Exception ex)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Error: Retrieving target site GUID for ID '" + base.SourceSiteId + "'. " + ex.Message;
                return;
            }

            if (targetSiteGuid == Guid.Empty)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Error: Target Site GUID is not found for Target Site ID: " + base.SourceSiteId;
                return;
            }

            // Set the target site for the migration, which is the same as the source site ID.
            base.SecurityHndlr.Security.SiteGuid = base.SecurityHndlr.GetSiteGuidById(base.SourceSiteId);

            // Get the transaction alias to be search and retrieve guids.
            var transactionAliasList = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasCollectionClass>(x => x.Enumerate(this.SecurityHndlr.Security));

            // Get the target tanks to be used to get tank guid.
            var targetTankList = FMChannelHelper.MakeCall<ITanks, TankCollectionClass>(x => x.Enumerate(this.SecurityHndlr.Security, false));

            // Get the list of target stations to be used to check if already exists.
            var targetStationsList = FMChannelHelper.MakeCall<IStations, StationCollectionClass>(x => x.Enumerate(base.SecurityHndlr.Security));
            int insertCount = 0;

            //var stationFileHandler = new FileHandler();
            //FileStream fileHandle = stationFileHandler.OpenMigrationFile(FileHandler.FileTypes.Station);

            foreach (Stations753ToV12Do sourceStationDo in sourceStationDoList)
            {
                bool stationExist = this.StationExists(sourceStationDo.Id, ref targetStationsList);

                if (stationExist == false)
                {
                    var targetStationDo = new StationClass
                    {
                        SiteGuid                            = targetSiteGuid,
                        SiteID                              = this.SourceSiteId,  // Target site and source will be the same
                        ID                                  = sourceStationDo.Id,
                        Type                                = sourceStationDo.Type == -99 ? STATION_TYPE.MAX_STATION_TYPE : (STATION_TYPE)sourceStationDo.Type,
                        SwingArmPosition                    = sourceStationDo.SwingArmPosition ? "A": "B",
                        VaporRecovery                       = sourceStationDo.VaporRecovery,
                        InterfaceType                       = (STATION_INTERFACE_TYPE)sourceStationDo.InterfaceType,
                        Enabled                             = sourceStationDo.Enabled,
                        BOLPrinter                          = sourceStationDo.BolPrinter,
                        PreloadPrinter                      = sourceStationDo.PreloadPrinter,
                        BOLAgeInMinutes                     = sourceStationDo.BolAgeInMinutes,
                        IssueByVolumeTransactionAliasGuid   = this.GetTransactionGuid(sourceStationDo.IssueByVolumeAliasName, ref transactionAliasList),
                        IssueByWeightTransactionAliasGuid   = this.GetTransactionGuid(sourceStationDo.IssueByWeightAliasName, ref transactionAliasList),
                        ReceiptByVolumeTransactionAliasGuid = this.GetTransactionGuid(sourceStationDo.ReceiptByVolumeAliasName, ref transactionAliasList),
                        ReceiptByWeightTransactionAliasGuid = this.GetTransactionGuid(sourceStationDo.ReceiptByWeightAliasName, ref transactionAliasList),
                        CardReader                          = sourceStationDo.CardReader,
                        ThirtyFiveBitCardSupport            = sourceStationDo.ThirtyFiveBitCardSupport,
                        NumberOfCopies                      = sourceStationDo.NumberOfCopies == null ? 0 : sourceStationDo.NumberOfCopies.Value,
                        NumberOfPreloadCopies               = sourceStationDo.NumberOfPreloadCopies == null ? 0 : sourceStationDo.NumberOfPreloadCopies.Value,
                        InhibitLoadingByLoadID              = sourceStationDo.InhibitLoadingByLoadId,
                        InhibitOperatingModePrompt          = sourceStationDo.InhibitOperatingModePrompt,
                        SynchronizeReferenceDensity         = sourceStationDo.SynchronizeReferenceDensity,
                        SignatureDevice                     = sourceStationDo.SignatureDevice,
                        SetDefaultPresetToZero              = sourceStationDo.SetDefaultPresetToZero,
                        AssociatedTankGuid                  = this.GetTankGuid(sourceStationDo.TankId, ref targetTankList),
                        ArmsServiced                        = sourceStationDo.ArmsServiced,
                        InhibitSettingRecipeNames           = sourceStationDo.InhibitSettingRecipeNames,
                        SignatureDevicePort                 = sourceStationDo.SignatureDevicePort == null ? 0 : sourceStationDo.SignatureDevicePort.Value,
                        SignatureDeviceBaudRate             = sourceStationDo.SignatureDeviceBaudRate == null ? 0 : sourceStationDo.SignatureDeviceBaudRate.Value,
                        MeterRecircCardNumber               = sourceStationDo.MeterRecircCardNumber,
                        RecircTransactionAliasGuid          = this.GetTransactionGuid(sourceStationDo.RecircAliasName, ref transactionAliasList),
                        TouchKeyReader                      = sourceStationDo.TouchKeyReader,
                        OffLoadByOffLoadID                  = sourceStationDo.OffLoadByOffLoadId,
                        UseManualMeterData                  = sourceStationDo.UseManualMeterData,
                        PromptForBOLNumber                  = sourceStationDo.PromptForBolNumber,
                        StationPromptTimeout                = sourceStationDo.StationPromptTimeout == null ? 0 : sourceStationDo.StationPromptTimeout.Value,
                        StationMessageTimeout               = sourceStationDo.StationMessageTimeout == null ? 0 : sourceStationDo.StationMessageTimeout.Value,
                        LogCommunications                   = sourceStationDo.LogCommunications,
                        LogCommPath                         = sourceStationDo.LogCommPath,
                        LastTransactionNumber               = sourceStationDo.LastTransactionNumber == null ? 0 : sourceStationDo.LastTransactionNumber.Value,
                        LastTransactionNumberDateTime       = sourceStationDo.LastTransactionNumberDateTime == null ? DateTime.Now : sourceStationDo.LastTransactionNumberDateTime.Value,
                        EnableScully                        = sourceStationDo.EnableScully,
                        EnableEquipmentValidate             = sourceStationDo.EnableEquipmentValidate,
                        QueryForTrailers                    = sourceStationDo.QueryForTrailers
                    };

                    // Create a meter based on the station ID.
                    MeterClass stationMeter = this.CreateMeters(sourceStationDo, targetSiteGuid);

                    if(stationMeter != null && stationMeter.IdentityGuid != Guid.Empty)
                    {
                        targetStationDo.Meter = stationMeter;
                    }

                    loadArmMapping.Message = string.Empty;
                    loadArmMapping.MessageFlag = false;
                    LoadArmCollectionClass targetLoadArmCollection = loadArmMapping.GetLoadArmCollection(sourceStationDo, this.processVariableMap, true);

                    if (targetLoadArmCollection != null && targetLoadArmCollection.Count > 0)
                    {
                        targetStationDo.LoadArmCollection = targetLoadArmCollection;
                    }

                    if (loadArmMapping.MessageFlag)
                    {
                        base.MessageFlag = true;
                        base.Message = base.Message + Environment.NewLine + loadArmMapping.Message;
                    }

                    var targetProcessVariablesCollection = this.processVariableMap.GetTargetProcessVariables(sourceStationDo.Index, PROCESS_VARIABLE_TYPE.MAX_PV, new List<UNIT_TYPE> { UNIT_TYPE.STATION_UNIT });
                    var targetPermissiveInputCollection  = this.processVariableMap.GetTargetProcessVariables(sourceStationDo.Index, PROCESS_VARIABLE_TYPE.INPUT_PERMISSIVE_PV, new List<UNIT_TYPE> { UNIT_TYPE.STATION_INPUT_PERMISSIVE });
                    var targetPermissiveOutputCollection = this.processVariableMap.GetTargetProcessVariables(sourceStationDo.Index, PROCESS_VARIABLE_TYPE.OUTPUT_PERMISSIVE_PV, new List<UNIT_TYPE> { UNIT_TYPE.STATION_OUTPUT_PERMISSIVE });

                    if (targetProcessVariablesCollection != null && targetProcessVariablesCollection.Count > 0)
                    {
                        targetStationDo.ProcessVariableCollection = targetProcessVariablesCollection;
                    }

                    if (targetPermissiveInputCollection != null && targetPermissiveInputCollection.Count > 0)
                    {
                        targetStationDo.StationPermissives.Inputs = targetPermissiveInputCollection;
                    }

                    if (targetPermissiveOutputCollection != null && targetPermissiveOutputCollection.Count > 0)
                    {
                        targetStationDo.StationPermissives.Outputs = targetPermissiveOutputCollection;
                    }

                    // Get station qualifications
                    QualificationMapCollectionClass testAndInspectionCollection = 
                            this.MapStationQualifications(sourceStationDo, targetStationDo, QualificationMapsBaseDo.QualificationMapTypes.EQUIPMENT_TEST_AND_INSPECTION_TO_STATION);
                    
                    QualificationMapCollectionClass licenseCollection =
                            this.MapStationQualifications(sourceStationDo, targetStationDo, QualificationMapsBaseDo.QualificationMapTypes.PERSON_LICENSE_TO_STATION);

                    QualificationMapCollectionClass qualificationCollection =
                            this.MapStationQualifications(sourceStationDo, targetStationDo, QualificationMapsBaseDo.QualificationMapTypes.PERSON_QUALIFICATION_TO_STATION);

                    QualificationMapCollectionClass trainingCollection =
                            this.MapStationQualifications(sourceStationDo, targetStationDo, QualificationMapsBaseDo.QualificationMapTypes.PERSON_TRAINING_TO_STATION);

                    if(testAndInspectionCollection != null && testAndInspectionCollection.Count > 0)
                    {
                        targetStationDo.ReqTestsandInspectionsCollection = testAndInspectionCollection;
                    }

                    if (licenseCollection != null && licenseCollection.Count > 0)
                    {
                        targetStationDo.ReqLicenseCollection = licenseCollection;
                    }

                    if (qualificationCollection != null && qualificationCollection.Count > 0)
                    {
                        targetStationDo.ReqQualificationsCollection = qualificationCollection;
                    }

                    if (trainingCollection != null && trainingCollection.Count > 0)
                    {
                        targetStationDo.ReqTrainingCollection = trainingCollection;
                    }

                    // Migration the station
                    try
                    {
                        Guid targetStationGuid = FMChannelHelper.MakeCall<IStations, Guid>(x => x.Add(base.SecurityHndlr.Security, targetStationDo));
                        targetStationDo.IdentityGuid = targetStationGuid;

                        //var migratedItem = new MigratedItem
                        //{
                        //    ID = targetStationDo.ID,
                        //    ItemGuid = targetStationGuid,
                        //    SiteGuid = targetSiteGuid
                        //};

                        //stationFileHandler.WriteMigrationData(fileHandle, migratedItem);

                        insertCount++;
                    }
                    catch (Exception ex)
                    {
                        base.MessageFlag = true;
                        base.Message = base.Message + Environment.NewLine
                                            + "Error: Adding Station ID '" + sourceStationDo.Id + "' to the target DB. " + ex.Message;
                    }
                }
                else
                {
                    base.MessageFlag = true;
                    base.Message = base.Message + Environment.NewLine
                                        + "Info: Station ID '" + sourceStationDo.Id + "' already exists at Target site '"
                                        + base.TargetSiteId + "'.";
                }
            }

            if (insertCount > 0)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Info: Successfully migrated " + insertCount + " Station items.";
            }

            // Reset the site guid to SiteAdmin.
            base.SecurityHndlr.Security.SiteGuid = base.SecurityHndlr.SiteAdminGuid;
            //fileHandle.Close();
        }

        /// <summary>
        /// This method is a helper to find if an existing station already exists at the
        /// target database.
        /// </summary>
        /// <param name="sourceStationId">The source station ID</param>
        /// <param name="targetStationList">The target station list.</param>
        /// <returns>Return false if the station does not exist at the target DB. Otherwise, returns true.</returns>
        private bool StationExists(string sourceStationId, ref StationCollectionClass targetStationList)
        {
            if (targetStationList == null || targetStationList.Count <= 0)
            {
                return false;
            }

            StationClass stationTank = targetStationList.Find(x => x.ID == sourceStationId);

            if (stationTank == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// This method will create a meter based on the station ID. In v7.5.3 there is no
        /// meter table, so it has to be created per station.
        /// </summary>
        /// <param name="sourceStationDo">The source station data object.</param>
        /// <param name="targetSiteGuid">The target site Guid.</param>
        private MeterClass CreateMeters(Stations753ToV12Do sourceStationDo, Guid targetSiteGuid)
        {
            var meterDo = new MeterClass
            {
                IdentityGuid            = Guid.Empty,
                SiteGuid                = targetSiteGuid,
                ID                      = sourceStationDo.Id,
                NumberOfDigits          = 8,
                RotatesBackwardsFlag    = false,
                ReceiptMeterFlag        = false,
                CreatedBy               = "Migration Tool",
                CreatedDate             = DateTime.Now,
                UpdatedBy               = "Migration Tool",
                UpdatedDate             = DateTime.Now
            };

            try
            {
                Guid foundMeterGuid = FMChannelHelper.MakeCall<IMeters, Guid>(x => x.GetIdentityGuid(this.SecurityHndlr.Security, sourceStationDo.Id));

                if (foundMeterGuid != null && foundMeterGuid != Guid.Empty)
                {
                    var foundMeter = FMChannelHelper.MakeCall<IMeters, MeterClass>(x => x.Get(this.SecurityHndlr.Security, foundMeterGuid));
                    return foundMeter;
                }
            }
            catch(Exception ex)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                        + "Error: Retrieving Meter for Station ID '" + sourceStationDo.Id + "' to the target DB. " + ex.Message;
            }

            try
            {
                var meterGuid = FMChannelHelper.MakeCall<IMeters, Guid>(x => x.Add(this.SecurityHndlr.Security, meterDo));
                meterDo.IdentityGuid = meterGuid;
            }
            catch(Exception ex)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                        + "Error: Adding Meter for Station ID '" + sourceStationDo.Id + "' to the target DB. " + ex.Message;
            }

            return meterDo;
        }

        /// <summary>
        /// This method will return a target transaction Guid for a given alias name.
        /// </summary>
        /// <param name="aliasName">The alias name to retrieve the Guid.</param>
        /// <param name="targetTransactionAliasList">The target transaction alias list.</param>
        /// <returns>Returns the transaction guid if found, otherwise returns empty guid.</returns>
        private Guid GetTransactionGuid(string aliasName, ref TransactionAliasCollectionClass targetTransactionAliasList)
        {
            if(string.IsNullOrEmpty(aliasName) || targetTransactionAliasList == null || targetTransactionAliasList.Count == 0)
            {
                return Guid.Empty;
            }

            TransactionAliasClass transactionAlias = targetTransactionAliasList.Find(x => x.ID == aliasName);

            if(transactionAlias == null)
            {
                return Guid.Empty;
            }

            return transactionAlias.IdentityGuid;
        }

        /// <summary>
        /// This method will return the target tank guid for a given tank.
        /// </summary>
        /// <param name="tankId">The tank ID to search on.</param>
        /// <param name="targetTankList">The target tank list.</param>
        /// <returns>Returns the tank guid if found, otherwise returns empty guid.</returns>
        private Guid GetTankGuid(string tankId, ref TankCollectionClass targetTankList)
        {
            if (string.IsNullOrEmpty(tankId) || targetTankList == null || targetTankList.Count == 0)
            {
                return Guid.Empty;
            }

            TankClass targetTank = targetTankList.Find(x => x.ID == tankId);

            if (targetTank == null)
            {
                return Guid.Empty;
            }

            return targetTank.IdentityGuid;
        }

        /// <summary>
        /// This is a helper function to get the target OPC Connection Guid based on
        /// the Prog ID.
        /// </summary>
        /// <param name="sourceOpcProgId">The source Prog ID to search on.</param>
        /// <returns>Returns the target OPC connection Guid or empty Guid if not found.</returns>
        private Guid GetOpcConnectionGuid(string sourceOpcProgId)
        {
            if(string.IsNullOrEmpty(sourceOpcProgId))
            {
                return Guid.Empty;
            }

            OpcConnection753ToV12Do targetOpcConnectionDo = this.targetOpcConnectionDoList.Find(x => x.ProgId == sourceOpcProgId);

            if(targetOpcConnectionDo == null)
            {
                return Guid.Empty;
            }

            return targetOpcConnectionDo.OpcConnectionGuid;
        }

        /// <summary>
        /// This method will map the station qualification maps.
        /// </summary>
        /// <param name="sourceStationDo">The source station data object.</param>
        /// <param name="targetStationDo">The target station data object.</param>
        /// <param name="mapType">The station map type.</param>
        /// <returns>Returns the equipment qualification map collection.</returns>
        private QualificationMapCollectionClass MapStationQualifications(Stations753ToV12Do sourceStationDo
                                                                        , StationClass targetStationDo
                                                                        , QualificationMapsBaseDo.QualificationMapTypes mapType)
        {
            int stationMapType = (int)mapType;

            QualificationMapCollectionClass stationQualificationCollection = new QualificationMapCollectionClass();

            if (this.qualificationMapsList.Count == 0)
            {
                return stationQualificationCollection;
            }

            List<QualificationMapsBaseDo> qualificationMapsBaseList =
                                            this.qualificationMapsList.FindAll(x => x.Index == sourceStationDo.Index
                                            && x.Type == stationMapType);

            if (qualificationMapsBaseList == null || qualificationMapsBaseList.Count == 0)
            {
                return stationQualificationCollection;
            }

            foreach (QualificationMapsBaseDo qualificationMapsBaseDo in qualificationMapsBaseList)
            {
                Guid targetQualificationGuid = Guid.Empty;
                var qualificationMaps753ToV12Do = (QualificationMaps753ToV12Do)qualificationMapsBaseDo;

                QualificationClass targetQualification =
                     this.targetQualificationCollection.Find(x => x.ID == qualificationMaps753ToV12Do.QualificationId);

                if (targetQualification != null)
                {
                    targetQualificationGuid = targetQualification.IdentityGuid;
                }

                QualificationMapClass targetQualificationMap = new QualificationMapClass
                {
                    ID                  = qualificationMaps753ToV12Do.Id,
                    Number              = qualificationMaps753ToV12Do.Id,
                    Type                = (QUALIFICATION_MAP_TYPE)qualificationMaps753ToV12Do.Type,
                    Rating              = qualificationMaps753ToV12Do.Rating,
                    HistoricalRecord    = qualificationMaps753ToV12Do.HistoricalRecord,
                    Instructor          = qualificationMaps753ToV12Do.Instructor,
                    SiteGuid            = targetStationDo.SiteGuid,
                    SiteID              = targetStationDo.SiteID,
                    AssignedGuid        = targetQualificationGuid
                };

                if (qualificationMaps753ToV12Do.DateCompleted != null)
                {
                    var newDate = new Date { Value = qualificationMaps753ToV12Do.DateCompleted.Value };
                    targetQualificationMap.DateCompleted = newDate;
                }

                if (qualificationMaps753ToV12Do.DateDue != null)
                {
                    var newDate = new Date { Value = qualificationMaps753ToV12Do.DateDue.Value };
                    targetQualificationMap.DateDue = newDate;
                }

                if (qualificationMaps753ToV12Do.ExpirationDate != null)
                {
                    var newDate = new Date { Value = qualificationMaps753ToV12Do.ExpirationDate.Value };
                    targetQualificationMap.ExpirationDate = newDate;
                }

                stationQualificationCollection.Add(targetQualificationMap);
            }

            return stationQualificationCollection;
        }

        /// <summary>
        /// This method will retrieve the target qualification data.
        /// </summary>
        private void GetTargetQualifications()
        {
            this.SecurityHndlr.Security.SiteGuid = this.targetSiteGuid;
            this.SecurityHndlr.Security.SiteID = base.TargetSiteId;

            this.targetQualificationCollection =
                            FMChannelHelper.MakeCall<IQualifications, QualificationCollectionClass>(x => x.Enumerate(this.SecurityHndlr.Security));

            this.SecurityHndlr.Security.SiteGuid = this.SecurityHndlr.SiteAdminGuid;
            this.SecurityHndlr.Security.SiteID = "SiteAdmin";
        }

        private void UpdateLoadArmProductMapProcessVariables(StationClass justSavedStation, LoadArmCollectionClass originalLoadArmCollection)
        {
            if(justSavedStation.LoadArmCollection == null || justSavedStation.LoadArmCollection.Count == 0)
            {
                return;
            }

            foreach(LoadArmClass originalLoadArm in originalLoadArmCollection)
            {
                LoadArmClass foundLoadArm = justSavedStation.LoadArmCollection.Find(x => x.BayAStationID == originalLoadArm.BayAStationID
                                                                                    && x.BayBStationID == originalLoadArm.BayBStationID
                                                                                    && x.BayAArmNumber == originalLoadArm.BayAArmNumber
                                                                                    && x.BayBArmNumber == originalLoadArm.BayBArmNumber);

                if(foundLoadArm != null)
                {
                    if(foundLoadArm.OffloadExternalProductCollection.Count > 0)
                    {
                        this.UpdateProductMapWithProcessVariables(foundLoadArm.OffloadExternalProductCollection, originalLoadArm.OffloadExternalProductCollection);
                    }

                    if (foundLoadArm.ComponentCollection.Count > 0)
                    {
                        this.UpdateProductMapWithProcessVariables(foundLoadArm.ComponentCollection, originalLoadArm.ComponentCollection);
                    }

                    if (foundLoadArm.ExternalComponentCollection.Count > 0)
                    {
                        this.UpdateProductMapWithProcessVariables(foundLoadArm.ExternalComponentCollection, originalLoadArm.ExternalComponentCollection);
                    }

                    if (foundLoadArm.FlowControlledAdditiveCollection.Count > 0)
                    {
                        this.UpdateProductMapWithProcessVariables(foundLoadArm.FlowControlledAdditiveCollection, originalLoadArm.FlowControlledAdditiveCollection);
                    }

                    if (foundLoadArm.AdditiveInjectorCollection.Count > 0)
                    {
                        this.UpdateProductMapWithProcessVariables(foundLoadArm.AdditiveInjectorCollection, originalLoadArm.AdditiveInjectorCollection);
                    }

                    if (foundLoadArm.ProductRecipeCollection.Count > 0)
                    {
                        this.UpdateProductMapWithProcessVariables(foundLoadArm.ProductRecipeCollection, originalLoadArm.ProductRecipeCollection);
                    }
                }
            }
        }

        private void UpdateProductMapWithProcessVariables(ProductMapCollectionClass productMapToUpdateList, ProductMapCollectionClass originalProductMapList)
        {
            foreach(ProductMapClass originalProductMap in originalProductMapList)
            {
                ProductMapClass foundProductMap = productMapToUpdateList.Find(x => x.Type == originalProductMap.Type
                                                                            && x.Sequence == originalProductMap.Sequence
                                                                            && x. AssignedGuid == originalProductMap.AssignedGuid
                                                                            && x.TankOrGroupGuid == originalProductMap.TankOrGroupGuid
                                                                            && x.Ratio == originalProductMap.Ratio);

                if(foundProductMap == null || originalProductMap.ProcessVariableCollection.Count == 0)
                {
                    return;
                }

                foreach(ProcessVariableClass processVariable in originalProductMap.ProcessVariableCollection)
                {
                    processVariable.UnitGuid = foundProductMap.IdentityGuid;

                    if(foundProductMap.ProcessVariableCollection == null)
                    {
                        foundProductMap.ProcessVariableCollection = new ProcessVariableCollectionClass();
                    }

                    foundProductMap.ProcessVariableCollection.Add(processVariable);
                }
            }
        }
        #endregion
    }
}
