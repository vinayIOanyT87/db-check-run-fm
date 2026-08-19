// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchemaChangeHistoryDBI.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for SchemaChangeHistoryDBI.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.InternalClasses
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Linq;

    using FMBusinessObjects.DataObjects;

    using FMBusinessServices.InternalClasses.SyncClasses;

	/// <summary>
    /// Summary description for SchemaChangeHistoryDBI.
    /// </summary>
    public class SchemaChangeHistoryDBI : SyncDBI
    {
        #region Attributes
        #endregion Attributes

        public SchemaChangeHistoryDBI(string user)
            : base(user)
        {
        }

        #region Public Data Access Methods

        /// <summary>
        /// The get list.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        /// </returns>
        public List<SchemaChangeHistoryDO> GetList(SecurityClass security)
        {
            List<SchemaChangeHistoryDO> list = new List<SchemaChangeHistoryDO>();

            DataSet ds = this.Load(security, (Guid?)null);

            if (ds.Tables[0].Rows.Count == 0)
            {
                return list;
            }

            list.AddRange(from DataRow row in ds.Tables[0].Rows select this.GetDataObjectFromDataRow(row));

            return list;
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
        /// The <see cref="SchemaChangeHistoryDO"/>.
        /// </returns>
        public SchemaChangeHistoryDO Get(SecurityClass security, Guid? identityGuid)
        {
            DataSet ds = this.Load(security, identityGuid);

            if (ds.Tables[0].Rows.Count == 0)
            {
                return null;
            }

            DataRow row = ds.Tables[0].Rows[0];
            SchemaChangeHistoryDO changeHistory = this.GetDataObjectFromDataRow(row);

            return changeHistory;
        }

        /// <summary>
        /// The get active session list.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="version">
        /// The version.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        /// </returns>
        public SchemaChangeHistoryDO Get(SecurityClass security, string version)
        {
            DataSet ds = this.Load(security, version);

            if (ds.Tables[0].Rows.Count == 0)
            {
                return null;
            }

            DataRow row = ds.Tables[0].Rows[0];
            SchemaChangeHistoryDO changeHistory = this.GetDataObjectFromDataRow(row);

            return changeHistory;
        }

        /// <summary>
        /// Saves the passed in <seealso cref="SchemaChangeHistoryDO"/> record.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="dataObject">The synchronization session information to persist.</param>
        /// <returns>True if the warning level for drawdown is hit.  Otherwise, false</returns>
        /// <remarks>Utilizes a merge stored procedure to implement Insert/Update operations in a single call.</remarks>
        public bool Save(SecurityClass security, SchemaChangeHistoryDO dataObject)
        {
            // Save the dataobject using a merge implementation
            this.Merge(security, dataObject);

            return true;
        }

        /// <summary>
        /// Deletes the passed in <seealso cref="SchemaChangeHistoryDO"/> record.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="dataObject">The synchronization session information to delete.</param>
        /// <param name="purge">Permanently delete the specified synchronization session.</param>
        /// <returns>True if the warning level for drawdown is hit.  Otherwise, false</returns>
        public bool Delete(SecurityClass security, SchemaChangeHistoryDO dataObject, bool purge)
        {
            try
            {
                // Delete the dataobject.
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
        /// Returns a DataSet containing the SchemaChangeHistory record containing the specified Primary Key.
        /// </summary>
        /// <param name="security">
        /// Contains security credentials
        /// </param>
        /// <param name="identityGuid">
        /// Primary key value for the SchemaChangeHistory record to load.
        /// </param>
        /// <returns>
        /// The <see cref="DataSet"/>.
        /// </returns>
        private DataSet Load(SecurityClass security, Guid? identityGuid)
        {
            DataSet ds = null;

            using (var cmd = this.PrepareSelectStatement())
            {
                SqlParameterCollection parms = cmd.Parameters;
                parms["@IdentityGuid"].Value = this.SetOptionalValue<Guid>(identityGuid);
                parms["@Version"].Value = DBNull.Value;

                ds = ConsolidatedDA.GetDataSet(cmd, security);
            }

            return ds;
        }

        /// <summary>
        /// Returns a DataSet containing all SchemaChangeHistory records that with a Started Date and no End Date
        /// </summary>
        /// <param name="security">
        /// Contains security credentials
        /// </param>
        /// <param name="version">
        /// The version.
        /// </param>
        /// <returns>
        /// Returns a <see cref="DataSet"/> populated with the matching SchemaChangeHistory.
        /// </returns>
        private DataSet Load(SecurityClass security, string version)
        {
            DataSet ds = null;

            using (var cmd = this.PrepareSelectStatement())
            {
                SqlParameterCollection parms = cmd.Parameters;
                parms["@IdentityGuid"].Value = DBNull.Value;
                parms["@Version"].Value = version;

                ds = ConsolidatedDA.GetDataSet(cmd, security);
            }

            return ds;
        }

        /// <summary>
        /// Merges current <seealso cref="SchemaChangeHistoryDO"/> record with an existing record.  If the record doesn't currently
        /// exist, a new record is inserted.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="dataObject">The synchronization session information to update or insert.</param>
        private void Merge(SecurityClass security, SchemaChangeHistoryDO dataObject)
        {
            using (var cmd = this.PrepareUpsertStatement())
            {
                SqlParameterCollection parms = cmd.Parameters;
                parms["@IdentityGuid"].Value = dataObject.IdentityGuid;
                parms["@Version"].Value = dataObject.Version;
                parms["@CreatedDate"].Value = dataObject.CreatedDate;
                parms["@CreatedBy"].Value = security.UserID;
                parms["@UpdatedBy"].Value = security.UserID;

                ConsolidatedDA.ExecuteQuery(security, cmd);

                Guid? retIdentityGuid = this.GetOutputValue<Guid>(parms["@NewRowGuid"], dataObject.IdentityGuid);

                if (retIdentityGuid.HasValue && retIdentityGuid.Value != dataObject.IdentityGuid)
                {
                    dataObject.IdentityGuid = retIdentityGuid.Value;
                }
            }
        }

        /// <summary>
        /// Removes the specified <seealso cref="SchemaChangeHistoryDO"/> record.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="dataObject">The synchronization session information to persist.</param>
        private void Delete(SecurityClass security, SchemaChangeHistoryDO dataObject)
        {
            using (var cmd = this.PrepareDeleteStatement())
            {
                SqlParameterCollection parms = null;
                parms = cmd.Parameters;
                parms["@IdentityGuid"].Value = dataObject.IdentityGuid;

                ConsolidatedDA.ExecuteQuery(security, cmd);
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

            cmd.CommandText = "sync.usp_SchemaChangeHistorySelect";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();

            cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@Version", SqlDbType.NVarChar, 80);

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

            cmd.CommandText = "sync.usp_SchemaChangeHistoryDeleteByGuid";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();

            cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);

            return cmd;
        }
        #endregion Override Implementations for Prepare Methods

        #region Private Support Methods

        /// <summary>
        /// The create merge statement.
        /// </summary>
        /// <returns>
        /// The <see cref="SqlCommand"/>.
        /// </returns>
        private SqlCommand CreateMergeStatement()
        {
            var cmd = new SqlCommand();

            cmd.CommandText = "sync.usp_SchemaChangeHistorySave";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();

            cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@Version", SqlDbType.NVarChar, 80);
            cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
            cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
            cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);

            cmd.Parameters.Add("@NewRowGuid", SqlDbType.UniqueIdentifier).Direction = ParameterDirection.Output;

            return cmd;
        }

        /// <summary>
        /// The get data object from data row.
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        /// <returns>
        /// The <see cref="SchemaChangeHistoryDO"/>.
        /// </returns>
        private SchemaChangeHistoryDO GetDataObjectFromDataRow(DataRow row)
        {
            SchemaChangeHistoryDO dataObject = new SchemaChangeHistoryDO();

            dataObject.IdentityGuid = DataObject.getValue<Guid>(row["SchemaChangeHistoryGuid"], Guid.Empty);
            dataObject.Version = DataObject.getString(row["Version"]);
            dataObject.HasSchemaChangeFlag = DataObject.getBool(row["HasSchemaChangeFlag"]);
            dataObject.CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
            dataObject.CreatedBy = DataObject.getString(row["CreatedBy"]);
            dataObject.UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], DateTimeOffset.Now);
            dataObject.UpdatedBy = DataObject.getString(row["UpdatedBy"]);

            return dataObject;
        }
        #endregion Private Support Methods
    }
}
