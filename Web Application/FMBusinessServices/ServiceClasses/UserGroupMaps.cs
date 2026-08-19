// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserGroupMaps.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Implementation for IUserGroupMaps service class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;

	/// <summary>
	/// Implementation for IUserGroupMaps service class
	/// </summary>
	[SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class UserGroupMaps : IUserGroupMaps, IDependency
	{
		#region Constants and Fields

		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		#endregion

		#region Public Methods and Operators

		public DataSet EnumerateByUserPermissionGrid(SecurityClass security, Guid modifyUser, Guid siteGuid, bool loadChildrenSites, string filter)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			using (var cmd = new SqlCommand())
			{
				var userGroupMap = new UserGroupMapClass();
				userGroupMap.EnumerateByUserPermissionGridSQL(cmd, security, modifyUser, siteGuid, loadChildrenSites, filter);
				return this.ConsolidatedDA.GetDataSet(cmd, security);
			}
		}

		public UserGroupMapCollectionClass EnumerateBySite(SecurityClass security, Guid siteGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			using (var cmd = new SqlCommand())
			{
				var userGroupMap = new UserGroupMapClass();
				userGroupMap.EnumerateBySiteSQL(cmd, security, siteGuid, ContextUtil.IsInTransaction);
				DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);
	
				var toRet = new UserGroupMapCollectionClass();

				var table = set.Tables[0];
				while (table.Rows.Count != 0)
				{
					var groupMap = new UserGroupMapClass();
					groupMap.Load(set);
					toRet.Add(groupMap);
					table.Rows.RemoveAt(0);
				}

				return toRet;
			}
		}

		public UserGroupMapCollectionClass EnumerateByGroupAndSite(SecurityClass security, Guid groupGuid, Guid siteGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			using (var cmd = new SqlCommand())
			{
				var userGroupMap = new UserGroupMapClass();
				userGroupMap.EnumerateByGroupAndSiteSQL(cmd, security, groupGuid, siteGuid, ContextUtil.IsInTransaction);
				DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);

				var toRet = new UserGroupMapCollectionClass();
	
				var table = set.Tables[0];
				while (table.Rows.Count != 0)
				{
					var groupMap = new UserGroupMapClass();
					groupMap.Load(set);
					toRet.Add(groupMap);
					table.Rows.RemoveAt(0);
				}

				return toRet;
			}
		}

		public UserGroupMapCollectionClass EnumerateByGroup(SecurityClass security, Guid groupGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			using (var cmd = new SqlCommand())
			{
				var userGroupMap = new UserGroupMapClass();
				userGroupMap.EnumerateByGroupSQL(cmd, security, groupGuid, ContextUtil.IsInTransaction);
				DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);

				var toRet = new UserGroupMapCollectionClass();

				var table = set.Tables[0];
				while (table.Rows.Count != 0)
				{
					userGroupMap = new UserGroupMapClass();
					userGroupMap.Load(set);
					toRet.Add(userGroupMap);
					table.Rows.RemoveAt(0);
				}

				return toRet;
			}
		}

		public UserGroupMapCollectionClass EnumerateByUser(SecurityClass security, Guid userGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			using (var cmd = new SqlCommand())
			{
				var userGroupMap = new UserGroupMapClass();
				userGroupMap.EnumerateByUserSQL(cmd, security, userGuid, ContextUtil.IsInTransaction);
				DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);

				var toRet = new UserGroupMapCollectionClass();

				var table = set.Tables[0];
				while (table.Rows.Count != 0)
				{
					userGroupMap = new UserGroupMapClass();
					userGroupMap.Load(set);
					toRet.Add(userGroupMap);
					table.Rows.RemoveAt(0);
				}

				return toRet;
			}
		}


		public UserGroupMapCollectionClass EnumerateByUserAndSite(SecurityClass security, Guid userGuid, Guid siteGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			using (var cmd = new SqlCommand())
			{
				var userGroupMap = new UserGroupMapClass();
				userGroupMap.EnumerateByUserAndSiteSQL(cmd, security, userGuid, siteGuid, ContextUtil.IsInTransaction);
				DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);

				var toRet = new UserGroupMapCollectionClass();
	
				var table = set.Tables[0];
				while (table.Rows.Count != 0)
				{
					var groupMap = new UserGroupMapClass();
					groupMap.Load(set);
					toRet.Add(groupMap);
					table.Rows.RemoveAt(0);
				}

				return toRet;
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Add(SecurityClass security, UserGroupMapClass userGroupMap)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (userGroupMap == null)
			{
				throw new ArgumentNullException("userGroupMap");
			}

			if (!security.HasRight(RIGHT.MODIFY_USERS) && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			using (var cmd = new SqlCommand())
			{
				userGroupMap.SelectSQL(cmd, ContextUtil.IsInTransaction);
				DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);
				if (set.Tables[0].Rows.Count != 0)
				{
					return;
				}
			}

			userGroupMap.CreatedDate = DateTimeOffset.Now;
			userGroupMap.CreatedBy = security.UserID;
			userGroupMap.UpdatedDate = userGroupMap.CreatedDate;
			userGroupMap.UpdatedBy = security.UserID;
			userGroupMap.Deleted = false;
			using (var cmd = new SqlCommand())
			{
				userGroupMap.InsertSQL(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void UpdateDenyFlag(SecurityClass security, UserGroupMapClass userGroupMap)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (userGroupMap == null)
            {
                throw new ArgumentNullException("userGroupMap");
            }

            if (!security.HasRight(RIGHT.MODIFY_USERS) && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
            {
                throw new FMInsufficientRightsException();
            }

            using (var cmd = new SqlCommand())
            {
                userGroupMap.UpdateDenySQL(cmd);
                DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);
                return;
            }
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid userGuid, Guid groupGuid, Guid siteGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.MODIFY_USERS) && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			if (UserClass.IsAdministratorGuid(userGuid)
			&& GroupClass.IsAdminGroupGuid(groupGuid)
			&& siteGuid == Guids.SiteAdminGuid)
			{
				throw new Exception("Cannot Purge SiteAdmin Administrator User Group Map");
			}

			var userGroupMap = new UserGroupMapClass
				{
					UserGuid = userGuid, 
					GroupGuid = groupGuid,
					SiteGuid = siteGuid
				};

			using (var cmd = new SqlCommand())
			{
				userGroupMap.PurgeSQL(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}
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

			// Purge Site
			if (typeof(SiteClass).IsInstanceOfType(Object))
			{
				var site = Object as SiteClass;
				var userGroupMapCollection = this.EnumerateBySite(security, site.IdentityGuid);
		
				foreach (UserGroupMapClass userGroupMap in userGroupMapCollection)
				{
					using (var cmd = new SqlCommand())
					{
						userGroupMap.PurgeSQL(cmd);
						this.ConsolidatedDA.ExecuteQuery(security, cmd);
					}
				}
			}

			else if (typeof(EntityToSiteMapClass).IsInstanceOfType(Object))
			{
				var entityToSiteMap = Object as EntityToSiteMapClass;
				UserGroupMapCollectionClass userGroupMapCollection = null;
				if (entityToSiteMap.TypeID == ENTITY_TYPE.USER)
				{
					userGroupMapCollection = this.EnumerateByUserAndSite(
						security, entityToSiteMap.IdentityGuid, entityToSiteMap.SiteGuid);
				}
				else if(entityToSiteMap.TypeID == ENTITY_TYPE.GROUP)
				{
					userGroupMapCollection = this.EnumerateByGroupAndSite(
						security, entityToSiteMap.IdentityGuid, entityToSiteMap.SiteGuid);

				}

				if (userGroupMapCollection != null)
				{
					foreach (UserGroupMapClass userGroupMap in userGroupMapCollection)
					{
						using (var cmd = new SqlCommand())
						{						
							userGroupMap.PurgeSQL(cmd);
							this.ConsolidatedDA.ExecuteQuery(security, cmd);
						}
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

		}

		#endregion

	}
}