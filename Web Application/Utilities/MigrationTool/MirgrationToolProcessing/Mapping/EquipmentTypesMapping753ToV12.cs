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

    public class EquipmentTypesMapping753ToV12 : EquipmentTypesMappingBase
    {
        #region Data members
        private List<QualificationMapsBaseDo> qualificationMapsList;
        private QualificationCollectionClass targetQualificationCollection;
        private Guid targetSiteGuid;
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public EquipmentTypesMapping753ToV12()
        {
            base.Init();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// This method implements the Perform Mapping for equipment types.
        /// </summary>
        /// <param name="equipmentTypeDo">The equipment type data object to map.</param>
        /// <param name="migrationDA">The Migration data access object.</param>
        public override void PerformMapping(EquipmentTypeBaseDo equipmentTypeDo, MigrationDatabaseDAClass migrationDA)
        {
            base.MessageFlag = false;
            base.Message = string.Empty;

            var equipmentTypeDoList = new List<EquipmentType753ToV12Do>();
            EquipmentType753ToV12Do equipmentType = equipmentTypeDo as EquipmentType753ToV12Do;
            DataSet sourceDataSet = null;

            if (string.IsNullOrEmpty(base.SourceSiteId) || string.IsNullOrEmpty(base.TargetSiteId))
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Error: Source Site ID or Target Site ID is null.";
                return;
            }

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
                equipmentType.EnumerateEquipmentTypesSql(command);
                sourceDataSet = migrationDA.GetDataSet(command);
            }

            if (sourceDataSet == null || sourceDataSet.Tables.Count == 0 || sourceDataSet.Tables[0].Rows.Count == 0)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Warning: No Equipment Types found in the 7.5.3 " + equipmentType.SourceDbName + " database.";
                return;
            }

            DataTable table = sourceDataSet.Tables[0];
            foreach (DataRow row in table.Rows)
            {
                var newEquipmentType = new EquipmentType753ToV12Do();
                newEquipmentType.Load(row);
                equipmentTypeDoList.Add(newEquipmentType);
            }

            if (equipmentTypeDoList.Count == 0)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Warning: No Equipment Types found in the 7.5.3 " + equipmentType.SourceDbName + " database.";
                return;
            }

            // Get the list of source DB qualification maps.
            var sourceDbQualificationMapping = new SourceDbQualificationMapping753ToV12();
            sourceDbQualificationMapping.GetSourceQualificationMaps(migrationDA);
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
                                    + "Error: Could not get the target site GUID for site: " + base.TargetSiteId + ". " + ex.Message;
                return;
            }

            this.MapEquipmentTypes(equipmentTypeDoList);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method maps the source equipment types to the target.
        /// </summary>
        /// <param name="equipmentTypesList">The list of source equipment types.</param>
        private void MapEquipmentTypes(List<EquipmentType753ToV12Do> equipmentTypesList)
        {
            var entityAssignmentProcessor = new EntityService.EntityAssignmentProcessor(this.SecurityHndlr);

            // This is the target site for entity assignment of equipment types.
            var targetEntitySiteGuid = FMChannelHelper.MakeCall<ISites, Guid>(x => x.GetIdentityGuid(this.SecurityHndlr.Security, "CITGO"));

            // For equipment types, the target site must be Site Admin!
            Guid targetSiteGuid = this.SecurityHndlr.SiteAdminGuid;

            // Set the target site for the migration.
            base.SecurityHndlr.Security.SiteGuid = targetSiteGuid;

            // Get the list of target equipment types to be used to check if already exists.
            var targetEquipTypesList = FMChannelHelper.MakeCall<IEquipmentTypes, EquipmentTypeCollectionClass>
                                        (x => x.Enumerate(base.SecurityHndlr.Security, string.Empty, string.Empty));
            int insertCount = 0;

            foreach (EquipmentType753ToV12Do sourceEquipmentTypeDo in equipmentTypesList)
            {
                EquipmentTypeClass existingEquipmentType = this.EquipmentTypeExists(sourceEquipmentTypeDo.EqTypeName, ref targetEquipTypesList);

                if (existingEquipmentType == null)
                {
                    var targetEquipmentTypeDo = new EquipmentTypeClass
                    {
                        ID                      = sourceEquipmentTypeDo.EqTypeName
                        , SiteGuid              = targetSiteGuid
                        , Description           = sourceEquipmentTypeDo.EqTypeDescription
                        , Make                  = sourceEquipmentTypeDo.Make
                        , Model                 = sourceEquipmentTypeDo.Model
                        , Attribute             = sourceEquipmentTypeDo.Attribute == null ? EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE: this.EquipmentTypeTypeHelper(sourceEquipmentTypeDo.Attribute.Value)
                        , Deleted               = sourceEquipmentTypeDo.DeleteFlag
                        , Isspt                 = sourceEquipmentTypeDo.IssPt
                        , IsMultiCompartment    = sourceEquipmentTypeDo.MultiCompartment
                        , Year                  = sourceEquipmentTypeDo.Year == null ? 0 : sourceEquipmentTypeDo.Year.Value
                        , CreatedBy             = "Migration Tool"
                        , UpdatedBy             = "Migration Tool"
                        , CreatedDate           = DateTimeOffset.Now
                        , UpdatedDate           = DateTimeOffset.Now
                    }; 
                    
                    var siConversion = new SIConversion();

                    targetEquipmentTypeDo.Capacity = "0";

                    if (sourceEquipmentTypeDo.Capacity != null)
                    {
                        double siValue = siConversion.ConvertVolumeFromSI(sourceEquipmentTypeDo.VolumeUnitIndex, sourceEquipmentTypeDo.Capacity.Value);
                        targetEquipmentTypeDo.Capacity = siValue.ToString();
                    }

                    targetEquipmentTypeDo.SafeFill = "0";

                    if (sourceEquipmentTypeDo.SafeFill != null)
                    {
                        double siValue = siConversion.ConvertVolumeFromSI(sourceEquipmentTypeDo.VolumeUnitIndex, sourceEquipmentTypeDo.SafeFill.Value);
                        targetEquipmentTypeDo.SafeFill = siValue.ToString();
                    }

                    targetEquipmentTypeDo.ReqQualificationsCollection = this.MapEquipmentQualifications(sourceEquipmentTypeDo
                                                                            , targetEquipmentTypeDo
                                                                            , QualificationMapsBaseDo.QualificationMapTypes.PERSON_QUALIFICATION_TO_EQUIPMENT_TYPE);

                    targetEquipmentTypeDo.ReqTrainingCollection = this.MapEquipmentQualifications(sourceEquipmentTypeDo
                                                                            , targetEquipmentTypeDo
                                                                            , QualificationMapsBaseDo.QualificationMapTypes.PERSON_TRAINING_TO_EQUIPMENT_TYPE);

                    try
                    {
                        Guid targetEquipTypeGuid = FMChannelHelper.MakeCall<IEquipmentTypes, Guid>(x => x.Add(base.SecurityHndlr.Security, targetEquipmentTypeDo));

                        // Assign the equipment type entity from site admin to CITGO.
                        string entityMessage = " For Equipment Type ID: '" + targetEquipmentTypeDo.ID + "' to the target Site: " + this.SourceSiteId + ".";
                        base.PerformEntityAssignmentHelper(entityAssignmentProcessor
                                                            , targetEquipTypeGuid
                                                            , this.SecurityHndlr.SiteAdminGuid
                                                            , targetEntitySiteGuid
                                                            , typeof(IEquipmentTypes).GUID
                                                            , ENTITY_TYPE.EQUIPMENT_TYPE
                                                            , entityMessage);

                        insertCount++;
                    }
                    catch (Exception ex)
                    {
                        base.MessageFlag = true;
                        base.Message = base.Message + Environment.NewLine
                                            + "Info: Adding Equipment Type Name '" + sourceEquipmentTypeDo.EqTypeName + "' to the target DB. " + ex.Message;
                    }
                }
                else
                {
                    base.MessageFlag = true;
                    base.Message = base.Message + Environment.NewLine
                                        + "Info: Equipment Type Name '" + sourceEquipmentTypeDo.EqTypeName + "' already exists at Target site 'SiteAdmin'.";

                    // Assign the equipment type entity from site admin to CITGO.
                    string entityMessage = " For Equipment Type ID: '" + sourceEquipmentTypeDo.EqTypeName + "' to the target Site: " + this.SourceSiteId + ".";
                    base.PerformEntityAssignmentHelper(entityAssignmentProcessor
                                                        , existingEquipmentType.IdentityGuid
                                                        , this.SecurityHndlr.SiteAdminGuid
                                                        , targetEntitySiteGuid
                                                        , typeof(IEquipmentTypes).GUID
                                                        , ENTITY_TYPE.EQUIPMENT_TYPE
                                                        , entityMessage);
                }
            }

            if (insertCount > 0)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Info: Successfully migrated " + insertCount + " Equipment Type items.";
            }
        }

        /// <summary>
        /// This method is a helper to find if an existing equipment types already exists at the
        /// target database.
        /// </summary>
        /// <param name="soureEquipmentTypeId">The source equipment type ID</param>
        /// <param name="targetEquipTypesList">The target equipment type list.</param>
        /// <returns>Return null if the equipment type does not exist at the target DB. Otherwise, returns the equipment type class.</returns>
        private EquipmentTypeClass EquipmentTypeExists(string soureEquipmentTypeId, ref EquipmentTypeCollectionClass targetEquipTypesList)
        {
            if (targetEquipTypesList == null || targetEquipTypesList.Count <= 0)
            {
                return null;
            }

            EquipmentTypeClass targetEquipmentType = targetEquipTypesList.Find(x => x.ID.ToUpper() == soureEquipmentTypeId.ToUpper());

            if (targetEquipmentType == null)
            {
                return null;
            }

            return targetEquipmentType;
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
        /// This method will map the Equipment type maps.
        /// </summary>
        /// <param name="sourceEquipmentTypeDo">The source equipment type data object.</param>
        /// <param name="equipmentTypeDo">The target equipment type data object.</param>
        /// <param name="mapType">The equipment Type map type.</param>
        /// <returns>Returns the equipment tyep qualification map collection.</returns>
        private QualificationMapCollectionClass MapEquipmentQualifications(EquipmentType753ToV12Do sourceEquipmentTypeDo
                                                                        , EquipmentTypeClass equipmentTypeDo
                                                                        , QualificationMapsBaseDo.QualificationMapTypes mapType)
        {
            int equipmentTypeMapType = (int)mapType;

            QualificationMapCollectionClass equipmentQualificationCollection = new QualificationMapCollectionClass();

            if (this.qualificationMapsList.Count == 0)
            {
                return equipmentQualificationCollection;
            }

            List<QualificationMapsBaseDo> qualificationMapsBaseList =
                                            this.qualificationMapsList.FindAll(x => x.Index == sourceEquipmentTypeDo.EqTypeIndex
                                            && x.Type == equipmentTypeMapType);

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
                    ID                  = qualificationMaps753ToV12Do.Id,
                    Number              = qualificationMaps753ToV12Do.Id,
                    Type                = (QUALIFICATION_MAP_TYPE)qualificationMaps753ToV12Do.Type,
                    Rating              = qualificationMaps753ToV12Do.Rating,
                    HistoricalRecord    = qualificationMaps753ToV12Do.HistoricalRecord,
                    Instructor          = qualificationMaps753ToV12Do.Instructor,
                    SiteGuid            = equipmentTypeDo.SiteGuid,
                    SiteID              = equipmentTypeDo.SiteID,
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

                equipmentQualificationCollection.Add(targetQualificationMap);
            }

            return equipmentQualificationCollection;
        }
        #endregion
    }
}
