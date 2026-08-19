// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncTableToScopeMapDBI.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for SyncTableToScopeMapDBI.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.InternalClasses.SyncClasses
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    using FMBusinessObjects.DataObjects;

    /// <summary>
    /// Summary description for SyncTableToScopeMapDBI.
    /// </summary>
    public class SyncTableToScopeMapDBI : SyncDBI
    {
        #region Attributes
        #endregion Attributes

        public SyncTableToScopeMapDBI(string user)
            : base(user)
        {
        }

        #region Public Data Access Methods
        public SyncTableToScopeMapCollection GetList(SecurityClass pSecurity, Guid pSyncScopeGuid)
        {
            SyncTableToScopeMapCollection syncTabletoScopeMaps = new SyncTableToScopeMapCollection();

            DataSet ds = Load(pSecurity, pSyncScopeGuid, null, null);

            if (ds.Tables[0].Rows.Count == 0)
                return (syncTabletoScopeMaps);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                SyncTableToScopeMapDO syncTabletoScopeMap = GetDataObjectFromDataRow(row);
                syncTabletoScopeMaps.Add(syncTabletoScopeMap);
            }

            return (syncTabletoScopeMaps);
        }

        public SyncTableToScopeMapCollection GetListForProfileByTableNames(SecurityClass pSecurity, Guid pProfileGuid, string pTableList)
        {
            SyncTableToScopeMapCollection syncTabletoScopeMaps = new SyncTableToScopeMapCollection();

            DataSet ds = LoadFilteredListByProfile(pSecurity, pProfileGuid, pTableList);

            if (ds.Tables[0].Rows.Count == 0)
                return (syncTabletoScopeMaps);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                SyncTableToScopeMapDO syncTabletoScopeMap = GetDataObjectFromDataRow(row);
                syncTabletoScopeMaps.Add(syncTabletoScopeMap);
            }

            return (syncTabletoScopeMaps);
        }
        
        public SyncTableToScopeMapDO Get(SecurityClass pSecurity, System.Nullable<Guid> pIdentityGuid, string pID)
        {
            DataSet ds = Load(pSecurity, null, pIdentityGuid, pID);

            if (ds.Tables[0].Rows.Count == 0)
                return null;

            DataRow row = ds.Tables[0].Rows[0];
            SyncTableToScopeMapDO syncTabletoScopeMap = GetDataObjectFromDataRow(row);

            return (syncTabletoScopeMap);
        }
        /// <summary>
        /// Saves the passed in <seealso cref="SyncTableToScopeMapDO"/> record.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="dataObject">The synchronization dependency group information to persist.</param>
        /// <returns>True if the warning level for drawdown is hit.  Otherwise, false</returns>
        /// <remarks>Utilizes a merge stored procedure to implement Insert/Update operations in a single call.</remarks>
        public bool Save(SecurityClass security, SyncTableToScopeMapDO dataObject)
        {
            // Save the dataobject using a merge implementation
            Merge(security, dataObject);

            return true;
        }

        /// <summary>
        /// Deletes the passed in <seealso cref="SyncProfileDO"/> record.
        /// </summary>
        /// <param name="pSecurity">Contains pSecurity credentials</param>
        /// <param name="dataObject">The synchronization dependency group information to delete.</param>
        /// <returns>True if the warning level for drawdown is hit.  Otherwise, false</returns>
        public bool Delete(SecurityClass pSecurity, SyncTableToScopeMapDO dataObject, bool pPurge)
        {
            try
            {
                // Save the dataobject using a merge implementation
                Delete(pSecurity, dataObject);

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
        /// Merges current <seealso cref="SyncProfileDO"/> record with an existing record.  If the record doesn't currently
        /// exist, a new record is inserted.
        /// </summary>
        /// <param name="pSecurity">Contains pSecurity credentials</param>
        /// <param name="pSyncScopeGuid">Specify the GUID of the Synchronization Scope to load all Table to Scope Mapping records for that scope.</param>
        /// <param name="pIdentityGuid">Optional parameter that specifies a specific Table To Scope Mapping Record.</param>
        /// <param name="pID">Optional parameter that specifies a specific Table to Scope Mapping ID.</param>
        private DataSet Load(SecurityClass pSecurity, System.Nullable<Guid> pSyncScopeGuid, System.Nullable<Guid> pIdentityGuid, string pID)
        {
            DataSet ds = null;

            using (var cmd = PrepareSelectStatement())
            {
                SqlParameterCollection parms = cmd.Parameters;
                parms["@SyncScopeGuid"].Value = this.SetOptionalValue<Guid>(pSyncScopeGuid);
                parms["@IdentityGuid"].Value = this.SetOptionalValue<Guid>(pIdentityGuid);
                parms["@ID"].Value = this.SetOptionalValue<string>(pID);

                ds = ConsolidatedDA.GetDataSet(cmd, pSecurity);
            }

            return (ds);
        }

        /// <summary>
        /// Merges current <seealso cref="SyncProfileDO"/> record with an existing record.  If the record doesn't currently
        /// exist, a new record is inserted.
        /// </summary>
        /// <param name="pSecurity">Contains pSecurity credentials</param>
        /// <param name="pSyncProfileGuid">The GUID of the sync profile to restrict the table search to.</param>
        /// <param name="pTableList">A comma separated list of table names.</param>
        private DataSet LoadFilteredListByProfile(SecurityClass pSecurity, Guid pSyncProfileGuid, string pTableList)
        {
            DataSet ds = null;

            using (var cmd = PrepareFilteredSelectStatement())
            {
                SqlParameterCollection parms = cmd.Parameters;
                parms["@SyncProfileGuid"].Value = pSyncProfileGuid;
                parms["@TableNames"].Value = pTableList;

                ds = ConsolidatedDA.GetDataSet(cmd, pSecurity);
            }

            return (ds);
        }

        /// <summary>
        /// Merges current <seealso cref="SyncTableToScopeMapDO"/> record with an existing record.  If the record doesn't currently
        /// exist, a new record is inserted.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="dataObject">The synchronization dependency group information to update or insert.</param>
        private void Merge(SecurityClass security, SyncTableToScopeMapDO dataObject)
        {
            using (var cmd = PrepareUpsertStatement())
            {
                SqlParameterCollection parms = cmd.Parameters;
                parms["@IdentityGuid"].Value = dataObject.IdentityGuid;
                parms["@ID"].Value = dataObject.ID;
                parms["@SyncScopeGuid"].Value = dataObject.SyncScopeGuid;
                parms["@SyncTableGuid"].Value = dataObject.SyncTableGuid;
                parms["@SyncOrder"].Value = dataObject.SyncOrder;
                parms["@SyncDirection"].Value = (int)dataObject.SyncDirection;
                parms["@MaxBatchSegmentRowCount"].Value = this.SetOptionalValue<int>(dataObject.MaxBatchSegmentRowCount);
                parms["@MaxTransferSegmentKB"].Value = this.SetOptionalValue<int>(dataObject.MaxTransferSegmentKB);
                parms["@AdditionalFilterJoinClause"].Value = this.SetOptionalValue<string>(dataObject.AdditionalFilterJoinClause);
                parms["@AdditionalFilterWhereClause"].Value = this.SetOptionalValue<string>(dataObject.AdditionalFilterWhereClause);
                parms["@ClientTableNameOverride"].Value = this.SetOptionalValue<string>(dataObject.ClientTableNameOverride);
                parms["@CreatedBy"].Value = security.UserID;
                parms["@UpdatedBy"].Value = security.UserID;
				parms["@FirstTimeSyncOption"].Value = this.SetOptionalValue<int>(dataObject.FirstTimeSyncOption);

				ConsolidatedDA.ExecuteQueryWithoutSessionContext(security, cmd);

                System.Nullable<Guid> retIdentityGuid = this.GetOutputValue<Guid>(parms["@NewRowGuid"], dataObject.IdentityGuid);

                if (retIdentityGuid.HasValue && retIdentityGuid.Value != dataObject.IdentityGuid)
                    dataObject.IdentityGuid = retIdentityGuid.Value;
            }
        }

        /// <summary>
        /// Removes the specified <seealso cref="SyncTableToScopeMapDO"/> record.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="dataObject">The synchronization dependency group information to persist.</param>
        private void Delete(SecurityClass security, SyncTableToScopeMapDO dataObject)
        {
            using (var cmd = PrepareDeleteStatement())
            {
                SqlParameterCollection parms = null;
                parms = cmd.Parameters;
                parms["@IdentityGuid"].Value = dataObject.IdentityGuid;

                ConsolidatedDA.ExecuteQueryWithoutSessionContext(security, cmd);
            }
        }
        #endregion Private Persistence Methods

        #region Override Implementations for Prepare Methods
        protected override SqlCommand PrepareUpsertStatement()
        {
            return (CreateMergeStatement());
        }
        protected override SqlCommand PrepareSelectStatement()
        {
            var cmd = new SqlCommand();

            cmd.CommandText = "sync.usp_SyncTableToScopeMapSelect";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();

            cmd.Parameters.Add("@SyncScopeGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 80);

            return (cmd);
        }
        protected override SqlCommand PrepareInsertStatement()
        {
            return (CreateMergeStatement());
        }
        protected override SqlCommand PrepareUpdateStatement()
        {
            return (CreateMergeStatement());
        }
        protected override SqlCommand PrepareDeleteStatement()
        {
            var cmd = new SqlCommand();

            cmd.CommandText = "sync.usp_SyncTableToScopeMapDeleteByGuid";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();

            cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);

            return (cmd);
        }
        #endregion Override Implementations for Prepare Methods

        #region Private Support Methods
        private SqlCommand CreateMergeStatement()
        {
            var cmd = new SqlCommand();

            cmd.CommandText = "sync.usp_SyncTableToScopeMapSave";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();

            cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 80);
            cmd.Parameters.Add("@SyncScopeGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@SyncTableGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@SyncOrder", SqlDbType.Int);
            cmd.Parameters.Add("@SyncDirection", SqlDbType.Int);
            cmd.Parameters.Add("@MaxBatchSegmentRowCount", SqlDbType.Int);
            cmd.Parameters.Add("@MaxTransferSegmentKB", SqlDbType.Int);
            cmd.Parameters.Add("@AdditionalFilterJoinClause", SqlDbType.NVarChar, 1024);
            cmd.Parameters.Add("@AdditionalFilterWhereClause", SqlDbType.NVarChar, 1024);
            cmd.Parameters.Add("@ClientTableNameOverride", SqlDbType.NVarChar, 1024);
            cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
            cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@FirstTimeSyncOption", SqlDbType.Int);

			cmd.Parameters.Add("@NewRowGuid", SqlDbType.UniqueIdentifier).Direction = ParameterDirection.Output;

            return (cmd);
        }

        private SqlCommand PrepareFilteredSelectStatement()
        {
            var cmd = new SqlCommand();

            cmd.CommandText = "sync.usp_SyncTableToScopeMapSelectByTableListForProfile";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();

            cmd.Parameters.Add("@SyncProfileGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@TableNames", SqlDbType.NVarChar, -1);

            return (cmd);
        }

        private SyncTableToScopeMapDO GetDataObjectFromDataRow(DataRow pRow)
        {
            SyncTableToScopeMapDO syncTableToScopeMap = new SyncTableToScopeMapDO();

            syncTableToScopeMap.IdentityGuid = DataObject.getValue<Guid>(pRow["SyncTableToScopeMapGuid"], Guid.Empty);
            syncTableToScopeMap.ID = DataObject.getString(pRow["ID"]);
            syncTableToScopeMap.SyncScopeGuid = DataObject.getValue<Guid>(pRow["SyncScopeGuid"], Guid.Empty);
            syncTableToScopeMap.SyncTableGuid = DataObject.getValue<Guid>(pRow["SyncTableGuid"], Guid.Empty);
            syncTableToScopeMap.SyncOrder = DataObject.getInt(pRow["SyncOrder"]);
            syncTableToScopeMap.SyncDirection = (SYNCDIRECTION)DataObject.getValue<int>(pRow["SyncDirection"], (int)SYNCDIRECTION.DOWNLOADONLY);
            syncTableToScopeMap.MaxBatchSegmentRowCount = DataObject.getValue<int>(pRow["MaxBatchSegmentRowCount"], 0);
            syncTableToScopeMap.MaxTransferSegmentKB = DataObject.getValue<int>(pRow["MaxTransferSegmentKB"], 0);
            syncTableToScopeMap.AdditionalFilterJoinClause = DataObject.getString(pRow["AdditionalFilterJoinClause"]);
            syncTableToScopeMap.AdditionalFilterWhereClause = DataObject.getString(pRow["AdditionalFilterWhereClause"]);
            syncTableToScopeMap.CreatedDate = DataObject.getValue<DateTimeOffset>(pRow["CreatedDate"], DateTimeOffset.Now);
            syncTableToScopeMap.CreatedBy = DataObject.getString(pRow["CreatedBy"]);
            syncTableToScopeMap.UpdatedDate = DataObject.getValue<DateTimeOffset>(pRow["UpdatedDate"], DateTimeOffset.Now);
            syncTableToScopeMap.UpdatedBy = DataObject.getString(pRow["UpdatedBy"]);
			syncTableToScopeMap.FirstTimeSyncOption = DataObject.getValue<int>(pRow["FirstTimeSyncOption"], 0);

			return (syncTableToScopeMap);
        }
        #endregion Private Support Methods
    }
}
