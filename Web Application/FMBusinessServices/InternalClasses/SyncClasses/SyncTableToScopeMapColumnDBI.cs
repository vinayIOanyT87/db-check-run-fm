// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncTableToScopeMapColumnDBI.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for SyncTableToScopeMapColumnDBI.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



namespace FMBusinessServices.InternalClasses.SyncClasses
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    using FMBusinessObjects.DataObjects;

    /// <summary>
    /// Summary description for SyncTableToScopeMapColumnDBI.
    /// </summary>
    public class SyncTableToScopeMapColumnDBI : SyncDBI
    {
        #region Attributes
        #endregion Attributes

        public SyncTableToScopeMapColumnDBI(string user)
            : base(user)
        {
        }

        #region Public Data Access Methods
        public SyncTableToScopeMapColumnCollection GetList(SecurityClass pSecurity, Guid pSyncTableToScopeMapGuid)
        {
            SyncTableToScopeMapColumnCollection syncColumns = new SyncTableToScopeMapColumnCollection();

            DataSet ds = Load(pSecurity, null, pSyncTableToScopeMapGuid, null);

            if (ds.Tables[0].Rows.Count == 0)
                return (syncColumns);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                SyncTableToScopeMapColumnDO syncColumn = GetDataObjectFromDataRow(row);
                syncColumns.Add(syncColumn);
            }

            return (syncColumns);
        }
        public SyncTableToScopeMapColumnDO Get(SecurityClass pSecurity, System.Nullable<Guid> pIdentityGuid, System.Nullable<Guid> pSyncTableToScopeMapGuid, string pColumnName)
        {
            DataSet ds = Load(pSecurity, pIdentityGuid, pSyncTableToScopeMapGuid, pColumnName);

            if (ds.Tables[0].Rows.Count == 0)
                return null;

            DataRow row = ds.Tables[0].Rows[0];
            SyncTableToScopeMapColumnDO syncColumn = GetDataObjectFromDataRow(row);

            return (syncColumn);
        }
        /// <summary>
        /// Saves the passed in <seealso cref="SyncTableToScopeMapColumnDO"/> record.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="dataObject">The synchronization dependency group information to persist.</param>
        /// <returns>True if the warning level for drawdown is hit.  Otherwise, false</returns>
        /// <remarks>Utilizes a merge stored procedure to implement Insert/Update operations in a single call.</remarks>
        public bool Save(SecurityClass security, SyncTableToScopeMapColumnDO dataObject)
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
        public bool Delete(SecurityClass pSecurity, SyncTableToScopeMapColumnDO dataObject, bool pPurge)
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
        private DataSet Load(SecurityClass pSecurity, System.Nullable<Guid> pIdentityGuid, System.Nullable<Guid> pSyncTableToScopeMapGuid, string pColumnName)
        {
            DataSet ds = null;

            using (var cmd = PrepareSelectStatement())
            {
                SqlParameterCollection parms = cmd.Parameters;
                parms["@SyncTableToScopeMapGuid"].Value = this.SetOptionalValue<Guid>(pSyncTableToScopeMapGuid);
                parms["@IdentityGuid"].Value = this.SetOptionalValue<Guid>(pIdentityGuid);
                parms["@ColumnName"].Value = this.SetOptionalValue<string>(pColumnName);

                ds = ConsolidatedDA.GetDataSet(cmd, pSecurity);
            }

            return (ds);
        }
        /// <summary>
        /// Merges current <seealso cref="SyncTableToScopeMapColumnDO"/> record with an existing record.  If the record doesn't currently
        /// exist, a new record is inserted.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="dataObject">The synchronization dependency group information to update or insert.</param>
        private void Merge(SecurityClass security, SyncTableToScopeMapColumnDO dataObject)
        {
            using (var cmd = PrepareUpsertStatement())
            {
                SqlParameterCollection parms = cmd.Parameters;
                parms["@IdentityGuid"].Value = dataObject.IdentityGuid;
                parms["@SyncTableToScopeMapIdentityGuid"].Value = dataObject.SyncTableToScopeMapGuid;
                parms["@ColumnName"].Value = dataObject.ColumnName;
                parms["@ColumnIndex"].Value = dataObject.ColumnIndex;
                parms["@ColumnType"].Value = dataObject.ColumnType;
                parms["@ColumnSize"].Value = this.SetOptionalValue<int>(dataObject.ColumnSize);
                parms["@ColumnPrecision"].Value = this.SetOptionalValue<int>(dataObject.ColumnPrecision);
                parms["@ColumnScale"].Value = this.SetOptionalValue<int>(dataObject.ColumnScale);
                parms["@IsNullableFlag"].Value = dataObject.IsNullableFlag;
                parms["@IsPrimaryKeyMemberFlag"].Value = dataObject.IsPrimaryKeyMemberFlag;
                parms["@IsidentityColumnFlag"].Value = dataObject.IsIdentityColumnFlag;
                parms["@CreatedBy"].Value = security.UserID;
                parms["@UpdatedBy"].Value = security.UserID;

                ConsolidatedDA.ExecuteQueryWithoutSessionContext(security, cmd);

                System.Nullable<Guid> retIdentityGuid = this.GetOutputValue<Guid>(parms["@NewRowGuid"], dataObject.IdentityGuid);

                if (retIdentityGuid.HasValue && retIdentityGuid.Value != dataObject.IdentityGuid)
                    dataObject.IdentityGuid = retIdentityGuid.Value;
            }
        }

        /// <summary>
        /// Removes the specified <seealso cref="SyncTableToScopeMapColumnDO"/> record.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="dataObject">The synchronization dependency group information to persist.</param>
        private void Delete(SecurityClass security, SyncTableToScopeMapColumnDO dataObject)
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

            cmd.CommandText = "sync.usp_SyncTableToScopeMapColumnSelect";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();

            cmd.Parameters.Add("@SyncTableToScopeMapGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@ColumnName", SqlDbType.NVarChar, 512);

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

            cmd.CommandText = "sync.usp_SyncTableToScopeMapColumnDeleteByGuid";
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

            cmd.CommandText = "sync.usp_SyncTableToScopeMapColumnSave";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();

            cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@SyncTableToScopeMapIdentityGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@ColumnName", SqlDbType.NVarChar, 512);
            cmd.Parameters.Add("@ColumnIndex", SqlDbType.Int);
            cmd.Parameters.Add("@ColumnType", SqlDbType.NVarChar, 256);
            cmd.Parameters.Add("@ColumnSize", SqlDbType.Int);
            cmd.Parameters.Add("@ColumnPrecision", SqlDbType.Int);
            cmd.Parameters.Add("@ColumnScale", SqlDbType.Int);
            cmd.Parameters.Add("@IsNullableFlag", SqlDbType.Bit);
            cmd.Parameters.Add("@IsPrimaryKeyMemberFlag", SqlDbType.Bit);
            cmd.Parameters.Add("@IsidentityColumnFlag", SqlDbType.Bit);
            cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
            cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);

            cmd.Parameters.Add("@NewRowGuid", SqlDbType.UniqueIdentifier).Direction = ParameterDirection.Output;

            return (cmd);
        }
        private SyncTableToScopeMapColumnDO GetDataObjectFromDataRow(DataRow pRow)
        {
            SyncTableToScopeMapColumnDO syncColumn = new SyncTableToScopeMapColumnDO();

            syncColumn.IdentityGuid = DataObject.getValue<Guid>(pRow["SyncTableToScopeMapColumnGuid"], Guid.Empty);
            syncColumn.SyncTableToScopeMapGuid = DataObject.getValue<Guid>(pRow["SyncTableToScopeMapGuid"], Guid.Empty);
            syncColumn.ColumnName = DataObject.getString(pRow["ColumnName"]);
            syncColumn.ColumnIndex = DataObject.getInt(pRow["ColumnIndex"]);
            syncColumn.ColumnType = DataObject.getString(pRow["ColumnType"]);
            syncColumn.ColumnSize = DataObject.getOptionalInt(pRow["ColumnSize"]);
            syncColumn.ColumnPrecision = DataObject.getOptionalInt(pRow["ColumnPrecision"]);
            syncColumn.ColumnScale = DataObject.getOptionalInt(pRow["ColumnScale"]);
            syncColumn.IsNullableFlag = DataObject.getBool(pRow["IsNullableFlag"]);
            syncColumn.IsPrimaryKeyMemberFlag = DataObject.getBool(pRow["IsPrimaryKeyMemberFlag"]);
            syncColumn.IsIdentityColumnFlag = DataObject.getBool(pRow["IsIdentityColumnFlag"]);
            syncColumn.CreatedDate = DataObject.getValue<DateTimeOffset>(pRow["CreatedDate"], DateTimeOffset.Now);
            syncColumn.CreatedBy = DataObject.getString(pRow["CreatedBy"]);
            syncColumn.UpdatedDate = DataObject.getValue<DateTimeOffset>(pRow["UpdatedDate"], DateTimeOffset.Now);
            syncColumn.UpdatedBy = DataObject.getString(pRow["UpdatedBy"]);

            return (syncColumn);
        }
        #endregion Private Support Methods
    }
}
