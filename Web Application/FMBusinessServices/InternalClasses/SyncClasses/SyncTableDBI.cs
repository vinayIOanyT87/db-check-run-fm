// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncTableDBI.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for SyncTableDBI.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.InternalClasses.SyncClasses
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Data.SqlTypes;

    using FMBusinessObjects.DataObjects;

    /// <summary>
    /// Summary description for SyncTableDBI.
    /// </summary>
    public class SyncTableDBI : SyncDBI
    {
        #region Attributes
        #endregion Attributes

        public SyncTableDBI(string pUser)
            : base(pUser)
        {
        }

        #region Public Data Access Methods
        public List<SyncTableDO> GetListForDependencyGroup(SecurityClass security, System.Nullable<Guid> dependencyGroupGuid, string dependencyGroupID)
        {
            List<SyncTableDO> syncTables = new List<SyncTableDO>();

            DataSet ds = LoadForDependencyGroup(security, dependencyGroupGuid, dependencyGroupID);

            if (ds.Tables[0].Rows.Count == 0)
                return (syncTables);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                SyncTableDO syncTable = GetDataObjectFromDataRow(row);
                syncTables.Add(syncTable);
            }

            return (syncTables);
        }
        public List<SyncTableDO> GetList(SecurityClass security)
        {
            List<SyncTableDO> syncTables = new List<SyncTableDO>();

            DataSet ds = Load(security, null, null);

            if (ds.Tables[0].Rows.Count == 0)
                return (syncTables);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                SyncTableDO syncTable = GetDataObjectFromDataRow(row);
                syncTables.Add(syncTable);
            }

            return (syncTables);
        }
        public SyncTableDO Get(SecurityClass security, System.Nullable<Guid> identityGuid, string id)
        {
            DataSet ds = Load(security, identityGuid, id);

            if (ds.Tables[0].Rows.Count == 0)
                return null;

            DataRow row = ds.Tables[0].Rows[0];
            SyncTableDO syncTable = GetDataObjectFromDataRow(row);

            return (syncTable);
        }

        /// <summary>
        /// Saves the passed in <seealso cref="SyncTableDO"/> record.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="dataObject">The synchronization dependency group information to persist.</param>
        /// <returns>True if the warning level for drawdown is hit.  Otherwise, false</returns>
        /// <remarks>Utilizes a merge stored procedure to implement Insert/Update operations in a single call.</remarks>
        public bool Save(SecurityClass security, SyncTableDO dataObject)
        {
            // Save the dataobject using a merge implementation
            Merge(security, dataObject);

            return true;
        }

        /// <summary>
        /// Deletes the passed in 
        /// <seealso cref="SyncProfileDO"/>
        /// record.
        /// </summary>
        /// <param name="security">
        /// Contains security credentials
        /// </param>
        /// <param name="dataObject">
        /// The synchronization dependency group information to delete.
        /// </param>
        /// <param name="purge">
        /// The p Purge.
        /// </param>
        /// <returns>
        /// True if the warning level for drawdown is hit.  Otherwise, false
        /// </returns>
        public bool Delete(SecurityClass security, SyncTableDO dataObject, bool purge)
        {
            try
            {
                // Save the dataobject using a merge implementation
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
        /// Loads and instance of a <seealso cref="SyncTableDO"/> record with the specified primary indentityGuid.  If a Guid is not specified
        /// then the table name must be provided.
        /// </summary>
        /// <param name="security">
        /// Contains security credentials
        /// </param>
        /// <param name="identityGuid">
        /// The synchronization table identity to retrieve.  Can be null if the tableName is specified.
        /// </param>
        /// <param name="tableName">
        /// The table name of the <seealso cref="SyncTableDO"/> record to retrieve.
        /// </param>
        /// <returns>
        /// A <see cref="DataSet"/> containing the matching SyncTableDO record.
        /// </returns>
        private DataSet Load(SecurityClass security, System.Nullable<Guid> identityGuid, string tableName)
        {
            DataSet ds = null;

            using (var cmd = PrepareSelectStatement())
            {
                SqlParameterCollection parms = cmd.Parameters;
                parms["@IdentityGuid"].Value = this.SetOptionalValue<Guid>(identityGuid);
                parms["@TableName"].Value = this.SetOptionalValue<string>(tableName);

                ds = ConsolidatedDA.GetDataSet(cmd, security);
            }

            return (ds);
        }

        private DataSet LoadForDependencyGroup(SecurityClass security, System.Nullable<Guid> dependencyGroupGuid, string dependencyGroupID)
        {
            DataSet ds = null;

            using (var cmd = PrepareSelectByDependencyGroupStatement())
            {
                SqlParameterCollection parms = cmd.Parameters;
                parms["@SyncDependencyGroupGuid"].Value = this.SetOptionalValue<Guid>(dependencyGroupGuid);
                parms["@SyncDependencyGroupID"].Value = this.SetOptionalValue<string>(dependencyGroupID);

                ds = ConsolidatedDA.GetDataSet(cmd, security);
            }

            return (ds);
        }

        /// <summary>
        /// Merges current <seealso cref="SyncTableDO"/> record with an existing record.  If the record doesn't currently
        /// exist, a new record is inserted.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="dataObject">The synchronization table information to update or insert.</param>
        private void Merge(SecurityClass security, SyncTableDO dataObject)
        {
            using (var cmd = PrepareUpsertStatement())
            {
                SqlParameterCollection parms = cmd.Parameters;
                parms["@IdentityGuid"].Value = dataObject.IdentityGuid;
                parms["@SyncDependencyGroupGuid"].Value = dataObject.SyncDependencyGroupGuid;
                parms["@TableName"].Value = dataObject.TableName;
                parms["@LastSchemaDate"].Value = dataObject.LastSchemaDate;
                parms["@IsSiteFilteredFlag"].Value = dataObject.IsSiteFilteredFlag;
                parms["@IsSiteFilteredOnDeleteFlag"].Value = dataObject.IsSiteFilteredOnDeleteFlag;
	            parms["@ParentSyncTableGuid"].Value = dataObject.ParentSyncTableGuid;
	            parms["@ParentForeignKeyColumnName"].Value = dataObject.ParentForeignKeyColumnName;
                parms["@CreatedBy"].Value = security.UserID;
                parms["@UpdatedBy"].Value = security.UserID;

                ConsolidatedDA.ExecuteQueryWithoutSessionContext(security, cmd);

                System.Nullable<Guid> retIdentityGuid = this.GetOutputValue<Guid>(parms["@NewRowGuid"], dataObject.IdentityGuid);

                if (retIdentityGuid.HasValue && retIdentityGuid.Value != dataObject.IdentityGuid)
                    dataObject.IdentityGuid = retIdentityGuid.Value;
            }
        }

        /// <summary>
        /// Removes the specified <seealso cref="SyncTableDO"/> record.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="dataObject">The synchronization table information to delete.</param>
        private void Delete(SecurityClass security, SyncTableDO dataObject)
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

        #region Custom Prepare Methods
        protected SqlCommand PrepareSelectByDependencyGroupStatement()
        {
            var cmd = new SqlCommand();

            cmd.CommandText = "sync.usp_SyncTableSelectForSyncDependencyGroupKey";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();

            cmd.Parameters.Add("@SyncDependencyGroupGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@SyncDependencyGroupID", SqlDbType.NVarChar, 80);

            return (cmd);
        }
        #endregion Custom Prepare Methods

        #region Override Implementations for Prepare Methods
        protected override SqlCommand PrepareUpsertStatement()
        {
            return (CreateMergeStatement());
        }
        protected override SqlCommand PrepareSelectStatement()
        {
            var cmd = new SqlCommand();

            cmd.CommandText = "sync.usp_SyncTableSelect";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();

            cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 1024);

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

            cmd.CommandText = "sync.usp_SyncTableDeleteByGuid";
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

            cmd.CommandText = "sync.usp_SyncTableSave";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();

            cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@SyncDependencyGroupGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 1024);
            cmd.Parameters.Add("@LastSchemaDate", SqlDbType.DateTimeOffset);
            cmd.Parameters.Add("@IsSiteFilteredFlag", SqlDbType.Bit);
            cmd.Parameters.Add("@IsSiteFilteredOnDeleteFlag", SqlDbType.Bit);
	        cmd.Parameters.Add("@ParentSyncTableGuid", SqlDbType.UniqueIdentifier);
	        cmd.Parameters.Add("@ParentForeignKeyColumnName", SqlDbType.NVarChar, 512);
            cmd.Parameters.Add("@CreatedBy", SqlDbType.DateTimeOffset);
            cmd.Parameters.Add("@UpdatedBy", SqlDbType.DateTimeOffset);

            cmd.Parameters.Add("@NewRowGuid", SqlDbType.UniqueIdentifier).Direction = ParameterDirection.Output;

            return (cmd);
        }
        private SyncTableDO GetDataObjectFromDataRow(DataRow row)
        {
            SyncTableDO syncTable = new SyncTableDO();

            syncTable.IdentityGuid = DataObject.getGuid(row["SyncTableGuid"]);
            syncTable.TableName = DataObject.getString(row["TableName"]);
            syncTable.SyncDependencyGroupGuid = DataObject.getValue<Guid>(row["SyncDependencyGroupGuid"], Guid.Empty);
            syncTable.LastSchemaDate = DataObject.getValue<DateTimeOffset>(row["LastSchemaDate"], DateTimeOffset.Now);
            syncTable.IsSiteFilteredFlag = DataObject.getBool(row["IsSiteFilteredFlag"]);
            syncTable.IsSiteFilteredOnDeleteFlag = DataObject.getBool(row["IsSiteFilteredOnDeleteFlag"]);
	        syncTable.ParentSyncTableGuid = DataObject.getValue<Guid?>(row["ParentSyncTableGuid"], null);
	        syncTable.ParentForeignKeyColumnName = DataObject.getString(row["ParentForeignKeyColumnName"]);
            syncTable.CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
            syncTable.CreatedBy = DataObject.getString(row["CreatedBy"]);
            syncTable.UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], DateTimeOffset.Now);
            syncTable.UpdatedBy = DataObject.getString(row["UpdatedBy"]);

            return (syncTable);
        }
        #endregion Private Support Methods
    }
}
