// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VersionDBI.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for VersionDBI.
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
    /// Summary description for VersionDBI.
    /// </summary>
    public class VersionDBI : SyncDBI
    {
        #region Attributes
        #endregion Attributes

        public VersionDBI(string user)
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
        public List<VersionDO> GetList(SecurityClass security)
        {
            List<VersionDO> versions = new List<VersionDO>();

            DataSet ds = this.Load(security, null);

            if (ds.Tables[0].Rows.Count == 0)
            {
                return versions;
            }

            versions.AddRange(from DataRow row in ds.Tables[0].Rows select this.GetDataObjectFromDataRow(row));

            return versions;
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
        /// The <see cref="VersionDO"/>.
        /// </returns>
        public VersionDO Get(SecurityClass security, Guid? identityGuid)
        {
            DataSet ds = this.Load(security, identityGuid);

            if (ds.Tables[0].Rows.Count == 0)
            {
                return null;
            }

            DataRow row = ds.Tables[0].Rows[0];
            VersionDO version = this.GetDataObjectFromDataRow(row);

            return version;
        }

        /// <summary>
        /// The get active session list.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        /// </returns>
        public VersionDO GetCurrentVersion(SecurityClass security)
        {
            DataSet ds = this.LoadCurrentVersion(security);

            if (ds.Tables[0].Rows.Count == 0)
            {
                return null;
            }

            DataRow row = ds.Tables[0].Rows[0];
            VersionDO version = this.GetDataObjectFromDataRow(row);

            return version;
        }

        /// <summary>
        /// Saves the passed in <seealso cref="VersionDO"/> record.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="dataObject">The synchronization session information to persist.</param>
        /// <returns>True if the warning level for drawdown is hit.  Otherwise, false</returns>
        /// <remarks>Utilizes an update stored procedure and does not provide the ability to create a new version record.</remarks>
        public bool Save(SecurityClass security, VersionDO dataObject)
        {
            // Save the dataobject using a merge implementation
            this.Update(security, dataObject);

            return true;
        }

        /// <summary>
        /// Deletes the passed in <seealso cref="VersionDO"/> record.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="dataObject">The synchronization session information to delete.</param>
        /// <param name="purge">Permanently delete the specified synchronization session.</param>
        /// <returns>True if the warning level for drawdown is hit.  Otherwise, false</returns>
        public bool Delete(SecurityClass security, VersionDO dataObject, bool purge)
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
        /// Returns a DataSet containing the Version record containing the specified Primary Key.
        /// </summary>
        /// <param name="security">
        /// Contains security credentials
        /// </param>
        /// <param name="identityGuid">
        /// Primary key value for the Version record to load.
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

                ds = ConsolidatedDA.GetDataSet(cmd, security);
            }

            return ds;
        }

        /// <summary>
        /// Returns a DataSet containing all Version records that with a Started Date and no End Date
        /// </summary>
        /// <param name="security">
        /// Contains security credentials
        /// </param>
        /// <returns>
        /// Returns a <see cref="DataSet"/> populated with the matching Versions.
        /// </returns>
        private DataSet LoadCurrentVersion(SecurityClass security)
        {
            DataSet ds = null;

            using (var cmd = this.PrepareSelectCurrentVersionStatement())
            {
                ds = ConsolidatedDA.GetDataSet(cmd, security);
            }

            return ds;
        }

        /// <summary>
        /// Updates a limited number of columns in the current <seealso cref="VersionDO"/> record with an existing record.
        /// This class does not allow programmatic creation of a new version record.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="dataObject">The synchronization session information to update.</param>
        private void Update(SecurityClass security, VersionDO dataObject)
        {
            using (var cmd = this.PrepareUpsertStatement())
            {
                SqlParameterCollection parms = cmd.Parameters;
                parms["@IdentityGuid"].Value = dataObject.IdentityGuid;
                parms["@SyncCompletedFlag"].Value = dataObject.SyncCompletedFlag;
                parms["@RowVersionSnapshot"].Value = this.SetOptionalValue<byte[]>(dataObject.RowVersionSnapshot);
                parms["@UpdatedBy"].Value = security.UserID;

                ConsolidatedDA.ExecuteQuery(security, cmd);
            }
        }

        /// <summary>
        /// Removes the specified <seealso cref="VersionDO"/> record.
        /// </summary>
        /// <param name="security">Contains security credentials</param>
        /// <param name="dataObject">The synchronization session information to persist.</param>
        private void Delete(SecurityClass security, VersionDO dataObject)
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
        /// Create a command object bound to an update stored procedure and parameters.  The framework typically performs an insert/update, but this class does not provide
        /// insert capabilities.
        /// </summary>
        /// <returns>
        /// An instance of a <see cref="SqlCommand"/> object bound to the appropriate stored procedure and with the appropriate parameters.
        /// </returns>
        protected override SqlCommand PrepareUpsertStatement()
        {
            return this.CreateUpdateStatement();
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

            cmd.CommandText = "dbo.usp_VersionSelect";
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
            throw new NotImplementedException("Automated creation of a version record is not permitted.");
        }

        /// <summary>
        /// Create a command object bound to an update stored procedure and parameters.
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
            return this.CreateUpdateStatement();
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

            cmd.CommandText = "dbo.usp_VersionDeleteByGuid";
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
        private SqlCommand PrepareSelectCurrentVersionStatement()
        {
            var cmd = new SqlCommand();

            cmd.CommandText = "dbo.usp_VersionSelectCurrent";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();

            return cmd;
        }

        /// <summary>
        /// The create a restricted update statement.
        /// </summary>
        /// <returns>
        /// The <see cref="SqlCommand"/>.
        /// </returns>
        private SqlCommand CreateUpdateStatement()
        {
            var cmd = new SqlCommand();

            cmd.CommandText = "dbo.usp_VersionUpdate";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();

            cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@SyncCompletedFlag", SqlDbType.Bit);
            cmd.Parameters.Add("@RowVersionSnapshot", SqlDbType.VarBinary, 8);
            cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);

            return cmd;
        }

        /// <summary>
        /// The get data object from data row.
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        /// <returns>
        /// The <see cref="VersionDO"/>.
        /// </returns>
        private VersionDO GetDataObjectFromDataRow(DataRow row)
        {
            VersionDO dataObject = new VersionDO();

            dataObject.IdentityGuid = DataObject.getValue<Guid>(row["VersionGuid"], Guid.Empty);
            dataObject.VersionIndex = DataObject.getOptionalInt(row["VersionIndex"]);
            dataObject.Version = DataObject.getString(row["Version"]);
            dataObject.PackageName = DataObject.getString(row["PackageName"]);
            dataObject.DateApplied = DataObject.getOptionalDateTimeOffset(row["DateApplied"]);
            dataObject.Comments = DataObject.getString(row["Comments"]);
            dataObject.Check1 = DataObject.getLong(row["Check1"]);
            dataObject.Check2 = DataObject.getLong(row["Check2"]);
            dataObject.SyncCompletedFlag = DataObject.getValue<bool>(row["SyncCompletedFlag"], false);
            dataObject.RowVersionSnapshot = DataObject.getOptionalVarBinary(row["RowVersionSnapshot"]);
            dataObject.RowVersion = DataObject.getOptionalVarBinary(row["_RowVersion"]);

            var createdDate = DataObject.getValue<DateTime>(row["CreatedDate"], DateTime.Now);
            var updatedDate = DataObject.getValue<DateTime>(row["UpdatedDate"], DateTime.Now);

            dataObject.CreatedDate = new DateTimeOffset(createdDate);
            dataObject.CreatedBy = DataObject.getString(row["CreatedBy"]);
            dataObject.UpdatedDate = new DateTimeOffset(updatedDate);
            dataObject.UpdatedBy = DataObject.getString(row["UpdatedBy"]);

            return dataObject;
        }
        #endregion Private Support Methods
    }
}
