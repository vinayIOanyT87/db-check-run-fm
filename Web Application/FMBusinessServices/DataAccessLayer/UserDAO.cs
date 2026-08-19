// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserDAO.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Data access routines for UserClass
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.DataAccessLayer
{
	using System;
	using System.Data;
	using System.Data.SqlClient;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Data access routines for UserClass
	/// </summary>
	internal static class UserDAO
	{
		internal const string ArchiveSQL =
			"INSERT INTO tblArchivedUsers (UserGuid, SiteGuid, UserID, Password, LastLoginDate, LastLogoffDate, ChangePassword, PasswordTimeStamp, "
			+ "Name, EmailAddress, PhoneNumber, AccountExpirationDate, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, PasswordHistory1, PasswordHistory2, PasswordHistory3, "
			+ "PasswordHistory4, PasswordHistory5, PasswordHistory6, PasswordHistory7, PasswordHistory8, PasswordHistory9,PasswordHistory10, "
			+ "PasswordHistory11, PasswordHistory12, PasswordHistory13, PasswordHistory14, PasswordHistory15, PasswordHistory16, PasswordHistory17, "
			+ "PasswordHistory18, PasswordHistory19, PasswordHistory20, PasswordHistory21, PasswordHistory22, PasswordHistory23, PasswordHistory24, "
			+ "PasswordLockoutCount, InactivityLockout, InactivityLockoutDate, PasswordHint, UserData1, UserData2, UserData3, UserData4, UserData5,"
			+ "UserData6, UserData7, UserData8) "
			+ "SELECT tblUsers.UserGuid, tblUsers.SiteGuid, UserID, Password, LastLoginDate, LastLogoffDate, ChangePassword, PasswordTimeStamp, "
			+ "Name, tblUsers.EmailAddress, tblUsers.PhoneNumber, tblUsers.AccountExpirationDate, tblUsers.CreatedDate, tblUsers.CreatedBy, tblUsers.UpdatedDate, tblUsers.UpdatedBy, PasswordHistory1, "
			+ "PasswordHistory2, PasswordHistory3, PasswordHistory4, PasswordHistory5, PasswordHistory6, PasswordHistory7, PasswordHistory8, "
			+ "PasswordHistory9,PasswordHistory10, PasswordHistory11, PasswordHistory12, PasswordHistory13, PasswordHistory14, PasswordHistory15, "
			+ "PasswordHistory16, PasswordHistory17, PasswordHistory18, PasswordHistory19, PasswordHistory20, PasswordHistory21, PasswordHistory22, "
			+ "PasswordHistory23, PasswordHistory24, PasswordLockoutCount, InactivityLockout, InactivityLockoutDate, PasswordHint,"
			+ "tblUsers.UserData1, tblUsers.UserData2, tblUsers.UserData3, tblUsers.UserData4, tblUsers.UserData5, tblUsers.UserData6, tblUsers.UserData7, tblUsers.UserData8 ";

		internal static void AddAllPasswordParameters( this UserClass user, SqlCommand cmd )
		{
			user.Add1PwdParameter( cmd, "@PasswordHistory1", user.PasswordHistory1 );
			user.Add1PwdParameter( cmd, "@PasswordHistory2", user.PasswordHistory2 );
			user.Add1PwdParameter( cmd, "@PasswordHistory3", user.PasswordHistory3 );
			user.Add1PwdParameter( cmd, "@PasswordHistory4", user.PasswordHistory4 );
			user.Add1PwdParameter( cmd, "@PasswordHistory5", user.PasswordHistory5 );
			user.Add1PwdParameter( cmd, "@PasswordHistory6", user.PasswordHistory6 );
			user.Add1PwdParameter( cmd, "@PasswordHistory7", user.PasswordHistory7 );
			user.Add1PwdParameter( cmd, "@PasswordHistory8", user.PasswordHistory8 );
			user.Add1PwdParameter( cmd, "@PasswordHistory9", user.PasswordHistory9 );
			user.Add1PwdParameter( cmd, "@PasswordHistory10", user.PasswordHistory10 );
			user.Add1PwdParameter( cmd, "@PasswordHistory11", user.PasswordHistory11 );
			user.Add1PwdParameter( cmd, "@PasswordHistory12", user.PasswordHistory12 );
			user.Add1PwdParameter( cmd, "@PasswordHistory13", user.PasswordHistory13 );
			user.Add1PwdParameter( cmd, "@PasswordHistory14", user.PasswordHistory14 );
			user.Add1PwdParameter( cmd, "@PasswordHistory15", user.PasswordHistory15 );
			user.Add1PwdParameter( cmd, "@PasswordHistory16", user.PasswordHistory16 );
			user.Add1PwdParameter( cmd, "@PasswordHistory17", user.PasswordHistory17 );
			user.Add1PwdParameter( cmd, "@PasswordHistory18", user.PasswordHistory18 );
			user.Add1PwdParameter( cmd, "@PasswordHistory19", user.PasswordHistory19 );
			user.Add1PwdParameter( cmd, "@PasswordHistory20", user.PasswordHistory20 );
			user.Add1PwdParameter( cmd, "@PasswordHistory21", user.PasswordHistory21 );
			user.Add1PwdParameter( cmd, "@PasswordHistory22", user.PasswordHistory22 );
			user.Add1PwdParameter( cmd, "@PasswordHistory23", user.PasswordHistory23 );
			user.Add1PwdParameter( cmd, "@PasswordHistory24", user.PasswordHistory24 );
		}

		internal static void ArchiveUserSQL( this UserClass user, SqlCommand cmd )
		{
			const string SQL = ArchiveSQL + "FROM tblUsers JOIN tblSites ON tblSites.SiteGuid = tblUsers.SiteGuid "
			                   + "WHERE UserGuid = @UserGuid";

			cmd.Parameters.AddWithValue( "@UserGuid", user.IdentityGuid );
			cmd.CommandText = SQL;
		}

		internal static void Add1PwdParameter( this UserClass user, SqlCommand cmd, string paramName, string pwdHistory )
		{
			var currentParam = new SqlParameter( paramName, SqlDbType.VarBinary, 256 );
			if ( pwdHistory.Length > 0 )
			{
				currentParam.Value = UserClass.encode( pwdHistory, user.SiteGuid );
			}
			else
			{
				currentParam.Value = DBNull.Value;
			}
			cmd.Parameters.Add( currentParam );
		}

		//***************************************************************************************************************************
		// This method will return an enumerated list of users using the security and filter
		// criterion.  This method is the same as the EnumerateSql with the exception that is has a filter parameter
		// that the user populates in order to only find users that contain their search criterion.
		//***************************************************************************************************************************
		internal static void EnumerateAndFilterSQL( this UserClass user, SqlCommand cmd, SecurityClass security, string filter )
		{
			const string FromClause = " FROM tblUsers";
			string where = " WHERE " + user.AppendSiteWhereClause( cmd, security, "tblUsers", "UserGuid" );
			const string OrderBy = " ORDER BY tblUsers.UserID";

			bool hasFilter = false;
			if ( string.IsNullOrEmpty(filter) == false )
			{
				where = where + " AND (tblUsers.UserID LIKE(UPPER(@SearchFilter))" + " OR tblUsers.Name LIKE(UPPER(@SearchFilter))"
						+ " OR tblUsers.EmailAddress LIKE(UPPER(@SearchFilter)))";
				hasFilter = true;
			}

			string sql = "SELECT tblUsers.*" + FromClause + where + OrderBy;

			cmd.CommandText = sql;

			if ( hasFilter )
			{
				cmd.Parameters.Add( "@SearchFilter", SqlDbType.NVarChar, 255 );
				cmd.Parameters["@SearchFilter"].Value = "%" + filter + "%";
			}
		}

		internal static void EnumerateByGroupSQL( this UserClass user, SqlCommand cmd, Guid groupGuid, bool inTransaction )
		{
			cmd.CommandText = "SELECT tblUsers.* FROM tblUsers, map.tblUserToGroup " + BaseDAO.SQLUpdateLock( inTransaction )
							  + " WHERE map.tblUserToGroup.UserGuid = tblUsers.UserGuid "
							  + " AND map.tblUserToGroup.GroupGuid = @GroupGuid "
							  + " AND map.tblUserToGroup.SiteGuid = @SiteGuid " + " ORDER BY tblUsers.UserID";

			cmd.Parameters.AddWithValue( "@GroupGuid", groupGuid );
			cmd.Parameters.AddWithValue( "@SiteGuid", user.SiteGuid );
		}

		internal static void SelectByIdsqlParameterized( this UserClass user, SqlCommand cmd, SecurityClass security, bool bInTransaction )
		{
			string sql = "SELECT tblUsers.* FROM tblUsers" + BaseDAO.SQLUpdateLock( bInTransaction ) + " WHERE "
			             + user.AppendSiteWhereClause( cmd, security, "tblUsers", "UserGuid" ) + " AND UserID = @UID";

			cmd.CommandText = sql;
			cmd.Parameters.AddWithValue( "@UID", user.ID );
		}

		/// <summary>
		///     Returns a SQL command returning the specific count of the database Login
		///     accounts with name matching userid already in the system.
		///     Very specific, as we need to check for
		///     ids which would not normally be available to us via site-entity
		///     relationships
		/// </summary>
		/// <param name="user">The "this" object declaration for extensions.</param>
		/// <param name="cmd">The sql command object to populate.</param>
		/// <param name="security">Security context of the user checking for the user id existence</param>
		/// <param name="userId">The user for whom we are checking for existence</param>
		/// <returns>an SQL command ready to execute.</returns>
		internal static void SelectLoginAccountCount( this UserClass user, SqlCommand cmd, SecurityClass security, String userId )
		{
			const string SQL = @"SELECT Count(name) FROM sys.syslogins l WHERE name = @UID 
			   AND NOT EXISTS(SELECT * FROM tblUsers WHERE LOWER(l.name)=LOWER(UserID))";

			cmd.CommandText = SQL;
			cmd.Parameters.AddWithValue( "@UID", user.ID );
		}

		internal static void SelectSQL( this UserClass user, SqlCommand cmd, bool bInTransaction )
		{
			string sql = "SELECT tblUsers.* FROM tblUsers " 
				+ BaseDAO.SQLUpdateLock( bInTransaction ) + " WHERE UserGuid = @UserGuid";

			cmd.Parameters.AddWithValue( "@UserGuid", user.IdentityGuid );
			cmd.CommandText = sql;
		}

		/// <summary>
		///     Returns a SQL command returning the specific count of the database User
		///     accounts with name matching userid already in the system.
		///     Very specific, as we need to check for
		///     ids which would not normally be available to us via site-entity
		///     relationships
		/// </summary>
		/// <param name="user">The "this" object declaration for extensions.</param>
		/// <param name="cmd">The sql command object to populate.</param>
		/// <param name="security">Security context of the user checking for the user id existence</param>
		/// <param name="userId">The user for whom we are checking for existence</param>
		/// <returns>an SQL command ready to execute.</returns>
		internal static void SelectUserAccountCount( this UserClass user, SqlCommand cmd, SecurityClass security, String userId )
		{
			const string SQL = @"SELECT Count(name) 
			   FROM (Select * from sys.database_principals union select * from master.sys.database_principals) l 
			   WHERE name = @UID AND NOT EXISTS(SELECT * FROM tblUsers WHERE LOWER(l.name)=LOWER(UserID))";

			cmd.CommandText = SQL;
			cmd.Parameters.AddWithValue( "@UID", user.ID );
		}

		/// <summary>
		///     Returns a SQL command returning the specific count of the user id
		///     already in the system.  Very specific, as we need to check for
		///     ids which would not normally be available to us via site-entity
		///     relationships
		/// </summary>
		/// <param name="user">The "this" object declaration for extensions.</param>
		/// <param name="cmd">The sql command object to populate.</param>
		/// <param name="security">Security context of the user checking for the user id existence</param>
		/// <param name="userId">The user for whom we are checking for existence</param>
		/// <returns>an SQL command ready to execute.</returns>
		internal static void SelectUserIdCount( this UserClass user, SqlCommand cmd, SecurityClass security, String userId )
		{
			const string SQL = "SELECT Count(UserID) FROM tblUsers" + " WHERE UserID = @UID";

			cmd.CommandText = SQL;
			cmd.Parameters.AddWithValue( "@UID", user.ID );
		}

		internal static void UpdateInactivityLockoutSQL( this UserClass user, SqlCommand cmd )
		{
			cmd.CommandText =
				"UPDATE tblUsers SET InactivityLockout = 1, InactivityLockoutDate = SYSDATETIMEOFFSET(), UpdatedDate = SYSDATETIMEOFFSET(), UpdatedBy = @UpdatedBy"
				+ "WHERE UserGuid = @UserGuid";

			cmd.Parameters.Add( "@UpdatedBy", SqlDbType.NVarChar, 100 ).Value = user.UpdatedBy;
			cmd.Parameters.Add( "@UserGuid", SqlDbType.UniqueIdentifier ).Value = user.IdentityGuid;
		}

		internal static void UpdateLogoutSQL( this UserClass user, SqlCommand cmd )
		{
			const string SQL = "UPDATE tblUsers SET LastLogoffDate = SYSDATETIMEOFFSET(), UpdatedDate = SYSDATETIMEOFFSET(), UpdatedBy = @ID "
			                   + "WHERE UserID = @ID";

			cmd.CommandText = SQL;

			cmd.Parameters.Add( "@ID", SqlDbType.NVarChar, 100 );
			cmd.Parameters["@ID"].Value = user.ID;
		}

		/// <summary>
		///     This property will return the Password count column update SQL. This
		///     column is used to track the number of failure attempts.
		/// </summary>
		/// <returns></returns>
		internal static void UpdatePasswordCountSQL( this UserClass user, SqlCommand cmd )
		{
			const string SQL = "UPDATE tblUsers SET UpdatedDate = SYSDATETIMEOFFSET(), UpdatedBy = @UpdatedBy, "
			                   + "PasswordLockoutCount = @PasswordLockoutCount " + "WHERE UserGuid = @UserGuid";

			cmd.CommandText = SQL;

			cmd.Parameters.Add( "@UpdatedBy", SqlDbType.NVarChar, 100 );
			cmd.Parameters["@UpdatedBy"].Value = user.UpdatedBy;
			cmd.Parameters.AddWithValue( "@PasswordLockoutCount", user.PasswordLockoutCount );
			cmd.Parameters.AddWithValue( "@UserGuid", user.IdentityGuid );
		}

		internal static void UpdateSQL( this UserClass user, SqlCommand cmd )
		{
			const string SQL = "UPDATE tblUsers SET " + "SiteGuid = @SiteGuid," + "UserID = @UserId," + "Password = @Password,"
			                   + "LastLoginDate = @LastLoginDate," + "LastLogoffDate = @LastLogoffDate," + "ChangePassword = @ChangePassword,"
			                   + "PasswordTimestamp = @PasswordTimestamp," + "Name = @Name," + "EmailAddress = @EmailAddress,"
							   + "PhoneNumber = @PhoneNumber," + "AccountExpirationDate = @AccountExpirationDate,"
			                   + "UpdatedDate = @UpdatedDate," + "UpdatedBy = @UpdatedBy, " + "PasswordHistory1 = @PasswordHistory1, "
			                   + "PasswordHistory2 = @PasswordHistory2, " + "PasswordHistory3 = @PasswordHistory3, "
			                   + "PasswordHistory4 = @PasswordHistory4, " + "PasswordHistory5 = @PasswordHistory5, "
			                   + "PasswordHistory6 = @PasswordHistory6, " + "PasswordHistory7 = @PasswordHistory7, "
			                   + "PasswordHistory8 = @PasswordHistory8, " + "PasswordHistory9 = @PasswordHistory9, "
			                   + "PasswordHistory10 = @PasswordHistory10, " + "PasswordHistory11 = @PasswordHistory11, "
			                   + "PasswordHistory12 = @PasswordHistory12, " + "PasswordHistory13 = @PasswordHistory13, "
			                   + "PasswordHistory14 = @PasswordHistory14, " + "PasswordHistory15 = @PasswordHistory15, "
			                   + "PasswordHistory16 = @PasswordHistory16, " + "PasswordHistory17 = @PasswordHistory17, "
			                   + "PasswordHistory18 = @PasswordHistory18, " + "PasswordHistory19 = @PasswordHistory19, "
			                   + "PasswordHistory20 = @PasswordHistory20, " + "PasswordHistory21 = @PasswordHistory21, "
			                   + "PasswordHistory22 = @PasswordHistory22, " + "PasswordHistory23 = @PasswordHistory23, "
			                   + "PasswordHistory24 = @PasswordHistory24, " + "PasswordLockoutCount = @PasswordLockoutCount, "
			                   + "InactivityLockout = @InactivityLockout, " + "PasswordHint = @PasswordHint, "
							   + "UserData1 = @UserData1, " + "UserData2 = @UserData2, " + "UserData3 = @UserData3, "
							   + "UserData4 = @UserData4, " + "UserData5 = @UserData5, " + "UserData6 = @UserData6, "
							   + "UserData7 = @UserData7, " + "UserData8 = @UserData8, " + "ActiveDirectoryUser = @ActiveDirectoryUser "
                               + "WHERE UserGuid = @UserGuid";

			cmd.CommandText = SQL;
			cmd.CommandType = CommandType.Text;

			cmd.Parameters.AddWithValue( "@SiteGuid", user.SiteGuid );
			cmd.Parameters.AddWithValue( "@UserId", user.ID );
			cmd.Parameters.AddWithValue( "@Password", UserClass.encode( user.Password, user.SiteGuid ) );
			cmd.Parameters.AddWithValue( "@LastLoginDate", user.LastLoginDate );
			cmd.Parameters.AddWithValue( "@LastLogoffDate", user.LastLogoffDate );
			cmd.Parameters.AddWithValue( "@ChangePassword", user.ChangePassword );
			cmd.Parameters.AddWithValue( "@PasswordTimestamp", user.PasswordTimestamp );
			cmd.Parameters.AddWithValue( "@Name", user.Name );
			cmd.Parameters.AddWithValue( "@EmailAddress", user.EmailAddress );
			cmd.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber);
			cmd.Parameters.AddWithValue("@AccountExpirationDate", user.AccountExpirationDate);
			cmd.Parameters.AddWithValue("@UpdatedDate", user.UpdatedDate);
			cmd.Parameters.AddWithValue( "@UpdatedBy", user.UpdatedBy );
            cmd.Parameters.AddWithValue( "@ActiveDirectoryUser", (user.ActiveDirectoryUser ? 1 : 0));

            user.AddAllPasswordParameters( cmd );

			for (int i = 0; i < UserClass.UserDataCount; i++)
			{
				cmd.Parameters.AddWithValue(string.Format("@UserData{0}", i + 1), user.UserData[i]);
			}

			cmd.Parameters.AddWithValue( "@PasswordLockoutCount", user.PasswordLockoutCount );
			cmd.Parameters.AddWithValue( "@InactivityLockout", user.InactivityLockout );
			cmd.Parameters.AddWithValue( "@PasswordHint", user.PasswordHint );
			cmd.Parameters.AddWithValue( "@UserGuid", user.IdentityGuid );
		}

		/// <summary>
		///     Enumerates for parent site by assigned user SQL.
		/// </summary>
		/// <param name="user">The "this" object declaration for extensions.</param>
		/// <param name="cmd">The CMD.</param>
		/// <param name="security">The security.</param>
		/// <param name="siteGuid">The site GUID.</param>
		/// <param name="inTransaction">if set to <c>true</c> [in transaction].</param>
		/// <returns></returns>
		internal static void EnumerateForParentSiteByAssignedUserSQL(
			this UserClass user,
			SqlCommand cmd,
			SecurityClass security,
			Guid siteGuid,
			bool inTransaction )
		{
			cmd.CommandText = "SELECT DISTINCT U.* " + " FROM map.tblSiteToSite SiteMap " + BaseDAO.SQLUpdateLock( inTransaction )
							  + " INNER JOIN dbo.tblSites Sites " + BaseDAO.SQLUpdateLock( inTransaction )
							  + " ON SiteMap.ChildSiteGuid = Sites.SiteGuid " + " INNER JOIN map.tblEntityUserToSite m "
							  + BaseDAO.SQLUpdateLock( inTransaction ) + " ON Sites.SiteGuid = m.SiteGuid AND [UserGuid]= @UserGuid"
							  + " INNER JOIN map.tblEntityUserToSite ESM " + BaseDAO.SQLUpdateLock( inTransaction )
							  + " ON ESM.SiteGuid = m.SiteGuid " + " INNER JOIN tblUsers U " + BaseDAO.SQLUpdateLock( inTransaction )
							  + " ON U.UserGuid = ESM.[UserGuid] " + " WHERE SiteMap.ParentSiteGuid = @SiteGuid"
							  + " ORDER BY U.UserID";

			cmd.Parameters.AddWithValue( "@UserGuid", security.UserGuid );
			cmd.Parameters.AddWithValue( "@SiteGuid", siteGuid );
		}

		internal static void EnumerateForSiteByAssignedUserSQL(
			this UserClass user,
			SqlCommand cmd,
			SecurityClass security,
			Guid siteGuid,
			bool inTransaction )
		{
			cmd.CommandText = "SELECT DISTINCT U.* " + " FROM map.tblEntityUserToSite m " + BaseDAO.SQLUpdateLock( inTransaction )
							  + " INNER JOIN map.tblEntityUserToSite ESM " + BaseDAO.SQLUpdateLock( inTransaction )
							  + " ON ESM.SiteGuid = m.SiteGuid " + " INNER JOIN tblUsers U " + BaseDAO.SQLUpdateLock( inTransaction )
							  + " ON U.UserGuid = ESM.[UserGuid] " + " WHERE m.[UserGuid]= @UserGuid AND m.SiteGuid = @SiteGuid"
							  + " ORDER BY U.UserID";

			cmd.Parameters.AddWithValue( "@UserGuid", security.UserGuid );
			cmd.Parameters.AddWithValue( "@SiteGuid", siteGuid );
		}

		internal static void EnumerateSQL( this UserClass user, SqlCommand cmd, SecurityClass security )
		{
			string sql = "SELECT tblUsers.* FROM tblUsers" + " WHERE "
			             + user.AppendSiteWhereClause( cmd, security, "tblUsers", "UserGuid" ) + " ORDER BY tblUsers.UserID";

			cmd.CommandText = sql;
		}

        /// <summary>
        /// This method will populate the SQL command to retrieve the active directory users.
        /// It only retrieves the User ID and User GUID columns.
        /// </summary>
        /// <param name="cmd">Teh SQL command.</param>
        internal static void EnumerateActiveDirectoryUsersSQL(SqlCommand cmd)
        {
            string sql = "SELECT tblUsers.UserID, tblUsers.UserGuid FROM tblUsers"
                         + " WHERE tblUsers.ActiveDirectoryUser = 1 "
                         + " ORDER BY tblUsers.UserID";

            cmd.CommandText = sql;
        }

        internal static void InsertSQL( this UserClass user, SqlCommand cmd )
		{
			const string SQL = "INSERT INTO tblUsers " + "(SiteGuid," + "UserID," + "Password," + "LastLoginDate," + "LastLogoffDate,"
			                   + "ChangePassword," + "PasswordTimestamp," + "Name," + "EmailAddress," + "CreatedDate," + "CreatedBy,"
							   + "PhoneNumber," + "AccountExpirationDate,"
			                   + "UpdatedDate," + "UpdatedBy, " + "PasswordHistory1, " + "PasswordHistory2, " + "PasswordHistory3, "
			                   + "PasswordHistory4, " + "PasswordHistory5, " + "PasswordHistory6, " + "PasswordHistory7, "
			                   + "PasswordHistory8, " + "PasswordHistory9, " + "PasswordHistory10, " + "PasswordHistory11, "
			                   + "PasswordHistory12, " + "PasswordHistory13, " + "PasswordHistory14, " + "PasswordHistory15, "
			                   + "PasswordHistory16, " + "PasswordHistory17, " + "PasswordHistory18, " + "PasswordHistory19, "
			                   + "PasswordHistory20, " + "PasswordHistory21, " + "PasswordHistory22, " + "PasswordHistory23, "
			                   + "PasswordHistory24, " + "PasswordLockoutCount, " + "InactivityLockout, " + "PasswordHint, " + "UserGuid, "
							   + "UserData1, UserData2, UserData3, UserData4, UserData5, UserData6, UserData7, UserData8, " + "ActiveDirectoryUser "
			                   + ") " + "VALUES (" + "@SiteGuid," + "@ID," + "@Password," + "@LastLoginDate," + "@LastLogoffDate,"
			                   + "@ChangePassword," + "@PasswordTimestamp," + "@Name," + "@EmailAddress," + "@CreatedDate," + "@CreatedBy,"
							   + "@PhoneNumber," + "@AccountExpirationDate,"
			                   + "@UpdatedDate," + "@UpdatedBy," + "@PasswordHistory1, " + "@PasswordHistory2, " + "@PasswordHistory3, "
			                   + "@PasswordHistory4, " + "@PasswordHistory5, " + "@PasswordHistory6, " + "@PasswordHistory7, "
			                   + "@PasswordHistory8, " + "@PasswordHistory9, " + "@PasswordHistory10, " + "@PasswordHistory11, "
			                   + "@PasswordHistory12, " + "@PasswordHistory13, " + "@PasswordHistory14, " + "@PasswordHistory15, "
			                   + "@PasswordHistory16, " + "@PasswordHistory17, " + "@PasswordHistory18, " + "@PasswordHistory19, "
			                   + "@PasswordHistory20, " + "@PasswordHistory21, " + "@PasswordHistory22, " + "@PasswordHistory23, "
			                   + "@PasswordHistory24, " + "@PasswordLockoutCount, " + "@InactivityLockout, " + "@PasswordHint, "
			                   + "@UserGuid, " + "@UserData1, @UserData2, @UserData3, @UserData4, @UserData5, @UserData6, @UserData7, @UserData8, "
                               + "@ActiveDirectoryUser)";

			cmd.CommandText = SQL;
			cmd.CommandType = CommandType.Text;

			cmd.Parameters.AddWithValue( "@SiteGuid", user.SiteGuid );
			cmd.Parameters.AddWithValue( "@ID", user.ID );
			cmd.Parameters.AddWithValue( "@Password", UserClass.encode( user.Password, user.SiteGuid ) );
			cmd.Parameters.AddWithValue( "@LastLoginDate", user.LastLoginDate );
			cmd.Parameters.AddWithValue( "@LastLogoffDate", user.LastLogoffDate );
			cmd.Parameters.AddWithValue( "@ChangePassword", user.ChangePassword );
			cmd.Parameters.AddWithValue( "@PasswordTimestamp", user.PasswordTimestamp );
			cmd.Parameters.AddWithValue( "@Name", user.Name );
			cmd.Parameters.AddWithValue( "@EmailAddress", user.EmailAddress );
			cmd.Parameters.AddWithValue( "@PhoneNumber", user.PhoneNumber);
			cmd.Parameters.AddWithValue( "@AccountExpirationDate", user.AccountExpirationDate);
			cmd.Parameters.AddWithValue( "@CreatedDate", user.CreatedDate);
			cmd.Parameters.AddWithValue( "@CreatedBy", user.CreatedBy );
			cmd.Parameters.AddWithValue( "@UpdatedDate", user.UpdatedDate );
			cmd.Parameters.AddWithValue( "@UpdatedBy", user.UpdatedBy );
            cmd.Parameters.AddWithValue( "@ActiveDirectoryUser", (user.ActiveDirectoryUser ? 1 : 0));

            user.AddAllPasswordParameters( cmd );

			for (int i = 0; i < UserClass.UserDataCount; i++)
			{
				cmd.Parameters.AddWithValue(string.Format("@UserData{0}", i + 1), user.UserData[i]);
			}

			cmd.Parameters.AddWithValue( "@PasswordLockoutcount", user.PasswordLockoutCount );
			cmd.Parameters.AddWithValue( "@InactivityLockout", user.InactivityLockout );
			cmd.Parameters.AddWithValue( "@PasswordHint", user.PasswordHint );
			cmd.Parameters.AddWithValue( "@UserGuid", user.IdentityGuid );
		}

		/// <summary>
		///     This method loads the object with the information from the
		///     database.
		/// </summary>
		internal static void LoadObject( this UserClass user, DataSet set )
		{
			if ( set == null )
			{
				throw new ArgumentNullException( "set" );
			}

			user.Reset();

			DataTable table = set.Tables[0];
			if ( table.Rows.Count == 0 )
			{
				return;
			}

			DataRow row = table.Rows[0];

			user.IdentityGuid           = DataObject.getValue( row["UserGuid"], Guid.Empty );
			user.SiteGuid               = DataObject.getValue( row["SiteGuid"], Guid.Empty );
			user.ID                     = DataObject.getValue( row["UserID"], "" );
			user.LastLoginDate          = DataObject.getValue( row["LastLoginDate"], DateTimeOffset.Now );
			user.LastLogoffDate         = DataObject.getValue( row["LastLogoffDate"], DateTimeOffset.Now );
			user.ChangePassword         = DataObject.getValue( row["ChangePassword"], false );
			user.PasswordTimestamp      = DataObject.getValue( row["PasswordTimeStamp"], DateTimeOffset.Now );
			user.Name                   = DataObject.getValue( row["Name"], "" );
			user.EmailAddress           = DataObject.getValue( row["EmailAddress"], "" );
			user.PhoneNumber            = DataObject.getValue(row["PhoneNumber"], "");
			user.AccountExpirationDate  = DataObject.getValue(row["AccountExpirationDate"], DateTime.Today);
			user.CreatedDate            = DataObject.getValue(row["CreatedDate"], DateTimeOffset.Now);
			user.CreatedBy              = DataObject.getValue( row["CreatedBy"], BaseDataObject.ADMIN );
			user.UpdatedDate            = DataObject.getValue( row["UpdatedDate"], user.CreatedDate );
			user.UpdatedBy              = DataObject.getValue( row["UpdatedBy"], BaseDataObject.ADMIN );
            user.ActiveDirectoryUser    = DataObject.getValue(row["ActiveDirectoryUser"], false);

		    user.Password = string.Empty;
		    if (user.ActiveDirectoryUser == false)
		    {
		        user.Password = row.IsNull("Password") ? string.Empty : UserClass.decode((byte[])row["Password"], user.SiteGuid);

		        user.PasswordHistory1 = row.IsNull("PasswordHistory1") ? "" : UserClass.decode((byte[])row["PasswordHistory1"], user.SiteGuid);
		        user.PasswordHistory2 = row.IsNull("PasswordHistory2") ? "" : UserClass.decode((byte[])row["PasswordHistory2"], user.SiteGuid);
		        user.PasswordHistory3 = row.IsNull("PasswordHistory3") ? "" : UserClass.decode((byte[])row["PasswordHistory3"], user.SiteGuid);
		        user.PasswordHistory4 = row.IsNull("PasswordHistory4") ? "" : UserClass.decode((byte[])row["PasswordHistory4"], user.SiteGuid);
		        user.PasswordHistory5 = row.IsNull("PasswordHistory5") ? "" : UserClass.decode((byte[])row["PasswordHistory5"], user.SiteGuid);
		        user.PasswordHistory6 = row.IsNull("PasswordHistory6") ? "" : UserClass.decode((byte[])row["PasswordHistory6"], user.SiteGuid);
		        user.PasswordHistory7 = row.IsNull("PasswordHistory7") ? "" : UserClass.decode((byte[])row["PasswordHistory7"], user.SiteGuid);
		        user.PasswordHistory8 = row.IsNull("PasswordHistory8") ? "" : UserClass.decode((byte[])row["PasswordHistory8"], user.SiteGuid);
		        user.PasswordHistory9 = row.IsNull("PasswordHistory9") ? "" : UserClass.decode((byte[])row["PasswordHistory9"], user.SiteGuid);

		        user.PasswordHistory10 = row.IsNull("PasswordHistory10") ? "" : UserClass.decode((byte[])row["PasswordHistory10"], user.SiteGuid);
		        user.PasswordHistory11 = row.IsNull("PasswordHistory11") ? "" : UserClass.decode((byte[])row["PasswordHistory11"], user.SiteGuid);
		        user.PasswordHistory12 = row.IsNull("PasswordHistory12") ? "" : UserClass.decode((byte[])row["PasswordHistory12"], user.SiteGuid);
		        user.PasswordHistory13 = row.IsNull("PasswordHistory13") ? "" : UserClass.decode((byte[])row["PasswordHistory13"], user.SiteGuid);
		        user.PasswordHistory14 = row.IsNull("PasswordHistory14") ? "" : UserClass.decode((byte[])row["PasswordHistory14"], user.SiteGuid);
		        user.PasswordHistory15 = row.IsNull("PasswordHistory15") ? "" : UserClass.decode((byte[])row["PasswordHistory15"], user.SiteGuid);
		        user.PasswordHistory16 = row.IsNull("PasswordHistory16") ? "" : UserClass.decode((byte[])row["PasswordHistory16"], user.SiteGuid);
		        user.PasswordHistory17 = row.IsNull("PasswordHistory17") ? "" : UserClass.decode((byte[])row["PasswordHistory17"], user.SiteGuid);
		        user.PasswordHistory18 = row.IsNull("PasswordHistory18") ? "" : UserClass.decode((byte[])row["PasswordHistory18"], user.SiteGuid);
		        user.PasswordHistory19 = row.IsNull("PasswordHistory19") ? "" : UserClass.decode((byte[])row["PasswordHistory19"], user.SiteGuid);
		        user.PasswordHistory20 = row.IsNull("PasswordHistory20") ? "" : UserClass.decode((byte[])row["PasswordHistory20"], user.SiteGuid);
		        user.PasswordHistory21 = row.IsNull("PasswordHistory21") ? "" : UserClass.decode((byte[])row["PasswordHistory21"], user.SiteGuid);
		        user.PasswordHistory22 = row.IsNull("PasswordHistory22") ? "" : UserClass.decode((byte[])row["PasswordHistory22"], user.SiteGuid);
		        user.PasswordHistory23 = row.IsNull("PasswordHistory23") ? "" : UserClass.decode((byte[])row["PasswordHistory23"], user.SiteGuid);
		        user.PasswordHistory24 = row.IsNull("PasswordHistory24") ? "" : UserClass.decode((byte[])row["PasswordHistory24"], user.SiteGuid);
		    }

		    user.PasswordLockoutCount = DataObject.getValue( row["PasswordLockoutCount"], 0 );
			user.InactivityLockout = DataObject.getValue( row["InactivityLockout"], false );
			user.PasswordHint = DataObject.getValue( row["PasswordHint"], "No hint available" );

			for (int i = 0; i < UserClass.UserDataCount; i++)
			{
				user.UserData[i] = DataObject.getValue(row[string.Format("UserData{0}", (i + 1))], "");
			}

		}

		internal static void PurgeSQL( this UserClass user, SqlCommand cmd )
		{
			const string SQL = "DELETE FROM tblUsers WHERE UserGuid = @UserGuid";
			cmd.Parameters.AddWithValue( "@UserGuid", user.IdentityGuid );

			cmd.CommandText = SQL;
		}

		internal static void DisableUserSQL(SqlCommand cmd)
		{

			string SQL = "UPDATE tblUsers SET tblUsers.InactivityLockout = 1, tblUsers.InactivityLockoutDate = SYSDATETIMEOFFSET() " +
			"FROM tblSites JOIN tblUsers ON tblUsers.SiteGuid = tblSites.SiteGuid " +
			"WHERE (tblUsers.UserID <> 'Administrator') AND (tblUsers.InactivityLockout = 0 OR tblUsers.InactivityLockout IS NULL) AND " +
			"( DATEADD(d, InactivityDisablePeriod, LastLoginDate) < SYSDATETIMEOFFSET() )";
			
			

			cmd.CommandText = SQL;


		}

		private const string strSQLError = " SELECT @intErrorCode = @@ERROR IF (@intErrorCode <> 0) GOTO PROBLEM ";
		private static string DeleteUserFromTableSQL(string table, string userGuidColumnName="UserGuid")
		{
			string sql = string.Format("DELETE FROM {0} FROM {0} u JOIN @usersToArchive a ON u.{1} = a.UserGuid" + strSQLError, table, userGuidColumnName);
			return sql;

		}

		private const string DeleteUsersQueries =
			"DELETE FROM map.tblQueryStorageToGroup FROM map.tblQueryStorageToGroup u join dbo.tblQueryStorage q on u.QueryStorageGuid = q.QueryStorageGuid join @usersToArchive a ON q.OwnerUserGuid = a.UserGuid SELECT @intErrorCode = @@ERROR IF (@intErrorCode <> 0) GOTO PROBLEM ";

		internal static void ArchiveUserSQL(SqlCommand cmd)
		{
			string strSQLFrom = "FROM tblUsers JOIN @usersToArchive u ON u.UserGuid = tblUsers.UserGuid";
			string SQL = "SET XACT_ABORT ON " +
						 "DECLARE @intErrorCode INT " +
						 "BEGIN TRAN " +
						 @"DECLARE @usersToArchive TABLE (userGuid uniqueidentifier)
INSERT INTO @usersToArchive(userGuid)
SELECT tblUsers.UserGuid FROM tblUsers JOIN tblSites ON tblSites.SiteGuid = tblUsers.SiteGuid WHERE (tblUsers.UserID <> 'Administrator') 
AND (DisableArchivePeriod > 0) AND ((InactivityLockout = 1) AND (((InactivityLockoutDate IS NOT NULL) AND (DATEADD(d, DisableArchivePeriod, InactivityLockoutDate) < SYSDATETIMEOFFSET())) 
OR (((InactivityLockoutDate IS NULL) AND (DATEADD(d, DisableArchivePeriod, PasswordTimeStamp) < SYSDATETIMEOFFSET())))) )
" +
						ArchiveSQL +
						strSQLFrom +
						strSQLError +
						DeleteUsersQueries +
						DeleteUserFromTableSQL("dbo.tblQueryStorage","OwnerUserGuid") +
						DeleteUserFromTableSQL("dbo.tblDispatchGridColumn") +
						DeleteUserFromTableSQL("map.tblUserToGroup") +
						DeleteUserFromTableSQL("map.tblEntityUserToSite") +
						DeleteUserFromTableSQL("dbo.tblUsers") +
						"COMMIT TRAN " +
						"PROBLEM: IF (@intErrorCode <> 0) BEGIN ROLLBACK TRAN END ";
		
			cmd.CommandText = SQL;

		}
	}
}
