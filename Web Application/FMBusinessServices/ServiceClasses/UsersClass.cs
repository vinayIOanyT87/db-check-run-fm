// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UsersClass.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Implementation for IUsers service class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Globalization;
	using System.Runtime.InteropServices;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.UtilityObjects;

	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;

	/// <summary>
	/// Implementation for IUsers service class
	/// </summary>
	[SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class UsersClass : IUsers, IDependency
	{
		#region Constants and Fields
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();
		#endregion

		#region Public Methods and Operators
		/// <summary>
		/// Adds the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="user">The user.</param>
		/// <returns>The Guid of the newly added user.</returns>
		/// <exception cref="System.ArgumentNullException">security</exception>
		/// <exception cref="System.Exception">Access Denied</exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, UserClass user)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			if (!security.HasRight(RIGHT.MODIFY_USERS))
			{
				throw new FMInsufficientRightsException();
			}

			// The validation method uses the User site index to retrieve
			// password configuration. Therefore, ensure the User site index
			// is set.
			if (user.SiteGuid == Guid.Empty)
			{
				user.SiteGuid = security.SiteGuid;
				user.SiteID = security.SiteID;
			}

			Validate(security, user, true, true, false, string.Empty);

			// Change to checking across entire system
			using (var cmd = new SqlCommand())
			{
				user.SelectUserIdCount(cmd, security, user.ID);
				DataSet ds = this.ConsolidatedDA.GetDataSet(cmd, security);
				var userCount = (int)(ds.Tables[0].Rows[0][0]);
				if (userCount > 0)
				{
					throw (new Exception("User Exists"));
				}
			}

			// For Single Sign On Mode, do not check for the user to have a
			// SQL Server account.
			if (this.IsSsoMode(security) == false)
			{
				//do not check database logins or users while running in Azure
				using (var cmd = new SqlCommand())
				{
					user.SelectLoginAccountCount(cmd, security, user.ID);
					DataSet ds = this.ConsolidatedDA.GetDataSet(cmd, security);
					var loginCount = (int)(ds.Tables[0].Rows[0][0]);
					if (loginCount > 0)
					{
						throw (new Exception("A database LOGIN account with the same name exists. Please contact database administrator."));
					}
				}

				using (var cmd = new SqlCommand())
				{
					user.SelectUserAccountCount(cmd, security, user.ID);
					DataSet ds = this.ConsolidatedDA.GetDataSet(cmd, security);
					var userCount = (int)(ds.Tables[0].Rows[0][0]);
					if (userCount > 0)
					{
						throw (new Exception("A database USER account with the same name exists. Please contact database administrator."));
					}
				}
			}

		    user.PasswordHistory1 = user.Password;
			user.PasswordTimestamp = DateTimeOffset.Now;
			user.SiteGuid = security.SiteGuid;
			user.CreatedDate = DateTimeOffset.Now;
			user.CreatedBy = security.UserID;
			user.UpdatedDate = user.CreatedDate;
			user.UpdatedBy = security.UserID;
			user.IdentityGuid = Guid.NewGuid();

			using (var cmd = new SqlCommand())
			{
				user.InsertSQL(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);

				// Create Entity to Site Map
				var entityToSiteMaps = new EntityToSiteMaps();
				var entityToSiteMap = new EntityToSiteMapClass(user);
				entityToSiteMaps.Add(security, entityToSiteMap, this.GetType().GUID);

				this.UpdateUserGroupMaps(security, user, null);

				return user.IdentityGuid;
			}
		}

		/// <summary>
		/// Modifies the inactivity lockout.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="user">The user.</param>
		/// <exception cref="System.ArgumentNullException">
		/// Security
		/// or
		/// User
		/// </exception>
		public void ModifyInactivityLockout(SecurityClass security, UserClass user)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			user.UpdatedDate = DateTime.UtcNow;
			user.UpdatedBy = "administrator";

			using (var command = new SqlCommand())
			{
				user.UpdateInactivityLockoutSQL(command);
				ConsolidatedDA.ExecuteQuery(security, command);
			}
		}


		/// <summary>
		/// Enumerates the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <returns>Collection of user objects.</returns>
		/// <exception cref="System.ArgumentNullException">security</exception>
		/// <exception cref="System.Exception">Access Denied</exception>
		public UserCollectionClass Enumerate(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_USERS) 
				&& !security.HasRight(RIGHT.MODIFY_USERS)
			    && !security.HasRight(RIGHT.VIEW_USER_GROUPS) 
				&& !security.HasRight(RIGHT.MODIFY_USER_GROUPS)
			    && !security.HasRight(RIGHT.VIEW_AUDIT_LOGS) 
				&& !security.HasRight(RIGHT.VIEW_QUALITYTAG_RECORD)
			    && !security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD) 
				&& !security.HasRight(RIGHT.VIEW_QUALITYTAG_LOGS)
			    && !security.HasRight(RIGHT.MODIFY_QUALITYTAG_LOGS) 
				&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			var user = new UserClass();
			using (var cmd = new SqlCommand())
			{
				user.EnumerateSQL(cmd, security);
				DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);
				var userCollection = new UserCollectionClass();

				DataTable table = set.Tables[0];
				while (table.Rows.Count != 0)
				{
					user = new UserClass();
					user.LoadObject(set);
					userCollection.Add(user);
					table.Rows.RemoveAt(0);
				}

				return userCollection;
			}
		}

	    public DataSet EnumerateActiveDirectoryUsers(SecurityClass security)
	    {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!security.HasRight(RIGHT.VIEW_USERS) && !security.HasRight(RIGHT.MODIFY_USERS))
            {
                throw new FMInsufficientRightsException();
            }

            using (var cmd = new SqlCommand())
            {
                UserDAO.EnumerateActiveDirectoryUsersSQL(cmd);
                DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);

                return set;
            }
        }

		//**********************************************************************************************************************
		// This method will return a user object collection of the users that meet the security and
		// filter criterion. This method is the same as the Enumerate method
		// with the exception that the user has supplied a filter to narrow the search on the list of users.
		//**********************************************************************************************************************
		public UserCollectionClass EnumerateAndFilter(SecurityClass security, string filter)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_USERS) && !security.HasRight(RIGHT.MODIFY_USERS))
			{
				throw new FMInsufficientRightsException();
			}

			var user = new UserClass();

			using (var cmd = new SqlCommand())
			{
				user.EnumerateAndFilterSQL(cmd, security, filter);
				DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);

				var userCollection = new UserCollectionClass();

				DataTable table = set.Tables[0];
				while (table.Rows.Count != 0)
				{
					user = new UserClass();
					user.LoadObject(set);
					userCollection.Add(user);
					table.Rows.RemoveAt(0);
				}

				return userCollection;
			}
		}

        //this function is designed for Reset Password
        public UserCollectionClass GetUsersByIDWithoutSite(SecurityClass security, string userID)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            if (userID.Equals(security.UserID, StringComparison.CurrentCultureIgnoreCase) == false
                && !security.HasRight(RIGHT.VIEW_USERS) && !security.HasRight(RIGHT.MODIFY_USERS)
                && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
            {
                throw new FMInsufficientRightsException();
            }

            var user = new UserClass { ID = userID };

            using (var cmd = new SqlCommand())
            {
                cmd.CommandText = "SELECT * FROM tblUsers WHERE UserID = @UID";
                cmd.Parameters.AddWithValue("@UID", user.ID);



                //if found more than one user records, find the one with an email address
                var userCollection = new UserCollectionClass();
                DataSet dataset = this.ConsolidatedDA.GetDataSet(cmd, security);
                DataTable table = dataset.Tables[0];
                while (table.Rows.Count != 0)
                {
                    user = new UserClass();
                    user.LoadObject(dataset);
                    userCollection.Add(user);
                    table.Rows.RemoveAt(0);
                }

                return userCollection;
            }
        }
        public UserCollectionClass EnumerateByGroup(SecurityClass security, Guid groupGuid)
		{
			return EnumerateByGroupAndSite(security, groupGuid, security.SiteGuid);
		}


		public UserCollectionClass EnumerateByGroupAndSite(SecurityClass security, Guid groupGuid, Guid siteGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_USERS)
				&& !security.HasRight(RIGHT.MODIFY_USERS)
				&& !security.HasRight(RIGHT.VIEW_USER_GROUPS)
				&& !security.HasRight(RIGHT.MODIFY_USER_GROUPS)
				&& !security.HasRight(RIGHT.MODIFY_COMPANY_DATA)
				&& !security.HasRight(RIGHT.MODIFY_QUERIES)
				&& !security.HasRight(RIGHT.VIEW_TRANSACTION_ALIASES)
				&& !security.HasRight(RIGHT.VIEW_QUERIES)
				&& !security.HasRight(RIGHT.CONFIGURE_QUERIES)
				&& !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA)
				&& !security.HasRight(RIGHT.EXPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			var user = new UserClass { SiteGuid = siteGuid };

			using (var cmd = new SqlCommand())
			{
				user.EnumerateByGroupSQL(cmd, groupGuid, ContextUtil.IsInTransaction);
				DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);
				var userCollection = new UserCollectionClass();

				DataTable table = set.Tables[0];
				while (table.Rows.Count != 0)
				{
					user = new UserClass();
					user.LoadObject(set);
					userCollection.Add(user);
					table.Rows.RemoveAt(0);
				}

				return userCollection;
			}
		}

		public UserCollectionClass EnumerateForParentSiteByAssignedUser(SecurityClass security, Guid siteGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_USERS)
				&& !security.HasRight(RIGHT.MODIFY_USERS)
			    && !security.HasRight(RIGHT.VIEW_USER_GROUPS)
				&& !security.HasRight(RIGHT.MODIFY_USER_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			var user = new UserClass();
			user.SiteGuid = security.SiteGuid;

			using (var cmd = new SqlCommand())
			{
				user.EnumerateForParentSiteByAssignedUserSQL(cmd, security, siteGuid, ContextUtil.IsInTransaction);
				DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);
				var userCollection = new UserCollectionClass();

				DataTable table = set.Tables[0];
				while (table.Rows.Count != 0)
				{
					user = new UserClass();
					user.LoadObject(set);
					userCollection.Add(user);
					table.Rows.RemoveAt(0);
				}

				return userCollection;
			}
		}

		public UserCollectionClass EnumerateForSiteByAssignedUser(SecurityClass security, Guid siteGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_USERS) 
				&& !security.HasRight(RIGHT.MODIFY_USERS)
			    && !security.HasRight(RIGHT.VIEW_USER_GROUPS) 
				&& !security.HasRight(RIGHT.MODIFY_USER_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			var user = new UserClass();
			user.SiteGuid = security.SiteGuid;

			using (var cmd = new SqlCommand())
			{
				user.EnumerateForSiteByAssignedUserSQL(cmd, security, siteGuid, ContextUtil.IsInTransaction);
				DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);
				var userCollection = new UserCollectionClass();

				DataTable table = set.Tables[0];
				while (table.Rows.Count != 0)
				{
					user = new UserClass();
					user.LoadObject(set);
					userCollection.Add(user);
					table.Rows.RemoveAt(0);
				}

				return userCollection;
			}
		}


		public UserClass Get(SecurityClass security, Guid userGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (userGuid != security.UserGuid
			&& !security.HasRight(RIGHT.VIEW_USERS)
			&& !security.HasRight(RIGHT.MODIFY_USERS)
			&& !security.HasRight(RIGHT.VIEW_DISPATCH))
			{
				throw new FMInsufficientRightsException();
			}

			var user = new UserClass { IdentityGuid = userGuid };

			using (var cmd = new SqlCommand())
			{
				user.SelectSQL(cmd, ContextUtil.IsInTransaction);
				user.LoadObject(this.ConsolidatedDA.GetDataSet(cmd, security));

				var userGroupMaps = new UserGroupMaps();
				user.UserGroupMapCollection = userGroupMaps.EnumerateByUserAndSite(security, userGuid, security.SiteGuid);

				return user;
			}
		}

		public UserClass GetBySite(SecurityClass security, Guid userGuid, Guid siteGuid )
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (userGuid != security.UserGuid && !security.HasRight(RIGHT.VIEW_USERS) && !security.HasRight(RIGHT.MODIFY_USERS))
			{
				throw new FMInsufficientRightsException();
			}

			var user = new UserClass { IdentityGuid = userGuid };

			using (var cmd = new SqlCommand())
			{
				user.SelectSQL(cmd, ContextUtil.IsInTransaction);
				user.LoadObject(this.ConsolidatedDA.GetDataSet(cmd, security));

				var userGroupMaps = new UserGroupMaps();
				user.UserGroupMapCollection = userGroupMaps.EnumerateByUserAndSite(security, userGuid, siteGuid);

				return user;
			}
		}

		public UserClass GetByID(SecurityClass security, string userID)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (userID.Equals(security.UserID, StringComparison.CurrentCultureIgnoreCase) == false
			    && !security.HasRight(RIGHT.VIEW_USERS) && !security.HasRight(RIGHT.MODIFY_USERS)
			    && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			var user = new UserClass { ID = userID };

			using (var cmd = new SqlCommand())
			{
				user.SelectByIdsqlParameterized(cmd, security, ContextUtil.IsInTransaction);
				user.LoadObject(this.ConsolidatedDA.GetDataSet(cmd, security));

				var userGroupMaps = new UserGroupMaps();
				user.UserGroupMapCollection = userGroupMaps.EnumerateByUserAndSite(security, user.IdentityGuid, security.SiteGuid);

				return user;
			}
		}
        

	    public UserClass GetByIDForLogOn(SecurityClass security, string userID)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (userID != security.UserID)
			{
				throw new ArgumentException("security");
			}

			var user = new UserClass { ID = userID };

			using (var cmd = new SqlCommand())
			{
				user.SelectByIdsqlParameterized(cmd, security, ContextUtil.IsInTransaction);
				user.LoadObject(this.ConsolidatedDA.GetDataSet(cmd, DBAccess.ServiceLoginAccess, string.Empty));
				security.UserGuid = user.IdentityGuid;
				security.UserGuid = user.IdentityGuid;

				var userGroupMaps = new UserGroupMaps();
				user.UserGroupMapCollection=userGroupMaps.EnumerateByUser(security, user.IdentityGuid);

				return user;
			}
		}

		public UserClass GetDuringLogOn(SecurityClass security, Guid guid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (guid != security.UserGuid && !security.HasRight(RIGHT.VIEW_USERS) && !security.HasRight(RIGHT.MODIFY_USERS))
			{
				throw new FMInsufficientRightsException();
			}

			var serviceSecurity = new SecurityClass { UserID = DBAccess.ServiceLoginAccess };

			var user = new UserClass { IdentityGuid = guid };

			using (var cmd = new SqlCommand())
			{
				user.SelectSQL(cmd, ContextUtil.IsInTransaction);
				user.LoadObject(this.ConsolidatedDA.GetDataSet(cmd, serviceSecurity));

				var userGroupMaps = new UserGroupMaps();
				user.UserGroupMapCollection = userGroupMaps.EnumerateByUserAndSite(security, user.IdentityGuid, security.SiteGuid);

				return user;
			}
		}

		public Guid GetIdentityGuid(SecurityClass security, string userID)
		{
			return this.GetIdentityGuidBySevice(security, userID, false);
		}

		public Guid GetIdentityGuidBySevice(SecurityClass security, string userID, bool service)
		{
			UserClass user;

			if(service)
			{
				user = this.GetByIDForLogOn(security, userID);
			}
			else
			{
				user = this.GetByID(security, userID);
			}

			if (user != null)
			{
				return user.IdentityGuid;
			}

			return Guid.Empty;
		}

		/// <summary>
		///     This method updates the user information in the database.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="user">
		/// </param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, UserClass user)
		{
			UserClass oldUser = this.Get(security, user.IdentityGuid);

			if (oldUser.IdentityGuid.IsEmpty())
			{
				throw (new Exception("User Not Found"));
			}


			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			// User can change his own account, to allow Password change
			if (user.IdentityGuid != security.UserGuid && !security.HasRight(RIGHT.MODIFY_USERS))
			{
				throw new FMInsufficientRightsException();
			}

			Validate(security, user, false, false, false, string.Empty);

			Guid targetUserGuid = this.GetIdentityGuid(security, user.ID);

			if (targetUserGuid.IsNotEmptyAndNotEqualTo(user.IdentityGuid))
			{
				throw new Exception("User Exists");
			}

			// Recompute Password if changed - Don't evaluate if the user is locked out due to inactivity
			if (user.InactivityLockout == false)
			{
				if(string.IsNullOrEmpty(user.Password))
				{
					// If a user is editing his own account and the ChangePassword Flag
					// is set they must change the Password
					if (security.UserGuid == user.IdentityGuid && oldUser.ChangePassword)
					{
						throw new Exception("Password must be changed");
					}

					// CSI 3342 - If the UserID has changed, the Password has to be specified so the saved
					// has value will be correct.
					if (user.ID != oldUser.ID)
					{
						throw new Exception("A Password must be specified to change ID");
					}

					user.Password = oldUser.Password;
				}
				else
				{
					user.PasswordTimestamp = DateTimeOffset.Now;
				}
			}

			else if(user.Password == string.Empty)
			{
				user.Password = oldUser.Password;
			}

			// Reset activity timestamp if setting from locked-out to not locked-out
			if (oldUser.InactivityLockout && user.InactivityLockout == false)
			{
				user.PasswordTimestamp = DateTimeOffset.Now;
				user.LastLoginDate = DateTimeOffset.Now;
			}

			user.UpdatedDate = DateTimeOffset.Now;
			user.UpdatedBy = security.UserID;

			using (var cmd = new SqlCommand())
			{
				user.UpdateSQL(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			var entityToSiteMaps = new EntityToSiteMaps();
			EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(
				security, user.EntityType, user.IdentityGuid);

			if (user.SiteGuid != oldUser.SiteGuid)
			{
				// Purge from EntityToSiteMap
				foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
				{
					entityToSiteMap.ID = user.ID;
					entityToSiteMaps.Purge(security, entityToSiteMap);
				}

				// Create Entity to Site Map
				var newEntityToSiteMap = new EntityToSiteMapClass(user);
				entityToSiteMaps.Add(security, newEntityToSiteMap, this.GetType().GUID);
			}

			if (security.HasRight(RIGHT.MODIFY_USERS))
			{
				this.UpdateUserGroupMaps(security, user, oldUser);
			}
		}

		/// <summary>
		///     This method will update the Password count column only. This
		///     column is used to track the number of failure attempts.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="user"></param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void ModifyPasswordCount(SecurityClass security, UserClass user)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			Guid targetUserGuid = this.GetIdentityGuidBySevice(security, user.ID, true);

			if (targetUserGuid.IsNotEmptyAndNotEqualTo(user.IdentityGuid))
			{
				throw (new Exception("User Exists"));
			}

			UserClass oldUser = this.GetDuringLogOn(security, user.IdentityGuid);

			if (oldUser.IdentityGuid.IsEmpty())
			{
				throw (new Exception("User Not Found"));
			}

			user.UpdatedDate = DateTimeOffset.Now;
			user.UpdatedBy = "administrator";

			using (var cmd = new SqlCommand())
			{
				user.UpdatePasswordCountSQL(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		///     This method updates the Password history items and then call the Modify
		///     method to perform the actually modifications.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="user"></param>
		/// <param name="oldPassword"></param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void ModifyWithPasswordHistory(SecurityClass security, UserClass user, string oldPassword)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			Guid targetUserGuid = this.GetIdentityGuid(security, user.ID);

			if (targetUserGuid.IsNotEmptyAndNotEqualTo(user.IdentityGuid))
			{
				throw (new Exception("User Exists"));
			}

			UserClass oldUser = this.Get(security, user.IdentityGuid);

			if (oldUser.IdentityGuid.IsEmpty())
			{
				throw (new Exception("User Not Found"));
			}

			Validate(security, user, false, true, oldUser.ChangePassword == false, oldPassword);

			user.PasswordHistory24 = user.PasswordHistory23;
			user.PasswordHistory23 = user.PasswordHistory22;
			user.PasswordHistory22 = user.PasswordHistory21;
			user.PasswordHistory21 = user.PasswordHistory20;
			user.PasswordHistory20 = user.PasswordHistory19;
			user.PasswordHistory19 = user.PasswordHistory18;
			user.PasswordHistory18 = user.PasswordHistory17;
			user.PasswordHistory17 = user.PasswordHistory16;
			user.PasswordHistory16 = user.PasswordHistory15;
			user.PasswordHistory15 = user.PasswordHistory14;
			user.PasswordHistory14 = user.PasswordHistory13;
			user.PasswordHistory13 = user.PasswordHistory12;
			user.PasswordHistory12 = user.PasswordHistory11;
			user.PasswordHistory11 = user.PasswordHistory10;
			user.PasswordHistory10 = user.PasswordHistory9;
			user.PasswordHistory9 = user.PasswordHistory8;
			user.PasswordHistory8 = user.PasswordHistory7;
			user.PasswordHistory7 = user.PasswordHistory6;
			user.PasswordHistory6 = user.PasswordHistory5;
			user.PasswordHistory5 = user.PasswordHistory4;
			user.PasswordHistory4 = user.PasswordHistory3;
			user.PasswordHistory3 = user.PasswordHistory2;
			user.PasswordHistory2 = user.PasswordHistory1;
			user.PasswordHistory1 = user.Password;
			user.PasswordTimestamp = DateTimeOffset.Now;

			this.Modify(security, user);
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void DisableUser(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			
			using (var cmd = new SqlCommand())
			{
				UserDAO.DisableUserSQL(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd); // SJIANG: Only DOD needs Archieve User.
			}

		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void ArchiveUser(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

	
			using (var cmd = new SqlCommand())
			{
				cmd.CommandTimeout = 600;
				UserDAO.ArchiveUserSQL(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd); // SJIANG: Only DOD needs Archieve User.
			}

		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid userGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.MODIFY_USERS) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			UserClass user = this.Get(security, userGuid);
			if (user.IdentityGuid.IsEmpty())
			{
				throw new Exception("User Not Found");
			}

			if (user.ID == security.UserID && user.SiteGuid == security.LoginSiteGuid)
			{
				throw new FMInsufficientRightsException();
			}

			if (user.IsAdministrator)
			{
				throw new Exception("[Cannot Purge] " + user.ID);
			}

			var hardwareKey = new HardwareKeyClass();
			
			this.UpdateUserGroupMaps(security, null, user);

			// Purge from EntityToSiteMap
			var entityToSiteMaps = new EntityToSiteMaps();
			EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(
				security, user.EntityType, userGuid);

			foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
			{
				entityToSiteMap.ID = user.ID;
				entityToSiteMaps.Purge(security, entityToSiteMap);
			}

			if (hardwareKey.IsDescKey())
			{
				using (var cmd = new SqlCommand())
				{
					user.ArchiveUserSQL(cmd);
					this.ConsolidatedDA.ExecuteQuery(security, cmd); // SJIANG: Only DOD needs Archieve User.
				}
			}

			var sessions = new SessionsClass();
			sessions.PurgeByUser(security, userGuid);

			var menuFavorites = new MenuFavorites();
			menuFavorites.PurgeByUser(security, userGuid);

			var dispatchColumns = new DispatchGridColumns();
			dispatchColumns.PurgeByUser(security, userGuid);

			var querys = new QueriesClass();
			querys.PurgeByUser(security, userGuid);

			var accessibilities = new AccessibilitiesClass();
			accessibilities.PurgeByUser(security, userGuid);

			var userViewStateSettings = new UserViewStateSettings();
			userViewStateSettings.PurgeByUser(security, userGuid);

			using (var cmd = new SqlCommand())
			{
				user.PurgeSQL(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void DeleteOrphanUserRecords(SecurityClass security, bool activeDirectoryUsers = true)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			using (var cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "[dbo].[usp_DeleteOrphanUserRecords]";
				cmd.Parameters.Add("@IsActiveDirectoryUsers", SqlDbType.Bit);

				cmd.Parameters["@IsActiveDirectoryUsers"].Value = activeDirectoryUsers ? 1 : 0;

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

			// Purge Users
			var siteObject = Object as SiteClass;

			if (siteObject != null)
			{
				var site = siteObject;
				UserCollectionClass userCollection = this.Enumerate(security);
				var entityToSiteMaps = new EntityToSiteMaps();

				foreach (UserClass user in userCollection)
				{
					if (site.SiteGuid == user.SiteGuid)
					{
						this.Purge(security, user.IdentityGuid);
					}
					else
					{
						var entityToSiteMap = new EntityToSiteMapClass
							{
								TypeID = user.EntityType,
								SiteGuid = site.SiteGuid,
								IdentityGuid = user.IdentityGuid
							};
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

			var siteObject = Object as SiteClass;

			if (siteObject != null)
			{
				var site = siteObject;
				UserCollectionClass userCollection = this.Enumerate(security);
				var entityToSiteMaps = new EntityToSiteMaps();

				foreach (UserClass user in userCollection)
				{
					if (site.SiteGuid == user.SiteGuid)
					{
						EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(
							security, user.EntityType, user.IdentityGuid);

						foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
						{
							if (entityToSiteMap.SiteGuid != site.SiteGuid)
							{
								entityToSiteMap.ID = user.ID;
								entityToSiteMaps.Purge(security, entityToSiteMap);
							}
						}
					}
				}
			}
		}
		#endregion

		#region Methods
		protected void UpdateUserGroupMaps(SecurityClass security, UserClass user, UserClass oldUser)
		{
			var userGroupMaps = new UserGroupMaps();

			if (user != null)
			{
				for (int item = 0; item < user.UserGroupMapCollection.Count; item++)
				{
					var userGroupMap = user.UserGroupMapCollection[item];
					userGroupMap.UserGuid = user.IdentityGuid;
					userGroupMap.SiteGuid = security.SiteGuid;

					if (oldUser == null)
					{
						userGroupMaps.Add(security, userGroupMap);
					}
					else
					{
						if (oldUser.UserGroupMapCollection.Find(x => x.GroupGuid == userGroupMap.GroupGuid) == null)
						{
							userGroupMaps.Add(security, userGroupMap);
						}
						else
						{
							oldUser.UserGroupMapCollection.Remove(userGroupMap);
						}
					}
				}
			}

			if (oldUser != null)
			{
				foreach (UserGroupMapClass userGroupMap in oldUser.UserGroupMapCollection)
				{
					userGroupMaps.Purge(security, userGroupMap.UserGuid, userGroupMap.GroupGuid, userGroupMap.SiteGuid);
				}
			}
		}

		// ReSharper disable UnusedParameter.Local
		public static void Validate(
			SecurityClass security, UserClass user, bool bAdd, bool bCheckPassword, bool bCheckMinimumTime, string oldPassword)
		{
			if (string.IsNullOrEmpty(user.ID))
			{
				throw (new Exception("ID Required"));
			}

			if (user.ID == "{None}" || user.ID == "{Unassigned}" || user.ID == "{All}")
			{
				throw new Exception("ID is reserved key word " + user.ID);
			}

			if (bAdd && user.Password == string.Empty && user.ActiveDirectoryUser == false)
			{
				throw (new Exception("Password Required"));
			}

			if (bCheckPassword && user.ActiveDirectoryUser == false)
			{
				UsersClass.ValidatePassword(security, user, bCheckMinimumTime, oldPassword);
			}
		}
		// ReSharper restore UnusedParameter.Local

		private static void ValidatePassword(SecurityClass security, UserClass user, bool bCheckMinimumTime, string oldPassword)
		{
			var sites = new SitesClass();
			Guid siteGuid = user.SiteGuid;
			SiteClass site = sites.GetUsingGuid(security, siteGuid);

			var fmSecurityVal = new FMSecurityValidation(user, site);
			fmSecurityVal.ParseUserInfo();
			fmSecurityVal.ParseConfiguration();

			var hardwareKey = new HardwareKeyClass();

			// Ensure that the new Password meets the Password security test.
			// If not, display an error dialog.
			if (fmSecurityVal.MeetsStrongPassword(user.Password) == false)
			{
				if (StrongPasswordUsage.Strong == (StrongPasswordUsage)site.StrongPasswordUse || StrongPasswordUsage.Enhanced == (StrongPasswordUsage)site.StrongPasswordUse)
				{
					throw new FMPasswordException("Your password does not meet the requirements. Check the password policy.");
				}
			}

			if (fmSecurityVal.MinimumOfCharacters(user.Password) == false)
			{
				throw new FMPasswordException("Your password does not meet the requirements. Check the password policy.");
			}

			// Check the minimum time between changes but allow the 
			// change if the administrator has set the "must change at login" setting
			if (fmSecurityVal.MinimumTimeAllowedToChange() == false && bCheckMinimumTime)
			{
				string errMsg = "Password cannot be changed for another " + fmSecurityVal.MinimumTimeAllowedToChangePassword + " days";
				throw new Exception(errMsg);
			}

			bool checkAlmostMatch = fmSecurityVal.StrongPassword != StrongPasswordUsage.None || hardwareKey.IsDescKey();

            //<summary>
            //  Checks to see if your new password equals or closely resembles a previous one
            //  (Miriam 11.12.18)
            //</summary
            if ( fmSecurityVal.PreviouslyExisted( user.Password, oldPassword, checkAlmostMatch ) )
			{
				string errMsg="";

				if (fmSecurityVal.PreviouslyExisted(user.Password, oldPassword, checkAlmostMatch).Equals(true)) 
				{
					errMsg = "Password is not sufficiently different from previously used passwords.";
				}
				throw new Exception(errMsg);
			}
		}

		/// <summary>
		/// This method gets the SSO mode.
		/// </summary>
		/// <returns>Returns true if in SSO mode, otherwise false.</returns>
		private bool IsSsoMode(SecurityClass security)
		{
			bool isSsoMode = false;

			try
			{
				var configSettings = new ConfigurationSettingsClass();
				var configSetting = configSettings.GetByKey(security, ConfigurationSettingDOClass.Key_SingleSignOnMode);

				if (configSetting != null && string.IsNullOrEmpty(configSetting.SettingValue) == false && configSetting.SettingValue == "1")
				{
					isSsoMode = true;
				}
			}
			catch (Exception)
			{
				return isSsoMode;
			}

			return isSsoMode;
		}
		#endregion
	}
}