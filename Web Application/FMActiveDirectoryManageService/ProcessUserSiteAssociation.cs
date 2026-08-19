namespace FMActiveDirectoryManageService
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;

    public class ProcessUserSiteAssociation
    {
        #region Data members
        private readonly EventLog FMEventLog;
        private readonly SecurityClass security;

        private readonly BuildSecurityClass buildSecurityClass;
        private Tuple<string, Guid> previousUser;
        private bool changeUserOwnerGuid;
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public ProcessUserSiteAssociation(SecurityClass inSecurity, EventLog inFMEventLog, bool inStopFlag)
        {
            this.security = inSecurity;
            this.FMEventLog = inFMEventLog;
            this.StopFlag = inStopFlag;
            this.buildSecurityClass = new BuildSecurityClass();
        }
        #endregion

        #region Properties
        public bool StopFlag { get; set; }
        public List<Tuple<string, Guid>> SiteIdAndGuidList { get; set; }
        #endregion

        /// <summary>
        /// This method starts the process of creating/deleting/updating the 
        /// </summary>
        /// <param name="siteToAdSiteList">Site to AD site mapping list.</param>
        /// <param name="adUserList">AD user list which contains the list of sites per user.</param>
        public void ProcessUsersBasedOnSites(List<SiteToAdSiteDO> siteToAdSiteList, List<ActiveDirectoryUserDTO> adUserList, bool debugFlag)
        {
            if (adUserList == null || adUserList.Count == 0)
            {
                string message = AdManageThread.MessagePrefixKey + " No users from active directory to process.";
                this.FMEventLog.WriteEntry(message, EventLogEntryType.Warning);
            }

            if (siteToAdSiteList == null || siteToAdSiteList.Count == 0)
            {
                string message = AdManageThread.MessagePrefixKey + " No FM Site to AD Site mapping configured.";
                this.FMEventLog.WriteEntry(message, EventLogEntryType.Warning);
                return;
            }

            DataSet dataSet;
            DataTable inputTable = new DataTable();
            inputTable.Columns.Add("UserId", typeof(string));
            inputTable.Columns.Add("SiteGuid", typeof(Guid));
            inputTable.TableName = "UserInfoTable";

            if (adUserList != null)
            {
                foreach (ActiveDirectoryUserDTO adUser in adUserList)
                {
                    foreach (string adSiteId in adUser.Sites)
                    {
                        SiteToAdSiteDO siteToSite = siteToAdSiteList.Find(x => x.ActiveDirectorySiteId == adSiteId);

                        if (siteToSite == null)
                        {
                            string message = AdManageThread.MessagePrefixKey
                                             + " Cannot find a FM site mapping for AD site group: " + adSiteId;
                            this.FMEventLog.WriteEntry(message, EventLogEntryType.Warning);

                            continue;
                        }

                        DataRow row = inputTable.NewRow();
                        row["UserId"] = adUser.UserName;
                        row["SiteGuid"] = siteToSite.SiteGuid;
                        inputTable.Rows.Add(row);
                    }
                }

                if(debugFlag)
                {
                    string mFileName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\UsersToSites.xml";
                    inputTable.WriteXml(mFileName);
                }
            }

            try
            {
                dataSet = FMChannelHelper.MakeCall<IActiveDirectoryMappings, DataSet>(x => x.GetUserMappingChangePlan(this.security, inputTable, true));
            }
            catch (Exception ex)
            {
                string message = AdManageThread.MessagePrefixKey + " Could not retrieve user mapping change plan from FM DB >> "
                                        + ex.Message;
                this.FMEventLog.WriteEntry(message, EventLogEntryType.Error);
                return;
            }

            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
            {
                this.previousUser = new Tuple<string, Guid>(string.Empty, Guid.Empty);
                this.changeUserOwnerGuid = false;

                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    var userMappingChangePlanDo = new UserMappingChangePlanDO
                    {
                        RunningIndex                = row.IsNull("runningIndex") ? 0 : (int)row["runningIndex"],
                        UserId                      = row.IsNull("UserId") ? string.Empty : (string)row["UserId"],
                        UserGuid                    = row.IsNull("UserGuid") ? Guid.Empty : (Guid)row["UserGuid"],
                        MappingChangeActionInt      = row.IsNull("MappingChangeAction") ? 0 : (int)row["MappingChangeAction"],
                        AssignedFromSiteGuid        = row.IsNull("AssignedFromSiteGuid") ? Guid.Empty : (Guid)row["AssignedFromSiteGuid"],
                        AssignedToSiteGuid          = row.IsNull("AssignedToSiteGuid") ? Guid.Empty : (Guid)row["AssignedToSiteGuid"],
                        AssignedToJierarchyLevel    = row.IsNull("AssignedToHierarchyLevel") ? 0 : (int)row["AssignedToHierarchyLevel"],
                        ErrorMessage                = row.IsNull("ErrorMessage") ? string.Empty : (string)row["ErrorMessage"]
                    };

                    this.UpdateFuelsManagerDbWithUserSite(userMappingChangePlanDo);

                    // Quit processing if the service stop flag is set.
                    if (this.StopFlag)
                    {
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// This method invokes the user/site mapping plan and updates the FuelsManager database,
        /// base on the mapping plan.
        /// </summary>
        /// <param name="userMappingChangePlanDO">A user mapping plan.</param>
        private void UpdateFuelsManagerDbWithUserSite(UserMappingChangePlanDO userMappingChangePlanDO)
        {
            if (userMappingChangePlanDO.MappingChangeAction == UserMappingChangePlanDO.MappingChangeActionTypes.NoAction)
            {
                if (string.IsNullOrEmpty(userMappingChangePlanDO.ErrorMessage) == false)
                {
                    string message = AdManageThread.MessagePrefixKey + " Error with user (" + userMappingChangePlanDO.UserId + "): " + userMappingChangePlanDO.ErrorMessage;
                    this.FMEventLog.WriteEntry(message, EventLogEntryType.Error);
                }

                return;
            }

            if (userMappingChangePlanDO.MappingChangeAction == UserMappingChangePlanDO.MappingChangeActionTypes.Add)
            {
                // When the Assigned From Site is equal to the Assigned To Site, this means to create the user
                // at that site.
                if (userMappingChangePlanDO.AssignedFromSiteGuid == userMappingChangePlanDO.AssignedToSiteGuid)
                {
                    SecurityClass localSecurity = this.buildSecurityClass.BuildSecurity(userMappingChangePlanDO.AssignedFromSiteGuid);

                    if (this.changeUserOwnerGuid)
                    {
                        // When the change user owner flag is set that means the user is already inserted and the
                        // user owner guid (site guid) needs to be updated.
                        this.changeUserOwnerGuid = false;
                        FMChannelHelper.MakeCall<IActiveDirectoryMappings>(
                                        x => x.UpdateUsersOwner(this.security, userMappingChangePlanDO.UserGuid, userMappingChangePlanDO.AssignedFromSiteGuid));

                        // Map user to the user/site mapping table
                        var entityMap = new EntityToSiteMapClass
                        {
                            TypeID = ENTITY_TYPE.USER,
                            IdentityGuid = userMappingChangePlanDO.UserGuid,
                            AssignedFromSiteGuid = userMappingChangePlanDO.AssignedFromSiteGuid,
                            SiteGuid = userMappingChangePlanDO.AssignedToSiteGuid
                        };

                        try
                        {
                            FMChannelHelper.MakeCall<IEntityToSiteMaps>(x => x.Add(this.security, entityMap, typeof(IUsers).GUID));
                        }
                        catch (Exception ex)
                        {
                            string fromSiteId = this.GetSiteId(userMappingChangePlanDO.AssignedFromSiteGuid);
                            string toSiteId = this.GetSiteId(userMappingChangePlanDO.AssignedToSiteGuid);
                            string message = AdManageThread.MessagePrefixKey + " Assigning User: " + userMappingChangePlanDO.UserId + " From Site: "
                                            + fromSiteId + " To Site: "
                                            + toSiteId + " >> " + ex.Message;
                            this.FMEventLog.WriteEntry(message, EventLogEntryType.Warning);
                        }
                    }
                    else
                    {
                        try
                        {
                            // Create user at the site
                            var fmUser = new UserClass
                            {
                                ActiveDirectoryUser = true,
                                ID = userMappingChangePlanDO.UserId,
										  SiteGuid = userMappingChangePlanDO.AssignedFromSiteGuid,
										  AccountExpirationDate = DateTime.Now.AddYears(100)
									 };

                            // A new user add.
                            fmUser.IdentityGuid = FMChannelHelper.MakeCall<IUsers, Guid>(x => x.Add(localSecurity, fmUser));
                            this.previousUser = new Tuple<string, Guid>(userMappingChangePlanDO.UserId, fmUser.IdentityGuid);
                        }
                        catch (Exception ex)
                        {
                            string fromSiteId = this.GetSiteId(userMappingChangePlanDO.AssignedFromSiteGuid);
                            string message = AdManageThread.MessagePrefixKey + "Creating User: " + userMappingChangePlanDO.UserId + " at Site: "
                                    + fromSiteId + " >> " + ex.Message;
                            this.FMEventLog.WriteEntry(message, EventLogEntryType.Warning);
                        }
                    }
                }
                else
                {
                    Guid userIdentity = userMappingChangePlanDO.UserGuid;
                    if (userMappingChangePlanDO.UserGuid == Guid.Empty && this.previousUser.Item1.Equals(userMappingChangePlanDO.UserId))
                    {
                        userIdentity = this.previousUser.Item2;
                    }

                    // Map user to the user/site mapping table
                    var entityMap = new EntityToSiteMapClass
                    {
                        TypeID = ENTITY_TYPE.USER,
                        IdentityGuid = userIdentity,
                        AssignedFromSiteGuid = userMappingChangePlanDO.AssignedFromSiteGuid,
                        SiteGuid = userMappingChangePlanDO.AssignedToSiteGuid
                    };

                    try
                    {
                        FMChannelHelper.MakeCall<IEntityToSiteMaps>(x => x.Add(this.security, entityMap, typeof(IUsers).GUID));
                    }
                    catch (Exception ex)
                    {
                        string fromSiteId = this.GetSiteId(userMappingChangePlanDO.AssignedFromSiteGuid);
                        string toSiteId = this.GetSiteId(userMappingChangePlanDO.AssignedToSiteGuid);
                        string message = AdManageThread.MessagePrefixKey + " Assigning User: " + userMappingChangePlanDO.UserId + " From Site: "
                                        + fromSiteId + " To Site: "
                                        + toSiteId + " >> " + ex.Message;
                        this.FMEventLog.WriteEntry(message, EventLogEntryType.Warning);
                    }
                }

                return;
            }

            if (userMappingChangePlanDO.MappingChangeAction == UserMappingChangePlanDO.MappingChangeActionTypes.Delete)
            {
                try
                {
                    // Delete user from the user/site mapping table
                    FMChannelHelper.MakeCall<IActiveDirectoryMappings>(
                        x => x.DeleteUserFromSite(this.security, userMappingChangePlanDO.UserGuid, userMappingChangePlanDO.AssignedToSiteGuid, false));

                    // If the FROM and TO site guids are the same during the delete, then probably a change
                    // user owner is occurring. Set the change user owner flag.
                    if (userMappingChangePlanDO.AssignedFromSiteGuid == userMappingChangePlanDO.AssignedToSiteGuid)
                    {
                        this.changeUserOwnerGuid = true;
                    }

                    // Since the user is deleted from the Site, then the user to user group record being referenced must be deleted.
                    FMChannelHelper.MakeCall<IActiveDirectoryMappings>(
                        x => x.DeleteUserFromGroups(this.security, userMappingChangePlanDO.UserGuid, userMappingChangePlanDO.AssignedToSiteGuid, null));
                }
                catch (Exception ex)
                {
                    string toSiteId = this.GetSiteId(userMappingChangePlanDO.AssignedToSiteGuid);
                    string message = AdManageThread.MessagePrefixKey + " Un-assigning user: " + userMappingChangePlanDO.UserId + " From Site: "
                                        + toSiteId + " >> " + ex.Message;
                    this.FMEventLog.WriteEntry(message, EventLogEntryType.Warning);
                }

                return;
            }

            if (userMappingChangePlanDO.MappingChangeAction == UserMappingChangePlanDO.MappingChangeActionTypes.DeleteMappingMissingUser)
            {
                try
                {
                    // Remove all entries of the user.
                    FMChannelHelper.MakeCall<IActiveDirectoryMappings>(
                        x => x.DeleteUserFromSite(this.security, userMappingChangePlanDO.UserGuid, null, true));
                }
                catch (Exception ex)
                {
                    string message = AdManageThread.MessagePrefixKey + " Removing user: " + userMappingChangePlanDO.UserId + " from FuelsManager >> "
                                        + ex.Message;
                    this.FMEventLog.WriteEntry(message, EventLogEntryType.Warning);
                }

                // Since the user is deleted from the Site, then the user to user group record being referenced must be deleted.
                FMChannelHelper.MakeCall<IActiveDirectoryMappings>(
                    x => x.DeleteUserFromGroups(this.security, userMappingChangePlanDO.UserGuid, userMappingChangePlanDO.AssignedToSiteGuid, null));
            }
        }

        /// <summary>
        /// This method will return the Site ID if found, otherwise it will return the site GUID.
        /// </summary>
        /// <param name="siteGuid">The site Guid used to find the site ID.</param>
        /// <returns>Returns a site ID if found.</returns>
        private string GetSiteId(Guid siteGuid)
        {
            if (this.SiteIdAndGuidList.Count == 0)
            {
                return siteGuid.ToString();
            }

            var siteIdAndGuid = this.SiteIdAndGuidList.Find(x => x.Item2 == siteGuid);

            if (siteIdAndGuid == null)
            {
                return siteGuid.ToString();
            }

            return siteIdAndGuid.Item1;
        }
    }
}
