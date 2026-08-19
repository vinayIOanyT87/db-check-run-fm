namespace FMBusinessServices.InternalClasses.SyncClasses
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    using FMBusinessObjects.DataObjects;

    /// <summary>
	/// Summary description for SyncDependencyGroupDBI.
	/// </summary>
    public class SyncDependencyGroupDBI : SyncDBI
    {
        #region Attributes
        #endregion Attributes

        public SyncDependencyGroupDBI(string user)
			: base(user)
		{
		}

        #region Public Data Access Methods
        public List<SyncDependencyGroupDO> GetList(SecurityClass pSecurity)
        {
            List<SyncDependencyGroupDO> syncDependencyGroups = new List<SyncDependencyGroupDO>();

            DataSet ds = Load(pSecurity, null, null);

            if (ds.Tables[0].Rows.Count == 0)
                return (syncDependencyGroups);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                SyncDependencyGroupDO syncDependencyGroup = GetDataObjectFromDataRow(row);
                syncDependencyGroups.Add(syncDependencyGroup);
            }

            return (syncDependencyGroups);
        }
        public SyncDependencyGroupDO Get(SecurityClass pSecurity, System.Nullable<Guid> pIdentityGuid, string pID)
        {
            DataSet ds = Load(pSecurity, pIdentityGuid, pID);

            if (ds.Tables[0].Rows.Count == 0)
                return null;

            DataRow row = ds.Tables[0].Rows[0];
            SyncDependencyGroupDO syncProfile = GetDataObjectFromDataRow(row);

            return (syncProfile);
        }
        /// <summary>
        /// Saves the passed in <seealso cref="SyncDependencyGroupDO"/> record.
		/// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="dataObject">The synchronization dependency group information to persist.</param>
		/// <returns>True if the warning level for drawdown is hit.  Otherwise, false</returns>
        /// <remarks>Utilizes a merge stored procedure to implement Insert/Update operations in a single call.</remarks>
        public bool Save(SecurityClass pSecurity, SyncDependencyGroupDO pDataObject)
		{
			// Save the dataobject using a merge implementation
            Merge(pSecurity, pDataObject);

			return true;
		}

        /// <summary>
        /// Deletes the passed in <seealso cref="SyncProfileDO"/> record.
        /// </summary>
        /// <param name="pSecurity">Contains pSecurity credentials</param>
        /// <param name="dataObject">The synchronization dependency group information to delete.</param>
        /// <returns>True if the warning level for drawdown is hit.  Otherwise, false</returns>
        public bool Delete(SecurityClass pSecurity, SyncDependencyGroupDO pDataObject, bool pPurge)
        {
            try
            {
                // Save the dataobject using a merge implementation
                Delete(pSecurity, pDataObject);

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
        private DataSet Load(SecurityClass pSecurity, System.Nullable<Guid> pIdentityGuid, string pID)
        {
            DataSet ds = null;

            using (var cmd = PrepareSelectStatement())
            {
                SqlParameterCollection parms = cmd.Parameters;
                parms["@IdentityGuid"].Value = this.SetOptionalValue<Guid>(pIdentityGuid);
                parms["@ID"].Value = this.SetOptionalValue<string>(pID);

                ds = ConsolidatedDA.GetDataSet(cmd, pSecurity);
            }

            return (ds);
        }
        /// <summary>
        /// Merges current <seealso cref="SyncDependencyGroupDO"/> record with an existing record.  If the record doesn't currently
        /// exist, a new record is inserted.
        /// </summary>
        /// <param name="pSecurity">Contains security credentials</param>
        /// <param name="pDataObject">The synchronization dependency group information to update or insert.</param>
        private void Merge(SecurityClass pSecurity, SyncDependencyGroupDO pDataObject)
        {
            using (var cmd = PrepareUpsertStatement())
            {
                SqlParameterCollection parms = cmd.Parameters;
                parms["@IdentityGuid"].Value = pDataObject.IdentityGuid;
                parms["@ID"].Value = pDataObject.ID;
                parms["@FriendlyName"].Value = this.SetOptionalValue<string>(pDataObject.FriendlyName);
                parms["@LongDescription"].Value = this.SetOptionalValue<string>(pDataObject.LongDescription);
                parms["@DependencyLevel"].Value = pDataObject.DependencyLevel;
                parms["@CreatedBy"].Value = pSecurity.UserID;
                parms["@UpdatedBy"].Value = pSecurity.UserID;

                ConsolidatedDA.ExecuteQueryWithoutSessionContext(pSecurity, cmd);

                System.Nullable<Guid> retIdentityGuid = this.GetOutputValue<Guid>(parms["@NewRowGuid"], pDataObject.IdentityGuid);

                if (retIdentityGuid.HasValue && retIdentityGuid.Value != pDataObject.IdentityGuid)
                    pDataObject.IdentityGuid = retIdentityGuid.Value;
            }
        }
        /// <summary>
        /// Removes the specified <seealso cref="SyncDependencyGroupDO"/> record.
		/// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="dataObject">The synchronization dependency group information to persist.</param>
        private void Delete(SecurityClass pSecurity, SyncDependencyGroupDO pDataObject)
		{
            using (var cmd = PrepareDeleteStatement())
            {
                SqlParameterCollection parms = null;
                parms = cmd.Parameters;
                parms["@IdentityGuid"].Value = pDataObject.IdentityGuid;

                ConsolidatedDA.ExecuteQueryWithoutSessionContext(pSecurity, cmd);
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

            cmd.CommandText = "sync.usp_SyncDependencyGroupSelect";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();

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

            cmd.CommandText = "sync.usp_SyncDependencyGroupDeleteByGuid";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();

            cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);

            return (cmd);
        }
        #endregion Override Implementations for Prepare Methods

        #region Private Merge Support Method
        private SqlCommand CreateMergeStatement()
        {
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "sync.usp_SyncDependencyGroupSave";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();

            cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 80);
            cmd.Parameters.Add("@FriendlyName", SqlDbType.NVarChar, 100);
            cmd.Parameters.Add("@LongDescription", SqlDbType.NVarChar, 1024);
            cmd.Parameters.Add("@DependencyLevel", SqlDbType.Int);
            cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
            cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);

            cmd.Parameters.Add("@NewRowGuid", SqlDbType.UniqueIdentifier).Direction = ParameterDirection.Output;

            return (cmd);
        }
        private SyncDependencyGroupDO GetDataObjectFromDataRow(DataRow pRow)
        {
            SyncDependencyGroupDO syncGroup = new SyncDependencyGroupDO();

            syncGroup.IdentityGuid = DataObject.getValue<Guid>(pRow["SyncDependencyGroupGuid"], Guid.Empty);
            syncGroup.ID = DataObject.getValue<string>(pRow["ID"], "");
            syncGroup.FriendlyName = DataObject.getString(pRow["FriendlyName"]);
            syncGroup.LongDescription = DataObject.getString(pRow["LongDescription"]);
            syncGroup.DependencyLevel = DataObject.getInt(pRow["DependencyLevel"]);
            syncGroup.CreatedDate = DataObject.getValue<DateTimeOffset>(pRow["CreatedDate"], DateTimeOffset.Now);
            syncGroup.CreatedBy = DataObject.getString(pRow["CreatedBy"]);
            syncGroup.UpdatedDate = DataObject.getValue<DateTimeOffset>(pRow["UpdatedDate"], DateTimeOffset.Now);
            syncGroup.UpdatedBy = DataObject.getString(pRow["UpdatedBy"]);

            return (syncGroup);
        }
        #endregion Private Merge Support Method
    }
}
