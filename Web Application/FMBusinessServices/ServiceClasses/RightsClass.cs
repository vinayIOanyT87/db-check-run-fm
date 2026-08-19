// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RightsClass.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Rights class service class definition
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Reflection;
	using System.Security;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.Interfaces;

	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;

	/// <summary>
	/// Rights class service class definition
	/// </summary>
	[SecuritySafeCritical]
	public class RightsClass : IRights
	{
		#region Constants and Fields

		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		#endregion

		#region Public Methods and Operators

		public RightCollectionClass Enumerate(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_USER_GROUPS) && !security.HasRight(RIGHT.MODIFY_USER_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			// Discover what security rights should be used
			return this.DiscoverSecurityRights(security);
		}

		public RightCollectionClass EnumerateByGroup(SecurityClass security, Guid groupGuid)
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
				 && !user.UserGroupMapCollection.Exists(map => map.GroupGuid == groupGuid))                 // A user should be able to view rights for their own groups
			{
				throw new FMInsufficientRightsException();
			}

			var right = new RightClass { SiteGuid = security.SiteGuid };

			DataSet set;
			using (var cmd = new SqlCommand())
			{
				right.EnumerateByGroupSQL(cmd, groupGuid, ContextUtil.IsInTransaction);
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			var rightCollection = new RightCollectionClass();
			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				rightCollection.Add((RIGHT)table.Rows[0][0]);
				table.Rows.RemoveAt(0);
			}

			return rightCollection;
		}

		public RightCollectionClass EnumerateByUserBySite(SecurityClass security, Guid userGuid, Guid siteGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			// User can enumerate there own rights, this is needed to allow password change. (IGO 2009-Sep-24)
			if (userGuid != security.UserGuid && !security.HasRight(RIGHT.VIEW_USERS) && !security.HasRight(RIGHT.MODIFY_USERS))
			{
				throw new FMInsufficientRightsException();
			}

			var rightCollection = new RightCollectionClass();

			var right = new RightClass();
			DataSet set;
			using (SqlCommand cmd = new SqlCommand())
			{
				right.EnumerateByUserAndSiteSQL(cmd, userGuid, siteGuid, DateTime.Today, ContextUtil.IsInTransaction);
				set = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			var table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				rightCollection.Add((RIGHT)table.Rows[0]["LookupRightIndex"]);
				table.Rows.RemoveAt(0);
			}

			return rightCollection;
		}



		#endregion

		#region Methods

		private RightCollectionClass DiscoverSecurityRights(SecurityClass security)
		{
			// Start with the core product security rights then add from discovery
			RightCollectionClass coreRights = SecurityClass.GetCoreProductSecurityRights();

			var hardwareKey = new HardwareKeyClass();
			uint options = 0;
			uint specialKeyCodes = 0;

			uint usenewkey = hardwareKey.GetUseNewLicenseFile();
			if (usenewkey == 0)
			{
				options = hardwareKey.GetOptionsCell();
				specialKeyCodes = hardwareKey.GetSpecialKeyCodes();
			}

			var configSettings = new ConfigurationSettingsClass();

			string strSecurityList = configSettings.GetKeyValueByKey(
				security, ConfigurationSettingDOClass.Key_ISecurityAssemblies);

			if (string.IsNullOrEmpty(strSecurityList))
			{
				return coreRights;
			}

			RightCollectionClass combinedRights = new RightCollectionClass();
			combinedRights.AddRange(coreRights);

			char[] separator = { ';' };
			string[] securityAssemList = strSecurityList.Split(separator, StringSplitOptions.RemoveEmptyEntries);

			string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
			foreach (string assemblyName in securityAssemList)
			{
				Assembly dll = null;
				if (!AssemblyDictionary.ContainsKey(assemblyName.ToLower()))
				{
					try
					{
						dll = Assembly.LoadFrom(baseDirectory + "\\bin\\" + assemblyName);
					}
					catch
					{
						try
						{
							dll = Assembly.Load(assemblyName);
						}
						catch
						{
							// don't treat failure to load these assemblies as critical.
							// log an error in the event log and allow the processing of
							// ISecurityDiscovery to continue
							FMEventLog eventLog = new FMEventLog();
							eventLog.WriteEntry($"Unable to load assembly : {assemblyName}", FMEventLogEntryType.Warning);
						}
					}

					if(dll != null)
					{
						AssemblyDictionary.Add(assemblyName.ToLower(), dll);
					}
				}
				else
				{
					dll = AssemblyDictionary.Get(assemblyName.ToLower());
				}

				// if we were unable to load the current assembly, continue to the next.
				// do not treat as a critical error
				if (dll == null)
				{
					continue;
				}

				try
				{
					Type[] types = dll.GetTypes();

					foreach (Type module in types)
					{
						Type securityInterface = module.GetInterface("ISecurityDiscovery");

						if (securityInterface != null)
						{
							object engine = Activator.CreateInstance(module);
							var discovery = (ISecurityDiscovery)engine;

							combinedRights.AddUnique(discovery.GetSecurityRights(security, options, specialKeyCodes));
						}
					}
				}
				catch { }
			}

			return combinedRights;
		}

		#endregion
	}
}