namespace FMBusinessServices.InternalClasses.SyncClasses
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    using FMBusinessObjects.DataObjects;

    /// <summary>
    /// Summary description for SyncTableToScopeMapCommandDBI.
    /// </summary>
    public class SyncTableToScopeMapCommandDBI : SyncDBI
    {
        #region Attributes
        #endregion Attributes

        public SyncTableToScopeMapCommandDBI(string user)
            : base(user)
        {
        }

        #region Public Data Access Methods
        public SyncTableToScopeMapCommandDO Get(SecurityClass pSecurity, System.Nullable<Guid> pSyncTableToScopeMapGuid, System.Nullable<Guid> pIdentityGuid)
        {
            DataSet ds = Load(pSecurity, pSyncTableToScopeMapGuid, pIdentityGuid);

            if (ds.Tables[0].Rows.Count == 0)
                return null;

            DataRow row = ds.Tables[0].Rows[0];
            SyncTableToScopeMapCommandDO syncCommand = GetDataObjectFromDataRow(row);

            return (syncCommand);
        }
        /// <summary>
        /// Saves the passed in <seealso cref="SyncTableToScopeMapCommandDO"/> record.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="dataObject">The synchronization dependency group information to persist.</param>
        /// <returns>True if the warning level for drawdown is hit.  Otherwise, false</returns>
        /// <remarks>Utilizes a merge stored procedure to implement Insert/Update operations in a single call.</remarks>
        public bool Save(SecurityClass security, SyncTableToScopeMapCommandDO dataObject)
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
        public bool Delete(SecurityClass pSecurity, SyncTableToScopeMapCommandDO dataObject, bool pPurge)
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
        /// <param name="dataObject">The synchronization dependency group information to update or insert.</param>
        private DataSet Load(SecurityClass pSecurity, System.Nullable<Guid> pSyncTableToScopeMapGuid, System.Nullable<Guid> pIdentityGuid)
        {
            DataSet ds = null;

            using (var cmd = PrepareSelectStatement())
            {
                SqlParameterCollection parms = cmd.Parameters;
                parms["@SyncTableToScopeMapGuid"].Value = this.SetOptionalValue<Guid>(pSyncTableToScopeMapGuid);
                parms["@IdentityGuid"].Value = this.SetOptionalValue<Guid>(pIdentityGuid);

                ds = ConsolidatedDA.GetDataSet(cmd, pSecurity);
            }

            return (ds);
        }
        /// <summary>
        /// Merges current <seealso cref="SyncTableToScopeMapCommandDO"/> record with an existing record.  If the record doesn't currently
        /// exist, a new record is inserted.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="dataObject">The synchronization dependency group information to update or insert.</param>
        private void Merge(SecurityClass security, SyncTableToScopeMapCommandDO dataObject)
        {
            using (var cmd = PrepareUpsertStatement())
            {
                SqlParameterCollection parms = cmd.Parameters;
                parms["@IdentityGuid"].Value = dataObject.IdentityGuid;
                parms["@SyncTableToScopeMapIdentityGuid"].Value = dataObject.SyncTableToScopeMapGuid;
                parms["@SelectIncrementalInserts"].Value = dataObject.SelectIncrementalInserts;
                parms["@ApplyIncrementalInserts"].Value = dataObject.ApplyIncrementalInserts;
                parms["@SelectIncrementalUpdates"].Value = dataObject.SelectIncrementalUpdates;
                parms["@ApplyIncrementalUpdates"].Value = dataObject.ApplyIncrementalUpdates;
                parms["@SelectIncrementalDeletes"].Value = dataObject.SelectIncrementalDeletes;
                parms["@ApplyIncrementalDeletes"].Value = dataObject.ApplyIncrementalDeletes;
                parms["@SelectUpdateConflicts"].Value = dataObject.SelectUpdateConflicts;
                parms["@SelectDeleteConflicts"].Value = dataObject.SelectDeleteConflicts;
                parms["@CreatedBy"].Value = security.UserID;
                parms["@UpdatedBy"].Value = security.UserID;

                ConsolidatedDA.ExecuteQueryWithoutSessionContext(security, cmd);

                System.Nullable<Guid> retIdentityGuid = this.GetOutputValue<Guid>(parms["@NewRowGuid"], dataObject.IdentityGuid);

                if (retIdentityGuid.HasValue && retIdentityGuid.Value != dataObject.IdentityGuid)
                    dataObject.IdentityGuid = retIdentityGuid.Value;
            }
        }

        /// <summary>
        /// Removes the specified <seealso cref="SyncTableToScopeMapCommandDO"/> record.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="dataObject">The synchronization dependency group information to persist.</param>
        private void Delete(SecurityClass security, SyncTableToScopeMapCommandDO dataObject)
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

            cmd.CommandText = "sync.usp_SyncTableToScopeMapCommandSelect";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();

            cmd.Parameters.Add("@SyncTableToScopeMapGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);

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

            cmd.CommandText = "sync.usp_SyncTableToScopeMapCommandDeleteByGuid";
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

            cmd.CommandText = "sync.usp_SyncTableToScopeMapCommandSave";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();

            cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@SyncTableToScopeMapIdentityGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@SelectIncrementalInserts", SqlDbType.NVarChar, 1024);
            cmd.Parameters.Add("@ApplyIncrementalInserts", SqlDbType.NVarChar, 1024);
            cmd.Parameters.Add("@SelectIncrementalUpdates", SqlDbType.NVarChar, 1024);
            cmd.Parameters.Add("@ApplyIncrementalUpdates", SqlDbType.NVarChar, 1024);
            cmd.Parameters.Add("@SelectIncrementalDeletes", SqlDbType.NVarChar, 1024);
            cmd.Parameters.Add("@ApplyIncrementalDeletes", SqlDbType.NVarChar, 1024);
            cmd.Parameters.Add("@SelectUpdateConflicts", SqlDbType.NVarChar, 1024);
            cmd.Parameters.Add("@SelectDeleteConflicts", SqlDbType.NVarChar, 1024);
            cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
            cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);

            cmd.Parameters.Add("@NewRowGuid", SqlDbType.UniqueIdentifier).Direction = ParameterDirection.Output;

            return (cmd);
        }
        private SyncTableToScopeMapCommandDO GetDataObjectFromDataRow(DataRow pRow)
        {
            SyncTableToScopeMapCommandDO syncCommand = new SyncTableToScopeMapCommandDO();

            syncCommand.IdentityGuid = DataObject.getValue<Guid>(pRow["SyncTableToScopeMapCommandGuid"], Guid.Empty);
            syncCommand.SyncTableToScopeMapGuid = DataObject.getValue<Guid>(pRow["SyncTableToScopeMapGuid"], Guid.Empty);
            syncCommand.SelectIncrementalInserts = DataObject.getString(pRow["SelectIncrementalInserts"]);
            syncCommand.ApplyIncrementalInserts = DataObject.getString(pRow["ApplyIncrementalInserts"]);
            syncCommand.SelectIncrementalUpdates = DataObject.getString(pRow["SelectIncrementalUpdates"]);
            syncCommand.ApplyIncrementalUpdates = DataObject.getString(pRow["ApplyIncrementalUpdates"]);
            syncCommand.SelectIncrementalDeletes = DataObject.getString(pRow["SelectIncrementalDeletes"]);
            syncCommand.ApplyIncrementalDeletes = DataObject.getString(pRow["ApplyIncrementalDeletes"]);
            syncCommand.SelectUpdateConflicts = DataObject.getString(pRow["SelectUpdateConflicts"]);
            syncCommand.SelectDeleteConflicts = DataObject.getString(pRow["SelectDeleteConflicts"]);
            syncCommand.CreatedDate = DataObject.getValue<DateTimeOffset>(pRow["CreatedDate"], DateTimeOffset.Now);
            syncCommand.CreatedBy = DataObject.getString(pRow["CreatedBy"]);
            syncCommand.UpdatedDate = DataObject.getValue<DateTimeOffset>(pRow["UpdatedDate"], DateTimeOffset.Now);
            syncCommand.UpdatedBy = DataObject.getString(pRow["UpdatedBy"]);

            return (syncCommand);
        }
        #endregion Private Support Methods
    }
}
