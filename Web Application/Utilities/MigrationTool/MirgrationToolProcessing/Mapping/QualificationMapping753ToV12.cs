namespace MirgrationToolProcessing.Mapping
{
    using BusinessObjects.MigrationObjects;
    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using MigrationToolDataAccessLayer;
    using MirgrationToolProcessing.EntityService;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    public class QualificationMapping753ToV12 : QualificationMappingBase
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public QualificationMapping753ToV12()
        {
            base.Init();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// This method implements the Perform Mapping for qualification.
        /// </summary>
        /// <param name="qualificationDo">The qualification data object to map.</param>
        /// <param name="migrationDA">The Migration data access object.</param>
        public override void PerformMapping(QualificationBaseDo qualificationDo, MigrationDatabaseDAClass migrationDA)
        {
            base.MessageFlag = false;
            base.Message = string.Empty;

            var qualificationDoList = new List<Qualification753ToV12Do>();
            Qualification753ToV12Do qualification = qualificationDo as Qualification753ToV12Do;
            DataSet sourceDataSet = null;

            if(string.IsNullOrEmpty(base.SourceSiteId) || string.IsNullOrEmpty(base.TargetSiteId))
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Error: Source Site ID or Target Site ID is null.";
                return;
            }

            int? sourceSiteIndex = null;

            using (var command = new SqlCommand())
            {
                qualification.GetSiteIndexByIdSql(command, base.SourceSiteId);
                DataSet dataSet = migrationDA.GetDataSet(command);
                sourceSiteIndex = qualification.GetSiteIndex(dataSet);
            }

            if(sourceSiteIndex == null)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Error: Source Site Index is not found.";
                return;
            }

            using (var command = new SqlCommand())
            {
                qualification.EnumerateQuantitiesSql(command, sourceSiteIndex.Value);
                sourceDataSet = migrationDA.GetDataSet(command);
            }

            if (sourceDataSet == null || sourceDataSet.Tables.Count == 0 || sourceDataSet.Tables[0].Rows.Count == 0)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Warning: No Qualification found in the 7.5.3 " + qualificationDo.SourceDbName + " database.";
                return;
            }

            DataTable table = sourceDataSet.Tables[0];
            foreach(DataRow row in table.Rows)
            {
                var newQualification = new Qualification753ToV12Do();
                newQualification.Load(row);
                qualificationDoList.Add(newQualification);
            }

            if(qualificationDoList.Count == 0)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Warning: No Qualification found in the 7.5.3 " + qualificationDo.SourceDbName + " database.";
                return;
            }

            this.MapQualifications(qualificationDoList);
            //var entityAssignmentProcessor = new EntityAssignmentProcessor(base.SecurityHndlr);
            //entityAssignmentProcessor.EntityAssign(ENTITY_TYPE.QUALIFICATION_PERSON_QUALIFICATION, TargetSiteId, SourceSiteId);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method maps the source qualification to the target.
        /// </summary>
        /// <param name="qualificationList">The list of source qualifications.</param>
        private void MapQualifications(List<Qualification753ToV12Do> qualificationList)
        {
            Guid targetSiteGuid = Guid.Empty;

            try
            {
                targetSiteGuid = FMChannelHelper.MakeCall<ISites, Guid>(x => x.GetIdentityGuid(base.SecurityHndlr.Security, base.TargetSiteId));
            }
            catch(Exception ex)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Error: Retrieving target site GUID for ID '" + base.TargetSiteId + "'. " + ex.Message;
                return;
            }

            if(targetSiteGuid == Guid.Empty)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Error: Target Site GUID is not found.";
                return;
            }

            // Set the target site for the migration.
            base.SecurityHndlr.Security.SiteGuid = base.SecurityHndlr.GetSiteGuidById(base.TargetSiteId);

            // Get the list of target qualification to be used to check if already exists.
            var targetQualList = FMChannelHelper.MakeCall<IQualifications, QualificationCollectionClass>(x => x.Enumerate(base.SecurityHndlr.Security));
            int insertCount = 0;
            
            foreach (Qualification753ToV12Do sourceQualificationDo in qualificationList)
            {
                bool qualificationExist = this.QualificationExists(sourceQualificationDo.Id, ref targetQualList);

                if (qualificationExist == false)
                {
                    var targetQualificationDo = new QualificationClass
                    {
                        ID = sourceQualificationDo.Id
                        , Description = sourceQualificationDo.Description
                        , Duration = sourceQualificationDo.Duration
                        , Reoccurrence = sourceQualificationDo.Reoccurance
                        , SiteGuid = targetSiteGuid
                        , Type = this.QualificationTypeHelper(sourceQualificationDo.Type)
                        , CreatedBy = "Migration Tool"
                        , UpdatedBy = "Migration Tool"
                        , CreatedDate = DateTimeOffset.Now
                        , UpdatedDate = DateTimeOffset.Now
                    };

                    try
                    {
                        Guid targetQualGuid = FMChannelHelper.MakeCall<IQualifications, Guid>(x => x.Add(base.SecurityHndlr.Security, targetQualificationDo));
                        insertCount++;
                    }
                    catch(Exception ex)
                    {
                        base.MessageFlag = true;
                        base.Message = base.Message + Environment.NewLine
                                            + "Error: Adding qualification ID '" + sourceQualificationDo.Id + "' to the target DB. " + ex.Message;
                    }
                }
                else
                {
                    base.MessageFlag = true;
                    base.Message = base.Message + Environment.NewLine
                                        + "Info: Qualification ID '" + sourceQualificationDo.Id + "' already exists at Target site '" 
                                        + base.TargetSiteId + "'.";
                }
            }

            if(insertCount > 0)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Info: Successfully migrated " + insertCount + " Qualification items.";
            }

            // Reset the site guid to SiteAdmin.
            base.SecurityHndlr.Security.SiteGuid = base.SecurityHndlr.SiteAdminGuid;
        }

        /// <summary>
        /// This method is a helper to find if an existing qualification already exists at the
        /// target database.
        /// </summary>
        /// <param name="soureQualificationId">The source qualification ID</param>
        /// <param name="targetQualList">The target qualification list.</param>
        /// <returns>Return false if the qualification does not exist at the target DB. Otherwise, returns false.</returns>
        private bool QualificationExists(string soureQualificationId, ref QualificationCollectionClass targetQualList)
        {
            if (targetQualList == null || targetQualList.Count <= 0)
            {
                return false;
            }

            QualificationClass targetQualification = targetQualList.Find(x => x.ID == soureQualificationId);

            if(targetQualification == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// This method is a helper to retrieve the qualification type for the target
        /// object.
        /// </summary>
        /// <param name="sourceType">The qualification source type.</param>
        /// <returns>Return the Qualification Type for the target object.</returns>
        private QUALIFICATION_TYPE QualificationTypeHelper(int sourceType)
        {
            switch(sourceType)
            {
                case 0:
                    return QUALIFICATION_TYPE.COMPANY_CERTIFICATE_AND_PERMIT;
                case 1:
                    return QUALIFICATION_TYPE.EQUIPMENT_TEST_AND_INSPECTION;
                case 2:
                    return QUALIFICATION_TYPE.EQUIPMENT_TAG_AND_LICENSE;
                case 3:
                    return QUALIFICATION_TYPE.PERSON_QUALIFICATION;
                case 4:
                    return QUALIFICATION_TYPE.PERSON_LICENSE;
                case 5:
                    return QUALIFICATION_TYPE.PERSON_TRAINING;
                case 6:
                    return QUALIFICATION_TYPE.MAX_QUALIFICATION_TYPE;
                default:
                    return QUALIFICATION_TYPE.MAX_QUALIFICATION_TYPE;
            }
        }
        #endregion
    }
}
