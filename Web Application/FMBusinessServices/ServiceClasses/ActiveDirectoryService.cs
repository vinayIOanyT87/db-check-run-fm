namespace FMBusinessServices.ServiceClasses
{
	using System;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessServices.DataAccessLayer;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Diagnostics;
	using System.DirectoryServices;
	using System.DirectoryServices.AccountManagement;
	using System.Linq;

	public class ActiveDirectoryService : IActiveDirectoryService
	{
		private readonly ConsolidatedDAClass consolidatedDataObject;

		public ActiveDirectoryService()
		{
			this.consolidatedDataObject = new ConsolidatedDAClass();
		}

		public bool ConfirmUser(string domainUserName)
		{
			var principalContext = this.GetPrincipalContext(domainUserName);
			if (principalContext == null)
			{
				return false;
			}

			var user = UserPrincipal.FindByIdentity(principalContext, domainUserName);
			if (user == null)
			{
				return false;
			}

			return true;
		}

		public bool AuthenticateUser(string domainUserName, string password)
		{
			bool isValid = false;
			var principalContext = this.GetPrincipalContext(domainUserName);

			var userName = domainUserName.Substring(domainUserName.IndexOf("\\", StringComparison.Ordinal) + 1);

			if (principalContext != null)
			{
				isValid = principalContext.ValidateCredentials(userName, password);
			}

			return isValid;
		}


		Dictionary<Guid, ActiveDirectorySecurityGroup> GetSecurityGroupsDictionary(string domainName, string organizationalUnitPath)
		{
			var principalContext = this.GetPrincipalContext(domainName, organizationalUnitPath);
			var findAllGroups = new GroupPrincipal(principalContext);
			PrincipalSearcher principalSearch = new PrincipalSearcher(findAllGroups);

			Dictionary<Guid, ActiveDirectorySecurityGroup> securityGroupsDictionary = new Dictionary<Guid, ActiveDirectorySecurityGroup>();
			foreach (var principal in principalSearch.FindAll())
			{
				var securityGroup = (GroupPrincipal)principal;
				if (securityGroup.IsSecurityGroup == true)
				{
					var activeDirectorySecurityGroup = new ActiveDirectorySecurityGroup
					{
						GUID = securityGroup.Guid ?? default(Guid),
						Name = securityGroup.Name,
						SID = securityGroup.Sid.Value
					};

					securityGroupsDictionary.Add(activeDirectorySecurityGroup.GUID, activeDirectorySecurityGroup);  //temp
				}

				if (securityGroupsDictionary.Count > 100) break; // temp
			}

			return securityGroupsDictionary;
		}

		public void RefreshSites(SecurityClass security)
		{
			var configSettings = new ConfigurationSettingsClass();

			var domainName = configSettings.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_ActiveDirectoryDomainName);
			var organizationalUnitPath = configSettings.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_ActiveDirectorySitesOrganizationalUnitPath);
			var activeDirectorySecurityGroups = this.GetSecurityGroupsDictionary(domainName, organizationalUnitPath);
			var fuelsManagerSecurityGroups = this.GetFuelsManagerSecurityGroupSites(security);

			var keysToAdd = activeDirectorySecurityGroups.Keys.Except(fuelsManagerSecurityGroups.Keys);
			var securityGroupsToAdd = keysToAdd.Select(id => activeDirectorySecurityGroups[id]);

			foreach (var securityGroup in securityGroupsToAdd)
			{
				var activeDirectorySite = new ActiveDirectorySiteGroup
				{
					ActiveDirectorySiteGroupGuid = securityGroup.GUID,
					Name = securityGroup.Name,
					Ssid = securityGroup.SID,
					CreatedBy = "ActiveDirectoryWindowsService",
					CreatedDate = DateTimeOffset.Now,
					UpdatedBy = "ActiveDirectoryWindowsService",
					UpdatedDate = DateTimeOffset.Now
				};

				using (var cmd = new SqlCommand())
				{
					activeDirectorySite.InsertSQL(cmd);
					this.consolidatedDataObject.ExecuteQuery(security, cmd);
				}
			}

			var keysToRemove = fuelsManagerSecurityGroups.Keys.Except(activeDirectorySecurityGroups.Keys);
			var securityGroupsToRemove = keysToRemove.Select(id => fuelsManagerSecurityGroups[id]);

			foreach (var securityGroup in securityGroupsToRemove)
			{
				var activeDirectorySite = new ActiveDirectorySiteGroup
				{
					ActiveDirectorySiteGroupGuid = securityGroup.GUID
				};

				using (var cmd = new SqlCommand())
				{
					activeDirectorySite.DeleteSQL(cmd);
					this.consolidatedDataObject.ExecuteQuery(security, cmd);
				}

			}

			var keysToNotUpdate = keysToAdd.Concat(keysToRemove);
			var keysToUpdate = activeDirectorySecurityGroups.Keys.Except(keysToNotUpdate);
			var activeDirectorySecurityGroupsToUpdate = keysToUpdate.Select(id => activeDirectorySecurityGroups[id]);
			var fuelsManagerSecurityGroupsToUpdate = keysToUpdate.Select(id => fuelsManagerSecurityGroups[id]);

			foreach (var securityGroup in activeDirectorySecurityGroupsToUpdate)
			{
				foreach (var fuelsManagerSecurityGroup in fuelsManagerSecurityGroupsToUpdate)
				{
					if (securityGroup.GUID == fuelsManagerSecurityGroup.GUID && (securityGroup.Name != fuelsManagerSecurityGroup.Name || securityGroup.SID != fuelsManagerSecurityGroup.SID))
					{
						var activeDirectorySite = new ActiveDirectorySiteGroup
						{
							ActiveDirectorySiteGroupGuid = securityGroup.GUID,
							Name = securityGroup.Name,
							Ssid = securityGroup.SID,
							UpdatedBy = "ActiveDirectoryWindowsService",
							UpdatedDate = DateTimeOffset.Now
						};


						using (SqlCommand cmd = new SqlCommand())
						{
							activeDirectorySite.UpdateSQL(cmd);
							this.consolidatedDataObject.ExecuteQuery(security, cmd);
						}
					}
				}
			}
		}

		public void RefreshUserGroups(SecurityClass security)
		{
			ConfigurationSettingsClass configSettings = new ConfigurationSettingsClass();

			var domainName = configSettings.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_ActiveDirectoryDomainName);
			var organizationalUnitPath = configSettings.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_ActiveDirectoryUserGroupsOrganizationalUnitPath);
			var activeDirectorySecurityGroups = this.GetSecurityGroupsDictionary(domainName, organizationalUnitPath);
			var fuelsManagerSecurityGroups = this.GetFuelsManagerSecurityUserGroups(security);

			var keysToAdd = activeDirectorySecurityGroups.Keys.Except(fuelsManagerSecurityGroups.Keys);
			var securityGroupsToAdd = keysToAdd.Select(id => activeDirectorySecurityGroups[id]);

			foreach (var securityGroup in securityGroupsToAdd)
			{
				var activeDirectoryUserGroup = new ActiveDirectoryUserGroup
				{
					ActiveDirectoryUserGroupGuid = securityGroup.GUID,
					Name = securityGroup.Name,
					Ssid = securityGroup.SID,
					CreatedBy = "ActiveDirectoryWindowsService",
					CreatedDate = DateTimeOffset.Now,
					UpdatedBy = "ActiveDirectoryWindowsService",
					UpdatedDate = DateTimeOffset.Now
				};

				using (var cmd = new SqlCommand())
				{
					activeDirectoryUserGroup.InsertSQL(cmd);
					this.consolidatedDataObject.ExecuteQuery(security, cmd);
				}
			}

			var keysToRemove = fuelsManagerSecurityGroups.Keys.Except(activeDirectorySecurityGroups.Keys);
			var securityGroupsToRemove = keysToRemove.Select(id => fuelsManagerSecurityGroups[id]);

			foreach (var securityGroup in securityGroupsToRemove)
			{
				var activeDirectoryUserGroup = new ActiveDirectoryUserGroup
				{
					ActiveDirectoryUserGroupGuid = securityGroup.GUID
				};

				using (var cmd = new SqlCommand())
				{
					activeDirectoryUserGroup.DeleteSQL(cmd);
					this.consolidatedDataObject.ExecuteQuery(security, cmd);
				}

			}

			var keysToNotUpdate = keysToAdd.Concat(keysToRemove);
			var keysToUpdate = activeDirectorySecurityGroups.Keys.Except(keysToNotUpdate);
			var activeDirectorySecurityGroupsToUpdate = keysToUpdate.Select(id => activeDirectorySecurityGroups[id]);
			var fuelsManagerSecurityGroupsToUpdate = keysToUpdate.Select(id => fuelsManagerSecurityGroups[id]);

			foreach (var securityGroup in activeDirectorySecurityGroupsToUpdate)
			{
				foreach (var fuelsManagerSecurityGroup in fuelsManagerSecurityGroupsToUpdate)
				{
					if (securityGroup.GUID == fuelsManagerSecurityGroup.GUID
						 && (securityGroup.Name != fuelsManagerSecurityGroup.Name || securityGroup.SID != fuelsManagerSecurityGroup.SID))
					{
						var activeDirectoryUserGroup = new ActiveDirectoryUserGroup
						{
							ActiveDirectoryUserGroupGuid = securityGroup.GUID,
							Name = securityGroup.Name,
							Ssid = securityGroup.SID,
							UpdatedBy = "ActiveDirectoryWindowsService",
							UpdatedDate = DateTimeOffset.Now
						};


						using (SqlCommand cmd = new SqlCommand())
						{
							activeDirectoryUserGroup.UpdateSQL(cmd);
							this.consolidatedDataObject.ExecuteQuery(security, cmd);
						}
					}
				}
			}
		}

		public void GetGroup(string domainUserName)
		{
			var domain = domainUserName.Substring(0, domainUserName.IndexOf("\\", StringComparison.Ordinal));

			PrincipalContext ctx = new PrincipalContext(ContextType.Domain, domain, "OU=Corporate,DC=corp,DC=leidos,DC=com");

			UserPrincipal qbeUser = new UserPrincipal(ctx);

			PrincipalSearcher srch = new PrincipalSearcher(qbeUser);

			DataTable dt = new DataTable();

			dt.Columns.Add("SAMAccountName");
			dt.Columns.Add("Name");
			dt.AcceptChanges();

			var principals = new List<Principal>();

			foreach (Principal p in srch.FindAll())

			{
				principals.Add(p);

				dt.Rows.Add(p.SamAccountName, p.Name);
				dt.AcceptChanges();
			}
		}

		public void Get_All_ADUsers_In_ADGroup(ref List<ActiveDirectoryUserDTO> activeDirectoryUsers, GroupPrincipal groupPrincipal,
			 GroupPrincipal memberOfGroup, string domainName, EventLog eventLog, bool siteGroup = false)
		{
			try
			{
				var members = groupPrincipal.GetMembers();

				foreach (var member in members)
				{
					if (member.StructuralObjectClass.Equals("user"))
					{
						var activeDirectoryUser = activeDirectoryUsers.Find(adu => adu.UserName == domainName + "\\" + member.SamAccountName);
						if (activeDirectoryUser == null)
						{
							var newActiveDirectoryUser = new ActiveDirectoryUserDTO();
							newActiveDirectoryUser.UserName = domainName + "\\" + member.SamAccountName;

							if (siteGroup)
								newActiveDirectoryUser.Sites.Add(memberOfGroup.Name);
							else
								newActiveDirectoryUser.UserGroups.Add(memberOfGroup.Name);

							activeDirectoryUsers.Add(newActiveDirectoryUser);
						}
						else
						{
							if (siteGroup)
								activeDirectoryUser.Sites.Add(memberOfGroup.Name);
							else
								activeDirectoryUser.UserGroups.Add(memberOfGroup.Name);
						}
					}
					else if (member.StructuralObjectClass.Equals("group"))
					{
						Get_All_ADUsers_In_ADGroup(ref activeDirectoryUsers, (GroupPrincipal)member, memberOfGroup, domainName, eventLog, siteGroup);
					}
				}
			}
			catch (Exception ex)
			{
				LoggerServiceClass logger = new LoggerServiceClass();
				logger.Log("A.D.S.", FMBusinessObjects.LogClient.LogLevel.ERROR, ex.Message);
				eventLog.WriteEntry(ex.Message + "... while trying to get users and associations from Active Directory. " + (siteGroup ? "Site" : "User Group") + " name: " + memberOfGroup.Name);
			}
		}
		public List<ActiveDirectoryUserDTO> GetUsersAndGroupAssociations(SecurityClass security)
		{
			var eventLog = new EventLog("Application", ".", "Active Directory Service");
			var configSettings = new ConfigurationSettingsClass();
			var domainName = configSettings.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_ActiveDirectoryDomainName);
			var userGroupsOrganizationalUnitPath = configSettings.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_ActiveDirectoryUserGroupsOrganizationalUnitPath);
			var userGroupsprincipalContext = this.GetPrincipalContext(domainName, userGroupsOrganizationalUnitPath);
			var findAllUserGroups = new GroupPrincipal(userGroupsprincipalContext);
			var userGroupsPrincipalSearch = new PrincipalSearcher(findAllUserGroups);
			var activeDirectoryUsers = new List<ActiveDirectoryUserDTO>();

			foreach (var principal in userGroupsPrincipalSearch.FindAll())
			{
				var userGroup = (GroupPrincipal)principal;
				try
				{
					var users = userGroup.GetMembers();
					foreach (var user in users)
					{
						if (user.StructuralObjectClass.Equals("user"))
						{
							var activeDirectoryUser = activeDirectoryUsers.Find(adu => adu.UserName == domainName + "\\" + user.SamAccountName);
							if (activeDirectoryUser == null)
							{
								var newActiveDirectoryUser = new ActiveDirectoryUserDTO();
								newActiveDirectoryUser.UserName = domainName + "\\" + user.SamAccountName;
								newActiveDirectoryUser.UserGroups.Add(userGroup.Name);
								activeDirectoryUsers.Add(newActiveDirectoryUser);
							}
							else
							{
								activeDirectoryUser.UserGroups.Add(userGroup.Name);
							}
						}
						else if (user.StructuralObjectClass.Equals("group"))
						{
							Get_All_ADUsers_In_ADGroup(ref activeDirectoryUsers, (GroupPrincipal)user, userGroup, domainName, eventLog);
						}
					}
				}
				catch (Exception ex)
				{
					LoggerServiceClass logger = new LoggerServiceClass();
					logger.Log("A.D.S.", FMBusinessObjects.LogClient.LogLevel.ERROR, ex.Message);
					eventLog.WriteEntry(ex.Message + "... while trying to get users and associations from Active Directory. User group name: " + userGroup.Name);
				}
			}

			var siteGroupsOrganizationalUnitPath = configSettings.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_ActiveDirectorySitesOrganizationalUnitPath);
			var siteGroupsprincipalContext = this.GetPrincipalContext(domainName, siteGroupsOrganizationalUnitPath);
			var findAllSiteGroups = new GroupPrincipal(siteGroupsprincipalContext);
			var siteGroupsPrincipalSearch = new PrincipalSearcher(findAllSiteGroups);

			foreach (var principal in siteGroupsPrincipalSearch.FindAll())
			{
				var siteGroup = (GroupPrincipal)principal;
				try
				{
					var users = siteGroup.GetMembers();
					foreach (var user in users)
					{
						if (user.StructuralObjectClass.Equals("user"))
						{
							var activeDirectoryUser = activeDirectoryUsers.Find(adu => adu.UserName == domainName + "\\" + user.SamAccountName);
							if (activeDirectoryUser == null)
							{
								var newActiveDirectoryUser = new ActiveDirectoryUserDTO();
								newActiveDirectoryUser.UserName = domainName + "\\" + user.SamAccountName;
								newActiveDirectoryUser.Sites.Add(siteGroup.Name);
								activeDirectoryUsers.Add(newActiveDirectoryUser);
							}
							else
							{
								activeDirectoryUser.Sites.Add(siteGroup.Name);
							}
						}
						else if (user.StructuralObjectClass.Equals("group"))
						{
							Get_All_ADUsers_In_ADGroup(ref activeDirectoryUsers, (GroupPrincipal)user, siteGroup, domainName, eventLog, true);
						}

					}
				}
				catch (Exception ex)
				{
					LoggerServiceClass logger = new LoggerServiceClass();
					logger.Log("A.D.S.", FMBusinessObjects.LogClient.LogLevel.ERROR, ex.Message);
					eventLog.WriteEntry(ex.Message + "... while trying to get users and associations from Active Directory. Site name: " + siteGroup.Name);
				}
			}
			return activeDirectoryUsers;
		}

		public void GetOrganizationalUnits()
		{
			// connect to "RootDSE" to find default naming context
			DirectoryEntry rootDse = new DirectoryEntry("LDAP://RootDSE");

			string defaultContext = rootDse.Properties["defaultNamingContext"][0].ToString();

			// bind to default naming context - if you *know* where you want to bind to - 
			// you can just use that information right away
			DirectoryEntry domainRoot = new DirectoryEntry("LDAP://" + defaultContext);

			// set up directory searcher based on default naming context entry
			DirectorySearcher ouSearcher = new DirectorySearcher(domainRoot);

			// SearchScope: OneLevel = only immediate subordinates (top-level OUs); 
			// subtree = all OU's in the whole domain (can take **LONG** time!)
			ouSearcher.SearchScope = SearchScope.OneLevel;
			// ouSearcher.SearchScope = SearchScope.Subtree;

			// define properties to load - here I just get the "OU" attribute, the name of the OU
			ouSearcher.PropertiesToLoad.Add("ou");

			// define filter - only select organizational units
			ouSearcher.Filter = "(objectCategory=organizationalUnit)";

			// do search and iterate over results
			foreach (SearchResult deResult in ouSearcher.FindAll())
			{
				string ouName = deResult.Properties["ou"][0].ToString();
			}
		}

		PrincipalContext GetPrincipalContext(string domainUserName)
		{

			if (string.IsNullOrWhiteSpace(domainUserName))
			{
				return null;
			}

			var domainName = domainUserName.Substring(0, domainUserName.IndexOf("\\", StringComparison.Ordinal));
			if (string.IsNullOrWhiteSpace(domainName))
			{
				return null;
			}

			return new PrincipalContext(ContextType.Domain, domainName);
		}

		PrincipalContext GetPrincipalContext(string domainName, string organizationalUnitPath)
		{
			if (string.IsNullOrWhiteSpace(domainName) || string.IsNullOrWhiteSpace(organizationalUnitPath))
			{
				return null;
			}

			return new PrincipalContext(ContextType.Domain, domainName, organizationalUnitPath);
		}

		public List<ActiveDirectorySiteGroup> GetActiveDirectorySitesFromFuelsManager(SecurityClass security)
		{
			var sites = new List<ActiveDirectorySiteGroup>();

			//using (SqlCommand cmd = new SqlCommand())
			//{
			//    var activeDirectorySite = new ActiveDirectorySiteGroupClass();
			//    activeDirectorySite.EnumerateAllSql(cmd);

			//    DataSet dataSet = this.consolidatedDataObject.GetDataSet(cmd, security);

			//    if (dataSet == null || dataSet.Tables.Count < 1 || dataSet.Tables[0].Rows.Count < 1)
			//    {
			//        return Sites;
			//    }

			//    foreach (DataRow row in dataSet.Tables[0].Rows)
			//    {
			//        activeDirectorySite = new ActiveDirectorySiteGroupClass();
			//        activeDirectorySite.LoadRecord(row);

			//        sites.Add(activeDirectorySite);
			//    }
			//}

			return sites;
		}

		Dictionary<Guid, ActiveDirectorySecurityGroup> GetFuelsManagerSecurityGroupSites(SecurityClass security)
		{
			var fuelsManagerSecurityGroups = new Dictionary<Guid, ActiveDirectorySecurityGroup>();

			using (SqlCommand cmd = new SqlCommand())
			{
				var activeDirectorySite = new ActiveDirectorySiteGroup();
				activeDirectorySite.EnumerateAllSQL(cmd);

				DataSet dataSet = this.consolidatedDataObject.GetDataSet(cmd, security);

				if (dataSet == null || dataSet.Tables.Count < 1 || dataSet.Tables[0].Rows.Count < 1)
				{
					return fuelsManagerSecurityGroups;
				}

				foreach (DataRow row in dataSet.Tables[0].Rows)
				{
					activeDirectorySite = new ActiveDirectorySiteGroup();
					activeDirectorySite.LoadRecord(row);

					var securityGroup = new ActiveDirectorySecurityGroup
					{
						GUID = activeDirectorySite.ActiveDirectorySiteGroupGuid,
						Name = activeDirectorySite.Name,
						SID = activeDirectorySite.Ssid
					};


					fuelsManagerSecurityGroups.Add(activeDirectorySite.ActiveDirectorySiteGroupGuid, securityGroup);
				}
			}

			return fuelsManagerSecurityGroups;
		}

		Dictionary<Guid, ActiveDirectorySecurityGroup> GetFuelsManagerSecurityUserGroups(SecurityClass security)
		{
			var fuelsManagerSecurityGroups = new Dictionary<Guid, ActiveDirectorySecurityGroup>();

			using (SqlCommand cmd = new SqlCommand())
			{
				var activeDirectorySite = new ActiveDirectoryUserGroup();
				activeDirectorySite.EnumerateAllSQL(cmd);

				DataSet dataSet = this.consolidatedDataObject.GetDataSet(cmd, security);

				if (dataSet == null || dataSet.Tables.Count < 1 || dataSet.Tables[0].Rows.Count < 1)
				{
					return fuelsManagerSecurityGroups;
				}

				foreach (DataRow row in dataSet.Tables[0].Rows)
				{
					activeDirectorySite = new ActiveDirectoryUserGroup();
					activeDirectorySite.LoadRecord(row);

					var securityGroup = new ActiveDirectorySecurityGroup
					{
						GUID = activeDirectorySite.ActiveDirectoryUserGroupGuid,
						Name = activeDirectorySite.Name,
						SID = activeDirectorySite.Ssid
					};


					fuelsManagerSecurityGroups.Add(activeDirectorySite.ActiveDirectoryUserGroupGuid, securityGroup);
				}
			}

			return fuelsManagerSecurityGroups;
		}
	}
}