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

    public class UserDataFieldMapping753ToV12 : UserDataFieldMappingBase
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public UserDataFieldMapping753ToV12()
        {
            base.Init();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// This method implements the Perform Mapping for qualification.
        /// </summary>
        /// <param name="userDataFieldsBaseDo">The user data fields object to map.</param>
        /// <param name="migrationDA">The Migration data access object.</param>
        public override void PerformMapping(UserDataFieldsBaseDo userDataFieldsBaseDo, MigrationDatabaseDAClass migrationDA)
        {
            base.MessageFlag = false;
            base.Message = string.Empty;

            var userDataFieldsDoList = new List<UserDataFields753ToV12Do>();
            UserDataFields753ToV12Do userDataFieldsDo = userDataFieldsBaseDo as UserDataFields753ToV12Do;
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
                userDataFieldsDo.GetSiteIndexByIdSql(command, base.SourceSiteId);
                DataSet dataSet = migrationDA.GetDataSet(command);
                sourceSiteIndex = userDataFieldsDo.GetSiteIndex(dataSet);
            }

            if (sourceSiteIndex == null)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Error: Source Site Index is not found.";
                return;
            }

            // Get the User Data Fields
            using (var command = new SqlCommand())
            {
                var userDataEntityTypeStr = userDataFieldsDo.GetEntityTypeAsString(base.UserDataEntityType);
                userDataFieldsDo.EnumerateUserDataFieldsSql(command, sourceSiteIndex.Value, userDataEntityTypeStr);
                sourceDataSet = migrationDA.GetDataSet(command);
            }

            if (sourceDataSet == null || sourceDataSet.Tables.Count == 0 || sourceDataSet.Tables[0].Rows.Count == 0)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Warning: No User Data Fields found in the 7.5.3 for entity type '" + base.UserDataEntityType 
                                    + " at source database " + userDataFieldsDo.SourceDbName + ".";
                return;
            }

            UserDataListValues753ToV12Do userDataListValuesDo = new UserDataListValues753ToV12Do(userDataFieldsDo.SourceDbName, userDataFieldsDo.TargetDbName);

            foreach (DataRow row in sourceDataSet.Tables[0].Rows)
            {
                var newUserDataField = new UserDataFields753ToV12Do();
                newUserDataField.Load(row);
                userDataFieldsDoList.Add(newUserDataField);

                var userDataListValueList = new List<UserDataListValuesBaseDo>();

                // Get the associated user data list values.
                using (var command = new SqlCommand())
                {
                    userDataListValuesDo.EnumerateUserDataListValuesSql(command, newUserDataField.Index);
                    DataSet sourceDataSet2 = migrationDA.GetDataSet(command);

                    if (sourceDataSet2 != null && sourceDataSet2.Tables.Count != 0 && sourceDataSet2.Tables[0].Rows.Count != 0)
                    {
                        foreach (DataRow row2 in sourceDataSet2.Tables[0].Rows)
                        {
                            var newUserDataListValueDo = new UserDataListValues753ToV12Do();
                            newUserDataListValueDo.Load(row2);
                            userDataListValueList.Add(newUserDataListValueDo);
                        }

                        newUserDataField.UserDataListValueList = userDataListValueList;
                    }
                }
            }

            if (userDataFieldsDoList.Count == 0)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Warning: No User Data Fields found in the 7.5.3 " + userDataFieldsDo.SourceDbName + " database.";
                return;
            }

            this.MapUserDataFields(userDataFieldsDoList);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method maps the source user data field to the target.
        /// </summary>
        /// <param name="userDataFieldsList">The list of source user data fields.</param>
        private void MapUserDataFields(List<UserDataFields753ToV12Do> userDataFieldsList)
        {
            Guid targetSiteGuid = Guid.Empty;

            try
            {
                targetSiteGuid = FMChannelHelper.MakeCall<ISites, Guid>(x => x.GetIdentityGuid(base.SecurityHndlr.Security, base.TargetSiteId));
            }
            catch (Exception ex)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Error: Retrieving target site GUID for ID '" + base.TargetSiteId + "'. " + ex.Message;
                return;
            }

            if (targetSiteGuid == Guid.Empty)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Error: Target Site GUID is not found.";
                return;
            }

            // Set the target site for the migration.
            base.SecurityHndlr.Security.SiteGuid = base.SecurityHndlr.GetSiteGuidById(base.TargetSiteId);

            // Get the list of target qualification to be used to check if already exists.
            var targetUserDataFieldList = FMChannelHelper.MakeCall<IUserDataFields, UserDataFieldCollectionClass>
                                            (x => x.Enumerate(base.SecurityHndlr.Security, base.UserDataEntityType));
            int insertCount = 0;

            foreach (UserDataFields753ToV12Do sourceUserDataFieldDo in userDataFieldsList)
            {
                bool userDataExist = this.UserDataExists(sourceUserDataFieldDo.Number, ref targetUserDataFieldList);

                if (userDataExist == false)
                {
                    var targetUserDataFieldDo = new UserDataFieldClass()
                    {
                        Number = sourceUserDataFieldDo.Number,
                        DisplayOrder = sourceUserDataFieldDo.DisplayOrder,
                        DisplayName = sourceUserDataFieldDo.DisplayName,
                        UserDataEntityType = this.EntityTypeHelper(sourceUserDataFieldDo.EntityTypeId),
                        UserDataType = (USER_DATA_TYPE)sourceUserDataFieldDo.Type,
                        SiteGuid = targetSiteGuid,
                        FieldRequired = sourceUserDataFieldDo.Required,
                        DispatchField = false,
                        ClearOnNew = false,
                        CreatedBy = "Migration Tool",
                        UpdatedBy = "Migration Tool",
                        CreatedDate = DateTimeOffset.Now,
                        UpdatedDate = DateTimeOffset.Now
                    };

                    foreach(UserDataListValues753ToV12Do sourceUserDataListValue in sourceUserDataFieldDo.UserDataListValueList)
                    {
                        var targetUserDataListValue = new UserDataListValueClass
                        {
                            ID = sourceUserDataListValue.Value
                        };

                        targetUserDataFieldDo.UserDataListValueCollection.Add(targetUserDataListValue);
                    }

                    try
                    {
                        FMChannelHelper.MakeCall<IUserDataFields>(x => x.Add(base.SecurityHndlr.Security, targetUserDataFieldDo));
                        insertCount++;
                    }
                    catch (Exception ex)
                    {
                        base.MessageFlag = true;
                        base.Message = base.Message + Environment.NewLine
                                            + "Error: Adding User Data Field display name '" + sourceUserDataFieldDo.DisplayName + "' to the target DB. " + ex.Message;
                    }
                }
                else
                {
                    base.MessageFlag = true;
                    base.Message = base.Message + Environment.NewLine
                                        + "Info: User Data Field display name '" + sourceUserDataFieldDo.DisplayName + "' already exists at Target site '"
                                        + base.TargetSiteId + "'.";
                }
            }

            if (insertCount > 0)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Info: Successfully migrated " + insertCount + " User Data Fields items.";
            }

            // Reset the site guid to SiteAdmin.
            base.SecurityHndlr.Security.SiteGuid = base.SecurityHndlr.SiteAdminGuid;
        }

        /// <summary>
        /// This method is a helper to find if an existing user data field already exists at the
        /// target database.
        /// </summary>
        /// <param name="soureUserDataNumber">The source user data number</param>
        /// <param name="targetUserDataList">The target user data field list.</param>
        /// <returns>Return false if the user data field does not exist at the target DB. Otherwise, returns true.</returns>
        private bool UserDataExists(int soureUserDataNumber, ref UserDataFieldCollectionClass targetUserDataList)
        {
            if (targetUserDataList == null || targetUserDataList.Count <= 0)
            {
                return false;
            }

            IEnumerator<FieldClass> fieldClassEnum = targetUserDataList.GetEnumerator();
            while(fieldClassEnum.MoveNext())
            {
                var userDataField = fieldClassEnum.Current as UserDataFieldClass;

                if(userDataField.Number == soureUserDataNumber)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// This method is a helper to retrieve the entity type for the target
        /// object.
        /// </summary>
        /// <param name="entityType">The entity source type.</param>
        /// <returns>Return the entity Type for the target object.</returns>
        private ENTITY_TYPE EntityTypeHelper(string entityType)
        {
            switch (entityType)
            {
                case UserDataFieldsBaseDo.EntityTypePersonnel:
                    return ENTITY_TYPE.PERSONNEL;
                case UserDataFieldsBaseDo.EntityTypeCompanies:
                    return ENTITY_TYPE.COMPANY;
                case UserDataFieldsBaseDo.EntityTypeProducts:
                    return ENTITY_TYPE.PRODUCT;
                case UserDataFieldsBaseDo.EntityTypeSites:
                    return ENTITY_TYPE.SITE;
                case UserDataFieldsBaseDo.EntityTypeEquipment:
                    return ENTITY_TYPE.EQUIPMENT;
                case UserDataFieldsBaseDo.EntityTypeTransactionAliases:
                    return ENTITY_TYPE.TRANSACTION_ALIAS;
                default:
                    return ENTITY_TYPE.UNDEFINED;
            }
        }
        #endregion
    }
}
