// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupDAO.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Data access routines for the GroupClass.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.DataAccessLayer
{
	using System;
	using System.Data;
	using System.Data.SqlClient;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Data access routines for the GroupClass.
	/// </summary>
	internal static class GroupDAO
	{
		#region Methods

		internal static SqlCommand EnumerateAllForGridSQL(this GroupClass group)
		{
			var cmd = new SqlCommand
			          {
				CommandText = "SET NOCOUNT ON"
									+ " SELECT GroupGuid, GroupID FROM tblGroups ORDER BY GroupID"
									+ " SELECT GroupGuid FROM map.tblGroupToRight WHERE LookupRightIndex = " + ((int)RIGHT.VIEW_OPERATE_ONLY).ToString()

			};

			return cmd;
		}

		internal static SqlCommand EnumerateBySiteSQL(
			this GroupClass group,
			SecurityClass security,
			Guid siteGuid,
			bool bInTransaction)
		{
			var cmd = new SqlCommand
			          {
				          CommandText =
							  "SELECT tblGroups.*, map.tblUserToGroup.SiteGuid 'UserSiteGuid', map.tblUserToGroup.ExpirationDate FROM tblGroups "
					          + BaseDAO.SQLUpdateLock(bInTransaction) + " INNER JOIN map.tblUserToGroup "
					          + BaseDAO.SQLUpdateLock(bInTransaction)
					          + " ON map.tblUserToGroup.GroupGuid = tblGroups.GroupGuid "
					          + " AND map.tblUserToGroup.SiteGuid = @SiteGuid" + " ORDER BY GroupID"
			          };

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = siteGuid;

			return cmd;
		}

		internal static SqlCommand EnumerateByUserByGroupSQL(
			this GroupClass group,
			SecurityClass security,
			Guid userGuid,
			Guid groupGuid,
			bool inTransaction)
		{
			var cmd = new SqlCommand
			          {
				          CommandText =
							  "SELECT tblGroups.*, map.tblUserToGroup.ExpirationDate FROM tblGroups " + BaseDAO.SQLUpdateLock(inTransaction)
					          + " INNER JOIN map.tblUserToGroup " + BaseDAO.SQLUpdateLock(inTransaction)
					          + " ON map.tblUserToGroup.GroupGuid = tblGroups.GroupGuid "
					          + " AND map.tblUserToGroup.UserGuid = @UserGuid"
					          + " AND map.tblUserToGroup.GroupGuid = @GroupGuid"
					          + " AND map.tblUserToGroup.SiteGuid = @SiteGuid" + " ORDER BY GroupID"
			          };

			cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = userGuid;
			cmd.Parameters.Add("@GroupGuid", SqlDbType.UniqueIdentifier).Value = groupGuid;
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = security.SiteGuid;

			return cmd;
		}

		internal static SqlCommand EnumerateByUserBySiteSQL(
			this GroupClass group,
			SecurityClass security,
			Guid userGuid,
			Guid siteGuid,
			bool inTransaction)
		{
			var cmd = new SqlCommand();

			cmd.CommandText = "SELECT tblGroups.*, map.tblUserToGroup.ExpirationDate FROM tblGroups " + BaseDAO.SQLUpdateLock(inTransaction)
			                  + " INNER JOIN map.tblUserToGroup " + BaseDAO.SQLUpdateLock(inTransaction)
			                  + " ON map.tblUserToGroup.GroupGuid = tblGroups.GroupGuid "
			                  + " AND map.tblUserToGroup.UserGuid = @UserGuid" + " WHERE"
			                  + group.AppendSiteWhereClause(cmd, security, "tblGroups", "GroupGuid") + " ORDER BY GroupID";

			cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = userGuid;
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = siteGuid;

			return cmd;
		}

		internal static SqlCommand EnumerateByUserSQL(
			this GroupClass group,
			SecurityClass security,
			Guid userGuid,
			bool inTransaction)
		{
			var cmd = new SqlCommand();

			cmd.CommandText = "SELECT tblGroups.*, map.tblUserToGroup.SiteGuid 'UserSiteGuid', map.tblUserToGroup.ExpirationDate" + " FROM tblGroups "
			                  + BaseDAO.SQLUpdateLock(inTransaction) + " INNER JOIN map.tblUserToGroup "
			                  + BaseDAO.SQLUpdateLock(inTransaction) + " ON map.tblUserToGroup.GroupGuid = tblGroups.GroupGuid"
			                  + " AND map.tblUserToGroup.UserGuid = @UserGuid " + " WHERE"
			                  + group.AppendSiteWhereClause(cmd, security, "tblGroups", "GroupGuid") + " ORDER BY GroupID";

			cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = userGuid;

			return cmd;
		}

		internal static SqlCommand EnumerateSQLCmd(this GroupClass group, SecurityClass security)
		{
			var cmd = new SqlCommand();

			cmd.CommandText = "SET NOCOUNT ON"
									+ " SELECT * FROM tblGroups" + " WHERE" + group.AppendSiteWhereClause(cmd, security, "tblGroups", "GroupGuid") + " ORDER BY GroupID"
									+ " SELECT GroupGuid FROM map.tblGroupToRight WHERE LookupRightIndex = " + ((int)RIGHT.VIEW_OPERATE_ONLY).ToString();


			return cmd;
		}

		/// <summary>
		/// Get a SQL Command to detect whether the specified user is a member of the specified group in any site.
		/// 1 is returned by the SQL Command if the user is a member of the group, otherwise, 0 is returned.
		/// This is used by login to detect whether the user should be locked out when their password expires
		/// </summary>
		/// <param name="userGuid">The user</param>
		/// <param name="groupGuid">The group</param>
		internal static SqlCommand IsUserMemberOfGroupForAnySiteSQL(Guid userGuid, Guid groupGuid)
		{
			var command = new SqlCommand
			              {
				              CommandText =
					              "IF EXISTS (SELECT * FROM map.tblUserToGroup utg "
								  + "WHERE utg.GroupGuid = @GroupGuid AND utg.UserGuid = @UserGuid) "
					              + "BEGIN SELECT 1 END ELSE BEGIN SELECT 0 END "
			              };

			command.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = userGuid;
			command.Parameters.Add("@GroupGuid", SqlDbType.UniqueIdentifier).Value = groupGuid;

			return command;
		}

		/// <summary>
		/// Return an SQL command to retrieve groups by user and site.
		/// </summary>
		/// <param name="group">The current group class object.</param>
		/// <param name="security">The security object.</param>
		/// <param name="userGuid">The user's GUID.</param>
		/// <param name="siteGuid">The site's GUID.</param>
		/// <returns>Returns an SQL Command object.</returns>
		internal static SqlCommand EnumerateByUserBySitesSQL(this GroupClass group, SecurityClass security, Guid userGuid, Guid siteGuid)
		{
			var command = new SqlCommand();
			const string Select = "SELECT tblGroups.*, utg.SiteGuid 'UserSiteGuid', utg.ExpirationDate ";

			const string From = "FROM tblGroups "
			                     + " INNER JOIN map.tblUserToGroup utg "
								 + " ON utg.GroupGuid = tblGroups.GroupGuid AND utg.UserGuid = @UserGuid "
								 + " AND (utg.SiteGuid = @SiteGuid "
								 + " OR utg.SiteGuid = @LoginSiteGuid )";

			string where = " WHERE " + group.AppendSiteWhereClause(command, security, "tblGroups", "GroupGuid");

			const string OrderBy = " ORDER BY GroupID";

			command.CommandText = Select + From + where + OrderBy;
			command.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = userGuid;
			command.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = siteGuid;
			command.Parameters.Add("@LoginSiteGuid", SqlDbType.UniqueIdentifier).Value = security.LoginSiteGuid;

			return command;
		}


		/// <summary>
		///     Gets the insert SQL command object.
		/// </summary>
		internal static void InsertSQLCmd(this GroupClass group, SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblGroups " + "(GroupID," + "GroupDescription," + "SiteGuid," + "CreatedDate,"
			                  + "CreatedBy," + "UpdatedDate," + "UpdatedBy," + "GroupGuid," + "SessionTimeout," + "ActiveDirectoryUserGroupGuid) VALUES ("
                              + DataObject.AddParameter(cmd, string.Empty, "@ID", group.ID)
			                  + DataObject.AddParameter(cmd, ",", "@Description", group.Description)
			                  + DataObject.AddParameter(cmd, ",", "@SiteGuid", group.SiteGuid)
                              + DataObject.AddParameter(cmd, ",", "@CreatedDate", group.CreatedDate)
			                  + DataObject.AddParameter(cmd, ",", "@CreatedBy", group.CreatedBy)
			                  + DataObject.AddParameter(cmd, ",", "@UpdatedDate", group.UpdatedDate)
			                  + DataObject.AddParameter(cmd, ",", "@UpdatedBy", group.UpdatedBy)
			                  + DataObject.AddParameter(cmd, ",", "@GroupGuid", group.IdentityGuid)
							  + DataObject.AddParameter(cmd, ",", "@SessionTimeout", group.SessionTimeout);

		    if (group.ActiveDirectoryUserGroupGuid == Guid.Empty)
		    {

		        cmd.CommandText = cmd.CommandText + DataObject.AddParameter(cmd, ",", "@ActiveDirectoryUserGroupGuid", DBNull.Value);
		    }
		    else
		    {
                cmd.CommandText = cmd.CommandText + DataObject.AddParameter(cmd, ",", "@ActiveDirectoryUserGroupGuid", group.ActiveDirectoryUserGroupGuid);
            }

            cmd.CommandText = cmd.CommandText + ")";
        }

		/// <summary>
		///     Loads the specified set.
		/// </summary>
		internal static void LoadObject(this GroupClass group, DataSet set)
		{
			if (set == null)
			{
				throw new ArgumentNullException("set");
			}

			group.Reset();

			DataTable table = set.Tables[0];
			if (table.Rows.Count == 0)
			{
				return;
			}

			DataRow row = table.Rows[0];

			group.IdentityGuid = DataObject.getValue(row["GroupGuid"], Guid.Empty);
			group.ID = DataObject.getValue(row["GroupID"], string.Empty);
			group.SessionTimeout = DataObject.getValue(row["SessionTimeout"], 5);
			group.Description = DataObject.getValue(row["GroupDescription"], string.Empty);
			group.SiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty);
            group.ActiveDirectoryUserGroupGuid = DataObject.getValue(row["ActiveDirectoryUserGroupGuid"], Guid.Empty);

            if (table.Columns.Contains("ExpirationDate"))
			{
				group.AssignedExpirationDate = DataObject.getValue(row["ExpirationDate"], DateTime.Today.AddYears(1).Date);
			}
			group.CreatedDate = DataObject.getValue(row["CreatedDate"], DateTimeOffset.Now);
			group.CreatedBy = DataObject.getValue(row["CreatedBy"], BaseDataObject.ADMIN);
			group.UpdatedDate = DataObject.getValue(row["UpdatedDate"], group.CreatedDate);
			group.UpdatedBy = DataObject.getValue(row["UpdatedBy"], BaseDataObject.ADMIN);
		}

		internal static SqlCommand PurgeSQLCmd(this GroupClass group)
		{
			var cmd = new SqlCommand();

			cmd.CommandText = "DELETE FROM tblGroups  WHERE "
			                  + DataObject.AddParameter(cmd, "GroupGuid = ", "@GroupGuid", group.IdentityGuid);

			return cmd;
		}

		internal static SqlCommand SelectByIdsqlCmd(this GroupClass group, SecurityClass security, bool bInTransaction)
		{
			var cmd = new SqlCommand();

			cmd.CommandText = "SELECT * FROM tblGroups " + BaseDAO.SQLUpdateLock(bInTransaction) + " WHERE"
			                  + group.AppendSiteWhereClause(cmd, security, "tblGroups", "GroupGuid")
			                  + DataObject.AddParameter(cmd, true, "GroupID", "@ID", group.ID);

			return cmd;
		}

		internal static SqlCommand SelectSQLCmd(this GroupClass group, bool bInTransaction)
		{
			var cmd = new SqlCommand();

			cmd.CommandText = "SELECT * FROM tblGroups " + BaseDAO.SQLUpdateLock(bInTransaction) + " WHERE "
			                  + DataObject.AddParameter(cmd, false, "GroupGuid", "@GroupGuid", group.IdentityGuid);

			return cmd;
		}

		internal static SqlCommand UpdateSQLCmd(this GroupClass group)
		{
			var cmd = new SqlCommand();

		    cmd.CommandText = "UPDATE tblGroups " + "SET " + DataObject.AddParameter(cmd, "GroupID = ", "@ID", group.ID)
		                      + DataObject.AddParameter(cmd, ", GroupDescription = ", "@Description", group.Description)
		                      + DataObject.AddParameter(cmd, ", SessionTimeout = ", "@SessionTimeout", group.SessionTimeout)
		                      + DataObject.AddParameter(cmd, ", SiteGuid =", "@SiteGuid", group.SiteGuid)
                              + DataObject.AddParameter(cmd, ", UpdatedDate = ", "@UpdatedDate", group.UpdatedDate)
                              + DataObject.AddParameter(cmd, ", UpdatedBy = ", "@UpdatedBy", group.UpdatedBy);

		    if (group.ActiveDirectoryUserGroupGuid == Guid.Empty)
		    {
		        cmd.CommandText = cmd.CommandText
		                          + DataObject.AddParameter(cmd, ", ActiveDirectoryUserGroupGuid =", "@ActiveDirectoryUserGroupGuid", DBNull.Value);
		    }
		    else
		    {
                cmd.CommandText = cmd.CommandText
                                  + DataObject.AddParameter(cmd, ", ActiveDirectoryUserGroupGuid =", "@ActiveDirectoryUserGroupGuid", group.ActiveDirectoryUserGroupGuid);
            }

            cmd.CommandText = cmd.CommandText + " WHERE " + DataObject.AddParameter(cmd, "GroupGuid = ", "@GroupGuid", group.IdentityGuid);

			return cmd;
		}

		#endregion
	}
}