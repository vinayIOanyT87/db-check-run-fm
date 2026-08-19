// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Sites.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	Sites service class implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessServices.ServiceClasses
{
   using System;
   using System.Collections.Generic;
   using System.Data;
   using System.Data.SqlClient;
   using System.Globalization;
   using System.Security;
   using System.ServiceModel;

   using FMBusinessObjects.BusinessInterfaces;
   using FMBusinessObjects.Constants;
   using FMBusinessObjects.DataObjects;
   using FMBusinessObjects.Exceptions;
   using FMBusinessObjects.ServiceRequests;
   using FMBusinessObjects.UtilityObjects;

	using FMCore;

	using DataAccessLayer;

   using InternalClasses;

   using IsolationLevel = System.Transactions.IsolationLevel;
    using FMBusinessObjects.ChannelFactories;
   using crypto;
   using System.Runtime.CompilerServices;

   /// <summary>
   /// The sites service class.
   /// </summary>
   [SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = IsolationLevel.ReadCommitted)]
	public class SitesClass : FMServiceBase, ISites, IDependency
	{
		#region Constants

		/// <summary>
		/// Trigger error message number
		/// </summary>
		private const string DbTriggerError001 = "DB_TRIGGER_ERROR_Sites_001";

		/// <summary>
		/// Trigger error message text.
		/// </summary>
		private const string DbTriggerErrorMsg001 = "Cannot set Enforce Single Owner due to multiple owner roles.";

		/// <summary>
		/// The default report directory
		/// </summary>
		private const string DefaultReportDirectory = "/Standard Reports";

		#endregion

		#region Static Fields

		#endregion

		#region Fields

		/// <summary>
		/// Database access object
		/// </summary>
		private readonly ConsolidatedDAClass consolidatedDa = new ConsolidatedDAClass();

      #endregion

      #region Public Methods and Operators

      /// <summary>
      /// Adds the specified site.
      /// </summary>
      /// <param name="security">The security.</param>
      /// <param name="site">The site.</param>
      /// <param name="userID">The user ID.</param>
      /// <param name="password">The password.</param>
      /// <returns>
      /// The identity GUID of the newly created site.
      /// </returns>
      [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, SiteClass site, string userID, string password)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (site == null)
			{
				throw new ArgumentNullException(nameof(site));
			}

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			this.Validate(security, site, null);

			if (this.GetIdentityGuid(security, site.ID) != Guid.Empty)
			{
				throw new Exception("Site Exists");
			}

			if (!string.IsNullOrEmpty(site.Note?.Note))
			{
				var notes = new Notes();
				site.NoteGuid = notes.Add(security, site.Note);
			}

			site.CreatedDate = DateTimeOffset.Now;
			site.CreatedBy = security.UserID;
			site.UpdatedDate = site.CreatedDate;
			site.UpdatedBy = security.UserID;
			site.Deleted = false;
			site.SiteGuid = Guid.NewGuid();
			site.IdentityGuid = site.SiteGuid;

			try
			{
				using (var cmd = new SqlCommand())
				{
					site.InsertSQL(cmd);
					this.consolidatedDa.ExecuteQuery(security, cmd);
				}

				SiteCache.AddSite(site);
			}
			catch (SqlException except)
			{
				if (except.Message.IndexOf(DbTriggerError001, StringComparison.Ordinal) > -1)
				{
					throw new Exception(DbTriggerErrorMsg001);
				}
			}

			security.SiteGuid = site.SiteGuid;

			var schedules = new SchedulesClass();
			schedules.ModifyCollection(security, site.SiteGuid, site.OperatingScheduleCollection, null);
			schedules.ModifyCollection(security, site.SiteGuid, site.HolidayScheduleCollection, null);

			var processVariables = new ProcessVariablesClass();
			processVariables.ModifyCollection(security, site.SiteGuid, site.ProcessVariableCollection, null);

			var applicationStrings = new ApplicationStringsClass();
			applicationStrings.ModifyCollection(security, site.SiteGuid, STRING_TYPE.SITE_CERTIFICATE, site.SiteCertificateCollection, null);

			var siteToSiteMaps = new SiteToSiteMapsClass();
			var siteMap = new SiteToSiteMapClass();
			if (site.SiteGroup)
			{
				foreach (SiteToSiteMapClass siteToSiteMap in site.SiteToSiteMapCollection)
				{
					siteToSiteMap.ParentSiteGuid = site.IdentityGuid;
					siteToSiteMap.ParentSiteID = site.ID;
					siteToSiteMaps.Add(security, siteToSiteMap);
				}
			}

         // Every Site is mapped to itself
         siteMap.ChildSiteGuid = site.SiteGuid;
			siteMap.ParentSiteGuid = site.SiteGuid;
			siteToSiteMaps.Add(security, siteMap);

         if (string.IsNullOrEmpty(userID) == false)
			{
				var administratorsGroup = new GroupClass { ID = "Local Adminstrators", Description = "Local System Administrators" };

				// Add Rights to Groups
				var rights = new RightsClass();
				administratorsGroup.RightCollection = rights.Enumerate(security);

				// delete the View Operate Only right form the list
				administratorsGroup.RightCollection.Remove(RIGHT.VIEW_OPERATE_ONLY);

				// Add Default User
				var user = new UserClass();
				var users = new UsersClass();
				user.ID = userID;
				user.Password = password;
				user.IdentityGuid = users.Add(security, user);

				// Assign to Administrator Group
				var userGroupMap = new UserGroupMapClass
				{
					UserGuid = user.IdentityGuid,
					GroupGuid = administratorsGroup.IdentityGuid,
					SiteGuid = site.IdentityGuid
				};

				administratorsGroup.UserGroupMapCollection.Add(userGroupMap);

				// Add the default company assignment of "{All}"
				var companyMapCollection = new CompanyMapCollectionClass();

				var companyMap = CompanyMapClass.CreateCompanyMap(COMPANY_MAP_TYPE.USER_GROUP_COMPANY_MAP);
				companyMap.AssignedGuid = Guid.Empty;

				companyMapCollection.Add(companyMap);

				administratorsGroup.CompanyMapCollection = companyMapCollection;

				// Add the groups to the system
				var groups = new GroupsClass();
				groups.Add(security, administratorsGroup);
			}

			// Create default Accounting configuration
			GeneralConfigDO generalConfigDO = new GeneralConfigDO
			{
				SiteGuid = site.IdentityGuid
			};
			GeneralConfigSR generalConfigSR = new GeneralConfigSR
			{
				Security = security,
				SiteGuid = site.IdentityGuid,
				GeneralConfigurationDO = generalConfigDO,
				Request = GeneralConfigSR.GeneralConfigurationRequests.SAVE_CONFIGURATION
			};
			GeneralConfigProcessorClass generalConfigProcessor = new GeneralConfigProcessorClass();
			generalConfigProcessor.Save(generalConfigSR);

         if (site.SiteGroup == false 
				&& site.CloseoutTime != null 
				&& security.HasRight(RIGHT.MODIFY_SITE_CLOSEOUT_TIME))
         {

            //Write 00:00:00 as old closeout time to closeout history table
            SiteCloseoutTimeClass closeoutTime = new SiteCloseoutTimeClass();

            closeoutTime.SiteGuid = site.SiteGuid;
            closeoutTime.ExpirationDate = DateTimeOffset.Now;
            closeoutTime.PointsChanged = false;
            SiteCloseoutTimes closeout = new SiteCloseoutTimes();
            closeout.SetCloseoutTime(security, closeoutTime);
            site.CloseoutTime = closeoutTime.CloseoutTime;

         }

         return site.SiteGuid;
		}

		/// <summary>
		/// Checks the current password.
		/// </summary>
		/// <param name="user">The user.</param>
		/// <param name="passwordText">The password text.</param>
		/// <returns>True if the password matches the user password.</returns>
		public bool CheckCurrentPassword(UserClass user, string passwordText)
		{
			// Change to support WI 6214 - we are now keeping the plaintext of passwords
			// in memory
			return (user.Password == passwordText) && (user.Password.Length > 0) ;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid CreateDefaultSingleSite(SecurityClass security, SiteClass site)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			using (var cmd = new SqlCommand())
			{
				site.CreateDefaultSingleSiteSQL(cmd);
				this.consolidatedDa.ExecuteQuery(security, cmd);
			}

			site = this.GetByID(security, site.ID);

			security.SiteGuid = site.SiteGuid;

			return site.SiteGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid CreateDefaultSingleSiteByLoginID(SecurityClass security, SiteClass site, string databaseLogOnId)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			if (!DBAccess.IsValidServiceLogin(databaseLogOnId))
			{
				throw new ArgumentException("This function may only be executed by a service login.", nameof(databaseLogOnId));
			}

			using (var cmd = new SqlCommand())
			{
				site.CreateDefaultSingleSiteSQL(cmd);
				this.consolidatedDa.ExecuteQuery(security, cmd, databaseLogOnId);
			}

			site = this.GetByID(security, site.ID);

			security.SiteGuid = site.SiteGuid;

			return site.SiteGuid;
		}

		/// <summary>
		/// Enumerates the sites in the system and returns them as a collection.
		/// </summary>
		/// <param name="security">A valid FuelsManager SecurityClass object.</param>
		/// <returns>
		/// SiteCollectionClass containing a list of SiteClass objects
		/// </returns>
		public SiteCollectionClass Enumerate(SecurityClass security)
		{
			this.CheckSecurity(security);

			var site = new SiteClass();
			using (var cmd = new SqlCommand())
			{
				site.EnumerateSQL(cmd);
				DataSet set = this.consolidatedDa.GetDataSet(cmd, security);
				var siteCollection = new SiteCollectionClass();

				DataTable table = set.Tables[0];
				while (table.Rows.Count != 0)
				{
					site = new SiteClass();
					site.Load(set);
					siteCollection.Add(site);
					table.Rows.RemoveAt(0);
				}

				return siteCollection;
			}
		}

		public byte[] GetRowVersion(SecurityClass security, Guid siteGuid)
		{
			using (var cmd = new SqlCommand())
			{
				SiteClass.GetRowVersionSQL(cmd, siteGuid);
				DataSet set = this.consolidatedDa.GetDataSet(cmd, security);
				if (set == null)
				{
					throw new ApplicationException("Null Set");
				}
				DataTable table = set.Tables[0];
				if (table.Rows.Count == 0)
				{
					return null;
				}
				DataRow row = table.Rows[0];
				var siteRowVer = DataObject.getValue<byte[]>(row["_Rowversion"], null);
				return siteRowVer;
			}
		}


		public SiteCollectionClass EnumerateByCandidateChildrenSites(SecurityClass security, Guid siteGuid)
		{
			this.CheckSecurity(security);

			var site = new SiteClass { SiteGuid = siteGuid };

			using (var cmd = new SqlCommand())
			{
				site.EnumerateByCandidateChildrenSitesSQL(cmd);
				DataSet set = this.consolidatedDa.GetDataSet(cmd, security);
				var siteCollection = new SiteCollectionClass();

				DataTable table = set.Tables[0];
				while (table.Rows.Count != 0)
				{
					site = new SiteClass();
					site.Load(set);

					// Every Site is mapped to itself and should be skipped here
					if (siteGuid != site.SiteGuid)
					{
						siteCollection.Add(site);
					}

					table.Rows.RemoveAt(0);
				}

				return siteCollection;
			}
		}

		/// <summary>
		/// This method enumerates the parent sites of the child site with the specified
		///	SiteGuid.  The collection includes the child site as well.
		/// </summary>
		/// <param name="security">
		/// </param>
		/// <param name="siteGuid">
		/// </param>
		/// <returns>
		/// The <see cref="SiteCollectionClass"/>.
		/// </returns>
		public SiteCollectionClass EnumerateByChildSite(SecurityClass security, Guid siteGuid)
		{
			this.CheckSecurity(security);

			var site = new SiteClass { SiteGuid = siteGuid };
			using (var cmd = new SqlCommand())
			{
				site.EnumerateByChildSiteSQL(cmd);
				DataSet set = this.consolidatedDa.GetDataSet(cmd, security);
				var siteCollection = new SiteCollectionClass();

				DataTable table = set.Tables[0];
				while (table.Rows.Count != 0)
				{
					site = new SiteClass();
					site.Load(set);

					// Every Site is mapped to itself and should be skipped here
					if (siteGuid != site.SiteGuid)
					{
						siteCollection.Add(site);
					}

					table.Rows.RemoveAt(0);
				}

				return siteCollection;
			}
		}

		/// <summary>
		/// Enumerates the by child site for user.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="siteGuid">
		/// The site GUID.
		/// </param>
		/// <returns>
		/// The <see cref="SiteCollectionClass"/>.
		/// </returns>
		public SiteCollectionClass EnumerateByChildSiteForUser(SecurityClass security, Guid siteGuid)
		{
			this.CheckSecurity(security);

			var site = new SiteClass { SiteGuid = siteGuid };

			using (var cmd = new SqlCommand())
			{
				site.EnumerateByChildSiteForUserSQL(cmd, security.UserGuid);
				DataSet set = this.consolidatedDa.GetDataSet(cmd, security);
				var siteCollection = new SiteCollectionClass();

				DataTable table = set.Tables[0];
				while (table.Rows.Count != 0)
				{
					site = new SiteClass();
					site.Load(set);

					// Every Site is mapped to itself and should be skipped here
					if (siteGuid != site.SiteGuid)
					{
						siteCollection.Add(site);
					}

					table.Rows.RemoveAt(0);
				}

				return siteCollection;
			}
		}

		public SiteSelectList EnumerateForSiteSelect(SecurityClass security, Guid parentSiteGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			DataSet set = null;
			var siteSelectList = new SiteSelectList();
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "dbo.usp_GetUserSiteHierarchy";
				cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@UserGuid"].Value = security.UserGuid == Guid.Empty ? DBNull.Value : (object)security.UserGuid;
				cmd.Parameters.Add("@StartSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@StartSiteGuid"].Value = parentSiteGuid == Guid.Empty ? DBNull.Value : (object)parentSiteGuid;
				set = this.consolidatedDa.GetDataSet(cmd, security);
			}


			if (set != null && set.Tables.Count > 0)
			{
				DataTable table = set.Tables[0];
				for (var index = 0; index < table.Rows.Count; ++index)
				{
					DataRow siteRow = table.Rows[index];

					var siteSelectInfo = new SiteSelectInfo()
					{
						ID = (string)siteRow["ID"],
						IsSiteGroup = (bool)siteRow["SiteGroupFlag"],
						Number = DataObject.getValue<string>(siteRow["Number"], ""),
						SiteGuid = (Guid)siteRow["SiteGuid"]
					};

					siteSelectList.Add(siteSelectInfo);
				}
			}

			return siteSelectList;

		}

		public SiteCollectionClass EnumerateByParentSite(SecurityClass security, Guid siteGuid)
		{
			var site = new SiteClass { SiteGuid = siteGuid };

			using (var cmd = new SqlCommand())
			{
				site.EnumerateByParentSiteSQL(cmd);
				DataSet set = this.consolidatedDa.GetDataSet(cmd, security);
				var siteCollection = new SiteCollectionClass();

				DataTable table = set.Tables[0];
				while (table.Rows.Count != 0)
				{
					site = new SiteClass();
					site.Load(set);

					// Every Site is mapped to itself and should be skipped here
					if (siteGuid != site.SiteGuid)
					{
						siteCollection.Add(site);
					}

					table.Rows.RemoveAt(0);
				}

				return siteCollection;
			}
		}

		/// <summary>
		/// Enumerates the by parent site current user assigned.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="siteGuid">
		/// The site GUID.
		/// </param>
		/// <returns>
		/// The <see cref="SiteCollectionClass"/>.
		/// </returns>
		public SiteCollectionClass EnumerateByParentSiteCurrentUserAssigned(SecurityClass security, Guid siteGuid)
		{
			return this.EnumerateSitesByAssignedUser(security, siteGuid, security.UserGuid);
		}

		public SiteCollectionClass EnumerateByServiceLogin(SecurityClass security, string serviceLogin)
		{
			this.CheckSecurity(security);

			if (!DBAccess.IsValidServiceLogin(serviceLogin))
			{
				throw new ArgumentException("The parameter 'serviceLogin' must be a valid service login", nameof(serviceLogin));
			}

			var site = new SiteClass(true);
			using (var cmd = new SqlCommand())
			{
				site.EnumerateSQL(cmd);
				DataSet set = this.consolidatedDa.GetDataSet(cmd, serviceLogin, string.Empty);
				var siteCollection = new SiteCollectionClass();

				DataTable table = set.Tables[0];
				while (table.Rows.Count != 0)
				{
					site = new SiteClass();
					site.Load(set);
					siteCollection.Add(site);
					table.Rows.RemoveAt(0);
				}

				return siteCollection;
			}
		}

		public SiteCollectionClass EnumerateBySiteGroup(SecurityClass security, bool siteGroup)
		{
			this.CheckSecurity(security);

			var site = new SiteClass { SiteGroup = siteGroup };
			using (var cmd = new SqlCommand())
			{
				site.EnumerateBySiteGroupSQL(cmd);
				DataSet set = this.consolidatedDa.GetDataSet(cmd, security);
				var siteCollection = new SiteCollectionClass();

				DataTable table = set.Tables[0];
				while (table.Rows.Count != 0)
				{
					site = new SiteClass();
					site.Load(set);
					siteCollection.Add(site);
					table.Rows.RemoveAt(0);
				}

				return siteCollection;
			}
		}

		/// <summary>
		/// The enumerate all site GUIDs, IDs, and site group flags.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <returns>
		/// The <see cref="DataSet"/>.
		/// </returns>
		public List<SiteClass> EnumerateIndexIdGroupFlag(SecurityClass security)
		{
			this.CheckSecurity(security);
			var siteIndexIdFlag = new List<SiteClass>();

			using (var sqlCommand = new SqlCommand())
			{
				var site = new SiteClass();
				site.EnumerateAllIndexIdSiteGroupSql(sqlCommand);
				DataSet dataSet = this.consolidatedDa.GetDataSet(sqlCommand, security);

				if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
				{
					DataTable table = dataSet.Tables[0];
					foreach (DataRow row in table.Rows)
					{
						string siteId = row.IsNull("ID") ? string.Empty : (string)row["ID"];
						Guid? siteGuid = row.IsNull("SiteGuid") ? Guid.Empty : row["SiteGuid"] as Guid?;
						bool? siteGroupFlag = row.IsNull("SiteGroupFlag") ? false : row["SiteGroupFlag"] as bool?;

						if (string.IsNullOrEmpty(siteId) || siteGuid == null || siteGuid == Guid.Empty)
						{
							continue;
						}

						if (siteGroupFlag == null)
						{
							siteGroupFlag = false;
						}

						site = new SiteClass { ID = siteId, SiteGuid = siteGuid.Value, SiteGroup = siteGroupFlag.Value };
						siteIndexIdFlag.Add(site);
					}
				}
			}

			return siteIndexIdFlag;
		}

		/// <summary>
		/// The enumerate limit site member by parent site.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="siteGuid">
		/// The site GUID.
		/// </param>
		/// <returns>
		/// The <see cref="SiteCollectionClass"/>.
		/// </returns>
		public SiteCollectionClass EnumerateLimitSiteMemberByParentSite(SecurityClass security, Guid siteGuid)
		{
			var siteCollection = new SiteCollectionClass();

			using (var sqlCommand = new SqlCommand())
			{
				var site = new SiteClass { SiteGuid = siteGuid };
				site.EnumerateLimitSiteMemberByParentSiteSql(sqlCommand);

				DataSet set = this.consolidatedDa.GetDataSet(sqlCommand, security);

				DataTable table = set.Tables[0];

				foreach (DataRow row in table.Rows)
				{
					site = new SiteClass(true);
					site.LoadLimitSiteMemberByRow(row);

					// Every Site is mapped to itself and should be skipped here
					if (siteGuid != site.SiteGuid)
					{
						siteCollection.Add(site);
					}
				}
			}

			return siteCollection;
		}

		/// <summary>
		/// The enumerate report directories.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <returns>
		/// A collection of site objects.
		/// </returns>
		public List<SiteClass> EnumerateReportDirectories(SecurityClass security)
		{
			this.CheckSecurity(security);
			var reportDirectoryList = new List<SiteClass>();

			using (var sqlCommand = new SqlCommand())
			{
				var site = new SiteClass();
				site.EnumerateReportDirectorySql(sqlCommand);
				DataSet dataSet = this.consolidatedDa.GetDataSet(sqlCommand, security);

				if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
				{
					DataTable table = dataSet.Tables[0];
					foreach (DataRow row in table.Rows)
					{
						string reportDirectory = row.IsNull("ManagedReportDirectory")
															? string.Empty
															: (string)row["ManagedReportDirectory"];

						bool manageReportFlag = !row.IsNull("ManageReports") && (bool)row["ManageReports"];

						if (string.IsNullOrEmpty(reportDirectory))
						{
							continue;
						}

						site = new SiteClass { ManageReports = manageReportFlag, ManagedReportDirectory = reportDirectory };
						reportDirectoryList.Add(site);
					}
				}
			}

			return reportDirectoryList;
		}

		/// <summary>
		/// This method will retrieve all sites with mail information based on the mail server field 
		/// being not null.
		/// </summary>
		/// <param name="security"></param>
		/// <returns>Returns a collection of sites with mail information.</returns>
		public List<SiteClass> EnumerateMailInfo(SecurityClass security)
		{
			this.CheckSecurity(security);
			var siteList = new List<SiteClass>();

			using (var sqlCommand = new SqlCommand())
			{
				var site = new SiteClass();
				site.EnumerateMailInfoSql(sqlCommand);
				DataSet dataSet = this.consolidatedDa.GetDataSet(sqlCommand, security);

				if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
				{
					DataTable table = dataSet.Tables[0];
					foreach (DataRow row in table.Rows)
					{
						Guid siteGuid = row.IsNull("SiteGuid") ? Guid.Empty : (Guid)row["SiteGuid"];

						if (siteGuid == Guid.Empty)
						{
							continue;
						}

						string siteId = row.IsNull("ID") ? string.Empty : (string)row["ID"];
						string mailServer = row.IsNull("MailServer") ? string.Empty : (string)row["MailServer"];
						string emailAddress = row.IsNull("EmailAddress") ? string.Empty : (string)row["EmailAddress"];
						string dialupName = row.IsNull("DialupName") ? string.Empty : (string)row["DialupName"];
						string mailFrom = row.IsNull("MailFrom") ? string.Empty : (string)row["MailFrom"];
						string mailUserName = row.IsNull("MailUserName") ? string.Empty : (string)row["MailUserName"];
						string mailpassword = row.IsNull("MailPassword") ? string.Empty : (string)row["MailPassword"];
						byte mailConnectMode = row.IsNull("LookupMailConnectModeIndex") ? (byte)0 : (byte)row["LookupMailConnectModeIndex"];

						site = new SiteClass
						{
							SiteGuid = siteGuid,
							SiteID = siteId,
							MailServer = mailServer,
							EmailAddress = emailAddress,
							DialupName = dialupName,
							MailFrom = mailFrom,
							MailUserName = mailUserName,
							MailPassword = mailpassword,
							MailConnectMode = (MAIL_SERVER_CONNECT_MODE)mailConnectMode
						};

						siteList.Add(site);
					}
				}
			}

			return siteList;
		}

		/// <summary>
		/// This method returns a dependency list of Sites (ancestors and descendants) for the passed in SiteGuid.  For Synchronization,
		///	all ancestors (parents) up to SiteAdmin must be synchronized first.  If the passed in SiteGuid is a SiteGroup, then in addition
		///	to the ancestors, all descendants (children) must be synchronized as well.
		/// </summary>
		/// <remarks>
		/// The returned collection will include the SiteClass for the passed in SiteGuid.  The returned collection will be in the order of their dependency.
		/// </remarks>
		/// <param name="security">
		/// </param>
		/// <param name="siteGuid">
		/// </param>
		/// <returns>
		/// Collection of SiteClass objects in the order of their dependency starting with SiteAdmin.
		/// </returns>
		public SiteSyncList EnumerateSiteSynchronizationListBySiteSQL(SecurityClass security, Guid siteGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			SiteClass site = this.GetUsingGuid(security, siteGuid);

			using (var cmd = new SqlCommand())
			{
				var orderedSiteCollection = new SiteSyncList();

				if (null != site)
				{
					site.EnumerateSiteSynchronizationListBySiteSQL(cmd);

					DataSet set = this.consolidatedDa.GetDataSet(cmd, security);

					DataTable table = set.Tables[0];
					while (table.Rows.Count != 0)
					{
						int syncLevel = DataObject.getValue(table.Rows[0]["Level"], 0);
						SiteClass siteToAdd = new SiteClass { SiteGuid = DataObject.getValue(table.Rows[0]["SiteGuid"], Guid.Empty) };

						using (var liteCmd = new SqlCommand())
						{
							siteToAdd.SelectPartialSQL(liteCmd, ContextUtil.IsInTransaction, false);
							siteToAdd.LoadPartial(this.consolidatedDa.GetDataSet(liteCmd, security));
						}

						orderedSiteCollection.Add(syncLevel, siteToAdd);

						table.Rows.RemoveAt(0);
					}
				}

				return orderedSiteCollection;
			}
		}

		/// <summary>
		/// Enumerates the sites by assigned user.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="siteGuid">
		/// The site GUID.
		/// </param>
		/// <param name="userGuid">
		/// The user GUID.
		/// </param>
		/// <returns>
		/// The <see cref="SiteCollectionClass"/>.
		/// </returns>
		public SiteCollectionClass EnumerateSitesByAssignedUser(SecurityClass security, Guid siteGuid, Guid userGuid)
		{
			var site = new SiteClass { SiteGuid = siteGuid };

			using (var cmd = new SqlCommand())
			{
				site.EnumerateByParentSiteAndAssignedUserSQL(cmd, userGuid);
				DataSet set = this.consolidatedDa.GetDataSet(cmd, security);
				var siteCollection = new SiteCollectionClass();

				DataTable table = set.Tables[0];
				while (table.Rows.Count != 0)
				{
					site = new SiteClass();
					site.Load(set);

					// Every Site is mapped to itself and should be skipped here
					if (siteGuid != site.SiteGuid)
					{
						siteCollection.Add(site);
					}

					table.Rows.RemoveAt(0);
				}

				return siteCollection;
			}
		}

		public SiteCollectionClass EnumerateSitesInfo(SecurityClass security)
		{
			var site = new SiteClass();
			using (var cmd = new SqlCommand())
			{
				site.EnumerateSiteInfoSQL(cmd);
				DataSet set = this.consolidatedDa.GetDataSet(cmd, security);
				var siteCollection = new SiteCollectionClass();

				foreach (DataRow row in set.Tables[0].Rows)
				{
					site = new SiteClass(true)
					{
						SiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty),
						ID = DataObject.getValue(row["ID"], string.Empty),
						_AdministrativeLockDate =
										{
											Value = DataObject.getValue(row["AdministrativeLockDate"], DateTimeOffset.MaxValue)
										},
						_OperationalLockDate =
										{
											Value = DataObject.getValue(row["OperationalLockDate"], DateTimeOffset.MaxValue)
										},
						SiteGroup = DataObject.getValue(row["SiteGroupFlag"], false),
						Number = DataObject.getValue(row["Number"], string.Empty)
					};

					siteCollection.Add(site);
				}

				return siteCollection;
			}
		}

		//Enumberate all the sites a user related to. Designed for reset password
		public SiteCollectionClass EnumerateByUser(SecurityClass security, Guid userGuid)
		{
			var site = new SiteClass();

			using (var cmd = new SqlCommand())
			{
				site.EnumerateByUser(cmd, userGuid);
				DataSet set = this.consolidatedDa.GetDataSet(cmd, security);
				var siteCollection = new SiteCollectionClass();

				foreach (DataRow row in set.Tables[0].Rows)
				{
					site = new SiteClass()
					{
						SiteGuid	= DataObject.getValue(row["SiteGuid"], Guid.Empty),
						ID			= DataObject.getValue(row["ID"], string.Empty),
						MailServer	= DataObject.getValue(row["MailServer"], string.Empty),
						MailFrom	= DataObject.getValue(row["MailFrom"], string.Empty),
						SiteGroup	= DataObject.getValue(row["SiteGroupFlag"], false)
					};

					siteCollection.Add(site);
				}

				return siteCollection;
			}
		}

		/// <summary>
		/// This method will get the site class without member sites,
		/// schedules, process variables, and associated aliases.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="identityGuid">The site's identity GUID.</param>
		/// <returns>Returns the site class object.</returns>
		public SiteClass GetBasic(SecurityClass security, Guid identityGuid)
		{
			const bool GetMemberSites = false;
			const bool GetSchedulesAndProcessVariables = false;
			const bool GetAssociatedAliases = false;

			return this.Get(security, identityGuid, GetMemberSites, GetSchedulesAndProcessVariables, GetAssociatedAliases);
		}

		/// <summary>
		/// This method is not exposed to WCF.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="identityGuid">The site's identity GUID.</param>
		/// <param name="getMemberSites">Flag to get the member sites too.</param>
		/// <returns>Returns the site class object.</returns>
		public SiteClass Get(SecurityClass security, Guid identityGuid, bool getMemberSites)
		{
			return this.Get(security, identityGuid, getMemberSites, getSchedulesAndProcessVariables: true, bGetAssociatedAliases: true);
		}

		/// <summary>
		/// This method will get the site data.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="identityGuid">The site's identity GUID.</param>
		/// <param name="bGetMemberSites">Flag to get the member sites too.</param>
		/// <param name="getSchedulesAndProcessVariables">Flag to get the schedules and process variables.</param>
		/// <param name="bGetAssociatedAliases">Flag to get the associated aliases.</param>
		/// <returns>Returns the site class object.</returns>
		public SiteClass Get(
			SecurityClass security,
			Guid identityGuid,
			bool bGetMemberSites,
			bool getSchedulesAndProcessVariables,
			bool bGetAssociatedAliases)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			// Check the site cache
			SiteClass site = SiteCache.GetSite(identityGuid);

			if (site == null || this.CachedSiteEntryExpired(security, site))
			{
				site = new SiteClass { SiteGuid = identityGuid };

				using (var cmd = new SqlCommand())
				{
					site.SelectSQL(cmd, ContextUtil.IsInTransaction, bGetAssociatedAliases);
					site.Load(this.consolidatedDa.GetDataSet(cmd, security));
				}

				SiteCache.AddSite(site);
			}

			SiteClass loginSite;

			if (identityGuid == security.LoginSiteGuid)
			{
				loginSite = site;
			}
			else
			{
				// Check the cache
				loginSite = SiteCache.GetSite(security.LoginSiteGuid);

				if (loginSite == null || this.CachedSiteEntryExpired(security, loginSite))
				{
					loginSite = new SiteClass { SiteGuid = security.LoginSiteGuid };

					using (var cmd = new SqlCommand())
					{
						loginSite.SelectSQL(cmd, ContextUtil.IsInTransaction, bGetAssociatedAliases);
						loginSite.Load(this.consolidatedDa.GetDataSet(cmd, security));
					}

					SiteCache.AddSite(loginSite);
				}
			}

			// Set Format for SIDoubles
			site._MaximumLoadAmount.Format = loginSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME);
			site._MaximumLoadAmount.Units = loginSite.VolumeUnits;
			site._MaximumFlushAmount.Format = loginSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME);
			site._MaximumFlushAmount.Units = loginSite.VolumeUnits;
			site._MaximumMeterProvingAmount.Format = loginSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME);
			site._MaximumMeterProvingAmount.Units = loginSite.VolumeUnits;
			site._MaximumReturnsAmount.Format = loginSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME);
			site._MaximumReturnsAmount.Units = loginSite.VolumeUnits;
			site._VRURateLimit.Format = loginSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME);
			site._VRURateLimit.Units = loginSite.VolumeUnits;
			site._VRUHourlyLimit.Format = loginSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME);
			site._VRUHourlyLimit.Units = loginSite.VolumeUnits;
			site._VRUDailyLimit.Format = loginSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME);
			site._VRUDailyLimit.Units = loginSite.VolumeUnits;
			site._VRUYearlyLimit.Format = loginSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME);
			site._VRUYearlyLimit.Units = loginSite.VolumeUnits;
			site._VRUCurrentYearLimit.Format = loginSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME);
			site._VRUCurrentYearLimit.Units = loginSite.VolumeUnits;
			site._VRURateActual.Format = loginSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME);
			site._VRURateActual.Units = loginSite.VolumeUnits;
			site._VRUHourlyActual.Format = loginSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME);
			site._VRUHourlyActual.Units = loginSite.VolumeUnits;
			site._VRUDailyActual.Format = loginSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME);
			site._VRUDailyActual.Units = loginSite.VolumeUnits;
			site._VRUYearlyActual.Format = loginSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME);
			site._VRUYearlyActual.Units = loginSite.VolumeUnits;
			site._VRUCurrentYearActual.Format = loginSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME);
			site._VRUCurrentYearActual.Units = loginSite.VolumeUnits;

			// Changed LoginSite to Site for Date and DateTime to fix Bug 189. A.Coker 12/05/08
			site._AdministrativeLockDate.Format = site.GetDateTimeFormatInfo();
			site._OperationalLockDate.Format = site.GetDateTimeFormatInfo();

			if (getSchedulesAndProcessVariables)
			{
				var schedules = new SchedulesClass();

				ScheduleCollectionClass operatingScheduleCollection
					= schedules.EnumerateByEntityGuidAndType(security, identityGuid, SCHEDULE_TYPE.TERMINAL_OPERATIONS_TYPE);

				if (operatingScheduleCollection.Count > 0)
				{
					site.OperatingScheduleCollection = operatingScheduleCollection;
				}

				// Set Format in Time members because Enumerate cannot
				foreach (ScheduleClass operatingSchedule in site.OperatingScheduleCollection)
				{
					operatingSchedule.OpeningTime.Format = site.GetDateTimeFormatInfo();
					operatingSchedule.ClosingTime.Format = site.GetDateTimeFormatInfo();
					operatingSchedule.EndOfDayTime.Format = site.GetDateTimeFormatInfo();
				}

				site.HolidayScheduleCollection = schedules.EnumerateByEntityGuidAndType(
					security, identityGuid, SCHEDULE_TYPE.HOLIDAY_TYPE);

				// Set Format in Time members because Enumerate cannot
				foreach (ScheduleClass holidaySchedule in site.HolidayScheduleCollection)
				{
					holidaySchedule.OpeningTime.Format = site.GetDateTimeFormatInfo();
					holidaySchedule.ClosingTime.Format = site.GetDateTimeFormatInfo();
					holidaySchedule.EndOfDayTime.Format = site.GetDateTimeFormatInfo();
				}

				var processVariables = new ProcessVariablesClass();
				site.ProcessVariableCollection = processVariables.EnumerateByUnit(security, identityGuid, UNIT_TYPE.SITE_UNIT);
			}

			// get notes
			if (site.NoteGuid != Guid.Empty)
			{
				var notes = new Notes();
				site.Note = notes.Get(security, site.NoteGuid);
			}

			if (site.SiteGroup && bGetMemberSites)
			{
				var siteToSiteMaps = new SiteToSiteMapsClass();
				site.SiteToSiteMapCollection = siteToSiteMaps.EnumerateByParentSite(security, identityGuid);
			}

			var applicationStrings = new ApplicationStringsClass();
			site.SiteCertificateCollection = applicationStrings.EnumerateByTypeAndSite(security, STRING_TYPE.SITE_CERTIFICATE, site.SiteGuid);

			return site;
		}

		public SiteClass GetByID(SecurityClass security, string siteID, bool skipReset = false)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			var site = new SiteClass(skipReset) { ID = siteID };

			using (var cmd = new SqlCommand())
			{
				site.SelectByIdsql(cmd, ContextUtil.IsInTransaction);
				site.Load(this.consolidatedDa.GetDataSet(cmd, security));
			}

			return site;
		}

		public SiteClass GetByMemberAndProcessVariables(
			SecurityClass security, Guid identityGuid, bool getMemberSites, bool getSchedulesAndProcessVariables)
		{
			return this.Get(security, identityGuid, getMemberSites, getSchedulesAndProcessVariables, bGetAssociatedAliases: true);
		}

		public string GetIDNoRefresh(SecurityClass security, Guid siteGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			// Check the site cache
			SiteClass site = SiteCache.GetSite(siteGuid);

			if (site == null || this.CachedSiteEntryExpired(security, site))
			{
				site = this.Get(security, siteGuid, false, false, false);
			}

			return site.ID;
		}

		/// <summary>
		/// Gets the identity GUID of the specified site.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="siteID">The site ID.</param>
		/// <returns>The identity guid of the specified site.</returns>
		public Guid GetIdentityGuid(SecurityClass security, string siteID)
		{
			SiteClass site = this.GetByID(security, siteID, true);
			return site.SiteGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public string GetNextDocumentNumber(SecurityClass security, DOCUMENT_TYPE type, Guid siteGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			SiteClass site = this.GetByMemberAndProcessVariables(security, siteGuid, false, false);

			string format;
			bool reset = false;

			SqlCommand numberQueryCmd;
			int nextNumber;
			switch (type)
			{
				case DOCUMENT_TYPE.AUTOMATIC_BOL:
					numberQueryCmd = SiteClass.FetchNextSequenceNumberSql(security, type, siteGuid);
					nextNumber = (int)this.consolidatedDa.ExecuteScalar(numberQueryCmd, security);
					format = "D" + site.AutomaticBOLEndNumber.Length.ToString(CultureInfo.InvariantCulture);
					if (nextNumber >= site._AutomaticBOLEndNumber)
					{
						reset = true;
					}

					break;

				case DOCUMENT_TYPE.MANUAL_BOL:
					if (!site.SeparateManualBOLNumbering)
					{
						numberQueryCmd = SiteClass.FetchNextSequenceNumberSql(security, DOCUMENT_TYPE.AUTOMATIC_BOL, siteGuid);
						nextNumber = (int)this.consolidatedDa.ExecuteScalar(numberQueryCmd, security);
						format = "D" + site.AutomaticBOLEndNumber.Length.ToString(CultureInfo.InvariantCulture);
						if (nextNumber >= site._AutomaticBOLEndNumber)
						{
							reset = true;
						}
					}
					else
					{
						numberQueryCmd = SiteClass.FetchNextSequenceNumberSql(security, type, siteGuid);
						nextNumber = (int)this.consolidatedDa.ExecuteScalar(numberQueryCmd, security);
						format = "D" + site.ManualBOLEndNumber.Length.ToString(CultureInfo.InvariantCulture);
						if (nextNumber >= site._ManualBOLEndNumber)
						{
							reset = true;
						}
					}

					break;

				case DOCUMENT_TYPE.TRANSACTION:
					numberQueryCmd = SiteClass.FetchNextSequenceNumberSql(security, type, siteGuid);
					nextNumber = (int)this.consolidatedDa.ExecuteScalar(numberQueryCmd, security);
					format = "D" + site.TransactionEndNumber.Length.ToString(CultureInfo.InvariantCulture);
					if (nextNumber >= site._TransactionEndNumber)
					{
						reset = true;
					}

					break;

				case DOCUMENT_TYPE.ORDER:
					numberQueryCmd = SiteClass.FetchNextSequenceNumberSql(security, type, siteGuid);
					nextNumber = (int)this.consolidatedDa.ExecuteScalar(numberQueryCmd, security);
					format = "D" + site.OrderEndNumber.Length.ToString(CultureInfo.InvariantCulture);
					if (nextNumber >= site._OrderEndNumber)
					{
						reset = true;
					}

					break;

				default:
					format = "D10";
					nextNumber = 0;
					break;
			}

			if (reset)
			{
				if (type == DOCUMENT_TYPE.MANUAL_BOL && !site.SeparateManualBOLNumbering)
				{
					this.consolidatedDa.ExecuteQuery(security, SiteClass.ResetSequenceNumberSql(security, DOCUMENT_TYPE.AUTOMATIC_BOL, siteGuid));
				}
				else
				{
					this.consolidatedDa.ExecuteQuery(security, SiteClass.ResetSequenceNumberSql(security, type, siteGuid));
				}
			}

			string prefix;
			if (site.NumberPrefix.Contains("%Date%"))
			{
				var siteTimeConverter = new SiteTimeConverter(site);
				string datePrefix = siteTimeConverter.ConvertToSiteTime(DateTime.UtcNow).ToString("yyyyMMdd");
				prefix = site.NumberPrefix.Replace("%Date%", datePrefix);
			}
			else
			{
				prefix = site.NumberPrefix;
			}

			return prefix + nextNumber.ToString(format);
		}

		/// <summary>
		/// Gets a specified number of next document numbers.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="documentTypes"></param>
		/// <param name="siteGuid">
		/// The site GUID.
		/// </param>
		/// <param name="numberDesired">
		/// The number desired.
		/// </param>
		/// <returns>
		/// A list of new doc numbers.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">
		/// security
		/// </exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public List<string> GetNextDocumentNumbers(
			SecurityClass security, List<DOCUMENT_TYPE> documentTypes, Guid siteGuid, int numberDesired)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			var site = new SiteClass { SiteGuid = siteGuid };

			using (var cmd = new SqlCommand())
			{
				site.SelectSQL(cmd, ContextUtil.IsInTransaction, false);
				site.Load(this.consolidatedDa.GetDataSet(cmd, security));
			}

			var documentNumbers = new List<string>();

			for (int index = 0; index < numberDesired; ++index)
			{
				documentNumbers.Add(this.GetNextDocumentNumber(security, documentTypes[index], siteGuid));
			}

			return documentNumbers;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public string GetNextInvoiceNumber(SecurityClass security)
		{
			// Make sure a valid security object is passed
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			Guid siteGuid = security.SiteGuid;

			// Get the site class
			SiteClass site = this.Get(security, siteGuid, false);

			int nextNumber = site._InvoiceNextNumber;
			site._InvoiceNextNumber++;
			string format = "D" + site.InvoiceEndNumber.Length;

			if (site._InvoiceNextNumber > site._InvoiceEndNumber)
			{
				site._InvoiceNextNumber = site._InvoiceStartNumber;
			}

			// Update the site
			this.Modify(security, DATA_TYPE.AUTOMIC, site);

			// Prepend a date stamp to the invoice number
			string prefix;

			if (site.NumberPrefix.Contains("%Date%"))
			{
				var siteTimeConverter = new SiteTimeConverter(site);
				string datePrefix = siteTimeConverter.ConvertToSiteTime(DateTime.UtcNow).ToString("yyyyMMdd");
				prefix = site.NumberPrefix.Replace("%Date%", datePrefix);
			}
			else
			{
				prefix = site.NumberPrefix;
			}

			return prefix + nextNumber.ToString(format);
		}

		public string GetReportDirectory(SecurityClass security, string reportPath)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			string retReportDirectory;
			if (!this.GetReportDirectoryHelper(security, reportPath, out retReportDirectory))
			{
				this.GetDefaultReportDirectoryHelper(security, out retReportDirectory);
			}

			return retReportDirectory;
		}

		public string GetReportDirectory(SecurityClass security, Guid reportGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			const string LocalDefaultReportDirectory = "/Standard Reports";
			string retReportDirectory = LocalDefaultReportDirectory;
			using (var cmd = new SqlCommand())
			{
				cmd.CommandText =
					"Exec map.usp_EntityReportConfigurationSettingsToSiteSelectReportDirectoryBySiteGuidReportGuid @CurrentSite, @ReportGuid";
				cmd.Parameters.AddWithValue("@CurrentSite", security.SiteGuid);
				cmd.Parameters.AddWithValue("@ReportGuid", reportGuid);
				DataSet set = this.consolidatedDa.GetDataSet(cmd, security);
				if (set == null)
				{
					throw new ArgumentException("Unable to find directory for specified report", nameof(reportGuid));
				}

				DataTable table = set.Tables[0];
				if (table.Rows.Count == 0)
				{
					return retReportDirectory;
				}

				DataRow row = table.Rows[0];
				retReportDirectory = DataObject.getValue(row[0], LocalDefaultReportDirectory);
			}

			return retReportDirectory;
		}

		public int GetSiteCount(SecurityClass security)
		{
			this.CheckSecurity(security);

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "SELECT COUNT(*) FROM tblSites";

				DataSet set = this.consolidatedDa.GetDataSet(cmd, security);

				return (int)set.Tables[0].Rows[0][0];
			}
		}

		public int GetSiteCountByServiceLogin(SecurityClass security, string serviceLogin)
		{
			this.CheckSecurity(security);

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "SELECT COUNT(*) FROM tblSites";

				DataSet set = this.consolidatedDa.GetDataSet(cmd, serviceLogin, string.Empty);

				return (int)set.Tables[0].Rows[0][0];
			}
		}

		/// <summary>
		/// Get security object for the specified token. Session is not renewed.
		/// </summary>
		/// <param name="token">
		/// Security token to recover from Sessions
		/// </param>
		/// <returns>
		/// Security object for the token
		/// </returns>
		/// <remarks>
		/// Don't call this unless you're a service!  Say this instead:
		///     SecurityClassSecurity = (SecurityClass) Session["Security"];
		/// </remarks>
		public SecurityClass GetSecurityWithoutSessionRenewal(string token)
		{
			return this.GetSecurity(token, checkSessionTimeout: true, renewSession: false);
		}

		/// <summary>
		/// Get security object for the specified token.
		/// </summary>
		/// <param name="token">
		/// Security token to recover from Sessions
		/// </param>
		/// <returns>
		/// Security object for the token
		/// </returns>
		/// <remarks>
		/// Don't call this unless you're a service!  Say this instead:
		///	SecurityClassSecurity = (SecurityClass) Session["Security"];
		/// </remarks>
		public SecurityClass GetSecurity(string token)
		{
			return this.GetSecurity(token, checkSessionTimeout: true);
		}

		/// <summary>
		/// Get security object for the specified token.
		/// </summary>
		/// <param name="token">
		/// Security token to recover from Sessions
		/// </param>
		/// <param name="checkSessionTimeout">Boolean indicating where session timeout should be checked.</param>
		/// <returns>
		/// Security object for the token
		/// </returns>
		/// <remarks>
		/// Don't call this unless you're a service!  Say this instead:
		///	SecurityClassSecurity = (SecurityClass) Session["Security"];
		/// </remarks>
		// ReSharper disable once UnusedParameter.Local
		private SecurityClass GetSecurity(string token, bool checkSessionTimeout)
		{
			return this.GetSecurity(token, checkSessionTimeout, renewSession: true);
		}

		/// <summary>
		/// Get security object for the specified token.
		/// </summary>
		/// <param name="token">
		/// Security token to recover from Sessions
		/// </param>
		/// <param name="checkSessionTimeout">
		/// Boolean indicating where session timeout should be checked.
		/// </param>
		/// <param name="renewSession">
		/// Boolean indicating whether session timeout should be renewed.
		/// </param>
		/// <returns>
		/// Security object for the token
		/// </returns>
		/// <remarks>
		/// Don't call this unless you're a service!  Say this instead:
		///     SecurityClassSecurity = (SecurityClass) Session["Security"];
		/// </remarks>
		private SecurityClass GetSecurity(string token, bool checkSessionTimeout, bool renewSession)
		{
			var hardwareKey = new HardwareKeyClass();

			uint usenewkey = hardwareKey.GetUseNewLicenseFile();
			if (usenewkey == 0)
			{
				uint options = hardwareKey.GetOptionsCell();

				if ((options & 0x4000) == 0)
				{
					throw new Exception("Hardware Key Failure");
				}
			}
			else
			{
				ushort version = hardwareKey.GetProgramVersionLIN();
				if (version == 0)
				{
					throw new Exception("Hardware Key Failure");
				}
			}

			var session = new SessionClass();
			var security = new SecurityClass { SiteGuid = Guids.SiteAdminGuid };

			// Add necessary rights
			security.AddRight(RIGHT.VIEW_USERS);

			session.Token = new Guid(token);
			using (var cmd = new SqlCommand())
			{
				session.SelectSQL(cmd, ContextUtil.IsInTransaction);
				session.LoadObject(this.consolidatedDa.GetDataSet(cmd, security));
			}
			if (session.Token == Guid.Empty)
			{
            throw new FMSessionInvalidException(string.Format("Invalid session token {0}.", token));

         }

			if (session.UserGuid == Guid.Empty)
			{
				throw new FMSessionInvalidException("UserGuid is empty aka credential error");
			}

         if (checkSessionTimeout
				&& (((DateTimeOffset.Now - session.UpdatedDate).TotalMinutes > session.Timeout) && (session.Timeout > 0)))
			{
            throw new FMSessionInvalidException(FMSessionInvalidException.SessionTimedOutExceptionMessage);
         }


         var user = new UserClass { IdentityGuid = session.UserGuid };

			using (var cmd = new SqlCommand())
			{
				user.SelectSQL(cmd, ContextUtil.IsInTransaction);
				user.LoadObject(this.consolidatedDa.GetDataSet(cmd, security));
			}

			if (renewSession)
			{
				// Reset the UpdatedDate to now
				session.UpdatedDate = DateTimeOffset.Now;
				session.UpdatedBy = session.UserID;
				using (var cmd = new SqlCommand())
				{
					session.UpdateSQL(cmd);
					this.consolidatedDa.ExecuteQuery(security, cmd);
				}
			}
			security.Token = session.Token;
			security.UserID = session.UserID;
			security.Password = user.Password;
			security.ForcePasswordUpdate = user.ChangePassword;
			security.UserGuid = session.UserGuid;
			security.SiteID = session.SiteID;
			security.SiteGuid = session.SiteGuid;
			security.LoginSiteID = session.LoginSiteID;
			security.LoginSiteGuid = session.LoginSiteGuid;
			security.CSRFToken = session.CSRFToken;
			security.ActiveDirectoryUser = user.ActiveDirectoryUser;

			security.SkipSessionTimeUpdate = false;

			var rights = new RightsClass();
			security.RightCollection = rights.EnumerateByUserBySite(security, session.UserGuid, session.SiteGuid);

			return security;
		}

		public SiteClass GetUsingGuid(SecurityClass security, Guid identityGuid)
		{
			return this.Get(security, identityGuid, bGetMemberSites: true, getSchedulesAndProcessVariables: true, bGetAssociatedAliases: true);
		}

		/// <summary>
		/// Determines whether the site identified by the identityGuid is a Site Group.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="identityGuid">
		/// The identity GUID of the site to check.
		/// </param>
		/// <returns>
		/// <c>true</c> if the site identified by the identityGuid is a Site Group; otherwise, <c>false</c>.
		/// </returns>
		public bool IsSiteGroup(SecurityClass security, Guid identityGuid)
		{
			using (var cmd = new SqlCommand())
			{
				SiteClass.IsGroupSiteSQL(cmd, identityGuid);

				DataSet results = this.consolidatedDa.GetDataSet(cmd, security);

				if (results.Tables.Count == 0 || results.Tables[0].Rows.Count == 0)
				{
					throw new ApplicationException("Site not found.");
				}

				var isSiteGroup = (bool)results.Tables[0].Rows[0][0];
				return isSiteGroup;
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public SecurityLoginResponse Login2(SecurityLoginRequest sr)
		{
			return this.Login(
				sr.SiteID,
				sr.UserID,
				sr.Password,
				sr.CACEnabled,
				sr.TimeOut);
		}

		/// <summary>
		/// This method processes the login for a given user and
		///	validates that user. Session entry for this user can be
		///	removed from tblSessions table if session times out.
		/// </summary>
		/// <param name="siteId">
		/// </param>
		/// <param name="userId">
		/// </param>
		/// <param name="password">
		/// </param>
		/// <param name="cacEnable">
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public SecurityLoginResponse Login(
							string siteId,
							string userId,
							string password,
			bool cacEnable)
		{
			return this.Login(siteId, userId, password, cacEnable, -1);
		}

		/// <summary>
		/// Checks if user password meets enhanced strong requirements.
		/// If user is administrator, it also checks that a default password is provided in FMBusinessService 
		/// web.config file and it is enhanced strong password. If default password is not specified in web.config
		/// or default password is not enhanced strong password, it prevents
		/// all users from logging into the system. 
		/// If administrator password is not enhanced strong or matches default password that is in web.config,
		/// then administrator is required to change it during login, and it must be done on the machine
		/// hosting the web server.
		/// If non-administrator user has password that is not enhanced strong, then user is locked out.
		/// Administrator must assign an enhanced strong password to enable user account.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="user"></param>
		/// <returns>false = indicates that administrator must change password while logging in from machine hosting the web server.</returns>
		protected bool ValidateBsmePasswordStrength(SecurityClass security, UserClass user)
		{
			var hardwareKey = new HardwareKeyClass();
			bool isBsme = hardwareKey.IsDescKey();

			if (isBsme)
			{
				string DefaultBsmePassword = System.Configuration.ConfigurationManager.AppSettings["DefaultPassword"];

				if (DefaultBsmePassword == null)
				{
					DefaultBsmePassword = string.Empty;
				}
				if (string.IsNullOrWhiteSpace(DefaultBsmePassword) || DefaultBsmePassword.Length < 15 || FMCore.FuelsManagerExtensions.IsEnhancedStrongPassword(DefaultBsmePassword) == false)
				{
					throw new Exception("Fuelsmanager will not allow users to login unless a default enhanced strong password is specified for administrator in FMBusinessServices/web.config file.");
				}

				DefaultBsmePassword = DefaultBsmePassword.Trim();

				UsersClass users = new UsersClass();

				bool enhancedStrongPassword = user.Password.Length >= 15 &&
					 FMCore.FuelsManagerExtensions.IsEnhancedStrongPassword(user.Password) == true;
				bool updateUser = false;

				if (user.IsAdministrator && user.Password == DefaultBsmePassword && user.ChangePassword == false)
				{
					// if user is an administrator and current password is default password, password needs to be changed at login time.
					user.ChangePassword = true;
					updateUser = true;

				}

				if (enhancedStrongPassword == false)
				{
					if (user.IsAdministrator)
					{
						//assign enhanced strong password to administrator, if current one is not
						user.Password = DefaultBsmePassword;//default password is always enhanced strong.
						user.ChangePassword = true;
					}
					else
					{
						//disable non-administrator user accounts if password is not strong enhanced.
						user.InactivityLockout = true;
					}
					updateUser = true;
				}

				if (updateUser)
				{
					users.Modify(security, user);
				}

				if (user.IsAdministrator && user.Password == DefaultBsmePassword)
				{
					return false;//indicates that administrator must change password while logging in from machine hosting the web server.
				}

			}
			return true;
		}

		/// <summary>
		/// This method processes the login for a given user and
		///	validates that user.
		/// </summary>
		/// <param name="siteId">
		/// </param>
		/// <param name="userId">
		/// </param>
		/// <param name="password">
		/// </param>
		/// <param name="cacEnable">
		/// </param>
		/// <param name="timeout">
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public SecurityLoginResponse Login(
														string siteId,
														string userId,
														string password,
														bool cacEnable,
														int timeout)
		{
			var alarmAndEventLogs = new AlarmAndEventLogsClass();
			var loginResponse = new SecurityLoginResponse();

			var hardwareKey = new HardwareKeyClass();
			hardwareKey.ForceRefreshLicenseFile();

			uint usenewkey = hardwareKey.GetUseNewLicenseFile();
			if (usenewkey == 0)
			{
				uint options = hardwareKey.GetOptionsCell();

				if ((options & 0x4000) == 0)
				{
					throw new FMHardwareKeyFailureException();
				}
			}
			ushort version = FMChannelHelper.MakeCall<IHardwareKey, ushort>(x => x.CheckActivatedLicenceVersion());

			if (version != 9999 && version != 120)
			{
				throw new Exception("Wrong License Key Version " + (version / 10.0).ToString(CultureInfo.InvariantCulture));
			}

			var innerSecurity = new SecurityClass();
			SiteClass site = this.GetByID(innerSecurity, siteId);

			innerSecurity.SiteGuid = site.IdentityGuid;
			innerSecurity.LoginSiteGuid = site.IdentityGuid;
			innerSecurity.UserID = userId;
			innerSecurity.Password = password;
			innerSecurity.SiteID = site.ID;
			innerSecurity.LoginSiteID = site.ID;

			if (innerSecurity.SiteGuid == Guid.Empty)
			{
				throw new FMSiteNotFoundException();
			}

			innerSecurity.AddRight(RIGHT.VIEW_USERS);
			innerSecurity.AddRight(RIGHT.VIEW_USER_GROUPS);
			innerSecurity.AddRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS);

			var users = new UsersClass();
			UserClass user = users.GetByIDForLogOn(innerSecurity, userId);

			innerSecurity.ActiveDirectoryUser = user.ActiveDirectoryUser;

			if (cacEnable)
			{
				innerSecurity.Password = user.Password;
				innerSecurity.ClientCertLogOn = true;
			}
			else
			{
				loginResponse.AdminMustChangePasswordAtWebServer = (this.ValidateBsmePasswordStrength(innerSecurity, user) == false);
			}

			if (user.IdentityGuid.IsEmpty())
			{
				innerSecurity.UserID = DBAccess.ServiceLoginAccess; // required for AlarmAnEventLogs.Add to work with DESC key
				user.ID = userId;
				user.SiteID = siteId;
				alarmAndEventLogs.Add(innerSecurity, user.LoginFailedEvent);
				loginResponse.ChangePassword = false;
				loginResponse.DaysUntilExpiration = 999;

				if (hardwareKey.IsDescKey())
				{
					innerSecurity.UserID = userId;
					this.consolidatedDa.SplashLogin(innerSecurity);
				}

				loginResponse.Security = innerSecurity;
				loginResponse.Result = "Login Failed";
				return loginResponse;
			}

			if (user.InactivityLockout)
			{
				innerSecurity.UserID = DBAccess.ServiceLoginAccess; // required for AlarmAnEventLogs.Add to work with DESC key
				user.ID = userId;
				user.SiteID = siteId;
				alarmAndEventLogs.Add(innerSecurity, user.LoginFailedEvent);
				loginResponse.ChangePassword = false;
				loginResponse.DaysUntilExpiration = 999;
				loginResponse.Security = innerSecurity;
				loginResponse.Result = "User Locked Out";
				return loginResponse;
			}

			if (user.AccountExpirationDate < DateTime.Today)
			{
				innerSecurity.UserID = DBAccess.ServiceLoginAccess;
				user.SiteID = siteId;
				alarmAndEventLogs.Add(innerSecurity, user.LoginFailedEvent);
				loginResponse.ChangePassword = false;
				loginResponse.DaysUntilExpiration = 999;
				loginResponse.Security = innerSecurity;
				loginResponse.Result = "User account has expired. Please contact the FuelsManager Administrator.";
				return loginResponse;
			}

			// the following code is being removed since the time is hardcoded at one year in advance and there is no requirement to expire user groups BDS 6-10-2020
			/*
		bool activeGroup = false;
		foreach (UserGroupMapClass userGroupMap in user.UserGroupMapCollection)
		{
			if (userGroupMap.ExpirationDate >= DateTime.Today)
			{
				activeGroup = true;
				break;
			}
		}

		if (!activeGroup && user.UserGroupMapCollection.Count > 0)
		{
			innerSecurity.UserID = DBAccess.ServiceLoginAccess;
			user.SiteID = siteId;
			alarmAndEventLogs.Add(innerSecurity, user.LoginFailedEvent);
			loginResponse.ChangePassword = false;
			loginResponse.DaysUntilExpiration = 999;
			loginResponse.Security = innerSecurity;
			loginResponse.Result = "Login Failed. All assigned groups have expired. Please contact the FuelsManager Administrator.";
			return loginResponse;
		}
		*/

			bool memberOfAdministratorGroup = false;
			foreach (UserGroupMapClass userGroupMap in user.UserGroupMapCollection)
			{
				if (userGroupMap.GroupGuid == Guids.GroupAdminGuid)
				{
					memberOfAdministratorGroup = true;
					break;
				}
			}

			var fmSecurityVal = new FMSecurityValidation(user, site);
			fmSecurityVal.ParseUserInfo();
			fmSecurityVal.ParseConfiguration();

			// Check for single sign on mode. We want to ignore checking password if in single
			// sign on mode and the user is an active directory user.
			bool activeDirectoryUserAndSsoMode = false;
			bool ssoMode = this.IsSsoMode(innerSecurity);

			if (ssoMode && user.ActiveDirectoryUser)
			{
				activeDirectoryUserAndSsoMode = true;
			}

			// Verify that the user's Password matches. If does not and the number
			// of Password failure attempts exceeds the lockout threshold limit, then
			// throw a lockout exception. Otherwise, throw a user not found exception.
			if (cacEnable == false && activeDirectoryUserAndSsoMode == false && (this.CheckCurrentPassword(user, password) == false))
			{
				user.PasswordLockoutCount++;
				innerSecurity.UserID = user.ID;
				users.ModifyPasswordCount(innerSecurity, user);
				user.SiteID = siteId;
				loginResponse.ChangePassword = user.ChangePassword;
				loginResponse.DaysUntilExpiration = 999;
				innerSecurity.UserID = DBAccess.ServiceLoginAccess;
				alarmAndEventLogs.Add(innerSecurity, user.LoginFailedEvent);

				if (fmSecurityVal.LockedOut(user.PasswordLockoutCount))
				{
					user.InactivityLockout = true;
					users.Modify(innerSecurity, user);
					if (hardwareKey.IsDescKey())
					{
						innerSecurity.UserID = user.ID;
						this.consolidatedDa.SplashLogin(innerSecurity);
						innerSecurity.UserID = DBAccess.ServiceLoginAccess;
					}

					loginResponse.Security = innerSecurity;
					loginResponse.Result = "User locked out";
					return loginResponse;
				}

				if (cacEnable == false && string.IsNullOrEmpty(user.Password))
				{
					user.InactivityLockout = true;
					users.Modify(innerSecurity, user);

 					if (hardwareKey.IsDescKey())
					{
						innerSecurity.UserID = user.ID;
						this.consolidatedDa.SplashLogin(innerSecurity);
						innerSecurity.UserID = DBAccess.ServiceLoginAccess;
					}

					loginResponse.Security = innerSecurity;
					loginResponse.Result = "Corrupted password, user locked out";
					return loginResponse;
				}

				if (hardwareKey.IsDescKey())
				{
					innerSecurity.UserID = user.ID;
					this.consolidatedDa.SplashLogin(innerSecurity);
				}

				loginResponse.Security = innerSecurity;
				loginResponse.Result = "Login Failed";
				return loginResponse;
			}

			if (fmSecurityVal.InactivityInExcess())
			{
				loginResponse.ChangePassword = user.ChangePassword;
				loginResponse.DaysUntilExpiration = 999;

				user.SiteID = siteId;
				innerSecurity.UserID = DBAccess.ServiceLoginAccess; // required for AlarmAnEventLogs.Add to work with DESC key
				alarmAndEventLogs.Add(innerSecurity, user.LoginFailedEvent);

				if (hardwareKey.IsDescKey())
				{
					innerSecurity.UserID = userId;
					this.consolidatedDa.SplashLogin(innerSecurity);
				}

				loginResponse.Security = innerSecurity;
				if (!user.InactivityLockout)
				{
					user.InactivityLockout = true;
					user.PasswordLockoutCount = 0;
					users.Modify(innerSecurity, user);
				}

				if (fmSecurityVal.CheckNumberOfTries(user.PasswordLockoutCount))
				{
					loginResponse.Result = "User locked out.";
					return loginResponse;
				}

				loginResponse.Result = "User locked out due to exceeding inactivity period.";
				return loginResponse;
			}

			// Check to see if the Password needs to be changed due
			// to age.
			if ((cacEnable == false) && fmSecurityVal.ExceededPasswordAge())
			{
				if (hardwareKey.IsDescKey() && !memberOfAdministratorGroup)
				{
					this.consolidatedDa.SplashLogin(innerSecurity);

					// Lockout all DESC users on password age with the exception of administrators (IGO 2009-Sep-25)
					loginResponse.ChangePassword = user.ChangePassword;
					loginResponse.DaysUntilExpiration = 999;
					loginResponse.Security = innerSecurity;

					user.InactivityLockout = true;
					users.Modify(innerSecurity, user);

					loginResponse.Result = "User locked out due to exceeding password age. Please contact the FuelsManager Administrator for assistance.";
					return loginResponse;
				}

				user.ChangePassword = true;
			}

			// Check to see if the Password needs to be changed due
			// to being too weak, most likely as a result of policy change.  DESC/DLA users do NOT get locked out in this case
			if ((cacEnable == false) && (!fmSecurityVal.MinimumOfCharacters(user.Password) || !fmSecurityVal.MeetsStrongPassword(user.Password)))
			{
				user.ChangePassword = true;
			}

			// Don't let the user login if not part of any group. 
			// DO THIS LAST!
			if (user.UserGroupMapCollection.Count == 0)
			{
				loginResponse.ChangePassword = false;
				loginResponse.DaysUntilExpiration = 999;
				if (hardwareKey.IsDescKey())
				{
					// only US DoD cares about this
					this.consolidatedDa.SplashLogin(innerSecurity);
				}

				loginResponse.Security = innerSecurity;

				// Set to service login so that the event can actually be logged.
				innerSecurity.UserID = DBAccess.ServiceLoginAccess;
				alarmAndEventLogs.Add(innerSecurity, user.LoginFailedNoGroupEvent);
				loginResponse.Result = "User is not a member of any group. Please contact the FuelsManager Administrator.";
				return loginResponse;
			}

			// The days until expiration are set in "ExceedPasswordAge()". (IGO 2009-Aug-10)
			loginResponse.DaysUntilExpiration = fmSecurityVal.DaysUntilExpiration;
			var session = new SessionClass
			{
				Token = Guid.NewGuid(),
				UserGuid = user.IdentityGuid,
				SiteGuid = innerSecurity.SiteGuid,
				LoginSiteGuid = innerSecurity.SiteGuid,
				CreatedDate = DateTimeOffset.Now,
				CreatedBy = innerSecurity.UserID
			};

			session.UpdatedDate = session.CreatedDate;
			session.UpdatedBy = innerSecurity.UserID;
			session.CSRFToken = innerSecurity.CSRFToken;

			// Get the max users settings
			session.MaxConcurrentSessionsPerUser = GetConfigurationIntValue(innerSecurity, ConfigurationSettingDOClass.Key_MaxConcurrentSessionsPerUser, 0);
			if (session.MaxConcurrentSessionsPerUser > 0)
			{
				this.CheckNumberOfSessions(innerSecurity, session);
			}

			if (!user.IsAdministrator)
			{
				string message;
				if (!this.CheckNumberOfConcurrentUsers(innerSecurity, session, alarmAndEventLogs, out message))
				{
					loginResponse.Security = innerSecurity;
					loginResponse.Result = message;
					return loginResponse;
				}
			}

			var groupClass = new GroupsClass();

			GroupCollectionClass groups = groupClass.EnumerateByUserDuringLogOn(innerSecurity, user.IdentityGuid);

			timeout = 0;   // default to no timeout set
			foreach (GroupClass group in groups)
			{
				// take the most restrctive timeout setting. A value of 0 means never timeout
				if ((group.SessionTimeout > 0 && timeout > group.SessionTimeout) ||
					 (group.SessionTimeout > 0 && timeout == 0))
				{
					timeout = group.SessionTimeout;  //we want the lowest session timeout that is non zero
				}
			}

			if (user != null)
			{
				loginResponse.LastLoginDateAndTime = user.LastLoginDate;
				loginResponse.NumberOfFailedAttempts = user.PasswordLockoutCount;
			}

			loginResponse.TimeOut = timeout;
			session.Timeout = timeout;
			this.CompleteUserLogin(userId, alarmAndEventLogs, innerSecurity, user, session, cacEnable);

			// If set to true, then the user must reset Password.
			loginResponse.ChangePassword = user.ChangePassword;

			loginResponse.Security = innerSecurity;
			var rightsClass = new RightsClass();
			loginResponse.Security.RightCollection = rightsClass.EnumerateByUserBySite(innerSecurity, user.IdentityGuid, site.IdentityGuid);

			SecurityClass tmpSecurity = loginResponse.Security;
			this.RefreshTransactionSecurityRightsCache(ref tmpSecurity);
			loginResponse.Security = tmpSecurity;
			loginResponse.Result = null;

			return loginResponse;
		}

		private static int GetConfigurationIntValue(SecurityClass innerSecurity, string key, int defaultValue)
        {
            ConfigurationSettingsClass configs = new ConfigurationSettingsClass();
            string setting = configs.GetKeyValueByKey(innerSecurity, key);
            int value = defaultValue;
            if (!string.IsNullOrWhiteSpace(setting))
            {
                value = Convert.ToInt32(setting);
            }
			return value;
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public string Login(
			out bool changePassword, out int daysUntilExpiration, out SecurityClass security, SecurityLoginRequest sr)
		{
			return this.Login(
				sr.SiteID,
				sr.UserID,
				sr.Password,
				sr.CACEnabled,
				sr.TimeOut,
				out changePassword,
				out daysUntilExpiration,
				out security);
		}

		/// <summary>
		/// This method processes the login for a given user and
		///	validates that user. Session entry for this user can be
		///	removed from tblSessions table if session times out.
		/// </summary>
		/// <param name="siteID">
		/// </param>
		/// <param name="userID">
		/// </param>
		/// <param name="password">
		/// </param>
		/// <param name="cacEnable">
		/// </param>
		/// <param name="changePassword">
		/// </param>
		/// <param name="daysUntilExpiration">
		/// </param>
		/// <param name="security">
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public string Login(
			string siteID,
			string userID,
			string password,
			bool cacEnable,
			out bool changePassword,
			out int daysUntilExpiration,
			out SecurityClass security)
		{
			return this.Login(siteID, userID, password, cacEnable, -1, out changePassword, out daysUntilExpiration, out security);
		}

		/// <summary>
		/// This method processes the login for a given user and
		///	validates that user.
		/// </summary>
		/// <param name="siteID">
		/// </param>
		/// <param name="userID">
		/// </param>
		/// <param name="password">
		/// </param>
		/// <param name="cacEnable">
		/// </param>
		/// <param name="timeout">
		/// </param>
		/// <param name="changePassword">
		/// </param>
		/// <param name="daysUntilExpiration">
		/// </param>
		/// <param name="security">
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public string Login(
			string siteID,
			string userID,
			string password,
			bool cacEnable,
			int timeout,
			out bool changePassword,
			out int daysUntilExpiration,
			out SecurityClass security)
		{
			var alarmAndEventLogs = new AlarmAndEventLogsClass();

			var hardwareKey = new HardwareKeyClass();
			hardwareKey.ForceRefreshLicenseFile();

			uint usenewkey = hardwareKey.GetUseNewLicenseFile();
			if (usenewkey == 0)
			{
				uint options = hardwareKey.GetOptionsCell();

				if ((options & 0x4000) == 0)
				{
					throw new FMHardwareKeyFailureException();
				}

			}
			ushort version = FMChannelHelper.MakeCall<IHardwareKey, ushort>(x => x.CheckActivatedLicenceVersion());

			if (version != 9999 && version != 120)
			{
				throw new Exception("Wrong License Key Version " + (version / 10.0).ToString(CultureInfo.InvariantCulture));
			}

			var innerSecurity = new SecurityClass();
			SiteClass site = this.GetByID(innerSecurity, siteID);

			innerSecurity.SiteGuid = site.IdentityGuid;
			innerSecurity.LoginSiteGuid = site.IdentityGuid;
			innerSecurity.UserID = userID;
			innerSecurity.Password = password;
			innerSecurity.SiteID = site.ID;
			innerSecurity.LoginSiteID = site.ID;

			if (innerSecurity.SiteGuid == Guid.Empty)
			{
				throw new FMSiteNotFoundException();
			}

			innerSecurity.AddRight(RIGHT.VIEW_USERS);
			innerSecurity.AddRight(RIGHT.VIEW_USER_GROUPS);
			innerSecurity.AddRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS);

			var users = new UsersClass();
			UserClass user = users.GetByIDForLogOn(innerSecurity, userID);

			if (cacEnable)
			{
				innerSecurity.Password = user.Password;
				innerSecurity.ClientCertLogOn = true;
			}

			if (user.IdentityGuid.IsEmpty())
			{
				innerSecurity.UserID = DBAccess.ServiceLoginAccess; // required for AlarmAnEventLogs.Add to work with DESC key
				user.ID = userID;
				user.SiteID = siteID;
				alarmAndEventLogs.Add(innerSecurity, user.LoginFailedEvent);
				changePassword = false;
				daysUntilExpiration = 999;
				if (hardwareKey.IsDescKey())
				{
					innerSecurity.UserID = userID;
					this.consolidatedDa.SplashLogin(innerSecurity);
				}

				security = innerSecurity;
				return "Login Failed";
			}

			if (user.InactivityLockout)
			{
				innerSecurity.UserID = DBAccess.ServiceLoginAccess; // required for AlarmAnEventLogs.Add to work with DESC key
				user.ID = userID;
				user.SiteID = siteID;
				alarmAndEventLogs.Add(innerSecurity, user.LoginFailedEvent);
				changePassword = false;
				daysUntilExpiration = 999;
				security = innerSecurity;
				return "User Locked Out";
			}

			bool memberOfAdministratorGroup = false;
			foreach (UserGroupMapClass userGroupMap in user.UserGroupMapCollection)
			{
				if (userGroupMap.GroupGuid == Guids.GroupAdminGuid)
				{
					memberOfAdministratorGroup = true;
					break;
				}
			}

			var fmSecurityVal = new FMSecurityValidation(user, site);
			fmSecurityVal.ParseUserInfo();
			fmSecurityVal.ParseConfiguration();

			// Verify that the user's Password matches. If does not and the number
			// of Password failure attempts exceeds the lockout threshold limit, then
			// throw a lockout exception. Otherwise, throw a user not found exception.
			if ((cacEnable == false) && (this.CheckCurrentPassword(user, password) == false))
			{
				user.PasswordLockoutCount++;
				innerSecurity.UserID = user.ID;
				Guid origSiteGuid = innerSecurity.SiteGuid;
				try
				{
					innerSecurity.SiteGuid = user.SiteGuid;
					users.ModifyPasswordCount(innerSecurity, user);
				}
				finally
				{
					innerSecurity.SiteGuid = origSiteGuid;
				}
				user.SiteID = siteID;
				changePassword = user.ChangePassword;
				daysUntilExpiration = 999;
				innerSecurity.UserID = DBAccess.ServiceLoginAccess;
				alarmAndEventLogs.Add(innerSecurity, user.LoginFailedEvent);

				if (fmSecurityVal.LockedOut(user.PasswordLockoutCount))
				{
					user.InactivityLockout = true;
					try
					{
						innerSecurity.SiteGuid = user.SiteGuid;
						users.Modify(innerSecurity, user);
					}
					finally
					{
						innerSecurity.SiteGuid = origSiteGuid;
					}
					if (hardwareKey.IsDescKey())
					{
						innerSecurity.UserID = user.ID;
						this.consolidatedDa.SplashLogin(innerSecurity);
						innerSecurity.UserID = DBAccess.ServiceLoginAccess;
					}

					security = innerSecurity;
					return "User locked out";
				}

				if (hardwareKey.IsDescKey())
				{
					innerSecurity.UserID = user.ID;
					this.consolidatedDa.SplashLogin(innerSecurity);
				}

				if (string.IsNullOrEmpty(user.Password))
				{
					user.InactivityLockout = true;
					try
					{
						innerSecurity.SiteGuid = user.SiteGuid;
						users.Modify(innerSecurity, user);
					}
					finally
					{
						innerSecurity.SiteGuid = origSiteGuid;
					}
					if (hardwareKey.IsDescKey())
					{
						innerSecurity.UserID = user.ID;
						this.consolidatedDa.SplashLogin(innerSecurity);
						innerSecurity.UserID = DBAccess.ServiceLoginAccess;
					}

					security = innerSecurity;
					return "Corrupted password, user locked out";
				}

				security = innerSecurity;
				return "Login Failed";
			}

			if (fmSecurityVal.InactivityInExcess())
			{
				changePassword = user.ChangePassword;
				daysUntilExpiration = 999;

				user.SiteID = siteID;
				innerSecurity.UserID = DBAccess.ServiceLoginAccess; // required for AlarmAnEventLogs.Add to work with DESC key
				alarmAndEventLogs.Add(innerSecurity, user.LoginFailedEvent);

				if (hardwareKey.IsDescKey())
				{
					innerSecurity.UserID = userID;
					this.consolidatedDa.SplashLogin(innerSecurity);
				}

				security = innerSecurity;
				if (!user.InactivityLockout)
				{
					user.InactivityLockout = true;
					user.PasswordLockoutCount = 0;
					Guid origSiteGuid = innerSecurity.SiteGuid;
					try
					{
						innerSecurity.SiteGuid = user.SiteGuid;
						users.Modify(innerSecurity, user);
					}
					finally
					{
						innerSecurity.SiteGuid = origSiteGuid;
					}
				}

				return fmSecurityVal.CheckNumberOfTries(user.PasswordLockoutCount)
					? "User locked out."
					: "User locked out due to exceeding inactivity period.";
			}

			// Check to see if the Password needs to be changed due
			// to age.
			if ((cacEnable == false) && fmSecurityVal.ExceededPasswordAge())
			{
				if (hardwareKey.IsDescKey() && !memberOfAdministratorGroup)
				{
					this.consolidatedDa.SplashLogin(innerSecurity);

					// Lockout all DESC users on password age with the exception of administrators (IGO 2009-Sep-25)
					changePassword = user.ChangePassword;
					daysUntilExpiration = 999;
					security = innerSecurity;

					user.InactivityLockout = true;
					Guid origSiteGuid = innerSecurity.SiteGuid;
					try
					{
						innerSecurity.SiteGuid = user.SiteGuid;
						users.Modify(innerSecurity, user);

					}
					finally
					{
						innerSecurity.SiteGuid = origSiteGuid;
					}
					return
						"User locked out due to exceeding password age. Please contact the FuelsManager Administrator for assistance.";
				}

				user.ChangePassword = true;
			}

			// Check to see if the Password needs to be changed due
			// to being too weak, most likely as a result of policy change.  DESC/DLA users do NOT get locked out in this case
			if ((cacEnable == false) && (!fmSecurityVal.MinimumOfCharacters(user.Password) || !fmSecurityVal.MeetsStrongPassword(user.Password)))
			{
				user.ChangePassword = true;
			}

			// Don't let the user login if not part of any group. 
			// DO THIS LAST!
			if (user.UserGroupMapCollection.Count == 0)
			{
				changePassword = false;
				daysUntilExpiration = 999;
				if (hardwareKey.IsDescKey())
				{
					// only US DoD cares about this
					this.consolidatedDa.SplashLogin(innerSecurity);
				}

				security = innerSecurity;

				// Set to service login so that the event can actually be logged.
				innerSecurity.UserID = DBAccess.ServiceLoginAccess;
				alarmAndEventLogs.Add(innerSecurity, user.LoginFailedNoGroupEvent);
				return "User is not a member of any group. Please contact the FuelsManager Administrator.";
			}

			// The days until expiration are set in "ExceedPasswordAge()". (IGO 2009-Aug-10)
			daysUntilExpiration = fmSecurityVal.DaysUntilExpiration;
			var session = new SessionClass
			{
				Token = Guid.NewGuid(),
				UserGuid = user.IdentityGuid,
				SiteGuid = innerSecurity.SiteGuid,
				LoginSiteGuid = innerSecurity.SiteGuid,
				CreatedDate = DateTimeOffset.Now,
				CreatedBy = innerSecurity.UserID
			};

			session.UpdatedDate = session.CreatedDate;
			session.UpdatedBy = innerSecurity.UserID;
			session.MaxConcurrentSessionsPerUser = GetConfigurationIntValue(innerSecurity, ConfigurationSettingDOClass.Key_MaxConcurrentSessionsPerUser, 0);
			session.CSRFToken = innerSecurity.CSRFToken;

			if (session.MaxConcurrentSessionsPerUser > 0)
			{
				this.CheckNumberOfSessions(innerSecurity, session);
			}
			if (!user.IsAdministrator)
			{
				string message;
				if (!this.CheckNumberOfConcurrentUsers(innerSecurity, session, alarmAndEventLogs, out message))
				{
					changePassword = false;
					security = innerSecurity;
					return message;
				}
			}

			var groupClass = new GroupsClass();

			GroupCollectionClass groups = groupClass.EnumerateByUserDuringLogOn(innerSecurity, user.IdentityGuid);

			timeout = 0;   // default to no timeout set
			foreach (GroupClass group in groups)
			{
				// take the most restrctive timeout setting. A value of 0 means never timeout
				if ((group.SessionTimeout > 0 && timeout > group.SessionTimeout) ||
					(group.SessionTimeout > 0 && timeout == 0))
				{
					timeout = group.SessionTimeout;  //we want the lowest session timeout that is non zero
				}
			}

			session.Timeout = timeout;
			this.CompleteUserLogin(userID, alarmAndEventLogs, innerSecurity, user, session, cacEnable);

			// If set to true, then the user must reset Password.
			changePassword = user.ChangePassword;

			security = innerSecurity;
			var rightsClass = new RightsClass();
			security.RightCollection = rightsClass.EnumerateByUserBySite(innerSecurity, user.IdentityGuid, site.IdentityGuid);
			this.RefreshTransactionSecurityRightsCache(ref security);

			return null;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Logout(SecurityClass security)
		{
			var user = new UserClass { ID = security.UserID };

			var session = new SessionClass { Token = security.Token };

			// It is unclear why we keep track of the LastLogoffDate - it does not appear to be used and a user is not required to log out of the system.
			// This functionality has been commented out for now until a determination can be made as to whether it is necessary.
			// It will cause deadlocks during concurrent login/logouts.
			// using (var cmd = new SqlCommand())
			// {
			//	user.UpdateLogoutSQL(cmd);
			//	this.consolidatedDa.ExecuteQuery(security, cmd);
			// }

			var alarmAndEventLogs = new AlarmAndEventLogsClass();
			alarmAndEventLogs.Add(security, user.LoggedOutEvent);

			if (security.Password != null
			&& security.Password.Length == 0)
			{
				using (var cmd = new SqlCommand())
				{
					session.PurgeSQL(cmd);
					this.consolidatedDa.ExecuteSessionCleanupQuery(security, cmd, DBAccess.ServiceLoginAccess);
				}
			}
			else
			{
				using (var cmd = new SqlCommand())
				{
					session.PurgeSQL(cmd);
					this.consolidatedDa.ExecuteSessionCleanupQuery(security, cmd);
				}
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void LogoutToken(string token)
		{
			SecurityClass security = this.GetSecurity(token, checkSessionTimeout: false);
			security.SiteGuid = security.LoginSiteGuid;
			var user = new UserClass { ID = security.UserID };

			var session = new SessionClass { Token = security.Token };

			var alarmAndEventLogs = new AlarmAndEventLogsClass();
			alarmAndEventLogs.Add(security, user.LoggedOutEvent);

			using (var cmd = new SqlCommand())
			{
				session.PurgeSQL(cmd);
				this.consolidatedDa.ExecuteSessionCleanupQuery(security, cmd);
			}

			// It is unclear why we keep track of the LastLogoffDate - it does not appear to be used and a user is not required to log out of the system.
			// This functionality has been commented out for now until a determination can be made as to whether it is necessary.
			// It will cause deadlocks during concurrent login/logouts.
			// using (var cmd = new SqlCommand())
			// {
			//	user.UpdateLogoutSQL(cmd);
			//	this.consolidatedDa.ExecuteQuery(security, cmd);
			// }
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, DATA_TYPE type, SiteClass site)
		{
			SiteClass oldSite = this.Get(security, site.SiteGuid, false, false, false);
			if (oldSite.IdentityGuid == Guid.Empty)
			{
				throw new Exception("Site Not Found");
			}

			site._AutomaticBOLNextNumber = oldSite._AutomaticBOLNextNumber;
			site._ManualBOLNextNumber = oldSite._ManualBOLNextNumber;
			site._OrderNextNumber = oldSite._OrderNextNumber;
			site._TransactionNextNumber = oldSite._TransactionNextNumber;

			this.Modify(security, type, site, false);
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, DATA_TYPE type, SiteClass site, bool updateDocumentNumbers)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (site == null)
			{
				throw new ArgumentNullException(nameof(site));
			}

			if (type == DATA_TYPE.CONFIG && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) && !security.HasRight(RIGHT.MODIFY_DISPATCH)
				&& !security.HasRight(RIGHT.VIEW_DISPATCH))
			{
				throw new FMInsufficientRightsException();
			}

			if (type == DATA_TYPE.SYNCCONFIG && !security.HasRight(RIGHT.MODIFY_CONFIGURATION_SETTINGS)
				&& !security.HasRight(RIGHT.MODIFY_SYNC_CONFIG_SITE_SETTINGS))
			{
				throw new FMInsufficientRightsException();
			}

			if (type == DATA_TYPE.DYNAMIC && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			if (type == DATA_TYPE.AUTOMIC && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
					&& !security.HasRight(RIGHT.CREATE_ORDERS) && !security.HasRight(RIGHT.MODIFY_ORDERS)
					&& !security.HasRight(RIGHT.CREATE_SUPPLY_ORDERS) && !security.HasRight(RIGHT.MODIFY_SUPPLY_ORDERS)
					&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) && !security.HasRight(RIGHT.MODIFY_DISPATCH)
				&& !security.HasRight(RIGHT.VIEW_DISPATCH))
			{
				throw new FMInsufficientRightsException();
			}

			this.Validate(security, site, type);

			Guid guid = this.GetIdentityGuid(security, site.ID);
			if (guid != Guid.Empty && guid != site.SiteGuid)
			{
				throw new Exception("Site Exists");
			}

			SiteClass oldSite = this.GetUsingGuid(security, site.SiteGuid);
			if (oldSite.IdentityGuid == Guid.Empty)
			{
				throw new Exception("Site Not Found");
			}

			// SYNCCONFIG is not capable of modifying any of the attributes below so we are only
			// interested in updating the current record that has to exist.
			if (type == DATA_TYPE.SYNCCONFIG)
			{
				// Modifications to a Site are allowed from a SiteGroup, force the Security.SiteGuid
				// to match the Site (use the old site index in case we are changing the site ID).
				security.SiteGuid = oldSite.IdentityGuid;

				site.UpdatedDate = DateTimeOffset.Now;
				site.UpdatedBy = security.UserID;

				try
				{
					using (var cmd = new SqlCommand())
					{
						site.UpdateSQL(cmd, type);
						this.consolidatedDa.ExecuteQuery(security, cmd);
					}

					SiteCache.AddSite(site); // Update Site in SiteCache
				}
				catch (SqlException except)
				{
					if (except.Message.IndexOf(DbTriggerError001, StringComparison.Ordinal) > -1)
					{
						throw new Exception(DbTriggerErrorMsg001);
					}
				}
			}
			else
			{
				if (!updateDocumentNumbers && type != DATA_TYPE.AUTOMIC)
				{
					if (oldSite._AutomaticBOLNextNumber != site._AutomaticBOLNextNumber
							|| oldSite._ManualBOLNextNumber != site._ManualBOLNextNumber
							|| oldSite._OrderNextNumber != site._OrderNextNumber
							|| oldSite._TransactionNextNumber != site._TransactionNextNumber)
					{
						throw new Exception("Change Document Numbers");
					}
				}

				// Update notes first since we might need to save a note guid in the site object
				bool deleteNotes = this.ModifyNotes(security, site);
				Guid noteGuidToBeDeleted = Guid.Empty;

				// if we detect that notes have been deleted, save the guid and delete the note record after nulling out the
				// guid in tblSites.
				if (deleteNotes)
				{
					noteGuidToBeDeleted = site.NoteGuid;
					site.NoteGuid = Guid.Empty;
				}

				// Modifications to a Site are allowed from a SiteGroup, force the Security.SiteGuid
				// to match the Site (use the old site index in case we are changing the site ID).
				security.SiteGuid = oldSite.IdentityGuid;

				site.UpdatedDate = DateTimeOffset.Now;
				site.UpdatedBy = security.UserID;

				try
				{
					using (var cmd = new SqlCommand())
					{
						cmd.CommandTimeout = 120;
						site.UpdateSQL(cmd, type);
						this.consolidatedDa.ExecuteQuery(security, cmd);
					}

					// Update Site in SiteCache
					SiteCache.AddSite(site);

					// If Exposted Settings on Point Change update Points
					if (oldSite.ID != site.ID
					|| oldSite.Number != site.Number)
					{
						var points = new Points();
						points.UpdateRowVersionBySite(security, site.SiteGuid);
					}
				}
				catch (SqlException except)
				{
					if (except.Message.IndexOf(DbTriggerError001, StringComparison.Ordinal) > -1)
					{
						throw new Exception(DbTriggerErrorMsg001);
					}
				}

				// We have to delete notes after deleting the Site to avoid violating the foreign key constraint
				if (deleteNotes)
				{
					var notes = new Notes();
					notes.Purge(security, noteGuidToBeDeleted);
				}

				if (site.UseLastKnownGoodTankData != oldSite.UseLastKnownGoodTankData)
				{
					var alarmAndEventLogs = new AlarmAndEventLogsClass();
					if (site.UseLastKnownGoodTankData)
					{
						alarmAndEventLogs.Add(security, site.UseLastKnownGoodTankDataEvent(security.UserID));
					}
					else
					{
						alarmAndEventLogs.Add(security, site.UseCurrentTankDataEvent(security.UserID));
					}
				}

				// Ensure that an Accounting general configuration exists; create a default if it doesn't
				GeneralConfigSR generalConfigSR = new GeneralConfigSR
				{
					Security = security,
					SiteGuid = site.SiteGuid,
					Request = GeneralConfigSR.GeneralConfigurationRequests.GET_CONFIGURATION_EXCLUDE_ALIASES
				};
				GeneralConfigProcessorClass generalConfigProcessor = new GeneralConfigProcessorClass();
				GeneralConfigDO generalConfigDO = generalConfigProcessor.Get(generalConfigSR);
				if ((generalConfigDO?.GeneralConfigurationGuid ?? Guid.Empty) == Guid.Empty)
            {
					// Couldn't find a config.  Need to create a default
					// GeneralConfigProccessorClass.Get() actually gave ua an empty/default config; we just need to give it a
					// site guid and save it back
					generalConfigDO.SiteGuid = site.SiteGuid;
					generalConfigSR.Request = GeneralConfigSR.GeneralConfigurationRequests.SAVE_CONFIGURATION;
					generalConfigSR.GeneralConfigurationDO = generalConfigDO;
					generalConfigProcessor.Save(generalConfigSR);
            }

				if (type == DATA_TYPE.CONFIG)
				{
					var schedules = new SchedulesClass();
					schedules.ModifyCollection(
							security, site.SiteGuid, site.OperatingScheduleCollection, oldSite.OperatingScheduleCollection);
					schedules.ModifyCollection(
							security, site.SiteGuid, site.HolidayScheduleCollection, oldSite.HolidayScheduleCollection);

					var processVariables = new ProcessVariablesClass();
					processVariables.ModifyCollection(
							security, site.SiteGuid, site.ProcessVariableCollection, oldSite.ProcessVariableCollection);

					var applicationStrings = new ApplicationStringsClass();
					applicationStrings.ModifyCollection(security, site.SiteGuid, STRING_TYPE.SITE_CERTIFICATE, site.SiteCertificateCollection, oldSite.SiteCertificateCollection);

					// Assign/Unassign Sites
					var siteToSiteMaps = new SiteToSiteMapsClass();
					SiteToSiteMapCollectionClass existingSiteToSiteMapCollection = siteToSiteMaps.EnumerateByParentSite(
							security, site.SiteGuid);
					SiteToSiteMapCollectionClass originalSiteToSiteMapCollection = siteToSiteMaps.EnumerateByParentSite(
							security, site.SiteGuid);
					SiteToSiteMapCollectionClass newSiteToSiteMapCollection = site.SiteToSiteMapCollection;

					if (site.SiteGroup == false 
						&& security.HasRight(RIGHT.MODIFY_SITE_CLOSEOUT_TIME) )
					{
						//Write old closeout time to closeout history table
						SiteCloseoutTimeClass closeoutTime = new SiteCloseoutTimeClass();

                  closeoutTime.SiteGuid = site.SiteGuid;
                  closeoutTime.ExpirationDate = DateTimeOffset.Now;
						closeoutTime.CloseoutTime = oldSite.CloseoutTime;
                  closeoutTime.PointsChanged = false;

                        FMChannelHelper.MakeCall<ISiteCloseoutTimes>(
                                          closeout =>
                                          closeout.SetCloseoutTime(security, closeoutTime)
                                       );
                  			
					}

					// ReSharper disable once ForCanBeConvertedToForeach
					for (int newItem = 0; newItem < newSiteToSiteMapCollection.Count; newItem++)
					{
						SiteToSiteMapClass newSiteToSiteMap = newSiteToSiteMapCollection[newItem];
						int existingItem;
						for (existingItem = 0; existingItem < existingSiteToSiteMapCollection.Count; existingItem++)
						{
							SiteToSiteMapClass existingSiteToSiteMap = existingSiteToSiteMapCollection[existingItem];
							if (existingSiteToSiteMap.ChildSiteGuid == newSiteToSiteMap.ChildSiteGuid)
							{
								break;
							}
						}

						if (existingItem == existingSiteToSiteMapCollection.Count)
						{
							siteToSiteMaps.Add(security, newSiteToSiteMap);
						}
						else
						{
							existingSiteToSiteMapCollection.RemoveAt(existingItem);
						}
					}

					foreach (SiteToSiteMapClass existingSiteToSiteMap in existingSiteToSiteMapCollection)
					{
						siteToSiteMaps.Purge(security, existingSiteToSiteMap.ParentSiteGuid, existingSiteToSiteMap.ChildSiteGuid);
					}

					// Enforce the effect of the site-to-site mapping changes onto Field Level Control configurations and Record Versioning					
					var flcService = new FieldLevelConfigMapsClass();
					flcService.ProcessSiteAssignmentChange(
							security, site.SiteGuid, originalSiteToSiteMapCollection, newSiteToSiteMapCollection);

					// This can be a major piece of work so only do it when SiteGroup is cleared
					if (!site.SiteGroup && oldSite.SiteGroup)
					{
						// Force the Security Context to match the Site being modified
						// this is to allow a Site to be modified while logged into a SiteGroup
						security.SiteGuid = site.SiteGuid;
						var dependencies = new DependenciesClass(security);
						dependencies.Update(security, site);
					}
            }
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void ModifySecurity(string token, SecurityClass security)
		{
			var session = new SessionClass
			{
				Token = Guid.Parse(token),
				SiteGuid = security.SiteGuid,
				UpdatedDate = DateTimeOffset.Now,
				UpdatedBy = security.UserID,
				CSRFToken = security.CSRFToken
			};

			using (var cmd = new SqlCommand())
			{
				session.UpdateSQL(cmd);
				this.consolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid identityGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			SiteClass site = this.GetUsingGuid(security, identityGuid);
			if (site.SiteGuid == Guid.Empty)
			{
				throw new Exception("Site Not Found");
			}

			if (SiteClass.IsAdminSiteGuid(identityGuid))
			{
				throw new Exception("[Cannot Purge] " + site.ID);
			}

			// Force the Security Context to match the Site being deleted
			// this is to allow a Site to be purged while logged into a SiteGroup
			security.SiteGuid = identityGuid;


			var dependencies = new DependenciesClass(security);
			dependencies.Purge(security, site);

			// Purge Schedules
			var schedules = new SchedulesClass();
			schedules.ModifyCollection(security, identityGuid, null, site.OperatingScheduleCollection);
			schedules.ModifyCollection(security, identityGuid, null, site.HolidayScheduleCollection);

			// Purge ProcessVariables
			var processVariables = new ProcessVariablesClass();
			processVariables.ModifyCollection(security, identityGuid, null, site.ProcessVariableCollection);

			var applicationStrings = new ApplicationStringsClass();
			applicationStrings.ModifyCollection(security, site.SiteGuid, STRING_TYPE.SITE_CERTIFICATE, null, site.SiteCertificateCollection);

			// Purge SiteToSiteMaps
			var flcService = new FieldLevelConfigMapsClass();
			var siteToSiteMaps = new SiteToSiteMapsClass();
			if (site.SiteGroup
			&& site.SiteToSiteMapCollection.Count > 0)
			{
				foreach (SiteToSiteMapClass childSiteMap in site.SiteToSiteMapCollection)
				{
					siteToSiteMaps.Purge(security, identityGuid, childSiteMap.ChildSiteGuid);
				}

				// Enforce the effect of the site-to-site mapping changes onto Field Level Control configurations and Record Versioning					
				var newSiteToSiteMapCollection = new SiteToSiteMapCollectionClass();
				flcService.ProcessSiteAssignmentChange(security, identityGuid, site.SiteToSiteMapCollection, newSiteToSiteMapCollection);

			}

			var siteCollection = this.EnumerateByChildSite(security, identityGuid);
			foreach (SiteClass parentSite in siteCollection)
			{
				var oldSiteToSiteMapCollection = siteToSiteMaps.EnumerateByParentSite(security, parentSite.SiteGuid);
				siteToSiteMaps.Purge(security, parentSite.SiteGuid, identityGuid);
				if (site.SiteGroup)
				{
					var newSiteToSiteMapCollection = siteToSiteMaps.EnumerateByParentSite(security, parentSite.SiteGuid);
					flcService.ProcessSiteAssignmentChange(security, parentSite.SiteGuid, oldSiteToSiteMapCollection, newSiteToSiteMapCollection);
				}
			}

			// Every Site is Mapped to itself
			siteToSiteMaps.Purge(security, identityGuid, identityGuid);

			var entityToSiteMaps = new EntityToSiteMaps();
			var entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndSiteGuid(security, ENTITY_TYPE.ALARM_AND_EVENT, site.SiteGuid);
			if (entityToSiteMapCollection != null
			&& entityToSiteMapCollection.Count == 1)
			{
				entityToSiteMaps.Purge(security, entityToSiteMapCollection[0]);
			}

			// Purge default Accounting configuration
			GeneralConfigDO generalConfigDO = new GeneralConfigDO
			{
				SiteGuid = site.IdentityGuid
			};
			GeneralConfigSR generalConfigSR = new GeneralConfigSR
			{
				Security = security,
				SiteGuid = site.IdentityGuid,
				GeneralConfigurationDO = generalConfigDO,
				Request = GeneralConfigSR.GeneralConfigurationRequests.PURGE
			};
			GeneralConfigProcessorClass generalConfigProcessor = new GeneralConfigProcessorClass();
			generalConfigProcessor.Purge(generalConfigSR);

			//Purge closeout times
			SiteCloseoutTimes closeoutTimes = new SiteCloseoutTimes();
			closeoutTimes.PurgeBySiteGuid(security, site.SiteGuid);

			using (var cmd = new SqlCommand())
			{
				site.PurgeSQL(cmd);
				this.consolidatedDa.ExecuteQuery(security, cmd);
			}

			if (site.NoteGuid != Guid.Empty)
			{
				var notes = new Notes();
				notes.Purge(security, site.NoteGuid);
			}


			SiteCache.RemoveSite(site.SiteGuid);

			security.SiteGuid = security.LoginSiteGuid;
		}

		/// <summary>
		/// Refreshes the caches holding VIEW and MODIFY transactional security rights for current user.
		/// Transactional security rights are based on user's group and site, and on the configuration of groups in the transaction alias itself
		/// </summary>
		/// <param name="security">The security object to be populated with the names of transaction aliases the user can view and modify</param>
		public void RefreshTransactionSecurityRightsCache(ref SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			var modifyTransactionSecurityRights = new Dictionary<string, TransactionTypes>(StringComparer.InvariantCultureIgnoreCase);
			var viewTransactionSecurityRights = new Dictionary<string, TransactionTypes>(StringComparer.InvariantCultureIgnoreCase);

			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandText = "usp_TransactionAliasSelectByUserAndSite";
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = security.UserGuid;
				cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = security.SiteGuid;

				DataSet set = this.consolidatedDa.GetDataSet(cmd, security);
				DataTable table = set.Tables[0];

				while (table.Rows.Count != 0)
				{
					DataRow row = table.Rows[0];
					string aliasName = DataObject.getValue(row["AliasName"], string.Empty);
					TransactionTypes transactionType = DataObject.getValue(row["LookupTransTypeIndex"], TransactionTypes.T_Maximum);
					GroupTransactionAliasMapClass.RIGHT rightType = DataObject.getValue(row["LookupRightIndex"], GroupTransactionAliasMapClass.RIGHT.VIEW);

					// For some reason, the case insensitive comparison specified above isn't being honored;
					// until we get that figured out, just coerce alias names to all caps.
					if (rightType == GroupTransactionAliasMapClass.RIGHT.MODIFY)
					{
						// The procedure may return the same alias name multiple times
						// so make sure the alias doesn't already exist in the dictionary before adding it
						if (!modifyTransactionSecurityRights.ContainsKey(aliasName.ToUpper()))
						{
							modifyTransactionSecurityRights.Add(aliasName.ToUpper(), transactionType);
						}

						// If you have the modify right you have the view right too.
						if (!viewTransactionSecurityRights.ContainsKey(aliasName.ToUpper()))
						{
							viewTransactionSecurityRights.Add(aliasName.ToUpper(), transactionType);
						}
					}
					else if (rightType == GroupTransactionAliasMapClass.RIGHT.VIEW && !viewTransactionSecurityRights.ContainsKey(aliasName.ToUpper()))
					{
						viewTransactionSecurityRights.Add(aliasName.ToUpper(), transactionType);
					}

					table.Rows.RemoveAt(0);
				}
			}

			security.ModifyTransactionSecurityRights = modifyTransactionSecurityRights;
			security.ViewTransactionSecurityRights = viewTransactionSecurityRights;
		}

		/// <summary>
		/// Initializes the next document / invoice numbers based on the current set of transactions in the database for the specified Site Id
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="siteId">The site identifier.</param>
		/// <exception cref="System.ArgumentNullException">security</exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void InitializeNextDocumentInvoiceNumbers(SecurityClass security, string siteId)
		{
			// Make sure a valid security object is passed
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			// Update the site
			this.UpdateNextDocumentInvoiceNumbers(security, siteId);
		}

		/// <summary>
		/// Determines if Alarm and Silense are enabled based upon IsEnterprise and Site.Enterprise
		/// </summary>
		/// <param name="security">The security.</param>
		/// <returns>isAcknowledxgeAndSilenceEnabled</returns>
		public bool IsAcknowledgeAndSilenceEnabled(SecurityClass security)
		{

			var configurationSettings = new ConfigurationSettingsClass();
			string isEnterpriseString = configurationSettings.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_IsEnterprise);
			var isEnterprise = (isEnterpriseString == "1") ? true : false;
			var site = GetBasic(security, security.SiteGuid);

			bool isAcknowledxgeAndSilenceEnabled = (isEnterprise && site.Enterprise || !isEnterprise && !site.Enterprise) ? true : false;

			return isAcknowledxgeAndSilenceEnabled;
		}

		#endregion Public Methods and Operators

		public bool CachedSiteEntryExpired(SecurityClass security, SiteClass site)
		{
			bool newerVersionExists;

			using (var cmd = new SqlCommand())
			{
				site.HasDatabaseChangedSQL(cmd);
				object result = this.consolidatedDa.ExecuteScalar(cmd, security);

				newerVersionExists = ((result != null) && !DataObject.isNull(result)) && Convert.ToBoolean(result);
			}

			return newerVersionExists;
		}

		#region Explicit Interface Methods

		void IDependency.Insert(SecurityClass security, BaseDataObject Object, bool preOperation)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (Object == null)
			{
				throw new ArgumentNullException(nameof(Object));
			}
		}

		void IDependency.Purge(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (Object == null)
			{
				throw new ArgumentNullException(nameof(Object));
			}

			var entityToSiteMap = Object as EntityToSiteMapClass;
			if (entityToSiteMap != null)
			{
				var transactionAlias = new TransactionAliasClass();
				if (entityToSiteMap.TypeID == transactionAlias.EntityType)
				{
					SiteCollectionClass siteCollection = this.Enumerate(security);
					foreach (SiteClass site in siteCollection)
					{
						if (site.IdentityGuid != entityToSiteMap.SiteGuid)
						{
							continue;
						}

						if (site.InventoryTransactionAliasGuid == entityToSiteMap.IdentityGuid
							|| site.AdjustmentTransactionAliasGuid == entityToSiteMap.IdentityGuid)
						{
							SiteClass completeSite = this.GetUsingGuid(security, site.IdentityGuid);

							if (completeSite.InventoryTransactionAliasGuid == entityToSiteMap.IdentityGuid)
							{
								completeSite.InventoryTransactionAliasGuid = Guid.Empty;
								completeSite.InventoryTransactionAliasID = string.Empty;
							}
							else if (completeSite.AdjustmentTransactionAliasGuid == entityToSiteMap.IdentityGuid)
							{
								completeSite.AdjustmentTransactionAliasGuid = Guid.Empty;
								completeSite.AdjustmentTransactionAliasID = string.Empty;
							}

							this.Modify(security, DATA_TYPE.CONFIG, completeSite);
						}
					}
				}
			}
		}

		void IDependency.Update(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (Object == null)
			{
				throw new ArgumentNullException(nameof(Object));
			}
		}

		#endregion

		#region Methods

		protected void Validate(SecurityClass security, SiteClass site, DATA_TYPE? modifyDataType)
		{
			if (site.ID == string.Empty)
			{
				throw new Exception("Name Required");
			}

			// Skip individual site validation for SiteAdmin or a site group. (IGO 2009-Aug-12)
			if (site.IsAdminSite || site.SiteGroup)
			{
				return;
			}

			// If adding a new Site or modifying an existing site and we're not changing the SYNCCONFIG settings,
			// we need to execute the complete validation process.
			if (!modifyDataType.HasValue || modifyDataType.Value != DATA_TYPE.SYNCCONFIG)
			{
				// Only require this fields when the key is DESC (IGO 2009-Oct-23)
				var hardwareKey = new HardwareKeyClass();
				if (hardwareKey.IsDescKey())
				{
					if (site.Address1 == string.Empty)
					{
						throw new Exception("Address Required");
					}

					if (site.City == string.Empty)
					{
						throw new Exception("City Required");
					}

					if (site.State == string.Empty)
					{
						throw new Exception("State Required");
					}

					if (site.Phone == string.Empty)
					{
						throw new Exception("Phone Required");
					}

					if (site.EmergencyContact == string.Empty)
					{
						throw new Exception("Contact Required");
					}

					if (site.Zip == string.Empty)
					{
						throw new Exception("Zip Required");
					}
				}

				if (site.WatchdogPeriod < 1)
				{
					throw new Exception("Minimum 1 second Watchdog Period");
				}

				this.ValidateUserData(security, site);
			}
		}

		/// <summary>
		/// This method will get the SSO mode setting.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <returns>Returns true if in SSO mode, otherwise false.</returns>
		private bool IsSsoMode(SecurityClass security)
        {
			bool ssoMode = false;

			try
			{
				var configSettings = new ConfigurationSettingsClass();
				ConfigurationSettingDOClass configSetting = configSettings.GetByKey(security, ConfigurationSettingDOClass.Key_SingleSignOnMode);

				if (configSetting != null && string.IsNullOrEmpty(configSetting.SettingValue) == false && configSetting.SettingValue == "1")
				{
					ssoMode = true;
				}
			}
			catch(Exception)
            {
				return ssoMode;
            }

			return ssoMode;
		}

		private void CheckSecurity(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
				&& !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA)
				&& !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
				&& !security.HasRight(RIGHT.MODIFY_DISPATCH)
				&& !security.HasRight(RIGHT.VIEW_DISPATCH)
					&& !security.HasRight(RIGHT.VIEW_ORDERS)
					&& !security.HasRight(RIGHT.MODIFY_ORDERS)
					&& !security.HasRight(RIGHT.CREATE_ORDERS)
					&& !security.HasRight(RIGHT.VIEW_SUPPLY_ORDERS)
					&& !security.HasRight(RIGHT.MODIFY_SUPPLY_ORDERS)
					&& !security.HasRight(RIGHT.CREATE_SUPPLY_ORDERS)
					&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES)
				&& !security.HasRight(RIGHT.VIEW_MAP_CONFIGURATION)
				&& !security.HasRight(RIGHT.MODIFY_MAP_CONFIGURATION))
			{
				throw new FMInsufficientRightsException();
			}
		}

		private void CheckNumberOfSessions(SecurityClass innerSecurity, SessionClass session)
		{
			bool purgePerformed = false;

			// Get the number of sessions current user has.
			using (var cmd = new SqlCommand())
			{
				session.GetUserSessionCountSQL(cmd);

			retry:

				DataSet dataSet = this.consolidatedDa.GetDataSet(cmd, innerSecurity);

				DataTable table = dataSet.Tables[0];
				DataRow row = table.Rows[0];
				int loginCount = DataObject.getValue(row["LoginCount"], 0);

				// Raise exception if session count exceeds the limit.
				if (loginCount >= session.MaxConcurrentSessionsPerUser)
				{
					if (!purgePerformed)
					{
                        using (var purgeCmd = new SqlCommand())
						{
							session.PurgeExpiredSQL(purgeCmd);
							this.consolidatedDa.ExecuteQuery(innerSecurity, purgeCmd);
							purgePerformed = true;
						}
						goto retry;
					}

					throw new Exception(
						"The number of allowed sessions per user has exceeded its limit ("
						+ session.MaxConcurrentSessionsPerUser.ToString(CultureInfo.InvariantCulture) + ").");
				}
			}
		}

        private bool CheckNumberOfConcurrentUsers(SecurityClass innerSecurity, SessionClass session, AlarmAndEventLogsClass alarmAndEventLogs, out string message)
        {
			message = string.Empty;
            int maxConcurrentUsersPerServer = GetConfigurationIntValue(innerSecurity, ConfigurationSettingDOClass.Key_MaxConcurrentUsersPerServer, 0);
            if (maxConcurrentUsersPerServer == 0)
            {
				return true;
            }

            bool purgePerformed = false;

            // Get the number of sessions current user has.
            using (var cmd = new SqlCommand())
            {
                session.GetConcurrentUserCountSQL(cmd);

            retry:

                DataSet dataSet = this.consolidatedDa.GetDataSet(cmd, innerSecurity);

                DataTable table = dataSet.Tables[0];
                DataRow row = table.Rows[0];
                int loginCount = DataObject.getValue(row["LoginCount"], 0);

                // Raise exception if session count exceeds the limit.
                if (loginCount >= maxConcurrentUsersPerServer)
                {
                    if (!purgePerformed)
                    {
                        SessionsClass sessions = new SessionsClass();
                        sessions.CleanupExpiredUserSessions(innerSecurity);
						purgePerformed = true;
                        goto retry;
                    }

					var alamEventDescriptor = new AlarmAndEventDescriptorClass(false, BaseObjectClass.SystemKey, "Concurrent Users exceeded");
					var AlarmAndEventLog = new AlarmAndEventLogClass(alamEventDescriptor);
					AlarmAndEventLog.AssociatedData = "UserID: " + innerSecurity.UserID + ", Site ID:" + innerSecurity.SiteID;
					alarmAndEventLogs.Add(innerSecurity, AlarmAndEventLog);
					message = "Login failed, number of concurrent users exceeded";

                    return false;
                }
				return true;
            }
        }

        private void CompleteUserLogin(
			string userID,
			AlarmAndEventLogsClass alarmAndEventLogs,
			SecurityClass innerSecurity,
			UserClass user,
			SessionClass session,
			bool cacEnable)
		{
			using (var cmd = new SqlCommand())
			{
				session.InsertSQL(cmd);
				this.consolidatedDa.ExecuteQuery(innerSecurity, cmd);
			}

			innerSecurity.UserGuid = user.IdentityGuid;
			innerSecurity.Token = session.Token;

			if (cacEnable)
			{
				user.PasswordTimestamp = DateTimeOffset.Now;
			}

			user.LastLoginDate = DateTimeOffset.Now;
			user.UpdatedDate = user.LastLoginDate;
			user.UpdatedBy = innerSecurity.UserID;

			// Reset the Password lockout count back to zero since
			// the user was successfully logged in.
			user.PasswordLockoutCount = 0;

			// Update the user information.
			using (var cmd = new SqlCommand())
			{
				user.UpdateSQL(cmd);
				this.consolidatedDa.ExecuteQuery(innerSecurity, cmd);
			}

			innerSecurity.UserID = userID;
			alarmAndEventLogs.Add(innerSecurity, user.LoggedInEvent);
		}

		// ReSharper disable once UnusedMethodReturnValue.Local
		private bool GetDefaultReportDirectoryHelper(SecurityClass security, out string reportDir)
		{
			string retReportDirectory = DefaultReportDirectory;
			bool ret = false;
			using (var defaultReportDirectoryCmd = new SqlCommand())
			{
				defaultReportDirectoryCmd.CommandText = "SELECT ReportDirectory from tblSites where SiteGuid = @CurrentSite";
				defaultReportDirectoryCmd.Parameters.AddWithValue("@CurrentSite", security.SiteGuid);
				DataSet defaultReportDirectorySet = this.consolidatedDa.GetDataSet(defaultReportDirectoryCmd, security);
				if (defaultReportDirectorySet != null)
				{
					DataTable defaultReportDirectoryTable = defaultReportDirectorySet.Tables[0];
					if (defaultReportDirectoryTable.Rows.Count != 0)
					{
						DataRow defaultReportDirectoryRow = defaultReportDirectoryTable.Rows[0];
						retReportDirectory = DataObject.getValue(defaultReportDirectoryRow["ReportDirectory"], DefaultReportDirectory);
						ret = true;
					}
				}
			}

			reportDir = retReportDirectory;
			return ret;
		}

		private bool GetReportDirectoryHelper(SecurityClass security, string reportPath, out string reportDir)
		{
			string retReportDirectory = DefaultReportDirectory;
			bool ret = false;
			using (var cmd = new SqlCommand())
			{
				cmd.CommandText =
					"Exec map.usp_EntityReportConfigurationSettingsToSiteSelectReportDirectoryBySiteGuidReportName @CurrentSite, @ReportName";
				cmd.Parameters.AddWithValue("@CurrentSite", security.SiteGuid);
				cmd.Parameters.AddWithValue("@ReportName", reportPath);
				DataSet set = this.consolidatedDa.GetDataSet(cmd, security);
				if (set == null)
				{
					throw new ArgumentException("Unable to find settings for specified reportPath", nameof(reportPath));
				}

				DataTable table = set.Tables[0];
				if (table.Rows.Count != 0)
				{
					DataRow row = table.Rows[0];
					retReportDirectory = DataObject.getValue(row[0], DefaultReportDirectory);
					ret = true;
				}
			}

			reportDir = retReportDirectory;
			return ret;
		}

		/// <summary>
		/// If the notes exist, modify them. If the notes don't exist, add them.
		///	If the notes are being removed, return true to indicate that they need
		///	to be deleted
		/// </summary>
		/// <param name="security">
		/// A Security object with the user's info
		/// </param>
		/// <param name="site">
		/// the Site that contains the notes
		/// </param>
		/// <returns>
		/// true if the note needs to be deleted. False otherwise
		/// </returns>
		private bool ModifyNotes(SecurityClass security, SiteClass site)
		{
			var notes = new Notes();

			if (site.Note != null && string.IsNullOrEmpty(site.Note.Note) == false)
			{
				if (site.NoteGuid != Guid.Empty)
				{
					notes.Modify(security, site.Note);
				}
				else
				{
					site.NoteGuid = notes.Add(security, site.Note);
				}
			}
			else if (site.NoteGuid != Guid.Empty)
			{
				// we have to purge the note after updating the site to avoid violating a foreign key constraint
				// return true so we know to delete the notes later.
				return true;
			}

			return false;
		}

		public long? GetMaxSiteRowVersion(SecurityClass security)
		{
			var consolidatedDA = new ConsolidatedDAClass();
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "SELECT MAX(RowVersion) AS RowVersion FROM"
										+ " (SELECT MAX(UpdatedRowVersion) AS RowVersion FROM track.tblSites"
										+ " UNION SELECT MAX(InsertedRowVersion) AS RowVersion FROM track.tblSites"
										+ " UNION SELECT MAX(DeletedRowVersion) AS RowVersion FROM track.tblSites "
										+ " UNION SELECT MAX( _RowVersion ) AS RowVersion FROM dbo.tblSites) RowVersions";
				set = consolidatedDA.GetDataSet(cmd, security);
			}

			if (set == null || set.Tables.Count == 0 || set.Tables[0].Rows.Count == 0)
			{
				return null;
			}

			DataTable table = set.Tables[0];
			DataRow row = table.Rows[0];

			return row.IsNull("RowVersion") ? null : (long?)BaseDataObject.RowVersionToInt64(row["RowVersion"] as byte[]);
		}


		public Dictionary<Guid, string> EnumerateTimeZonesForSiteGuidList(SecurityClass security, List<Guid> siteGuidList)
		{
			var timeZoneDictionary = new Dictionary<Guid, string>();

			var consolidatedDA = new ConsolidatedDAClass();
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "SELECT SiteGuid, TimeZone FROM [dbo].[tblSites] WHERE SiteGuid IN (SELECT SiteGuid FROM @SiteGuidList)";

				using (var parameterTable = new DataTable())
				{
					parameterTable.Columns.Add("SiteGuid", typeof(Guid));

					foreach (var siteGuid in siteGuidList)
					{
						parameterTable.Rows.Add(new object[] { siteGuid });
					}

					var pList = new SqlParameter("@SiteGuidList", SqlDbType.Structured);
					pList.TypeName = "dbo.GuidListType";
					pList.Value = parameterTable;
					cmd.Parameters.Add(pList);

					set = consolidatedDA.GetDataSet(cmd, security);
				}

				if (set == null
				|| set.Tables.Count == 0
				|| set.Tables[0].Rows.Count == 0)
				{
					return null;
				}

				DataTable table = set.Tables[0];

				foreach (DataRow row in table.Rows)
				{
					timeZoneDictionary.Add((Guid) row["SiteGuid"], row.IsNull("TimeZone") ? "Eastern Standard Time" : row["TimeZone"] as String);
				}
			}

			return timeZoneDictionary;
		}



		public bool ApplyGlobalRecordVersionUpdates(SecurityClass security)
		{
			ERVProcessSettingsClass ervProcessSettings;

			//This should run only an enterprise system.We can't use
			//IsEnterpriseKey though, because that only checks for NSPA enterprise or
			//DESC enterprise.Multisite is the best flag to key off of.
			var hardwareKeyService = new HardwareKeyClass();
			if (!hardwareKeyService.IsMultipleSiteKey())
			{
				return false;
			}

			try
			{
				using (var cmd = new SqlCommand())
				{
					cmd.CommandText = "erv.usp_GetFirstGlobalSpecificQueueRecord";
					cmd.CommandType = CommandType.StoredProcedure;
					DataTable queuedGlobalSpecificChanges = this.consolidatedDa.GetDataTable(cmd, security);
					while (queuedGlobalSpecificChanges.Rows.Count > 0)
					{						
						DataRow queuedChange = queuedGlobalSpecificChanges.Rows[0];

						cmd.Parameters.Clear();
						switch (DataObject.getValue(queuedChange["EntityTypeId"], string.Empty))
						{
							case CompanyClass.EntityTypeID:
								cmd.CommandText = "erv.usp_PropagateCompanyGlobalSpecificRecordChange";
								break;
							case EquipmentClass.ENTITY_TYPE_ID:
								cmd.CommandText = "erv.usp_PropagateEquipmentGlobalSpecificRecordChange";
								break;
							case PersonClass.ENTITY_TYPE_ID:
								cmd.CommandText = "erv.usp_PropagatePersonnelGlobalSpecificRecordChange";
								break;
							case ProductClass.ENTITY_TYPE_ID:
								cmd.CommandText = "erv.usp_PropagateProductGlobalSpecificRecordChange";
								break;
							case TransactionAliasClass.ENTITY_TYPE_ID:
								cmd.CommandText = "erv.usp_PropagateTransactionAliasGlobalSpecificRecordChange";
								break;
							default:
								cmd.CommandText = string.Empty;
								break;
						}

						if (string.IsNullOrEmpty(cmd.CommandText))
						{
							continue;
						}

						cmd.Parameters.Add("@SourceEntityGuid", SqlDbType.UniqueIdentifier);
						cmd.Parameters["@SourceEntityGuid"].Value = DataObject.getValue(queuedChange["EntityGuid"], Guid.Empty);
						this.consolidatedDa.ExecuteQuery(security, cmd);

						cmd.Parameters.Clear();
						cmd.CommandText = "erv.usp_ClearGlobalSpecificQueueRecords";
						cmd.Parameters.Add("@GSQueueGuid", SqlDbType.UniqueIdentifier);
						cmd.Parameters["@GSQueueGuid"].Value = DataObject.getValue(queuedChange["GSQueueGuid"], Guid.Empty);
						this.consolidatedDa.ExecuteQuery(security, cmd);

						ervProcessSettings = FMChannelHelper.MakeCall<IFieldLevelConfigMaps, ERVProcessSettingsClass>(x => x.GetProcessSettings(security));
						if ((ervProcessSettings == null) || (ervProcessSettings.InhibitGlobalFieldsProcessing))
						{
							return true; //Request received to inhibit Global Fields Queue processing. Abort current processing of the queue. Processing will resume from windows service once inhibit flag is cleared.
						}

						cmd.Parameters.Clear();
						cmd.CommandText = "erv.usp_GetFirstGlobalSpecificQueueRecord";
						cmd.CommandType = CommandType.StoredProcedure;
						queuedGlobalSpecificChanges = this.consolidatedDa.GetDataTable(cmd, security);
					}
				}

				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		/// <summary>
		/// Gets Movement ID.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <returns>
		/// The MovementID.
		/// </returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public string GetMovementID(SecurityClass security)
		{
			string movementID = string.Empty;

			security.ThrowIfNull("security");

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "dbo.usp_GetMovementID";
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@SiteGuid", security.SiteGuid);
				cmd.Parameters.AddWithValue("@UserID", security.UserID);
				cmd.Parameters.Add("@MovementID", SqlDbType.NVarChar, 30);
				cmd.Parameters["@MovementID"].Value = DBNull.Value;
				cmd.Parameters["@MovementID"].Direction = ParameterDirection.Output;


				this.consolidatedDa.GetDataSet(cmd, security);
				movementID = cmd.Parameters["@MovementID"].Value as string;
			}

			return movementID;
		}

		/// <summary>
		/// Updates the next document invoice numbers.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="siteId">The site identifier.</param>
		private void UpdateNextDocumentInvoiceNumbers(SecurityClass security, string siteId)
		{
			try
			{
				using (var cmd = new SqlCommand())
				{
					this.consolidatedDa.ExecuteQuery(security, new SqlCommand("DISABLE TRIGGER [dbo].[trg_insupd_tblSites_ForSync] ON [dbo].[tblSites]"));

					cmd.CommandText = "dbo.usp_so_PopulateSiteNextNumbers";
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Clear();

					cmd.Parameters.Add("@SiteId", SqlDbType.NVarChar, 30);

					cmd.Parameters["@SiteId"].Value = siteId;

					this.consolidatedDa.ExecuteQuery(security, cmd);
				}
			}
			finally
			{
				this.consolidatedDa.ExecuteQuery(security, new SqlCommand("ENABLE TRIGGER [dbo].[trg_insupd_tblSites_ForSync] ON [dbo].[tblSites]"));
			}
		}

		#endregion
	}
}