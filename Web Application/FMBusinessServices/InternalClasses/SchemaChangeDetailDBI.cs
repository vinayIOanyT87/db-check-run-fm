// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchemaChangeDetailDBI.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for SchemaChangeDetailDBI.
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
    /// Summary description for SchemaChangeDetailDBI.
    /// </summary>
    public class SchemaChangeDetailDBI : SyncDBI
    {
        #region Attributes
        #endregion Attributes

        public SchemaChangeDetailDBI(string user)
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
        /// <param name="schemaChangeHistoryGuid">
        /// The schema Change History.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        /// </returns>
        public List<SchemaChangeDetailDO> GetList(SecurityClass security, Guid schemaChangeHistoryGuid)
        {
            List<SchemaChangeDetailDO> list = new List<SchemaChangeDetailDO>();

            DataSet ds = this.LoadByChangeHistoryGuid(security, schemaChangeHistoryGuid);

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
        /// The <see cref="SchemaChangeDetailDO"/>.
        /// </returns>
        public SchemaChangeDetailDO Get(SecurityClass security, Guid identityGuid)
        {
            DataSet ds = this.Load(security, identityGuid);

            if (ds.Tables[0].Rows.Count == 0)
            {
                return null;
            }

            DataRow row = ds.Tables[0].Rows[0];
            SchemaChangeDetailDO version = this.GetDataObjectFromDataRow(row);

            return version;
        }

        /// <summary>
        /// Saves the passed in <seealso cref="SchemaChangeDetailDO"/> record.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="dataObject">The synchronization session information to persist.</param>
        /// <returns>True if the warning level for drawdown is hit.  Otherwise, false</returns>
        /// <remarks>Utilizes a merge stored procedure to implement Insert/Update operations in a single call.</remarks>
        public bool Save(SecurityClass security, SchemaChangeDetailDO dataObject)
        {
            // Save the dataobject using a merge implementation
            this.Merge(security, dataObject);

            return true;
        }

        /// <summary>
        /// Deletes the passed in <seealso cref="SchemaChangeDetailDO"/> record.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="dataObject">The synchronization session information to delete.</param>
        /// <param name="purge">Permanently delete the specified synchronization session.</param>
        /// <returns>True if the warning level for drawdown is hit.  Otherwise, false</returns>
        public bool Delete(SecurityClass security, SchemaChangeDetailDO dataObject, bool purge)
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
        /// Returns a DataSet containing the SchemaChangeDetail record containing the specified Primary Key.
        /// </summary>
        /// <param name="security">
        /// Contains security credentials
        /// </param>
        /// <param name="identityGuid">
        /// Primary key value for the SchemaChangeDetail record to load.
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
        /// Returns a DataSet containing all SchemaChangeDetail records that with a Started Date and no End Date
        /// </summary>
        /// <param name="security">
        /// Contains security credentials
        /// </param>
        /// <param name="schemaChangeHistoryGuid">
        /// The schema Change History Guid.
        /// </param>
        /// <returns>
        /// Returns a <see cref="DataSet"/> populated with the matching SchemaChangeDetails.
        /// </returns>
        private DataSet LoadByChangeHistoryGuid(SecurityClass security, Guid schemaChangeHistoryGuid)
        {
            DataSet ds = null;

            using (var cmd = this.PrepareSelectSchemaChangeDetailByHistoryStatement())
            {
                ds = ConsolidatedDA.GetDataSet(cmd, security);
            }

            return ds;
        }

        /// <summary>
        /// Merges current <seealso cref="SchemaChangeDetailDO"/> record with an existing record.  If the record doesn't currently
        /// exist, a new record is inserted.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="dataObject">The synchronization session information to update or insert.</param>
        private void Merge(SecurityClass security, SchemaChangeDetailDO dataObject)
        {
            using (var cmd = this.PrepareUpsertStatement())
            {
                SqlParameterCollection parms = cmd.Parameters;
                parms["@IdentityGuid"].Value = dataObject.IdentityGuid;
                parms["@SchemaChangeHistoryGuid"].Value = dataObject.SchemaChangeHistoryGuid;
                parms["@SchemaObjectTypeIndex"].Value = dataObject.SchemaObjectTypeIndex;
                parms["@SchemaName"].Value = dataObject.SchemaName;
                parms["@ObjectName"].Value = dataObject.ObjectName;
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
        /// Removes the specified <seealso cref="SchemaChangeDetailDO"/> record.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="dataObject">The synchronization session information to persist.</param>
        private void Delete(SecurityClass security, SchemaChangeDetailDO dataObject)
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

            cmd.CommandText = "sync.usp_SchemaChangeDetailSelect";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();

            cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);

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

            cmd.CommandText = "sync.usp_SchemaChangeDetailDeleteByGuid";
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
        private SqlCommand PrepareSelectSchemaChangeDetailByHistoryStatement()
        {
            var cmd = new SqlCommand();

            cmd.CommandText = "sync.usp_SchemaChangeDetailSelectForSchemaChangeHistory";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();

            cmd.Parameters.Add("@SchemaChangeHistoryGuid", SqlDbType.UniqueIdentifier);

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

            cmd.CommandText = "sync.usp_SchemaChangeDetailSave";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();

            cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@SchemaChangeHistoryGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@SchemaObjectTypeIndex", SqlDbType.BigInt);
            cmd.Parameters.Add("@SchemaName", SqlDbType.NVarChar, 64);
            cmd.Parameters.Add("@ObjectName", SqlDbType.NVarChar, 512);
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
        /// The <see cref="SchemaChangeDetailDO"/>.
        /// </returns>
        private SchemaChangeDetailDO GetDataObjectFromDataRow(DataRow row)
        {
            SchemaChangeDetailDO dataObject = new SchemaChangeDetailDO();

            dataObject.IdentityGuid = DataObject.getValue<Guid>(row["SchemaChangeDetailGuid"], Guid.Empty);
            dataObject.SchemaChangeHistoryGuid = DataObject.getValue<Guid>(row["SchemaChangeDetailGuid"], Guid.Empty);

            long indexValue = DataObject.getValue<long>(row["SchemaObjectTypeIndex"], (long)SCHEMAOBJECTTYPE.None);
            SCHEMAOBJECTTYPE schemaObjectType = SCHEMAOBJECTTYPE.None;

            if (Enum.TryParse(indexValue.ToString(CultureInfo.InvariantCulture), true, out schemaObjectType))
            {
                dataObject.SchemaObjectTypeIndex = schemaObjectType;
            }

            dataObject.SchemaName = DataObject.getString(row["SchemaName"]);
            dataObject.ObjectName = DataObject.getString(row["ObjectName"]);
            dataObject.CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
            dataObject.CreatedBy = DataObject.getString(row["CreatedBy"]);
            dataObject.UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], DateTimeOffset.Now);
            dataObject.UpdatedBy = DataObject.getString(row["UpdatedBy"]);

            return dataObject;
        }
        #endregion Private Support Methods
    }
}
