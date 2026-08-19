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

    public class ApplicationStringMapping753ToV12 : ApplicationStringMappingBase
    {
        #region data members
        private MigrationDatabaseDAClass migrationDA;
        private Guid targetSiteGuid;
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public ApplicationStringMapping753ToV12()
        {
            base.Init();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// This method implements the Perform Mapping for application strings.
        /// </summary>
        /// <param name="applicationStringDo">The application string data object to map.</param>
        /// <param name="migrationDA">The Migration data access object.</param>
        public override void PerformMapping(ApplicationStringBaseDo applicationStringDo, MigrationDatabaseDAClass migrationDA)
        {
            this.migrationDA = migrationDA;

            base.MessageFlag = false;
            base.Message = string.Empty;

            var applicationStringDoList = new List<ApplicationString753ToV12Do>();
            var applicationString = applicationStringDo as ApplicationString753ToV12Do;
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
                applicationStringDo.GetSiteIndexByIdSql(command, base.SourceSiteId);
                DataSet dataSet = this.migrationDA.GetDataSet(command);
                sourceSiteIndex = applicationStringDo.GetSiteIndex(dataSet);
            }

            if (sourceSiteIndex == null)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Error: Source Site Index is not found.";
                return;
            }

            // Get Target site info
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
                applicationString.EnumerateSourceApplicationStringSql(command, sourceSiteIndex.Value);
                sourceDataSet = this.migrationDA.GetDataSet(command);
            }

            if (sourceDataSet == null || sourceDataSet.Tables.Count == 0 || sourceDataSet.Tables[0].Rows.Count == 0)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Warning: No Application Strings found in the 7.5.3 " + applicationString.SourceDbName + " database.";
                return;
            }

            foreach (DataRow row in sourceDataSet.Tables[0].Rows)
            {
                var newApplicationString = new ApplicationString753ToV12Do();
                newApplicationString.Load(row);
                applicationStringDoList.Add(newApplicationString);
            }

            if (applicationStringDoList.Count == 0)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Warning: No Application Strings found in the 7.5.3 " + applicationString.SourceDbName + " database.";
                return;
            }

            this.MapApplicationStringConnections(applicationStringDoList);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method maps the source application string to the target.
        /// </summary>
        /// <param name="applicationStringList">The list of source application strings.</param>
        private void MapApplicationStringConnections(List<ApplicationString753ToV12Do> applicationStringList)
        {
            // Set the target site for the migration.
            base.SecurityHndlr.Security.SiteGuid = this.targetSiteGuid;

            int insertCount = 0;

            foreach (ApplicationString753ToV12Do sourceApplicationStringDo in applicationStringList)
            {
                bool appStrExists = this.ApplicationStringExists(sourceApplicationStringDo.ID, (STRING_TYPE)sourceApplicationStringDo.Type);

                if (appStrExists == false)
                {
                    var targetApplicationStringDo = new ApplicationStringClass
                    {
                        ID = sourceApplicationStringDo.ID,
                        Type = (STRING_TYPE)sourceApplicationStringDo.Type,
                        SiteGuid = targetSiteGuid,
                        CreatedBy = "Migration Tool",
                        UpdatedBy = "Migration Tool",
                        CreatedDate = DateTimeOffset.Now,
                        UpdatedDate = DateTimeOffset.Now
                    };

                    try
                    {
                        Guid targetApplicationStringGuid = FMChannelHelper.MakeCall<IApplicationStrings, Guid>(x => x.Add(base.SecurityHndlr.Security, targetApplicationStringDo));
                        targetApplicationStringDo.IdentityGuid = targetApplicationStringGuid;
                        insertCount++;
                    }
                    catch (Exception ex)
                    {
                        base.MessageFlag = true;
                        base.Message = base.Message + Environment.NewLine
                                            + "Error: Adding Allication String for ID '" + sourceApplicationStringDo.ID + "' to the target DB. " + ex.Message;

                        base.SecurityHndlr.Security.SiteGuid = base.SecurityHndlr.SiteAdminGuid;
                    }
                }
                else
                {
                    base.MessageFlag = true;
                    base.Message = base.Message + Environment.NewLine
                                        + "Info: Application String with ID '" + sourceApplicationStringDo.ID + "' already exists at target DB.";
                }
            }

            base.SecurityHndlr.Security.SiteGuid = base.SecurityHndlr.SiteAdminGuid;

            if (insertCount > 0)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Info: Successfully migrated " + insertCount + " Application String items.";
            }
        }

        /// <summary>
        /// This method will check to see if the application string already exists.
        /// </summary>
        /// <param name="applicationStringId">The application string ID to search.</param>
        /// <param name="applicationStringType">The application string type to search.</param>
        /// <returns>Returns True if exists, otherwise returns false.</returns>
        private bool ApplicationStringExists(string applicationStringId, STRING_TYPE applicationStringType)
        {
            try
            {
                var applicationStringGuid = FMChannelHelper.MakeCall<IApplicationStrings, Guid>(
                                                x => x.GetIdentityGuid(this.SecurityHndlr.Security, applicationStringType, applicationStringId));

                if(applicationStringGuid != Guid.Empty)
                {
                    return true;
                }
            }
            catch(Exception)
            {
                return false;
            }

            return false;
        }
        #endregion
    }
}
