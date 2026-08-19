// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncSessionLogDBI.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for SyncSessionLogDBI.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.InternalClasses.SyncClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Globalization;
	using System.Linq;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for SyncSessionLogDBI.
	/// </summary>
	public class SyncSessionLogDBI : SyncDBI
	{
		#region Attributes
		#endregion Attributes


		public SyncSessionLogDBI(string user)
			: base(user)
		{
		}


		#region Public Data Access Methods

		/// <summary>
		/// The get list.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="syncNodeGuid">The sync node.</param>
		/// <param name="startDatetimeOffset">The start datetime offset.</param>
		/// <param name="endDateTimeOffset">The end date time offset.</param>
		/// <returns>
		/// The <see><cref>List</cref></see>
		/// .
		/// </returns>
		public List<SyncSessionLogDO> GetList(SecurityClass security, Guid? syncNodeGuid, DateTimeOffset? startDatetimeOffset, DateTimeOffset? endDateTimeOffset, bool? withConflicts)
		{
			List<SyncSessionLogDO> syncSessions = new List<SyncSessionLogDO>();

			DataSet ds = this.Load(security, null, syncNodeGuid, startDatetimeOffset, endDateTimeOffset, withConflicts);

			if (ds.Tables[0].Rows.Count == 0)
			{
				return syncSessions;
			}

			syncSessions.AddRange(from DataRow row in ds.Tables[0].Rows select this.GetDataObjectFromDataRow(row));

			return syncSessions;
		}

		/// <summary>
		/// The get.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="identityGuid">
		/// The identity <see cref="Guid"/>.
		/// </param>
		/// <returns>
		/// The <see cref="SyncSessionLogDO"/>.
		/// </returns>
		public SyncSessionLogDO Get(SecurityClass security, Guid? identityGuid)
		{
			DataSet ds = this.Load(security, identityGuid, null, null, null, null);

			if (ds.Tables[0].Rows.Count == 0)
			{
				return null;
			}

			DataRow row = ds.Tables[0].Rows[0];
			SyncSessionLogDO syncSessionLog = this.GetDataObjectFromDataRow(row);

			return syncSessionLog;
		}

		/// <summary>
		/// The get active session list.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <returns>
		/// The <see>
		///         <cref>List</cref>
		///     </see>
		///     .
		/// </returns>
		public List<SyncSessionLogDO> GetActiveSessionList(SecurityClass security)
		{
			List<SyncSessionLogDO> syncSessionLogs = new List<SyncSessionLogDO>();

			DataSet ds = this.LoadActive(security);

			if (ds.Tables[0].Rows.Count == 0)
			{
				return syncSessionLogs;
			}

			syncSessionLogs.AddRange(from DataRow row in ds.Tables[0].Rows select this.GetDataObjectFromDataRow(row));

			return syncSessionLogs;
		}

		/// <summary>
		/// The get last sync session.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <returns>
		/// The <see cref="SyncSessionLogDO"/>.
		/// </returns>
		public SyncSessionLogDO GetLastSyncSession(SecurityClass security)
		{
			DataSet ds = this.LoadLatest(security);

			if (ds.Tables[0].Rows.Count == 0)
			{
				return null;
			}

			DataRow row = ds.Tables[0].Rows[0];
			SyncSessionLogDO syncSessionLog = this.GetDataObjectFromDataRow(row);

			return syncSessionLog;
		}

		/// <summary>
		/// Gets a distinct list of remote node machine names from the synchronization session logs.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <returns>
		/// The <see>
		///         <cref>List</cref>
		///     </see>
		/// </returns>
		public Dictionary<Guid, string> GetRemoteNodes(SecurityClass security)
		{
			var nodeDictionary = new Dictionary<Guid, string>();

			DataSet ds = this.LoadDistinctRemoteNodes(security);

			foreach (DataRow row in ds.Tables[0].Rows)
			{
				nodeDictionary.Add((Guid)row["RemoteNodeGuid"], row["RemoteNodeMachineName"] as string);
			}

			return nodeDictionary;
		}

		/// <summary>
		/// Saves the passed in <seealso cref="SyncSessionLogDO"/> record.
		/// </summary>
		/// <param name="security">Contains security credentials</param>
		/// <param name="dataObject">The synchronization session information to persist.</param>
		/// <returns>True if the warning level for drawdown is hit.  Otherwise, false</returns>
		/// <remarks>Utilizes a merge stored procedure to implement Insert/Update operations in a single call.</remarks>
		public bool Save(SecurityClass security, SyncSessionLogDO dataObject)
		{
			// Save the dataobject using a merge implementation
			this.Merge(security, dataObject);

			return true;
		}

		/// <summary>
		/// Deletes the passed in <seealso cref="SyncSessionLogDO"/> record.
		/// </summary>
		/// <param name="security">Contains security credentials</param>
		/// <param name="dataObject">The synchronization session information to delete.</param>
		/// <param name="purge">Permanently delete the specified synchronization session.</param>
		/// <returns>True if the warning level for drawdown is hit.  Otherwise, false</returns>
		public bool Delete(SecurityClass security, SyncSessionLogDO dataObject, bool purge)
		{
			try
			{
				// Delete the dataobject.
				this.Delete(security, dataObject);

				return true;
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Cleanups the active sessions.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <returns>System.Int32.</returns>
		public void CleanupActiveSessions(SecurityClass security)
		{
			try
			{
				using (var cmd = new SqlCommand())
				{

					cmd.CommandText = "sync.usp_SyncSessionLogCleanupActive";
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.Clear();

					this.ConsolidatedDA.ExecuteQueryWithoutSessionContext(security, cmd);
				}
			}
			catch (Exception)
			{
				throw;
			}

			return;
		}

		public DataSet GetNodeHealthSummary(SecurityClass security, int nodeStatus)
		{
			DataSet ds = this.LoadNodeHealthSummary(security, nodeStatus);

			if (ds.Tables[0].Rows.Count == 0)
			{
				return null;
			}

			return ds;
		}

		public DataSet GetNodeHealthWithOrderSummary(SecurityClass security, string order, int nodeStatus)
		{
			DataSet ds = this.LoadNodeHealthWithOrderSummary(security, order, nodeStatus);

			if (ds.Tables[0].Rows.Count == 0)
			{
				return null;
			}

			return ds;
		}

		#endregion Public Data Access Methods

		#region Private Persistence Methods
		/// <summary>
		/// Returns a DataSet containing the SyncSession record containing the specified Primary Key.
		/// </summary>
		/// <param name="security">Contains security credentials</param>
		/// <param name="identityGuid">The identity unique identifier.</param>
		/// <param name="syncNodeGuid">Synchronization Node Guid.</param>
		/// <param name="startDateTimeOffset">The start date time offset.</param>
		/// <param name="endDateTimeOffset">The end date time offset.</param>
		/// <param name="withConflicts">The with conflicts.</param>
		/// <returns>
		/// The <see cref="DataSet" />.
		/// </returns>
		private DataSet Load(SecurityClass security, Guid? identityGuid, Guid? syncNodeGuid, DateTimeOffset? startDateTimeOffset, DateTimeOffset? endDateTimeOffset, bool? withConflicts)
		{
			DataSet ds = null;

			using (var cmd = this.PrepareSelectStatement())
			{
				SqlParameterCollection parms = cmd.Parameters;
				parms["@StartDateTimeOffset"].Value = this.SetOptionalValue<DateTimeOffset>(startDateTimeOffset);
				parms["@EndDateTimeOffset"].Value = this.SetOptionalValue<DateTimeOffset>(endDateTimeOffset);
				parms["@IdentityGuid"].Value = this.SetOptionalValue<Guid>(identityGuid);
				parms["@SyncNodeGuid"].Value = this.SetOptionalValue<Guid>(syncNodeGuid == Guid.Empty ? null : syncNodeGuid);
				parms["@WithConflicts"].Value = this.SetOptionalValue<bool>(withConflicts);

				ds = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			return ds;
		}

		/// <summary>
		/// Returns a DataSet containing all SyncSession records with a Started Date and no End Date
		/// </summary>
		/// <param name="security">
		/// Contains security credentials
		/// </param>
		/// <returns>
		/// Returns a <see cref="DataSet"/> populated with the matching SyncSessions.
		/// </returns>
		private DataSet LoadActive(SecurityClass security)
		{
			DataSet ds = null;

			using (var cmd = this.PrepareSelectActiveSessionsStatement())
			{
				ds = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			return ds;
		}

		/// <summary>
		/// Returns a DataSet containing the most recent SyncSession record with an End Date
		/// </summary>
		/// <param name="security">
		/// Contains security credentials
		/// </param>
		/// <returns>
		/// Returns a <see cref="DataSet"/> populated with the matching SyncSession.
		/// </returns>
		private DataSet LoadLatest(SecurityClass security)
		{
			DataSet ds = null;

			using (var cmd = this.PrepareSelectLastSessionsStatement())
			{
				ds = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			return ds;
		}

		/// <summary>
		/// Returns a DataSet containing a list of distinct remote node machine names from the SyncSessionLog records
		/// </summary>
		/// <param name="security">
		/// Contains security credentials
		/// </param>
		/// <returns>
		/// The <see cref="DataSet"/>.
		/// </returns>
		private DataSet LoadDistinctRemoteNodes(SecurityClass security)
		{
			DataSet ds = null;

			using (var cmd = this.CreateSelectDistinctNodesStatement())
			{
				SqlParameterCollection parms = cmd.Parameters;

				cmd.Parameters["@SiteGuid"].Value = security.SiteGuid;
				ds = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			return ds;
		}

		private DataSet LoadNodeHealthSummary(SecurityClass security, int nodeStatus)
		{
			DataSet ds = null;

			using (var cmd = this.CreateNodeHealthSummaryCommand(security, nodeStatus))
			{
				ds = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			return ds;
		}

		private DataSet LoadNodeHealthWithOrderSummary(SecurityClass security, string order, int nodeStatus)
		{
			DataSet ds = null;

			using (var cmd = this.CreateNodeHealthSummaryWithOrderCommand(security, order, nodeStatus))
			{
				ds = this.ConsolidatedDA.GetDataSet(cmd, security);
			}

			return ds;
		}

		/// <summary>
		/// Merges current <seealso cref="SyncSessionLogDO"/> record with an existing record.  If the record doesn't currently
		/// exist, a new record is inserted.
		/// </summary>
		/// <param name="security">Contains security credentials</param>
		/// <param name="dataObject">The synchronization session information to update or insert.</param>
		private void Merge(SecurityClass security, SyncSessionLogDO dataObject)
		{
			using (var cmd = this.PrepareUpsertStatement())
			{
				SqlParameterCollection parms = cmd.Parameters;
				parms["@IdentityGuid"].Value = dataObject.IdentityGuid;
				parms["@SyncProfileID"].Value = dataObject.ID;
				parms["@SyncRequestTypeIndex"].Value = (int)dataObject.SyncRequestTypeIndex;
				parms["@SyncTransferTypeIndex"].Value = (int)dataObject.SyncTransferTypeIndex;
				parms["@SyncSessionStatusIndex"].Value = (int)dataObject.SyncSessionStatusIndex;
				parms["@SyncSessionStateIndex"].Value = (int)dataObject.SyncSessionStateIndex;
				parms["@SyncDateRangeStart"].Value =
					this.SetOptionalValue<DateTimeOffset>(dataObject.SyncDateRangeStart);
				parms["@SyncDateRangeEnd"].Value =
					this.SetOptionalValue<DateTimeOffset>(dataObject.SyncDateRangeEnd);
				parms["@StartDate"].Value =
					this.SetOptionalValue<DateTimeOffset>(dataObject.StartDate);
				parms["@EndDate"].Value =
					this.SetOptionalValue<DateTimeOffset>(dataObject.EndDate);
				parms["@RemoteNodeGuid"].Value = dataObject.RemoteNodeGuid;
				parms["@RemoteNodeMachineName"].Value = dataObject.RemoteNodeMachineName;
				parms["@SyncAnchorMax"].Value = dataObject.SyncAnchorMax;
				parms["@CreatedBy"].Value = security.UserID;
				parms["@UpdatedBy"].Value = security.UserID;

				this.ConsolidatedDA.ExecuteQueryWithoutSessionContext(security, cmd);

				Guid? retIdentityGuid = this.GetOutputValue<Guid>(parms["@NewRowGuid"], dataObject.IdentityGuid);

				if (retIdentityGuid.HasValue && retIdentityGuid.Value != dataObject.IdentityGuid)
				{
					dataObject.IdentityGuid = retIdentityGuid.Value;
				}

				dataObject.Changed = false;
			}
		}

		/// <summary>
		/// Removes the specified <seealso cref="SyncSessionLogDO"/> record.
		/// </summary>
		/// <param name="security">Contains security credentials</param>
		/// <param name="dataObject">The synchronization session information to persist.</param>
		private void Delete(SecurityClass security, SyncSessionLogDO dataObject)
		{
			using (var cmd = this.PrepareDeleteStatement())
			{
				SqlParameterCollection parms = null;
				parms = cmd.Parameters;
				parms["@IdentityGuid"].Value = dataObject.IdentityGuid;

				this.ConsolidatedDA.ExecuteQueryWithoutSessionContext(security, cmd);
			}
		}
		#endregion Private Persistence Methods

		#region Override Implementations for Prepare Methods

		/// <summary>
		/// Create a command object bound to a merge (insert/update) stored procedure and parameters.
		/// </summary>
		/// <returns>
		/// An instance of a <see cref="SqlCommand"/> object bound to the appropriate stored procedure and with the appropriate parameters.
		/// </returns>
		protected override SqlCommand PrepareUpsertStatement()
		{
			return this.CreateMergeStatement();
		}

		/// <summary>
		/// Create a command object bound to a select stored procedure and parameters.
		/// </summary>
		/// <returns>
		/// An instance of a <see cref="SqlCommand"/> object bound to the appropriate stored procedure and with the appropriate parameters.
		/// </returns>
		protected override SqlCommand PrepareSelectStatement()
		{
			var cmd = new SqlCommand();

			cmd.CommandText = "sync.usp_SyncSessionLogSelect";
			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.Clear();

			cmd.Parameters.Add("@StartDateTimeOffset", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@EndDateTimeOffset", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@SyncNodeGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@WithConflicts", SqlDbType.Bit);

			return cmd;
		}

		/// <summary>
		/// Create a command object bound to a merge (insert/update) stored procedure and parameters.
		/// </summary>
		/// <returns>
		/// An instance of a <see cref="SqlCommand"/> object bound to the appropriate stored procedure and with the appropriate parameters.
		/// </returns>
		/// <remarks>
		/// This method is provided to meet the required implementation of an Abstract Method.  All Insert / Update logic has been implemented as a Merge stored 
		/// procedure so Modify or Insert can be used interchangeably.
		/// </remarks>
		protected override SqlCommand PrepareInsertStatement()
		{
			return this.CreateMergeStatement();
		}

		/// <summary>
		/// Create a command object bound to a merge (insert/update) stored procedure and parameters.
		/// </summary>
		/// <returns>
		/// An instance of a <see cref="SqlCommand"/> object bound to the appropriate stored procedure and with the appropriate parameters.
		/// </returns>
		/// <remarks>
		/// This method is provided to meet the required implementation of an Abstract Method.  All Insert / Update logic has been implemented as a Merge stored 
		/// procedure so Modify or Insert can be used interchangeably.
		/// </remarks>
		protected override SqlCommand PrepareUpdateStatement()
		{
			return this.CreateMergeStatement();
		}

		/// <summary>
		/// The prepare delete statement.
		/// </summary>
		/// <returns>
		/// The <see cref="SqlCommand"/>.
		/// </returns>
		protected override SqlCommand PrepareDeleteStatement()
		{
			var cmd = new SqlCommand();

			cmd.CommandText = "sync.usp_SyncSessionLogDeleteByGuid";
			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.Clear();

			cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);

			return cmd;
		}
		#endregion Override Implementations for Prepare Methods

		#region Private Support Methods

		/// <summary>
		/// The prepare select active sessions statement.
		/// </summary>
		/// <returns>
		/// The <see cref="SqlCommand"/>.
		/// </returns>
		private SqlCommand PrepareSelectActiveSessionsStatement()
		{
			var cmd = new SqlCommand();

			cmd.CommandText = "sync.usp_SyncSessionLogSelectActive";
			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.Clear();

			return cmd;
		}

		/// <summary>
		/// The prepare select last session statement.
		/// </summary>
		/// <returns>
		/// The <see cref="SqlCommand"/>.
		/// </returns>
		private SqlCommand PrepareSelectLastSessionsStatement()
		{
			var cmd = new SqlCommand();

			cmd.CommandText = "sync.usp_SyncSessionLogSelectLast";
			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.Clear();

			return cmd;
		}

		/// <summary>
		/// The create merge statement.
		/// </summary>
		/// <returns>
		/// The <see cref="SqlCommand"/>.
		/// </returns>
		private SqlCommand CreateMergeStatement()
		{
			var cmd = new SqlCommand();

			cmd.CommandText = "sync.usp_SyncSessionLogSave";
			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.Clear();

			cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@SyncProfileID", SqlDbType.NVarChar, 80);
			cmd.Parameters.Add("@SyncRequestTypeIndex", SqlDbType.BigInt);
			cmd.Parameters.Add("@SyncTransferTypeIndex", SqlDbType.BigInt);
			cmd.Parameters.Add("@SyncSessionStatusIndex", SqlDbType.BigInt);
			cmd.Parameters.Add("@SyncSessionStateIndex", SqlDbType.BigInt);
			cmd.Parameters.Add("@SyncDateRangeStart", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@SyncDateRangeEnd", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@StartDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@EndDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@RemoteNodeGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@RemoteNodeMachineName", SqlDbType.NVarChar, 256);
			cmd.Parameters.Add("@SyncAnchorMax", SqlDbType.BigInt);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);

			cmd.Parameters.Add("@NewRowGuid", SqlDbType.UniqueIdentifier).Direction = ParameterDirection.Output;

			return cmd;
		}

		/// <summary>
		/// Create a command object bound to a select stored procedure and parameters.
		/// </summary>
		/// <returns>
		/// An instance of a <see cref="SqlCommand"/> object bound to the appropriate stored procedure and with the appropriate parameters.
		/// </returns>
		protected SqlCommand CreateSelectDistinctNodesStatement()
		{
			var cmd = new SqlCommand();

			cmd.CommandText = "sync.usp_SyncSessionLogSelectDistinctNodes";
			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.Clear();

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

			return cmd;
		}

		private const string NodeHealthSelect = @"
			DECLARE @CriticalThresholdHours int
			SET @CriticalThresholdHours = (SELECT NodeHealthCriticalThresholdHours from tblSyncServerConfiguration)

			DECLARE @CautionThresholdHours int
			SET @CautionThresholdHours = (SELECT NodeHealthCautionThresholdHours from tblSyncServerConfiguration)

			SELECT * FROM (
			SELECT d.*,
					CASE
						WHEN d.conflicts > 0 OR d.lastSyncHours >= @CriticalThresholdHours THEN 2 
						WHEN d.lastSyncHours >= @CautionThresholdHours AND d.lastSyncHours < @CriticalThresholdHours THEN 1 
						ELSE 0 
					END AS 'nodeHealthIndicator'
			FROM (SELECT sessionLog.RemoteNodeMachineName AS 'nodeName'
					,sites.Number AS 'siteName'
					,sites.ID AS 'dodaac'
					,COUNT(conflict.TargetNodeGuid) AS 'conflicts'
					,MAX(sessionLog.EndDate) AS 'lastSyncDate'
					,DATEDIFF(HOUR, CAST(MAX(sessionLog.EndDate) as datetime), CURRENT_TIMESTAMP) as 'lastSyncHours'
					,SUM(scopeLog.TotalChangesCount) AS 'syncCount'
					,MAX(DATEDIFF(MINUTE, sessionLog.StartDate, sessionLog.EndDate)) AS 'syncTimeMinutes' 
				FROM tblSites sites WITH (NOLOCK)		
				INNER JOIN dbo.udf_GetSiteToSiteHierarchyListForSiteGuid(@SiteGuid,1,0,1,0,1,0) AS sh on sites.ID = sh.siteid	
				LEFT JOIN sync.tblSyncSessionScopeLog scopeLog WITH (NOLOCK) 
						INNER JOIN sync.tblSyncRecordConflictToSyncSessionScopeLog mapScopeLog WITH (NOLOCK) 
							INNER JOIN sync.tblSyncRecordConflict conflict WITH (NOLOCK) ON conflict.SyncRecordConflictGuid = mapScopeLog.SyncRecordConflictGuid 
								and conflict.syncconflictresolutionstatusindex not in (1,2) 
							ON mapScopeLog.SyncSessionScopeLogGuid = scopeLog.SyncSessionScopeLogGuid
						INNER JOIN sync.tblSyncSessionLog sessionLog WITH (NOLOCK) 
					ON scopeLog.SyncSessionLogGuid = sessionLog.SyncSessionLogGuid
				ON sites.SiteGuid = scopeLog.SiteGuid
				GROUP BY sessionLog.RemoteNodeMachineName, sites.Number, sites.ID
				) AS d ) AS SUB";

		private const string NodeHealthDefaultOrderBy = " ORDER BY nodeHealthIndicator DESC, dodaac ASC";

		protected SqlCommand CreateNodeHealthSummaryCommand(SecurityClass security, int nodeStatus)
		{
			string sql = NodeHealthSelect;

			var cmd = new SqlCommand();

			if (nodeStatus != -1) //all
			{
				sql += " WHERE nodeHealthIndicator = @nodeStatus";
				cmd.Parameters.Add("@nodeStatus", SqlDbType.Int).Value = nodeStatus;
			}

			sql += NodeHealthDefaultOrderBy;

			cmd.CommandText = sql;
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = security.SiteGuid;

			return cmd;
		}

		protected SqlCommand CreateNodeHealthSummaryWithOrderCommand(SecurityClass security, string orderBy, int nodeStatus)
		{
			var sql = NodeHealthSelect;

			var cmd = new SqlCommand();

			if (nodeStatus != -1) //all
			{
				sql += " WHERE nodeHealthIndicator = @nodeStatus";
				cmd.Parameters.Add("@nodeStatus", SqlDbType.Int).Value = nodeStatus;
			}

			cmd.CommandText = sql;

			sql += " ORDER BY " + orderBy;

			cmd.CommandText = sql;
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = security.SiteGuid;

			return cmd;
		}

		/// <summary>
		/// The get data object from data row.
		/// </summary>
		/// <param name="row">
		/// The row.
		/// </param>
		/// <returns>
		/// The <see cref="SyncSessionLogDO"/>.
		/// </returns>
		private SyncSessionLogDO GetDataObjectFromDataRow(DataRow row)
		{
			SyncSessionLogDO dataObject = new SyncSessionLogDO();

			dataObject.IdentityGuid = DataObject.getValue<Guid>(row["SyncSessionLogGuid"], Guid.Empty);
			dataObject.SyncProfileID = DataObject.getValue<string>(row["SyncProfileID"], string.Empty);

			long indexValue = DataObject.getValue<long>(row["SyncRequestTypeIndex"], (long)SYNCREQUESTTYPE.MANUAL);
			SYNCREQUESTTYPE syncRequestType = SYNCREQUESTTYPE.MANUAL;

			if (Enum.TryParse(indexValue.ToString(CultureInfo.InvariantCulture), true, out syncRequestType))
			{
				dataObject.SyncRequestTypeIndex = syncRequestType;
			}

			indexValue = DataObject.getValue<long>(row["SyncTransferTypeIndex"], (long)SYNCTRANSFERTYPE.ONLINE);
			SYNCTRANSFERTYPE syncTransferType = SYNCTRANSFERTYPE.ONLINE;

			if (Enum.TryParse(indexValue.ToString(CultureInfo.InvariantCulture), true, out syncTransferType))
			{
				dataObject.SyncTransferTypeIndex = syncTransferType;
			}

			indexValue = DataObject.getValue<long>(row["SyncSessionStatusIndex"], (long)SYNCSESSIONSTATUS.NEW);
			SYNCSESSIONSTATUS syncSessionStatus = SYNCSESSIONSTATUS.NEW;

			if (Enum.TryParse(indexValue.ToString(CultureInfo.InvariantCulture), true, out syncSessionStatus))
			{
				dataObject.SyncSessionStatusIndex = syncSessionStatus;
			}

			indexValue = DataObject.getValue<long>(row["SyncSessionStateIndex"], (long)SYNCSESSIONSTATE.INIT);
			SYNCSESSIONSTATE syncSessionState = SYNCSESSIONSTATE.INIT;

			if (Enum.TryParse(indexValue.ToString(CultureInfo.InvariantCulture), true, out syncSessionState))
			{
				dataObject.SyncSessionStateIndex = syncSessionState;
			}

			dataObject.SyncDateRangeStart = DataObject.getOptionalDateTimeOffset(row["SyncDateRangeStart"]);
			dataObject.SyncDateRangeEnd = DataObject.getOptionalDateTimeOffset(row["SyncDateRangeEnd"]);
			dataObject.StartDate = DataObject.getOptionalDateTimeOffset(row["StartDate"]);
			dataObject.EndDate = DataObject.getOptionalDateTimeOffset(row["EndDate"]);
			dataObject.RemoteNodeGuid = DataObject.getValue<Guid>(row["RemoteNodeGuid"], Guid.Empty);
			dataObject.RemoteNodeMachineName = DataObject.getString(row["RemoteNodeMachineName"]);
			dataObject.SyncAnchorMax = DataObject.getLong(row["SyncAnchorMax"]);
			if (row.Table.Columns.Contains("Conflicts"))
			{
				dataObject.Conflicts = DataObject.getInt(row["Conflicts"]);
			}
			dataObject.CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
			dataObject.CreatedBy = DataObject.getString(row["CreatedBy"]);
			dataObject.UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], DateTimeOffset.Now);
			dataObject.UpdatedBy = DataObject.getString(row["UpdatedBy"]);

			return dataObject;
		}
		#endregion Private Support Methods
	}
}
