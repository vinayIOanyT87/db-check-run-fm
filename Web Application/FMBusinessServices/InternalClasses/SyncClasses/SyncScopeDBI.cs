namespace FMBusinessServices.InternalClasses.SyncClasses
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;

    using FMBusinessObjects.DataObjects;

    /// <summary>
    /// Summary description for SyncScopeDBI.
    /// </summary>
    public class SyncScopeDBI : SyncDBI
    {
        #region Attributes
        #endregion Attributes

        public SyncScopeDBI(string user)
            : base(user)
        {
        }

        #region Public Data Access Methods
        public List<SyncScopeDO> GetList(SecurityClass pSecurity, Guid pSyncProfileGuid)
        {
            List<SyncScopeDO> syncScopes = new List<SyncScopeDO>();

            DataSet ds = Load(pSecurity, null, pSyncProfileGuid, null);

            if (ds.Tables[0].Rows.Count == 0)
                return (syncScopes);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                SyncScopeDO syncScope = GetDataObjectFromDataRow(row);
                syncScopes.Add(syncScope);
            }

            return (syncScopes);
        }
        public SyncScopeDO Get(SecurityClass pSecurity, Guid pIdentityGuid)
        {
            DataSet ds = Load(pSecurity, pIdentityGuid, null, null);

            if (ds.Tables[0].Rows.Count == 0)
                return null;

            DataRow row = ds.Tables[0].Rows[0];
            SyncScopeDO syncScope = GetDataObjectFromDataRow(row);

            return (syncScope);
        }
        public SyncScopeDO Get(SecurityClass pSecurity, Guid pSyncProfileGuid, string pID)
        {
            DataSet ds = Load(pSecurity, null, pSyncProfileGuid, pID);

            if (ds.Tables[0].Rows.Count == 0)
                return null;

            DataRow row = ds.Tables[0].Rows[0];
            SyncScopeDO syncScope = GetDataObjectFromDataRow(row);

            return (syncScope);
        }
        /// <summary>
        /// Saves the passed in <seealso cref="SyncScopeDO"/> record.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="dataObject">The synchronization dependency group information to persist.</param>
        /// <returns>True if the warning level for drawdown is hit.  Otherwise, false</returns>
        /// <remarks>Utilizes a merge stored procedure to implement Insert/Update operations in a single call.</remarks>
        public bool Save(SecurityClass security, SyncScopeDO dataObject)
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
        public bool Delete(SecurityClass pSecurity, SyncScopeDO dataObject, bool pPurge)
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
        /// <param name="pIdentityGuid">Contains pSecurity credentials</param>
        /// <param name="pSyncProfileGuid">Contains pSecurity credentials</param>
        /// <param name="pID">Contains pSecurity credentials</param>
        private DataSet Load(SecurityClass pSecurity, System.Nullable<Guid> pIdentityGuid, System.Nullable<Guid> pSyncProfileGuid, string pID)
        {
            DataSet ds = null;

            using (var cmd = PrepareSelectStatement())
            {
                cmd.Parameters["@SyncProfileGuid"].Value = this.SetOptionalValue<Guid>(pSyncProfileGuid);
                cmd.Parameters["@IdentityGuid"].Value = this.SetOptionalValue<Guid>(pIdentityGuid);
                cmd.Parameters["@ID"].Value = this.SetOptionalValue<string>(pID);

                ds = ConsolidatedDA.GetDataSet(cmd, pSecurity);
            }

            return (ds);
        }
        /// <summary>
        /// Merges current <seealso cref="SyncScopeDO"/> record with an existing record.  If the record doesn't currently
        /// exist, a new record is inserted.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="dataObject">The synchronization dependency group information to update or insert.</param>
        private void Merge(SecurityClass security, SyncScopeDO dataObject)
        {
            using (var cmd = PrepareUpsertStatement())
            {
                cmd.Parameters["@IdentityGuid"].Value = dataObject.IdentityGuid;
                cmd.Parameters["@ID"].Value = dataObject.ID;
                cmd.Parameters["@ScopeTypeIndex"].Value = dataObject.SyncScopeTypeIndex;
                cmd.Parameters["@FriendlyName"].Value = this.SetOptionalValue<string>(dataObject.FriendlyName);
                cmd.Parameters["@LongDescription"].Value = this.SetOptionalValue<string>(dataObject.LongDescription);
                cmd.Parameters["@SyncProfileGuid"].Value = dataObject.SyncProfileGuid;
                cmd.Parameters["@SyncOrder"].Value = dataObject.SyncOrder;
                cmd.Parameters["@CreatedBy"].Value = security.UserID;
                cmd.Parameters["@UpdatedBy"].Value = security.UserID;
	            cmd.Parameters["@SyncSinglePass"].Value = dataObject.SyncSinglePass;

                ConsolidatedDA.ExecuteQueryWithoutSessionContext(security, cmd);

                System.Nullable<Guid> retIdentityGuid = this.GetOutputValue<Guid>(cmd.Parameters["@NewRowGuid"], dataObject.IdentityGuid);

                if (retIdentityGuid.HasValue && retIdentityGuid.Value != dataObject.IdentityGuid)
                    dataObject.IdentityGuid = retIdentityGuid.Value;
            }
        }

        /// <summary>
        /// Removes the specified <seealso cref="SyncScopeDO"/> record.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="dataObject">The synchronization dependency group information to persist.</param>
        private void Delete(SecurityClass security, SyncScopeDO dataObject)
        {
            using (var cmd = PrepareDeleteStatement())
            {
                cmd.Parameters["@IdentityGuid"].Value = dataObject.IdentityGuid;

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

            cmd.CommandText = "sync.usp_SyncScopeSelect";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();

            cmd.Parameters.Add("@SyncProfileGuid", SqlDbType.UniqueIdentifier);
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

            cmd.CommandText = "sync.usp_SyncScopeDeleteByGuid";
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

            cmd.CommandText = "sync.usp_SyncScopeSave";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();

            cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 80);
            cmd.Parameters.Add("@ScopeTypeIndex", SqlDbType.BigInt);
            cmd.Parameters.Add("@FriendlyName", SqlDbType.NVarChar, 100);
            cmd.Parameters.Add("@LongDescription", SqlDbType.NVarChar, 1024);
            cmd.Parameters.Add("@SyncProfileGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@SyncOrder", SqlDbType.Int);
            cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
            cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
	        cmd.Parameters.Add("@SyncSinglePass", SqlDbType.Bit);

            cmd.Parameters.Add("@NewRowGuid", SqlDbType.UniqueIdentifier).Direction = ParameterDirection.Output;

            return (cmd);
        }
        private SyncScopeDO GetDataObjectFromDataRow(DataRow pRow)
        {
            SyncScopeDO syncScope = new SyncScopeDO();

            syncScope.IdentityGuid = DataObject.getGuid(pRow["SyncScopeGuid"]);
            syncScope.ID = DataObject.getString(pRow["ID"]);

            long indexValue = DataObject.getValue<long>(pRow["SyncScopeTypeIndex"], (long)SYNCSCOPETYPE.REFERENCE_ONLY);
            SYNCSCOPETYPE syncScopeType = SYNCSCOPETYPE.REFERENCE_ONLY;

            if (Enum.TryParse(indexValue.ToString(CultureInfo.InvariantCulture), true, out syncScopeType))
            {
                syncScope.SyncScopeTypeIndex = syncScopeType;
            }
            
            syncScope.FriendlyName = DataObject.getString(pRow["FriendlyName"]);
            syncScope.LongDescription = DataObject.getString(pRow["LongDescription"]);
            syncScope.SyncProfileGuid = DataObject.getGuid(pRow["SyncProfileGuid"]);
            syncScope.SyncOrder = DataObject.getInt(pRow["SyncOrder"]);
	        syncScope.SyncSinglePass = DataObject.getBool(pRow["SyncSinglePass"]);

            syncScope.CreatedDate = DataObject.getValue<DateTimeOffset>(pRow["CreatedDate"], DateTimeOffset.Now);
            syncScope.CreatedBy = DataObject.getString(pRow["CreatedBy"]);
            syncScope.UpdatedDate = DataObject.getValue<DateTimeOffset>(pRow["UpdatedDate"], DateTimeOffset.Now);
            syncScope.UpdatedBy = DataObject.getString(pRow["UpdatedBy"]);

            return (syncScope);
        }
        #endregion Private Support Methods
    }
}
