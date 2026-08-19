namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Linq;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.Constants;

	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;
	using System.Globalization;

	/// <summary>
	/// Service providing access to point configuration data.
	/// </summary>
	[SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class UserViewStateSettings : FMServiceBase, IUserViewStateSettings, IDependency
	{
		/// <summary>
		/// Adds the specified point.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="setting">The user view state setting to add.</param>
		/// <returns>The identity guid of the newly added point.</returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, UserViewStateSetting setting)
		{
			if (security == null)
			{
			   throw new ArgumentNullException("security");
			}

			if (setting == null)
			{
			   throw new ArgumentNullException("setting");
			}

			var consolidatedDA = new ConsolidatedDAClass();

			using (var cmd = new SqlCommand())
			{
			   setting.SetCreationStamp(security);
			   setting.UserViewStateSettingGuid = Guid.NewGuid();
			   setting.GetInsertSQL(cmd);
			   consolidatedDA.ExecuteQuery(security, cmd);
			}

				
			return setting.IdentityGuid;
		}

		/// <summary>
		/// Purges the specified security.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="settingGuid">The user view state setting unique identifier.</param>
		/// <exception cref="System.ArgumentNullException">security</exception>
		/// <exception cref="System.Exception">Point not found.</exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid settingGuid)
		{
			if (security == null)
			{
			   throw new ArgumentNullException("security");
			}

			// TODO: Check security rights

			var setting = this.Get(security, settingGuid);
			if (setting.IdentityGuid == Guid.Empty)
			{
			   throw new Exception("User View State Setting not found.");
			}
				
			// Delete user view state setting
			var consolidatedDA = new ConsolidatedDAClass();
			using (var cmd = new SqlCommand())
			{
			   UserViewStateSetting.GetPurgeSQL(cmd,settingGuid);
			   consolidatedDA.ExecuteQuery(security, cmd);
			}
		}


	/// <summary>
	/// Purges for given site.
	/// </summary>
	/// <param name="security">The security.</param>
	/// <param name="siteGuid">The site unique identifier.</param>
	/// <exception cref="System.ArgumentNullException">security</exception>
	/// <exception cref="System.Exception">Point not found.</exception>
	[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
	public void PurgeBySite(SecurityClass security, Guid siteGuid)
	{
		if (security == null)
		{
			throw new ArgumentNullException("security");
		}

		// TODO: Check security rights

		// Delete user view state setting
		var consolidatedDA = new ConsolidatedDAClass();
		using (var cmd = new SqlCommand())
		{
			UserViewStateSetting.GetPurgeBySiteSQL(cmd, siteGuid);
			consolidatedDA.ExecuteQuery(security, cmd);
		}
	}


	/// <summary>
	/// Purges for given user.
	/// </summary>
	/// <param name="security">The security.</param>
	/// <param name="userGuid">The user unique identifier.</param>
	/// <exception cref="System.ArgumentNullException">security</exception>
	/// <exception cref="System.Exception">Point not found.</exception>
	[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
	public void PurgeByUser(SecurityClass security, Guid userGuid)
	{
		if (security == null)
		{
			throw new ArgumentNullException("security");
		}

		// TODO: Check security rights

		// Delete user view state setting
		var consolidatedDA = new ConsolidatedDAClass();
		using (var cmd = new SqlCommand())
		{
			UserViewStateSetting.GetPurgeByUserSQL(cmd, userGuid);
			consolidatedDA.ExecuteQuery(security, cmd);
		}
	}


	/// <summary>
	/// Gets the specified security.
	/// </summary>
	/// <param name="security">The security.</param>
	/// <param name="settingGuid">The user view state setting unique identifier.</param>
	/// <returns></returns>
	public UserViewStateSetting Get(SecurityClass security, Guid settingGuid)
	{
			var consolidatedDA = new ConsolidatedDAClass();
			DataSet set;

			using (var cmd = new SqlCommand())
			{
			   UserViewStateSetting.GetSQL(cmd, settingGuid);
			   set = consolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];
			if (table.Rows.Count == 0)
			{
			   return null;
			}

			var setting = new UserViewStateSetting();
			setting.AutoLoad(table.Rows[0]);
				
			return setting;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, UserViewStateSetting setting)
		{
			if (security == null)
			{
			   throw new ArgumentNullException("security");
			}

			if (setting == null)
			{
			   throw new ArgumentNullException("setting");
			}

			var consolidatedDA = new ConsolidatedDAClass();

			using (var cmd = new SqlCommand())
			{
			   setting.SetModifyStamp(security);
			   setting.GetUpdateSQL(cmd);
			   consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		private static UserViewStateSettingCollection PopulateCollection(SecurityClass security, SqlCommand cmd)
		{
			var consolidatedDA = new ConsolidatedDAClass();
			var set = consolidatedDA.GetDataSet(cmd, security);
			var collection = new UserViewStateSettingCollection();

			var table = set.Tables[0];

			foreach (DataRow row in table.Rows)
			{
			   var setting = new UserViewStateSetting();
			   setting.AutoLoad(row);
			   collection.Add(setting);

			}
			return collection;
		}
		
		/// <summary>
		/// Enumerates points for the specified site.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="siteGuid">The site unique identifier.</param>
		/// <returns>
		/// A collection of Points.
		/// </returns>
		public UserViewStateSettingCollection EnumerateBySite(SecurityClass security, Guid siteGuid)
		{
			if (security == null)
			{
			   throw new ArgumentNullException("security");
			}
				
			using (var cmd = new SqlCommand())
			{
			   UserViewStateSetting.EnumerateBySiteSQL(cmd, siteGuid);
			   return PopulateCollection(security, cmd);
			}
		}


		/// <summary>
		/// Enumerates points for the specified site.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="userGuid">The user unique identifier.</param>
		/// <returns>
		/// A collection of Points.
		/// </returns>
		public UserViewStateSettingCollection EnumerateByUser(SecurityClass security, Guid userGuid)
		{
			if (security == null)
			{
			   throw new ArgumentNullException("security");
			}

			using (var cmd = new SqlCommand())
			{
			   UserViewStateSetting.EnumerateByUserSQL(cmd, userGuid);
			   return PopulateCollection(security, cmd);
			}
		}


		/// <summary>
		/// Enumerates points for the specified site.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="siteGuid">The site unique identifier.</param>
		/// <param name="userGuid">The user unique identifier.</param>
		/// <returns>
		/// A collection of Points.
		/// </returns>
		public UserViewStateSettingCollection EnumerateBySiteAndUser(SecurityClass security, Guid siteGuid, Guid userGuid)
		{
			if (security == null)
			{
			   throw new ArgumentNullException("security");
			}

			using (var cmd = new SqlCommand())
			{
			   UserViewStateSetting.EnumerateBySiteAndUserSQL(cmd, siteGuid, userGuid);
			   return PopulateCollection(security, cmd);
			}  
		}


		/// <summary>
		/// Enumerates points for the specified site.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="siteGuid">The site unique identifier.</param>
		/// <param name="userGuid">The user unique identifier.</param>
		/// <param name="viewID">The view ID</param>
		/// <returns>
		/// A collection of Points.
		/// </returns>
		public UserViewStateSettingCollection EnumerateBySiteUserClientIpAddressWindowNameAndViewID(SecurityClass security, Guid siteGuid, Guid userGuid, string windowName, string viewID)
		{
			if (security == null)
			{
			   throw new ArgumentNullException("security");
			}

			using (var cmd = new SqlCommand())
			{
			   UserViewStateSetting.EnumerateBySiteAndUserAndWindowNameAndViewIDSQL(cmd, siteGuid, userGuid, security.ClientIpAddress, windowName, viewID);
			   return PopulateCollection(security, cmd);
			}
		}

		


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

		   if (Object is SiteClass)
		   {
			   var site = Object as SiteClass;
			   this.PurgeBySite(security, site.IdentityGuid);
		   }

		   else if (Object is UserClass)
		   {
			   var user = Object as UserClass;
			   this.PurgeByUser(security, user.IdentityGuid);
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
 
 
 