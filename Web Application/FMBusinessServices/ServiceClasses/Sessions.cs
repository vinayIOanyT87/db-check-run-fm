// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Sessions.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.ServiceModel;
	using Cassandra;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	using FMBusinessServices.DataAccessLayer;

	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class SessionsClass : ISessions
	{
		#region Fields

		/// <summary>
		/// The number of seconds that must elapse since the last update to tblSessions to indicate the time the user was last active
		/// before we update tblSessions again. This is to prevent repeat modifies of tblSessions
		/// </summary>
		private const int PingSessionUpdateThrottleSeconds = 30;

		internal ConsolidatedDAClass consolidatedDA = new ConsolidatedDAClass();

		#endregion

		#region Public Methods and Operators

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Add(SecurityClass security, SessionClass session)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (session == null)
			{
				throw new ArgumentNullException("session");
			}

			// var hardwareKey = new HardwareKeyClass();

			using (var cmd = new SqlCommand())
			{
				session.InsertSQL(cmd);

				/* Desc Security
				if (hardwareKey.IsDescKey())
				{
					consolidatedDA.ExecuteQuery(security, cmd, DBAccess.ServiceLoginAccess);
				}
				else
				{
				*/
				this.consolidatedDA.ExecuteQuery(security, cmd);

				// }
			}
		}

		/// <summary>
		/// This method retrieves a distinct list of user sessions currently logged into FuelsManager
		/// </summary>
		/// <param name="security">
		/// A SecurityClass instance
		/// </param>
		/// <returns>
		/// A collection of SessionClass instances
		/// </returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public SessionClassCollection GetDistinctUserSessions(SecurityClass security)
		{
			SessionClassCollection retVal;
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var sessionClass = new SessionClass();
			DataSet set = null;
			using (var cmd = new SqlCommand())
			{
				sessionClass.GetDistinctUserSessions(cmd);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			var sessionClassCollection = new SessionClassCollection();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				sessionClass = new SessionClass();
				sessionClass.LoadUserGuidOnly(set);
				sessionClassCollection.Add(sessionClass);
				table.Rows.RemoveAt(0);
			}

			retVal = sessionClassCollection;

			return retVal;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public SessionClassCollection EnumerateUserSessions(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var sessionClass = new SessionClass();
			DataSet set = null;
			using (var cmd = new SqlCommand())
			{
				sessionClass.EnumerateSQL(cmd);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			var sessionClassCollection = new SessionClassCollection();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				sessionClass = new SessionClass();
				sessionClass.LoadObject(set);
				sessionClassCollection.Add(sessionClass);
				table.Rows.RemoveAt(0);
			}

			return sessionClassCollection;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public SessionClassCollection EnumerateUserSessionsWithOrder(SecurityClass security, string order)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (string.IsNullOrEmpty(order))
			{
				return this.EnumerateUserSessions(security);
			}

			var sessionClass = new SessionClass();
			DataSet set = null;
			using (var cmd = new SqlCommand())
			{
				sessionClass.EnumerateSQLWithOrder(cmd, order);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			var sessionClassCollection = new SessionClassCollection();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				sessionClass = new SessionClass();
				sessionClass.LoadObject(set);
				sessionClassCollection.Add(sessionClass);
				table.Rows.RemoveAt(0);
			}

			return sessionClassCollection;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public DataSet GetUserSessionsList(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var sessionClass = new SessionClass();
			DataSet set = null;
			using (var cmd = new SqlCommand())
			{
				sessionClass.GetUserSessionsList(cmd, security);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			return set;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public DataSet GetUserSessionsListWithOrder(SecurityClass security, string order)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (string.IsNullOrEmpty(order))
			{
				return this.GetUserSessionsList(security);
			}

			var sessionClass = new SessionClass();
			DataSet set = null;
			using (var cmd = new SqlCommand())
			{
				sessionClass.GetUserSessionsListWithOrder(cmd, security, order);
				set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			return set;
		}

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public DataSet GetActiveOperateScreensList(SecurityClass security)
        {
            if (security == null)
            {
                throw new ArgumentNullException("security");
            }

            var sessionClass = new SessionClass();
            DataSet set = null;
            using (var cmd = new SqlCommand())
            {
                OperateStatistics.GetActiveOperateScreensListSQL(security, cmd);
                set = this.consolidatedDA.GetDataSet(cmd, security);
            }

            return set;
        }


        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, SessionClass session)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (session == null)
			{
				throw new ArgumentNullException("session");
			}

			// var hardwareKey = new HardwareKeyClass();

			using (var cmd = new SqlCommand())
			{
				session.UpdateSQL(cmd);

				/* Desc Security
				if (hardwareKey.IsDescKey())
				{
					consolidatedDA.ExecuteQuery(security, cmd, DBAccess.ServiceLoginAccess);
				}
				else
				{
				*/
				this.consolidatedDA.ExecuteQuery(security, cmd);

				// }
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid sessionGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			// var hardwareKey = new HardwareKeyClass();

			using (var cmd = new SqlCommand())
			{
				var session = new SessionClass
				{
					Token = sessionGuid
				};
				session.PurgeSQL(cmd);

				/* Desc Secureity
				if (hardwareKey.IsDescKey())
				{
					consolidatedDA.ExecuteQuery(security, cmd, DBAccess.ServiceLoginAccess);
				}
				else
				{
				*/
				this.consolidatedDA.ExecuteSessionCleanupQuery(security, cmd);

				// }
			}
		}

		/// <summary>
		/// Delete any sessions that have expired based on the timeout value
		/// </summary>
		/// <param name="security">A SecurityClass instance</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void PurgeExpired(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			using (var cmd = new SqlCommand())
			{
				var session = new SessionClass();

				session.PurgeExpiredSQL(cmd);

				this.consolidatedDA.ExecuteSessionCleanupQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void PurgeByUser(SecurityClass security, Guid userGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			using (var cmd = new SqlCommand())
			{
				var session = new SessionClass
				{
					UserGuid = userGuid
				};
				session.PurgeByUserSQL(cmd);

				this.consolidatedDA.ExecuteSessionCleanupQuery(security, cmd);
			}
		}

		/// <summary>
		/// Pings the session to keep it alive.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <exception cref="System.ArgumentNullException">security</exception>
		/// <exception cref="AggregateException">Invalid session.</exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void PingSession(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			SessionClass session = GetSessionInfo(security);
			// Only modify tblSessions.UpdatedDate with the current date and time if a specified amount of time has elapsed since the last modify.
			// The time the user was last active does not need to be incredibly precise.
			if (security.SkipSessionTimeUpdate == false)
			{
				session.UpdatedDate = DateTimeOffset.Now;
				session.SiteGuid = security.SiteGuid;
				this.Modify(security, session);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public bool IsSessionValid(string token)
		{
			try
			{
				using (var cmd = new SqlCommand())
				{
					var session = new SessionClass();
					Guid tokenGuid = Guid.Empty;
					if (!Guid.TryParse(token, out tokenGuid))
					{
						return false;
					}
					session.Token = tokenGuid;

					// Just get the basic session items we need to check for session validity.
					session.PingSelectSQL(cmd, false);
					SecurityClass security = new SecurityClass();

					var dataSet = this.consolidatedDA.GetDataSet(cmd, null);

					if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
					{
						// Careful - we do not get UserID, SiteID, LoginSiteID, or AuditLoggingEnabled in this load. 
						// Getting those items is an expensive operation.  Trying to be quick here.
						session.LoadObject(dataSet);

						if (session.Timeout > 0 && (DateTimeOffset.Now - session.UpdatedDate).TotalMinutes > session.Timeout)
						{
							return false;
						}
						return true;
					}
				}
			}
			catch
			{
				;
			}
			return false;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public SessionClass GetSessionInfo(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			using (var cmd = new SqlCommand())
			{
				var session = new SessionClass
				{
					Token = security.Token
				};

				// Just get the basic session items we need to check for session validity.
				session.PingSelectSQL(cmd, false);

				var dataSet = this.consolidatedDA.GetDataSet(cmd, security);

				if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
				{
					// Careful - we do not get UserID, SiteID, LoginSiteID, or AuditLoggingEnabled in this load. 
					// Getting those items is an expensive operation.  Trying to be quick here.
					session.LoadObject(dataSet);

					session.ClientIpAddress = security.ClientIpAddress;
					session.WebServerIpAddress = security.WebServerIpAddress;


					if (session.Timeout > 0 && (DateTimeOffset.Now - session.UpdatedDate).TotalMinutes > session.Timeout)
					{
						throw new FMSessionInvalidException(FMSessionInvalidException.SessionTimedOutExceptionMessage);
					}

					return session;
				}
				else
				{
					throw new FMSessionInvalidException();
				}
			}
		}

		/// <summary>
		/// This method will call an SP to clean the expired user sessions.
		/// </summary>
		/// <param name="security">The security object.</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void CleanupExpiredUserSessions(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			using (var cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "usp_CleanSessionTable";

				int retCode = this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// This method retrieves a distinct list of user sessions currently expired in FuelsManager
		/// </summary>
		/// <param name="security">
		/// A SecurityClass instance
		/// </param>
		/// <returns>
		/// A collection of SessionClass instances
		/// </returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public SessionClassCollection GetExpiredUserSessions(SecurityClass security)
		{
			SessionClassCollection retVal;
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var sessionClass = new SessionClass();
			DataSet Set = null;
			using (var cmd = new SqlCommand())
			{
				sessionClass.EnumerateExpiredSessionsSQL(cmd);
				Set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			var sessionClassCollection = new SessionClassCollection();

			DataTable table = Set.Tables[0];
			while (table.Rows.Count != 0)
			{
				sessionClass = new SessionClass();
				sessionClass.LoadObject(Set);
				sessionClassCollection.Add(sessionClass);
				table.Rows.RemoveAt(0);
			}

			retVal = sessionClassCollection;

			return retVal;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public int GetCountActiveOperateScreens(SecurityClass security)
		{
			int result;

			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			using (var cmd = new SqlCommand())
			{
				OperateStatistics.GetActiveOperateScreenCountSQL(security, cmd);

				object resultVar = this.consolidatedDA.ExecuteScalar(cmd, security);
				try
				{
					result = Convert.ToInt32(resultVar);
				}
				catch (Exception ex)
				{
					_ = ex;
					throw;
					//result = 0;
				}
			}

			return result;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void MarkScreenUsingOperate(SecurityClass security, string windowName ,bool usingOperate)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if(usingOperate) {

				SessionClass session = GetSessionInfo(security);
				session.SiteGuid = security.SiteGuid;
				var configurationSettings = new ConfigurationSettingsClass();
				try
				{
					session.OperateAlarmRefreshInterval = Convert.ToInt32(configurationSettings.GetKeyValueByKey(security, "OperateAlarmRefreshInterval")) * 1000;
				}
				catch (Exception ex) when (ex is OverflowException || ex is FormatException)
				{
				_ = ex;
				}

				try
				{
					session.OperateTagRefreshInterval = Convert.ToInt32(configurationSettings.GetKeyValueByKey(security, "OperateTagRefreshInterval")) * 1000;
				}
				catch (Exception ex) when (ex is OverflowException || ex is FormatException)
				{
					_ = ex;
				}

				this.Modify(security, session);

			}

			using (var cmd = new SqlCommand())
			{
				OperateStatistics.GetActivateOperateScreenSQL(security, windowName, usingOperate, cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void DeactivateStaleOperateScreens(SecurityClass security, int staleSessionTimeout)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = "UPDATE dbo.tblOperateStatistics set OperateActiveStopTime = SYSDATETIMEOFFSET() " +
					"WHERE OperateActiveStopTime IS NULL and UpdatedDate < dateadd(s, @staleTime, SYSDATETIMEOFFSET())";
				cmd.Parameters.AddWithValue("@staleTime", -1 * Math.Abs(staleSessionTimeout));
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
			#endregion
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void SaveSessionOperateStatistics(SecurityClass security, OperateStatistics statistics)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			using (var cmd = new SqlCommand())
			{
				statistics.OperateStatisticsSaveSQL(security, cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}
	}
}
