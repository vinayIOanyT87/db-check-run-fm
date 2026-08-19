// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncRecordConflictDBI.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for SyncRecordConflictDBI.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.InternalClasses.SyncClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.IO;
	using System.Linq;
	using System.Runtime.Serialization;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for SyncRecordConflictDBI.
	/// </summary>
	public class SyncRecordConflictDBI : SyncDBI
	{
		#region Attributes
		private SqlTransaction _Transaction = null;
		#endregion Attributes

		#region Properties
		public SqlTransaction Transaction
		{
			get
			{
				return (_Transaction);
			}
			set
			{
				_Transaction = value;
			}
		}
		#endregion Properties

		public SyncRecordConflictDBI(string user)
			: base(user)
		{
		}

		#region Public Data Access Methods

		public SyncRecordConflictDO Get(SecurityClass security, Guid? identityGuid)
		{
			DataSet ds = Load(security, identityGuid);

			if (ds.Tables[0].Rows.Count == 0)
			{
				return null;
			}

			DataRow row = ds.Tables[0].Rows[0];
			SyncRecordConflictDO syncResult = GetDataObjectFromDataRow(row);

			return (syncResult);
		}

		public SyncRecordConflictDO GetByTableAndEntityKey(
			SecurityClass security,
			string tableName,
			string entityKey,
			bool onlyUnresolved)
		{
			DataSet ds = this.LoadByTableAndEntityKey(security, tableName, entityKey, onlyUnresolved);

			if (ds.Tables[0].Rows.Count == 0)
			{
				return (null);
			}

			DataRow row = ds.Tables[0].Rows[0];
			SyncRecordConflictDO syncResult = GetDataObjectFromDataRow(row);

			return (syncResult);
		}

		public List<SyncRecordConflictDO> GetList(SecurityClass pSecurity, Guid? identityGuid)
		{
			List<SyncRecordConflictDO> syncResults = new List<SyncRecordConflictDO>();

			DataSet ds = this.Load(pSecurity, identityGuid);

			if (ds.Tables[0].Rows.Count == 0)
			{
				return (syncResults);
			}

			syncResults.AddRange(from DataRow row in ds.Tables[0].Rows select this.GetDataObjectFromDataRow(row));

			return (syncResults);
		}

		public List<SyncRecordConflictDO> GetSyncSessionLogList(SecurityClass pSecurity, Guid syncSessionLogGuid, Int64? maxRecords, Int64 startRowVersion)
		{
			List<SyncRecordConflictDO> syncResults = new List<SyncRecordConflictDO>();

			DataSet ds = LoadSyncSessionLogConflictList(pSecurity, syncSessionLogGuid, maxRecords, startRowVersion);

			if (ds.Tables[0].Rows.Count == 0)
			{
				return (syncResults);
			}

			syncResults.AddRange(from DataRow row in ds.Tables[0].Rows select this.GetDataObjectFromDataRow(row));

			return (syncResults);
		}

		public List<SyncRecordConflictDO> GetSyncSessionScopeLogList(SecurityClass pSecurity, Guid syncSessionScopeLogGuid)
		{
			List<SyncRecordConflictDO> syncResults = new List<SyncRecordConflictDO>();

			DataSet ds = LoadSyncSessionScopeLogConflictList(pSecurity, syncSessionScopeLogGuid);

			if (ds.Tables[0].Rows.Count == 0)
			{
				return (syncResults);
			}

			syncResults.AddRange(from DataRow row in ds.Tables[0].Rows select this.GetDataObjectFromDataRow(row));

			return (syncResults);
		}

		public List<SyncRecordConflictDO> GetUnresolvedList(SecurityClass security, Guid syncNodeGuid, Int64? maxRecords, Int64 startRowVersion)
		{
			List<SyncRecordConflictDO> syncResults = new List<SyncRecordConflictDO>();

			DataSet ds = LoadUnresolved(security, syncNodeGuid, maxRecords, startRowVersion);

			if (ds.Tables[0].Rows.Count == 0)
			{
				return (syncResults);
			}

			syncResults.AddRange(from DataRow row in ds.Tables[0].Rows select this.GetDataObjectFromDataRow(row));

			return (syncResults);
		}

		public SyncRecordConflictCountDO GetUnresolvedCount(SecurityClass security, Guid? syncNodeGuid)
		{

			DataSet ds = LoadUnresolvedCount(security, syncNodeGuid);

			if (ds.Tables[0].Rows.Count == 0)
			{
				return null;
			}

			var syncRecordConflictCount = new SyncRecordConflictCountDO();

			syncRecordConflictCount.Count = DataObject.getValue<int>(ds.Tables[0].Rows[0]["Count"],0);
			syncRecordConflictCount.OldestDate = DataObject.getValue<DateTimeOffset>(ds.Tables[0].Rows[0]["OldestDate"],DateTimeOffset.Now);

			return syncRecordConflictCount;
		}


		public List<SyncRecordConflictDO> GetByStatusList(
			SecurityClass pSecurity,
			SYNCCONFLICTRESOLUTIONSTATUS pConflictResolutionStatusIndex,
			Guid? sessionLogGuid)
		{
			List<SyncRecordConflictDO> syncResults = new List<SyncRecordConflictDO>();

			DataSet ds = LoadByStatus(pSecurity, pConflictResolutionStatusIndex, sessionLogGuid);

			if (ds.Tables[0].Rows.Count == 0)
			{
				return (syncResults);
			}

			syncResults.AddRange(from DataRow row in ds.Tables[0].Rows select this.GetDataObjectFromDataRow(row));

			return (syncResults);
		}

		/// <summary>
		/// Saves the passed in 
		/// <seealso cref="SyncRecordConflictDO"/>
		/// record.
		/// </summary>
		/// <param name="security">
		/// Contains security credentials
		/// </param>
		/// <param name="syncSessionScopeLog">
		/// The sync Session Detail.
		/// </param>
		/// <param name="dataObject">
		/// The p Data Object.
		/// </param>
		/// <returns>
		/// True if the warning level for drawdown is hit.  Otherwise, false
		/// </returns>
		/// <remarks>
		/// Utilizes a merge stored procedure to implement Insert/Update operations in a single call.
		/// </remarks>
		public bool Save(SecurityClass security, SyncSessionScopeLogDO syncSessionScopeLog, SyncRecordConflictDO dataObject)
		{
			// Save the dataobject using a merge implementation
			Merge(security, syncSessionScopeLog, dataObject);

			return true;
		}

		/// <summary>
		/// Deletes the passed in <seealso cref="SyncRecordConflictDO"/> record.
		/// </summary>
		/// <param name="security">Contains security credentials</param>
		/// <param name="dataObject">The synchronization conflict record information to delete.</param>
		/// <param name="purge">Permanently delete the specified synchronization conflict record.</param>
		/// <returns>True if the warning level for drawdown is hit.  Otherwise, false</returns>
		public bool Delete(SecurityClass security, SyncRecordConflictDO dataObject, bool purge)
		{
			try
			{
				Delete(security, dataObject);

				return (true);
			}
			catch (Exception eX)
			{
				throw (eX);
			}
		}

		#endregion Public Data Access Methods

		#region Private Persistence Methods

		/// <summary>
		/// Returns a DataSet containing the SyncRecordConflict record containing the specified Primary Key.
		/// </summary>
		/// <param name="security">Contains security credentials</param>
		/// <param name="identityGuid">Primary key value for the SyncRecordConflict record to load.</param>
		private DataSet Load(SecurityClass security, System.Nullable<Guid> identityGuid)
		{
			DataSet ds = null;

			using (var cmd = this.PrepareSelectStatement())
			{
				SqlParameterCollection parms = cmd.Parameters;
				parms["@IdentityGuid"].Value = this.SetOptionalValue<Guid>(identityGuid);

				if (null != _Transaction)
				{
					ds = this.ConsolidatedDA.GetDataSet(cmd, security, _Transaction);
				}
				else
				{
					ds = this.ConsolidatedDA.GetDataSet(cmd, security);
				}
			}

			return (ds);
		}

		/// <summary>
		/// Returns a DataSet containing the SyncRecordConflict records associated with the specified syncSessionLog record.
		/// </summary>
		/// <param name="security">Contains security credentials</param>
		/// <param name="syncSessionLogGuid">Primary key value for the SyncRecordConflict record to load.</param>
		private DataSet LoadSyncSessionLogConflictList(SecurityClass security, Guid syncSessionLogGuid, Int64? maxRecords, Int64 startRowVersion)
		{
			DataSet ds = null;

			using (var cmd = this.PrepareSelectBySyncSessionLogStatement())
			{
				SqlParameterCollection parms = cmd.Parameters;
				parms["@SyncSessionLogGuid"].Value = syncSessionLogGuid;
				parms["@MaxRecords"].Value = maxRecords.HasValue ? maxRecords.Value : Int64.MaxValue;
				parms["@StartRowVersion"].Value = startRowVersion;

				if (null != _Transaction)
				{
					ds = this.ConsolidatedDA.GetDataSet(cmd, security, _Transaction);
				}
				else
				{
					ds = this.ConsolidatedDA.GetDataSet(cmd, security);
				}
			}

			return (ds);
		}

		/// <summary>
		/// Returns a DataSet containing the SyncRecordConflict records associated with the specified syncSessionScopeLog record.
		/// </summary>
		/// <param name="security">Contains security credentials</param>
		/// <param name="syncSessionScopeLogGuid">Primary key value for the SyncRecordConflict record to load.</param>
		private DataSet LoadSyncSessionScopeLogConflictList(SecurityClass security, Guid syncSessionScopeLogGuid)
		{
			DataSet ds = null;

			using (var cmd = this.PrepareSelectBySyncSessionScopeLogStatement())
			{
				SqlParameterCollection parms = cmd.Parameters;
				parms["@SyncSessionScopeLogGuid"].Value = syncSessionScopeLogGuid;

				if (null != _Transaction)
				{
					ds = this.ConsolidatedDA.GetDataSet(cmd, security, _Transaction);
				}
				else
				{
					ds = this.ConsolidatedDA.GetDataSet(cmd, security);
				}
			}

			return (ds);
		}

		/// <summary>
		/// Returns a DataSet containing the SyncRecordConflict records that have a resolution status not set to Cleared.
		/// </summary>
		/// <param name="security">Contains security credentials</param>
		/// <param name="syncNodeGuid">The sync node.</param>
		/// <returns></returns>
		private DataSet LoadUnresolved(SecurityClass security, Guid? syncNodeGuid, Int64? maxRecords, Int64 startRowVersion)
		{
			DataSet ds = null;

			using (var cmd = this.PrepareSelectUnresolvedStatement())
			{
				SqlParameterCollection parms = cmd.Parameters;
				parms["@SyncNodeGuid"].Value = this.SetOptionalValue<Guid>(syncNodeGuid == Guid.Empty ? null : syncNodeGuid);
				parms["@MaxRecords"].Value = maxRecords.HasValue ? maxRecords.Value : Int64.MaxValue;
				parms["@StartRowVersion"].Value = startRowVersion;

				if (null != _Transaction)
				{
					ds = this.ConsolidatedDA.GetDataSet(cmd, security, _Transaction);
				}
				else
				{
					ds = this.ConsolidatedDA.GetDataSet(cmd, security);
				}
			}

			return (ds);
		}

		/// <summary>
		/// Returns a DataSet containing the SyncRecordConflictCount that have a resolution status not set to Cleared.
		/// </summary>
		/// <param name="security">Contains security credentials</param>
		/// <param name="syncNodeGuid">The sync node.</param>
		/// <returns></returns>
		private DataSet LoadUnresolvedCount(SecurityClass security, Guid? syncNodeGuid)
		{
			DataSet ds = null;

			using (var cmd = this.PrepareSelectUnresolvedCountStatement())
			{
				SqlParameterCollection parms = cmd.Parameters;
				parms["@SyncNodeGuid"].Value = this.SetOptionalValue<Guid>(syncNodeGuid == Guid.Empty ? null : syncNodeGuid);

				if (null != _Transaction)
				{
					ds = this.ConsolidatedDA.GetDataSet(cmd, security, _Transaction);
				}
				else
				{
					ds = this.ConsolidatedDA.GetDataSet(cmd, security);
				}
			}

			return (ds);
		}


		/// <summary>
		/// Returns a DataSet containing the SyncRecordConflict records that have a resolution status not set to Cleared.
		/// </summary>
		/// <param name="security">Contains security credentials</param>
		/// <param name="conflictResolutionStatusIndex">The conflict resolution status of the synchronization conflict records that should be loaded.</param>
		/// <param name="sessionLogGuid">The session log unique identifier.</param>
		/// <returns></returns>
		private DataSet LoadByStatus(
			SecurityClass security,
			SYNCCONFLICTRESOLUTIONSTATUS conflictResolutionStatusIndex,
			Guid? sessionLogGuid)
		{
			DataSet ds = null;

			using (var cmd = this.PrepareSelectByStatusStatement())
			{
				SqlParameterCollection parms = cmd.Parameters;
				parms["@SyncSessionLogGuid"].Value = this.SetOptionalValue<Guid>(sessionLogGuid);
				parms["@SyncConflictResolutionStatusIndex"].Value = (int)conflictResolutionStatusIndex;

				if (null != _Transaction)
				{
					ds = this.ConsolidatedDA.GetDataSet(cmd, security, _Transaction);
				}
				else
				{
					ds = this.ConsolidatedDA.GetDataSet(cmd, security);
				}
			}

			return (ds);
		}

		/// <summary>
		/// Returns a DataSet containing the SyncRecordConflict records for the specified Entity Key within the specified TableName
		/// </summary>
		/// <param name="security">
		/// Contains security credentials
		/// </param>
		/// <param name="tableName">
		/// The name of the table (basically entity type) to restrict the entity key to.
		/// </param>
		/// <param name="pEntityKey">
		/// The identity guid of the record to load the conflict record for.
		/// </param>
		/// <param name="onlyUnresolved">
		/// Only look for conflict records that have not been resolved.
		/// </param>
		private DataSet LoadByTableAndEntityKey(
			SecurityClass security,
			string tableName,
			string pEntityKey,
			bool onlyUnresolved)
		{
			DataSet ds = null;

			using (var cmd = this.PrepareSelectByTableAndEntityKeyStatement())
			{
				SqlParameterCollection parms = cmd.Parameters;
				parms["@TableName"].Value = !string.IsNullOrEmpty(tableName) ? tableName : (object)DBNull.Value;
				parms["@RecordKey"].Value = !string.IsNullOrEmpty(pEntityKey) ? pEntityKey : (object)DBNull.Value;
				parms["@OnlyUnresolved"].Value = onlyUnresolved;

				if (null != _Transaction)
				{
					ds = this.ConsolidatedDA.GetDataSet(cmd, security, _Transaction);
				}
				else
				{
					ds = this.ConsolidatedDA.GetDataSet(cmd, security);
				}
			}

			return (ds);
		}

		/// <summary>
		/// Merges current <seealso cref="SyncRecordConflictDO"/> record with an existing record.  If the record doesn't currently
		/// exist, a new record is inserted.
		/// </summary>
		/// <param name="security">Contains security credentials</param>
		/// <param name="syncSessionScopeLog"></param>
		/// <param name="dataObject">The synchronization record conflict information to update or insert.</param>
		private void Merge(SecurityClass security, SyncSessionScopeLogDO syncSessionScopeLog, SyncRecordConflictDO dataObject)
		{
			using (var cmd = this.PrepareUpsertStatement())
			{
				SqlParameterCollection parms = cmd.Parameters;
				parms["@IdentityGuid"].Value = (dataObject.IdentityGuid != Guid.Empty)
					? dataObject.IdentityGuid
					: (object)DBNull.Value;
				parms["@TargetNodeGuid"].Value = dataObject.TargetNodeGuid;
				parms["@TargetNodeName"].Value = dataObject.TargetNodeName;
				parms["@TableName"].Value = dataObject.TableName;
				parms["@RecordKey"].Value = dataObject.RecordKey;
				parms["@RecordRowVersion"].Value = dataObject.RecordRowVersion;
				parms["@ReSyncAnchorMin"].Value = dataObject.ReSyncAnchorMin;
				parms["@ReSyncAnchorMax"].Value = dataObject.ReSyncAnchorMax;
				parms["@SyncConflictTypeIndex"].Value = (long)dataObject.SyncConflictTypeIndex;
				parms["@SyncConflictResolutionStatusIndex"].Value = (long)dataObject.SyncConflictResolutionStatusIndex;
				parms["@ResolvedDate"].Value = this.SetOptionalValue(dataObject.ResolvedDate);
				parms["@ResolvedBy"].Value = this.SetOptionalValue(dataObject.ResolvedBy);
				if (syncSessionScopeLog != null)
				{
					parms["@SyncSessionScopeLogGuid"].Value = syncSessionScopeLog.IdentityGuid;
				}
				parms["@CreatedBy"].Value = security.UserID;
				parms["@UpdatedBy"].Value = security.UserID;
				parms["@ConflictDescription"].Value = dataObject.ConflictDescription;
				parms["@CommandText"].Value = dataObject.CommandText;
				parms["@CommandType"].Value = dataObject.CommandType;
				parms["@Retrys"].Value = dataObject.Retrys;

				var knownTypeList = new List<Type>();
				knownTypeList.Add(typeof(DateTimeOffset));
				knownTypeList.Add(typeof(DBNull));
				var stream = new MemoryStream();
				var parameterSerializer = new DataContractSerializer(typeof(Dictionary<string, object>), knownTypeList);
				parameterSerializer.WriteObject(stream, dataObject.Parameters);

				parms["@Parameters"].Value = stream.ToArray();

				if (null != _Transaction)
				{
					this.ConsolidatedDA.ExecuteQueryWithoutSessionContext(security, cmd, _Transaction); 
				}
				else
				{
					this.ConsolidatedDA.ExecuteQueryWithoutSessionContext(security, cmd);
				}

				System.Nullable<Guid> retIdentityGuid = this.GetOutputValue<Guid>(parms["@NewRowGuid"], dataObject.IdentityGuid);

				if (retIdentityGuid.HasValue && retIdentityGuid.Value != dataObject.IdentityGuid) dataObject.IdentityGuid = retIdentityGuid.Value;
			}
		}

		/// <summary>
		/// Removes the specified <seealso cref="SyncRecordConflictDO"/> record.
		/// </summary>
		/// <param name="security">Contains security credentials</param>
		/// <param name="dataObject">The synchronization record conflict information to delete.</param>
		private void Delete(SecurityClass security, SyncRecordConflictDO dataObject)
		{
			using (var cmd = this.PrepareDeleteStatement())
			{
				SqlParameterCollection parms = null;
				parms = cmd.Parameters;
				parms["@IdentityGuid"].Value = dataObject.IdentityGuid;

				if (null != _Transaction)
				{
					this.ConsolidatedDA.ExecuteQueryWithoutSessionContext(security, cmd, _Transaction);
				}
				else
				{
					this.ConsolidatedDA.ExecuteQueryWithoutSessionContext(security, cmd);
				}
			}
		}

		#endregion Private Persistence Methods

		#region Override Implementations for Prepare Methods

		protected override SqlCommand PrepareUpsertStatement()
		{
			return (this.CreateMergeStatement());
		}

		protected override SqlCommand PrepareSelectStatement()
		{
			var cmd = new SqlCommand();

			cmd.CommandText = "sync.usp_SyncRecordConflictSelect";
			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.Clear();

			cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);

			return (cmd);
		}

		protected override SqlCommand PrepareInsertStatement()
		{
			return (this.CreateMergeStatement());
		}

		protected override SqlCommand PrepareUpdateStatement()
		{
			return (this.CreateMergeStatement());
		}

		protected override SqlCommand PrepareDeleteStatement()
		{
			var cmd = new SqlCommand();

			cmd.CommandText = "sync.usp_SyncRecordConflictDeleteByGuid";
			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.Clear();

			cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);

			return (cmd);
		}

		#endregion Override Implementations for Prepare Methods

		#region Private Support Methods

		private SqlCommand PrepareSelectBySyncSessionLogStatement()
		{
			var cmd = new SqlCommand
					{
						CommandText = "sync.usp_SyncRecordConflictSelectBySyncSessionLog",
						CommandType = CommandType.StoredProcedure
					};


			cmd.Parameters.Clear();

			cmd.Parameters.Add("@SyncSessionLogGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@MaxRecords", SqlDbType.BigInt);
			cmd.Parameters.Add("@StartRowVersion", SqlDbType.BigInt);

			return (cmd);
		}

		private SqlCommand PrepareSelectBySyncSessionScopeLogStatement()
		{
			var cmd = new SqlCommand
					{
						CommandText = "sync.usp_SyncRecordConflictSelectBySyncSessionScopeLog",
						CommandType = CommandType.StoredProcedure
					};


			cmd.Parameters.Clear();

			cmd.Parameters.Add("@SyncSessionScopeLogGuid", SqlDbType.UniqueIdentifier);

			return (cmd);
		}

		private SqlCommand PrepareSelectUnresolvedStatement()
		{
			var cmd = new SqlCommand
					{
						CommandText = "sync.usp_SyncRecordConflictSelectUnresolved",
						CommandType = CommandType.StoredProcedure
					};


			cmd.Parameters.Clear();
			cmd.Parameters.Add("@SyncNodeGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@MaxRecords", SqlDbType.BigInt);
			cmd.Parameters.Add("@StartRowVersion", SqlDbType.BigInt);

			return (cmd);
		}

		private SqlCommand PrepareSelectUnresolvedCountStatement()
		{
			var cmd = new SqlCommand
			{
				CommandText = "sync.usp_SyncRecordConflictSelectUnresolvedCount",
				CommandType = CommandType.StoredProcedure
			};


			cmd.Parameters.Clear();
			cmd.Parameters.Add("@SyncNodeGuid", SqlDbType.UniqueIdentifier);

			return (cmd);
		}



		private SqlCommand PrepareSelectByStatusStatement()
		{
			var cmd = new SqlCommand
					{
						CommandText = "sync.usp_SyncRecordConflictSelectByStatus",
						CommandType = CommandType.StoredProcedure
					};

			cmd.Parameters.Clear();

			cmd.Parameters.Add("@SyncSessionLogGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@SyncConflictResolutionStatusIndex", SqlDbType.BigInt);

			return (cmd);
		}

		private SqlCommand PrepareSelectByTableAndEntityKeyStatement()
		{
			var cmd = new SqlCommand
					{
						CommandText = "sync.usp_SyncRecordConflictSelectByTableAndEntityKey",
						CommandType = CommandType.StoredProcedure
					};


			cmd.Parameters.Clear();

			cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 256);
			cmd.Parameters.Add("@RecordKey", SqlDbType.NVarChar, 64);
			cmd.Parameters.Add("@OnlyUnresolved", SqlDbType.Bit);

			return (cmd);
		}

		private SqlCommand CreateMergeStatement()
		{
			var cmd = new SqlCommand
					{
						CommandText = "sync.usp_SyncRecordConflictSave",
						CommandType = CommandType.StoredProcedure
					};

			cmd.Parameters.Clear();

			cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@TargetNodeGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@TargetNodeName", SqlDbType.NVarChar, 256);
			cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 256);
			cmd.Parameters.Add("@RecordKey", SqlDbType.NVarChar, 64);
			cmd.Parameters.Add("@RecordRowVersion", SqlDbType.BigInt);
			cmd.Parameters.Add("@ReSyncAnchorMin", SqlDbType.BigInt);
			cmd.Parameters.Add("@ReSyncAnchorMax", SqlDbType.BigInt);
			cmd.Parameters.Add("@SyncConflictTypeIndex", SqlDbType.BigInt);
			cmd.Parameters.Add("@SyncConflictResolutionStatusIndex", SqlDbType.BigInt);
			cmd.Parameters.Add("@ResolvedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@ResolvedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@SyncSessionScopeLogGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@ConflictDescription", SqlDbType.NVarChar, 4000);
			cmd.Parameters.Add("@CommandText", SqlDbType.NVarChar, 4000);
			cmd.Parameters.Add("@CommandType", SqlDbType.BigInt);
			cmd.Parameters.Add("@Parameters", SqlDbType.VarBinary);
			cmd.Parameters.Add("@Retrys", SqlDbType.Int);
			cmd.Parameters.Add("@NewRowGuid", SqlDbType.UniqueIdentifier).Direction = ParameterDirection.Output;

			return (cmd);
		}

		private SyncRecordConflictDO GetDataObjectFromDataRow(DataRow row)
		{
			SyncRecordConflictDO dataObject = new SyncRecordConflictDO
												{
												IdentityGuid = DataObject.getValue<Guid>(row["SyncRecordConflictGuid"], Guid.Empty),
												TargetNodeGuid = DataObject.getValue<Guid>(row["TargetNodeGuid"], Guid.Empty),
												TargetNodeName = DataObject.getString(row["TargetNodeName"]),
												TableName = DataObject.getString(row["TableName"]),
												RecordKey = DataObject.getString(row["RecordKey"]),
												RecordRowVersion = DataObject.getLong(row["RecordRowVersion"]),
												ReSyncAnchorMin = DataObject.getLong(row["ReSyncAnchorMin"]),
												ReSyncAnchorMax = DataObject.getLong(row["ReSyncAnchorMax"]),
												SyncConflictTypeIndex =
													(SYNCCONFLICTTYPE)
													DataObject.getValue<long>(row["SyncConflictTypeIndex"], (int)SYNCCONFLICTTYPE.UNKNOWN),
												SyncConflictResolutionStatusIndex =
													(SYNCCONFLICTRESOLUTIONSTATUS)
													DataObject.getValue<long>(
														row["SyncConflictResolutionStatusIndex"],
														(int)SYNCCONFLICTRESOLUTIONSTATUS.PENDING),
												ResolvedDate = DataObject.getOptionalDateTimeOffset(row["ResolvedDate"]),
												ResolvedBy = DataObject.getString(row["ResolvedBy"]),
												CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now),
												CreatedBy = DataObject.getString(row["CreatedBy"]),
												UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], DateTimeOffset.Now),
												UpdatedBy = DataObject.getString(row["UpdatedBy"]),
												ConflictDescription = DataObject.getString(row["ConflictDescription"])
											};

			if (row.Table.Columns.Contains("CommandText") && !row.IsNull("CommandText"))
			{
				dataObject.CommandText = DataObject.getString(row["CommandText"]);
			}
			if (row.Table.Columns.Contains("CommandType") && !row.IsNull("CommandType"))
			{
				dataObject.CommandType = (CommandType)DataObject.getLong(row["CommandType"]);
			}
			if (row.Table.Columns.Contains("Retrys") && !row.IsNull("Retrys"))
			{
				dataObject.Retrys = DataObject.getInt(row["Retrys"]);
			}
			if (row.Table.Columns.Contains("Parameters") && !row.IsNull("Parameters"))
			{
				byte[] parameterArray = DataObject.getOptionalVarBinary(row["Parameters"]);
				var knownTypeList = new List<Type> { typeof(DateTimeOffset), typeof(DBNull) };
				var parameterSerializer = new DataContractSerializer(typeof(Dictionary<string, object>), knownTypeList);
				var stream = new MemoryStream(parameterArray);
				dataObject.Parameters = parameterSerializer.ReadObject(stream) as Dictionary<string, object>;
			}

			return (dataObject);
		}
		#endregion Private Support Methods
	}
}
