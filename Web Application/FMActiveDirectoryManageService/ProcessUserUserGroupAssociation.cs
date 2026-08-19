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

    public class ProcessUserUserGroupAssociation
    {
        #region Data members
        private readonly EventLog FMEventLog;
        private readonly SecurityClass security;
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public ProcessUserUserGroupAssociation(SecurityClass inSecurity, EventLog inFMEventLog, bool inStopFlag)
        {
            this.security = inSecurity;
            this.FMEventLog = inFMEventLog;
            this.StopFlag = inStopFlag;
        }
        #endregion

        #region Properties
        public bool StopFlag { get; set; }
        public List<Tuple<string, Guid>> SiteIdAndGuidList { get; set; }
        #endregion

        /// <summary>
        /// This method starts the process of updating and removing users to user group
        /// group assigments in FuelsManager database based on the active directory settings. 
        /// </summary>
        /// <param name="userGrpToAdUserGrpList">User group to AD user group mapping list.</param>
        /// <param name="adUserList">AD user list which contains the list of sites per user.</param>
        public void ProcessUsersBasedOnUserGroups(List<UserGroupToAdUserGroupDO> userGrpToAdUserGrpList, List<ActiveDirectoryUserDTO> adUserList, bool debugFlag)
        {
            if (adUserList == null || adUserList.Count == 0)
            {
                string message = AdManageThread.MessagePrefixKey + " No users from active directory to process.";
                this.FMEventLog.WriteEntry(message, EventLogEntryType.Warning);
            }

            if (userGrpToAdUserGrpList == null || userGrpToAdUserGrpList.Count == 0)
            {
                string message = AdManageThread.MessagePrefixKey + " No FM User Group to AD User Group mapping configured.";
                this.FMEventLog.WriteEntry(message, EventLogEntryType.Warning);
                return;
            }

            DataSet dataSet;
            DataTable inputTable = new DataTable();
            inputTable.Columns.Add("UserId", typeof(string));
            inputTable.Columns.Add("UserGroupGuid", typeof(Guid));
            inputTable.TableName = "UserGroupInfoTable";

            if (adUserList != null)
            {
                foreach (ActiveDirectoryUserDTO adUser in adUserList)
                {
                    foreach (string adUserGroupId in adUser.UserGroups)
                    {
                        UserGroupToAdUserGroupDO userGroupToAdUserGroup =
                            userGrpToAdUserGrpList.Find(x => x.ActiveDirectoryUserGroupID == adUserGroupId);

                        if (userGroupToAdUserGroup == null)
                        {
                            string message = AdManageThread.MessagePrefixKey
                                             + " Cannot find a FM user group mapping for AD user group: "
                                             + adUserGroupId;
                            this.FMEventLog.WriteEntry(message, EventLogEntryType.Warning);

                            continue;
                        }

                        DataRow row = inputTable.NewRow();
                        row["UserId"] = adUser.UserName;
                        row["UserGroupGuid"] = userGroupToAdUserGroup.UserGroupGuid;
                        inputTable.Rows.Add(row);
                    }
                }

                if (debugFlag)
                {
                    string mFileName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\UsersToUserGroups.xml";
                    inputTable.WriteXml(mFileName);
                }
            }

            try
            {
                dataSet = FMChannelHelper.MakeCall<IActiveDirectoryMappings, DataSet>(x => x.GetUserGroupMappingChangePlan(this.security, inputTable, true));
            }
            catch (Exception ex)
            {
                string message = AdManageThread.MessagePrefixKey + " Could not retrieve user group mapping change plan from FM DB >> "
                                        + ex.Message;
                this.FMEventLog.WriteEntry(message, EventLogEntryType.Error);
                return;
            }

            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    var userGroupMappingChangePlanDo = new UserGroupMappingChangePlanDO()
                    {
                        UserId                  = row.IsNull("UserId") ? string.Empty : (string)row["UserId"],
                        UserGuid                = row.IsNull("UserGuid") ? Guid.Empty : (Guid)row["UserGuid"],
                        MappingChangeActionInt  = row.IsNull("MappingChangeAction") ? 0 : (int)row["MappingChangeAction"],
                        SiteGuid                = row.IsNull("SiteGuid") ? Guid.Empty : (Guid)row["SiteGuid"],
                        UserGroupGuid           = row.IsNull("UserGroupGuid") ? Guid.Empty : (Guid)row["UserGroupGuid"],
                        ErrorMessage            = row.IsNull("ErrorMessage") ? string.Empty : (string)row["ErrorMessage"]
                    };

                    this.UpdateFuelsManagerDbWithUserGroup(userGroupMappingChangePlanDo);

                    // Quit processing if the service stop flag is set.
                    if (this.StopFlag)
                    {
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// This method invokes the user/user group mapping plan and updates the FuelsManager database,
        /// base on the mapping plan.
        /// </summary>
        /// <param name="userGroupMappingChangePlanDO">A user group mapping plan.</param>
        private void UpdateFuelsManagerDbWithUserGroup(UserGroupMappingChangePlanDO userGroupMappingChangePlanDO)
        {
            if (userGroupMappingChangePlanDO.MappingChangeAction == UserGroupMappingChangePlanDO.MappingChangeActionTypes.NoAction)
            {
                if (string.IsNullOrEmpty(userGroupMappingChangePlanDO.ErrorMessage) == false)
                {
                    string message = AdManageThread.MessagePrefixKey + " Error with user (" + userGroupMappingChangePlanDO.UserId + "): " + userGroupMappingChangePlanDO.ErrorMessage;
                    this.FMEventLog.WriteEntry(message, EventLogEntryType.Error);
                }

                return;
            }

            if (userGroupMappingChangePlanDO.MappingChangeAction == UserGroupMappingChangePlanDO.MappingChangeActionTypes.Add)
            {
                var userGroupMapping = new UserGroupMapClass
                {
                    UserGuid = userGroupMappingChangePlanDO.UserGuid,
                    SiteGuid = userGroupMappingChangePlanDO.SiteGuid,
                    GroupGuid = userGroupMappingChangePlanDO.UserGroupGuid
                };

                try
                {
                    FMChannelHelper.MakeCall<IUserGroupMaps>(x => x.Add(this.security, userGroupMapping));
                }
                catch (Exception ex)
                {
                    string message = AdManageThread.MessagePrefixKey + " Could not add user (" + userGroupMappingChangePlanDO.UserId + ") to user map: "
                                            + ex.Message;
                    this.FMEventLog.WriteEntry(message, EventLogEntryType.Warning);
                }

                return;
            }

            if (userGroupMappingChangePlanDO.MappingChangeAction == UserGroupMappingChangePlanDO.MappingChangeActionTypes.Delete)
            {
                try
                {
                    FMChannelHelper.MakeCall<IActiveDirectoryMappings>(
                        x => x.DeleteUserFromGroups(this.security, userGroupMappingChangePlanDO.UserGuid, userGroupMappingChangePlanDO.SiteGuid, userGroupMappingChangePlanDO.UserGroupGuid));
                }
                catch (Exception ex)
                {
                    string siteId = this.GetSiteId(userGroupMappingChangePlanDO.SiteGuid);
                    string message = AdManageThread.MessagePrefixKey + " Could not remove user (" + userGroupMappingChangePlanDO.UserId
                                    + ") from user map at site (" + siteId + ": "
                                    + ex.Message;
                    this.FMEventLog.WriteEntry(message, EventLogEntryType.Warning);
                }

                return;
            }

            if (userGroupMappingChangePlanDO.MappingChangeAction == UserGroupMappingChangePlanDO.MappingChangeActionTypes.DeleteMappingMissingUser)
            {
                try
                {
                    FMChannelHelper.MakeCall<IActiveDirectoryMappings>(
                                            x => x.DeleteUserFromGroups(this.security, userGroupMappingChangePlanDO.UserGuid, null, null));
                }
                catch (Exception ex)
                {
                    string siteId = this.GetSiteId(userGroupMappingChangePlanDO.SiteGuid);
                    string message = AdManageThread.MessagePrefixKey + " Could not remove user (" + userGroupMappingChangePlanDO.UserId
                                    + ") from user map at site (" + siteId + ": "
                                    + ex.Message;
                    this.FMEventLog.WriteEntry(message, EventLogEntryType.Warning);
                }
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
