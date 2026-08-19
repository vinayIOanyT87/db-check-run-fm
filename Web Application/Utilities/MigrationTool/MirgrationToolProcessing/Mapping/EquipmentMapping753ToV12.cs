namespace MirgrationToolProcessing.Mapping
{
    using BusinessObjects.MigrationObjects;
    using BusinessObjects.Utilities;
    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using MigrationToolDataAccessLayer;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using Varec.CommonComponents.EngineeringUnitsLibrary;

    public class EquipmentMapping753ToV12 : EquipmentMappingBase
    {
        #region Data members
        private List<QualificationMapsBaseDo> qualificationMapsList;
        private List<EquipmentTypeBaseDo> equipmentTypeMapsList;
        private QualificationCollectionClass targetQualificationCollection;
        private EquipmentTypeCollectionClass targetEquipmentTypeCollection;
        private Guid targetSiteGuid;
        private MigrationDatabaseDAClass migrationDA;
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public EquipmentMapping753ToV12()
        {
            base.Init();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// This method implements the Perform Mapping for equipment types.
        /// </summary>
        /// <param name="equipmentDo">The equipment data object to map.</param>
        /// <param name="migrationDA">The Migration data access object.</param>
        public override void PerformMapping(EquipmentBaseDo equipmentDo, MigrationDatabaseDAClass migrationDA)
        {
            this.migrationDA = migrationDA;
            base.MessageFlag = false;
            base.Message = string.Empty;

            var sourceEquipmentDoList = new List<Equipment753ToV12Do>();
            var sourceEquipmentCompartmentDoList = new List<Equipment753ToV12Do>();
            Equipment753ToV12Do equipment = equipmentDo as Equipment753ToV12Do;
            DataSet sourceDataSet = null;

            if (string.IsNullOrEmpty(base.SourceSiteId) || string.IsNullOrEmpty(base.TargetSiteId))
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Error: Source Site ID or Target Site ID is null.";
                return;
            }

            int? sourceSiteIndex = null;
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

            using (var command = new SqlCommand())
            {
                equipment.GetSiteIndexByIdSql(command, base.SourceSiteId);
                DataSet dataSet = migrationDA.GetDataSet(command);
                sourceSiteIndex = equipment.GetSiteIndex(dataSet);
            }

            if (sourceSiteIndex == null)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Error: Source Site Index is not found.";
                return;
            }

            // Query for equipments that are not compartments
            using (var command = new SqlCommand())
            {
                equipment.EnumerateEquipmentSql(command, sourceSiteIndex.Value);
                sourceDataSet = migrationDA.GetDataSet(command);
            }

            if (sourceDataSet == null || sourceDataSet.Tables.Count == 0 || sourceDataSet.Tables[0].Rows.Count == 0)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Warning: No Equipment found in the 7.5.3 " + equipment.SourceDbName + " database.";
                return;
            }

            // Load all the equipments that are not compartments.
            foreach (DataRow row in sourceDataSet.Tables[0].Rows)
            {
                var newEquipment = new Equipment753ToV12Do();
                newEquipment.Load(row);

                sourceEquipmentDoList.Add(newEquipment);
            }

            if (sourceEquipmentDoList.Count == 0)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Warning: No Equipment found in the 7.5.3 " + equipment.SourceDbName + " database.";
                return;
            }

            sourceDataSet = null;

            // Query for equipments that are compartments
            using (var command = new SqlCommand())
            {
                equipment.EnumerateEquipmentCompartments(command, sourceSiteIndex.Value);
                sourceDataSet = migrationDA.GetDataSet(command);
            }

            if (sourceDataSet != null && sourceDataSet.Tables.Count > 0 && sourceDataSet.Tables[0].Rows.Count > 0)
            {
                // Load all the equipments that are not compartments.
                foreach (DataRow row in sourceDataSet.Tables[0].Rows)
                {
                    var newEquipment = new Equipment753ToV12Do();
                    newEquipment.Load(row);

                    sourceEquipmentCompartmentDoList.Add(newEquipment);
                }
            }

            // Get the list of source DB qualification maps.
            var sourceDbQualificationMapping = new SourceDbQualificationMapping753ToV12();
            sourceDbQualificationMapping.GetSourceQualificationMaps(this.migrationDA);
            this.qualificationMapsList = sourceDbQualificationMapping.QualificationMapsBaseList;

            // Get the list of source DB equipment types.
            var sourceDbEquipTypeMapping = new SourceDbEquipTypesMapping753ToV12();
            sourceDbEquipTypeMapping.GetSourceEquipmentTypeMaps(this.migrationDA, equipmentDo.SourceDbName);
            this.equipmentTypeMapsList = sourceDbEquipTypeMapping.EquipmentTypesBaseList;

            try
            {
                // Get the target qualification list.
                this.GetTargetQualifications();
            }
            catch (Exception ex)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Error: Could not get the target site GUID for site: " + base.TargetSiteId + ". " + ex.Message;
                return;
            }

            try
            {
                // Get the target equipment type list.
                this.GetTargetEquipmentTypes();
            }
            catch (Exception ex)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Error: Could not get the target site GUID for site: " + base.TargetSiteId + ". " + ex.Message;
                return;
            }

            this.MapEquipment(sourceEquipmentDoList, sourceEquipmentCompartmentDoList);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method maps the source equipment to the target.
        /// </summary>
        /// <param name="equipmentList">The list of source equipment.</param>
        private void MapEquipment(List<Equipment753ToV12Do> sourceEquipmentList, List<Equipment753ToV12Do> sourceEquipmentCompartmentList)
        {
            // Set the target site for the migration.
            base.SecurityHndlr.Security.SiteGuid = base.SecurityHndlr.GetSiteGuidById(base.TargetSiteId);

            var entityAssignmentProcessor = new EntityService.EntityAssignmentProcessor(this.SecurityHndlr);

            // Note, the source site for entity assignment is the target site ID and the target entity assignment is the source site ID.
            var sourceEntitySiteGuid = FMChannelHelper.MakeCall<ISites, Guid>(x => x.GetIdentityGuid(this.SecurityHndlr.Security, this.TargetSiteId));
            var targetEntitySiteGuid = FMChannelHelper.MakeCall<ISites, Guid>(x => x.GetIdentityGuid(this.SecurityHndlr.Security, this.SourceSiteId));

            // Get the list of target equipment to be used to check if already exists.
            var targetEquipList = FMChannelHelper.MakeCall<IEquipments, EquipmentCollectionClass>
                                        (x => x.Enumerate(base.SecurityHndlr.Security));
            int insertCount = 0;
            int insertCompartmentCount = 0;

            foreach (Equipment753ToV12Do sourceEquipmentDo in sourceEquipmentList)
            {
                EquipmentClass existingEquipment = this.EquipmentExists(sourceEquipmentDo.Id, ref targetEquipList);

                // Get the matching equipment type Guid based on the source equipment.
                Guid targetEquipmentTypeGuid = this.MapEquipmentTypeForGuid(sourceEquipmentDo);

                if (existingEquipment == null)
                {
                    var targetEquipmentDo = this.PopulateTargetEquipment(sourceEquipmentDo);

                    if (string.IsNullOrEmpty(sourceEquipmentDo.CompanyId) == false)
                    {
                        Guid companyGuid = base.FindCompany(sourceEquipmentDo.CompanyId);

                        if (companyGuid == Guid.Empty)
                        {
                            base.MessageFlag = true;
                            base.Message = base.Message + Environment.NewLine
                                                + "Info: Company ID '" + sourceEquipmentDo.CompanyId + "' for Equipment ID '" + sourceEquipmentDo.Id
                                                + "' could not be found at target site '"
                                                + base.TargetSiteId + "'.";
                        }
                        else
                        {
                            targetEquipmentDo.CompanyGuid = companyGuid;
                        }
                    }

                    if (string.IsNullOrEmpty(sourceEquipmentDo.ProductId) == false)
                    {
                        Guid productGuid = base.FindProduct(sourceEquipmentDo.ProductId);

                        if (productGuid == Guid.Empty)
                        {
                            base.MessageFlag = true;
                            base.Message = base.Message + Environment.NewLine
                                                + "Info: Product ID '" + sourceEquipmentDo.ProductId + "' for Equipment ID '" + sourceEquipmentDo.Id
                                                + "' could not be found at target site '"
                                                + base.TargetSiteId + "'.";
                        }
                        else
                        {
                            targetEquipmentDo.ProductGuid = productGuid;
                        }
                    }

                    if (string.IsNullOrEmpty(sourceEquipmentDo.FuelCardId) == false)
                    {
                        Guid fuelCardGuid = base.FindFuelCard(sourceEquipmentDo.FuelCardId);

                        if (fuelCardGuid == Guid.Empty)
                        {
                            base.MessageFlag = true;
                            base.Message = base.Message + Environment.NewLine
                                                + "Info: Fuel Card ID '" + sourceEquipmentDo.FuelCardId + "' for Equipment ID '" + sourceEquipmentDo.Id
                                                + "' could not be found at target site '"
                                                + base.TargetSiteId + "'.";
                        }
                        else
                        {
                            targetEquipmentDo.FuelCardGuid = fuelCardGuid;
                        }
                    }

                    targetEquipmentDo.TagAndLicenseCollection = this.MapEquipmentQualifications(sourceEquipmentDo
                                                                            , targetEquipmentDo
                                                                            , QualificationMapsBaseDo.QualificationMapTypes.EQUIPMENT_TAG_AND_LICENSE_TO_EQUIPMENT);

                    targetEquipmentDo.TestAndInspectionCollection = this.MapEquipmentQualifications(sourceEquipmentDo
                                                                            , targetEquipmentDo
                                                                            , QualificationMapsBaseDo.QualificationMapTypes.EQUIPMENT_TEST_AND_INSPECTION_TO_EQUIPMENT);

                    if(targetEquipmentTypeGuid == Guid.Empty)
                    {
                        base.MessageFlag = true;
                        base.Message = base.Message + Environment.NewLine
                                            + "Info: Could not find target Equipment Type for source Equipment ID '" + sourceEquipmentDo.Id + "' at Target site '"
                                            + base.TargetSiteId + "'.";
                        continue;
                    }

                    targetEquipmentDo.EquipmentTypeGuid = targetEquipmentTypeGuid;
                    targetEquipmentDo.EqTypeName = sourceEquipmentDo.EqTypeName;

                    // Retrieve all compartments associated to the source equipment.
                    EquipmentCollectionClass equipmentCompartmentList = this.GetCompartments(sourceEquipmentDo, sourceEquipmentCompartmentList);

                    // Set the compartment collection on the target equipment.
                    if(equipmentCompartmentList.Count > 0)
                    {
                        targetEquipmentDo.CompartmentCollection = equipmentCompartmentList;
                    }

                    try
                    {
                        Guid targetEquipGuid = FMChannelHelper.MakeCall<IEquipments, Guid>(x => x.Add(base.SecurityHndlr.Security, targetEquipmentDo));
                        targetEquipmentDo.IdentityGuid = targetEquipGuid;

                        if (targetEquipmentTypeGuid != Guid.Empty)
                        {
                            // Entity assign the associated equipment type
                            base.PerformEntityAssignmentHelper(entityAssignmentProcessor
                                                                , targetEquipmentDo.EquipmentTypeGuid
                                                                , sourceEntitySiteGuid
                                                                , targetEntitySiteGuid
                                                                , typeof(IEquipmentTypes).GUID
                                                                , ENTITY_TYPE.EQUIPMENT_TYPE
                                                                , string.Empty);
                        }

                        // Entity assign the equipment to the target site.
                        string entityMessage = " For Equipment ID: '" + targetEquipmentDo.ID + "' to the target Site: " + this.SourceSiteId + ".";
                        base.PerformEntityAssignmentHelper(entityAssignmentProcessor
                                                            , targetEquipGuid
                                                            , sourceEntitySiteGuid
                                                            , targetEntitySiteGuid
                                                            , typeof(IEquipments).GUID
                                                            , ENTITY_TYPE.EQUIPMENT
                                                            , entityMessage);

                        // If the equipment has compartments, then they also need to be entity assigned down.
                        if(equipmentCompartmentList.Count > 0)
                        {
                            var targetEquipmentCompartmentGuidList = FMChannelHelper.MakeCall<IEquipments, Dictionary<string, Guid>>
                                                                    (x => x.GetEquipmentCompartmentGuids(this.SecurityHndlr.Security, targetEquipGuid));

                            if(targetEquipmentCompartmentGuidList.Count > 0)
                            {
                                foreach(KeyValuePair<string, Guid> equipCompartmentItem in targetEquipmentCompartmentGuidList)
                                {
                                    // Entity assign the equipment to the target site.
                                    entityMessage = " For Equipment Comparment ID: '" + equipCompartmentItem.Key + "' to the target Site: " + this.SourceSiteId + ".";
                                    base.PerformEntityAssignmentHelper(entityAssignmentProcessor
                                                                        , equipCompartmentItem.Value
                                                                        , sourceEntitySiteGuid
                                                                        , targetEntitySiteGuid
                                                                        , typeof(IEquipments).GUID
                                                                        , ENTITY_TYPE.EQUIPMENT
                                                                        , entityMessage);

                                    insertCompartmentCount++;
                                }
                            }
                        }

                        insertCount++;
                    }
                    catch (Exception ex)
                    {
                        base.MessageFlag = true;
                        base.Message = base.Message + Environment.NewLine
                                            + "Error: Adding Equipment ID '" + sourceEquipmentDo.Id + "' to the target DB. " + ex.Message;
                    }
                }
                else
                {
                    base.MessageFlag = true;
                    base.Message = base.Message + Environment.NewLine
                                        + "Info: Equipment ID '" + sourceEquipmentDo.Id + "' already exists at Target site '"
                                        + base.TargetSiteId + "'.";

                    // Entity assign the associated equipment type
                    base.PerformEntityAssignmentHelper(entityAssignmentProcessor
                                                        , targetEquipmentTypeGuid
                                                        , sourceEntitySiteGuid
                                                        , targetEntitySiteGuid
                                                        , typeof(IEquipmentTypes).GUID
                                                        , ENTITY_TYPE.EQUIPMENT_TYPE
                                                        , string.Empty);

                    // Entity assign the equipment to the target site.
                    string entityMessage = " For Equipment ID: '" + sourceEquipmentDo.Id + "' to the target Site: " + this.SourceSiteId + ".";
                    base.PerformEntityAssignmentHelper(entityAssignmentProcessor
                                                        , existingEquipment.IdentityGuid
                                                        , sourceEntitySiteGuid
                                                        , targetEntitySiteGuid
                                                        , typeof(IEquipments).GUID
                                                        , ENTITY_TYPE.EQUIPMENT
                                                        , entityMessage);

                    // Entity assign the existing equipment's compartment to the site.
                    var targetEquipmentCompartmentGuidList = FMChannelHelper.MakeCall<IEquipments, Dictionary<string, Guid>>
                                                                    (x => x.GetEquipmentCompartmentGuids(this.SecurityHndlr.Security, existingEquipment.IdentityGuid));

                    if (targetEquipmentCompartmentGuidList.Count > 0)
                    {
                        foreach (KeyValuePair<string, Guid> equipCompartmentItem in targetEquipmentCompartmentGuidList)
                        {
                            entityMessage = " For Equipment Comparment ID: '" + equipCompartmentItem.Key + "' to the target Site: " + this.SourceSiteId + ".";
                            base.PerformEntityAssignmentHelper(entityAssignmentProcessor
                                                                , equipCompartmentItem.Value
                                                                , sourceEntitySiteGuid
                                                                , targetEntitySiteGuid
                                                                , typeof(IEquipments).GUID
                                                                , ENTITY_TYPE.EQUIPMENT
                                                                , entityMessage);
                        }
                    }
                }
            }

            if (insertCount > 0)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Info: Successfully migrated " + insertCount + " Equipment items.";

                if (insertCompartmentCount > 0)
                {
                    base.Message = base.Message + Environment.NewLine
                                        + "Info: Successfully migrated " + insertCompartmentCount + " Equipment Compartment items.";
                }
            }

            // Reset the site guid to SiteAdmin.
            base.SecurityHndlr.Security.SiteGuid = base.SecurityHndlr.SiteAdminGuid;
        }

        /// <summary>
        /// This method will return a collection of target equipment of type compartments.
        /// </summary>
        /// <param name="sourceEquipmentDo">The current source equipment data object.</param>
        /// <param name="sourceEquipmentCompartmentList">The list of source equipment of type compartments.</param>
        /// <returns>Returns a list of target equipment compartments for a given source equipment.</returns>
        private EquipmentCollectionClass GetCompartments(Equipment753ToV12Do sourceEquipmentDo, List<Equipment753ToV12Do> sourceEquipmentCompartmentList)
        {

            var equipmentCompartmentList = new EquipmentCollectionClass();

            if (sourceEquipmentCompartmentList.Count == 0)
            {
                return equipmentCompartmentList;
            }

            var siConversion = new SIConversion();
            List<Equipment753ToV12Do> compartmentList = sourceEquipmentCompartmentList.FindAll(x => x.EquipmentIndex == sourceEquipmentDo.Index);

            if(compartmentList == null || compartmentList.Count == 0)
            {
                return equipmentCompartmentList;
            }

            foreach(Equipment753ToV12Do sourceEquipmentCompartment in compartmentList)
            {
                EquipmentClass targetEquipment = new EquipmentClass
                {
                    Type = EQUIPMENT_TYPE.COMPARTMENT_TYPE
                    , EquipmentSequence = sourceEquipmentCompartment.EquipmentSequence
                    , IdentityGuid = Guid.Empty
                    , ParentEquipmentGuid = Guid.Empty
                };

                if (sourceEquipmentCompartment.Capacity != null)
                {
                    double siValue = siConversion.ConvertVolumeFromSI((EngineeringUnit)sourceEquipmentDo.VolumeUnitIndex, sourceEquipmentCompartment.Capacity.Value);
                    targetEquipment.Capacity = siValue.ToString();
                }

                if (sourceEquipmentCompartment.SafeFill != null)
                {
                    double siValue = siConversion.ConvertVolumeFromSI((EngineeringUnit)sourceEquipmentDo.VolumeUnitIndex, sourceEquipmentCompartment.SafeFill.Value);
                    targetEquipment.SafeFill = siValue.ToString();
                }

                equipmentCompartmentList.Add(targetEquipment);
            }

            return equipmentCompartmentList;
        }

        /// <summary>
        /// This method will populate the target equipment object with the main information.
        /// </summary>
        /// <param name="sourceEquipmentDo">The source equipment data object.</param>
        /// <returns>Returns the target equipment object.</returns>
        private EquipmentClass PopulateTargetEquipment(Equipment753ToV12Do sourceEquipmentDo)
        {
            var targetEquipmentDo = new EquipmentClass
            {
                ID                          = sourceEquipmentDo.Id
                , SiteGuid                  = targetSiteGuid
                , Description               = sourceEquipmentDo.Description
                , Make                      = sourceEquipmentDo.Make
                , Model                     = sourceEquipmentDo.Model
                , Year                      = sourceEquipmentDo.Year == null ? 0 : sourceEquipmentDo.Year.Value
                , IssPtNum                  = sourceEquipmentDo.IssPtNum
                , Fixed                     = sourceEquipmentDo.Fixed
                , StorageType               = sourceEquipmentDo.StorageType
                , InUse                     = sourceEquipmentDo.InUse
                , FixedVolume               = sourceEquipmentDo.FixedVolume
                , IntoPlane                 = sourceEquipmentDo.IntoPlane
                , Mobile                    = sourceEquipmentDo.Mobile
                , AttachedTo                = sourceEquipmentDo.AttachedTo
                , MediaType                 = sourceEquipmentDo.MediaType.ToString()
                , Meters                    = sourceEquipmentDo.Meters == null ? 0 : sourceEquipmentDo.Meters.Value
                , DefuelMeterForwards       = sourceEquipmentDo.DefuelMeterForwards
                , PulseRatio                = sourceEquipmentDo.PulseRatio == null ? 0.0 : sourceEquipmentDo.PulseRatio.Value
                , Round                     = sourceEquipmentDo.Round
                , Xref                      = sourceEquipmentDo.Xref
                , LowStockWarning           = sourceEquipmentDo.LowStockWarning == null ? string.Empty : sourceEquipmentDo.LowStockWarning.Value.ToString()
                , StockTrack                = sourceEquipmentDo.StockTrack
                , Totalisor1                = sourceEquipmentDo.Totalisor1
                , Totalisor2                = sourceEquipmentDo.Totalisor2
                , FuelingState              = sourceEquipmentDo.FuelingState
                , MeterReading              = sourceEquipmentDo.MeterReading == null ? 0.0 : sourceEquipmentDo.MeterReading.Value
                //, Consecutive_OOS_Variance = sourceEquipmentDo.ConsectiveOosVariance
                , Notes                     = sourceEquipmentDo.Notes
                , VolumeUnits               = sourceEquipmentDo.VolumeUnitIndex == null ? EngineeringUnit.FmvUsGal : (EngineeringUnit)sourceEquipmentDo.VolumeUnitIndex.Value
                , TemperatureUnits          = sourceEquipmentDo.TemperatureUnitIndex == null ? EngineeringUnit.FmtDegF : (EngineeringUnit)sourceEquipmentDo.TemperatureUnitIndex.Value
                , DensityUnits              = sourceEquipmentDo.DensityUnitIndex == null ? EngineeringUnit.FmdUsLbGal : (EngineeringUnit)sourceEquipmentDo.DensityUnitIndex.Value
                , MassUnits                 = sourceEquipmentDo.MassUnitIndex == null ? EngineeringUnit.FmmLb : (EngineeringUnit)sourceEquipmentDo.MassUnitIndex.Value
                , VolumeDecimalPlaces       = sourceEquipmentDo.VolumeDecimalPlaces == null ? (short)0 : sourceEquipmentDo.VolumeDecimalPlaces.Value
                , TemperatureDecimalPlaces  = sourceEquipmentDo.TemperatureDecimalPlaces == null ? (short)0 : sourceEquipmentDo.TemperatureDecimalPlaces.Value
                , DensityDecimalPlaces      = sourceEquipmentDo.DensityDecimalPlaces == null ? (short)0 : sourceEquipmentDo.DensityDecimalPlaces.Value
                , MassDecimalPlaces         = sourceEquipmentDo.MassDecimalPlaces == null ? (short)0 : sourceEquipmentDo.MassDecimalPlaces.Value
                , EquipmentSequence         = sourceEquipmentDo.EquipmentSequence
                , LockedOut                 = sourceEquipmentDo.LockedOut
                , LockedOutDate             = sourceEquipmentDo.LockedOutDate == null ? string.Empty : sourceEquipmentDo.LockedOutDate.Value.ToString()
                , LockedOutReason           = sourceEquipmentDo.LockedOutReason
                , SerialNumber              = sourceEquipmentDo.SerialNumber
                , CompanyEquipmentID        = sourceEquipmentDo.CompanyEquipmentId
                , TruckCardNumber           = sourceEquipmentDo.TruckCardNumber
                , RatedGPM                  = sourceEquipmentDo.RatedGpm == null ? 0.0 : sourceEquipmentDo.RatedGpm.Value
                , ActualGPM                 = sourceEquipmentDo.ActualGpm == null ? 0.0 : sourceEquipmentDo.ActualGpm.Value
                , FuelAdditiveFlag          = sourceEquipmentDo.FuelAdditiveFlag
                , ManufactureDate           = sourceEquipmentDo.ManufactureDate == null ? string.Empty : sourceEquipmentDo.ManufactureDate.Value.ToString()
                , InstallationDate          = sourceEquipmentDo.InstallationDate == null ? string.Empty : sourceEquipmentDo.InstallationDate.Value.ToString()
                , InspectionDate            = sourceEquipmentDo.InspectionDate == null ? string.Empty : sourceEquipmentDo.InspectionDate.Value.ToString()
                , CalibrationDate           = sourceEquipmentDo.CalibrationDate == null ? string.Empty : sourceEquipmentDo.CalibrationDate.Value.ToString()
                , QCDate                    = sourceEquipmentDo.QcDate == null ? string.Empty : sourceEquipmentDo.QcDate.Value.ToString()
                , SecondaryStorageFlag      = sourceEquipmentDo.SecondaryStorageFlag
                , ManagedEquipmentFlag      = sourceEquipmentDo.ManagedEquipmentFlag
                , FuelingType               = sourceEquipmentDo.FuelingType == null ? FUELING_TYPES.NONE : (FUELING_TYPES)sourceEquipmentDo.FuelingType
                , UserData1                 = sourceEquipmentDo.UserData1
                , UserData2                 = sourceEquipmentDo.UserData2
                , UserData3                 = sourceEquipmentDo.UserData3
                , UserData4                 = sourceEquipmentDo.UserData4
                , UserData5                 = sourceEquipmentDo.UserData5
                , UserData6                 = sourceEquipmentDo.UserData6
                , UserData7                 = sourceEquipmentDo.UserData7
                , UserData8                 = sourceEquipmentDo.UserData8
                , UserData9                 = sourceEquipmentDo.UserData9
                , UserData10                = sourceEquipmentDo.UserData10
                , UserData11                = sourceEquipmentDo.UserData11
                , UserData12                = sourceEquipmentDo.UserData12
                , UserData13                = sourceEquipmentDo.UserData13
                , UserData14                = sourceEquipmentDo.UserData14
                , UserData15                = sourceEquipmentDo.UserData15
                , UserData16                = sourceEquipmentDo.UserData16
                , UserData17                = sourceEquipmentDo.UserData17
                , UserData18 = sourceEquipmentDo.UserData18
                , UserData19 = sourceEquipmentDo.UserData19
                , UserData20 = sourceEquipmentDo.UserData20
                , UserData21 = sourceEquipmentDo.UserData21
                , UserData22 = sourceEquipmentDo.UserData22
                , UserData23 = sourceEquipmentDo.UserData23
                , UserData24 = sourceEquipmentDo.UserData24
                , ScullyRequired = sourceEquipmentDo.ScullyRequired
                , CreatedBy = "Migration Tool"
                , UpdatedBy = "Migration Tool"
                , CreatedDate = DateTimeOffset.Now
                , UpdatedDate = DateTimeOffset.Now
            };

            var siConversion = new SIConversion();

            if (sourceEquipmentDo.Volume != null)
            {
                double siValue = siConversion.ConvertVolumeFromSI(targetEquipmentDo.VolumeUnits, sourceEquipmentDo.Volume.Value);
                targetEquipmentDo.Volume = siValue.ToString();
            }

            if (sourceEquipmentDo.Capacity != null)
            {
                double siValue = siConversion.ConvertVolumeFromSI(targetEquipmentDo.VolumeUnits, sourceEquipmentDo.Capacity.Value);
                targetEquipmentDo.Capacity = siValue.ToString();
            }

            if (sourceEquipmentDo.SafeFill != null)
            {
                double siValue = siConversion.ConvertVolumeFromSI(targetEquipmentDo.VolumeUnits, sourceEquipmentDo.SafeFill.Value);
                targetEquipmentDo.SafeFill = siValue.ToString();
            }

            return targetEquipmentDo;
        }

        /// <summary>
        /// This method is a helper to find if an existing equipment already exists at the
        /// target database.
        /// </summary>
        /// <param name="soureEquipmentId">The source equipment ID</param>
        /// <param name="targetEquipmentList">The target equipment list.</param>
        /// <returns>Return null if the equipment does not exist at the target DB. Otherwise, returns the equipment class.</returns>
        private EquipmentClass EquipmentExists(string soureEquipmentId, ref EquipmentCollectionClass targetEquipmentList)
        {
            if (targetEquipmentList == null || targetEquipmentList.Count <= 0)
            {
                return null;
            }

            EquipmentClass targetEquipment = targetEquipmentList.Find(x => x.ID.ToUpper() == soureEquipmentId.ToUpper());

            if (targetEquipment == null)
            {
                return null;
            }

            return targetEquipment;
        }

        /// <summary>
        /// This method is a helper to retrieve the equipment type type for the target
        /// object.
        /// </summary>
        /// <param name="sourceType">The equipment type source type.</param>
        /// <returns>Return the Equipment Type Type for the target object.</returns>
        private EQUIPMENT_TYPE EquipmentTypeTypeHelper(int sourceType)
        {
            switch (sourceType)
            {
                case 0:
                    return EQUIPMENT_TYPE.TRAILER_TYPE;
                case 1:
                    return EQUIPMENT_TYPE.TRACTOR_TYPE;
                case 2:
                    return EQUIPMENT_TYPE.AIRCRAFT_TYPE;
                case 3:
                    return EQUIPMENT_TYPE.RAILCAR_TYPE;
                case 4:
                    return EQUIPMENT_TYPE.BARGE_TYPE;
                case 5:
                    return EQUIPMENT_TYPE.COMPARTMENT_TYPE;
                case 6:
                    return EQUIPMENT_TYPE.SHIP_TYPE;
                case 7:
                    return EQUIPMENT_TYPE.PIPELINE_TYPE;
                case 8:
                    return EQUIPMENT_TYPE.HYDRANT_CART_TYPE;
                case 9:
                    return EQUIPMENT_TYPE.TANKER_TYPE;
                case 10:
                    return EQUIPMENT_TYPE.STATIONARY_CART_TYPE;
                case 11:
                    return EQUIPMENT_TYPE.OTHER_TYPE;
                case 12:
                    return EQUIPMENT_TYPE.SYSTEM_TYPE;
                case 13:
                    return EQUIPMENT_TYPE.TANK_TYPE;
                case 14:
                    return EQUIPMENT_TYPE.FILLSTAND_TYPE;
                case 15:
                    return EQUIPMENT_TYPE.CONTAINER;
                case 16:
                    return EQUIPMENT_TYPE.VEHICLE;
                case 17:
                    return EQUIPMENT_TYPE.INFRASTRUCTURE;
                case 18:
                    return EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE;
                default:
                    return EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE;
            }
        }

        /// <summary>
        /// This method will map the Equipment test & inspection and tag & licence maps.
        /// </summary>
        /// <param name="sourceEquipmentDo">The source equipment data object.</param>
        /// <param name="equipmentDo">The target equipment data object.</param>
        /// <param name="mapType">The equipment map type.</param>
        /// <returns>Returns the equipment qualification map collection.</returns>
        private QualificationMapCollectionClass MapEquipmentQualifications(Equipment753ToV12Do sourceEquipmentDo
                                                                        , EquipmentClass equipmentDo
                                                                        , QualificationMapsBaseDo.QualificationMapTypes mapType)
        {
            int equipmentMapType = (int)mapType;

            QualificationMapCollectionClass equipmentQualificationCollection = new QualificationMapCollectionClass();

            if (this.qualificationMapsList.Count == 0)
            {
                return equipmentQualificationCollection;
            }

            List<QualificationMapsBaseDo> qualificationMapsBaseList =
                                            this.qualificationMapsList.FindAll(x => x.Index == sourceEquipmentDo.Index
                                            && x.Type == equipmentMapType);

            if (qualificationMapsBaseList == null || qualificationMapsBaseList.Count == 0)
            {
                return equipmentQualificationCollection;
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
                    ID = qualificationMaps753ToV12Do.Id
                    , Number = qualificationMaps753ToV12Do.Id
                    , Type = (QUALIFICATION_MAP_TYPE)qualificationMaps753ToV12Do.Type
                    , Rating = qualificationMaps753ToV12Do.Rating
                    , HistoricalRecord = qualificationMaps753ToV12Do.HistoricalRecord
                    , Instructor = qualificationMaps753ToV12Do.Instructor
                    , SiteGuid = equipmentDo.SiteGuid
                    , SiteID = equipmentDo.SiteID
                    , AssignedGuid = targetQualificationGuid
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

                equipmentQualificationCollection.Add(targetQualificationMap);
            }

            return equipmentQualificationCollection;
        }

        /// <summary>
        /// This method will map the equipment type between the source and target DBs to get the
        /// equipment type Guid for the source equipment.
        /// </summary>
        /// <param name="sourceEquipmentDo">The source equipment data object.</param>
        /// <returns>Returns the equipment type GUID or empty Guid if not found.</returns>
        private Guid MapEquipmentTypeForGuid(Equipment753ToV12Do sourceEquipmentDo)
        {
            EquipmentType753ToV12Do sourceEquipmentType = (EquipmentType753ToV12Do)this.equipmentTypeMapsList.Find(x => x.EqTypeIndex == sourceEquipmentDo.EqTypeIndex);
            
            if(sourceEquipmentType == null)
            {
                return Guid.Empty;
            }

            EquipmentTypeClass targetEquipmentType = this.targetEquipmentTypeCollection.Find(x => x.ID.ToUpper() == sourceEquipmentType.EqTypeName.ToUpper());

            if(targetEquipmentType == null)
            {
                return Guid.Empty;
            }

            return targetEquipmentType.IdentityGuid;
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

        /// <summary>
        /// This method will retrieve the target target equipment type data.
        /// </summary>
        private void GetTargetEquipmentTypes()
        {
            this.SecurityHndlr.Security.SiteGuid = this.targetSiteGuid;
            this.SecurityHndlr.Security.SiteID = base.TargetSiteId;

            this.targetEquipmentTypeCollection =
                            FMChannelHelper.MakeCall<IEquipmentTypes, EquipmentTypeCollectionClass>(x => x.Enumerate(this.SecurityHndlr.Security, null, null));

            this.SecurityHndlr.Security.SiteGuid = this.SecurityHndlr.SiteAdminGuid;
            this.SecurityHndlr.Security.SiteID = "SiteAdmin";
        }
        #endregion
    }
}
