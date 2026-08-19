namespace FMBusinessServices.DataAccessLayer
{
	using System;
	using System.Data;
	using System.Data.SqlClient;

	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	internal static class SessionDAO
	{
		private const string SelectClause = "SELECT *," +
														"(SELECT UserID 			FROM tblUsers WITH (NOLOCK) WHERE tblUsers.UserGuid = tblSessions.UserGuid)		 AS UserID," +
														"(SELECT ID 				FROM tblSites WITH (NOLOCK) WHERE tblSites.SiteGuid = tblSessions.SiteGuid)		 AS SiteID," +
														"(SELECT ID 				FROM tblSites WITH (NOLOCK) WHERE tblSites.SiteGuid = tblSessions.LoginSiteGuid) AS LoginSiteID," +
														"(SELECT EnableAuditLogging	FROM tblSites WITH (NOLOCK) WHERE tblSites.SiteGuid = tblSessions.SiteGuid)		 AS AuditLoggingEnabled ";

		/// <summary>
		/// The number of seconds that must elapse since the last update to tblSessions to indicate the time the user was last active
		/// before we update tblSessions again. This is to prevent repeat modifies of tblSessions
		/// </summary>
		private const int PingSessionUpdateThrottleSeconds = 30;

		internal static void LoadObject( this SessionClass session, DataSet set )
		{
			if (set == null)
			{
				throw new ArgumentNullException( "set" );
			}

			session.Reset();

			DataTable table = set.Tables[0];
			if (table.Rows.Count == 0)
			{
				session.Token = Guid.Empty;
				return;
			}

			DataRow row = table.Rows[0];

			session.UserGuid = DataObject.getValue(row, "UserGuid", Guid.Empty);
			session.SiteGuid = DataObject.getValue(row, "SiteGuid", Guid.Empty);
			session.LoginSiteGuid = DataObject.getValue(row, "LoginSiteGuid", Guid.Empty);
			session.CreatedDate = DataObject.getValue(row, "CreatedDate", DateTimeOffset.Now);
			session.CreatedBy = DataObject.getValue(row, "CreatedBy", BaseDataObject.ADMIN);
			session.UpdatedDate = DataObject.getValue(row, "UpdatedDate", session.CreatedDate);
			session.UpdatedBy = DataObject.getValue(row, "UpdatedBy", BaseDataObject.ADMIN);
			session.Timeout = DataObject.getValue(row, "Timeout", -1);
			session.Token = DataObject.getGuid( row, "SessionGuid");
			session.SynchronizationNodeGuid = DataObject.getValue<Guid?>( row, "SynchronizationNodeGuid", null );
			session.CSRFToken = DataObject.getValue( row, "CSRFToken", string.Empty );
			session.WebServerName = DataObject.getValue( row, "WebServerName", string.Empty );
			session.WebServerIpAddress = DataObject.getValue( row, "WebServerIpAddress", string.Empty );
			session.ClientIpAddress = DataObject.getValue( row, "ClientIpAddress", string.Empty );
			session.UserID = DataObject.getValue( row, "UserID", string.Empty );
			session.SiteID = DataObject.getValue( row, "SiteID", string.Empty );
			session.LoginSiteID = DataObject.getValue( row, "LoginSiteID", string.Empty );
		}

        internal static void LoadUserGuidOnly( this SessionClass session, DataSet set )
		{
			if (set == null)
			{
				throw new ArgumentNullException( "set" );
			}

			session.Reset();

			DataTable table = set.Tables[0];
			if (table.Rows.Count == 0)
			{
				return;
			}

			session.UserGuid = DataObject.getValue( table.Rows[0], "UserGuid", Guid.Empty );
		}

		internal static void InsertSQL( this SessionClass session, SqlCommand cmd )
		{
			cmd.CommandText = "INSERT INTO tblSessions (" +
					"UserGuid," +
					"SiteGuid," +
					"LoginSiteGuid," +
					"CreatedDate," +
					"CreatedBy," +
					"UpdatedDate," +
					"UpdatedBy," +
					"Timeout," +
					"SessionGuid," +
					"SynchronizationNodeGuid," +
					"CSRFToken," +
					"WebServerName," +
					"WebServerIpAddress," +
					"ClientIpAddress" +
                    ") VALUES (" +
					"@UserGuid," +
					"@SiteGuid," +
					"@LoginSiteGuid," +
					"@CreatedDate," +
					"@CreatedBy," +
					"@UpdatedDate," +
					"@UpdatedBy," +
					"@Timeout," +
					"@SessionGuid," +
					"@SynchronizationNodeGuid," +
					"@CSRFToken," +
					"@WebServerName," +
					"@WebServerIpAddress," +
					"@ClientIpAddress" +
              ")";

			cmd.Parameters.AddWithValue( "@UserGuid", session.UserGuid );
			cmd.Parameters.AddWithValue( "@SiteGuid", session.SiteGuid );
			cmd.Parameters.AddWithValue( "@LoginSiteGuid", session.LoginSiteGuid );
			cmd.Parameters.AddWithValue( "@CreatedDate", session.CreatedDate );
			cmd.Parameters.AddWithValue( "@CreatedBy", session.CreatedBy );
			cmd.Parameters.AddWithValue( "@UpdatedDate", session.UpdatedDate );
			cmd.Parameters.AddWithValue( "@UpdatedBy", session.UpdatedBy );
			cmd.Parameters.AddWithValue( "@Timeout", session.Timeout );
			cmd.Parameters.AddWithValue( "@SessionGuid", session.Token );
			if (session.SynchronizationNodeGuid == null)
			{
				cmd.Parameters.AddWithValue("@SynchronizationNodeGuid", DBNull.Value);
			}
			else
			{
				cmd.Parameters.AddWithValue("@SynchronizationNodeGuid", session.SynchronizationNodeGuid);
			}
			cmd.Parameters.AddWithValue( "@CSRFToken", session.CSRFToken );
			cmd.Parameters.AddWithValue( "@WebServerName", Environment.MachineName );
			cmd.Parameters.AddWithValue( "@WebServerIpAddress", session.WebServerIpAddress );
			cmd.Parameters.AddWithValue( "@ClientIpAddress", session.ClientIpAddress );
        }

        internal static void UpdateSQL( this SessionClass session, SqlCommand cmd )
		{
			cmd.CommandText = "UPDATE tblSessions " +
					"SET SiteGuid = @SiteGuid, " +
					"UpdatedDate = @UpdatedDate, " +
					"UpdatedBy = @UpdatedBy," +
					"WebServerName = @WebServerName," +
					"WebServerIpAddress = @WebServerIpAddress," +
					"ClientIpAddress = @ClientIpAddress" +
               " WHERE SessionGuid = @SessionGuid";

			cmd.Parameters.AddWithValue( "@SiteGuid", session.SiteGuid );
			cmd.Parameters.AddWithValue( "@UpdatedDate", session.UpdatedDate );
			cmd.Parameters.AddWithValue( "@UpdatedBy", session.UpdatedBy );
			cmd.Parameters.AddWithValue( "@SessionGuid", session.Token );
			cmd.Parameters.AddWithValue( "@WebServerName", session.WebServerName );
			if (session.WebServerIpAddress == null)
			{
				session.WebServerIpAddress = string.Empty;
			}
			cmd.Parameters.AddWithValue( "@WebServerIpAddress", session.WebServerIpAddress );
			if(session.ClientIpAddress == null)
			{
				session.ClientIpAddress = string.Empty;
			}
			cmd.Parameters.AddWithValue( "@ClientIpAddress", session.ClientIpAddress );
      }

		internal static void PurgeSQL( this SessionClass session, SqlCommand cmd )
		{
			cmd.CommandText = "DELETE FROM dbo.tblOperateStatistics WHERE SessionGuid = @SessionGuid"
									+ " DELETE FROM dbo.tblSessions WHERE SessionGuid = @SessionGuid";
			cmd.Parameters.AddWithValue( "@SessionGuid", session.Token );
		}

		internal static void PurgeByUserSQL( this SessionClass session, SqlCommand cmd )
		{
			cmd.CommandText = "DELETE FROM dbo.tblOperateStatistics WHERE SessionGuid IN (SELECT SessionGuid FROM dbo.tblSessions WHERE UserGuid = @UserGuid) "
									+ "DELETE FROM tblSessions WHERE UserGuid = @UserGuid";
			cmd.Parameters.AddWithValue( "@UserGuid", session.UserGuid );
		}

		internal static void GetUserSessionCountSQL( this SessionClass session, SqlCommand cmd )
		{
			cmd.CommandText = "SELECT COUNT(*) AS LoginCount FROM dbo.tblSessions WHERE TimeOut > 0 AND UserGuid = @UserGuid";
			cmd.Parameters.AddWithValue( "@UserGuid", session.UserGuid );
		}

		internal static void GetConcurrentUserCountSQL(this SessionClass session, SqlCommand cmd)
		{
				// We are comparing UpdatedDate to Configuration setting ConcurrentUsersTimeOut (10 mins) to see which sessions are active.
				// UpdatedDate should be updated every second on alarm notification check if session is active.
				// Actual session time out in table can be zero or very long leading to abandoned session being retained for 24 hours
				// Using the value from table could lead to all users being locked out for 24 hours.
				cmd.CommandText = "SELECT COUNT(*) AS LoginCount FROM dbo.tblSessions " +
					 "WHERE DATEADD(minute, (SELECT CONVERT(int, SettingValue) FROM tblConfigurationSetting WHERE settingkey = 'ConcurrentUsersTimeOut'), UpdatedDate) > GETUTCDATE() " +
					 "AND SynchronizationNodeGuid IS NULL " +
					 "AND UserGuid <> '00000000-0000-0000-0000-000000000002' " + // Exclude Administrator user and services running as it.
					 "AND WebServerName = @WebServerName ";
				cmd.Parameters.AddWithValue("@WebServerName", session.WebServerName);
		}

		internal static void PurgeExpiredSQL( this SessionClass session, SqlCommand cmd )
		{
			_ = session;
			cmd.CommandText = "dbo.usp_SessionsDeleteExpired";
			cmd.CommandType = CommandType.StoredProcedure;
		}

		internal static void SelectSQL( this SessionClass session, SqlCommand cmd, bool bInTransaction )
		{
			cmd.CommandText = SelectClause +
					" FROM tblSessions " + BaseDAO.SQLUpdateLock( bInTransaction ) + " WHERE SessionGuid = @SessionGuid";

			cmd.Parameters.AddWithValue( "@SessionGuid", session.Token );
		}

		internal static void PingSelectSQL( this SessionClass session, SqlCommand cmd, bool bInTransaction )
		{
			cmd.CommandText = "SELECT * FROM tblSessions " + BaseDAO.SQLUpdateLock( bInTransaction ) + " WHERE SessionGuid = @SessionGuid";
			cmd.Parameters.AddWithValue( "@SessionGuid", session.Token );
		}

		internal static void EnumerateSQL( this SessionClass session, SqlCommand cmd )
		{
			_ = session;
			cmd.CommandText = SelectClause + " FROM tblSessions";
		}

		internal static void EnumerateSQLWithOrder(this SessionClass session, SqlCommand cmd, string orderBy)
		{
			_ = session;
			cmd.CommandText = SelectClause + " FROM tblSessions ORDER BY " + orderBy;
		}

		internal static void GetDistinctUserSessions(this SessionClass session, SqlCommand cmd)
		{
			_ = session;
			cmd.CommandText = "Select distinct(UserGuid) from tblSessions";
		}

		internal static void PurgeAllHostMachineAccountingWebSessionsSQL( this SessionClass session, SqlCommand cmd )
		{
			cmd.CommandText = "DELETE FROM tblSessions WHERE MachineID = @WebServerName";
			cmd.Parameters.AddWithValue( "@WebServerName", session.WebServerName );
		}

		internal static void EnumerateExpiredSessionsSQL(this SessionClass session, SqlCommand cmd)
		{
			_ = session;
			cmd.CommandText = "DECLARE @Now DATETIMEOFFSET = SYSDATETIMEOFFSET() " + SelectClause + " FROM dbo.tblSessions "
									+ " WHERE ([Timeout] > 0 AND DATEDIFF(mi, UpdatedDate, @Now) > [Timeout]) "
									+ " OR ([Timeout] < 0 AND DATEDIFF(mi, UpdatedDate, @Now) > 1440) ";
		}

		internal static void GetUserSessionsList(this SessionClass session, SqlCommand cmd, SecurityClass security)
		{
			_ = session;
			var siteAdminExcludeParentTreeFlag = security.SiteGuid.Equals(Guids.SiteAdminGuid) ? 0 : 1;

			cmd.CommandText = SelectClause + " FROM tblSessions ";
			cmd.CommandText += "WHERE tblSessions.SiteGuid IN (SELECT SiteGuid FROM dbo.udf_GetSiteToSiteHierarchyListForSiteGuid(@SiteGuid, 0, 0, @SiteAdminExcludeParentTreeFlag, 0, 0, 0)) ";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = security.SiteGuid;
			cmd.Parameters.Add("@SiteAdminExcludeParentTreeFlag", SqlDbType.Int).Value = siteAdminExcludeParentTreeFlag;
		}

		internal static void GetUserSessionsListWithOrder(this SessionClass session, SqlCommand cmd, SecurityClass security, string orderBy)
		{
			_ = session;
			var siteAdminExcludeParentTreeFlag = security.SiteGuid.Equals(Guids.SiteAdminGuid) ? 0 : 1;

			cmd.CommandText = SelectClause + " FROM tblSessions ";
			cmd.CommandText += "WHERE tblSessions.SiteGuid IN (SELECT SiteGuid FROM dbo.udf_GetSiteToSiteHierarchyListForSiteGuid(@SiteGuid, 0, 0, @SiteAdminExcludeParentTreeFlag, 0, 0, 0)) ";
			cmd.CommandText += "ORDER BY " + orderBy;

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = security.SiteGuid;
			cmd.Parameters.Add("@SiteAdminExcludeParentTreeFlag", SqlDbType.Int).Value = siteAdminExcludeParentTreeFlag;
		}

         
		internal static void Add(this SessionClass session, SecurityClass security)
		{
			var consolidatedDA = new ConsolidatedDAClass();

			using ( var cmd = new SqlCommand() )
			{
				session.InsertSQL( cmd );
				consolidatedDA.ExecuteQuery( security, cmd );
			}
		}

		internal static void Modify(this SessionClass session, SecurityClass security)
		{
			var consolidatedDA = new ConsolidatedDAClass();

			using ( var cmd = new SqlCommand() )
			{
				session.UpdateSQL( cmd );
				consolidatedDA.ExecuteQuery( security, cmd );
			}
		}

		internal static void Purge(this SessionClass session, SecurityClass security)
		{
			var consolidatedDA = new ConsolidatedDAClass();

			using ( var cmd = new SqlCommand() )
			{
				session.PurgeSQL( cmd );
				consolidatedDA.ExecuteSessionCleanupQuery( security, cmd );
			}
		}

		internal static void PurgeExpired(this SessionClass session, SecurityClass security)
		{
			var consolidatedDA = new ConsolidatedDAClass();

			using ( var cmd = new SqlCommand() )
			{
				session.PurgeExpiredSQL( cmd );
				consolidatedDA.ExecuteSessionCleanupQuery( security, cmd );
			}
		}

		internal static void PurgeByUser(this SessionClass session, SecurityClass security)
		{
			var consolidatedDA = new ConsolidatedDAClass();

			using ( var cmd = new SqlCommand() )
			{
				session.PurgeByUserSQL( cmd );
				consolidatedDA.ExecuteSessionCleanupQuery( security, cmd );
			}
		}

		internal static SessionClassCollection GetDistinctUserSessions(this SessionClass session, SecurityClass security)
		{
			var consolidatedDA = new ConsolidatedDAClass();

			SessionClassCollection userSessions = new SessionClassCollection();;

			DataSet set;
			using ( var cmd = new SqlCommand() )
			{
				session.GetDistinctUserSessions( cmd );
				set = consolidatedDA.GetDataSet( cmd, security );
			}

			DataTable table = set.Tables[0];
			while ( table.Rows.Count != 0 )
			{
				var sessionClass = new SessionClass();
				sessionClass.LoadUserGuidOnly( set );
				userSessions.Add( sessionClass );
				table.Rows.RemoveAt( 0 );
			}

			return userSessions;
		}

		internal static void PingSession(this SessionClass session, SecurityClass security)
		{
			var consolidatedDA = new ConsolidatedDAClass();

			using ( var cmd = new SqlCommand() )
			{
				// Just get the basic session items we need to check for session validity.
				session.PingSelectSQL( cmd, bInTransaction:false );

				var dataSet = consolidatedDA.GetDataSet( cmd, security );

				if ( dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0 )
				{
					// Careful - we do not get UserID, SiteID, LoginSiteID, or AuditLoggingEnabled in this load. 
					// Getting those items is an expensive operation.  Trying to be quick here.
					session.LoadObject( dataSet );

					if (session.Timeout > 0 && ( DateTimeOffset.Now - session.UpdatedDate ).TotalMinutes > session.Timeout )
					{
                  throw new FMSessionInvalidException(FMSessionInvalidException.SessionTimedOutExceptionMessage);
               }

               // Only modify tblSessions.UpdatedDate with the current date and time if a specified amount of time has elapsed since the last modify.
               // The time the user was last active does not need to be incredibly precise.
               //if ( ( DateTimeOffset.Now - session.UpdatedDate ).TotalSeconds > PingSessionUpdateThrottleSeconds )
               {
						session.UpdatedDate = DateTimeOffset.Now;

						session.Modify( security );
					}
				}
				else
				{
					throw new FMSessionInvalidException();
				}
			}
		}

	}
}
