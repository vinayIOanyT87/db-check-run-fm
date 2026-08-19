// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupsClass.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Implementation for the IGroups service interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.UtilityObjects;

	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;

	using IsolationLevel = System.Transactions.IsolationLevel;

	/// <summary>
	/// Implementation for the IGroups service interface.
	/// </summary>
	[SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = IsolationLevel.ReadCommitted)]
	public class GroupsClass : IDependency, IGroups
	{
		#region Fields

		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Adds the specified group.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="group">The group.</param>
		/// <returns>The identity guid of the newly added group.</returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, GroupClass group)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (group == null)
			{
				throw new ArgumentNullException("group");
			}

			if (!security.HasRight(RIGHT.MODIFY_USER_GROUPS) && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			this.Validate(group);

			if (!this.GetIdentityGuid(security, @group.ID).IsEmpty())
			{
				throw new Exception("Group Exists");
			}

			group.SiteGuid = security.SiteGuid;
			group.CreatedDate = DateTimeOffset.Now;
			group.CreatedBy = security.UserID;
			group.UpdatedDate = @group.CreatedDate;
			group.UpdatedBy = security.UserID;
			group.IdentityGuid = Guid.NewGuid();

			using (var cmd = new SqlCommand())
			{
				group.InsertSQLCmd(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			// Create Entity to Site Map
			var entityToSiteMaps = new EntityToSiteMaps();
			var entityToSiteMap = new EntityToSiteMapClass(group);
			entityToSiteMaps.Add(security, entityToSiteMap, this.GetType().GUID);

			this.UpdateRights(security, group, null, false);
			this.UpdateUserGroupMaps(security, group, null, false);
			this.UpdateCompanies(security, group, null);

			return group.IdentityGuid;
		}

		public GroupCollectionClass Enumerate(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			// Just about anyone can enumerate groups.
			// This is because it is used in the View Company
			if (!security.HasRight(RIGHT.VIEW_USER_GROUPS) && !security.HasRight(RIGHT.MODIFY_USER_GROUPS)
			    && !security.HasRight(RIGHT.VIEW_USERS) && !security.HasRight(RIGHT.MODIFY_USERS)
			    && !security.HasRight(RIGHT.VIEW_COMPANY_DATA) && !security.HasRight(RIGHT.MODIFY_COMPANY_DATA)
			    && !security.HasRight(RIGHT.VIEW_PERSONNEL_DATA) && !security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA)
			    && !security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) && !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
			    && !security.HasRight(RIGHT.VIEW_TICKETING_DATA) && !security.HasRight(RIGHT.MODIFY_TICKETING_DATA)
			    && !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA) && !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
			    && !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) && !security.HasRight(RIGHT.VIEW_REPORTS)
			    && !security.HasRight(RIGHT.MODIFY_REPORTS) && !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
			    && !security.HasRight(RIGHT.VIEW_TRANSACTION_ALIASES) && !security.HasRight(RIGHT.VIEW_LOAD_RACK_DATA)
			    && !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA) && !security.HasRight(RIGHT.MODIFY_DISPATCH)
			    && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA) && !security.HasRight(RIGHT.VIEW_DISPATCH)
                && !security.HasRight(RIGHT.CREATE_ORDERS) && !security.HasRight(RIGHT.VIEW_ORDERS)
                && !security.HasRight(RIGHT.MODIFY_ORDERS) && !security.HasRight(RIGHT.CREATE_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.VIEW_SUPPLY_ORDERS) && !security.HasRight(RIGHT.MODIFY_SUPPLY_ORDERS)
                && !security.HasRight(RIGHT.MODIFY_QUERIES) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			var group = new GroupClass();
			DataSet set;
			using (SqlCommand cmd = group.EnumerateSQLCmd(security))
			{
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			var groupCollection = new GroupCollectionClass();

			DataTable table = set.Tables[0];
			var groupDictionary = new Dictionary<Guid, GroupClass>();
			while (table.Rows.Count != 0)
			{
				group = new GroupClass();
				group.LoadObject(set);
				groupCollection.Add(group);
				table.Rows.RemoveAt(0);
				groupDictionary.Add(group.IdentityGuid, group);
			}

			DataTable groupRightMapTable = set.Tables[1];
			foreach (DataRow row in groupRightMapTable.Rows)
			{
				var groupGuid = (Guid)row["GroupGuid"];
				if (groupDictionary.ContainsKey(groupGuid))
				{
					groupDictionary[groupGuid].RightCollection.Add(RIGHT.VIEW_OPERATE_ONLY);
				}
			}

			return groupCollection;
		}

		public GroupCollectionClass EnumerateAllForGrid(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_USER_GROUPS) && !security.HasRight(RIGHT.MODIFY_USER_GROUPS)
			    && !security.HasRight(RIGHT.VIEW_USERS) && !security.HasRight(RIGHT.MODIFY_USERS))
			{
				throw new FMInsufficientRightsException();
			}

			// Cannot enumerate Users by Group.  Would cause infinite loop.
			var group = new GroupClass();
			using (SqlCommand cmd = group.EnumerateAllForGridSQL())
			{
				DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);

				var groupCollection = new GroupCollectionClass();

				DataTable groupTable = set.Tables[0];
				var groupDictionary = new Dictionary<Guid, GroupClass>();
				foreach (DataRow row in groupTable.Rows)
				{
					group = new GroupClass
						        {
							        IdentityGuid = DataObject.getValue(row["GroupGuid"], Guid.Empty), 
							        ID = DataObject.getValue(row["GroupID"], string.Empty)
						        };

					groupCollection.Add(group);
					groupDictionary.Add(group.IdentityGuid, group);
				}

				DataTable groupRightMapTable = set.Tables[1];
				foreach (DataRow row in groupRightMapTable.Rows)
				{
					var groupGuid = (Guid)row["GroupGuid"];
					if (groupDictionary.ContainsKey(groupGuid))
					{
						groupDictionary[groupGuid].RightCollection.Add(RIGHT.VIEW_OPERATE_ONLY);
					}
				}

				return groupCollection;
			}
		}

		public GroupCollectionClass EnumerateBySite(SecurityClass security, Guid siteGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_USER_GROUPS) && !security.HasRight(RIGHT.MODIFY_USER_GROUPS)
			    && !security.HasRight(RIGHT.VIEW_USERS) && !security.HasRight(RIGHT.MODIFY_USERS)
			    && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			// Cannot enumerate Users by Group.  Would cause infinite loop.
			var group = new GroupClass();
			using (SqlCommand cmd = group.EnumerateBySiteSQL(security, siteGuid, ContextUtil.IsInTransaction))
			{
				DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);

				var toRet = new GroupCollectionClass();

				DataTable table = set.Tables[0];
				while (table.Rows.Count != 0)
				{
					group = new GroupClass();
					group.LoadObject(set);
					toRet.Add(group);
					table.Rows.RemoveAt(0);
				}

				return toRet;
			}
		}

		/// <summary>
		/// Enumerates the group membership of the user specified by UserGuid
		/// </summary>
		/// <param name="security">
		/// security object of the current user
		/// </param>
		/// <param name="userGuid">
		/// user to return group membership of
		/// </param>
		/// <returns>
		/// Collection of group objects user belongs to
		/// </returns>
		/// <remarks>
		/// Do not enumerate users by group from within this function - an infinite
		///     recursion will result.
		/// </remarks>
		public GroupCollectionClass EnumerateByUser(SecurityClass security, Guid userGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (security.UserGuid != userGuid && !security.HasRight(RIGHT.VIEW_USER_GROUPS)
			    && !security.HasRight(RIGHT.MODIFY_USER_GROUPS) && !security.HasRight(RIGHT.VIEW_USERS)
			    && !security.HasRight(RIGHT.MODIFY_USERS) && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

         // Cannot enumerate Users by Group.  Would cause infinite loop.
         var group = new GroupClass
         {
            SiteGuid = security.SiteGuid
         };
         DataSet set;
			using (SqlCommand cmd = group.EnumerateByUserSQL(security, userGuid, ContextUtil.IsInTransaction))
			{
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			var toRet = new GroupCollectionClass();
			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				group = new GroupClass();
				group.LoadObject(set);
				toRet.Add(group);
				table.Rows.RemoveAt(0);
			}

			return toRet;
		}

		/// <summary>
		/// Enumerates the by user by group.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="userGuid">
		/// The user GUID.
		/// </param>
		/// <param name="groupGuid">
		/// The group GUID.
		/// </param>
		/// <returns>
		/// The <see cref="GroupCollectionClass"/>.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">
		/// Security
		/// </exception>
		/// <exception cref="System.UnauthorizedAccessException">
		/// Access Denied
		/// </exception>
		public GroupCollectionClass EnumerateByUserByGroup(SecurityClass security, Guid userGuid, Guid groupGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (security.UserGuid != userGuid && !security.HasRight(RIGHT.VIEW_USER_GROUPS)
			    && !security.HasRight(RIGHT.MODIFY_USER_GROUPS) && !security.HasRight(RIGHT.VIEW_USERS)
			    && !security.HasRight(RIGHT.MODIFY_USERS) && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			// Cannot enumerate Users by Group.  Would cause infinite loop.
			var group = new GroupClass();
			DataSet set;
			using (SqlCommand cmd = group.EnumerateByUserByGroupSQL(security, userGuid, groupGuid, ContextUtil.IsInTransaction))
			{
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			var toRet = new GroupCollectionClass();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				group = new GroupClass();
				group.LoadObject(set);
				toRet.Add(group);
				table.Rows.RemoveAt(0);
			}

			return toRet;
		}

		/// <summary>
		/// Enumerates the by user by site.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="userGuid">
		/// The user GUID.
		/// </param>
		/// <param name="siteGuid">
		/// The site GUID.
		/// </param>
		/// <returns>
		/// The <see cref="GroupCollectionClass"/>.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">
		/// Security
		/// </exception>
		/// <exception cref="System.UnauthorizedAccessException">
		/// Access Denied
		/// </exception>
		public GroupCollectionClass EnumerateByUserBySite(SecurityClass security, Guid userGuid, Guid siteGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (security.UserGuid != userGuid && !security.HasRight(RIGHT.VIEW_USER_GROUPS)
			    && !security.HasRight(RIGHT.MODIFY_USER_GROUPS) && !security.HasRight(RIGHT.VIEW_USERS)
			    && !security.HasRight(RIGHT.MODIFY_USERS) && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

         // Cannot enumerate Users by Group.  Would cause infinite loop.
         var group = new GroupClass
         {
            SiteGuid = siteGuid
         };
         DataSet set;
			using (SqlCommand cmd = group.EnumerateByUserBySiteSQL(security, userGuid, siteGuid, ContextUtil.IsInTransaction))
			{
				set = this.ConsolidatedDA.GetDataSet(cmd, security);

				var groupCollection = new GroupCollectionClass();

				DataTable table = set.Tables[0];
				while (table.Rows.Count != 0)
				{
					group = new GroupClass();
					group.LoadObject(set);
					groupCollection.Add(group);
					table.Rows.RemoveAt(0);
				}

				return groupCollection;
			}
		}

		/// <summary>
		/// Enumerates the group membership of the user specified by UserGuid
		///     This is intended to be used only during the login process, as it uses
		///     the service account rather than the user's account to connect to the data store
		/// </summary>
		/// <param name="security">
		/// security object of the current user
		/// </param>
		/// <param name="userGuid">
		/// user to return group membership of
		/// </param>
		/// <returns>
		/// Collection of group objects user belongs to
		/// </returns>
		/// <remarks>
		/// Do not enumerate users by group from within this function - an infinite
		///     recursion will result.
		/// </remarks>
		/// <exception cref="ArgumentNullException">
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Thrown if the user in security does not match userGuid.  This version may only be called
		///     to find the group membership of self
		/// </exception>
		public GroupCollectionClass EnumerateByUserDuringLogOn(SecurityClass security, Guid userGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (security.UserGuid != userGuid)
			{
				throw new ArgumentException("security");
			}

         // Cannot enumerate Users by Group.  Would cause infinite loop.
         var group = new GroupClass
         {
            SiteGuid = security.SiteGuid
         };
         DataSet set;
			using (
				SqlCommand cmd = group.EnumerateByUserBySiteSQL(security, userGuid, security.SiteGuid, ContextUtil.IsInTransaction))
			{
				set = this.ConsolidatedDA.GetDataSet(cmd, DBAccess.ServiceLoginAccess, string.Empty);
			}

			var groupCollection = new GroupCollectionClass();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				group = new GroupClass();
				group.LoadObject(set);
				groupCollection.Add(group);
				table.Rows.RemoveAt(0);
			}

			return groupCollection;
		}

		public GroupClass Get(SecurityClass security, Guid targetGroupGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var users = new UsersClass();
			UserClass user = users.Get(security, security.UserGuid);

			if (!security.HasRight(RIGHT.VIEW_USER_GROUPS) && !security.HasRight(RIGHT.MODIFY_USER_GROUPS)
				&& !security.HasRight(RIGHT.MODIFY_COMPANY_DATA) && !security.HasRight(RIGHT.VIEW_COMPANY_DATA)
				&& !security.HasRight(RIGHT.MODIFY_QUERIES) && !security.HasRight(RIGHT.VIEW_QUERIES) && !security.HasRight(RIGHT.CONFIGURE_QUERIES) 
				&& !security.HasRight(RIGHT.VIEW_TRANSACTION_ALIASES) 
				&& !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA)
				&& !security.HasRight(RIGHT.VIEW_USERS)
				&& !user.UserGroupMapCollection.Exists(map => map.GroupGuid == targetGroupGuid))
			{
				throw new FMInsufficientRightsException();
			}

			var group = new GroupClass { IdentityGuid = targetGroupGuid };

			using (SqlCommand cmd = group.SelectSQLCmd(ContextUtil.IsInTransaction))
			{
				group.LoadObject(this.ConsolidatedDA.GetDataSet(cmd, security));
			}

			var userGroupMaps = new UserGroupMaps();
			group.UserGroupMapCollection = userGroupMaps.EnumerateByGroupAndSite(security, group.IdentityGuid, security.SiteGuid);

			var rights = new RightsClass();
			group.RightCollection = rights.EnumerateByGroup(security, group.IdentityGuid);

            var companyMaps = new CompanyMapsClass();
            group.CompanyMapCollection = companyMaps.EnumerateByAssignedToGuidAndType(security
                                                                                    , group.IdentityGuid
                                                                                    , COMPANY_MAP_TYPE.USER_GROUP_COMPANY_MAP);

			return group;
		}

		public Guid GetIdentityGuid(SecurityClass security, string groupID)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_USER_GROUPS)
				&& !security.HasRight(RIGHT.MODIFY_USER_GROUPS)
			    && !security.HasRight(RIGHT.MODIFY_QUERIES)
				&& !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA)
				&& !security.HasRight(RIGHT.EXPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

         var group = new GroupClass
         {
            ID = groupID,
            SiteGuid = security.SiteGuid
         };
         using (SqlCommand cmd = group.SelectByIdsqlCmd(security, ContextUtil.IsInTransaction))
			{
				group.LoadObject(this.ConsolidatedDA.GetDataSet(cmd, security));
			}

			return group.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Import(SecurityClass Security, GroupClass group)
		{
			if (Security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (group == null)
			{
				throw new ArgumentNullException("group");
			}

			SecurityClass security = Security.Clone();

			var sites = new SitesClass();
			SiteClass site = sites.GetUsingGuid(security, security.SiteGuid);

			var companies = new CompaniesClass();
			var users = new UsersClass();

			try
			{
				// Group itself
				group.IdentityGuid = this.GetIdentityGuid(security, group.ID);

				// If the entity exists and is not owned by this site, do not update it.
				if ((!group.IdentityGuid.IsEmpty()) && this.Get(security, group.IdentityGuid).SiteGuid != security.SiteGuid)
				{
					return;
				}

				// Users - we are explicitly NOT creating the users if they do not exist
				for (int item = 0; item < group.UserGroupMapCollection.Count; item++)
				{
					UserGroupMapClass userGroupMap = group.UserGroupMapCollection[item];

					userGroupMap.UserGuid = users.GetIdentityGuid(security, userGroupMap.UserID);

					if (userGroupMap.UserGuid.IsEmpty())
					{
						group.UserGroupMapCollection.Remove(userGroupMap);
					}
				}

				// Companies
				foreach (CompanyMapClass map in group.CompanyMapCollection)
				{
					if (map.AssignedID != "{All}")
					{
						Guid identityGuid = companies.GetIdentityGuid(security, map.AssignedID);
						if (identityGuid == Guid.Empty)
						{
							// Create company with no role
							var company = new CompanyClass(site) { ID = map.AssignedID };
							identityGuid = companies.Add(security, company);
						}

						map.AssignedGuid = identityGuid;
					}
				}

				if (group.IdentityGuid.IsEmpty())
				{
					this.Add(security, group);
				}
				else
				{
					this.Modify(security, group);
				}
			}
			catch (Exception except)
			{
				throw new ApplicationException("[Group Import Error ID] : " + group.ID + ", " + except.Message);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, GroupClass Group)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (Group == null)
			{
				throw new ArgumentNullException("Group");
			}

			if (!security.HasRight(RIGHT.MODIFY_USER_GROUPS) && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			this.Validate(Group);

			Guid groupGuid = this.GetIdentityGuid(security, Group.ID);
			if (groupGuid.IsNotEmptyAndNotEqualTo(Group.IdentityGuid))
			{
				throw new Exception("Group Exists");
			}

			GroupClass oldGroup = this.Get(security, Group.IdentityGuid);
			if (oldGroup.IdentityGuid.IsEmpty())
			{
				throw new Exception("Group Not Found");
			}

			// Preclude rename of Administrator Group
			if (oldGroup.ID == "Administrator" && oldGroup.ID != Group.ID)
			{
				throw new Exception("Group Exists");
			}

			Group.UpdatedDate = DateTimeOffset.Now;
			Group.UpdatedBy = security.UserID;
			using (SqlCommand cmd = Group.UpdateSQLCmd())
			{
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			var entityToSiteMaps = new EntityToSiteMaps();
			EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(
				security, Group.EntityType, Group.IdentityGuid);

			if (Group.SiteGuid != oldGroup.SiteGuid)
			{
				// Purge from EntityToSiteMap
				foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
				{
					entityToSiteMap.ID = Group.ID;
					entityToSiteMaps.Purge(security, entityToSiteMap);
				}

				// Create Entity to Site Map
				var newEntityToSiteMap = new EntityToSiteMapClass(Group);
				entityToSiteMaps.Add(security, newEntityToSiteMap, this.GetType().GUID);
			}

			// Need to purge unassigned rights and users first before adding new ones.
			this.UpdateRights(security, Group, oldGroup, true);

			// The user collection can be null if coming from Import
			if (Group.UserGroupMapCollection != null)
			{
				this.UpdateUserGroupMaps(security, Group, oldGroup, true);
			}

			this.UpdateRights(security, Group, oldGroup, false);

			// The user collection can be null if coming from Import
			if (Group.UserGroupMapCollection != null)
			{
				this.UpdateUserGroupMaps(security, Group, oldGroup, false);
			}

			this.UpdateCompanies(security, Group, oldGroup);
			this.PropagateCompanyMappings(security, Group.IdentityGuid);
		}

		/// <summary>
		/// Propagates down the Company record-versioning hierarchy, the latest Company-UserGroup mapping updates made to a UserGroup record
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="groupGuid">
		/// The Guid of usergroup being modified
		/// </param>
		/// <exception cref="System.ArgumentNullException">
		/// security
		/// </exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void PropagateCompanyMappings(SecurityClass security, Guid groupGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			using (var cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "erv.usp_PropagateCompanyUserGroupMappingsByUserGroup";
				cmd.Parameters.Add("@SourceUserGroupGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@SourceUserGroupGuid"].Value = groupGuid;
				cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
				cmd.Parameters["@CreatedBy"].Value = security.UserID;
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid targetGroupGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.MODIFY_USER_GROUPS) && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			var entityToSiteMaps = new EntityToSiteMaps();

			GroupClass group = this.Get(security, targetGroupGuid);
			if (group.IdentityGuid.IsEmpty())
			{
				throw new Exception("Group Not Found");
			}

			if (group.IsAdminGroup)
			{
				throw new Exception("[Cannot Purge] " + group.ID);
			}

			// Delete all right mappings associated to a group.
			this.DeleteAllRightsAssociatedToGroup(security, group.IdentityGuid);

			//this.UpdateRights(security, null, group, true);
			this.UpdateUserGroupMaps(security, null, group, true);
			this.UpdateCompanies(security, null, group);

			var groupTransactionAliasMaps = new GroupTransactionAliasMapsClass();
			groupTransactionAliasMaps.Purge(security, group.IdentityGuid, Guid.Empty);

			var groupLedgerViewMaps = new GroupLedgerViewMapsClass();
			groupLedgerViewMaps.Purge(security, group.IdentityGuid, Guid.Empty);

			// Purge from EntityToSiteMap
			EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(
				security, group.EntityType, targetGroupGuid);

			foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
			{
				entityToSiteMap.ID = group.ID;
				entityToSiteMaps.Purge(security, entityToSiteMap);
			}

			using (SqlCommand cmd = group.PurgeSQLCmd())
			{
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

      /// <summary>
      /// Return the Permission set for a given user down the site hierarchy from a given site down
      /// </summary>
      /// <param name="security"></param>
      /// <param name="userGuid"></param>
      /// <param name="siteGuid"></param>
      /// <returns></returns>
      public GroupCollectionClass EnumerateByUserForSiteHierarchy(SecurityClass security, Guid userGuid, Guid siteGuid)
      {
         if (security == null)
         {
            throw new ArgumentNullException("security");
         }

         if (security.UserGuid != userGuid && !security.HasRight(RIGHT.VIEW_USER_GROUPS)
             && !security.HasRight(RIGHT.MODIFY_USER_GROUPS) && !security.HasRight(RIGHT.VIEW_USERS)
             && !security.HasRight(RIGHT.MODIFY_USERS) && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
         {
            throw new FMInsufficientRightsException();
         }

         DataSet set;
         using (SqlCommand cmd = new SqlCommand())
         {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "map.usp_GetUserPermissionForSiteHierarchy";
            cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier);
            if (security.UserGuid == Guid.Empty)
            {
               cmd.Parameters["@UserGuid"].Value = DBNull.Value;
            }
            else
            {
               cmd.Parameters["@UserGuid"].Value = security.UserGuid;
            }

            cmd.Parameters.Add("@StartSiteGuid", SqlDbType.UniqueIdentifier);
            if (siteGuid == Guid.Empty)
            {
               cmd.Parameters["@StartSiteGuid"].Value = DBNull.Value;
            }
            else
            {
               cmd.Parameters["@StartSiteGuid"].Value = siteGuid;
            }

            set = this.ConsolidatedDA.GetDataSet(cmd, security);
         }

         GroupCollectionClass groupCollection = new GroupCollectionClass();
         GroupClass group = new GroupClass();
         Guid lastSiteGuid = Guid.Empty;
         Guid lastUserGroupGuid = Guid.Empty;

         if (set != null && set.Tables.Count > 0)
         {
            DataTable table = set.Tables[0];
            foreach (DataRow row in table.Rows)
            {
               var newSiteGuid = (Guid)row["SiteGuid"];
               var newGroupGuid = (Guid)row["GroupGuid"];

               if (newSiteGuid != lastSiteGuid || newGroupGuid != lastUserGroupGuid)
               {
                  lastSiteGuid = newSiteGuid;
                  lastUserGroupGuid = newGroupGuid;

                  group = new GroupClass
                  {
                     IdentityGuid = DataObject.getValue(row["GroupGuid"], Guid.Empty),
                     ID = DataObject.getValue(row["GroupID"], string.Empty),
                     Description = DataObject.getValue(row["GroupDescription"], string.Empty),
                     SiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty),
                     SiteID = DataObject.getValue(row["SiteId"], string.Empty)
                  };

                  groupCollection.Add(group);
               }

               RightClass right = new RightClass
               {
                  IdentityGuid = DataObject.getValue(row["RightGuid"], Guid.Empty),
                  RightIndex = DataObject.getValue(row["RightIndex"], 0),
                  Name = DataObject.getValue(row["RightName"], string.Empty),
                  Description = DataObject.getValue(row["RightDescription"], string.Empty)
               };

               group.RightCollectionExt.Add(right);
            }
         }

         return groupCollection;
      }
      #endregion

      #region Explicit Interface Methods

      void IDependency.Insert(SecurityClass security, BaseDataObject Object, bool preOperation)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (Object == null)
			{
				throw new ArgumentNullException("Object");
			}
		}

		void IDependency.Purge(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (Object == null)
			{
				throw new ArgumentNullException("Object");
			}

			// Deleted/Undelete Groups
			if (typeof(SiteClass).IsInstanceOfType(Object))
			{
				var site = (SiteClass)Object;
				GroupCollectionClass groupCollection = this.Enumerate(security);
				var entityToSiteMaps = new EntityToSiteMaps();
				foreach (GroupClass group in groupCollection)
				{
					if (site.SiteGuid == group.SiteGuid)
					{
						this.Purge(security, group.IdentityGuid);
					}
					else
					{
						var entityToSiteMap = new EntityToSiteMapClass(group) { SiteGuid = site.SiteGuid };
						entityToSiteMaps.Purge(security, entityToSiteMap);
					}
				}
			}
		}

		void IDependency.Update(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (Object == null)
			{
				throw new ArgumentNullException("Object");
			}

			if (typeof(SiteClass).IsInstanceOfType(Object))
			{
				var site = (SiteClass)Object;
				GroupCollectionClass groupCollection = this.Enumerate(security);
				var entityToSiteMaps = new EntityToSiteMaps();
				foreach (GroupClass group in groupCollection)
				{
					if (site.SiteGuid == group.SiteGuid)
					{
						EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(
							security, group.EntityType, group.IdentityGuid);
						foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
						{
							if (entityToSiteMap.SiteGuid != site.SiteGuid)
							{
								entityToSiteMap.ID = group.ID;
								entityToSiteMaps.Purge(security, entityToSiteMap);
							}
						}
					}
				}
			}
		}

		#endregion

		#region Methods

		protected void UpdateRights(SecurityClass security, GroupClass @group, GroupClass oldGroup, bool purgeOld)
		{
			var groupRightMaps = new GroupRightMapsClass();

			if (@group != null && @group.RightCollection != null)
			{
				foreach (RIGHT right in @group.RightCollection)
				{
					if (oldGroup != null)
					{
						int item;
						for (item = 0; item < oldGroup.RightCollection.Count; item++)
						{
							RIGHT existingRight = oldGroup.RightCollection[item];
							if (existingRight == right)
							{
								break;
							}
						}

						if (item == oldGroup.RightCollection.Count)

						{
							if (!purgeOld)
							{
                        var groupRightMap = new GroupRightMapClass
                        {
                           Right = right,
                           GroupGuid = @group.IdentityGuid,
                           ID = right.ToString()
                        };
                        groupRightMaps.Add(security, groupRightMap);
							}
						}
						else
						{
							oldGroup.RightCollection.RemoveAt(item);
						}
					}
					else
					{
						if (!purgeOld)
						{
                     var groupRightMap = new GroupRightMapClass
                     {
                        Right = right,
                        GroupGuid = @group.IdentityGuid,
                        ID = right.ToString()
                     };
                     groupRightMaps.Add(security, groupRightMap);
						}
					}
				}
			}

			if (oldGroup != null)
			{
				foreach (RIGHT right in oldGroup.RightCollection)
				{
					groupRightMaps.Purge(security, oldGroup.IdentityGuid, right);
				}
			}
		}

		protected void UpdateUserGroupMaps(SecurityClass security, GroupClass group, GroupClass oldGroup, bool purgeOld)
		{
			var userGroupMaps = new UserGroupMaps();

			if (group != null)
			{
				for (int item = 0; item < group.UserGroupMapCollection.Count; item++)
				{
					UserGroupMapClass userGroupMap = group.UserGroupMapCollection[item];
					userGroupMap.GroupGuid = group.IdentityGuid;
					userGroupMap.SiteGuid = security.SiteGuid;

					if (oldGroup == null)
					{
						if (!purgeOld)
						{
							userGroupMaps.Add(security, userGroupMap);
						}
					}
					else
					{
						if (oldGroup.UserGroupMapCollection.Find(x => x.UserGuid == userGroupMap.UserGuid) == null)
						{
							if (!purgeOld)
							{
								userGroupMaps.Add(security, userGroupMap);
							}
						}
						else
						{
							oldGroup.UserGroupMapCollection.Remove(userGroupMap);
						}
					}
				}
			}

			if (oldGroup != null)
			{
				foreach (UserGroupMapClass userGroupMap in oldGroup.UserGroupMapCollection)
				{
					userGroupMaps.Purge(security, userGroupMap.UserGuid, userGroupMap.GroupGuid, userGroupMap.SiteGuid);
				}
			}
		}

		private void UpdateCompanies(SecurityClass security, GroupClass Group, GroupClass OldGroup)
		{
			var companyMaps = new CompanyMapsClass();

			if (Group != null)
			{
				foreach (CompanyMapClass newCompanyMap in Group.CompanyMapCollection)
				{
					newCompanyMap.AssignedToGuid = Group.IdentityGuid;
					newCompanyMap.AssignedToID = Group.ID;

					if (OldGroup != null)
					{
						int item = 0;
						foreach (CompanyMapClass existingCompanyMap in OldGroup.CompanyMapCollection)
						{
							if (existingCompanyMap.AssignedGuid == newCompanyMap.AssignedGuid)
							{
								break;
							}

							item++;
						}

						Guid siteGuid = newCompanyMap.SiteGuid;

						if (newCompanyMap.SiteGuid == Guid.Empty)
						{
							siteGuid = security.SiteGuid;
						}

						if (item == OldGroup.CompanyMapCollection.Count)
						{
							companyMaps.Add(security, newCompanyMap);
						}
						else
						{
							OldGroup.CompanyMapCollection.RemoveAt(item);
						}
					}
					else
					{
						companyMaps.Add(security, newCompanyMap);
					}
				}
			}

			if (OldGroup != null)
			{
				foreach (CompanyMapClass existingCompanyMap in OldGroup.CompanyMapCollection)
				{
					companyMaps.Purge(security, existingCompanyMap.IdentityGuid, existingCompanyMap.Type);
				}
			}
		}

		/// <summary>
		/// This method will call an SP to delete all rights associated to a group.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="groupGuid">The group Guid used to delete the right mapping.</param>
		private void DeleteAllRightsAssociatedToGroup(SecurityClass security, Guid groupGuid)
        {
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_DeleteGroupToRightByGroupGuid";
				cmd.Parameters.Add("@GroupGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@GroupGuid"].Value = groupGuid;

				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// Returns a RecordVersioning-aware guid of a Company for a UserGroup mapping
		/// </summary>
		/// <param name="companyGuid">
		/// </param>
		/// <param name="siteGuid">
		/// </param>
		private Guid GetCompanyReferenceGuid(SecurityClass security, Guid companyGuid, Guid siteGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			DataSet set = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "erv.usp_GetCompanyReferenceGuidForUserGroup";
				cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@CompanyGuid"].Value = companyGuid;
				cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@SiteGuid"].Value = siteGuid;
				//SqlParameter sqlParamResult = cmd.Parameters.Add("@result", SqlDbType.UniqueIdentifier);
				set = ConsolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];

			if (table.Rows.Count > 0)
			{
				DataRow row = table.Rows[0];
				return DataObject.getValue<Guid>(row["CompanyGuid"], Guid.Empty);
			}

			return Guid.Empty;
		}

		private void Validate(string groupdId)
		{
			if (string.IsNullOrEmpty(groupdId))
			{
				throw new Exception("User Group Name Required");
			}
		}

		private void Validate(GroupClass group)
		{
			if (string.IsNullOrEmpty(group.ID))
			{
				throw new Exception("User Group Name Required");
			}
		}

		#endregion
	}
}