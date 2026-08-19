// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncSessionScopeLogDBI.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	Summary description for SyncSessionScopeLogDBI.
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
	/// Summary description for SyncSessionScopeLogDBI.
	/// </summary>
	public class SyncSessionScopeLogDBI : SyncDBI
	{
		#region Attributes
		#endregion Attributes

		public SyncSessionScopeLogDBI(string user)
				: base(user)
		{
		}

		#region Public Data Access Methods

		/// <summary>
		/// The get.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="identityGuid">
		/// The identity guid.
		/// </param>
		/// <returns>
		/// The <see cref="SyncSessionScopeLogDO"/>.
		/// </returns>
		public SyncSessionScopeLogDO Get(SecurityClass security, Guid identityGuid)
		{
				DataSet ds = this.Load(security, identityGuid);

				if (ds.Tables[0].Rows.Count == 0)
				{
					return null;
				}

				DataRow row = ds.Tables[0].Rows[0];
				SyncSessionScopeLogDO syncProfile = this.GetDataObjectFromDataRow(row);

				return syncProfile;
		}

		/// <summary>
		/// The get by site GUID.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="syncSessionLogGuid">
		/// The sync session GUID.
		/// </param>
		/// <param name="siteGuid">
		/// The site GUID.
		/// </param>
		/// <returns>
		/// The <see cref="SyncSessionScopeLogDO"/>.
		/// </returns>
		public SyncSessionScopeLogDO GetBySiteGuid(SecurityClass security, Guid syncSessionLogGuid, Guid? siteGuid)
		{
				DataSet ds = this.LoadBySiteGuid(security, syncSessionLogGuid, siteGuid);

				if (ds.Tables[0].Rows.Count == 0)
				{
					return null;
				}

				DataRow row = ds.Tables[0].Rows[0];
				SyncSessionScopeLogDO syncProfile = this.GetDataObjectFromDataRow(row);

				return syncProfile;
		}

		/// <summary>
		/// The get by a unique composite key made up of the Sync Session Log GUID, Site GUID, Scope ID and Site Type Index
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="syncSessionLogGuid">
		/// The sync session Log GUID.
		/// </param>
		/// <param name="siteGuid">
		/// The site GUID.
		/// </param>
		/// <param name="scopeID">
		/// The scope ID.
		/// </param>
		/// <returns>
		/// The <see cref="SyncSessionScopeLogDO"/>.
		/// </returns>
		public SyncSessionScopeLogDO GetByCompositeKey(SecurityClass security, Guid syncSessionLogGuid, Guid? siteGuid, string scopeID)
		{
				DataSet ds = this.LoadByCompositeKey(security, syncSessionLogGuid, siteGuid, scopeID);

				if (ds.Tables[0].Rows.Count == 0)
				{
					return null;
				}

				DataRow row = ds.Tables[0].Rows[0];
				SyncSessionScopeLogDO syncProfile = this.GetDataObjectFromDataRow(row);

				return syncProfile;
		}

		/// <summary>
		/// The get list by sync session.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="syncSessionIdentityGuid">
		/// The sync session identity GUID.
		/// </param>
		/// <returns>
		/// A collection of <see cref="SyncSessionScopeLogDO"/> data objects that meet the specified criteria.
		/// </returns>
		public List<SyncSessionScopeLogDO> GetListBySyncSession(SecurityClass security, Guid syncSessionIdentityGuid)
		{
				List<SyncSessionScopeLogDO> syncSessionScopeLogs = new List<SyncSessionScopeLogDO>();

				DataSet ds = this.LoadBySyncSession(security, syncSessionIdentityGuid);

				if (ds.Tables[0].Rows.Count == 0)
				{
					return syncSessionScopeLogs;
				}

				foreach (DataRow row in ds.Tables[0].Rows)
				{
					SyncSessionScopeLogDO syncSessionDetail = this.GetDataObjectFromDataRow(row);
					syncSessionScopeLogs.Add(syncSessionDetail);
				}

				return syncSessionScopeLogs;
		}

		/// <summary>
		/// Saves the passed in <seealso cref="SyncSessionScopeLogDO"/> record.
		/// </summary>
		/// <param name="security">Contains security credentials</param>
		/// <param name="dataObject">The synchronization session detail information to persist.</param>
		/// <returns>True if the passed in data object was saved successfully.  Otherwise, false</returns>
		/// <remarks>Utilizes a merge stored procedure to implement Insert/Update operations in a single call.</remarks>
		public bool Save(SecurityClass security, SyncSessionScopeLogDO dataObject)
		{
				// Save the data object using a merge implementation
				this.Merge(security, dataObject);

				return true;
		}

		/// <summary>
		/// Deletes the passed in <seealso cref="SyncSessionScopeLogDO"/> record.
		/// </summary>
		/// <param name="security">Contains security credentials</param>
		/// <param name="dataObject">The synchronization session detail information to delete.</param>
		/// <param name="purge">Permanently delete the specified synchronization session detail.</param>
		/// <returns>True if the specified record was deleted</returns>
		public bool Delete(SecurityClass security, SyncSessionScopeLogDO dataObject, bool purge)
		{
				try
				{
					// Delete the data object
					this.Delete(security, dataObject);

					return true;
				}
				catch (Exception eX)
				{
					throw eX;
				}
		}
		#endregion Public Data Access Methods

		#region Private Persistence Methods
		/// <summary>
		/// Loads the specified 
		/// <seealso cref="SyncSessionScopeLogDO"/>
		/// record by the Primary Key.
		/// </summary>
		/// <param name="security">
		/// Contains security credentials
		/// </param>
		/// <param name="identityGuid">
		/// The synchronization session detail information to retrieve.
		/// </param>
		/// <returns>
		/// The <see cref="DataSet"/>.
		/// </returns>
		private DataSet Load(SecurityClass security, Guid identityGuid)
		{
			DataSet ds = null;

			using (var cmd = this.PrepareSelectStatement())
			{
				SqlParameterCollection parms = cmd.Parameters;
				parms["@IdentityGuid"].Value = identityGuid;

				ds = ConsolidatedDA.GetDataSet(cmd, security);
			}

			return ds;
		}

		/// <summary>
		/// Loads a Synchronization Session Detail record for a specific Site GUID within the scope of the specified Synchronization Session GUID.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="syncSessionLogGuid">
		/// The sync session GUID.
		/// </param>
		/// <param name="siteGuid">
		/// The site GUID.
		/// </param>
		/// <returns>
		/// The <see cref="DataSet"/>.
		/// </returns>
		private DataSet LoadBySiteGuid(SecurityClass security, Guid syncSessionLogGuid, Guid? siteGuid)
		{
			DataSet ds = null;

			using (var cmd = this.PrepareSelectBySiteGuidStatement())
			{
				SqlParameterCollection parms = cmd.Parameters;
				parms["@SyncSessionLogGuid"].Value = syncSessionLogGuid;
				parms["@SiteGuid"].Value = (siteGuid.HasValue && siteGuid.Value != Guid.Empty) ? siteGuid.Value : (object)DBNull.Value;

				ds = ConsolidatedDA.GetDataSet(cmd, security);
			}

			return ds;
		}

		/// <summary>
		/// Loads a Synchronization Session Detail record for a specific Site GUID, Scope ID and Site Type Index within the scope of the specified Synchronization Session GUID.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="syncSessionLogGuid">
		/// The sync session GUID.
		/// </param>
		/// <param name="siteGuid">
		/// The site GUID.
		/// </param>
		/// <param name="scopeID">
		/// The scope ID.
		/// </param>
		/// <returns>
		/// The <see cref="DataSet"/>.
		/// </returns>
		private DataSet LoadByCompositeKey(SecurityClass security, Guid syncSessionLogGuid, Guid? siteGuid, string scopeID)
		{
			DataSet ds = null;

			using (var cmd = this.PrepareSelectByCompositeKeyStatement())
			{
				SqlParameterCollection parms = cmd.Parameters;
				parms["@SyncSessionLogGuid"].Value = syncSessionLogGuid;
				parms["@SiteGuid"].Value = (siteGuid.HasValue && siteGuid.Value != Guid.Empty) ? siteGuid.Value : (object)DBNull.Value;
				parms["@ScopeID"].Value = scopeID;

				ds = ConsolidatedDA.GetDataSet(cmd, security);
			}

			return ds;
		}

		/// <summary>
		/// Loads all  
		/// <seealso cref="SyncSessionScopeLogDO"/>
		/// records associated with the specified synchronization session.
		/// </summary>
		/// <param name="security">
		/// Contains security credentials
		/// </param>
		/// <param name="syncSessionIdentityGuid">
		/// The synchronization session to retrieve the session details for.
		/// </param>
		/// <returns>
		/// The <see cref="DataSet"/>.
		/// </returns>
		private DataSet LoadBySyncSession(SecurityClass security, Guid syncSessionIdentityGuid)
		{
			DataSet ds = null;

			using (var cmd = this.PrepareSelectBySyncSessionLogStatement())
			{
				SqlParameterCollection parms = cmd.Parameters;
				parms["@SyncSessionLogGuid"].Value = syncSessionIdentityGuid;
				parms["@MaxRecords"].Value = Int64.MaxValue;
				parms["@StartRowVersion"].Value = 0;

				ds = ConsolidatedDA.GetDataSet(cmd, security);
			}

			return ds;
		}

		/// <summary>
		/// Merges current <seealso cref="SyncSessionScopeLogDO"/> record with an existing record.  If the record doesn't currently
		/// exist, a new record is inserted.
		/// </summary>
		/// <param name="security">Contains security credentials</param>
		/// <param name="dataObject">The synchronization session detail information to update or insert.</param>
		private void Merge(SecurityClass security, SyncSessionScopeLogDO dataObject)
		{
			using (var cmd = this.PrepareUpsertStatement())
			{
				SqlParameterCollection parms = cmd.Parameters;
				parms["@IdentityGuid"].Value = dataObject.IdentityGuid;
				parms["@SyncSessionLogGuid"].Value = dataObject.SyncSessionLogGuid;
				parms["@SiteGuid"].Value = (dataObject.SiteGuid != Guid.Empty) ? dataObject.SiteGuid : (object)DBNull.Value;
				parms["@ScopeID"].Value = dataObject.SyncScopeID;
				parms["@SiteTypeIndex"].Value = dataObject.SiteTypeIndex.HasValue ? (long)dataObject.SiteTypeIndex.Value : (object)DBNull.Value;
				parms["@SyncSessionStatusIndex"].Value = (int)dataObject.SyncSessionStatusIndex;
				parms["@SyncSessionStateIndex"].Value = (int)dataObject.SyncSessionStateIndex;
				parms["@StartDate"].Value = this.SetOptionalValue<DateTimeOffset>(dataObject.StartDate);
				parms["@EndDate"].Value = this.SetOptionalValue<DateTimeOffset>(dataObject.EndDate);
				parms["@TableCount"].Value = dataObject.TableCount;
				parms["@TableSuccessCount"].Value = dataObject.TableSuccessCount;
				parms["@TableErrorCount"].Value = dataObject.TableErrorCount;
				parms["@TotalChangesCount"].Value = dataObject.TotalChangesCount;
				parms["@TotalChangesAppliedCount"].Value = dataObject.TotalChangesAppliedCount;
				parms["@TotalChangesFailedCount"].Value = dataObject.TotalChangesFailedCount;
				parms["@TotalChangesPendingCount"].Value = dataObject.TotalChangesPendingCount;
				parms["@TotalDeleteCount"].Value = dataObject.TotalDeleteCount;
				parms["@TotalInsertCount"].Value = dataObject.TotalInsertCount;
				parms["@TotalUpdateCount"].Value = dataObject.TotalUpdateCount;
				parms["@BatchFileName"].Value = dataObject.BatchFileName;

				parms["@CreatedBy"].Value = security.UserID;
				parms["@UpdatedBy"].Value = security.UserID;

				ConsolidatedDA.ExecuteQueryWithoutSessionContext(security, cmd);

				Guid? retIdentityGuid = this.GetOutputValue<Guid>(parms["@NewRowGuid"], dataObject.IdentityGuid);

				if (retIdentityGuid.HasValue && retIdentityGuid.Value != dataObject.IdentityGuid)
				{
					dataObject.IdentityGuid = retIdentityGuid.Value;
				}
			}
		}

		/// <summary>
		/// Removes the specified <seealso cref="SyncSessionScopeLogDO"/> record.
		/// </summary>
		/// <param name="security">Contains security credentials</param>
		/// <param name="dataObject">The synchronization session detail information to persist.</param>
		private void Delete(SecurityClass security, SyncSessionScopeLogDO dataObject)
		{
			using (var cmd = this.PrepareDeleteStatement())
			{
				SqlParameterCollection parms = null;
				parms = cmd.Parameters;
				parms["@IdentityGuid"].Value = dataObject.IdentityGuid;

				ConsolidatedDA.ExecuteQueryWithoutSessionContext(security, cmd);
			}
		}
		#endregion Private Persistence Methods

		#region Override Implementations for Prepare Methods

		/// <summary>
		/// The prepare upsert statement.
		/// </summary>
		/// <returns>
		/// The <see cref="SqlCommand"/>.
		/// </returns>
		protected override SqlCommand PrepareUpsertStatement()
		{
			return this.CreateMergeStatement();
		}

		/// <summary>
		/// The prepare select statement.
		/// </summary>
		/// <returns>
		/// The <see cref="SqlCommand"/>.
		/// </returns>
		protected override SqlCommand PrepareSelectStatement()
		{
			var cmd = new SqlCommand();

			cmd.CommandText = "sync.usp_SyncSessionScopeLogSelect";
			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.Clear();

			cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);

			return cmd;
		}

		/// <summary>
		/// The prepare insert statement.
		/// </summary>
		/// <returns>
		/// The <see cref="SqlCommand"/>.
		/// </returns>
		protected override SqlCommand PrepareInsertStatement()
		{
			return this.CreateMergeStatement();
		}

		/// <summary>
		/// The prepare update statement.
		/// </summary>
		/// <returns>
		/// The <see cref="SqlCommand"/>.
		/// </returns>
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

			cmd.CommandText = "sync.usp_SyncSessionScopeLogDeleteByGuid";
			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.Clear();

			cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);

			return cmd;
		}
		#endregion Override Implementations for Prepare Methods

		#region Private Support Methods

		/// <summary>
		/// The prepare select by site GUID statement.
		/// </summary>
		/// <returns>
		/// The <see cref="SqlCommand"/>.
		/// </returns>
		protected SqlCommand PrepareSelectBySiteGuidStatement()
		{
			var cmd = new SqlCommand();

			cmd.CommandText = "sync.usp_SyncSessionScopeLogSelectBySiteGuid";
			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.Clear();

			cmd.Parameters.Add("@SyncSessionLogGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

			return cmd;
		}

		/// <summary>
		/// The prepare select by composite key statement.
		/// </summary>
		/// <returns>
		/// The <see cref="SqlCommand"/>.
		/// </returns>
		protected SqlCommand PrepareSelectByCompositeKeyStatement()
		{
			var cmd = new SqlCommand();

			cmd.CommandText = "sync.usp_SyncSessionScopeLogSelectByCompositeKey";
			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.Clear();

			cmd.Parameters.Add("@SyncSessionLogGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@ScopeID", SqlDbType.NVarChar, 80);

			return cmd;
		}

		/// <summary>
		/// The prepare select by sync session statement.
		/// </summary>
		/// <returns>
		/// The <see cref="SqlCommand"/>.
		/// </returns>
		protected SqlCommand PrepareSelectBySyncSessionLogStatement()
		{
			var cmd = new SqlCommand();

			cmd.CommandText = "sync.usp_SyncSessionScopeLogSelectBySyncSessionLogGuid";
			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.Clear();

			cmd.Parameters.Add("@SyncSessionLogGuid", SqlDbType.UniqueIdentifier);

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

			cmd.CommandText = "sync.usp_SyncSessionScopeLogSave";
			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.Clear();

			cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@SyncSessionLogGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@ScopeID", SqlDbType.NVarChar, 80);
			cmd.Parameters.Add("@SiteTypeIndex", SqlDbType.BigInt);
			cmd.Parameters.Add("@SyncSessionStatusIndex", SqlDbType.BigInt);
			cmd.Parameters.Add("@SyncSessionStateIndex", SqlDbType.BigInt);
			cmd.Parameters.Add("@StartDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@EndDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@TableCount", SqlDbType.Int);
			cmd.Parameters.Add("@TableSuccessCount", SqlDbType.Int);
			cmd.Parameters.Add("@TableErrorCount", SqlDbType.Int);
			cmd.Parameters.Add("@TotalChangesCount", SqlDbType.Int);
			cmd.Parameters.Add("@TotalChangesAppliedCount", SqlDbType.Int);
			cmd.Parameters.Add("@TotalChangesFailedCount", SqlDbType.Int);
			cmd.Parameters.Add("@TotalChangesPendingCount", SqlDbType.Int);
			cmd.Parameters.Add("@TotalDeleteCount", SqlDbType.Int);
			cmd.Parameters.Add("@TotalInsertCount", SqlDbType.Int);
			cmd.Parameters.Add("@TotalUpdateCount", SqlDbType.Int);
			cmd.Parameters.Add("@BatchFileName", SqlDbType.NVarChar, 384);

			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);

			cmd.Parameters.Add("@NewRowGuid", SqlDbType.UniqueIdentifier).Direction = ParameterDirection.Output;

			return cmd;
		}

		/// <summary>
		/// Constructs an instance of a <see cref="SyncSessionScopeLogDO"/> data object from the passed in data row.
		/// </summary>
		/// <param name="pRow">
		/// Populated <see cref="DataRow"/> that represents an instance of the <see cref="SyncSessionScopeLogDO"/> to build.
		/// </param>
		/// <returns>
		/// A populated <see cref="SyncSessionScopeLogDO"/> instance based on the contents of the passed in <see cref="DataRow"/>.
		/// </returns>
		private SyncSessionScopeLogDO GetDataObjectFromDataRow(DataRow pRow)
		{
			SyncSessionScopeLogDO dataObject = new SyncSessionScopeLogDO();

			dataObject.IdentityGuid = DataObject.getValue<Guid>(pRow["SyncSessionScopeLogGuid"], Guid.Empty);
			dataObject.SyncSessionLogGuid = DataObject.getValue<Guid>(pRow["SyncSessionLogGuid"], Guid.Empty);
			dataObject.SiteGuid = DataObject.getValue<Guid>(pRow["SiteGuid"], Guid.Empty);
			dataObject.SyncScopeID = DataObject.getValue<string>(pRow["ScopeID"], string.Empty);

			long? indexValue = null;
				
			if (!DataObject.isNull(pRow["SiteTypeIndex"]))
			{
				indexValue = DataObject.getValue<long>(pRow["SiteTypeIndex"], (long)SYNCSITETYPE.REFERENCE);

				SYNCSITETYPE syncSiteType = SYNCSITETYPE.REFERENCE;
				if (Enum.TryParse(indexValue.ToString(), true, out syncSiteType))
				{
					dataObject.SiteTypeIndex = syncSiteType;
				}
			}
			else
			{
				dataObject.SiteTypeIndex = null;
			}

			indexValue = DataObject.getValue<long>(pRow["SyncSessionStatusIndex"], (long)SYNCSESSIONSTATUS.NEW);
			SYNCSESSIONSTATUS syncSessionStatus = SYNCSESSIONSTATUS.NEW;

			if (Enum.TryParse(indexValue.ToString(), true, out syncSessionStatus))
			{
				dataObject.SyncSessionStatusIndex = syncSessionStatus;
			}

			indexValue = DataObject.getValue<long>(pRow["SyncSessionStateIndex"], (long)SYNCSESSIONSTATE.INIT);
			SYNCSESSIONSTATE syncSessionState = SYNCSESSIONSTATE.INIT;

			if (Enum.TryParse(indexValue.ToString(), true, out syncSessionState))
			{
				dataObject.SyncSessionStateIndex = syncSessionState;
			}

			dataObject.StartDate = DataObject.getOptionalDateTimeOffset(pRow["StartDate"]);
			dataObject.EndDate = DataObject.getOptionalDateTimeOffset(pRow["EndDate"]);

			dataObject.TableCount = DataObject.getValue<int>(pRow["TableCount"], 0);
			dataObject.TableSuccessCount = DataObject.getValue<int>(pRow["TableSuccessCount"], 0);
			dataObject.TableErrorCount = DataObject.getValue<int>(pRow["TableErrorCount"], 0);
			dataObject.TotalChangesCount = DataObject.getValue<int>(pRow["TotalChangesCount"], 0);
			dataObject.TotalChangesAppliedCount = DataObject.getValue<int>(pRow["TotalChangesAppliedCount"], 0);
			dataObject.TotalChangesFailedCount = DataObject.getValue<int>(pRow["TotalChangesFailedCount"], 0);
			dataObject.TotalChangesPendingCount = DataObject.getValue<int>(pRow["TotalChangesPendingCount"], 0);
			dataObject.TotalDeleteCount = DataObject.getValue<int>(pRow["TotalDeleteCount"], 0);
			dataObject.TotalInsertCount = DataObject.getValue<int>(pRow["TotalInsertCount"], 0);
			dataObject.TotalUpdateCount = DataObject.getValue<int>(pRow["TotalUpdateCount"], 0);
			dataObject.BatchFileName = DataObject.getString(pRow["BatchFileName"]);

			dataObject.CreatedDate = DataObject.getValue<DateTimeOffset>(pRow["CreatedDate"], DateTimeOffset.Now);
			dataObject.CreatedBy = DataObject.getString(pRow["CreatedBy"]);
			dataObject.UpdatedDate = DataObject.getValue<DateTimeOffset>(pRow["UpdatedDate"], DateTimeOffset.Now);
			dataObject.UpdatedBy = DataObject.getString(pRow["UpdatedBy"]);

			return dataObject;
		}
		#endregion Private Support Methods
	}
}
