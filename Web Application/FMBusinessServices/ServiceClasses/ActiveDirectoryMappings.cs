namespace FMBusinessServices.ServiceClasses
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.ServiceModel;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Exceptions;

    using FMBusinessServices.DataAccessLayer;

    [ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
    public class ActiveDirectoryMappings : IActiveDirectoryMappings
    {
        internal ConsolidatedDAClass ConsolidatedDa = new ConsolidatedDAClass();

        #region Constructors
        #endregion

        #region Public methods

        /// <summary>
        /// This method returns a list of active directory site groups.
        /// </summary>
        /// <param name="security">The security object.</param>
        /// <param name="sitesActiveDirectoryGuid">The Site's current active directory site group GUID.</param>
        /// <returns>Returns active directory site group list.</returns>
        public List<ActiveDirectorySiteGroup> EnumerateActiveDirectorySiteList(SecurityClass security, Guid sitesActiveDirectoryGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException();
            }

            if (security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) == false
                && security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) == false)
            {
                throw new FMInsufficientRightsException();
            }

            var adSiteGroupList = new List<ActiveDirectorySiteGroup>();

            using (SqlCommand cmd = new SqlCommand())
            {
                var siteGroupObj = new ActiveDirectorySiteGroup();
                siteGroupObj.EnumerateSQL(cmd, sitesActiveDirectoryGuid);

                DataSet dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);

                if (dataSet == null || dataSet.Tables.Count < 1 || dataSet.Tables[0].Rows.Count < 1)
                {
                    return adSiteGroupList;
                }

                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    siteGroupObj = new ActiveDirectorySiteGroup();
                    siteGroupObj.LoadRecord(row);

                    adSiteGroupList.Add(siteGroupObj);
                }
            }

            return adSiteGroupList;
        }

        /// <summary>
        /// This method will return a list of all the active directory site groups.
        /// </summary>
        /// <param name="security">The security object.</param>
        /// <returns>Return list of active directory site groups.</returns>
        public List<ActiveDirectorySiteGroup> EnumerateAllActiveDirectorySites(SecurityClass security)
        {
            if (security == null)
            {
                throw new ArgumentNullException();
            }

            if (security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) == false
                && security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) == false)
            {
                throw new FMInsufficientRightsException();
            }

            var adSiteGroupList = new List<ActiveDirectorySiteGroup>();

            using (SqlCommand cmd = new SqlCommand())
            {
                var siteGroupObj = new ActiveDirectorySiteGroup();
                siteGroupObj.EnumerateAllSQL(cmd);

                DataSet dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);

                if (dataSet == null || dataSet.Tables.Count < 1 || dataSet.Tables[0].Rows.Count < 1)
                {
                    return adSiteGroupList;
                }

                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    siteGroupObj = new ActiveDirectorySiteGroup();
                    siteGroupObj.LoadRecord(row);

                    adSiteGroupList.Add(siteGroupObj);
                }
            }

            return adSiteGroupList;
        }

        /// <summary>
        /// This method returns a list of active directory user groups.
        /// </summary>
        /// <param name="security">The security object.</param>
        /// <param name="usersActiveDirectoryGuid">The user's active directory Guid</param>
        /// <returns>Returns active directory user group list.</returns>
        public List<ActiveDirectoryUserGroup> EnumerateActiveDirectoryUserList(SecurityClass security, Guid usersActiveDirectoryGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException();
            }

            if (security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) == false
                && security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) == false)
            {
                throw new FMInsufficientRightsException();
            }

            var adUserGroupList = new List<ActiveDirectoryUserGroup>();

            using (SqlCommand cmd = new SqlCommand())
            {
                var userGroupObj = new ActiveDirectoryUserGroup();
                userGroupObj.EnumerateSQL(cmd, usersActiveDirectoryGuid);

                DataSet dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);

                if (dataSet == null || dataSet.Tables.Count < 1 || dataSet.Tables[0].Rows.Count < 1)
                {
                    return adUserGroupList;
                }

                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    userGroupObj = new ActiveDirectoryUserGroup();
                    userGroupObj.LoadRecord(row);

                    adUserGroupList.Add(userGroupObj);
                }
            }

            return adUserGroupList;
        }

        /// <summary>
        /// This method returns a list of all active directory user groups.
        /// </summary>
        /// <param name="security">The security object.</param>
        /// <returns>Returns active directory user group list.</returns>
        public List<ActiveDirectoryUserGroup> EnumerateAllActiveDirectoryUser(SecurityClass security)
        {
            if (security == null)
            {
                throw new ArgumentNullException();
            }

            if (security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) == false
                && security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) == false)
            {
                throw new FMInsufficientRightsException();
            }

            var adUserGroupList = new List<ActiveDirectoryUserGroup>();

            using (SqlCommand cmd = new SqlCommand())
            {
                var userGroupObj = new ActiveDirectoryUserGroup();
                userGroupObj.EnumerateAllSQL(cmd);

                DataSet dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);

                if (dataSet == null || dataSet.Tables.Count < 1 || dataSet.Tables[0].Rows.Count < 1)
                {
                    return adUserGroupList;
                }

                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    userGroupObj = new ActiveDirectoryUserGroup();
                    userGroupObj.LoadRecord(row);

                    adUserGroupList.Add(userGroupObj);
                }
            }

            return adUserGroupList;
        }

        /// <summary>
        /// This method will return the site to AD site mappings.
        /// </summary>
        /// <param name="security">The security object.</param>
        /// <returns>Return a dataset or null.</returns>
        public DataSet EnumerateSiteToActiveDirectorySiteMapping(SecurityClass security)
        {
            if (security == null)
            {
                throw new ArgumentNullException();
            }

            if (security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) == false
                && security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) == false)
            {
                throw new FMInsufficientRightsException();
            }

            using (SqlCommand cmd = new SqlCommand())
            {
                var siteGroupObj = new ActiveDirectorySiteGroup();
                siteGroupObj.EnumerateSiteToAdSiteMappingSQL(cmd);

                DataSet dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);

                if (dataSet == null || dataSet.Tables.Count < 1 || dataSet.Tables[0].Rows.Count < 1)
                {
                    return null;
                }

                return dataSet;
            }
        }

        /// <summary>
        /// This method will return the user group to AD user group mappings.
        /// </summary>
        /// <param name="security">The security object.</param>
        /// <returns>Return a dataset or null.</returns>
        public DataSet EnumerateUserGroupToActiveDirectoryUserGroupMapping(SecurityClass security)
        {
            if (security == null)
            {
                throw new ArgumentNullException();
            }

            if (security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) == false
                && security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) == false)
            {
                throw new FMInsufficientRightsException();
            }

            using (SqlCommand cmd = new SqlCommand())
            {
                var userGroupObj = new ActiveDirectoryUserGroup();
                userGroupObj.EnumerateUserGroupToAdUserGroupMappingSQL(cmd);

                DataSet dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);

                if (dataSet == null || dataSet.Tables.Count < 1 || dataSet.Tables[0].Rows.Count < 1)
                {
                    return null;
                }

                return dataSet;
            }
        }

        /// <summary>
        /// This method will retrieve a list of site IDs and Guids.
        /// </summary>
        /// <param name="security">The security object.</param>
        /// <returns>Returns a dataset of site IDs and Guids.</returns>
        public DataSet EnumerateAllSiteIdAndGuid(SecurityClass security)
        {
            if (security == null)
            {
                throw new ArgumentNullException();
            }

            if (security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) == false
                && security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) == false)
            {
                throw new FMInsufficientRightsException();
            }

            using (var cmd = new SqlCommand())
            {
                cmd.CommandText = "[dbo].[usp_EnumerateAllSiteIdAndGuid]";
                cmd.CommandType = CommandType.StoredProcedure;

                DataSet dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);

                return dataSet;
            }
        }

        /// <summary>
        /// This method will return a data set with the user mapping plan information.
        /// </summary>
        /// <param name="security">The security object.</param>
        /// <param name="userInfoTable">A table that contains a list of users with a list of sites and user groups.</param>
        /// <param name="deleteMappingsNonExistingUsers"></param>
        /// <returns>Return a data set with the mapping plan.</returns>
        public DataSet GetUserMappingChangePlan(SecurityClass security, DataTable userInfoTable, bool deleteMappingsNonExistingUsers)
        {
            if (security == null)
            {
                throw new ArgumentNullException();
            }

            if (security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) == false
                && security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) == false)
            {
                throw new FMInsufficientRightsException();
            }

            using (var cmd = new SqlCommand())
            {
                cmd.CommandText = "[dbo].[usp_GetUserMappingChangePlan]";
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter tableValuedParameter = cmd.Parameters.Add("@UserADMappingTable", SqlDbType.Structured);
                tableValuedParameter.Value = userInfoTable;
                tableValuedParameter.TypeName = "[dbo].[utt_UserADMapping]";

                cmd.Parameters.AddWithValue("@DeleteMappingsOfNonListedADUsers", deleteMappingsNonExistingUsers ? 1 : 0);

                DataSet dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);

                return dataSet;
            }
        }

        /// <summary>
        /// This method will return a data set wtih the user group mapping plan information.
        /// </summary>
        /// <param name="security">The security object.</param>
        /// <param name="userGroupInfoTable">A table that contains a list of users with a list of user groups.</param>
        /// <param name="deleteMappingsNonExistingUsers"></param>
        /// <returns>Return a data set with the mapping plan.</returns>
        public DataSet GetUserGroupMappingChangePlan(SecurityClass security, DataTable userGroupInfoTable, bool deleteMappingsNonExistingUsers)
        {
            if (security == null)
            {
                throw new ArgumentNullException();
            }

            if (security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) == false
                && security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) == false
                && security.HasRight(RIGHT.MODIFY_USER_GROUPS) == false
                && security.HasRight(RIGHT.VIEW_USER_GROUPS) == false)
            {
                throw new FMInsufficientRightsException();
            }

            using (var cmd = new SqlCommand())
            {
                cmd.CommandText = "[dbo].[usp_GetUserGroupMappingChangePlan]";
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter tableValuedParameter = cmd.Parameters.Add("@UserGroupADMappingTable", SqlDbType.Structured);
                tableValuedParameter.Value = userGroupInfoTable;
                tableValuedParameter.TypeName = "[dbo].[utt_UserGroupADMapping]";

                cmd.Parameters.AddWithValue("@DeleteMappingsOfNonListedADUsers", deleteMappingsNonExistingUsers ? 1 : 0);

                DataSet dataSet = this.ConsolidatedDa.GetDataSet(cmd, security);

                return dataSet;
            }
        }

        /// <summary>
        /// This method will delete a user from a site or from all sites.
        /// </summary>
        /// <param name="security">The security object.</param>
        /// <param name="userGuid">The user to delete via the user guid.</param>
        /// <param name="assignedToSiteGuid">If present, the user to site mapping to delete. When null, all the user to site mappings are delete.</param>
        /// <param name="deleteBaseMapping">The the user from base site that it was created.</param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void DeleteUserFromSite(SecurityClass security, Guid userGuid, Guid? assignedToSiteGuid,  bool deleteBaseMapping)
        {
            if (security == null)
            {
                throw new ArgumentNullException();
            }

            if (security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) == false
                && security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) == false)
            {
                throw new FMInsufficientRightsException();
            }

            int deleteBit = deleteBaseMapping ? 0 : 1;

            using (var cmd = new SqlCommand())
            {
                cmd.CommandText = "[map].[usp_UserToSiteDelete]";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@EntityRecordGuid", userGuid);
                cmd.Parameters.AddWithValue("@DeleteBaseMapping", deleteBit);

                if (assignedToSiteGuid == null)
                {
                    cmd.Parameters.AddWithValue("@AssignedToSiteGuid", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@AssignedToSiteGuid", assignedToSiteGuid.Value);
                }

                this.ConsolidatedDa.ExecuteQuery(security, cmd);
            }
        }

        /// <summary>
        /// This method will update the users owner Guid (SiteGuid).
        /// </summary>
        /// <param name="security">The security object.</param>
        /// <param name="userGuid">The user to update.</param>
        /// <param name="siteGuid">The new owner Guid (SiteGuid)</param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void UpdateUsersOwner(SecurityClass security, Guid userGuid, Guid siteGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException();
            }

            if (security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) == false
                && security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) == false
                && security.HasRight(RIGHT.MODIFY_USERS)
                && security.HasRight(RIGHT.VIEW_USERS))
            {
                throw new FMInsufficientRightsException();
            }

            using (var cmd = new SqlCommand())
            {
                cmd.CommandText = "[dbo].[usp_UserUpdateSiteGuid]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserGuid", userGuid);
                cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);

                this.ConsolidatedDa.ExecuteQuery(security, cmd);
            }
        }


        /// <summary>
        /// This method will delete a user from the user to group mapping table.
        /// </summary>
        /// <param name="security">The security object.</param>
        /// <param name="userGuid">The user to delete via the user guid.</param>
        /// <param name="siteGuid">If present, the site Guid to use to delete the user group record.</param>
        /// <param name="userGroupGuid">If present, the user group Guid to use to delete the user group record.</param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void DeleteUserFromGroups(SecurityClass security, Guid userGuid, Guid? siteGuid, Guid? userGroupGuid)
        {
            if (security == null)
            {
                throw new ArgumentNullException();
            }

            if (security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) == false
                && security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) == false
                && security.HasRight(RIGHT.MODIFY_USER_GROUPS) == false
                && security.HasRight(RIGHT.VIEW_USER_GROUPS) == false)
            {
                throw new FMInsufficientRightsException();
            }

            using (var cmd = new SqlCommand())
            {
                cmd.CommandText = "[map].[usp_ActiveDirectoryUserToUserGroupDelete]";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@UserGuid", userGuid);

                if (siteGuid == null)
                {
                    cmd.Parameters.AddWithValue("@SiteGuid", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@SiteGuid", siteGuid.Value);
                }

                if (userGroupGuid == null)
                {
                    cmd.Parameters.AddWithValue("@UserGroupGuid", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@UserGroupGuid", userGroupGuid.Value);
                }

                this.ConsolidatedDa.ExecuteQuery(security, cmd);
            }
        }
        #endregion
    }
}